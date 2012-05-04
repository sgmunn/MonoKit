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
        
        ~TableViewController()
        {
            Console.WriteLine("TableViewController");
        }
        
        /// <summary>
        /// Gets the source for the UITableView.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
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
            
            var tableView = new UITableView(UIScreen.MainScreen.Bounds, this.tableStyle);
            tableView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin;
            tableView.AutosizesSubviews = true;
            
            this.View = tableView;
            this.Source.TableView = tableView;
        }
        
        public override void ViewDidUnload ()
        {
            this.Source.TableView.Dispose();
            this.Source.TableView = null;
            base.ViewDidUnload ();
        }
        
        public override void ViewWillDisappear (bool animated)
        {
            base.ViewWillDisappear (animated);
        }
        
        public override void ViewDidAppear (bool animated)
        {
            base.ViewDidAppear (animated);
        }
        
        public override void ViewDidDisappear (bool animated)
        {
            base.ViewDidDisappear (animated);
        }
        
        protected override void Dispose (bool disposing)
        {
            base.Dispose (disposing);
        }
    }
}

