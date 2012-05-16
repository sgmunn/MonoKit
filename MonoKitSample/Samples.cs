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
using MonoKit.Domain.Commands;

namespace MonoKitSample
{
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
            
            section2.Add(new DisclosureElement("Event Sourced Domain Test") { Command = this.DoDomainTest1 });
            section2.Add(new DisclosureElement("Snapshot Domain Test") { Command = this.DoDomainTest2 });
            section2.Add(new DisclosureElement("Custom Control") { Command = this.GotoCustomControl });
            section2.Add(new DisclosureElement("GC Tests") { Command = this.GotoGCTests });
            
            var section3 = new TableViewSection(source);
            
            section3.Header = "Utils";
            
            section3.Add(new StringElement("GC.Collect") { Command = this.DoGCCollect });
        }
                
        private void GotoElementSamples(Element element)
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
                
        private void GotoBindingSamples(Element element)
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
                
        private void GotoLargeDataSample(Element element)
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
        
        private void GotoBindToSingleObject(Element element)
        {
            var tb = new TableViewController(UITableViewStyle.Grouped);
            tb.Title = "Single Object Binding Sample";

            var section1 = new TableViewSection(tb.Source);
            
            section1.Add(new TextInputElement(this.bindToSingleObjectPersion, null, new Binding("FirstName")) { Placeholder = "First Name" });
            section1.Add(new TextInputElement(this.bindToSingleObjectPersion, null, new Binding("LastName")) { Placeholder = "Last Name" });
            section1.Add(new DateInputElement(this.bindToSingleObjectPersion, null, new Binding("Birthdate")) );

            this.rootController.PushViewController(tb, true);
        }
                        
        private void GotoCustomControl(Element element)
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
        
        private void DoGCCollect(Element element)
        {
            GC.Collect();
        }
                        
        private void GotoGCTests(Element element)
        {
            var tb = new TableViewController(UITableViewStyle.Grouped);
            tb.Title = "GC Tests";

            var section1 = new TableViewSection(tb.Source);
            
            section1.Add(new StringElement("Self Referencing Items"){ Command = this.SelfReferencingItemsTest});

            this.rootController.PushViewController(tb, true);
        }
        
        private void SelfReferencingItemsTest(Element element)
        {
        }

        
        private void DoDomainTest1(Element element)
        {
            SampleDB.Main.CreateTable<SQLStoredEvent>();
   
            var storage = new EventStoreRepository<SQLStoredEvent>(SampleDB.Main);
            var context = new SampleContext1(SampleDB.Main, storage, null);

            var id = new Guid("{4332b5a6-ab99-49dc-b37f-216c67247f14}");
            var cmd = new DomainCommandExecutor<EventSourcedTestRoot>(context);
            cmd.Execute(new CreateCommand() { AggregateId = id });
 
            var scope = context.GetScope();
            
            using (scope)
            {
                cmd.Execute(scope, new TestCommand() { AggregateId = id, Description = DateTime.Now.ToShortTimeString(), });
                scope.Commit();
            }
        }
        
        private void DoDomainTest2(Element element)
        {
            // setup and bootstrap context
            SampleDB.Main.CreateTable<TestSnapshot>();
   
            var storage = new EventStoreRepository<SQLStoredEvent>(SampleDB.Main);
            
            var context = new SampleContext2(SampleDB.Main, storage, null);

            context.RegisterSnapshot<SnapshotTestRoot>(c => new SnapshotRepository<TestSnapshot>(SampleDB.Main));
            
            context.RegisterDenormalizer<SnapshotTestRoot>(c => new TestDenormalizer(new SQLiteRepository<TestReadModel>(SampleDB.Main)));
            
            // the commanding bit
            var id = new Guid("{c239587e-c8bc-4654-9f28-6a79a7feb12a}");
            var cmd = new DomainCommandExecutor<SnapshotTestRoot>(context);
            cmd.Execute(new CreateCommand() { AggregateId = id });
 
            var scope = context.GetScope();
            
            using (scope)
            {
                cmd.Execute(scope, new TestCommand() { AggregateId = id, Description = DateTime.Now.ToShortTimeString(), });
                scope.Commit();
            }
        }
    }
    
    public class SampleContext1 : SQLiteDomainContext
    {
        public SampleContext1(SQLiteConnection connection, IEventStoreRepository eventStore, IDomainEventBus eventBus) : base(connection, eventStore, eventBus)
        {
        }

        public override ISerializer Serializer
        {
            get
            {
                return new SampleSerializer<EventBase>();
            }
        }

        public override IAggregateRepository<T> AggregateRepository<T>() 
        {
            return new AggregateRepository<T>(this.Serializer, this.EventStore, new EventBus<T>(this));
        }
    }
    
    public class SampleContext2 : SQLiteDomainContext
    {
        public SampleContext2(SQLiteConnection connection, IEventStoreRepository eventStore, IDomainEventBus eventBus) : base(connection, eventStore, eventBus)
        {
        }

        public override IAggregateRepository<T> AggregateRepository<T>() 
        {
            return new SnapshotAggregateRepository<T>(this.GetSnapshotRepository(typeof(T)), new EventBus<T>(this));
        }

        public override ISnapshotRepository GetSnapshotRepository(Type aggregateType)
        {
            // todo: register a Func<Type, ISnapshotRepository> to create the snapshot repository
            
            return new SnapshotRepository<TestSnapshot>(this.Connection);
        }
    }
}

