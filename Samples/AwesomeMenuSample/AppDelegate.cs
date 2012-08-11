//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file=".cs" company="sgmunn">
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
//
using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace AwesomeMenuSample
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register ("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        UIWindow window;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // create a new window instance based on the screen size
            window = new UIWindow(UIScreen.MainScreen.Bounds);
            
            // If you have defined a view, add it here:
            // window.AddSubview (navigationController.View);

            var storyMenuItemImage = UIImage.FromFile("Images/bg-menuitem.png");
            var storyMenuItemImagePressed = UIImage.FromFile("Images/bg-menuitem-highlighted.png");
    
            var starImage = UIImage.FromFile("Images/icon-star.png");

            var starMenuItem1 = new MonoKit.UI.AwesomeMenu.MenuItem(storyMenuItemImage, storyMenuItemImagePressed, starImage);
            var starMenuItem2 = new MonoKit.UI.AwesomeMenu.MenuItem(storyMenuItemImage, storyMenuItemImagePressed, starImage);
            var starMenuItem3 = new MonoKit.UI.AwesomeMenu.MenuItem(storyMenuItemImage, storyMenuItemImagePressed, starImage);

            MonoKit.UI.AwesomeMenu.Menu menu = new MonoKit.UI.AwesomeMenu.Menu(this.window.Bounds,
                new[] { starMenuItem1, starMenuItem2, starMenuItem3 }
                                                                                       );

            menu.MenuItemSelected += (sender, e) => Console.WriteLine(e.Selected);

            menu.BackgroundColor = UIColor.White;
            this.window.AddSubview(menu);
            
            // make the window visible
            window.MakeKeyAndVisible();
            
            return true;
        }
    }
}

