// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableViewController.cs" company="sgmunn">
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

namespace MonoKit.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using MonoKit.ViewModels;
    using MonoKit.ViewModels.Elements;
    using MonoKit.DataBinding;
    using MonoKit.Reactive;
    using MonoTouch.Foundation;
    using MonoTouch.UIKit;

    public class TableViewController : UITableViewController
    {
        private IViewModel viewModel;

        private bool hasViewAppeared;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.Controls.TableViewController"/> class.
        /// </summary>
        public TableViewController()
        {
            this.Lifetime = new CompositeDisposable();
            this.EventSubscriptions = new CompositeDisposable();
            this.Source = new TableViewSource();
            this.TemplateSelectors = new List<IDataTemplateSelector>();
            this.RegisterDefaultTemplates();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.Controls.TableViewController"/> class.
        /// </summary>
        public TableViewController(UITableViewStyle tableStyle) : base(tableStyle)
        {
            this.Lifetime = new CompositeDisposable();
            this.EventSubscriptions = new CompositeDisposable();
            this.Source = new TableViewSource();
            this.TemplateSelectors = new List<IDataTemplateSelector>();
            this.RegisterDefaultTemplates();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.Controls.TableViewController"/> class.
        /// </summary>
        public TableViewController(UITableViewStyle tableStyle, TableViewSource source) : base(tableStyle)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            this.Lifetime = new CompositeDisposable();
            this.EventSubscriptions = new CompositeDisposable();
            this.Source = source;
            this.TemplateSelectors = new List<IDataTemplateSelector>();
            this.RegisterDefaultTemplates();
        }

        ~TableViewController()
        {
            Console.WriteLine("~TableViewController");
        }

        public IViewModel ViewModel 
        { 
            get
            {
                return this.viewModel;
            }

            set
            {
                if (value != this.viewModel)
                {
                    if (this.viewModel != null)
                    {
                        this.EventSubscriptions.Clear();
                        this.viewModel.Dispose();
                    }

                    this.viewModel = value;
                    this.LoadViewModel();
                }
            }
        } 

        /// <summary>
        /// Gets a lifetime that will dispose objects when the table is no longer visible.
        /// This will also dispose when the view model changes
        /// </summary>
        public CompositeDisposable Lifetime
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the source for the UITableView.
        /// </summary>
        public TableViewSource Source
        {
            get;
            private set;
        }

        public List<IDataTemplateSelector> TemplateSelectors
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Gets a lifetime that will dispose objects when the table is no longer visible.
        /// This will also dispose when the view model changes
        /// </summary>
        protected CompositeDisposable EventSubscriptions
        {
            get;
            private set;
        }

        protected override void Dispose(bool disposing)
        {
            Console.WriteLine("Dispose.TableViewController");
            base.Dispose(disposing);

            if (disposing)
            {
                this.Lifetime.Clear();
                this.EventSubscriptions.Dispose();
                if (this.ViewModel != null)
                {
                    this.ViewModel.Dispose();
                }
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.TableView.Source = this.Source;
            this.Source.ApplyTemplateSelectors(this.TemplateSelectors);
        }

        public override void ViewWillAppear(bool animated)
        {
            this.EventSubscriptions.Clear();
            this.SubscribeToViewModelChanges();

            // this is when the table will first query the source to get sections and rows
            base.ViewWillAppear(animated);
        }

        public override void ViewDidDisappear(bool animated)
        {
            Console.WriteLine("tableview disappeared {0}", this.NavigationController != null);
            this.hasViewAppeared = false;
            base.ViewDidDisappear(animated);

            if (!this.IsInNavigationStack())
            {
                this.EventSubscriptions.Clear();
                var lifetime = this.ViewModel as ILifetime;
                if (lifetime != null)
                {
                    Console.WriteLine("subscriptions cleared");
                    lifetime.Lifetime.Clear();
                }
            }

            this.Lifetime.Clear();
        }
        
        public DataTemplateSelector RegisterTemplate(string reuseId)
        {
            var template = new DataTemplateSelector(reuseId);
            
            this.TemplateSelectors.Add(template);
            return template;
        }

        public void ApplyTemplateSelectors(IList<IDataTemplateSelector> templates)
        {
            this.TemplateSelectors.Clear();
            this.TemplateSelectors.AddRange(templates);

            this.Source.ApplyTemplateSelectors(templates);

            if (this.TableView != null && this.hasViewAppeared)
            {
                this.TableView.ReloadData();
            }
        }

        protected virtual bool IsInNavigationStack()
        {
            return this.NavigationController != null;
        }

        protected virtual UITableViewRowAnimation GetInsertionAnimation(object viewModel)
        {
            return UITableViewRowAnimation.Automatic;
        }

        protected virtual UITableViewRowAnimation GetDeletionAnimation(object viewModel)
        {
            return UITableViewRowAnimation.Automatic;
        }

        protected virtual void LoadViewModel()
        {
            this.Source.Root.Sections.Clear();

            var title = this.ViewModel as ITitle;
            if (title != null && !string.IsNullOrEmpty(title.Title))
            {
                this.Title = title.Title;
            }

            var root = this.ViewModel as ISectionRoot;
            if (root != null)
            {
                this.LoadViewModel(root);
            }
            else
            {
                var section = this.ViewModel as ISection;
                if (section != null)
                {
                    this.LoadViewModel(section);
                }
                else
                {
                    var items = this.ViewModel as IEnumerable;
                    if (items != null)
                    {
                        this.LoadViewModel(items);
                    }
                }
            }
        }
        
        protected virtual void SubscribeToViewModelChanges()
        {
            var root = this.ViewModel as ISectionRoot;
            if (root != null)
            {
                this.SubscribeToViewModelChanges(root);
            }
            else
            {
                var section = this.ViewModel as ISection;
                if (section != null)
                {
                    this.SubscribeToViewModelChanges(section, this.Source.Root[0]);
                }
                else
                {
                    var collectionChangeSource = this.ViewModel as INotifyCollectionChanged;
                    if (collectionChangeSource != null)
                    {
                        this.SubscribeToViewModelChanges(collectionChangeSource, this.Source.Root[0]);
                    }
                }
            }
        }

        private void LoadViewModel(ISectionRoot root)
        {
            // copy data from view model to local source
            foreach (var section in root.Sections)
            {
                this.Source.Root.Sections.Add(new Section(section));
            }
        }

        private void LoadViewModel(ISection section)
        {
            this.Source.Root.Sections.Add(new Section(section));
        }

        private void LoadViewModel(IEnumerable items)
        {
            var proxySection = new Section();
            this.Source.Root.Sections.Add(proxySection);
            
            foreach (var item in items)
            {
                proxySection.Items.Add(item);
            }
        }
        
        private void SubscribeToViewModelChanges(ISectionRoot root)
        {
            // register for changes on the sections collection
            var sectionsCollection = root.Sections as INotifyCollectionChanged;
            if (sectionsCollection != null)
            {
                this.SubscribeToRootChanges(sectionsCollection);
            }
            
            if (root.Sections.Count != this.Source.Root.Sections.Count)
            {
                Console.WriteLine("ViewModel / CollectionView.Root inconsistency -- section counts are different {0}, {1}", root.Sections.Count, this.Source.Root.Sections.Count);
            }
            
            for (int i = 0; i < Math.Min(root.Sections.Count, this.Source.Root.Sections.Count); i++)
            {
                this.SubscribeToItemChanges(root.Sections[i], this.Source.Root.Sections[i]);
            }
        }

        private void SubscribeToViewModelChanges(ISection section, ISection proxySection)
        {
            this.SubscribeToItemChanges(section, proxySection);
        }

        private void SubscribeToViewModelChanges(INotifyCollectionChanged items, ISection proxySection)
        {
            var proxy = new WeakEventWrapper<TableViewController, NotifyCollectionChangedEventArgs>(this, (t,s,o) => { 
                t.HandleItemsCollectionChanged(s, new SectionItemsCollectionChangedEventArgs(proxySection, o)); 
            });
            
            items.CollectionChanged += proxy.HandleEvent;
            this.EventSubscriptions.Add(proxy);
        }

        private void SubscribeToRootChanges(INotifyCollectionChanged collection)
        {
            var proxy = new WeakEventWrapper<TableViewController, NotifyCollectionChangedEventArgs>(this, (t,s,o) => { 
                t.HandleSectionsCollectionChanged(s, o); 
            });
            
            collection.CollectionChanged += proxy.HandleEvent;
            this.EventSubscriptions.Add(proxy);
        }
        
        private void SubscribeToItemChanges(ISection section, ISection proxySection)
        {
            var itemsCollection = section.Items as INotifyCollectionChanged;
            if (itemsCollection != null)
            {
                var proxy = new WeakEventWrapper<TableViewController, NotifyCollectionChangedEventArgs>(this, (t,s,o) => { 
                    t.HandleItemsCollectionChanged(s, new SectionItemsCollectionChangedEventArgs(proxySection, o)); 
                });
                
                itemsCollection.CollectionChanged += proxy.HandleEvent;
                this.EventSubscriptions.Add(proxy);
            }
        }
        
        private void UnsubscribeFromItemChanges(ISection section)
        {
            var itemsCollection = section.Items as INotifyCollectionChanged;
            if (itemsCollection != null)
            {
                // todo: unsubscribe from item changes - we might have to clear and re-subscribe to all
            }
        }

        private void HandleSectionsCollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
        {
            int index = 0;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    
                    index = e.NewStartingIndex;
                    foreach (var section in e.NewItems.OfType<ISection>())
                    {
                        var viewSection = new Section(section);
                        this.Source.Root.Sections.Insert(index++, viewSection);
                        this.SubscribeToItemChanges(section, viewSection);
                    }
                    
                    this.InsertSectionVisual(e.NewStartingIndex, e.NewItems.Count);
                    
                    break;
                    
                case NotifyCollectionChangedAction.Remove:
                    foreach (var section in e.OldItems.OfType<ISection>())
                    {
                        this.Source.Root.Sections.RemoveAt(e.OldStartingIndex);
                        this.UnsubscribeFromItemChanges(section);
                    }
                    
                    this.DeleteSectionVisual(e.OldStartingIndex, e.OldItems.Count);
                    
                    break;
                    
                case NotifyCollectionChangedAction.Replace:
                    // todo: handle replacement of a section in the tableview

                    break;
                    
                case NotifyCollectionChangedAction.Reset:
                    this.EventSubscriptions.Clear();
                    this.LoadViewModel();

                    // todo: only subscribe if we have already
                    this.SubscribeToViewModelChanges();
                    break;
            }
        }
        
        private int FindSectionFromItemsChange(object items)
        {
            int result = 0;
            foreach (var section in this.Source.Root.Sections)
            {
                if (section.Items == items)
                {
                    return result;
                }
                
                result++;
            }
            
            return -1;
        }
        
        private void HandleItemsCollectionChanged (object sender, SectionItemsCollectionChangedEventArgs e)
        {
            this.HandleItemsCollectionChanged(e.Section.Items, e.InnerArgs);
        }
        
        private void HandleItemsCollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
        {
            var sectionIndex = this.FindSectionFromItemsChange(sender);
            
            if (sectionIndex < 0)
            {
                return;
            }

            int index = 0;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    index = e.NewStartingIndex;
                    foreach (var item in e.NewItems)
                    {
                        this.Source.Root[sectionIndex].Items.Insert(index++, item);
                    }
                    
                    this.InsertItemVisual(sectionIndex, e.NewStartingIndex, this.GetInsertionAnimation(e.NewItems[0]), e.NewItems.Count);
                    
                    break;
                    
                case NotifyCollectionChangedAction.Remove:
                    index = e.OldStartingIndex;
                    foreach (var item in e.OldItems)
                    {
                        this.Source.Root[sectionIndex].Items.RemoveAt(index);
                    }
                    
                    this.DeleteItemVisual(sectionIndex, e.OldStartingIndex, this.GetDeletionAnimation(e.OldItems[0]), e.OldItems.Count);
                    
                    break;
                    
                case NotifyCollectionChangedAction.Replace:
                    // todo: replace items in tableview
                    foreach (var item in e.OldItems)
                    {
                    }
                    
                    foreach (var item in e.NewItems)
                    {
                    }
                    
                    break;
                    
                case NotifyCollectionChangedAction.Reset:

                    // todo: reload view


                    this.TableView.ReloadData();
                    break;
            }
        }

        private void InsertItemVisual(int sectionIndex, int idx, UITableViewRowAnimation anim, int count)
        {
            var paths = new NSIndexPath[count];
            for (int i = 0; i < count; i++)
            {
                paths[i] = NSIndexPath.FromRowSection(idx + i, sectionIndex);
            }
            
            this.TableView.InsertRows(paths, anim);
        }
        
        private void DeleteItemVisual(int sectionIndex, int idx, UITableViewRowAnimation anim, int count)
        {
            var paths = new NSIndexPath[count];
            for (int i = 0; i < count; i++)
            {
                paths[i] = NSIndexPath.FromRowSection(idx + i, sectionIndex);
            }
            
            this.TableView.DeleteRows(paths, anim);
        }
        
        private void InsertSectionVisual(int idx, int count)
        {
            this.TableView.InsertSections(NSIndexSet.FromNSRange(new NSRange(idx, count)), UITableViewRowAnimation.Automatic);
        }
        
        private void DeleteSectionVisual(int idx, int count)
        {
            this.TableView.DeleteSections(NSIndexSet.FromNSRange(new NSRange(idx, count)), UITableViewRowAnimation.Automatic);
        }

        private void RegisterDefaultTemplates()
        {
            this.RegisterTemplate("TableViewCell_ISectionRoot")
                .Creates((id) => new TableViewCell(UITableViewCellStyle.Default, id))
                    .WhenInitializing<TableViewCell>((view) => view.Accessory = UITableViewCellAccessory.DisclosureIndicator)
                    .WhenBinding<ISectionRoot, TableViewCell>((vm, view) => view.Text = vm.ToString());

            this.RegisterTemplate("TableViewCell_INavigate")
                .Creates((id) => new TableViewCell(UITableViewCellStyle.Default, id))
                    .WhenInitializing<TableViewCell>((view) => view.Accessory = UITableViewCellAccessory.DisclosureIndicator)
                    .WhenBinding<INavigate, TableViewCell>((vm, view) => view.Text = vm.ToString());
            
            this.RegisterTemplate("CheckboxTableViewCell")
                .Creates((id) => new CheckboxTableViewCell(UITableViewCellStyle.Default, id))
                    .WhenBinding<LabelledCheckboxElement, CheckboxTableViewCell>((vm, view) => {
                        view.AddBinding(view, "Text", vm, "Text");
                        view.AddBinding(view, "IsChecked", vm, "Value");
                    });

            this.RegisterTemplate("BooleanTableViewCell")
                .Creates((id) => new BooleanTableViewCell(UITableViewCellStyle.Default, id))
                    .WhenBinding<LabelledBooleanElement, BooleanTableViewCell>((vm, view) => {
                        view.AddBinding(view, "Text", vm, "Text");
                        view.AddBinding(view, "IsChecked", vm, "Value");
                    });

            this.RegisterTemplate("StringInputTableViewCell")
                .Creates((id) => new StringInputTableViewCell(UITableViewCellStyle.Default, id))
                    .WhenInitializing<StringInputTableViewCell>((view) => {
                        new TextInputBehaviour(view);
                    })
                    .WhenBinding<LabelledStringInputElement, StringInputTableViewCell>((vm, view) => {
                        view.AddBinding(view, "Text", vm, "Text");
                        view.AddBinding(view, "InputValue", vm, "Value");
                    });

            this.RegisterTemplate("DateInputTableViewCell")
                .Creates((id) => new DateInputTableViewCell(UITableViewCellStyle.Default, id))
                    .WhenBinding<LabelledDateTimeInputElement, DateInputTableViewCell>((vm, view) => {
                        view.AddBinding(view, "Text", vm, "Text");
                        view.AddBinding(view, "InputValue", vm, "Value");
                    });

            this.RegisterTemplate("DecimalInputTableViewCell")
                .Creates((id) => new DecimalInputTableViewCell(UITableViewCellStyle.Default, id))
                    .WhenBinding<LabelledDecimalInputElement, DecimalInputTableViewCell>((vm, view) => {
                        view.AddBinding(view, "Text", vm, "Text");
                        view.AddBinding(view, "InputValue", vm, "Value");
                    });
        }
    }
}

