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

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoKit.UI.ViewDeck;
using MonoKit.UI;
using MonoKit.UI.Elements;

namespace ViewDeckSample
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
            
            window.RootViewController = new MainSampleController();

            // make the window visible
            window.MakeKeyAndVisible();
            
            return true;
        }
    }


    public class MainSampleController : TableViewController
    {
        public MainSampleController() : base(UITableViewStyle.Plain)
        {
        }

        public override void LoadView()
        {
            base.LoadView();
            this.InitSample();
        }

        public void InitSample()
        {
            var section1 = new TableViewSection(this.Source);
            section1.Add(new StringElement("Simple") { Command = this.StartSimpleSample });
            section1.Add(new StringElement("Navigation (Contained)") { Command = this.StartContainedSample });
            section1.Add(new StringElement("Navigation (Integrated)") { Command = this.StartIntegratedSample });
            section1.Add(new StringElement("Multi-Deck") { Command = this.StartMultiDeckSample });
        }

        public void StartSimpleSample(Element element)
        {
            var leftController = new LeftController();
            var rightController = new RightController();

            var centerController = new CenterController(true);

            var deckController = new ViewDeckController(centerController, leftController, rightController);
            deckController.RightLedge = 40;
            deckController.LeftLedge = 100;

            deckController.Enabled = false;

            this.PresentViewController(deckController, true, null);

        }

        public void StartContainedSample(Element element)
        {
            var nav = new UINavigationController(new NavigationStartController(true));

            this.PresentViewController(nav, true, null);
        }

        public void StartIntegratedSample(Element element)
        {
            var nav = new UINavigationController(new NavigationStartController(false));

            this.PresentViewController(nav, true, null);
        }

        public void StartMultiDeckSample(Element element)
        {
            var leftController = new MultiLeftController(); 
            var bottomController = new MultiBottomController();

            var centerController = new MultiCenterController();

            var secondDeckController = new ViewDeckController(leftController, bottomController);
            secondDeckController.LeftLedge = 100;

            var deckController = new ViewDeckController(centerController, secondDeckController);
            deckController.LeftLedge = 30;

            this.PresentViewController(deckController, true, null);
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return true;
        }
    }

    public class CenterController : TableViewController
    {
        private bool addCloseButton;

        public CenterController(bool addCloseButton) : base(UITableViewStyle.Grouped)
        {
            this.addCloseButton = addCloseButton;
        }

        public override void LoadView()
        {
            base.LoadView();
            this.InitController();
        }

        public void InitController()
        {
            if (this.addCloseButton)
            {
                var section1 = new TableViewSection(this.Source);
                section1.Add(new StringElement("Close") { Command = this.Close });
            }

            var section2 = new TableViewSection(this.Source);
            section2.Add(new StringElement("Open Left") { Command = this.OpenLeft });
            section2.Add(new StringElement("Open Right") { Command = this.OpenRight });

            var section3 = new TableViewSection(this.Source);
            section3.Add(new StringElement("Swap Left & Right") { Command = this.Swap });

            // for navigation sample
            if (this.NavigationController != null)
            {
                var section4 = new TableViewSection(this.Source);
                section4.Add(new StringElement("Some other controller") { Command = this.GoNext });
            }
        }
        
        public void Close(Element element)
        {
            this.DismissViewController(true, null);
        }
        
        public void OpenLeft(Element element)
        {
            var deck = this.ParentViewController as ViewDeckController;
            deck.OpenLeftView();
        }

        public void OpenRight(Element element)
        {
            var deck = this.ParentViewController as ViewDeckController;
            deck.OpenRightView();
        }
                
        public void Swap(Element element)
        {
            var deck = this.ParentViewController as ViewDeckController;
            var left = deck.LeftController;
            var right = deck.RightController;
            deck.LeftController = right;
            deck.RightController = left;
        }
                
        public void GoNext(Element element)
        {
            this.NavigationController.PushViewController(new SomeOtherController(), true);
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return true;
        }
    }

    public class LeftController : TableViewController
    {
        public LeftController() : base(UITableViewStyle.Grouped)
        {
        }

        public override void LoadView()
        {
            base.LoadView();
            this.InitController();
        }

        public void InitController()
        {
            var section1 = new TableViewSection(this.Source);
            section1.Header = "Left";
            section1.Add(new StringElement("Show Center") { Command = this.ShowCenter });
        }
                
        public void ShowCenter(Element element)
        {
            var deck = this.ParentViewController as ViewDeckController;
            if (deck != null)
            {
                deck.ShowCenterView();
            }
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return true;
        }
    }

    public class RightController : TableViewController
    {
        public RightController() : base(UITableViewStyle.Grouped)
        {
        }

        public override void LoadView()
        {
            base.LoadView();
            this.InitController();
        }

        public void InitController()
        {
            var section1 = new TableViewSection(this.Source);
            section1.Header = "Right";
            section1.Add(new StringElement("Show Center") { Command = this.ShowCenter });
        }
                
        public void ShowCenter(Element element)
        {
            var deck = this.ParentViewController as ViewDeckController;
            if (deck != null)
            {
                deck.ShowCenterView();
            }
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return true;
        }
    }


    public class NavigationStartController : TableViewController
    {
        private bool contained;

        public NavigationStartController(bool contained) : base(UITableViewStyle.Grouped)
        {
            this.contained = contained;
            this.Title = "Start";
        }

        public override void LoadView()
        {
            base.LoadView();
            this.InitController();
        }

        public void InitController()
        {
            var section1 = new TableViewSection(this.Source);
            section1.Header = "";
            section1.Add(new StringElement("Close") { Command = this.Close });

            var section2 = new TableViewSection(this.Source);
            section2.Header = "";
            section2.Add(new StringElement("Go to Deck") { Command = this.GotoDeck });
        }

        public void Close(Element element)
        {
            this.DismissViewController(true, null);
        }   

        public void GotoDeck(Element element)
        {
            var leftController = new LeftController(); 
            var rightController = new RightController();

            var centerController = new CenterController(false);
            centerController.Title = "Center";

            var deckController = new ViewDeckController(centerController, leftController, rightController);
            deckController.RightLedge = 40;
            deckController.LeftLedge = 100;

            if (this.contained)
            {
                deckController.NavigationControllerBehavior = ViewDeckNavigationControllerBehavior.Contained;
            }
            else
            {
                deckController.NavigationControllerBehavior = ViewDeckNavigationControllerBehavior.Integrated;
            }

            this.NavigationController.PushViewController(deckController, true);
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return true;
        }
    }


    public class SomeOtherController : TableViewController
    {
        public SomeOtherController() : base(UITableViewStyle.Grouped)
        {
            this.Title = "Someother View";
        }

        public override void LoadView()
        {
            base.LoadView();
            this.InitController();
        }

        public void InitController()
        {
            var section1 = new TableViewSection(this.Source);
            section1.Header = "Something";
            section1.Add(new StringElement("Hello") { Command = this.Hello });
        }
                
        public void Hello(Element element)
        {
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return true;
        }
    }


    public class MultiLeftController : TableViewController
    {
        public MultiLeftController() : base(UITableViewStyle.Grouped)
        {
        }

        public override void LoadView()
        {
            base.LoadView();
            this.InitController();
        }

        public void InitController()
        {
            var section1 = new TableViewSection(this.Source);
            section1.Header = "Left (Middle)";
            section1.Add(new StringElement("Open Left") { Command = this.Hello });
        }
                
        public void Hello(Element element)
        {
            var deck = this.ParentViewController as ViewDeckController;
            deck.OpenLeftView();
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return true;
        }
    }

    public class MultiBottomController : TableViewController
    {
        public MultiBottomController() : base(UITableViewStyle.Grouped)
        {
        }

        public override void LoadView()
        {
            base.LoadView();
            this.InitController();
        }

        public void InitController()
        {
            var section1 = new TableViewSection(this.Source);
            section1.Header = "Bottom";
            section1.Add(new StringElement("Hello") { Command = this.Hello });
        }
                
        public void Hello(Element element)
        {
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return true;
        }
    }

    public class MultiCenterController : TableViewController
    {
        public MultiCenterController() : base(UITableViewStyle.Grouped)
        {
        }

        public override void LoadView()
        {
            base.LoadView();
            this.InitController();
        }

        public void InitController()
        {
            var section1 = new TableViewSection(this.Source);
            section1.Header = "Center (Top)";
            section1.Add(new StringElement("Close") { Command = this.Close });

            var section2 = new TableViewSection(this.Source);
            section2.Add(new StringElement("Open Left") { Command = this.Hello });
        }
                
        public void Close(Element element)
        {
            this.DismissViewController(true, null);
        }
                
        public void Hello(Element element)
        {
            var deck = this.ParentViewController as ViewDeckController;
            deck.OpenLeftView();
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return true;
        }
    }
}

