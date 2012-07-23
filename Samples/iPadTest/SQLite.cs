//
// Copyright (c) 2009-2010 Krueger Systems, Inc.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

//Original source is located at http://code.google.com/p/sqlite-net/
//Forked by Joe Feser @joefeser
//The main purpose is to remove the bitness issue of sqlite allowing for "AnyCPU"
//The second purpose is to modify the insert to allow for a nX speed increase.

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SQLite {
    public class SQLiteException : System.Exception {
        public SQLite3.Result Result {
            get;
            private set;
        }
        protected SQLiteException(SQLite3.Result r, string message)
            : base(message) {
            Result = r;
        }
        public static SQLiteException New(SQLite3.Result r, string message) {
            return new SQLiteException(r, message);
        }
    }

    /// <summary>
    /// Represents an open connection to a SQLite database.
    /// </summary>
    public class SQLiteConnection : IDisposable {
        private bool _open;
        private TimeSpan _busyTimeout;
        private Dictionary<string, TableMapping> _mappings = null;
        private Dictionary<string, TableMapping> _tables = null;

        private System.Diagnostics.Stopwatch _sw;
        private long _elapsedMilliseconds = 0;

        public IntPtr Handle {
            get;
            private set;
        }
        public string DatabasePath {
            get;
            private set;
        }

        public bool TimeExecution {
            get;
            set;
        }
        public bool Trace {
            get;
            set;
        }

        /// <summary>
        /// Constructs a new SQLiteConnection and opens a SQLite database specified by databasePath.
        /// </summary>
        /// <param name="databasePath">
        /// Specifies the path to the database file.
        /// </param>
        public SQLiteConnection(string databasePath) {
            DatabasePath = databasePath;
            IntPtr handle;
            var r = SQLite3.Open(DatabasePath, out handle);
            Handle = handle;
            if (r != SQLite3.Result.OK) {
                throw SQLiteException.New(r, "Could not open database file: " + DatabasePath);
            }
            _open = true;

            BusyTimeout = TimeSpan.FromSeconds(0.1);
        }

        /// <summary>
        /// Sets a busy handler to sleep the specified amount of time when a table is locked.
        /// The handler will sleep multiple times until a total time of <see cref="BusyTimeout"/> has accumulated.
        /// </summary>
        public TimeSpan BusyTimeout {
            get {
                return _busyTimeout;
            }
            set {
                _busyTimeout = value;
                if (Handle != IntPtr.Zero) {
                    SQLite3.BusyTimeout(Handle, (int)_busyTimeout.TotalMilliseconds);
                }
            }
        }

        /// <summary>
        /// Returns the mappings from types to tables that the connection
        /// currently understands.
        /// </summary>
        public IEnumerable<TableMapping> TableMappings {
            get {
                if (_tables == null) {
                    return Enumerable.Empty<TableMapping>();
                }
                else {
                    return _tables.Values;
                }
            }
        }

        /// <summary>
        /// Retrieves the mapping that is automatically generated for the given type.
        /// </summary>
        /// <param name="type">
        /// The type whose mapping to the database is returned.
        /// </param>
        /// <param name="ignoreColumns">The columns to ignore</param>
        /// <returns>
        /// The mapping represents the schema of the columns of the database and contains 
        /// methods to set and get properties of objects.
        /// </returns>
        public TableMapping GetMapping(Type type, TableNameAttribute overrideTableName, IEnumerable<string> ignoreColumns) {
            if (_mappings == null) {
                _mappings = new Dictionary<string, TableMapping>();
            }

            ignoreColumns = ignoreColumns ?? _defaultIgnore;

            TableMapping map;
            if (!_mappings.TryGetValue(type.FullName, out map)) {
                map = new TableMapping(type, overrideTableName, ignoreColumns);
                _mappings[type.FullName] = map;
            }
            return map;
        }

        /// <summary>
        /// Executes a "create table if not exists" on the database. It also
        /// creates any specified indexes on the columns of the table. It uses
        /// a schema automatically generated from the specified type. You can
        /// later access this schema by calling GetMapping.
        /// </summary>
        /// <returns>
        /// The number of entries added to the database schema.
        /// </returns>
        public int CreateTable<T>() {
            return CreateTable<T>(null);
        }

        private static List<string> _defaultIgnore = new List<string>() { "EntityKey", "EntityState" }; //EF columns that are never part of the table structure.

        /// <summary>
        /// Executes a "create table if not exists" on the database. It also
        /// creates any specified indexes on the columns of the table. It uses
        /// a schema automatically generated from the specified type. You can
        /// later access this schema by calling GetMapping.
        /// </summary>
        /// <param name="ignoreColumns">The columns to ignore</param>
        /// <returns>
        /// The number of entries added to the database schema.
        /// </returns>
        public int CreateTable<T>(IEnumerable<string> ignoreColumns) {
            return CreateTable<T>(default(TableNameAttribute), default(IEnumerable<string>));
        }

        /// <summary>
        /// Executes a "create table if not exists" on the database. It also
        /// creates any specified indexes on the columns of the table. It uses
        /// a schema automatically generated from the specified type. You can
        /// later access this schema by calling GetMapping.
        /// </summary>
        /// <param name="overrideTableName">Table Name</param>
        /// <param name="ignoreColumns">The columns to ignore</param>
        /// <returns>
        /// The number of entries added to the database schema.
        /// </returns>
        public int CreateTable<T>(TableNameAttribute overrideTableName, IEnumerable<string> ignoreColumns) {
            var ty = typeof(T);

            if (_tables == null) {
                _tables = new Dictionary<string, TableMapping>();
            }

            ignoreColumns = ignoreColumns ?? _defaultIgnore;

            TableMapping map;
            if (!_tables.TryGetValue(ty.FullName, out map)) {
                map = GetMapping(ty, overrideTableName, ignoreColumns);
                _tables.Add(ty.FullName, map);
            }
            var query = "create table if not exists \"" + map.TableName + "\"(\n";

            var decls = map.Columns.Select(p => Orm.SqlDecl(p));
            var decl = string.Join(",\n", decls.ToArray());
            query += decl;
            query += ")";

            var count = Execute(query);

            if (count == 0) { //Possible bug: This always seems to return 0?
                // Table already exists, migrate it
                MigrateTable(map);
            }

            foreach (var p in map.Columns.Where(x => x.IsIndexed)) {
                var indexName = map.TableName + "_" + p.Name;
                var q = string.Format("create index if not exists \"{0}\" on \"{1}\"(\"{2}\")", indexName, map.TableName, p.Name);
                count += Execute(q);
            }

            return count;
        }

        class TableInfo {
            public int cid {
                get;
                set;
            }
            public string name {
                get;

                set;
            }
            public string type {
                get;
                set;
            }
            public int notnull {
                get;
                set;
            }
            public string dflt_value {
                get;
                set;
            }
            public int pk {
                get;
                set;
            }
        }

        void MigrateTable(TableMapping map) {
            var query = "pragma table_info(\"" + map.TableName + "\")";

            var existingCols = Query<TableInfo>(query);

            var toBeAdded = new List<TableMapping.Column>();

            foreach (var p in map.Columns) {
                var found = false;
                foreach (var c in existingCols) {
                    found = p.Name == c.name;
                    if (found)
                        break;
                }
                if (!found) {
                    toBeAdded.Add(p);
                }
            }

            foreach (var p in toBeAdded) {
                var addCol = "alter table \"" + map.TableName + "\" add column " + Orm.SqlDecl(p);
                Execute(addCol);
            }
        }

        /// <summary>
        /// Creates a new SQLiteCommand given the command text with arguments. Place a '?'
        /// in the command text for each of the arguments.
        /// </summary>
        /// <param name="cmdText">
        /// The fully escaped SQL.
        /// </param>
        /// <param name="args">
        /// Arguments to substitute for the occurences of '?' in the command text.
        /// </param>
        /// <returns>
        /// A <see cref="SQLiteCommand"/>
        /// </returns>
        public SQLiteCommand CreateCommand(string cmdText, params object[] ps) {
            if (!_open) {
                throw SQLiteException.New(SQLite3.Result.Error, "Cannot create commands from unopened database");
            }
            else {
                var cmd = new SQLiteCommand(this);
                cmd.CommandText = cmdText;
                foreach (var o in ps) {
                    cmd.Bind(o);
                }
                return cmd;
            }
        }

        /// <summary>
        /// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
        /// in the command text for each of the arguments and then executes that command.
        /// Use this method instead of Query when you don't expect rows back. Such cases include
        /// INSERTs, UPDATEs, and DELETEs.
        /// You can set the Trace or TimeExecution properties of the connection
        /// to profile execution.
        /// </summary>
        /// <param name="query">
        /// The fully escaped SQL.
        /// </param>
        /// <param name="args">
        /// Arguments to substitute for the occurences of '?' in the query.
        /// </param>
        /// <returns>
        /// The number of rows modified in the database as a result of this execution.
        /// </returns>
        public int Execute(string query, params object[] args) {
            var cmd = CreateCommand(query, args);
            return Execute(cmd);
        }

        /// <summary>
        /// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
        /// in the command text for each of the arguments and then executes that command.
        /// Use this method instead of Query when you don't expect rows back. Such cases include
        /// INSERTs, UPDATEs, and DELETEs.
        /// You can set the Trace or TimeExecution properties of the connection
        /// to profile execution.
        /// </summary>
        /// <param name="cmd">
        /// The fully prepared command
        /// </param>
        /// <returns>
        /// The number of rows modified in the database as a result of this execution.
        /// </returns>
        public int Execute(SQLiteCommand cmd) {

            if (TimeExecution) {
                if (_sw == null) {
                    _sw = new System.Diagnostics.Stopwatch();
                }
                _sw.Reset();
                _sw.Start();
            }

            int r = cmd.ExecuteNonQuery();

            if (TimeExecution) {
                _sw.Stop();
                _elapsedMilliseconds += _sw.ElapsedMilliseconds;
                Console.WriteLine("Finished in {0} ms ({1:0.0} s total)", _sw.ElapsedMilliseconds, _elapsedMilliseconds / 1000.0);
            }

            return r;
        }

        /// <summary>
        /// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
        /// in the command text for each of the arguments and then executes that command.
        /// It returns each row of the result using the mapping automatically generated for
        /// the given type.
        /// </summary>
        /// <param name="query">
        /// The fully escaped SQL.
        /// </param>
        /// <param name="args">
        /// Arguments to substitute for the occurences of '?' in the query.
        /// </param>
        /// <returns>
        /// An enumerable with one result for each row returned by the query.
        /// </returns>
        public List<T> Query<T>(string query, params object[] args) where T : new() {
            var cmd = CreateCommand(query, args);
            return cmd.ExecuteQuery<T>();
        }

        /// <summary>
        /// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
        /// in the command text for each of the arguments and then executes that command.
        /// It returns each row of the result using the specified mapping. This function is
        /// only used by libraries in order to query the database via introspection. It is
        /// normally not used.
        /// </summary>
        /// <param name="map">
        /// A <see cref="TableMapping"/> to use to convert the resulting rows
        /// into objects.
        /// </param>
        /// <param name="query">
        /// The fully escaped SQL.
        /// </param>
        /// <param name="args">
        /// Arguments to substitute for the occurences of '?' in the query.
        /// </param>
        /// <returns>
        /// An enumerable with one result for each row returned by the query.
        /// </returns>
        public List<object> Query(TableMapping map, string query, params object[] args) {
            var cmd = CreateCommand(query, args);
            return cmd.ExecuteQuery<object>(map);
        }

        /// <summary>
        /// Returns a queryable interface to the table represented by the given type.
        /// </summary>
        /// <returns>
        /// A queryable object that is able to translate Where, OrderBy, and Take
        /// queries into native SQL.
        /// </returns>
        public TableQuery<T> Table<T>() where T : new() {
            return new TableQuery<T>(this);
        }

        /// <summary>
        /// Attempts to retrieve an object with the given primary key from the table
        /// associated with the specified type. Use of this method requires that
        /// the given type have a designated PrimaryKey (using the PrimaryKeyAttribute).
        /// </summary>
        /// <param name="pk">
        /// The primary key.
        /// </param>
        /// <returns>
        /// The object with the given primary key. Throws a not found exception
        /// if the object is not found.
        /// </returns>
        public T Get<T>(object pk) where T : new() {
            var map = GetMapping(typeof(T), default(TableNameAttribute), _defaultIgnore);
            string query = string.Format("select * from \"{0}\" where \"{1}\" = ?", map.TableName, map.PK.Name);
            return Query<T>(query, pk).First();
        }

        /// <summary>
        /// Whether <see cref="BeginTransaction"/> has been called and the database is waiting for a <see cref="Commit"/>.
        /// </summary>
        public bool IsInTransaction {
            get;
            private set;
        }

        /// <summary>
        /// Begins a new transaction. Call <see cref="Commit"/> to end the transaction.
        /// </summary>
        public void BeginTransaction() {
            if (!IsInTransaction) {
                Execute("begin transaction");
                IsInTransaction = true;
            }
        }

        /// <summary>
        /// Rolls back the transaction that was begun by <see cref="BeginTransaction"/>.
        /// </summary>
        public void Rollback() {
            if (IsInTransaction) {
                Execute("rollback");
                IsInTransaction = false;
            }
        }

        /// <summary>
        /// Commits the transaction that was begun by <see cref="BeginTransaction"/>.
        /// </summary>
        public void Commit() {
            if (IsInTransaction) {
                Execute("commit");
                IsInTransaction = false;
            }
        }

        /// <summary>
        /// Executes <param name="action"> within a transaction and automatically rollsback the transaction
        /// if an exception occurs. The exception is rethrown.
        /// </summary>
        /// <param name="action">
        /// The <see cref="Action"/> to perform within a transaction. <param name="action"> can contain any number
        /// of operations on the connection but should never call <see cref="BeginTransaction"/>,
        /// <see cref="Rollback"/>, or <see cref="Commit"/>.
        /// </param>
        public void RunInTransaction(Action action) {
            if (IsInTransaction) {
                throw new InvalidOperationException("The connection must not already be in a transaction when RunInTransaction is called");
            }
            try {
                BeginTransaction();
                action();
                Commit();
            }
            catch (Exception) {
                Rollback();
                throw;
            }
        }

        /// <summary>
        /// Inserts all specified objects.
        /// </summary>
        /// <param name="objects">
        /// An <see cref="IEnumerable"/> of the objects to insert.
        /// </param>
        /// <returns>
        /// The number of rows added to the table.
        /// </returns>
        public int InsertAll(System.Collections.IEnumerable objects) {

            //do not start a new transaction if one already exists.
            var inTransaction = IsInTransaction;

            if (!inTransaction) {
                BeginTransaction();
            }

            var c = 0;
            foreach (var r in objects) {
                c += Insert(r);
            }

            if (!inTransaction) {
                Commit();
            }

            return c;
        }

        /// <summary>
        /// Inserts the given object and retrieves its
        /// auto incremented primary key if it has one.
        /// </summary>
        /// <param name="obj">
        /// The object to insert.
        /// </param>
        /// <returns>
        /// The number of rows added to the table.
        /// </returns>
        public int Insert(object obj) {
            if (obj == null) {
                return 0;
            }
            return Insert(obj, "", obj.GetType());
        }
        public int Insert(object obj, Type objType) {
            return Insert(obj, "", objType);
        }
        public int Insert(object obj, string extra) {
            if (obj == null) {
                return 0;
            }
            return Insert(obj, extra, obj.GetType());
        }

        /// <summary>
        /// Inserts the given object and retrieves its
        /// auto incremented primary key if it has one.
        /// </summary>
        /// <param name="obj">
        /// The object to insert.
        /// </param>
        /// <param name="extra">
        /// Literal SQL code that gets placed into the command. INSERT {extra} INTO ...
        /// </param>
        /// <returns>
        /// The number of rows added to the table.
        /// </returns>
        public int Insert(object obj, string extra, Type objType) {
            if (obj == null || objType == null) {
                return 0;
            }

            var map = GetMapping(objType, default(TableNameAttribute), _defaultIgnore);

            var cols = map.InsertColumns;
            var vals = new object[cols.Length];
            for (var i = 0; i < vals.Length; i++) {
                vals[i] = cols[i].GetValue(obj);
            }

            var insertCmd = map.GetInsertCommand(this, extra);
            var count = insertCmd.ExecuteNonQuery(vals);
            //var count = Execute(map.InsertSql(extra), vals);
            if (map.HasAutoIncPK) {
                var id = SQLite3.LastInsertRowid(Handle);
                map.SetAutoIncPK(obj, id);
            }
            return count;
        }

        /// <summary>
        /// Updates all of the columns of a table using the specified object
        /// except for its primary key.
        /// The object is required to have a primary key.
        /// </summary>
        /// <param name="obj">
        /// The object to update. It must have a primary key designated using the PrimaryKeyAttribute.
        /// </param>
        /// <returns>
        /// The number of rows updated.
        /// </returns>
        public int Update(object obj) {
            if (obj == null) {
                return 0;
            }
            return Update(obj, obj.GetType());
        }

        public int Update(object obj, Type objType) {
            if (obj == null || objType == null) {
                return 0;
            }

            var map = GetMapping(objType, default(TableNameAttribute), _defaultIgnore);

            var pk = map.PK;

            if (pk == null) {
                throw new NotSupportedException("Cannot update " + map.TableName + ": it has no PK");
            }

            var cols = from p in map.Columns
                       where p != pk
                       select p;
            var vals = from c in cols
                       select c.GetValue(obj);
            var ps = new List<object>(vals);
            ps.Add(pk.GetValue(obj));
            var q = string.Format("update \"{0}\" set {1} where {2} = ? ", map.TableName, string.Join(",", (from c in cols
                                                                                                            select "\"" + c.Name + "\" = ? ").ToArray()), pk.Name);
            return Execute(q, ps.ToArray());
        }

        /// <summary>
        /// Deletes the given object from the database using its primary key.
        /// </summary>
        /// <param name="obj">
        /// The object to delete. It must have a primary key designated using the PrimaryKeyAttribute.
        /// </param>
        /// <returns>
        /// The number of rows deleted.
        /// </returns>
        public int Delete<T>(T obj) {
            var map = GetMapping(obj.GetType(), default(TableNameAttribute), _defaultIgnore);
            var pk = map.PK;
            if (pk == null) {
                throw new NotSupportedException("Cannot delete " + map.TableName + ": it has no PK");
            }
            var q = string.Format("delete from \"{0}\" where \"{1}\" = ?", map.TableName, pk.Name);
            return Execute(q, pk.GetValue(obj));
        }

        public void Close() {
            if (_open && Handle != IntPtr.Zero) {
                SQLite3.Close(Handle);
                Handle = IntPtr.Zero;
                _open = false;
            }
        }

        public void Dispose() {
            Close();
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if (disposing) {
                if (_mappings != null) {
                    foreach (var item in _mappings) {
                        item.Value.Dispose();
                    }
                }
                if (_tables != null) {
                    foreach (var item in _tables) {
                        item.Value.Dispose();
                    }
                }
            }
        }

        ~SQLiteConnection() {
            Dispose(false);
        }
    }

    public class TableNameAttribute : Attribute {
        public string Name {
            get;
            set;
        }
        public TableNameAttribute() {

        }
        public TableNameAttribute(string name) {
            this.Name = name;
        }
    }

    public class PrimaryKeyAttribute : Attribute {
    }

    public class AutoIncrementAttribute : Attribute {
    }

    public class IndexedAttribute : Attribute {
    }

    public class IgnoreAttribute : Attribute {
    }

    public class MaxLengthAttribute : Attribute {
        public int Value {
            get;
            private set;
        }
        public MaxLengthAttribute(int length) {
            Value = length;
        }
    }

    public class CollationAttribute : Attribute {
        public string Value {
            get;
            private set;
        }

        public CollationAttribute(string collation) {
            Value = collation;
        }
    }

    public class TableMapping : IDisposable {

        public Type MappedType {
            get;
            private set;
        }

        public string TableName {
            get;
            private set;
        }

        public Column[] Columns {
            get;
            private set;
        }

        public Column PK {
            get;
            private set;
        }

        Column _autoPk = null;
        Column[] _insertColumns = null;

        string _insertSql = null;

        public TableMapping(Type type, TableNameAttribute overrideTableName, IEnumerable<string> ignoreColumns) {
            MappedType = type;
            TableName = MappedType.Name;

            //see if we have a TableName Attribute
            var tableAttribute = type.GetCustomAttributes(typeof(TableNameAttribute), true).FirstOrDefault() as TableNameAttribute;
            if (tableAttribute != null) {
                TableName = tableAttribute.Name;
            }
            if (overrideTableName != null) {
                TableName = overrideTableName.Name;
            }

            var props = MappedType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
            var cols = new List<Column>();
            foreach (var p in props) {
                var ignore = p.GetCustomAttributes(typeof(IgnoreAttribute), true).Length > 0;

                if (!ignore && ignoreColumns.FirstOrDefault(item => item.Equals(p.Name, StringComparison.OrdinalIgnoreCase)) != null) {
                    ignore = true;
                }

                //this code is used to support using read only properties for saving to the database.
                var validateType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;

                //make sure the property is not an enumerable or list
                //string implements IEnumerable for chars so ignore that.
                if (!ignore && (!validateType.IsValueType && validateType != typeof(string) && validateType != typeof(byte[]))) {
                    ignore = true;
                }

                if (!ignore) { //p.CanWrite  we understand read only items.
                    var gt = typeof(PropColumn<,>).MakeGenericType(type, p.PropertyType);
                    Column newColumn = Activator.CreateInstance(gt, p) as Column;
                    cols.Add(newColumn);
                }
            }
            Columns = cols.ToArray();
            foreach (var c in Columns) {
                if (c.IsAutoInc && c.IsPK) {
                    _autoPk = c;
                }
                if (c.IsPK) {
                    PK = c;
                }
            }
            HasAutoIncPK = _autoPk != null;
        }

        public bool HasAutoIncPK {
            get;
            private set;
        }

        public void SetAutoIncPK(object obj, long id) {
            if (_autoPk != null) {
                _autoPk.SetValue(obj, Convert.ChangeType(id, _autoPk.ColumnType));
            }
        }

        public Column[] InsertColumns {
            get {
                if (_insertColumns == null) {
                    _insertColumns = Columns.Where(c => !c.IsAutoInc).ToArray();
                }
                return _insertColumns;
            }
        }

        public Column FindColumn(string name) {
            var exact = Columns.Where(c => c.Name == name).FirstOrDefault();
            return exact;
        }

        public string InsertSql(string extra) {
            if (_insertSql == null) {
                var cols = InsertColumns;
                _insertSql = string.Format("insert {3} into \"{0}\"({1}) values ({2})", TableName, string.Join(",", (from c in cols
                                                                                                                     select "\"" + c.Name + "\"").ToArray()), string.Join(",", (from c in cols
                                                                                                                                                                                select "?").ToArray()), extra);
            }
            return _insertSql;
        }

        PreparedSqlLiteInsertCommand _insertCommand;
        string _insertCommandExtra = null;

        public PreparedSqlLiteInsertCommand GetInsertCommand(SQLiteConnection conn, string extra) {
            if (_insertCommand == null || _insertCommandExtra != extra) {
                var insertSql = InsertSql(extra);
                _insertCommand = new PreparedSqlLiteInsertCommand(conn);
                _insertCommand.CommandText = insertSql;
                _insertCommandExtra = extra;
            }
            return _insertCommand;
        }

        public abstract class Column {
            public string Name {
                get;
                protected set;
            }

            public Type ColumnType {
                get;
                protected set;
            }

            public string Collation {
                get;
                protected set;
            }

            public bool IsAutoInc {
                get;
                protected set;
            }

            public bool IsPK {
                get;
                protected set;
            }

            public bool IsIndexed {
                get;
                protected set;
            }

            public bool IsNullable {
                get;
                protected set;
            }

            public int MaxStringLength {
                get;
                protected set;
            }

            public bool ReadOnly {
                get;
                set;
            }

            public abstract void SetValue(object obj, object val);

            public abstract object GetValue(object obj);

        }

        /// <summary>
        /// Create a column with the correct type and property type.
        /// </summary>
        /// <typeparam name="T">object type</typeparam>
        /// <typeparam name="P">property type</typeparam>
        public class PropColumn<T, P> : Column {

            //we want a delegate to obtain the value for speed reasons

            Func<T, object> _get = null;
            Action<T, P> _set = null;

            PropertyInfo _prop;

            public PropColumn(PropertyInfo prop) {
                _prop = prop;
                Name = prop.Name;
                //If this type is Nullable<T> then Nullable.GetUnderlyingType returns the T, otherwise it returns null, so get the the actual type instead
                ColumnType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                Collation = Orm.Collation(prop);
                IsAutoInc = Orm.IsAutoInc(prop);
                IsPK = Orm.IsPK(prop);
                IsIndexed = Orm.IsIndexed(prop);
                IsNullable = !IsPK;
                ReadOnly = !prop.CanWrite;

                //if we are not nullable and not a string, then we are not nullable
                if (IsNullable && Nullable.GetUnderlyingType(prop.PropertyType) == null
                        && ColumnType != typeof(string) && ColumnType != typeof(byte[])) {
                    IsNullable = false;
                }

                MaxStringLength = Orm.MaxStringLength(prop);
                _get = DelegateHelper.BuildGetDelegateExpression<T>(prop);
                _set = DelegateHelper.BuildSetDelegateExpression<T, P>(prop);
            }

            public override void SetValue(object obj, object val) {
                _set((T)obj, (P)val);
            }

            public override object GetValue(object obj) {
                return _get((T)obj);
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if (disposing) {
                if (_insertCommand != null) {
                    _insertCommand.Dispose();
                    _insertCommand = null;
                }
            }
        }

        ~TableMapping() {
            Dispose(false);
        }
    }

    public static class Orm {
        public const int DefaultMaxStringLength = 140;

        public static string SqlDecl(TableMapping.Column p) {

            string decl = "\"" + p.Name + "\" " + SqlType(p) + " ";

            if (p.IsPK) {
                decl += "primary key ";
            }
            if (p.IsAutoInc) {
                decl += "autoincrement ";
            }
            if (!p.IsNullable) {
                decl += "not null ";
            }
            if (!string.IsNullOrEmpty(p.Collation)) {
                decl += "collate " + p.Collation + " ";
            }

            return decl;
        }

        public static string SqlType(TableMapping.Column p) {
            var clrType = p.ColumnType;

            if (p.IsAutoInc) {
                return "integer"; //required for auto number columns
            }
            if (clrType == typeof(Boolean)) {
                return "bool";
            }
            if (clrType == typeof(Byte)) {
                return "byte";
            }
            if (clrType == typeof(UInt16)) {
                return "uint16";
            }
            if (clrType == typeof(SByte)) {
                return "sbyte";
            }
            if (clrType == typeof(Int16)) {
                return "short";
            }
            if (clrType == typeof(Int32)) {
                return "int";
            }
            else if (clrType == typeof(UInt32)) {
                return "uint32";
            }
            else if (clrType == typeof(Int64)) {
                return "integer"; //was long
            }
            else if (clrType == typeof(Single)) {
                return "single";
            }
            else if (clrType == typeof(Double)) {
                return "double";
            }
            else if (clrType == typeof(Decimal)) {
                return "decimal";
            }
            else if (clrType == typeof(String)) {
                int len = p.MaxStringLength;
                return "varchar(" + len + ")";
            }
            else if (clrType == typeof(DateTime)) {
                return "datetime";
            }
            else if (clrType.IsEnum) {
                return "integer";
            }
            else if (clrType == typeof(byte[])) {
                return "byte";
            }
            else if (clrType == typeof(Guid)) {
                return "Guid";
            }
            else {
                throw new NotSupportedException("Don't know about " + clrType);
            }
        }

        public static bool IsPK(MemberInfo p) {
            var attrs = p.GetCustomAttributes(typeof(PrimaryKeyAttribute), true);
            return attrs.Length > 0;
        }

        public static string Collation(MemberInfo p) {
            var attrs = p.GetCustomAttributes(typeof(CollationAttribute), true);
            if (attrs.Length > 0) {
                return ((CollationAttribute)attrs[0]).Value;
            }
            else {
                return string.Empty;
            }
        }
        public static bool IsAutoInc(MemberInfo p) {
            var attrs = p.GetCustomAttributes(typeof(AutoIncrementAttribute), true);
            return attrs.Length > 0;
        }

        public static bool IsIndexed(MemberInfo p) {
            var attrs = p.GetCustomAttributes(typeof(IndexedAttribute), true);
            return attrs.Length > 0;
        }

        public static int MaxStringLength(PropertyInfo p) {
            var attrs = p.GetCustomAttributes(typeof(MaxLengthAttribute), true);
            if (attrs.Length > 0) {
                return ((MaxLengthAttribute)attrs[0]).Value;
            }
            else {
                return DefaultMaxStringLength;
            }
        }
    }

    public class SQLiteCommand {

        private List<IBinding> _bindings;

        protected SQLiteConnection Connection {
            get;
            set;
        }

        public string CommandText {
            get;
            set;
        }

        internal SQLiteCommand(SQLiteConnection conn) {
            Connection = conn;
            _bindings = new List<IBinding>();
            CommandText = "";
        }

        public virtual int ExecuteNonQuery() {
            if (Connection.Trace) {
                Console.WriteLine("Executing: " + this);
            }

            var r = SQLite3.Result.OK;
            var stmt = Prepare();
            r = SQLite3.Step(stmt);
            Finalize(stmt);
            if (r == SQLite3.Result.Done) {
                int rowsAffected = SQLite3.Changes(Connection.Handle);
                return rowsAffected;
            }
            else if (r == SQLite3.Result.Error) {
                string msg = SQLite3.GetErrmsg(Connection.Handle);
                throw SQLiteException.New(r, msg);
            }
            else if (r == SQLite3.Result.Row) {
                if (CommandText.IndexOf("Pragma", StringComparison.OrdinalIgnoreCase) == -1) {
                    throw SQLiteException.New(r, r.ToString());
                }
                return 0;
            }
            else {
                throw SQLiteException.New(r, r.ToString());
            }
        }

        public virtual List<T> ExecuteQuery<T>() where T : new() {
            return ExecuteQuery<T>(Connection.GetMapping(typeof(T), default(TableNameAttribute), null));
        }

        public virtual List<T> ExecuteQuery<T>(TableMapping map) {
            if (Connection.Trace) {
                Console.WriteLine("Executing Query: " + this);
            }

            var r = new List<T>();

            var stmt = Prepare();

            var cols = new TableMapping.Column[SQLite3.ColumnCount(stmt)];

            for (int i = 0; i < cols.Length; i++) {
                var name = Marshal.PtrToStringUni(SQLite3.ColumnName16(stmt, i));
                cols[i] = map.FindColumn(name);
            }

            while (SQLite3.Step(stmt) == SQLite3.Result.Row) {
                var obj = Activator.CreateInstance(map.MappedType);
                for (int i = 0; i < cols.Length; i++) {
                    var currentCol = cols[i];
                    //do not try to set a read only column
                    if (currentCol == null || currentCol.ReadOnly)
                        continue;
                    var colType = SQLite3.ColumnType(stmt, i);
                    var val = ReadCol(stmt, i, colType, currentCol.ColumnType);
                    currentCol.SetValue(obj, val);
                }
                r.Add((T)obj);
            }

            Finalize(stmt);
            return r;
        }

        public virtual T ExecuteScalar<T>() {
            if (Connection.Trace) {
                Console.WriteLine("Executing: " + this);
            }

            T val = default(T);

            var stmt = Prepare();
            if (SQLite3.Step(stmt) == SQLite3.Result.Row) {
                var colType = SQLite3.ColumnType(stmt, 0);
                val = (T)ReadCol(stmt, 0, colType, typeof(T));
            }
            Finalize(stmt);

            return val;
        }

        public virtual void Bind(string name, object val) {
            _bindings.Add(new Binding {
                Name = name,
                Value = val
            });
        }
        public virtual void Bind(object val) {
            Bind(null, val);
        }

        public override string ToString() {
            var parts = new string[1 + _bindings.Count];
            parts[0] = CommandText;
            var i = 1;
            foreach (var b in _bindings) {
                parts[i] = string.Format("  {0}: {1}", i - 1, b.Value);
                i++;
            }
            return string.Join(Environment.NewLine, parts);
        }

        protected virtual IntPtr Prepare() {
            var stmt = SQLite3.Prepare2(Connection.Handle, CommandText);
            SetColumnIndexes(stmt, _bindings);
            BindAll(stmt, _bindings);
            return stmt;
        }

        protected virtual void Finalize(IntPtr stmt) {
            SQLite3.Finalize(stmt);
        }

        protected static void SetColumnIndexes(IntPtr stmt, IEnumerable<IBinding> bindings) {
            int nextIdx = 1;
            foreach (var b in bindings) {
                if (b.Name != null) {
                    b.Index = SQLite3.BindParameterIndex(stmt, b.Name);
                }
                else {
                    b.Index = nextIdx++;
                }
            }
        }

        protected static void BindAll(IntPtr stmt, IEnumerable<IBinding> bindings) {
            foreach (var b in bindings) {
                BindParameter(stmt, b.Index, b.Value);
            }
        }

        internal static IntPtr NegativePointer = new IntPtr(-1);

        internal static void BindParameter(IntPtr stmt, int index, object value) {
            if (value == null) {
                SQLite3.BindNull(stmt, index);
            }
            else {
                if (value is Int32) {
                    SQLite3.BindInt(stmt, index, (int)value);
                }
                else if (value is String) {
                    SQLite3.BindText(stmt, index, ToUTF8((string)value), -1, NegativePointer);
                }
                else if (value is Byte || value is UInt16 || value is SByte || value is Int16) {
                    SQLite3.BindInt(stmt, index, Convert.ToInt32(value));
                }
                else if (value is Boolean) {
                    SQLite3.BindInt(stmt, index, (bool)value ? 1 : 0);
                }
                else if (value is UInt32 || value is Int64) {
                    SQLite3.BindInt64(stmt, index, Convert.ToInt64(value));
                }
                else if (value is Single || value is Double || value is Decimal) {
                    SQLite3.BindDouble(stmt, index, Convert.ToDouble(value));
                }
                else if (value is DateTime) {
                    SQLite3.BindText(stmt, index, ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss"), -1, NegativePointer);
                }
                else if (value.GetType().IsEnum) {
                    SQLite3.BindInt(stmt, index, Convert.ToInt32(value));
                }
                else if (value is byte[]) {
                    SQLite3.BindBlob(stmt, index, (byte[])value, ((byte[])value).Length, NegativePointer);
                }
                else if (value is Guid) {
                    SQLite3.BindText(stmt, index, ((Guid)value).ToString(), -1, NegativePointer);
                }
                else {
                    throw new NotSupportedException("Cannot store type: " + value.GetType());
                }
            }
        }

        private static Encoding _utf8 = UTF8Encoding.UTF8;

        /// <summary>
        /// Converts a string to a UTF-8 encoded byte array sized to include a null-terminating character.
        /// </summary>
        /// <param name="sourceText">The string to convert to UTF-8</param>
        /// <returns>A byte array containing the converted string plus an extra 0 terminating byte at the end of the array.</returns>
        public static byte[] ToUTF8(string sourceText) {
            Byte[] byteArray;
            int nlen = _utf8.GetByteCount(sourceText) + 1;

            byteArray = new byte[nlen];
            nlen = _utf8.GetBytes(sourceText, 0, sourceText.Length, byteArray, 0);
            byteArray[nlen] = 0;

            return byteArray;
        }

        public interface IBinding {
            string Name {
                get;
            }
            object Value {
                get;
            }
            int Index {
                get;
                set;
            }
        }

        private class Binding : IBinding {
            public string Name {
                get;
                set;
            }
            public object Value {
                get;
                set;
            }
            public int Index {
                get;
                set;
            }
        }

        private object ReadCol(IntPtr stmt, int index, SQLite3.ColType type, Type clrType) {
            if (type == SQLite3.ColType.Null) {
                return null;
            }
            else {
                if (clrType == typeof(String)) {
                    return SQLite3.ColumnString(stmt, index);
                }
                else if (clrType == typeof(Int32)) {
                    return (int)SQLite3.ColumnInt(stmt, index);
                }
                else if (clrType == typeof(Boolean)) {
                    return SQLite3.ColumnInt(stmt, index) == 1;
                }
                else if (clrType == typeof(double)) {
                    return SQLite3.ColumnDouble(stmt, index);
                }
                else if (clrType == typeof(float)) {
                    return (float)SQLite3.ColumnDouble(stmt, index);
                }
                else if (clrType == typeof(DateTime)) {
                    var text = SQLite3.ColumnString(stmt, index);
                    if (text != null && text.Length > 0) {
                        return DateTime.Parse(text);
                    }
                    return text;
                }
                else if (clrType == typeof(Guid)) {
                    var text = SQLite3.ColumnString(stmt, index);
                    if (text != null && text.Length > 0) {
                        return new Guid(text);
                    }
                    return text;
                }
                else if (clrType.IsEnum) {
                    return SQLite3.ColumnInt(stmt, index);
                }
                else if (clrType == typeof(Int64)) {
                    return SQLite3.ColumnInt64(stmt, index);
                }
                else if (clrType == typeof(UInt32)) {
                    return (uint)SQLite3.ColumnInt64(stmt, index);
                }
                else if (clrType == typeof(decimal)) {
                    return (decimal)SQLite3.ColumnDouble(stmt, index);
                }
                else if (clrType == typeof(Byte)) {
                    return (byte)SQLite3.ColumnInt(stmt, index);
                }
                else if (clrType == typeof(UInt16)) {
                    return (ushort)SQLite3.ColumnInt(stmt, index);
                }
                else if (clrType == typeof(Int16)) {
                    return (short)SQLite3.ColumnInt(stmt, index);
                }
                else if (clrType == typeof(sbyte)) {
                    return (sbyte)SQLite3.ColumnInt(stmt, index);
                }
                else if (clrType == typeof(byte[])) {
                    return SQLite3.ColumnByteArray(stmt, index);
                }
                else {
                    throw new NotSupportedException("Don't know how to read " + clrType);
                }
            }
        }
    }


    //TODO make thread safe

    /// <summary>
    /// Since the insert never changed, we only need to prepare once.
    /// </summary>
    public class PreparedSqlLiteInsertCommand : IDisposable {

        public bool Initialized {
            get;
            set;
        }

        protected SQLiteConnection Connection {
            get;
            set;
        }

        public string CommandText {
            get;
            set;
        }

        protected IntPtr Statement {
            get;
            set;
        }

        internal PreparedSqlLiteInsertCommand(SQLiteConnection conn) {
            Connection = conn;
        }

        public int ExecuteNonQuery(object[] source) {
            if (Connection.Trace) {
                Console.WriteLine("Executing: " + CommandText);
            }

            var r = SQLite3.Result.OK;

            if (!Initialized) {
                Statement = Prepare();
                Initialized = true;
            }

            //bind the values.
            if (source != null) {
                for (int i = 0; i < source.Length; i++) {
                    SQLiteCommand.BindParameter(Statement, i + 1, source[i]);
                }
            }
            r = SQLite3.Step(Statement);

            //trying to remove leak
            var result = SQLite3.Finalize(Statement);
            Initialized = false;
            Statement = IntPtr.Zero;

            ////clear the binding so they do not get saved.
            //SQLite3.ClearBindings(Statement);

            if (r == SQLite3.Result.Done) {
                int rowsAffected = SQLite3.Changes(Connection.Handle);
                SQLite3.Reset(Statement);
                return rowsAffected;
            }
            else if (r == SQLite3.Result.Error) {
                string msg = SQLite3.GetErrmsg(Connection.Handle);
                SQLite3.Reset(Statement);
                throw SQLiteException.New(r, msg);
            }
            else {
                SQLite3.Reset(Statement);
                throw SQLiteException.New(r, r.ToString());
            }
        }

        protected virtual IntPtr Prepare() {
            var stmt = SQLite3.Prepare2(Connection.Handle, CommandText);
            return stmt;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {

            if (Statement != IntPtr.Zero) {
                try {
                    var result = SQLite3.Finalize(Statement);
                }
                //catch (Exception exception) {
                //    if (ClientUtils.IsSecurityOrCriticalException(exception)) {
                //        throw;
                //    }
                //}
                finally {
                    Statement = IntPtr.Zero;
                    Connection = null;
                }
            }
        }

        ~PreparedSqlLiteInsertCommand() {
            Dispose(false);
        }
    }

    public class TableQuery<T> : IEnumerable<T> where T : new() {

        public SQLiteConnection Connection {
            get;
            private set;
        }

        public TableMapping Table {
            get;
            private set;
        }

        Expression _where;
        List<Ordering> _orderBys;
        int? _limit;
        int? _offset;

        class Ordering {
            public string ColumnName {
                get;
                set;
            }
            public bool Ascending {
                get;
                set;
            }
        }

        TableQuery(SQLiteConnection conn, TableMapping table) {
            Connection = conn;
            Table = table;
        }

        public TableQuery(SQLiteConnection conn) {
            Connection = conn;
            Table = Connection.GetMapping(typeof(T), default(TableNameAttribute), null);
        }

        public TableQuery<T> Clone() {
            var q = new TableQuery<T>(Connection, Table);
            q._where = _where;
            if (_orderBys != null) {
                q._orderBys = new List<Ordering>(_orderBys);
            }
            q._limit = _limit;
            q._offset = _offset;
            return q;
        }

        public TableQuery<T> Where(Expression<Func<T, bool>> predExpr) {
            if (predExpr.NodeType == ExpressionType.Lambda) {
                var lambda = (LambdaExpression)predExpr;
                var pred = lambda.Body;
                var q = Clone();
                q.AddWhere(pred);
                return q;
            }
            else {
                throw new NotSupportedException("Must be a predicate");
            }
        }

        public TableQuery<T> Take(int n) {
            var q = Clone();
            q._limit = n;
            return q;
        }

        public TableQuery<T> Skip(int n) {
            var q = Clone();
            q._offset = n;
            return q;
        }

        public TableQuery<T> OrderBy<U>(Expression<Func<T, U>> orderExpr) {
            return AddOrderBy<U>(orderExpr, true);
        }

        public TableQuery<T> OrderByDescending<U>(Expression<Func<T, U>> orderExpr) {
            return AddOrderBy<U>(orderExpr, false);
        }

        private TableQuery<T> AddOrderBy<U>(Expression<Func<T, U>> orderExpr, bool asc) {
            if (orderExpr.NodeType == ExpressionType.Lambda) {
                var lambda = (LambdaExpression)orderExpr;
                var mem = lambda.Body as MemberExpression;
                if (mem != null && (mem.Expression.NodeType == ExpressionType.Parameter)) {
                    var q = Clone();
                    if (q._orderBys == null) {
                        q._orderBys = new List<Ordering>();
                    }
                    q._orderBys.Add(new Ordering {
                        ColumnName = mem.Member.Name,
                        Ascending = asc
                    });
                    return q;
                }
                else {
                    throw new NotSupportedException("Order By does not support: " + orderExpr);
                }
            }
            else {
                throw new NotSupportedException("Must be a predicate");
            }
        }

        private void AddWhere(Expression pred) {
            if (_where == null) {
                _where = pred;
            }
            else {
                _where = Expression.AndAlso(_where, pred);
            }
        }

        private SQLiteCommand GenerateCommand(string selectionList) {
            var cmdText = "select " + selectionList + " from \"" + Table.TableName + "\"";
            var args = new List<object>();
            if (_where != null) {
                var w = CompileExpr(_where, args);
                cmdText += " where " + w.CommandText;
            }
            if ((_orderBys != null) && (_orderBys.Count > 0)) {
                var t = string.Join(", ", _orderBys.Select(o => "\"" + o.ColumnName + "\"" + (o.Ascending ? "" : " desc")).ToArray());
                cmdText += " order by " + t;
            }
            if (_limit.HasValue) {
                cmdText += " limit " + _limit.Value;
            }
            if (_offset.HasValue) {
                if (!_limit.HasValue) {
                    cmdText += " limit -1 ";
                }
                cmdText += " offset " + _offset.Value;
            }
            return Connection.CreateCommand(cmdText, args.ToArray());
        }

        class CompileResult {
            public string CommandText {
                get;
                set;
            }
            public object Value {
                get;
                set;
            }
        }

        private CompileResult CompileExpr(Expression expr, List<object> queryArgs) {
            if (expr == null) {
                throw new NotSupportedException("Expression is NULL");
            }
            else if (expr is BinaryExpression) {
                var bin = (BinaryExpression)expr;

                var leftr = CompileExpr(bin.Left, queryArgs);
                var rightr = CompileExpr(bin.Right, queryArgs);

                string text;
                if (leftr.CommandText == "?" && leftr.Value == null)
                    text = CompileNullBinaryExpression(bin, rightr);
                else if (rightr.CommandText == "?" && rightr.Value == null)
                    text = CompileNullBinaryExpression(bin, leftr);
                else
                    text = "(" + leftr.CommandText + " " + GetSqlName(bin) + " " + rightr.CommandText + ")";
                return new CompileResult {
                    CommandText = text
                };
            }
            else if (expr.NodeType == ExpressionType.Call) {

                var call = (MethodCallExpression)expr;
                var args = new CompileResult[call.Arguments.Count];

                for (var i = 0; i < args.Length; i++) {
                    args[i] = CompileExpr(call.Arguments[i], queryArgs);
                }

                var sqlCall = "";

                if (call.Method.Name == "Like" && args.Length == 2) {
                    sqlCall = "(" + args[0].CommandText + " like " + args[1].CommandText + ")";
                }
                else if (call.Method.Name == "Contains" && args.Length == 2) {
                    sqlCall = "(" + args[1].CommandText + " in " + args[0].CommandText + ")";
                }
                else {
                    sqlCall = call.Method.Name.ToLower() + "(" + string.Join(",", args.Select(a => a.CommandText).ToArray()) + ")";
                }
                return new CompileResult {
                    CommandText = sqlCall
                };

            }
            else if (expr.NodeType == ExpressionType.Constant) {
                var c = (ConstantExpression)expr;
                queryArgs.Add(c.Value);
                return new CompileResult {
                    CommandText = "?",
                    Value = c.Value
                };
            }
            else if (expr.NodeType == ExpressionType.Convert) {
                var u = (UnaryExpression)expr;
                var ty = u.Type;
                var valr = CompileExpr(u.Operand, queryArgs);
                return new CompileResult {
                    CommandText = valr.CommandText,
                    Value = valr.Value != null ? Convert.ChangeType(valr.Value, ty) : null
                };
            }
            else if (expr.NodeType == ExpressionType.MemberAccess) {
                var mem = (MemberExpression)expr;

                if (mem.Expression.NodeType == ExpressionType.Parameter) {
                    //
                    // This is a column of our table, output just the column name
                    //
                    return new CompileResult {
                        CommandText = "\"" + mem.Member.Name + "\""
                    };
                }
                else {
                    object obj = null;
                    if (mem.Expression != null) {
                        var r = CompileExpr(mem.Expression, queryArgs);
                        if (r.Value == null) {
                            throw new NotSupportedException("Member access failed to compile expression");
                        }
                        if (r.CommandText == "?") {
                            queryArgs.RemoveAt(queryArgs.Count - 1);
                        }
                        obj = r.Value;
                    }

                    //
                    // Get the member value
                    //
                    object val = null;
                    if (mem.Member.MemberType == MemberTypes.Property) {
                        var m = (PropertyInfo)mem.Member;
                        val = m.GetValue(obj, null);
                    }
                    else if (mem.Member.MemberType == MemberTypes.Field) {
                        var m = (FieldInfo)mem.Member;
                        val = m.GetValue(obj);
                    }
                    else {
                        throw new NotSupportedException("MemberExpr: " + mem.Member.MemberType.ToString());
                    }
                    //
                    // Work special magic for enumerables
                    //
                    if (val != null && val is System.Collections.IEnumerable && !(val is string)) {
                        var sb = new System.Text.StringBuilder();
                        sb.Append("(");
                        var head = "";
                        foreach (var a in (System.Collections.IEnumerable)val) {
                            queryArgs.Add(a);
                            sb.Append(head);
                            sb.Append("?");
                            head = ",";
                        }
                        sb.Append(")");
                        return new CompileResult {
                            CommandText = sb.ToString(),
                            Value = val
                        };
                    }
                    else {
                        queryArgs.Add(val);
                        return new CompileResult {
                            CommandText = "?",
                            Value = val
                        };
                    }
                }
            }
            throw new NotSupportedException("Cannot compile: " + expr.NodeType.ToString());
        }

        /// <summary>
        /// Compiles a BinaryExpression where one of the parameters is null.
        /// </summary>
        /// <param name="parameter">The non-null parameter</param>
        private string CompileNullBinaryExpression(BinaryExpression expression, CompileResult parameter) {
            if (expression.NodeType == ExpressionType.Equal)
                return "(" + parameter.CommandText + " is ?)";
            else if (expression.NodeType == ExpressionType.NotEqual)
                return "(" + parameter.CommandText + " is not ?)";
            else
                throw new NotSupportedException("Cannot compile Null-BinaryExpression with type " + expression.NodeType.ToString());
        }

        string GetSqlName(Expression expr) {
            var n = expr.NodeType;
            if (n == ExpressionType.GreaterThan)
                return ">";
            else if (n == ExpressionType.GreaterThanOrEqual) {
                return ">=";
            }
            else if (n == ExpressionType.LessThan) {
                return "<";
            }
            else if (n == ExpressionType.LessThanOrEqual) {
                return "<=";
            }
            else if (n == ExpressionType.And) {
                return "and";
            }
            else if (n == ExpressionType.AndAlso) {
                return "and";
            }
            else if (n == ExpressionType.Or) {
                return "or";
            }
            else if (n == ExpressionType.OrElse) {
                return "or";
            }
            else if (n == ExpressionType.Equal) {
                return "=";
            }
            else if (n == ExpressionType.NotEqual) {
                return "!=";
            }
            else {
                throw new System.NotSupportedException("Cannot get SQL for: " + n.ToString());
            }
        }

        public int Count() {
            return GenerateCommand("count(*)").ExecuteScalar<int>();
        }
        public IEnumerator<T> GetEnumerator() {
            return GenerateCommand("*").ExecuteQuery<T>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }

    public static class SQLite3 {

        public enum Result : int {
            OK = 0,
            Error = 1,
            Internal = 2,
            Perm = 3,
            Abort = 4,
            Busy = 5,
            Locked = 6,
            NoMem = 7,
            ReadOnly = 8,
            Interrupt = 9,
            IOError = 10,
            Corrupt = 11,
            NotFound = 12,
            TooBig = 18,
            Constraint = 19,
            Row = 100,
            Done = 101
        }

        public enum ConfigOption : int {
            SingleThread = 1,
            MultiThread = 2,
            Serialized = 3
        }

        public static string GetErrmsg(IntPtr db) {
            return Marshal.PtrToStringUni(Errmsg(db));
        }

        public static IntPtr Prepare2(IntPtr db, string query) {
            IntPtr stmt;
            var r = Prepare2(db, query, query.Length, out stmt, IntPtr.Zero);
            if (r != Result.OK) {
                throw SQLiteException.New(r, GetErrmsg(db));
            }
            return stmt;
        }

        public static string ColumnString(IntPtr stmt, int index) {
            return Marshal.PtrToStringUni(SQLite3.ColumnText16(stmt, index));
        }

        public static byte[] ColumnByteArray(IntPtr stmt, int index) {
            int length = ColumnBytes(stmt, index);
            byte[] result = new byte[length];
            if (length > 0)
                Marshal.Copy(ColumnBlob(stmt, index), result, 0, length);
            return result;
        }

        public enum ColType : int {
            Integer = 1,
            Float = 2,
            Text = 3,
            Blob = 4,
            Null = 5
        }

        private static bool x64 = false;

        static SQLite3() {
            x64 = IntPtr.Size == 8;
        }

        public static ColType ColumnType(IntPtr stmt, int index) {
            if (x64)
                return SQLite64.ColumnType(stmt, index);
            else
                return SQLite32.ColumnType(stmt, index);
        }
        public static IntPtr ColumnBlob(IntPtr stmt, int index) {
            if (x64)
                return SQLite64.ColumnBlob(stmt, index);
            else
                return SQLite32.ColumnBlob(stmt, index);
        }
        public static IntPtr ColumnName(IntPtr stmt, int index) {
            if (x64)
                return SQLite64.ColumnName(stmt, index);
            else
                return SQLite32.ColumnName(stmt, index);
        }
        public static IntPtr ColumnName16(IntPtr stmt, int index) {
            if (x64)
                return SQLite64.ColumnName16(stmt, index);
            else
                return SQLite32.ColumnName16(stmt, index);
        }
        public static IntPtr ColumnText(IntPtr stmt, int index) {
            if (x64)
                return SQLite64.ColumnText(stmt, index);
            else
                return SQLite32.ColumnText(stmt, index);
        }
        public static IntPtr ColumnText16(IntPtr stmt, int index) {
            if (x64)
                return SQLite64.ColumnText16(stmt, index);
            else
                return SQLite32.ColumnText16(stmt, index);
        }
        public static IntPtr Errmsg(IntPtr db) {
            if (x64)
                return SQLite64.Errmsg(db);
            else
                return SQLite32.Errmsg(db);
        }
        public static Result BusyTimeout(IntPtr db, int milliseconds) {
            if (x64)
                return SQLite64.BusyTimeout(db, milliseconds);
            else
                return SQLite32.BusyTimeout(db, milliseconds);
        }
        public static Result Close(IntPtr db) {
            if (x64)
                return SQLite64.Close(db);
            else
                return SQLite32.Close(db);
        }
        public static Result Config(ConfigOption option) {
            if (x64)
                return SQLite64.Config(option);
            else
                return SQLite32.Config(option);
        }
        public static Result Finalize(IntPtr stmt) {
            if (x64)
                return SQLite64.Finalize(stmt);
            else
                return SQLite32.Finalize(stmt);
        }
        public static Result Open(string filename, out IntPtr db) {
            if (x64)
                return SQLite64.Open(filename, out db);
            else
                return SQLite32.Open(filename, out db);
        }
        public static Result Prepare2(IntPtr db, string sql, int numBytes, out IntPtr stmt, IntPtr pzTail) {
            if (x64)
                return SQLite64.Prepare2(db, sql, numBytes, out stmt, pzTail);
            else
                return SQLite32.Prepare2(db, sql, numBytes, out stmt, pzTail);
        }
        public static Result Reset(IntPtr stmt) {
            if (x64)
                return SQLite64.Reset(stmt);
            else
                return SQLite32.Reset(stmt);
        }
        public static Result Step(IntPtr stmt) {
            if (x64)
                return SQLite64.Step(stmt);
            else
                return SQLite32.Step(stmt);
        }
        public static int BindBlob(IntPtr stmt, int index, byte[] val, int n, IntPtr free) {
            if (x64)
                return SQLite64.BindBlob(stmt, index, val, n, free);
            else
                return SQLite32.BindBlob(stmt, index, val, n, free);
        }
        public static int BindDouble(IntPtr stmt, int index, double val) {
            if (x64)
                return SQLite64.BindDouble(stmt, index, val);
            else
                return SQLite32.BindDouble(stmt, index, val);
        }
        public static int BindInt(IntPtr stmt, int index, int val) {
            if (x64)
                return SQLite64.BindInt(stmt, index, val);
            else
                return SQLite32.BindInt(stmt, index, val);
        }
        public static int BindInt64(IntPtr stmt, int index, long val) {
            if (x64)
                return SQLite64.BindInt64(stmt, index, val);
            else
                return SQLite32.BindInt64(stmt, index, val);
        }
        public static int BindNull(IntPtr stmt, int index) {
            if (x64)
                return SQLite64.BindNull(stmt, index);
            else
                return SQLite32.BindNull(stmt, index);
        }
        public static int BindParameterIndex(IntPtr stmt, string name) {
            if (x64)
                return SQLite64.BindParameterIndex(stmt, name);
            else
                return SQLite32.BindParameterIndex(stmt, name);
        }
        //only used for date and guid. other strings need to be null terminated byte array
        public static int BindText(IntPtr stmt, int index, string val, int n, IntPtr free) {
            if (x64)
                return SQLite64.BindText(stmt, index, val, n, free);
            else
                return SQLite32.BindText(stmt, index, val, n, free);
        }
        public static int BindText(IntPtr stmt, int index, byte[] val, int n, IntPtr free) {
            if (x64)
                return SQLite64.BindText(stmt, index, val, n, free);
            else
                return SQLite32.BindText(stmt, index, val, n, free);
        }
        public static int Changes(IntPtr db) {
            if (x64)
                return SQLite64.Changes(db);
            else
                return SQLite32.Changes(db);
        }
        public static int ClearBindings(IntPtr db) {
            if (x64)
                return SQLite64.ClearBindings(db);
            else
                return SQLite32.ClearBindings(db);
        }

        public static int ColumnCount(IntPtr stmt) {
            if (x64)
                return SQLite64.ColumnCount(stmt);
            else
                return SQLite32.ColumnCount(stmt);
        }

        public static int ColumnBytes(IntPtr stmt, int index) {
            if (x64)
                return SQLite64.ColumnBytes(stmt, index);
            else
                return SQLite32.ColumnBytes(stmt, index);
        }
        public static double ColumnDouble(IntPtr stmt, int index) {
            if (x64)
                return SQLite64.ColumnDouble(stmt, index);
            else
                return SQLite32.ColumnDouble(stmt, index);
        }
        public static int ColumnInt(IntPtr stmt, int index) {
            if (x64)
                return SQLite64.ColumnInt(stmt, index);
            else
                return SQLite32.ColumnInt(stmt, index);
        }
        public static long ColumnInt64(IntPtr stmt, int index) {
            if (x64)
                return SQLite64.ColumnInt64(stmt, index);
            else
                return SQLite32.ColumnInt64(stmt, index);
        }
        public static long LastInsertRowid(IntPtr db) {
            if (x64)
                return SQLite64.LastInsertRowid(db);
            else
                return SQLite32.LastInsertRowid(db);
        }

        private static class SQLite32 {
            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_bind_parameter_index")]
            public static extern int BindParameterIndex(IntPtr stmt, string name);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_bind_null")]
            public static extern int BindNull(IntPtr stmt, int index);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_bind_int")]
            public static extern int BindInt(IntPtr stmt, int index, int val);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_bind_int64")]
            public static extern int BindInt64(IntPtr stmt, int index, long val);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_bind_double")]
            public static extern int BindDouble(IntPtr stmt, int index, double val);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_bind_text")]
            public static extern int BindText(IntPtr stmt, int index, string val, int n, IntPtr free);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_bind_text")]
            public static extern int BindText(IntPtr stmt, int index, byte[] val, int n, IntPtr free);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_bind_blob")]
            public static extern int BindBlob(IntPtr stmt, int index, byte[] val, int n, IntPtr free);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_busy_timeout")]
            public static extern Result BusyTimeout(IntPtr db, int milliseconds);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_changes")]
            public static extern int Changes(IntPtr db);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_clear_bindings")]
            public static extern int ClearBindings(IntPtr db);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_close")]
            public static extern Result Close(IntPtr db);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_config")]
            public static extern Result Config(ConfigOption option);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_errmsg16")]
            public static extern IntPtr Errmsg(IntPtr db);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_finalize")]
            public static extern Result Finalize(IntPtr stmt);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_last_insert_rowid")]
            public static extern long LastInsertRowid(IntPtr db);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_open")]
            public static extern Result Open(string filename, out IntPtr db);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_prepare_v2")]
            public static extern Result Prepare2(IntPtr db, string sql, int numBytes, out IntPtr stmt, IntPtr pzTail);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_reset")]
            public static extern Result Reset(IntPtr stmt);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_step")]
            public static extern Result Step(IntPtr stmt);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_column_count")]
            public static extern int ColumnCount(IntPtr stmt);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_column_name")]
            public static extern IntPtr ColumnName(IntPtr stmt, int index);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_column_name16")]
            public static extern IntPtr ColumnName16(IntPtr stmt, int index);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_column_type")]
            public static extern ColType ColumnType(IntPtr stmt, int index);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_column_int")]
            public static extern int ColumnInt(IntPtr stmt, int index);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_column_int64")]
            public static extern long ColumnInt64(IntPtr stmt, int index);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_column_double")]
            public static extern double ColumnDouble(IntPtr stmt, int index);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_column_text")]
            public static extern IntPtr ColumnText(IntPtr stmt, int index);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_column_text16")]
            public static extern IntPtr ColumnText16(IntPtr stmt, int index);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_column_blob")]
            public static extern IntPtr ColumnBlob(IntPtr stmt, int index);

            [DllImport("SqLite3.dll", EntryPoint = "sqlite3_column_bytes")]
            public static extern int ColumnBytes(IntPtr stmt, int index);
        }

        private static class SQLite64 {
            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_bind_parameter_index")]
            public static extern int BindParameterIndex(IntPtr stmt, string name);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_bind_null")]
            public static extern int BindNull(IntPtr stmt, int index);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_bind_int")]
            public static extern int BindInt(IntPtr stmt, int index, int val);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_bind_int64")]
            public static extern int BindInt64(IntPtr stmt, int index, long val);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_bind_double")]
            public static extern int BindDouble(IntPtr stmt, int index, double val);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_bind_text")]
            public static extern int BindText(IntPtr stmt, int index, string val, int n, IntPtr free);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_bind_text")]
            public static extern int BindText(IntPtr stmt, int index, byte[] val, int n, IntPtr free);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_bind_blob")]
            public static extern int BindBlob(IntPtr stmt, int index, byte[] val, int n, IntPtr free);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_busy_timeout")]
            public static extern Result BusyTimeout(IntPtr db, int milliseconds);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_changes")]
            public static extern int Changes(IntPtr db);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_clear_bindings")]
            public static extern int ClearBindings(IntPtr db);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_close")]
            public static extern Result Close(IntPtr db);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_config")]
            public static extern Result Config(ConfigOption option);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_errmsg16")]
            public static extern IntPtr Errmsg(IntPtr db);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_finalize")]
            public static extern Result Finalize(IntPtr stmt);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_last_insert_rowid")]
            public static extern long LastInsertRowid(IntPtr db);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_open")]
            public static extern Result Open(string filename, out IntPtr db);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_prepare_v2")]
            public static extern Result Prepare2(IntPtr db, string sql, int numBytes, out IntPtr stmt, IntPtr pzTail);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_reset")]
            public static extern Result Reset(IntPtr stmt);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_step")]
            public static extern Result Step(IntPtr stmt);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_column_count")]
            public static extern int ColumnCount(IntPtr stmt);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_column_name")]
            public static extern IntPtr ColumnName(IntPtr stmt, int index);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_column_name16")]
            public static extern IntPtr ColumnName16(IntPtr stmt, int index);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_column_type")]
            public static extern ColType ColumnType(IntPtr stmt, int index);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_column_int")]
            public static extern int ColumnInt(IntPtr stmt, int index);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_column_int64")]
            public static extern long ColumnInt64(IntPtr stmt, int index);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_column_double")]
            public static extern double ColumnDouble(IntPtr stmt, int index);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_column_text")]
            public static extern IntPtr ColumnText(IntPtr stmt, int index);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_column_text16")]
            public static extern IntPtr ColumnText16(IntPtr stmt, int index);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_column_blob")]
            public static extern IntPtr ColumnBlob(IntPtr stmt, int index);

            [DllImport("SqLite364.dll", EntryPoint = "sqlite3_column_bytes")]
            public static extern int ColumnBytes(IntPtr stmt, int index);
        }
    }

    public static class DelegateHelper {

        public static Func<TDeclaringType, object> BuildGetDelegateExpression<TDeclaringType>(PropertyInfo property) {

            var getMethod = property.GetGetMethod();

            var instance = Expression.Parameter(typeof(TDeclaringType), "instance");
            var propertyExpression = Expression.Property(
                getMethod.IsStatic ? null : instance, property);
            var lambda = Expression.Lambda<Func<TDeclaringType, object>>(
                Expression.TypeAs(propertyExpression, typeof(object)), instance);
            return lambda.Compile();

        }

        public static Action<T, P> BuildSetDelegateExpression<T, P>(PropertyInfo property) {

            var setinfo = property.GetSetMethod(true);
            if (setinfo != null) {
                ParameterExpression paramSet = Expression.Parameter(typeof(T), "param");
                ParameterExpression paramSetValue = Expression.Parameter(typeof(P), "newvalue");

                MethodCallExpression MethodCallSetterOfProperty = Expression.Call(paramSet, setinfo, paramSetValue);
                Expression SetPropertyValueExp = Expression.Lambda(MethodCallSetterOfProperty, paramSet, paramSetValue);
                var SetPropertyValueLambda = (Expression<Action<T, P>>)SetPropertyValueExp;
                return SetPropertyValueLambda.Compile();
            }
            return null;
        }
    }
}
