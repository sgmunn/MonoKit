using System;
using System.IO;
using MonoKit.Data.SQLite;
using MonoKit.Data;
using System.Linq;

namespace MonoKitSample
{
    public class SampleDB : SQLiteConnection
    {
        private static SampleDB MainDatabase = new SampleDB();

        public static string SampleDatabasePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Sample.db");
        }

        public static SampleDB Main
        {
            get
            {
                return MainDatabase;
            }
        }

        protected SampleDB() : base(SampleDatabasePath())
        {
        }

        public ISQLiteRepository<T> NewRepository<T>() where T: class, new()
        {
            return new SQLiteRepository<T>(this, false);
        }
    }
}

