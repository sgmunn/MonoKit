//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="CollectionViewController.cs" company="sgmunn">
//    (c) sgmunn 2012  
//
//    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//    documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//    the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//    to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//    The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//    the Software.
//
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//    THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//    CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//    IN THE SOFTWARE.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------

namespace MonoKit.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Drawing;
    using System.Linq;
    using MonoKit.ViewModels;
    using MonoKit.ViewModels.Elements;
    using MonoKit.DataBinding;
    using MonoKit.Reactive;
    using MonoTouch.Foundation;
    using MonoTouch.UIKit;

    // Issues - both appear to be an issue with the native control and not MT
    // - when adding an item and there is an empty section (possibly at the end), then we get an assertion failure
    // - when adding a section, we get an assertion failure.  seems to work when there is an empty section at the end.


    // need to test with tableviews.  set up in a similar manner and test adding / removing 


    public class SectionItemsCollectionChangedEventArgs : NotifyCollectionChangedEventArgs
    {
        public SectionItemsCollectionChangedEventArgs(ISection section, NotifyCollectionChangedEventArgs args) 
            : base(NotifyCollectionChangedAction.Reset)
        {
            this.Section = section;
            this.InnerArgs = args;
        }

        public ISection Section { get; private set; }

        public NotifyCollectionChangedEventArgs InnerArgs { get; private set; }
    }

    public class CollectionViewController : UICollectionViewController
    {
        public static readonly string SupplementalViewKey = "SupplementalViewKey";

        private IViewModel viewModel;
        
        private bool isTableSourceDirectlyBoundToViewModel;
        
        private bool hasViewAppeared;

        public CollectionViewController() : base(new UICollectionViewFlowLayout() {HeaderReferenceSize = new SizeF(100, 20), FooterReferenceSize = new SizeF(100, 20)})
        {
            this.Root = new SectionRoot();

            this.Lifetime = new CompositeDisposable();
            this.TemplateSelectors = new List<IDataTemplateSelector>();
            this.RegisterDefaultTemplates();
        }

        public CollectionViewController(UICollectionViewLayout layout) : base(layout)
        {
            this.Root = new SectionRoot();

            this.Lifetime = new CompositeDisposable();
            this.TemplateSelectors = new List<IDataTemplateSelector>();
            this.RegisterDefaultTemplates();
        }
        
        ~CollectionViewController()
        {
            Console.WriteLine("~CollectionViewController");
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
        
        public List<IDataTemplateSelector> TemplateSelectors
        {
            get;
            private set;
        }
        
        protected ISectionRoot Root 
        { 
            get; 
            private set; 
        } 

        protected override void Dispose(bool disposing)
        {
            Console.WriteLine("Dispose.CollectionViewController");
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

        public override void LoadView()
        {
            base.LoadView();
            this.CollectionView.BackgroundColor = UIColor.White;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            this.hasViewAppeared = true;
            this.SubscribeToViewModelChanges();

            this.RegisterTemplatesWithView();

            base.ViewWillAppear(animated);
        }

        public override void ViewDidDisappear(bool animated)
        {
            Console.WriteLine("collectionview disappeared {0}", this.NavigationController != null);
            base.ViewDidDisappear(animated);
            
            this.Lifetime.Clear();
            var lifetime = this.ViewModel as ILifetime;
            if (lifetime != null)
            {
                Console.WriteLine("lifetime cleared");
                lifetime.Lifetime.Clear();
            }
        }

        
        public override int NumberOfSections(UICollectionView collectionView)
        {
            Console.WriteLine("section count");
            return this.Root.Sections.Count;
        }

        public override int GetItemsCount(UICollectionView collectionView, int section)
        {
            Console.WriteLine("item count {0}", section);
            return this.Root[section].Items.Count;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            Console.WriteLine(">>get cell");
            var viewModel = this.Root[indexPath.Section].Items[indexPath.Row];
            var template = this.GetTemplate(viewModel);
            if (template != null)
            {
                var cell = (UICollectionViewCell)collectionView.DequeueReusableCell(new NSString(template.ReuseIdentifier), indexPath);
                template.InitializeView(cell);
                template.BindViewModel(viewModel, cell);
                
                Console.WriteLine("<<get cell");
                return cell;
            }
            else
            {
                var cell = (UICollectionViewCell)collectionView.DequeueReusableCell(new NSString("Default"), indexPath);
                cell.BackgroundColor = UIColor.Gray;

                Console.WriteLine("<<get cell");
                return cell;
            }
        }

        public override UICollectionReusableView GetViewForSupplementaryElement(UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
        {
            Console.WriteLine(">>get view");
            var kind = UICollectionElementKindSection.Header;
            object viewModel = null;

            if (elementKind == UICollectionElementKindSectionKey.Header)
            {
                kind = UICollectionElementKindSection.Header;
                viewModel = this.Root[indexPath.Section].Header;
            }
            else
            {
                kind = UICollectionElementKindSection.Footer;
                viewModel = this.Root[indexPath.Section].Footer;
            }

            var template = this.GetTemplate(viewModel, kind);
            if (template != null)
            {
                var cell = (UICollectionReusableView)collectionView.DequeueReusableSupplementaryView(elementKind, new NSString(template.ReuseIdentifier), indexPath);
                template.InitializeView(cell);
                template.BindViewModel(viewModel, cell);
                
                Console.WriteLine("<<get view");
                return cell;
            }
            else
            {
                var cell = (UICollectionReusableView)collectionView.DequeueReusableSupplementaryView(elementKind, new NSString("Default"), indexPath);
                cell.BackgroundColor = UIColor.Brown;
                
                Console.WriteLine("<<get view");
                return cell;
            }
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var item = this.GetViewModelForIndexPath(indexPath) as ICommand;
            
            if (item != null && item.GetCanExecute())
            {
                item.Execute();
            }

         //   base.ItemSelected(collectionView, indexPath);
        }

//        public override void PerformAction(UICollectionView collectionView, MonoTouch.ObjCRuntime.Selector action, NSIndexPath indexPath, NSObject sender)
//        {
//            base.PerformAction(collectionView, action, indexPath, sender);
//        }
        
        public DataTemplateSelector RegisterTemplate(string reuseId)
        {
            var template = new DataTemplateSelector(reuseId);
            
            this.TemplateSelectors.Add(template);
            return template;
        }
        
        public Type QueryGetCellType(int section, int row)
        {
            var viewModel = this.Root[section].Items[row];
            
            var template = this.GetTemplate(viewModel); 
            
            if (template != null)
            {
                return template.ViewType;
            }
            
            return typeof(UICollectionView);
        }

        protected object GetViewModelForIndexPath(NSIndexPath indexPath)
        {
            return this.Root[indexPath.Section].Items[indexPath.Row];
        }
        
        protected virtual IDataTemplateSelector GetTemplate(object viewModel)
        {
            IDataTemplateSelector exactMatch = null;
            IDataTemplateSelector semiMatch = null;
            
            // keep looping even if we find one, match on last
            foreach (var template in this.TemplateSelectors)
            {
                switch (template.AppliesToViewModel(viewModel))
                {
                    case TemplateMatch.Exact:
                        exactMatch = template;
                        break;
                        
                    case TemplateMatch.Assignable:
                        semiMatch = template;
                        break;
                }
            }
            
            return exactMatch ?? semiMatch;
        }
        
        protected virtual IDataTemplateSelector GetTemplate(object viewModel, UICollectionElementKindSection elementKind)
        {
            IDataTemplateSelector exactMatch = null;
            IDataTemplateSelector semiMatch = null;
            
            // keep looping even if we find one, match on last
            foreach (var template in this.TemplateSelectors)
            {
                var suppViewKind = template[CollectionViewController.SupplementalViewKey];
                if (suppViewKind != null && (UICollectionElementKindSection)suppViewKind == elementKind)
                {
                    switch (template.AppliesToViewModel(viewModel))
                    {
                        case TemplateMatch.Exact:
                            exactMatch = template;
                            break;
                            
                        case TemplateMatch.Assignable:
                            semiMatch = template;
                            break;
                    }
                }
            }
            
            return exactMatch ?? semiMatch;
        }

        protected virtual void ApplyViewModelToCollectionSource()
        {
//            this.isTableSourceDirectlyBoundToViewModel = false;
//            
//            var root = this.ViewModel as ISectionRoot;
//            if (root != null)
//            {
//                // Source points to the same collection as the view model, thus we do not need to
//                // update source when the collection changes, just the tableview
//                this.isTableSourceDirectlyBoundToViewModel = true;
//                
//                var sectionsCollection = root.Sections as INotifyCollectionChanged;
//                if (sectionsCollection != null)
//                {
//                    this.AttachToSectionsChanged(sectionsCollection);
//                }
//                
//                foreach (var section in root.Sections)
//                {
//                    this.Root.Sections.Add(section);
//                    this.SubscribeToSectionItemChanges(section);
//                }
//            }
//            else
//            {
//                // handle the case where we are just passed a single section
//                var section = this.ViewModel as ISection;
//                if (section != null)
//                {
//                    this.isTableSourceDirectlyBoundToViewModel = true;
//                    
//                    this.Root.Sections.Add(section);
//                    this.SubscribeToSectionItemChanges(section);
//                }
//                else
//                {
//                    // support generic items if the view model supports INotifyCollectionChanged
//                    // source is not directly bound to the collection and we'll need to keep source in sync
//                    var collectionChangeSource = this.ViewModel as INotifyCollectionChanged;
//                    if (collectionChangeSource != null)
//                    {
//                        this.AttachToSectionsChanged(collectionChangeSource);
//                    }
//                    
//                    var items = this.ViewModel as IEnumerable;
//                    if (items != null)
//                    {
//                        var proxySection = new SectionViewModel();
//                        this.Root.Sections.Add(proxySection);
//                        
//                        foreach (var item in items)
//                        {
//                            proxySection.Items.Add(item);
//                        }
//                    }
//                }
//            }

        }
        
        /// <summary>
        /// Loads the view model into Root
        /// </summary>
        protected virtual void LoadViewModel()
        {
            this.Root.Sections.Clear();
            
            var root = this.ViewModel as ISectionRoot;
            if (root != null)
            {
                this.LoadViewModel(root);
            }
        }
        
        protected virtual void SubscribeToViewModelChanges()
        {
            var root = this.ViewModel as ISectionRoot;
            if (root != null)
            {
                this.SubscribeToViewModelChanges(root);
            }
        }

        private void LoadViewModel(ISectionRoot root)
        {
            // copy data from view model to local source
            foreach (var section in root.Sections)
            {
                var viewSection = new Section();
                this.Root.Sections.Add(viewSection);

                foreach (var item in section.Items)
                {
                    viewSection.Items.Add(item);
                }
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

            if (root.Sections.Count != this.Root.Sections.Count)
            {
                Console.WriteLine("ViewModel / CollectionView.Root inconsistency -- section counts are different {0}, {1}", root.Sections.Count, this.Root.Sections.Count);
            }

            for (int i = 0; i < Math.Min(root.Sections.Count, this.Root.Sections.Count); i++)
            {
                this.SubscribeToItemChanges(root.Sections[i], this.Root.Sections[i]);
            }
        }
        
        private void SubscribeToRootChanges(INotifyCollectionChanged collection)
        {
            var proxy = new WeakEventWrapper<CollectionViewController, NotifyCollectionChangedEventArgs>(this, (t,s,o) => { 
                t.HandleSectionsCollectionChanged(s, o); 
            });

            collection.CollectionChanged += proxy.HandleEvent;
            this.Lifetime.Add(proxy);
        }
        
        private void SubscribeToItemChanges(ISection section, ISection proxySection)
        {
            var itemsCollection = section.Items as INotifyCollectionChanged;
            if (itemsCollection != null)
            {
                var proxy = new WeakEventWrapper<CollectionViewController, NotifyCollectionChangedEventArgs>(this, (t,s,o) => { 
                    t.HandleItemsCollectionChanged(s, new SectionItemsCollectionChangedEventArgs(proxySection, o)); 
                });

                itemsCollection.CollectionChanged += proxy.HandleEvent;
                this.Lifetime.Add(proxy);
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
                        this.Root.Sections.Insert(index++, section);
//xxxx                        this.SubscribeToItemChanges(section);
                    }

                    this.InsertSectionVisual(e.NewStartingIndex, e.NewItems.Count);
                    
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
                    this.ApplyViewModelToCollectionSource();
                    break;
            }
        }
        
        private int FindSectionFromItemsChange(object items)
        {
            int result = 0;
            foreach (var section in this.Root.Sections)
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
            
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    // add items to source if we didn't bind directly to the view model's list
                    //if (!this.isTableSourceDirectlyBoundToViewModel)
                    {
                    Console.WriteLine("addd to root {0}", sender);
                        int i = e.NewStartingIndex;
                        foreach (var item in e.NewItems)
                        {
                            this.Root[sectionIndex].Items.Insert(i++, item);
                        }
                    }
                    
                    this.InsertItemVisual(sectionIndex, e.NewStartingIndex, e.NewItems.Count);
                    
                    break;
                    
                case NotifyCollectionChangedAction.Remove:
                    //if (!this.isTableSourceDirectlyBoundToViewModel)
                    {
                        int i = e.OldStartingIndex;
                        foreach (var item in e.OldItems)
                        {
                            this.Root[sectionIndex].Items.RemoveAt(i++);
                        }
                    }
                    
                    this.DeleteItemVisual(sectionIndex, e.OldStartingIndex, e.OldItems.Count);
                    
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
                    this.CollectionView.ReloadData();
                    break;
            }
        }
        
        private void SubscribeToRootSectionsChanges(INotifyCollectionChanged collection)
        {
            var proxy = new WeakEventWrapper<CollectionViewController, NotifyCollectionChangedEventArgs>(this, (t,s,o) => { t.HandleSectionsCollectionChanged(s, o); });
            collection.CollectionChanged += proxy.HandleEvent;
            this.Lifetime.Add(proxy);
        }
        
        private void SubscribeToSectionItemChanges(ISection section)
        {
            var itemsCollection = section.Items as INotifyCollectionChanged;
            if (itemsCollection != null)
            {
                var proxy = new WeakEventWrapper<CollectionViewController, NotifyCollectionChangedEventArgs>(this, (t,s,o) => { t.HandleItemsCollectionChanged(s, o); });
                itemsCollection.CollectionChanged += proxy.HandleEvent;
                this.Lifetime.Add(proxy);
            }
        }
        
        private void InsertItemVisual(int sectionIndex, int idx, int count)
        {
            var paths = new NSIndexPath[count];
            for (int i = 0; i < count; i++)
            {
                paths[i] = NSIndexPath.FromRowSection(idx + i, sectionIndex);
            }
            
            Console.WriteLine("insert visual");
            this.CollectionView.InsertItems(paths);
        }
        
        private void DeleteItemVisual(int sectionIndex, int idx, int count)
        {
            var paths = new NSIndexPath[count];
            for (int i = 0; i < count; i++)
            {
                paths[i] = NSIndexPath.FromRowSection(idx + i, sectionIndex);
            }
            
            this.CollectionView.DeleteItems(paths);
        }
        
        private void InsertSectionVisual(int idx, int count)
        {
            this.CollectionView.InsertSections(NSIndexSet.FromNSRange(new NSRange(idx, count)));
        }

        private void RegisterTemplatesWithView()
        {
            this.CollectionView.RegisterClassForCell(typeof(UICollectionViewCell), new NSString("Default"));
            this.CollectionView.RegisterClassForSupplementaryView(typeof(UICollectionReusableView), UICollectionElementKindSection.Header, new NSString("Default"));
            this.CollectionView.RegisterClassForSupplementaryView(typeof(UICollectionReusableView), UICollectionElementKindSection.Footer, new NSString("Default"));

            foreach (var template in this.TemplateSelectors)
            {
                var supplementalViewKind = template[CollectionViewController.SupplementalViewKey];
                if (supplementalViewKind != null)
                {
                    this.CollectionView.RegisterClassForSupplementaryView(template.ViewType, (UICollectionElementKindSection)supplementalViewKind, new NSString(template.ReuseIdentifier));
                }
                else
                {
                    this.CollectionView.RegisterClassForCell(template.ViewType, new NSString(template.ReuseIdentifier));
                }
            }
        }

        private void RegisterDefaultTemplates()
        {
        }

    }
    
}
