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

using MonoKit.Interactivity;
namespace MonoKit.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using MonoKit.DataBinding;
    using MonoTouch.Foundation;
    using MonoTouch.UIKit;

    public abstract class TableViewSectionBase : IEnumerable
    {
        protected const string DefaultViewReuseIdentifier = "DefaultCell";
        
        private object header;
        private object footer;
        private readonly List<IViewDefinition> viewDefinitions;
        
        protected TableViewSectionBase(TableViewSource source)
        {
            this.Source = source;
            this.viewDefinitions = new List<IViewDefinition>();
            this.Source.Add(this);
        }
        
        protected TableViewSectionBase(TableViewSource source, params IViewDefinition[] viewDefinitions)
        {
            this.Source = source;
            this.viewDefinitions = new List<IViewDefinition>(viewDefinitions);
            this.Source.Add(this);
        }
                 
        ~TableViewSectionBase()
        {
            Console.WriteLine("TableViewSectionBase");
        }
               
        public static AttachedProperty SectionProperty
        {
            get
            {
                return AttachedProperty.Register("Section", typeof(TableViewSectionBase), typeof(TableViewSectionBase));
            }
        }

        public TableViewSource Source
        {
            get;
            private set;
        }
       
        public string Header
        {
            get
            { 
                return this.header as string; 
            }
            
            set
            { 
                this.header = value; 
            }
        }
        
        public string Footer
        {
            get
            { 
                return this.footer as string; 
            }
            
            set
            { 
                this.footer = value; 
            }
        }

        public UIView HeaderView
        {
            get
            { 
                return this.header as UIView; 
            }
            
            set
            { 
                this.header = value; 
            }
        }

        public UIView FooterView
        {
            get
            { 
                return this.footer as UIView; 
            }
            
            set
            { 
                this.footer = value; 
            }
        }
                
        public int Count
        { 
            get
            {
                return this.GetCount();
            }
        }
        
        public bool AllowMoveItems
        {
            get;
            set;
        }
        
        public abstract void Clear();
        
        public abstract UITableViewCell GetCell(int row);
        
        public abstract void RowSelected(NSIndexPath indexPath);
        
        public abstract bool CanEditRow(NSIndexPath indexPath);
        
        public abstract void EditRow(UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath);
                
        public abstract void MoveRow(NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath);

        public abstract float GetHeightForRow(NSIndexPath indexPath);

        public abstract IEnumerator GetEnumerator();
  
        protected abstract int GetCount();
        
        protected virtual UITableViewCell CreateDefaultView()
        {
            return new UITableViewCell(UITableViewCellStyle.Default, DefaultViewReuseIdentifier);
        }
                
        protected UITableViewCell GetViewForObject(object element)
        {
            UITableViewCell cell = null;
            
            var viewDef = this.GetViewDefinition(element);
            if (viewDef == null)
            {
                cell = this.GetDefaultView();
                
                // perform a basic bind to the cell so that we can see that something should be in the table
                cell.TextLabel.Text = element.ToString();
                cell.SetValue(DataContextAttachedProperty.DataContextProperty, element);
                
                return cell;
            }

            cell = this.GetViewFromDeclaration(viewDef);
            
            // if the element is a wrapper, then bind to the wrapped object instead
            var dataContext = element;
            if (element is IDataViewWrapper)
            {
                dataContext = ((IDataViewWrapper)element).Data;
            }
            
            cell.SetValue(DataContextAttachedProperty.DataContextProperty, dataContext);
            viewDef.Bind(cell, dataContext);
            
            return cell;
        }
  
        private static void AttachBehavioursToView (List<Type> behaviours, UITableViewCell cell)
        {
            foreach (var behaviourType in behaviours)
            {
                var behaviour = System.Activator.CreateInstance(behaviourType) as Behaviour;
                behaviour.AttachedObject = cell;
            }
        }
        
        private static NSString GetReuseIndentiferForView(Type viewType, UITableViewCellStyle style)
        {
            return new NSString(viewType.ToString()+"_"+style.ToString());
        }
                               
        private UITableViewCell GetViewFromDeclaration(IViewDefinition viewDefinition)
        {
            var style = UITableViewCellStyle.Default;
            if (viewDefinition.Param is UITableViewCellStyle)
            {
                style = (UITableViewCellStyle)viewDefinition.Param;
            }
            
            var reuseIndentifer = GetReuseIndentiferForView(viewDefinition.ViewType, style);
            var cell = this.Source.TableView.DequeueReusableCell(reuseIndentifer);
            
            if (cell == null)
            {
                cell = this.CreateView(viewDefinition.ViewType, style, reuseIndentifer);
                
                cell.SetValue(TableViewSection.SectionProperty, this);
                AttachBehavioursToView(viewDefinition.Behaviours, cell);
            }
            
            return cell;
        }
                
        private UITableViewCell GetDefaultView()
        {
            var cell = this.Source.TableView.DequeueReusableCell(DefaultViewReuseIdentifier);
            if (cell == null)
            {
                cell = this.CreateDefaultView();
                cell.SetValue(TableViewSection.SectionProperty, this);
            }
            
            return cell;
        }
        
        private UITableViewCell CreateView(Type viewType, UITableViewCellStyle style, NSString reuseIndentifer)
        {
            var cell = System.Activator.CreateInstance(viewType, new object[] {style, reuseIndentifer});
            return cell as UITableViewCell;
        }

        private IViewDefinition GetViewDefinition(object element)
        {
            if (element is IDataViewWrapper)
            {
                return ((IDataViewWrapper)element).ViewDefinition;
            }
            
            var view = this.viewDefinitions.FirstOrDefault(x => x.Renders(element));
            return view;
        }
    }
}

