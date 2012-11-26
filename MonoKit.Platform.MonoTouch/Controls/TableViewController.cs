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
    using MonoKit.Runtime;
    using MonoTouch.Foundation;
    using MonoTouch.UIKit;


    // todo: bind to property changes of things like headers ???????

    public class TableViewController : UITableViewController
    {
        private IViewModel viewModel;

        private bool isTableSourceDirectlyBoundToViewModel;

        private bool hasViewAppeared;

        public TableViewController()
        {
            this.Lifetime = new CompositeDisposable();
            this.Source = new TableViewSource(this);
            this.TemplateSelectors = new List<IDataTemplateSelector>();
            this.RegisterDefaultTemplates();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.Controls.TableViewController"/> class.
        /// </summary>
        public TableViewController(UITableViewStyle tableStyle) : base(tableStyle)
        {
            this.Lifetime = new CompositeDisposable();
            this.Source = new TableViewSource(this);
            this.TemplateSelectors = new List<IDataTemplateSelector>();
            this.RegisterDefaultTemplates();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.Controls.TableViewController"/> class.
        /// </summary>
        public TableViewController(UITableViewStyle tableStyle, bool variableHeightRows) : base(tableStyle)
        {
            this.Lifetime = new CompositeDisposable();
            this.Source = variableHeightRows ? new SizingTableViewSource(this) : new TableViewSource(this);
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
                        this.Lifetime.Clear();
                        this.viewModel.Dispose();
                    }

                    this.viewModel = value;

                    if (this.hasViewAppeared)
                    {
                        this.ApplyViewModelToTableSource();
                    }
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

        protected override void Dispose(bool disposing)
        {
            Console.WriteLine("Dispose.TableViewController");
            base.Dispose(disposing);

            if (disposing)
            {
                this.Lifetime.Dispose();
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
        }

        public override void ViewWillAppear(bool animated)
        {
            this.hasViewAppeared = true;
            this.ApplyViewModelToTableSource();
            // this is when the table will first query the source to get sections and rows
            base.ViewWillAppear(animated);
        }

        public override void ViewDidDisappear(bool animated)
        {
            Console.WriteLine("tableview disappeared {0}", this.NavigationController != null);
            this.hasViewAppeared = false;
            base.ViewDidDisappear(animated);

            this.Lifetime.Clear();
            var lifetime = this.ViewModel as ILifetime;
            if (lifetime != null)
            {
                Console.WriteLine("lifetime cleared");
                lifetime.Lifetime.Clear();
            }
        }
        
        public DataTemplateSelector RegisterTemplate(string reuseId)
        {
            var template = new DataTemplateSelector(reuseId);
            
            this.TemplateSelectors.Add(template);
            return template;
        }

        public void ApplyTemplateSelectors(IList<IDataTemplateSelector> templates)
        {
            // todo: tweak this and check the templates for anything that defines a height func, then
            // we can automatically create a sizing source and we don't have to remember to do that bit
            // if we change the source we'll need to make sure we clear the current source of any data
            this.TemplateSelectors.Clear();
            this.TemplateSelectors.AddRange(templates);

            if (this.TableView != null && this.hasViewAppeared)
            {
                this.TableView.ReloadData();
            }
        }

        protected virtual UITableViewRowAnimation GetInsertionAnimation(object viewModel)
        {
            return UITableViewRowAnimation.Automatic;
        }

        protected virtual UITableViewRowAnimation GetDeletionAnimation(object viewModel)
        {
            return UITableViewRowAnimation.Automatic;
        }

        /// <summary>
        /// Applies the view model and converts the view model into a collection of ISectionRoot
        /// </summary>
        protected virtual void ApplyViewModelToTableSource()
        {
            this.Source.Clear();
            this.isTableSourceDirectlyBoundToViewModel = false;

            var root = this.ViewModel as ISectionRoot;
            if (root != null)
            {
                // Source points to the same collection as the view model, thus we do not need to
                // update source when the collection changes, just the tableview
                this.isTableSourceDirectlyBoundToViewModel = true;

                var sectionsCollection = root.Sections as INotifyCollectionChanged;
                if (sectionsCollection != null)
                {
                    this.AttachToSectionsChanged(sectionsCollection);
                }

                foreach (var section in root.Sections)
                {
                    this.Source.Root.Sections.Add(section);
                    this.SubscribeToSectionItemChanges(section);
                }
            }
            else
            {
                // handle the case where we are just passed a single section
                var section = this.ViewModel as ISection;
                if (section != null)
                {
                    this.isTableSourceDirectlyBoundToViewModel = true;

                    this.Source.Root.Sections.Add(section);
                    this.SubscribeToSectionItemChanges(section);
                }
                else
                {
                    // support generic items if the view model supports INotifyCollectionChanged
                    // source is not directly bound to the collection and we'll need to keep source in sync
                    var collectionChangeSource = this.ViewModel as INotifyCollectionChanged;
                    if (collectionChangeSource != null)
                    {
                        this.AttachToSectionsChanged(collectionChangeSource);
                    }

                    var items = this.ViewModel as IEnumerable;
                    if (items != null)
                    {
                        var proxySection = new SectionViewModel();
                        this.Source.Root.Sections.Add(proxySection);

                        foreach (var item in items)
                        {
                            proxySection.Items.Add(item);
                        }
                    }
                }
            }
        }

        private void HandleSectionsCollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var section in e.NewItems.OfType<ISection>())
                    {
                        this.SubscribeToSectionItemChanges(section);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    //foreach (var section in e.OldItems.OfType<ISection>())
                    //{
                    //    this.UnsubscribeToSectionItemChanges(section);
                    //}
                    
                    break;

                case NotifyCollectionChangedAction.Replace:
                    //foreach (var section in e.OldItems.OfType<ISection>())
                    //{
                    //    this.UnsubscribeToSectionItemChanges(section);
                    //}
                    
                    foreach (var section in e.NewItems.OfType<ISection>())
                    {
                        this.SubscribeToSectionItemChanges(section);
                    }

                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.Lifetime.Clear();
                    this.ApplyViewModelToTableSource();
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

        private void HandleItemsCollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
        {
            var sectionIndex = this.FindSectionFromItemsChange(sender);

            if (sectionIndex < 0)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    // add items to source if we didn't bind directly to the view model's list
                    if (!this.isTableSourceDirectlyBoundToViewModel)
                    {
                        int i = e.NewStartingIndex;
                        foreach (var item in e.NewItems)
                        {
                            this.Source.Root[sectionIndex].Items.Insert(i++, item);
                        }
                    }

                    this.InsertVisual(sectionIndex, e.NewStartingIndex, this.GetInsertionAnimation(e.NewItems[0]), e.NewItems.Count);

                    break;
                    
                case NotifyCollectionChangedAction.Remove:
                    if (!this.isTableSourceDirectlyBoundToViewModel)
                    {
                        int i = e.OldStartingIndex;
                        foreach (var item in e.OldItems)
                        {
                            this.Source.Root[sectionIndex].Items.RemoveAt(i++);
                        }
                    }

                    this.DeleteVisual(sectionIndex, e.OldStartingIndex, this.GetDeletionAnimation(e.OldItems[0]), e.OldItems.Count);

                    break;
                    
                case NotifyCollectionChangedAction.Replace:
                    foreach (var item in e.OldItems)
                    {
                    }
                    
                    foreach (var item in e.NewItems)
                    {
                    }
                    
                    break;
                    
                case NotifyCollectionChangedAction.Reset:
                    this.TableView.ReloadData();
                    break;
            }
        }
        
        private void AttachToSectionsChanged(INotifyCollectionChanged collection)
        {
            var proxy = new WeakEventWrapper<TableViewController, NotifyCollectionChangedEventArgs>(this, (t,s,o) => { t.HandleSectionsCollectionChanged(s, o); });
            collection.CollectionChanged += proxy.HandleEvent;
            this.Lifetime.Add(proxy);
        }

        private void SubscribeToSectionItemChanges(ISection section)
        {
            var itemsCollection = section.Items as INotifyCollectionChanged;
            if (itemsCollection != null)
            {
                var proxy = new WeakEventWrapper<TableViewController, NotifyCollectionChangedEventArgs>(this, (t,s,o) => { t.HandleItemsCollectionChanged(s, o); });
                itemsCollection.CollectionChanged += proxy.HandleEvent;
                this.Lifetime.Add(proxy);
            }
        }
        
        private void InsertVisual(int sectionIndex, int idx, UITableViewRowAnimation anim, int count)
        {
            var paths = new NSIndexPath[count];
            for (int i = 0; i < count; i++)
            {
                paths[i] = NSIndexPath.FromRowSection(idx + i, sectionIndex);
            }
            
            this.TableView.InsertRows(paths, anim);
        }
        
        private void DeleteVisual(int sectionIndex, int idx, UITableViewRowAnimation anim, int count)
        {
            var paths = new NSIndexPath[count];
            for (int i = 0; i < count; i++)
            {
                paths[i] = NSIndexPath.FromRowSection(idx + i, sectionIndex);
            }
            
            this.TableView.DeleteRows(paths, anim);
        }

        private void RegisterDefaultTemplates()
        {
            // next issue is to get two way binding between the view and the view model
            // IBindingScope
            // and then we need to clear bindings when (a) view disappeared ?? or on dispose
            // who is the correct binding scope ?? the cell is, ......
            // IPropertyInjection
            // dispose the bindings and injected properties
            // why do we have scope and target as different objects ?
            // 









            this.RegisterTemplate("TableViewCell")
                .Creates((id) => new TableViewCell(UITableViewCellStyle.Default, id))
                    .HavingHeight(65f)
                    .WhenBinding<string, TableViewCell>((vm, view) => view.Text = vm);

            this.RegisterTemplate("TableViewCell")
                .Creates((id) => new TableViewCell(UITableViewCellStyle.Default, id))
                    .WhenInitializing<TableViewCell>((view) => view.Accessory = UITableViewCellAccessory.DisclosureIndicator)
                    .WhenBinding<ISectionRoot, TableViewCell>((vm, view) => view.Text = vm.ToString());

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

            // behaviours -- base template selector or view construction ??
            // probably easiest if view construction which mirros xaml


            // animation - need additional properties, or just override the tableviewcontroller should you need it

        }
    }
}

