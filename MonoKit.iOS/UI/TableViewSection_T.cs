// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableViewSection_T.cs" company="sgmunn">
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

namespace MonoKit.UI
{
    using System;
    using System.Collections;
    using MonoTouch.UIKit;
    using System.Collections.Generic;
    using MonoTouch.Foundation;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using MonoKit.DataBinding;
    using MonoKit.Interactivity;
    
    public class TableViewSection<TItem> : TableViewSectionBase, IEnumerable<TItem>, INotifyCollectionChanged where TItem : IDisposable
    {
        private int notifyCollectionChanges;
        
        private readonly List<TItem> items;
        
        public TableViewSection(TableViewSource source) : base(source)
        {
            this.items = new List<TItem>();
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.UI.TableViewSection`1"/> class.
        /// </summary>
        /// <param name='viewDefinitions'>
        /// View declarations.   more specific definitions should be before more general ones
        /// </param>
        public TableViewSection(TableViewSource source, params IViewDefinition[] viewDefinitions) : base(source, viewDefinitions)
        {
            this.items = new List<TItem>();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private List<TItem> Items
        {
            get
            {
                return this.items;
            }
        }
        
        public void BeginUpdate()
        {
            this.notifyCollectionChanges++;
        }
        
        public void EndUpdate()
        {
            this.notifyCollectionChanges--;
            
            if (this.notifyCollectionChanges <= 0)
            {
                this.notifyCollectionChanges = 0;
                
                var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                this.OnCollectionChanged(args);
            }
        }

        public override void Clear()
        {
            foreach (var item in this.Items)
            {
                item.Dispose();
            }

            this.Items.Clear();
            
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            this.OnCollectionChanged(args);
        }
        
        public void Add(TItem item)
        {
            this.Items.Add(item);   
            
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item,this.Items.Count -1);
            this.OnCollectionChanged(args);
        }
        
        public void AddRange(IEnumerable<TItem> items)
        {
            var index = this.Items.Count;
            var newItems = new List<TItem>(items);

            this.Items.AddRange(newItems);   
            
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems, index);
            this.OnCollectionChanged(args);
        }
        
        public int IndexOf(TItem item)
        {
            return this.Items.IndexOf(item);
        }
        
        public void Insert(int index, TItem item)
        {
            this.Items.Insert(index, item);
            
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index);
            this.OnCollectionChanged(args);
        }
        
        public void Remove(TItem item)
        {
            var index = this.Items.IndexOf(item);
            this.Items.Remove(item);
            
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index);
            this.OnCollectionChanged(args);
        }
        
        public void RemoveAt(int index)
        {
            var item = this.Items[index];
            this.Items.RemoveAt(index);
            
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index);
            this.OnCollectionChanged(args);
        }
        
        public void RemoveRange(int index, int count)
        {
            var items = this.Items.GetRange(index, count);
            this.Items.RemoveRange(index, count);
            
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items, index);
            this.OnCollectionChanged(args);
        }
        
        public override UITableViewCell GetCell(int row)
        {
            return this.GetViewForObject(this.Items[row]);
        }
        
        public override void RowSelected(NSIndexPath indexPath)
        {
            this.Source.TableView.DeselectRow(indexPath, true);

            var item = this.Items[indexPath.Row];
            
            var command = item as ICommand;
            if (command != null && command.GetCanExecute())
            {
                command.Execute();                
            }
        }
        
        public override bool CanEditRow(NSIndexPath indexPath)
        {
            var item = this.Items[indexPath.Row];
            
            var edit = item as IEdit;
            if (edit != null)
            {
                return edit.GetCanEdit();                
            }

            return true;
        }
        
        public override void EditRow(UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
        {
            var item = this.Items[indexPath.Row];
            
            var edit = item as IEdit;
            if (edit != null)
            {
                edit.ExecuteEdit();                
            }
            
            switch (editingStyle)
            {
                case UITableViewCellEditingStyle.Delete:
                    this.notifyCollectionChanges++;
                    this.Items.Remove(item);
                    this.notifyCollectionChanges--;
                    break;
            }
        }
        
                        
        public override void MoveRow(NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
        {
            var item = this.Items[sourceIndexPath.Row];
            
            // tell item that it has new index
            
        }

        public override float GetHeightForRow(NSIndexPath indexPath)
        {
            // todo: get the height for a given item in a more efficient manner
            
            //var item = this.GetElementAtIndex(indexPath.Row);
            //var info = this.GetViewInfoForElement(item);
            //if (info != null)
            //{
            //    return info.RowHeight;
            //}
            
            return -1;
        }
        
        public override IEnumerator GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }
                
        protected override int GetCount()
        {
            return this.Items.Count;
        }

        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (this.notifyCollectionChanges == 0)
            {
                var ev = this.CollectionChanged;
                if (ev != null)
                {
                    ev(this, args);
                }
            }
        }
        
        IEnumerator<TItem> System.Collections.Generic.IEnumerable<TItem>.GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }
    }
}
