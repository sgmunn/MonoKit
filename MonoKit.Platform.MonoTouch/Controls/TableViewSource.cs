//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="TableViewSource.cs" company="sgmunn">
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
    using System.Collections.Generic;
    using MonoKit.ViewModels;
    using MonoTouch.Foundation;
    using MonoTouch.UIKit;

    public class TableViewSource : UITableViewSource
    {
        public TableViewSource()
        {
            this.TemplateSelectors = new List<IDataTemplateSelector>();
            this.Root = new SectionRoot();
        }
        
        ~TableViewSource()
        {
            Console.WriteLine("~TableViewSource");
        }

        protected override void Dispose(bool disposing)
        {
            Console.WriteLine("Dispose.TableViewSource {0}", disposing);
            base.Dispose(disposing);
        }

        public ISectionRoot Root 
        { 
            get; 
            private set; 
        } 
        
        protected List<IDataTemplateSelector> TemplateSelectors
        {
            get;
            private set;
        }

        public void Clear()
        {
            this.Root.Sections.Clear();
        }
        
        public void ApplyTemplateSelectors(IList<IDataTemplateSelector> templates)
        {
            this.TemplateSelectors.Clear();
            this.TemplateSelectors.AddRange(templates);
        }

        public override int NumberOfSections(UITableView tableView)
        {
            return this.Root.Sections.Count;
        }
        
        public override string TitleForHeader(UITableView tableView, int section)
        {
            object header = this.Root[section].Header;
            if (header != null)
            {
                return header.ToString();
            }

            return string.Empty;
        }

        public override UIView GetViewForHeader(UITableView tableView, int section)
        {
            object header = this.Root[section].Header;
            if (header != null && !(header is string))
            {
                var template = this.GetTemplate(header);
                if (template != null)
                {
                    var view = (UIView)template.CreateView();
                    template.InitializeView(view);
                    template.BindViewModel(header, view);

                    return view;
                }
            }
            
            return null;
        }
        
        public override float GetHeightForHeader(UITableView tableView, int section)
        {
            object header = this.Root[section].Header;
            if (header != null && !(header is string))
            {
                var template = this.GetTemplate(header);
                if (template != null)
                {
                    return template.CalculateHeight(header);
                }
            }

            return -1;
        }
        
        public override string TitleForFooter(UITableView tableView, int section)
        {
            object footer = this.Root[section].Footer;
            if (footer != null)
            {
                return footer.ToString();
            }
            
            return string.Empty;
        }

        public override UIView GetViewForFooter(UITableView tableView, int section)
        {
            object footer = this.Root[section].Footer;
            if (footer != null && !(footer is string))
            {
                var template = this.GetTemplate(footer);
                if (template != null)
                {
                    var view = (UIView)template.CreateView();
                    template.InitializeView(view);
                    template.BindViewModel(footer, view);
                    
                    return view;
                }
            }
            
            return null;
        }
        
        public override float GetHeightForFooter(UITableView tableView, int section)
        {
            object footer = this.Root[section].Footer;
            if (footer != null && !(footer is string))
            {
                var template = this.GetTemplate(footer);
                if (template != null)
                {
                    return template.CalculateHeight(footer);
                }
            }

            return -1;
        }

        public override int RowsInSection(UITableView tableview, int section)
        {
            return this.Root[section].Items.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var viewModel = this.Root[indexPath.Section].Items[indexPath.Row];

            var template = this.GetTemplate(viewModel); 

            if (template != null)
            {
                var templateCell = tableView.DequeueReusableCell(template.ReuseIdentifier);
                if (templateCell == null)
                {
                    templateCell = (UITableViewCell)template.CreateView();
                }

                template.InitializeView(templateCell);

                template.BindViewModel(viewModel, templateCell);

                return templateCell;
            }

            var defaultCell = tableView.DequeueReusableCell("TableViewCellDefault");
            if (defaultCell == null)
            {
                defaultCell = new UITableViewCell(UITableViewCellStyle.Default, "TableViewCellDefault");
            }

            defaultCell.TextLabel.Text = viewModel.ToString();

            return defaultCell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);

            var row = this.GetViewModelForIndexPath(indexPath) as ICommand;

            if (row != null && row.GetCanExecute())
            {
                row.Execute();
            }
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
        }

        public Type QueryGetCellType(int section, int row)
        {
            var viewModel = this.Root[section].Items[row];
            
            var template = this.GetTemplate(viewModel); 
            
            if (template != null)
            {
                return template.ViewType;
            }
            
            return typeof(UITableViewCell);
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
    }
}
