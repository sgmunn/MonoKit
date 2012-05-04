using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoKit.UI.PagedViews;
using System.ComponentModel;
using MonoKit.UI;
using MonoKit.UI.Elements;
using MonoKit.DataBinding;
using MonoKitSample;

namespace iPhoneTest
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register ("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        private UIWindow window;
        private Samples monokitSamples;
                        
        private MetroPageView pgTitle;
        private UIViewController ctrl;
        
        private void TestPaging()
        {
            this.ctrl = new UIViewController();
            window.AddSubview(this.ctrl.View);
            var f = this.ctrl.View.Frame;
            f.X = 0;
            f.Y = 0;
            f.Width = f.Width - 0;
            
            this.pgTitle = new MetroPageView(f, this.GetPages());
            //this.pgTitle.Frame = f;
            this.ctrl.View.AddSubview(this.pgTitle);
        }
        
        private IEnumerable<MetroPage> GetPages()
        {
            yield return new MetroPage() { Title = "page1" };
            yield return new MetroPage() { Title = "page2" };
            yield return new MetroPage() { Title = "page3" };
        }
        
        public class MyNavigationController : UINavigationController
        {
            public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
            {
                return true;
            }
        }
  
        
        private UINavigationController controller;
        private TableViewController tableController;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            this.controller = new MyNavigationController();
            this.monokitSamples = new Samples(this.controller);
            

            // create a new window instance based on the screen size
            this.window = new UIWindow(UIScreen.MainScreen.Bounds);

            this.window.AddSubview(this.controller.View);
   
            // setup main index
            this.tableController = new TableViewController(UITableViewStyle.Grouped);
            this.tableController.Title = "MonoKit";
            this.controller.PushViewController(this.tableController, false);
            this.monokitSamples.SetupMainIndexSection(this.tableController.Source);
            
//            this.tableController.NavigationItem.RightBarButtonItem = new UIBarButtonItem("Next", UIBarButtonItemStyle.Plain, this.DoNext);
            
            this.window.MakeKeyAndVisible();
            
  //          this.SetupSection1();
            
            return true;
        }
        
        
        
        private TextInputElement text1 = new TextInputElement("Text Input Element 1", "v1") {Placeholder = "Placeholder text" };
        private TextInputElement text2 = new TextInputElement("", "v2") {Placeholder = "Different Placeholder text" };
        
        private void SetupSection1()
        {
            var section = new TableViewSection(this.tableController.Source);
            
            section.Header = "Sample Elements";
            section.Footer = "Section Footer";
            
            section.Add(new StringElement("String Element") { Command = this.DoNext1 });
            section.Add(new StringElement("String Element") { Command = this.DoNext2 });
            section.Add(new StringElement("String Element") { Command = this.DoNext3 });
            section.Add(new SubtitleStringElement("Subtitle String Element", "Subtitle"));
            section.Add(new Value1StringElement("Value1 String Element", "Detail"));
            section.Add(new Value2StringElement("Value2 String Element", "Detail"));
            section.Add(new BooleanElement("Boolean Element 1", true));
            section.Add(new CheckboxElement("Checkbox Element", true));
            section.Add(text1);
            section.Add(new PasswordInputElement("Password"));
            
            for (int i = 0; i < 1000; i++)
            {
                section.Add(new StringElement("String Element " + i.ToString()));
            }
            
            section.Add(text2);
            section.Add(new BooleanElement("Boolean Element 2", false));
        }
        
        private void DoNext(object sender, EventArgs args)
        {
            var tb = new TableViewController(UITableViewStyle.Grouped);
            this.controller.PushViewController(tb, true);
        }
        
        private static int tx = 0;
        
        [Register("MKTest")]
        public class Test 
        {
            private string test1;
            private string test2;
            private string test3;
            private string test4;
            private string test5;
            
            ~Test()
            {
                tx = tx + 1;
                if (tx > 1000)
                {
                    Console.WriteLine("Destroy Test {0}", tx);
                    tx = 0;
            }
            }
            
            public string Test1
            {
                get
                {
                    return this.test1;
                }
                
                set
                {
                    if (value != this.test1)
                    {
                        this.test1 = value;
                        this.OnPropertyChange("Test1");
                    }
                }
            }
            
            public string Test2
            {
                get
                {
                    return this.test2;
                }
                
                set
                {
                    if (value != this.test2)
                    {
                        this.test2 = value;
                        this.OnPropertyChange("Test2");
                    }
                }
            }
            
            public string Test3
            {
                get
                {
                    return this.test3;
                }
                
                set
                {
                    if (value != this.test3)
                    {
                        this.test3 = value;
                        this.OnPropertyChange("Test3");
                    }
                }
            }
            
            public string Test4
            {
                get
                {
                    return this.test4;
                }
                
                set
                {
                    if (value != this.test4)
                    {
                        this.test4 = value;
                        this.OnPropertyChange("Test4");
                    }
                }
            }
            
            public string Test5
            {
                get
                {
                    return this.test5;
                }
                
                set
                {
                    if (value != this.test5)
                    {
                        this.test5 = value;
                        this.OnPropertyChange("Test5");
                    }
                }
            }
            
            
            #region INotifyPropertyChanged implementation
            public event PropertyChangedEventHandler PropertyChanged;
            #endregion
            
            public void OnPropertyChange(string propertyName)
            {
                if (this.PropertyChanged != null)
                {
                    var ev = new PropertyChangedEventArgs(propertyName);
                    this.PropertyChanged(this, ev);
                }
            }
        }
        
        public class TestX : Test, INotifyPropertyChanged
        {
            
        }
        
        private void DoNext1(Element element)
        {
            (element as StringElement).Text = "Hello";
            //var tb = new TableViewController(UITableViewStyle.Grouped);
            //this.controller.PushViewController(tb, true);

            var s1 = DateTime.Now;
            
            var source = new List<Test>();
            var target = new List<Test>();
            for (int i = 0; i < 10000; i++)
            {
                var t1 = new TestX();
                var t2 = new Test();
                source.Add(t1);
                target.Add(t2);
                
                if (true)
                {
                t2.SetBinding("Test1", t1, new Binding<Test>("Test1") { GetValue = (x) => x.Test1});
                t2.SetBinding("Test2", t1, new Binding<Test>("Test2") { GetValue = (x) => x.Test2});
                t2.SetBinding("Test3", t1, new Binding<Test>("Test3") { GetValue = (x) => x.Test3});
                t2.SetBinding("Test4", t1, new Binding<Test>("Test4") { GetValue = (x) => x.Test4});
                t2.SetBinding("Test5", t1, new Binding<Test>("Test5") { GetValue = (x) => x.Test5});
                }
            }
            
            var e1 = DateTime.Now;
  
            var s = DateTime.Now;
            
            foreach (var t in source)
            {
                t.Test1 = "Hello";
                t.Test2 = "World";
                t.Test3 = "This";
                t.Test4 = "is";
                t.Test5 = "a test";
            }
            
            var e = DateTime.Now;
            
            (element as StringElement).Text = (e1-s1).TotalMilliseconds.ToString() + "  " + (e-s).TotalMilliseconds.ToString() + "  " + target[3].Test4;
            Console.WriteLine((element as StringElement).Text);
            
            source.Clear();
            target.Clear();
        }
        
        private void DoNext2(Element element)
        {
            BindingOperations.RemoveStaleBindings();
        }
        
        private void DoNext3(Element element)
        {
            GC.Collect(0);
        }
    }

}

