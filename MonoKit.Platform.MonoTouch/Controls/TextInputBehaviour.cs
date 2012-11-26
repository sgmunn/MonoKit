//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="TextInputBehaviour.cs" company="sgmunn">
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
    using System.Linq;
    using MonoKit.DataBinding;
    using MonoTouch.Foundation;
    using MonoTouch.UIKit;

    public sealed class TextInputBehaviour : Behaviour
    {
        public TextInputBehaviour(object attachedObject)
        {
            this.AttachedObject = attachedObject;
        }

        public StringInputTableViewCell View
        {
            get
            {
                return (StringInputTableViewCell)this.AttachedObject;
            }
        }

        protected override void OnAttach(object instance)
        {
            var obj = instance as StringInputTableViewCell; 
            if (obj != null)
            {
                obj.TextField.Started -= this.HandleStarted;
                obj.TextField.ShouldReturn -= this.HandleShouldReturn;
                obj.TextField.Started += this.HandleStarted;
                obj.TextField.ShouldReturn += this.HandleShouldReturn;
            }
        }
        
        protected override void OnDetach(object instance)
        {
            var obj = instance as StringInputTableViewCell; 
            if (obj != null)
            {
                obj.TextField.Started -= this.HandleStarted;
                obj.TextField.ShouldReturn -= this.HandleShouldReturn;
            }
        }
        
        private void HandleStarted(object sender, EventArgs e)
        {
            var tableView = this.View.GetTableView();
            if (tableView == null)
            {
                return;
            }

            var source = tableView.Source as TableViewSource;
            if (source == null)
            {
                return;
            }

            var index = tableView.IndexPathForCell(this.View);

            // search the current section
            // todo: search all sections ??

            bool isLast = true;
            for (int i = index.Row + 1; i < source.Root[index.Section].Items.Count; i++)
            {
                var viewType = source.QueryGetCellType(index.Section, i);
                if (viewType.GetInterfaces().Contains(typeof(IFocusableInputCell)))
                {
                    isLast = false;
                }
            }

            if (isLast)
            {
                this.View.TextField.ReturnKeyType = UIReturnKeyType.Done;
            }
            else
            {
                this.View.TextField.ReturnKeyType = UIReturnKeyType.Next;
            }
        }
        
        private bool HandleShouldReturn(UITextField textField)
        {
            if (this.View.TextField.ReturnKeyType == UIReturnKeyType.Done)
            {
                this.View.ResignFirstResponder();
                return true;
            }

            var tableView = this.View.GetTableView();
            if (tableView == null)
            {
                return true;
            }
            
            var source = tableView.Source as TableViewSource;
            if (source == null)
            {
                return true;
            }
            
            var index = tableView.IndexPathForCell(this.View);
            
            // search the current section
            // todo: search all sections ??
            
            for (int i = index.Row + 1; i < source.Root[index.Section].Items.Count; i++)
            {
                var viewType = source.QueryGetCellType(index.Section, i);
                if (viewType.GetInterfaces().Contains(typeof(IFocusableInputCell)))
                {
                    var newCell = tableView.CellAt(NSIndexPath.FromRowSection(i, index.Section));
                    
                    // animate scroll if we have the cell, otherwise don't animate - not animating will allow the tableview to construct the cell
                    // during this call so that we can set responder afterwards
                    tableView.ScrollToRow(NSIndexPath.FromRowSection(i, index.Section), UITableViewScrollPosition.Middle, newCell != null);
                    
                    if (newCell == null)
                    {
                        newCell = tableView.CellAt(NSIndexPath.FromRowSection(i, index.Section));
                    }
                    
                    if (newCell != null)
                    {
                        (newCell).BecomeFirstResponder();
                    }
                    
                    return true;
                }
            }

            return true;
        }
    }
}

