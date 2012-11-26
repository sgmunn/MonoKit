//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="HomeController.cs" company="sgmunn">
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
using MonoKit.ViewModels;

namespace Sample.TableViews
{
    using System;
    using MonoKit.Controls;
    using MonoTouch.UIKit;

    public class HomeController : UIViewController
    {
        public HomeController()
        {
            this.Title = "Home";

            this.NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add, (s,e) => {
                this.NavigationController.PushViewController(new StandardViewController(), true);
            });

            this.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add, (s,e) => {
                this.NavigationController.PushViewController(new AlternateViewController(), true);
            });
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }
    }

    public class StandardViewController : TableViewController, INavigationService
    {
        public StandardViewController()
        {
            this.Title = "Std";

            var vm = new HomeViewModel(this);
            vm.Load();

            this.ViewModel = vm;

            // define the template selectors here if we need to
        }
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        public void NavigateTo(INavigationRequest request)
        {
            this.NavigationController.PushViewController(new TableViewController() { ViewModel = request.ViewModel }, true);
        }

        public void NavigateTo(IViewModel viewModel)
        {
            this.NavigationController.PushViewController(new TableViewController() { ViewModel = viewModel }, true);
        }

        public void Close(IViewModel viewModel)
        {
        }
    }

    public class AlternateViewController : TableViewController
    {
        public AlternateViewController()
        {
            this.Title = "Alternate";
            this.ViewModel = new AlternateHomeViewModel();
        }
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }
    }
}

