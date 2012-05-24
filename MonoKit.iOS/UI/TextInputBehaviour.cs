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
    using System.Linq;
    using MonoKit.DataBinding;
    using MonoKit.UI.Controls;
    using MonoKit.UI.Elements;
    using MonoKit.Interactivity;
    using MonoTouch.Foundation;
    using MonoTouch.UIKit;
    
    public class TextInputBehaviour : Behaviour
    {
        protected override void OnAttach (object instance)
        {
            var obj = instance as TextInputElementTableViewCell; 
            if (obj != null)
            {
                obj.TextField.Started -= HandleStarted;
                obj.TextField.ShouldReturn -= HandleShouldReturn;
                obj.TextField.Started += HandleStarted;
                obj.TextField.ShouldReturn += HandleShouldReturn;
            }
        }

        protected override void OnDetach (object instance)
        {
            var obj = instance as TextInputElementTableViewCell; 
            if (obj != null)
            {
                obj.TextField.Started -= HandleStarted;
                obj.TextField.ShouldReturn -= HandleShouldReturn;
            }
        }

        private void HandleStarted (object sender, EventArgs e)
        {
            var section = this.AttachedObject.GetValue(TableViewSection.SectionProperty) as TableViewSectionBase;
            if (section != null)
            {
                var source = this.AttachedObject.GetValue(DataContextAttachedProperty.DataContextProperty);
                
                bool found = false;
                bool isLast = true;
                foreach (var obj in section)

                {
                    if (obj.Equals(source))
                    {
                        found = true;
                        continue;
                    }
                    
                    if (found && obj is IInputElement)
                    {
                        isLast = false;
                        break;
                    }
                }
                
                if (isLast)
                {
                    ((TextInputElementTableViewCell)this.AttachedObject).TextField.ReturnKeyType = UIReturnKeyType.Done;
                }
                else
                {
                    ((TextInputElementTableViewCell)this.AttachedObject).TextField.ReturnKeyType = UIReturnKeyType.Next;
                }
            }
        }
        
        private bool HandleShouldReturn (UITextField textField)
        {
            if (((TextInputElementTableViewCell)this.AttachedObject).TextField.ReturnKeyType == UIReturnKeyType.Done)
            {
                textField.ResignFirstResponder();
                return true;
            }

            var section = this.AttachedObject.GetValue(TableViewSection.SectionProperty) as TableViewSectionBase;
            if (section != null)
            {            
                var source = this.AttachedObject.GetValue(DataContextAttachedProperty.DataContextProperty);
            
                var indx = section.Source.TableView.IndexPathForCell(this.AttachedObject as UITableViewCell);
                
                bool found = false;
                int row = indx.Row;
                
                foreach (var obj in section)
                {
                    if (obj.Equals(source))
                    {
                        found = true;
                        continue;
                    }
                    
                    if (found)
                    {
                        row += 1;
                    }
                    
                    if (found && obj is IInputElement)
                    {
                        var newCell = section.Source.TableView.CellAt(NSIndexPath.FromRowSection(row, indx.Section));
                        
                        // animate scroll if we have the cell, otherwise don't animate - not animating will allow the tableview to construct the cell
                        // during this call so that we can set responder afterwards
                        section.Source.TableView.ScrollToRow(NSIndexPath.FromRowSection(row, indx.Section), UITableViewScrollPosition.Middle, newCell != null);
                        
                        if (newCell == null)
                        {
                            newCell = section.Source.TableView.CellAt(NSIndexPath.FromRowSection(row, indx.Section));
                        }
                        
                        if (newCell != null)
                        {
                            (newCell).BecomeFirstResponder();
                        }
                        
                        break;
                    }
                }
            }
            
            return true;
        }
    }
}

