namespace MonoKit.UI
{
    using System;
    using MonoTouch.UIKit;
    using MonoTouch.Foundation;
    
    public class SizingTableViewSource : TableViewSource
    {
        public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
        {
            // GetHeightForRow is called prior to GetCell, so we need to know the height of the view for the element 
            // before we get to construct the view itself.  
            
            var section = this.SectionAt(indexPath.Section);
            return section.GetHeightForRow(indexPath);
        }
        
    }
}

