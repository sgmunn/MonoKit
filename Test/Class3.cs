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
using MonoTouch.Foundation;
using MonoTouch.CoreAnimation;
using MonoTouch.UIKit;
using System.Drawing;

namespace Test
{


    public class ViewDeckControllerDelegate : NSObject
    {

        public virtual bool ApplyShadow(ViewDeckController viewDeckController, CALayer shadowLayer, RectangleF rect) 
        {
            return false;
        }

        public virtual void DidPanToOffset(ViewDeckController viewDeckController, float offset) 
        {

        }

        public virtual void SlideOffsetChanged(ViewDeckController viewDeckController, float offset) 
        {

        }


        public virtual void DidBounceWithClosingController(ViewDeckController viewDeckController, UIViewController openController) 
        {

        }

        public virtual bool WillOpenLeftView(ViewDeckController viewDeckController, bool animated) 
        {
            return true;
        }

        public virtual void DidOpenLeftView(ViewDeckController viewDeckController, bool animated) 
        {

        }

        public virtual bool WillCloseLeftView(ViewDeckController viewDeckController, bool animated) 
        {
            return true;
        }

        public virtual void DidCloseLeftView(ViewDeckController viewDeckController, bool animated) 
        {

        }

        public virtual bool WillOpenRightView(ViewDeckController viewDeckController, bool animated) 
        {
            return true;
        }

        public virtual void DidOpenRightView(ViewDeckController viewDeckController, bool animated) 
        {

        }

        public virtual bool WillCloseRightView(ViewDeckController viewDeckController, bool animated) 
        {
            return true;
        }

        public virtual void DidCloseRightView(ViewDeckController viewDeckController, bool animated) 
        {

        }

        public virtual void DidShowCenterView(ViewDeckController viewDeckController, bool animated) 
        {

        }
    }
}

