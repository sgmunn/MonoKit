// --------------------------------------------------------------------------------------------------------------------
// <copyright file=".cs" company="sgmunn">
//   (c) sgmunn 2012  
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//   the Software.
// 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//   THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//   IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using MonoKitSample.Domain;

namespace MonoKitSample
{
    using System;
    using MonoKit.UI;
    using MonoKit.UI.Controls;
    using MonoKit.UI.Elements;
    using MonoTouch.UIKit;
    using MonoKit.DataBinding;
    using MonoKit.Domain;
    using System.Linq;
    using MonoKit.Domain.Data;
    using MonoKit.Data;
    using System.Collections.Generic;
    using MonoKit.Data.SQLite;
    using MonoKit.Domain.Data.SQLite;
    using System.Reflection;
    using MonoKit.Tasks;
    using MonoKit.Reactive;
    using MonoKit;
    using System.Threading;
    using System.Threading.Tasks;
    using MonoKit.Reactive.Linq;
    using MonoKitSample.Data;

    public class Samples
    {
        private UINavigationController rootController;
                
        private Person bindToSingleObjectPersion;

        public Samples(UINavigationController rootController)
        {
            this.rootController = rootController;
            
            this.bindToSingleObjectPersion = new Person()
            {
                FirstName = "Greg",
                LastName = "Munn",
                Birthdate = DateTime.Today.AddDays(-10),
            };
        }
        
        public void SetupMainIndexSection(TableViewSource source)
        {
            var section1 = new TableViewSection(source);
            
            section1.Header = "MonoKit Sample Index";
            
            section1.Add(new DisclosureElement("Element Samples") { Command = this.GotoElementSamples });
            section1.Add(new DisclosureElement("Binding Samples") { Command = this.GotoBindingSamples });
            section1.Add(new DisclosureElement("Large Data Sample") { Command = this.GotoLargeDataSample });
            section1.Add(new DisclosureElement("Bind to Single Object") { Command = this.GotoBindToSingleObject });
            
            var section2 = new TableViewSection(source);
            
            section2.Header = "Test Samples";
            
            section2.Add(new DisclosureElement("Tasks Test") { Command = this.DoTaskTest });
            section2.Add(new DisclosureElement("Snapshot Domain Test") { Command = this.DoDomainTest2 });
            section2.Add(new DisclosureElement("Event Sourced Domain Test") { Command = this.DoDomainTest1 });
            section2.Add(new DisclosureElement("SQLite Admin Test") { Command = this.DoSqliteTest });
            section2.Add(new DisclosureElement("Custom Control") { Command = this.GotoCustomControl });
            section2.Add(new DisclosureElement("GC Tests") { Command = this.GotoGCTests });
            
            var section3 = new TableViewSection(source);
            
            section3.Header = "Utils";
            
            section3.Add(new StringElement("GC.Collect") { Command = this.DoGCCollect });
        }
                
        protected void GotoElementSamples(Element element)
        {
            var tb = new TableViewController(UITableViewStyle.Grouped);
            tb.Title = "Element Samples";
            
            var section = new TableViewSection(tb.Source);
            
            section.Header = "Sample Elements";
            
            section.Add(new StringElement("String"));
            section.Add(new SubtitleStringElement("Subtitle String", "Subtitle"));
            section.Add(new Value1StringElement("Value1 String", "Detail"));
            section.Add(new Value2StringElement("Value2 String", "Detail"));
            section.Add(new BooleanElement("Boolean", true));
            section.Add(new CheckboxElement("Checkbox", true));
            section.Add(new TextInputElement("Text Input", "") {Placeholder = "Placeholder text" });
            section.Add(new PasswordInputElement("Password"));
            section.Add(new DateInputElement("Date", DateTime.Today));
            
            // Use a DataViewWrapper to wrap an element with a different view definition from the one it would normally get
            var wrapper = new ElementDataViewWrapper(DefaultElementViewDefintions.DisclosureElementViewDefintion, new StringElement("Wrapped String"));
            section.Add(wrapper);
            

            this.rootController.PushViewController(tb, true);
        }
                
        protected void GotoBindingSamples(Element element)
        {
            var tb = new TableViewController(UITableViewStyle.Grouped);
            tb.Title = "Binding Samples";

            var section1 = new TableViewSection(tb.Source);
            
            section1.Header = "Same element";
            section1.Footer = "Note that both elements update";

            var boolElement = new BooleanElement("Bool Value", false);
            section1.Add(boolElement);
            section1.Add(boolElement);

            this.rootController.PushViewController(tb, true);
        }
                
        protected void GotoLargeDataSample(Element element)
        {
            var tb = new TableViewController(UITableViewStyle.Grouped);
            tb.Title = "Large Data Sample";

            var section1 = new TableViewSection(tb.Source);
            
            section1.Header = "1000 Items";
            
            for (int i = 0; i < 1000; i++)
            {
                section1.Add(new StringElement("String Element " + i.ToString()));
            }

            this.rootController.PushViewController(tb, true);
        }
        
        protected void GotoBindToSingleObject(Element element)
        {
            var tb = new TableViewController(UITableViewStyle.Grouped);
            tb.Title = "Single Object Binding Sample";

            var section1 = new TableViewSection(tb.Source);
            
            section1.Add(new TextInputElement(this.bindToSingleObjectPersion, null, new Binding("FirstName")) { Placeholder = "First Name" });
            section1.Add(new TextInputElement(this.bindToSingleObjectPersion, null, new Binding("LastName")) { Placeholder = "Last Name" });
            section1.Add(new DateInputElement(this.bindToSingleObjectPersion, null, new Binding("Birthdate")) );

            this.rootController.PushViewController(tb, true);
        }
                        
        protected void GotoCustomControl(Element element)
        {
            var tb = new TableViewController(UITableViewStyle.Grouped);
            tb.Title = "Custom Control";

            var section1 = new TableViewSection(tb.Source);
            
            section1.Header = "Custom Control";
   
            var view = new UIViewDefinition<CustomElementTableViewCell, StringElement>(DefaultElementViewDefintions.SimpleElementBinding) 
            { 
                Param = UITableViewCellStyle.Default 
            };

            
            var customData = new StringElement("");
            var wrapper = new ElementDataViewWrapper(view, customData);
            section1.Add(wrapper);

            this.rootController.PushViewController(tb, true);
        }
        
        protected void DoGCCollect(Element element)
        {
            GC.Collect();
        }
                        
        protected void GotoGCTests(Element element)
        {
            var tb = new TableViewController(UITableViewStyle.Grouped);
            tb.Title = "GC Tests";

            var section1 = new TableViewSection(tb.Source);
            
            section1.Add(new StringElement("Self Referencing Items"){ Command = this.SelfReferencingItemsTest});

            this.rootController.PushViewController(tb, true);
        }
        
        protected void SelfReferencingItemsTest(Element element)
        {
        }

        
        protected void DoDomainTest1(Element element)
        {
            var tb = new TableViewController(UITableViewStyle.Grouped);
            tb.Title = "Event Sourced";

            var section1 = new TableViewSection(tb.Source);
            
            section1.Header = "1";
            section1.Add(new StringElement("Test Command 1"){ Command = (x) => EventSourceSamples.DoTest1()});

            this.rootController.PushViewController(tb, true);




            // register events before creating context if using default serializer
//            KnownTypes.RegisterEvents(Assembly.GetExecutingAssembly());
//
//            SampleDB.Main.CreateTable<SerializedEvent>();
//   
//            var storage = new EventStoreRepository<SerializedEvent>(SampleDB.Main);
//            var context = new SampleContext1(SampleDB.Main, storage, null);
//
//            var id = new Guid("{4332b5a6-ab99-49dc-b37f-216c67247f14}");
//            var cmd = new DomainCommandExecutor<EventSourcedRoot>(context);
//            cmd.Execute(new CreateCommand() { AggregateId = new Identity(id) });
 



            // not supported at the moment
//            var scope = context.BeginUnitOfWork();
//            
//            using (scope)
//            {
//                cmd.Execute(scope, new TestCommand() { AggregateId = id, Description = DateTime.Now.ToShortTimeString(), });
//                scope.Commit();
//            }
        }
        
        protected void DoDomainTest2(Element element)
        {
            var tb = new TableViewController(UITableViewStyle.Grouped);
            tb.Title = "Snapshot Sourced";

            var section1 = new TableViewSection(tb.Source);
            
            section1.Header = "1";
            section1.Add(new StringElement("Test Command 1"){ Command = (x) => SnapshotSamples.DoTest1()});

            var adminSection = new TableViewSection(tb.Source);
            
            adminSection.Header = "Sql";
            adminSection.Add(new StringElement("Browse"){ Command = (x) => 
                {
                var admin = new SQLite.MonoTouchAdmin.SQLiteAdmin(SnapshotSourcedDB.Main);
                this.rootController.PushViewController(admin.NewTablesViewController(), true);
                }
            });

            this.rootController.PushViewController(tb, true);
        }
        
        protected void DoMinionTest(Element element)
        {
            // register events before creating context if using default serializer
//            KnownTypes.RegisterEvents(Assembly.GetExecutingAssembly());
//
//            // setup and bootstrap context
//            SampleDB.Main.CreateTable<MinionDataContract>();
//            SampleDB.Main.CreateTable<PocketMoneyTransactionDataContract>();
//   
//            var storage = new EventStoreRepository<SerializedEvent>(SampleDB.Main);
//            
//            var context = new MinionContext(SampleDB.Main, storage, null);
//
//            context.RegisterSnapshot<Minion>(c => new SnapshotRepository<MinionDataContract>(SampleDB.Main));
//            
//            context.RegisterBuilder<Minion>(c => new TransactionReadModelBuilder(new SqlRepository<PocketMoneyTransactionDataContract>(SampleDB.Main)));
//            
//            // the commanding bit
//            var id = new Guid("{b6dc9675-a6ca-4767-8b49-e24b4262ac93}");
//            var cmd = context.NewCommandExecutor<Minion>();
//            cmd.Execute(new CreateCommand() { AggregateId = new Identity(id) });
 




            // not supported just yet
//            var scope = context.BeginUnitOfWork();
//            
//            using (scope)
//            {
//                cmd.Execute(
//                    scope,
//                    new EarnPocketMoneyCommand() { AggregateId = id, Date = DateTime.Today, Amount = 10, });
//                scope.Commit();
//            }


//            cmd.Execute(new EarnPocketMoneyCommand() { AggregateId = new Identity(id), Date = DateTime.Today, Amount = 10, });
        }
                
        protected void DoSqliteTest(Element element)
        {
            var admin = new SQLite.MonoTouchAdmin.SQLiteAdmin(SampleDB.Main);
            this.rootController.PushViewController(admin.NewTablesViewController(), true);
        }

        private IDisposable temp;
                
        protected void DoTaskTest(Element element)
        {
            Console.WriteLine("start test {0}", Thread.CurrentThread.ManagedThreadId);

            Action act = () => {Thread.Sleep(2000);Console.WriteLine("Nested {0}", Thread.CurrentThread.ManagedThreadId);};

            var task = new Task(() => {Thread.Sleep(1000);
                Console.WriteLine("Task {0}", Thread.CurrentThread.ManagedThreadId);
            
            var next = new Task(act,TaskCreationOptions.AttachedToParent);
                next.Start();
            });

            // continuations not supported, nested work fine though
            // we could do an asobservable on it but without the .Start -- we'd need a different AsObservable though
            // OR if we want to continue tasks then chain them using observables
            // task.ContinueWith((_) => {Console.WriteLine("Continue {0}", Thread.CurrentThread.ManagedThreadId);});

            if (temp != null)
            {
                Console.WriteLine("reset temp {0}", Thread.CurrentThread.ManagedThreadId);

                temp.Dispose();
                temp = null;
                return;
            }

            temp = Observable.Start(act).
            //temp = Observable.Start( () => { 
            //    Thread.Sleep(5000);
            //    return Unit.Default; }  ).
            //task.AsObservable().
                ObserveOnMainThread().Subscribe
                (_ => 
                 {
                    Console.WriteLine("Done {0}", Thread.CurrentThread.ManagedThreadId);
                    element.Text = "Done";
                });

            Console.WriteLine("Done button click");
        }
    }
    
    public class SampleContext1 : SqlDomainContext
    {
        public SampleContext1(SQLiteConnection connection, IEventStoreRepository eventStore, IDomainEventBus eventBus) : base(connection, eventStore, eventBus)
        {
        }
    }
    
    public class SampleContext2 : SqlDomainContext
    {
        public SampleContext2(SQLiteConnection connection, IEventStoreRepository eventStore, IDomainEventBus eventBus) : base(connection, eventStore, eventBus)
        {
        }
    }
}

