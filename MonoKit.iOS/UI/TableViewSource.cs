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

namespace MonoKit.UI
{
    using System;
    using MonoTouch.UIKit;
    using MonoTouch.Foundation;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    
    public class TableViewSource : UITableViewSource, IEnumerable<TableViewSectionBase>
    {
        private UITableView tableView;
        
        private readonly List<TableViewSectionBase> sections;

        public TableViewSource()
        {
            this.sections = new List<TableViewSectionBase>();
        }

        public UITableView TableView
        {
            get 
            { 
                return this.tableView; 
            }
            
            set 
            { 
                if (value != this.tableView)
                {
                    if (this.tableView != null)
                    {
                        this.tableView.Source = null;
                    }
                    
                    this.tableView = value; 
                    
                    if (this.tableView != null)
                    {
                        this.tableView.Source = this;
                    }
                }
            }
        }
        
        public int Count
        {
            get
            {
                return this.Sections.Count;
            }
        }
        
        private List<TableViewSectionBase> Sections
        {
            get
            {
                return this.sections;
            }
        }
        
        public void ClearData()
        {
            foreach (var section in this.Sections) 
            {
                section.Clear();
            }
        }
        
        internal void Add(TableViewSectionBase section)
        {
            if (section.Source != this)
            {
                throw new InvalidOperationException("Cannot add an TableViewSectionBase to a TableViewSource that is owned by another source");
            }
            
            this.Sections.Add(section);
            
            var notifyCollection = section as INotifyCollectionChanged;
            if (notifyCollection != null)
            {
                notifyCollection.CollectionChanged -= HandleCollectionChanged;
                notifyCollection.CollectionChanged += HandleCollectionChanged;
            }
        }
        
        public TableViewSectionBase SectionAt(int index)
        {
            return this.Sections[index];
        }

        public IEnumerator<TableViewSectionBase> GetEnumerator()
        {
            return this.sections.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.sections.GetEnumerator();
        }

               
        public void Reload()
        {
            if (this.TableView != null)
            {
                this.TableView.ReloadData();
            }
        }       

        public override int RowsInSection(UITableView tableview, int section)
        {
            var s = this.Sections[section];
            var count = s.Count;
            
            return count;
        }

        public override int NumberOfSections(UITableView tableView)
        {
            return this.Sections.Count;
        }
        
        public override string TitleForHeader(UITableView tableView, int section)
        {
            return this.Sections[section].Header;
        }

        public override string TitleForFooter(UITableView tableView, int section)
        {
            return this.Sections[section].Footer;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var section = this.Sections[indexPath.Section];
        
            return section.GetCell(indexPath.Row);            
        }

        //public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        //{
            //if (Root.NeedColorUpdate)
            //{
            //    var section = Root.Sections[indexPath.Section];
            //    var element = section.Elements[indexPath.Row];
            //    var colorized = element as IColorizeBackground;
            //    if (colorized != null)
            //        colorized.WillDisplay(tableView, cell, indexPath);
            //}
        //}

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var section = this.Sections [indexPath.Section];
            section.RowSelected(indexPath);
        }
        
        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            var section = this.Sections [indexPath.Section];
            return section.CanEditRow(indexPath);
        }
        
        public override bool CanMoveRow(UITableView tableView, NSIndexPath indexPath)
        {
            var section = this.Sections [indexPath.Section];
            return section.AllowMoveItems;
        }
        
        public override void MoveRow(UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
        {
          // todo: tell section that was moved  
            
            // could be multiple sections !
            
        }
        
        public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
        {
            var section = this.Sections [indexPath.Section];
            // todo: EditRow should return something to indicate that we need to add a row.
            section.EditRow(editingStyle, indexPath);
            
            switch (editingStyle)
            {
                case UITableViewCellEditingStyle.Delete:
                    this.TableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Bottom);
                    break;
                case UITableViewCellEditingStyle.Insert:
                    break;
            }
        }

        public override UIView GetViewForHeader(UITableView tableView, int sectionIdx)
        {
            var section = this.Sections [sectionIdx];
            return section.HeaderView;
        }

        public override float GetHeightForHeader(UITableView tableView, int sectionIdx)
        {
            var section = this.Sections [sectionIdx];
            if (section.HeaderView == null)
            {
                return -1;
            }
        
            return section.HeaderView.Frame.Height;
        }

        public override UIView GetViewForFooter(UITableView tableView, int sectionIdx)
        {
            var section = this.Sections [sectionIdx];
            return section.FooterView;
        }

        public override float GetHeightForFooter(UITableView tableView, int sectionIdx)
        {
            var section = this.Sections [sectionIdx];
            if (section.FooterView == null)
            {
                return -1;
            }
        
            return section.FooterView.Frame.Height;
        }
         
        private void HandleCollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.TableView == null)
            {
                return;
            }
            
            var sectionIndex = this.Sections.IndexOf(sender as TableViewSectionBase);
            
            switch (e.Action) 
            {
                case NotifyCollectionChangedAction.Add:
                    this.InsertVisual(sectionIndex, e.NewStartingIndex, UITableViewRowAnimation.Top, e.NewItems.Count);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.DeleteVisual(sectionIndex, e.OldStartingIndex, UITableViewRowAnimation.Bottom, e.OldItems.Count);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.ReloadSection(sectionIndex);
                    break;
                default:
                    break;
            }           
        }
        
        private void InsertVisual(int sectionIndex, int idx, UITableViewRowAnimation anim, int count)
        {
            var paths = new NSIndexPath[count];
            for (int i = 0; i < count; i++)
            {
                paths [i] = NSIndexPath.FromRowSection(idx + i, sectionIndex);
            }
            
            this.TableView.InsertRows(paths, anim);
        }
        
        private void DeleteVisual(int sectionIndex, int idx, UITableViewRowAnimation anim, int count)
        {
            var paths = new NSIndexPath[count];
            for (int i = 0; i < count; i++)
            {
                paths [i] = NSIndexPath.FromRowSection(idx + i, sectionIndex);
            }
            
            this.TableView.DeleteRows(paths, anim);
        }
        
        private void ReloadSection(int sectionIndex)
        {
            this.TableView.ReloadData();
            //this.TableView.ReloadSections(new NSIndexSet ((uint) sectionIndex), UITableViewRowAnimation.Right);
        }
    }
}
