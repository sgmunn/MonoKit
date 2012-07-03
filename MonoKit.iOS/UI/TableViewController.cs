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
 
    /// <summary>
    /// Defines a view controller that controls and owns a UITableView
    /// </summary>
    public class TableViewController : UITableViewController
    {
        private readonly UITableViewStyle tableStyle;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.UI.TableViewController"/> class.
        /// </summary>
        public TableViewController(UITableViewStyle tableStyle)
        {
            this.tableStyle = tableStyle;
            this.Source = new TableViewSource();
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.UI.TableViewController"/> class.
        /// </summary>
        /// <param name='source'>
        /// The Table view source to use for the UITableView.
        /// </param>
        public TableViewController(UITableViewStyle tableStyle, TableViewSource source)
        {
            this.tableStyle = tableStyle;
            this.Source = source;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.UI.TableViewController"/> class.
        /// </summary>
        /// <param name='handle'>Handle to the underlying UIKit object</param>
        public TableViewController(IntPtr handle) : base(handle)
        {
        }

        /// <summary>
        /// Gets the source for the UITableView.
        /// </summary>
        public TableViewSource Source
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Loads the view, creates the table view and adds it as the view of the controller.
        /// </summary>
        public override void LoadView ()
        {
            base.LoadView ();

            // todo: fix up 
            var tableView = new UITableView(UIScreen.MainScreen.Bounds, this.tableStyle);
            tableView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin;
            tableView.AutosizesSubviews = true;
            
            this.View = tableView;
            this.Source.TableView = tableView;
        }

        /// <summary>
        /// Called when the controller’s view is released from memory. 
        /// </summary>
        public override void ViewDidUnload ()
        {
            this.Source.TableView.Dispose();
            this.Source.TableView = null;
            base.ViewDidUnload ();
        }
    }
}

