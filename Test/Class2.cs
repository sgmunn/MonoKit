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
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Diagnostics;
using MonoTouch.CoreGraphics;
using MonoTouch.ObjCRuntime;

namespace Test
{
    public enum PanningMode
    {
        NoPanning,
        FullViewPanning,
        NavigationBarPanning,
        PanningViewPanning
    }

    public enum CenterHiddenInteractivity
    {
        UserInteractive,
        NotUserInteractive,
        NotUserInteractiveWithTapToClose,
        NotUserInteractiveWithTapToCloseBouncing
    }

    public enum NavigationControllerBehavior
    {
        Contained,
        Integrated
    }

    public enum RotationBehavior
    {
        KeepsLedgeSizes,
        KeepsViewSizes
    }

    public class ViewDeckController : UIViewController
    {
        private bool elastic;
        private PanningMode panningMode;
        private NavigationControllerBehavior navigationControllerBehavior;
        private CenterHiddenInteractivity centerHiddenInteractivity;
        private RotationBehavior rotationBehavior;
        private bool viewAppeared;
        private bool resizesCenterView;
        private bool automaticallyUpdateTabBarItems;
        private List<UIGestureRecognizer> panners;
        private bool enabled;

        private UIViewController centerController;
        private UIViewController slidingController;
        private UIViewController leftController;
        private UIViewController rightController;

        private float originalShadowRadius;
        private SizeF originalShadowOffset;
        private UIColor originalShadowColor;
        private float originalShadowOpacity;
        private float leftLedge;
        public float rightLedge;

        private UIView referenceView;
        private UIBezierPath originalShadowPath;
        private UIView centerView;
        private UIButton centerTapper;
        private UIView panningView;

        private float maxLedge;
        private float offset;
        private float preRotationWidth;
        private float preRotationCenterWidth;
        private float leftWidth;
        private float rightWidth;
        private float panOrigin;

        public ViewDeckController(UIViewController centerController)
        {
            this.elastic = true;
            this.setPanningMode(PanningMode.FullViewPanning);
            this.centerHiddenInteractivity = CenterHiddenInteractivity.UserInteractive;
            this.rotationBehavior = RotationBehavior.KeepsLedgeSizes;
            this.panners = new List<UIGestureRecognizer>();
            this.enabled = true;

            this.originalShadowRadius = 0;
            this.originalShadowOffset = SizeF.Empty;
            this.originalShadowColor = UIColor.Clear;
            this.originalShadowOpacity = 0;
            this.originalShadowPath = null;
        
            //this.centerController = centerController;
            this.setCenterController(centerController);

            this.leftLedge = 44;
            this.rightLedge = 44;
        }


        public ViewDeckController(UIViewController centerController, UIViewController leftController) : this(centerController)
        {
            this.leftController = leftController;
        }

        public ViewDeckController(UIViewController centerController, UIViewController leftController, UIViewController rightController) : this(centerController)
        {
            this.setLeftController(leftController);
            this.setRightController(rightController);
        }

        private void CleanUp()
        {
            this.originalShadowRadius = 0;
            this.originalShadowOpacity = 0;
            this.originalShadowColor = null;
            this.originalShadowOffset = SizeF.Empty;
            this.originalShadowPath = null;
            
            this.slidingController = null;
            this.referenceView = null;
            this.centerView = null;
            this.centerTapper = null;
        }

        private void Dealloc()
        {
            this.CleanUp();
            
//            this.centerController.viewDeckController = null;
            this.centerController = null;

            if (this.leftController != null)
            {
//                this.leftController.viewDeckController = null;
                this.leftController = null;
            }

            if (this.rightController != null)
            {
//                this.rightController.viewDeckController = null;
                this.rightController = null;
            }

            this.panners.Clear();
        }


        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            this.centerController.DidReceiveMemoryWarning();

            if (this.leftController != null)
            {
                this.leftController.DidReceiveMemoryWarning();
            }

            if (this.rightController != null)
            {
                this.rightController.DidReceiveMemoryWarning();
            }
        }

        private List<UIViewController> controllers()
        {
            var result = new List<UIViewController>();

            result.Add(this.centerController);

            if (this.leftController != null)
            {
                result.Add(this.leftController);
            }

            if (this.rightController != null)
            {
                result.Add(this.rightController);
            }

            return result;
        }

        private RectangleF referenceBounds
        {
            get
            {
                if (this.referenceView != null)
                {
                    return this.referenceView.Bounds;
                }

                return RectangleF.Empty;
            }
        }

        private float relativeStatusBarHeight
        {
            get
            {
                if (!this.referenceView.GetType().IsSubclassOf(typeof(UIWindow)))
                {
                    return 0;
                }   

                return this.statusBarHeight;
            }
        }

        private float statusBarHeight 
        {
            get
            {
                switch (UIApplication.SharedApplication.StatusBarOrientation)
                {
                    case UIInterfaceOrientation.LandscapeLeft:
                    case UIInterfaceOrientation.LandscapeRight:
                        return UIApplication.SharedApplication.StatusBarFrame.Width;
                    default:
                        return UIApplication.SharedApplication.StatusBarFrame.Height;
                }
            }
        }

        private static RectangleF II_RectangleFShrink(RectangleF rect, float width, float height)
        {
            return new RectangleF(rect.X, rect.Y, rect.Width - width, rect.Height - height);
        }

        private RectangleF centerViewBounds 
        {
            get
            {
                if (this.navigationControllerBehavior == NavigationControllerBehavior.Contained)
                    return this.referenceBounds;
            
                return II_RectangleFShrink(this.referenceBounds, 
                                           0, 
                                           this.relativeStatusBarHeight + 
                                           (this.NavigationController.NavigationBarHidden ? 0 : this.NavigationController.NavigationBar.Frame.Size.Height));
            }
        }

        private static RectangleF II_RectangleFOffsetTopAndShrink(RectangleF rect, float offset)
        {
            return new RectangleF(rect.X, rect.Y, rect.Width, rect.Height - offset);
        }

        private RectangleF sideViewBounds 
        {
            get
            {
                if (this.navigationControllerBehavior == NavigationControllerBehavior.Contained)
                    return this.referenceBounds;
            
                return II_RectangleFOffsetTopAndShrink(this.referenceBounds, this.relativeStatusBarHeight);
            }
        }

        private float limitOffset(float offset) 
        {
            if (this.leftController != null && this.rightController != null) 
                return offset;
            
            if (this.leftController != null && this.maxLedge > 0) 
            {
                float left = this.referenceBounds.Size.Width - this.maxLedge;
                offset = Math.Max(offset, left);
            }
            else if (this.rightController != null && this.maxLedge > 0) 
            {
                float right = this.maxLedge - this.referenceBounds.Size.Width;
                offset = Math.Min(offset, right);
            }
            
            return offset;
        }

        private RectangleF slidingRectForOffset(float offset) 
        {
            offset = this.limitOffset(offset);

            var sz = this.slidingSizeForOffset(offset);

            return new RectangleF(this.resizesCenterView && offset < 0 ? 0 : offset, 0, sz.Width, sz.Height);
        }

        private SizeF slidingSizeForOffset(float offset) 
        {
            if (!this.resizesCenterView) 
                return this.referenceBounds.Size;
            
            offset = this.limitOffset(offset);

            if (offset < 0) 
                return new SizeF(this.centerViewBounds.Size.Width + offset, this.centerViewBounds.Size.Height);
            
            return new SizeF(this.centerViewBounds.Size.Width - offset, this.centerViewBounds.Size.Height);
        }

        private void setSlidingFrameForOffset(float offset) 
        {
            this.offset = this.limitOffset(offset);
            this.slidingControllerView.Frame = this.slidingRectForOffset(offset);

//delegate            this.performOffsetDelegate(@selector(viewDeckController:slideOffsetChanged:), this.offset);
        }

        private void hideAppropriateSideViews() 
        {
            this.leftController.View.Hidden = this.slidingControllerView.Frame.GetMinX() <= 0;

            this.rightController.View.Hidden = this.slidingControllerView.Frame.GetMaxX() >= this.referenceBounds.Size.Width;
        }

        private static bool II_FLOAT_EQUAL(float a, float b)
        {
            return (a - b == 0);
        }
        
        private static float SLIDE_DURATION(bool animated, float duration)
        {
            return animated ? duration : 0;
        }

        private static float CLOSE_SLIDE_DURATION(bool animated)
        {
            return SLIDE_DURATION(animated, 0.3f);
        }

        private static float OPEN_SLIDE_DURATION(bool animated)
        {
            return SLIDE_DURATION(animated, 0.3f);
        }


        private void setLeftLedge(float leftLedge) 
        {
            // Compute the final ledge in two steps. This prevents a strange bug where
            // nesting MAX(X, MIN(Y, Z)) with miniscule referenceBounds returns a bogus near-zero value.

            float minLedge = Math.Min(this.referenceBounds.Size.Width, leftLedge);
            leftLedge = Math.Max(leftLedge, minLedge);

            if (this.viewAppeared && II_FLOAT_EQUAL(this.slidingControllerView.Frame.Location.X, this.referenceBounds.Size.Width - this.leftLedge)) 
            {
                if (leftLedge < this.leftLedge) 
                {
                    UIView.Animate(CLOSE_SLIDE_DURATION(true), () =>
                    {
                        this.setSlidingFrameForOffset(this.referenceBounds.Size.Width - leftLedge);
                    });
                }
                else if (leftLedge > this.leftLedge) 
                {
                    UIView.Animate(OPEN_SLIDE_DURATION(true),() =>
                   {
                        this.setSlidingFrameForOffset(this.referenceBounds.Size.Width - leftLedge);
                    });
                }
            }

            this.leftLedge = leftLedge;
        }

        private void setLeftLedge(float leftLedge, Action<bool> completion)
        {
            // Compute the final ledge in two steps. This prevents a strange bug where
            // nesting MAX(X, MIN(Y, Z)) with miniscule referenceBounds returns a bogus near-zero value.

            float minLedge = Math.Min(this.referenceBounds.Size.Width, leftLedge);
            leftLedge = Math.Max(leftLedge, minLedge);

            if (this.viewAppeared && II_FLOAT_EQUAL(this.slidingControllerView.Frame.Location.X, this.referenceBounds.Size.Width - this.leftLedge)) 
            {
                if (leftLedge < this.leftLedge) 
                {
                    UIView.Animate(CLOSE_SLIDE_DURATION(true), () =>
                    {
                        this.setSlidingFrameForOffset(this.referenceBounds.Size.Width - leftLedge);
                    }, () => completion(true));
                }
                else if (leftLedge > this.leftLedge) {
                    UIView.Animate(OPEN_SLIDE_DURATION(true),() =>
                    {
                        this.setSlidingFrameForOffset(this.referenceBounds.Size.Width - leftLedge);
                    }, () => completion(true));
                }
            }

            this.leftLedge = leftLedge;
        }


        public void setRightLedge(float rightLedge) 
        {
            // Compute the final ledge in two steps. This prevents a strange bug where
            // nesting MAX(X, MIN(Y, Z)) with miniscule referenceBounds returns a bogus near-zero value.

            float minLedge = Math.Min(this.referenceBounds.Size.Width, rightLedge);
            rightLedge = Math.Max(rightLedge, minLedge);

            if (this.viewAppeared && II_FLOAT_EQUAL(this.slidingControllerView.Frame.Location.X, this.rightLedge - this.referenceBounds.Size.Width)) 
            {
                if (rightLedge < this.rightLedge) 
                {
                    UIView.Animate(CLOSE_SLIDE_DURATION(true), () =>
                    {
                        this.setSlidingFrameForOffset(rightLedge - this.referenceBounds.Size.Width);
                    });
                }
                else if (rightLedge > this.rightLedge) 
                {
                    UIView.Animate(OPEN_SLIDE_DURATION(true),() =>
                    {
                        this.setSlidingFrameForOffset(rightLedge - this.referenceBounds.Size.Width);
                    });
                }
            }

            this.rightLedge = rightLedge;
        }

        private void setRightLedge(float rightLedge, Action<bool> completion)
        {
            // Compute the final ledge in two steps. This prevents a strange bug where
            // nesting MAX(X, MIN(Y, Z)) with miniscule referenceBounds returns a bogus near-zero value.

            float minLedge = Math.Min(this.referenceBounds.Size.Width, rightLedge);
            rightLedge = Math.Max(rightLedge, minLedge);

            if (this.viewAppeared && II_FLOAT_EQUAL(this.slidingControllerView.Frame.Location.X, this.rightLedge - this.referenceBounds.Size.Width)) 
            {
                if (rightLedge < this.rightLedge) 
                {
                    UIView.Animate(CLOSE_SLIDE_DURATION(true), () =>
                    {
                        this.setSlidingFrameForOffset(rightLedge - this.referenceBounds.Size.Width);
                    }, () => completion(true));
                }
                else if (rightLedge > this.rightLedge) 
                {
                    UIView.Animate(OPEN_SLIDE_DURATION(true),() =>
                    {
                        this.setSlidingFrameForOffset(rightLedge - this.referenceBounds.Size.Width);
                    }, () => completion(true));
                }
            }

            this.rightLedge = rightLedge;
        }


        private void setMaxLedge(float maxLedge) 
        {
            this.maxLedge = maxLedge;

            if (this.leftController != null && this.rightController != null) 
            {
                Console.WriteLine("ViewDeckController: warning: setting maxLedge with 2 side controllers. Value will be ignored.");
                return;
            }
            
            if (this.leftController != null && this.leftLedge > this.maxLedge) 
            {
                this.leftLedge = this.maxLedge;
            }
            else if (this.rightController != null && this.rightLedge > this.maxLedge) 
            {
                this.rightLedge = this.maxLedge;
            }
            
            this.setSlidingFrameForOffset(this.offset);
        }


        public override void LoadView()
        {
            this.offset = 0;
            this.viewAppeared = false;

            this.View = new UIView();
            this.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            this.View.AutosizesSubviews = true;
            this.View.ClipsToBounds = true;
        }

        public override void ViewDidLoad() 
        {
            base.ViewDidLoad();
            
            this.centerView = new UIView();
            this.centerView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            this.centerView.AutosizesSubviews = true;
            this.centerView.ClipsToBounds = true;
            this.View.AddSubview(this.centerView);
            
            this.originalShadowRadius = 0;
            this.originalShadowOpacity = 0;
            this.originalShadowColor = null;
            this.originalShadowOffset = SizeF.Empty;
            this.originalShadowPath = null;
        }

        public override void ViewDidUnload()
        {
            this.CleanUp();
            base.ViewDidUnload();
        }



        public override void ViewWillAppear(bool animated) 
        {
            base.ViewWillAppear(animated);
            
            bool wasntAppeared = !this.viewAppeared;

            // was .Setting
            this.View.AddObserver(this, new NSString("bounds"),  NSKeyValueObservingOptions.Initial, IntPtr.Zero);

            NSAction applyViews = () => 
            {        
                this.centerController.View.RemoveFromSuperview();
                this.centerView.AddSubview(this.centerController.View);
                this.leftController.View.RemoveFromSuperview();
                this.referenceView.InsertSubviewBelow(this.leftController.View, this.slidingControllerView);

                this.rightController.View.RemoveFromSuperview();
                this.referenceView.InsertSubviewBelow(this.rightController.View, this.slidingControllerView);
                
                this.reapplySideController(this.leftController);
                this.reapplySideController(this.rightController);
                
                this.setSlidingFrameForOffset(this.offset);
                this.slidingControllerView.Hidden = false;
                
                this.centerView.Frame = this.centerViewBounds;
                this.centerController.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                this.centerController.View.Frame = this.centerView.Bounds;
                
                this.leftController.View.Frame = this.sideViewBounds;
                this.leftController.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                
                this.rightController.View.Frame = this.sideViewBounds;
                this.rightController.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

                this.applyShadowToSlidingView();
            };

            if (this.setSlidingAndReferenceViews()) 
            {
                applyViews();
            }

            this.viewAppeared = true;

            // after 0.01 sec, since in certain cases the sliding view is reset.
//            dispatch_after(dispatch_time(DISPATCH_TIME_NOW, 0.001 * NSEC_PER_SEC), dispatch_get_main_queue(), () =>
//            {
//                if (!this.referenceView) 
//                {
//                    this.setSlidingAndReferenceViews();
//                    applyViews();
//                }
//
//                this.setSlidingFrameForOffset(this.offset);
//                this.hideAppropriateSideViews();
//            });
            
            this.addPanners();
            
            if (this.slidingControllerView.Frame.Location.X == 0.0f) 
            {
                this.centerViewVisible();
            }
            else
            {
                this.centerViewHidden();
            }

 //           this.relayAppearanceMethod:^(UIViewController *controller) 
 //               {
 //               [controller viewWillAppear:animated);
 //           } forced:wasntAppeared);
        }

        public override void ViewDidAppear(bool animated) 
        {
            base.ViewDidAppear(animated);
            
 //           this.relayAppearanceMethod:^(UIViewController *controller) {
 //               [controller viewDidAppear:animated);
 //           });
        }

        public override void ViewWillDisappear(bool animated) 
        {
            base.ViewWillDisappear(animated);
            
 //           this.relayAppearanceMethod:^(UIViewController *controller) 
 //                               {
 //               [controller viewWillDisappear:animated);
 //           });
            
            this.removePanners();
        }

        public override void ViewDidDisappear(bool animated) 
        {
            base.ViewDidDisappear(animated);
            
            try 
            {
                this.View.RemoveObserver(this, new NSString("bounds"));
            }
            catch(Exception ex)
            {
                //do nothing, obviously it wasn't attached because an exception was thrown
            }
            
//            this.relayAppearanceMethod:^(UIViewController *controller) {
//                [controller viewDidDisappear:animated);
//            });
        }




        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            this.preRotationWidth = this.referenceBounds.Size.Width;
            this.preRotationCenterWidth = this.centerViewBounds.Size.Width;//todo: was - this.centerView.Bounds.Size.Width;
            
            if (this.rotationBehavior == RotationBehavior.KeepsViewSizes) 
           {
                this.leftWidth = this.leftController.View.Frame.Size.Width;
                this.rightWidth = this.rightController.View.Frame.Size.Width;
            }
            
            bool should = true;
            if (this.centerController != null)
                {
                should = this.centerController.ShouldAutorotateToInterfaceOrientation(toInterfaceOrientation);
          }

            return should;
        }

         public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);
            
//            this.relayAppearanceMethod:^(UIViewController *controller) {
//                [controller willAnimateRotationToInterfaceOrientation:toInterfaceOrientation duration:duration);
//            });
            
            this.arrangeViewsAfterRotation();
        }

         public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillRotate(toInterfaceOrientation, duration);
            this.restoreShadowToSlidingView();
            
//            this.relayAppearanceMethod:^(UIViewController *controller) {
//                [controller willRotateToInterfaceOrientation:toInterfaceOrientation duration:duration);
//            });
        }

         public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);
            this.applyShadowToSlidingView();
            
//            this.relayAppearanceMethod:^(UIViewController *controller) {
//                [controller didRotateFromInterfaceOrientation:fromInterfaceOrientation);
//            });
        }

        private void arrangeViewsAfterRotation() 
        {
            if (this.preRotationWidth <= 0) return;
            
            float offset = this.slidingControllerView.Frame.Location.X;

            if (this.resizesCenterView != null && offset == 0) 
            {
                offset = offset + (this.preRotationCenterWidth - this.preRotationWidth);
            }
            
            if (this.rotationBehavior == RotationBehavior.KeepsLedgeSizes) 
            {
                if (offset > 0) 
                {
                    offset = this.referenceBounds.Size.Width - this.preRotationWidth + offset;
                }
                else if (offset < 0) 
                {
                    offset = offset + this.preRotationWidth - this.referenceBounds.Size.Width;
                }
            }
            else 
            {
                this.leftLedge = this.leftLedge + this.referenceBounds.Size.Width - this.preRotationWidth; 
                this.rightLedge = this.rightLedge + this.referenceBounds.Size.Width - this.preRotationWidth; 
                this.maxLedge = this.maxLedge + this.referenceBounds.Size.Width - this.preRotationWidth; 
            }

            this.setSlidingFrameForOffset(offset);
            
            this.preRotationWidth = 0;
        }


        private bool leftControllerIsClosed 
        {
            get
            {
                return this.leftController == null || this.slidingControllerView.Frame.GetMinX() <= 0;
            }
        }

        private bool rightControllerIsClosed 
        {
            get
            {
                return this.rightController == null || this.slidingControllerView.Frame.GetMaxX() >= this.referenceBounds.Size.Width;
            }
        }

        private bool leftControllerIsOpen 
        {
            get
            {
                return this.leftController != null && this.slidingControllerView.Frame.GetMinX() < this.referenceBounds.Size.Width 
                    && this.slidingControllerView.Frame.GetMinX() >= this.rightLedge;
            }
        }

        private bool rightControllerIsOpen 
        {
            get
            {
            return this.rightController != null && this.slidingControllerView.Frame.GetMaxX() < this.referenceBounds.Size.Width 
                    && this.slidingControllerView.Frame.GetMaxX() >= this.leftLedge;
            }
        }

        private void showCenterView() 
        {
            this.showCenterView(true);
        }

        private void showCenterView(bool animated) 
        {
            this.showCenterView(animated, null);
        }

        private void showCenterView(bool animated, Action<ViewDeckController> completed)
        {
            bool mustRunCompletion = completed != null;

            if (this.leftController != null&& !this.leftController.View.Hidden) 
            {
                this.closeLeftViewAnimated(animated, completed);
                mustRunCompletion = false;
            }
            
            if (this.rightController != null && !this.rightController.View.Hidden) 
            {
                this.closeRightViewAnimated(animated, completed);
                mustRunCompletion = false;
            }
            
            if (mustRunCompletion)
                completed(this);
        }

        private bool toggleLeftView() 
        {
            return this.toggleLeftViewAnimated(true);
        }

        private bool openLeftView() 
        {
            return this.openLeftViewAnimated(true);
        }

        private bool closeLeftView()
        {
            return this.closeLeftViewAnimated(true);
        }

        private bool toggleLeftViewAnimated(bool animated)
        {
            return this.toggleLeftViewAnimated(animated, null);
        }

        private bool toggleLeftViewAnimated(bool animated, Action<ViewDeckController> completed)
        {
            if (this.leftControllerIsClosed) 
            {
                return this.openLeftViewAnimated(animated, completed);
            }
            else
            {
                return this.closeLeftViewAnimated(animated, completed);
            }
        }

        private bool openLeftViewAnimated(bool animated) 
        {
            return this.openLeftViewAnimated(animated, null);
        }

        private bool openLeftViewAnimated(bool animated, Action<ViewDeckController> completed)
        {
            return this.openLeftViewAnimated(animated, UIViewAnimationOptions.CurveEaseInOut, true, completed);
        }

        private bool openLeftViewAnimated(bool animated, bool callDelegate, Action<ViewDeckController> completed)
        {
            return this.openLeftViewAnimated(animated, UIViewAnimationOptions.CurveEaseInOut, callDelegate, completed);
        }

        private bool openLeftViewAnimated(bool animated, UIViewAnimationOptions options, bool callDelegate, Action<ViewDeckController> completed)
        {
            if (this.leftController == null || II_FLOAT_EQUAL(this.slidingControllerView.Frame.GetMinX(), this.leftLedge)) return true;


            // check the delegate to allow opening
//delegate            if (callDelegate && !this.checkDelegate:@selector(viewDeckControllerWillOpenLeftView:animated:) animated:animated]) return false;

            // also close the right view if it's open. Since the delegate can cancel the close, check the result.
//delegate            if (callDelegate && !this.closeRightViewAnimated(animated, options, callDelegate, completed]) return false;
            
            UIView.Animate(OPEN_SLIDE_DURATION(animated), 0, options | UIViewAnimationOptions.LayoutSubviews | UIViewAnimationOptions.BeginFromCurrentState, () =>
                           {
                this.leftController.View.Hidden = false;
                this.setSlidingFrameForOffset(this.referenceBounds.Size.Width - this.leftLedge);
                this.centerViewHidden();
            }, () =>
            {
                if (completed != null) completed(this);
                if (callDelegate) 
                {
//delegate                this.performDelegate:@selector(viewDeckControllerDidOpenLeftView:animated:) animated:animated);
                }

            });
            
            return true;
        }

        private bool openLeftViewBouncing(Action<ViewDeckController> bounced)
        {
            return this.openLeftViewBouncing(bounced, null);
        }

        private bool openLeftViewBouncing(Action<ViewDeckController> bounced, Action<ViewDeckController>completed) 
        {
            return this.openLeftViewBouncing(bounced, true, completed);
        }

        private bool openLeftViewBouncing(Action<ViewDeckController> bounced, bool callDelegate, Action<ViewDeckController> completed) 
        {
            return this.openLeftViewBouncing(bounced, UIViewAnimationOptions.CurveEaseInOut, true, completed);
        }

        private bool openLeftViewBouncing(Action<ViewDeckController> bounced, UIViewAnimationOptions options, bool callDelegate, Action<ViewDeckController> completed)
        {
            if (this.leftController == null || II_FLOAT_EQUAL(this.slidingControllerView.Frame.GetMinX(), this.leftLedge)) return true;
            
            // check the delegate to allow opening
//            if (callDelegate && !this.checkDelegate:@selector(viewDeckControllerWillOpenLeftView:animated:) animated:YES]) return false;

            // also close the right view if it's open. Since the delegate can cancel the close, check the result.
//            if (callDelegate && !this.closeRightViewAnimated:YES options:options callDelegate:callDelegate completion:completed]) return false;
            
            // first open the view completely, run the block (to allow changes)
            UIView.Animate(OPEN_SLIDE_DURATION(true), 0, UIViewAnimationOptions.CurveEaseIn | UIViewAnimationOptions.LayoutSubviews, () =>
            {
                this.leftController.View.Hidden = false;
                this.setSlidingFrameForOffset(this.referenceBounds.Size.Width);
            }, () => {
                // run block if it's defined
                if (bounced != null) bounced(this);
                this.centerViewHidden();
                
                // now slide the view back to the ledge position
                UIView.Animate(OPEN_SLIDE_DURATION(true), 0, options | UIViewAnimationOptions.LayoutSubviews | UIViewAnimationOptions.BeginFromCurrentState,
                               () => {
                    this.setSlidingFrameForOffset(this.referenceBounds.Size.Width - this.leftLedge);
                }, () => {
                    if (completed != null) completed(this);
//                    if (callDelegate) this.performDelegate:@selector(viewDeckControllerDidOpenLeftView:animated:) animated:YES);
                });
            });
            
            return true;
        }

        private bool closeLeftViewAnimated(bool animated) 
        {
            return this.closeLeftViewAnimated(animated, null);
        }

        private bool closeLeftViewAnimated(bool animated, Action<ViewDeckController> completed)
        {
            return this.closeLeftViewAnimated(animated,true, completed);
        }

        private bool closeLeftViewAnimated(bool animated, bool callDelegate, Action<ViewDeckController> completed) 
        {
            return this.closeLeftViewAnimated(animated, UIViewAnimationOptions.CurveEaseInOut, callDelegate, completed);
        }

        private bool closeLeftViewAnimated(bool animated, UIViewAnimationOptions options, bool callDelegate, Action<ViewDeckController> completed) 
        {
            if (this.leftControllerIsClosed) return true;
            
            // check the delegate to allow closing
//            if (callDelegate && !this.checkDelegate:@selector(viewDeckControllerWillCloseLeftView:animated:) animated:animated]) return NO;
            
            UIView.Animate(CLOSE_SLIDE_DURATION(animated), 0, options | UIViewAnimationOptions.LayoutSubviews, () => {
                this.setSlidingFrameForOffset(0);
                this.centerViewVisible();
            }, () =>  {
                this.hideAppropriateSideViews();
                if (completed != null) completed(this);
                if (callDelegate) 
                {
//                    this.performDelegate:@selector(viewDeckControllerDidCloseLeftView:animated:) animated:animated);
//                    this.performDelegate:@selector(viewDeckControllerDidShowCenterView:animated:) animated:animated);
                }
            });
            
            return true;
        }

        private bool closeLeftViewBouncing(Action<ViewDeckController> bounced) 
        {
            return this.closeLeftViewBouncing(bounced, null);
        }

        private bool closeLeftViewBouncing(Action<ViewDeckController> bounced, Action<ViewDeckController> completed) 
        {
            return this.closeLeftViewBouncing(bounced, true, completed);
        }

        private bool closeLeftViewBouncing(Action<ViewDeckController> bounced, bool callDelegate, Action<ViewDeckController> completed) 
        {
            if (this.leftControllerIsClosed) return true;
            
            // check the delegate to allow closing
//            if (callDelegate && !this.checkDelegate:@selector(viewDeckControllerWillCloseLeftView:animated:) animated:YES]) return NO;
            
            // first open the view completely, run the block (to allow changes) and close it again.
            UIView.Animate(OPEN_SLIDE_DURATION(true), 0, UIViewAnimationOptions.CurveEaseIn | UIViewAnimationOptions.LayoutSubviews,
                           () => 
                           {
                this.setSlidingFrameForOffset(this.referenceBounds.Size.Width);
            }, () => 
            {
                // run block if it's defined
                if (bounced != null) bounced(this);

//                if (callDelegate && this.delegate && [this.delegate respondsToSelector:@selector(viewDeckController:didBounceWithClosingController:)]) 
//                    [this.delegate viewDeckController:self didBounceWithClosingController:this.leftController);
                
                UIView.Animate(CLOSE_SLIDE_DURATION(true), 0, UIViewAnimationOptions.CurveEaseOut | UIViewAnimationOptions.LayoutSubviews, () => {
                    this.setSlidingFrameForOffset(0);
                    this.centerViewVisible();
                } , () => {
                    this.hideAppropriateSideViews();
                    if (completed != null) completed(this);
                    if (callDelegate) 
                    {
//                        this.performDelegate:@selector(viewDeckControllerDidCloseLeftView:animated:) animated:YES);
//                        this.performDelegate:@selector(viewDeckControllerDidShowCenterView:animated:) animated:YES);
                    }
                });
            });
            
            return true;
        }


        private bool toggleRightView() 
        {
            return this.toggleRightViewAnimated(true);
        }

        private bool openRightView() 
        {
            return this.openRightViewAnimated(true);
        }

        private bool closeRightView() 
        {
            return this.closeRightViewAnimated(true);
        }

        private bool toggleRightViewAnimated(bool animated)
        {
            return this.toggleRightViewAnimated(animated, null);
        }

        private bool toggleRightViewAnimated(bool animated, Action<ViewDeckController> completed) 
        {
            if (this.rightControllerIsClosed) 
                {
                return this.openRightViewAnimated(animated, completed);
                }
                else
                {
                    return this.closeRightViewAnimated(animated, completed);
                }
        }

        private bool openRightViewAnimated(bool animated)
        {
            return this.openRightViewAnimated(animated, null);
        }

        private bool openRightViewAnimated(bool animated, Action<ViewDeckController> completed) 
        {
            return this.openRightViewAnimated(animated, UIViewAnimationOptions.CurveEaseInOut,true, completed);
        }

        private bool openRightViewAnimated(bool animated, bool callDelegate, Action<ViewDeckController> completed)
        {
            return this.openRightViewAnimated(animated, UIViewAnimationOptions.CurveEaseInOut, callDelegate, completed);
        }

        private bool openRightViewAnimated(bool animated, UIViewAnimationOptions options, bool callDelegate, Action<ViewDeckController> completed)
        {
            if (this.rightController == null || II_FLOAT_EQUAL(this.slidingControllerView.Frame.GetMaxX(), this.rightLedge)) return true;
            
            // check the delegate to allow opening
//            if (callDelegate && !this.checkDelegate:@selector(viewDeckControllerWillOpenRightView:animated:) animated:animated]) return NO;

            // also close the left view if it's open. Since the delegate can cancel the close, check the result.
//            if (callDelegate && !this.closeLeftViewAnimated:animated options:options callDelegate:callDelegate completion:completed]) return NO;
            
            UIView.Animate(OPEN_SLIDE_DURATION(animated), 0, options | UIViewAnimationOptions.LayoutSubviews, () => {
                this.rightController.View.Hidden = false;
                this.setSlidingFrameForOffset(this.rightLedge - this.referenceBounds.Size.Width);
                this.centerViewHidden();
            }, () => {
                if (completed != null) completed(this);
//                if (callDelegate) this.performDelegate:@selector(viewDeckControllerDidOpenRightView:animated:) animated:animated);
            });

            return true;
        }

        private bool openRightViewBouncing(Action<ViewDeckController> bounced) 
        {
            return this.openRightViewBouncing(bounced, null);
        }

        private bool openRightViewBouncing(Action<ViewDeckController> bounced, Action<ViewDeckController> completed) 
        {
            return this.openRightViewBouncing(bounced, true, completed);
        }

        private bool openRightViewBouncing(Action<ViewDeckController> bounced, bool callDelegate, Action<ViewDeckController> completed)
        {
            return this.openRightViewBouncing(bounced, UIViewAnimationOptions.CurveEaseInOut, true, completed);
        }

        private bool openRightViewBouncing(Action<ViewDeckController> bounced, UIViewAnimationOptions options, bool callDelegate, Action<ViewDeckController> completed)
        {
            if (this.rightController == null || II_FLOAT_EQUAL(this.slidingControllerView.Frame.GetMinX(), this.rightLedge)) return true;
            
            // check the delegate to allow opening
//            if (callDelegate && !this.checkDelegate:@selector(viewDeckControllerWillOpenRightView:animated:) animated:YES]) return NO;

            // also close the right view if it's open. Since the delegate can cancel the close, check the result.
//            if (callDelegate && !this.closeLeftViewAnimated:YES options:options callDelegate:callDelegate completion:completed]) return NO;
            
            // first open the view completely, run the block (to allow changes)
            UIView.Animate(OPEN_SLIDE_DURATION(true), 0, UIViewAnimationOptions.CurveEaseIn | UIViewAnimationOptions.LayoutSubviews, () => {
                this.rightController.View.Hidden = false;
                this.setSlidingFrameForOffset(-this.referenceBounds.Size.Width);
            }, () =>  {
                // run block if it's defined
                if (bounced != null) bounced(this);
                this.centerViewHidden();
                
                // now slide the view back to the ledge position
                UIView.Animate(OPEN_SLIDE_DURATION(true), 0, options | UIViewAnimationOptions.LayoutSubviews | UIViewAnimationOptions.BeginFromCurrentState, () => {
                    this.setSlidingFrameForOffset(this.rightLedge - this.referenceBounds.Size.Width);
                }, () => {
                    if (completed != null) completed(this);
//                    if (callDelegate) this.performDelegate:@selector(viewDeckControllerDidOpenRightView:animated:) animated:YES);
                });
            });
            
            return true;
        }

        private bool closeRightViewAnimated(bool animated)
        {
            return this.closeRightViewAnimated(animated, null);
        }

        private bool closeRightViewAnimated(bool animated, Action<ViewDeckController> completed)
        {
            return this.closeRightViewAnimated(animated, UIViewAnimationOptions.CurveEaseInOut, true, completed);
        }

        private bool closeRightViewAnimated(bool animated, bool callDelegate, Action<ViewDeckController> completed)
        {
            return this.openRightViewAnimated(animated, UIViewAnimationOptions.CurveEaseInOut, callDelegate, completed);
        }

        private bool closeRightViewAnimated(bool animated, UIViewAnimationOptions options, bool callDelegate, Action<ViewDeckController> completed) 
        {
            if (this.rightControllerIsClosed) return true;
            
            // check the delegate to allow closing
//            if (callDelegate && !this.checkDelegate:@selector(viewDeckControllerWillCloseRightView:animated:) animated:animated]) return NO;
            
            UIView.Animate(CLOSE_SLIDE_DURATION(animated), 0, options | UIViewAnimationOptions.LayoutSubviews, () => {
                this.setSlidingFrameForOffset(0);
                this.centerViewVisible();
            }, () => {
                if (completed != null) completed(this);
                this.hideAppropriateSideViews();
                if (callDelegate) {
//                    this.performDelegate:@selector(viewDeckControllerDidCloseRightView:animated:) animated:animated);
//                    this.performDelegate:@selector(viewDeckControllerDidShowCenterView:animated:) animated:animated);
                }
            });
            
            return true;
        }

        private bool closeRightViewBouncing(Action<ViewDeckController> bounced) 
        {
            return this.closeRightViewBouncing(bounced, null);
        }

        private bool closeRightViewBouncing(Action<ViewDeckController> bounced, Action<ViewDeckController> completed) 
        {
            return this.closeRightViewBouncing(bounced, true, completed);
        }

        private bool closeRightViewBouncing(Action<ViewDeckController> bounced, bool callDelegate, Action<ViewDeckController> completed) 
        {
            if (this.rightControllerIsClosed) return true;
            
            // check the delegate to allow closing
//            if (callDelegate && !this.checkDelegate:@selector(viewDeckControllerWillCloseRightView:animated:) animated:YES]) return NO;
            
            UIView.Animate(OPEN_SLIDE_DURATION(true), 0,  UIViewAnimationOptions.CurveEaseIn | UIViewAnimationOptions.LayoutSubviews, () => {
                this.setSlidingFrameForOffset(-this.referenceBounds.Size.Width);
            }, () =>  {
                if (bounced != null) bounced(this);
//                if (callDelegate && this.delegate && [this.delegate respondsToSelector:@selector(viewDeckController:didBounceWithClosingController:)]) 
//                    [this.delegate viewDeckController:self didBounceWithClosingController:this.rightController);
                
                UIView.Animate(CLOSE_SLIDE_DURATION(true), 0, UIViewAnimationOptions.CurveEaseOut | UIViewAnimationOptions.LayoutSubviews, () => {
                    this.setSlidingFrameForOffset(0);
                    this.centerViewVisible();
                }, () =>  {
                    this.hideAppropriateSideViews();
                    if (completed != null) completed(this);
//                    this.performDelegate:@selector(viewDeckControllerDidCloseRightView:animated:) animated:YES);
//                    this.performDelegate:@selector(viewDeckControllerDidShowCenterView:animated:) animated:YES);
                });
            });
            
            return true;
        }

        private static RectangleF  RectangleFOffset(RectangleF rect, float dx, float dy)
        {
            // todo: is this correct
            return rect.Inset(dx, dy);
        }

        private void rightViewPushViewControllerOverCenterController(UIViewController controller) 
        {
            Debug.Assert(this.centerController.GetType().IsSubclassOf(typeof(UINavigationController)), "cannot rightViewPushViewControllerOverCenterView when center controller is not a navigation controller");

            UIGraphics.BeginImageContextWithOptions(this.View.Bounds.Size, true, 0);

            CGContext context = UIGraphics.GetCurrentContext();
            this.View.Layer.RenderInContext(context);

            UIImage deckshot = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            
            UIImageView shotView = new UIImageView(deckshot);
            shotView.Frame = this.View.Frame; 
            this.View.Superview.AddSubview(shotView);

            RectangleF targetFrame = this.View.Frame; 

            this.View.Frame = RectangleFOffset(this.View.Frame, this.View.Frame.Size.Width, 0);
            
            this.closeRightViewAnimated(true);

            UINavigationController navController = ((UINavigationController)this.centerController);
            navController.PushViewController(controller, false);
            
            UIView.Animate(0.3, 0, UIViewAnimationOptions.TransitionNone, () =>
           {
                shotView.Frame = RectangleFOffset(shotView.Frame, -this.View.Frame.Size.Width, 0);
                this.View.Frame = targetFrame;
            },
                () => 
                {
                shotView.RemoveFromSuperview();
            });
        }



       // #pragma mark - Pre iOS5 message relaying

        private void relayAppearanceMethod(Action<UIViewController> relay, bool forced) 
        {
//            bool shouldRelay = ![self respondsToSelector:@selector(automaticallyForwardAppearanceAndRotationMethodsToChildViewControllers)] || ![self performSelector:@selector(automaticallyForwardAppearanceAndRotationMethodsToChildViewControllers)];
//            
//            // don't relay if the controller supports automatic relaying
//            if (!shouldRelay && !forced) 
//                return;                                                                                                                                       
//            
//            relay(self.centerController);
//            relay(self.leftController);
//            relay(self.rightController);
        }

        private void relayAppearanceMethod(Action<UIViewController> relay)
        {
//            [self relayAppearanceMethod:relay forced:NO];
        }

        //#pragma mark - center view hidden stuff

        private void centerViewVisible()
        {
            this.removePanners();
            if (this.centerTapper != null) 
            {
// todo:                this.centerTapper.RemoveTarget(this, @selector(centerTapped), UIControlEventTouchUpInside);
                this.centerTapper.RemoveFromSuperview();
            }

            this.centerTapper = null;
            this.addPanners();
        }

        private void centerViewHidden() 
        {
            if (this.centerHiddenInteractivity == CenterHiddenInteractivity.UserInteractive) 
                return;
            
            this.removePanners();

            if (this.centerTapper == null) 
            {
                this.centerTapper =  new UIButton(UIButtonType.Custom);
                this.centerTapper.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                this.centerTapper.Frame = this.centerView.Bounds;
                this.centerView.AddSubview(this.centerTapper);
                this.centerTapper.AddTarget(this, new Selector("centerTapped"), UIControlEvent.TouchUpInside);
                this.centerTapper.BackgroundColor = UIColor.Clear;
                
            }

            this.centerTapper.Frame = this.centerView.Bounds;
            this.addPanners();
        }

        [Export("centerTapped")]
        private void centerTapped() 
        {
            // todo: handle additinal cases better
            if (this.centerHiddenInteractivity != CenterHiddenInteractivity.UserInteractive) 
            {
                if (this.leftController != null && this.slidingControllerView.Frame.GetMinX() > 0) 
                {
                    if (this.centerHiddenInteractivity == CenterHiddenInteractivity.NotUserInteractiveWithTapToClose) 
                    {
                        this.closeLeftView();
                    }
                    else
                    {
                        this.closeLeftViewBouncing(null);
                    }
                }

                if (this.rightController != null && this.slidingControllerView.Frame.GetMinX() < 0) 
                {
                    if (this.centerHiddenInteractivity == CenterHiddenInteractivity.NotUserInteractiveWithTapToClose) 
                    {
                        this.closeRightView();
                    }
                    else
                    {
                        this.closeRightViewBouncing(null);
                    }
                }
                
            }
        }

        //#pragma mark - Panning

        [Export("gestureRecognizerShouldBegin:")]
        private bool gestureRecognizerShouldBegin(UIGestureRecognizer gestureRecognizer)
        {
            float px = this.slidingControllerView.Frame.Location.X;
            if (px != 0) return true;
                
            float x = this.locationOfPanner((UIPanGestureRecognizer)gestureRecognizer);
            bool ok =  true;

            if (x > 0) 
            {
// todo                ok = [self checkDelegate:@selector(viewDeckControllerWillOpenLeftView:animated:) animated:NO];
                if (!ok)
                    this.closeLeftViewAnimated(false);
            }
            else if (x < 0) 
            {
// todo                ok = [self checkDelegate:@selector(viewDeckControllerWillOpenRightView:animated:) animated:NO];
                if (!ok)
                    this.closeRightViewAnimated(false);
            }
            
            return ok;
        }

        [Export("gestureRecognizer:shouldReceiveTouch:")]
        private bool gestureRecognizer(UIGestureRecognizer gestureRecognizer, UITouch touch) 
        {
            this.panOrigin = this.slidingControllerView.Frame.Location.X;
            return true;
        }

        private float locationOfPanner(UIPanGestureRecognizer panner) 
        {
            PointF pan = panner.TranslationInView(this.referenceView);
            float x = pan.X + this.panOrigin;
            
            if (this.leftController == null) x = Math.Min(0, x);

            if (this.rightController == null) x = Math.Max(0, x);
            
            float w = this.referenceBounds.Size.Width;
            float lx = Math.Max(Math.Min(x, w - this.leftLedge), -w + this.rightLedge);
            
            if (this.elastic) 
            {
                float dx = Math.Abs(x) - Math.Abs(lx);

                if (dx > 0) 
                {
                    dx = dx / (float)Math.Log(dx + 1) * 2;
                    x = lx + (x < 0 ? -dx : dx);
                }
            }
            else 
            {
                x = lx;
            }
            
            return this.limitOffset(x);
        }

        [Export("panned:")]
        private void panned(UIPanGestureRecognizer panner) 
        {
            if (!this.enabled) return;

            float px = this.slidingControllerView.Frame.Location.X;
            float x = this.locationOfPanner(panner);
            float w = this.referenceBounds.Size.Width;

            Selector didCloseSelector = null;
            Selector didOpenSelector = null;
            
            // if we move over a boundary while dragging, ... 
            if (px <= 0 && x >= 0 && px != x) 
            {
                // ... then we need to check if the other side can open.
                if (px < 0) 
                {
                    bool canClose = true;// todo: this.checkDelegate:@selector(viewDeckControllerWillCloseRightView:animated:) animated:NO];
                    if (!canClose)
                        return;
                    didCloseSelector = new Selector("viewDeckControllerDidCloseRightView:animated:");
                }

                if (x > 0) 
                {
                    bool canOpen = true;// todo: [self checkDelegate:@selector(viewDeckControllerWillOpenLeftView:animated:) animated:NO];
                    didOpenSelector = new Selector("viewDeckControllerDidOpenLeftView:animated:");
                    if (!canOpen) 
                    {
                        this.closeRightViewAnimated(false);
                        return;
                    }
                }
            }
            else if (px >= 0 && x <= 0 && px != x) 
            {
                if (px > 0) 
                {
                    bool canClose = true;// todo: [self checkDelegate:@selector(viewDeckControllerWillCloseLeftView:animated:) animated:NO];
                    if (!canClose) 
                    {
                        return;
                    }

                    didCloseSelector = new Selector("viewDeckControllerDidCloseLeftView:animated:");
                }

                if (x < 0) 
                {
                    bool canOpen = true;// todo: [self checkDelegate:@selector(viewDeckControllerWillOpenRightView:animated:) animated:NO];
                    didOpenSelector = new Selector("viewDeckControllerDidOpenRightView:animated:");
                    if (!canOpen) 
                    {
                        this.closeLeftViewAnimated(false);
                        return;
                    }
                }
            }
            
            this.setSlidingFrameForOffset(x);
            
            bool rightWasHidden = this.rightController.View.Hidden;
            bool leftWasHidden = this.leftController.View.Hidden;
            
            // todo: [self performOffsetDelegate:@selector(viewDeckController:didPanToOffset:) offset:x];
            
            if (panner.State == UIGestureRecognizerState.Ended) 
            {
                if (this.slidingControllerView.Frame.Location.X == 0.0f) 
                {
                    this.centerViewVisible();
                }
                else
                {
                    this.centerViewHidden();
                }

                float lw3 = (w - this.leftLedge) / 3.0f;
                float rw3 = (w - this.rightLedge) / 3.0f;
                float velocity = panner.VelocityInView(this.referenceView).X;

                if (Math.Abs(velocity) < 500) 
                {
                    // small velocity, no movement
                    if (x >= w - this.leftLedge - lw3) 
                    {
                        this.openLeftViewAnimated(true, UIViewAnimationOptions.CurveEaseOut, false, null);
                    }
                    else if (x <= this.rightLedge + rw3 - w) 
                    {
                        this.openRightViewAnimated(true, UIViewAnimationOptions.CurveEaseOut, false, null);
                    }
                    else
                    {
                        this.showCenterView(true);
                    }
                }
                else if (velocity < 0) 
                {
                    // swipe to the left
                    if (x < 0) 
                    {
                        this.openRightViewAnimated(true, UIViewAnimationOptions.CurveEaseOut, true, null);
                    }
                    else 
                    {
                        this.showCenterView(true);
                    }
                }
                else if (velocity > 0) 
                {
                    // swipe to the right
                    if (x > 0) 
                    {
                        this.openLeftViewAnimated(true, UIViewAnimationOptions.CurveEaseOut, true, null);
                    }
                    else 
                    {
                        this.showCenterView(true);
                    }
                }
            }
            else
            {
                this.hideAppropriateSideViews();
            }

            if (didCloseSelector != null)
            {
                // todo: [self performDelegate:didCloseSelector animated:NO];
            }

            if (didOpenSelector != null)
            {
                // todo: [self performDelegate:didOpenSelector animated:NO];
            }
        }


        private void addPanner(UIView view) 
        {
            if (view == null) return;

            UIPanGestureRecognizer panner = new UIPanGestureRecognizer(this, new Selector("panned:"));

            panner.CancelsTouchesInView = true;
            panner.WeakDelegate = this;

            this.View.AddGestureRecognizer(panner);
            this.panners.Add(panner);
        }


        private void addPanners() 
        {
            this.removePanners();
            
            switch (this.panningMode) 
            {
                case PanningMode.NoPanning: 
                    break;
                    
                case PanningMode.FullViewPanning:
                    this.addPanner(this.slidingControllerView);

                    // also add to disabled center
                    if (this.centerTapper != null)
                        this.addPanner(this.centerTapper);

                    // also add to navigationbar if present
                    if (this.NavigationController != null && !this.NavigationController.NavigationBarHidden) 
                        this.addPanner(this.NavigationController.NavigationBar);

                    break;
                    
                case PanningMode.NavigationBarPanning:
                    if (this.NavigationController != null && !this.NavigationController.NavigationBarHidden) 
                    {
                        this.addPanner(this.NavigationController.NavigationBar);
                    }
                    
                    if (this.centerController.NavigationController != null && !this.centerController.NavigationController.NavigationBarHidden) 
                    {
                        this.addPanner(this.centerController.NavigationController.NavigationBar);
                    }
                    
                    if (this.centerController.GetType().IsSubclassOf(typeof(UINavigationController)) && !((UINavigationController)this.centerController).NavigationBarHidden) 
                    {
                        this.addPanner(((UINavigationController)this.centerController).NavigationBar);
                    }

                    break;
                case PanningMode.PanningViewPanning:
                    if (this.panningView != null) 
                    {
                        this.addPanner(this.panningView);
                    }

                    break;
            }
        }


        private void removePanners() 
        {
            foreach (var panner in this.panners) 
            {
                panner.View.RemoveGestureRecognizer(panner);
            }

            this.panners.Clear();
        }

        //#pragma mark - Delegate convenience methods

//        private bool checkDelegate(SEL selector, bool animated) 
//        {
//            BOOL ok = YES;
//            // used typed message send to properly pass values
//            BOOL (*objc_msgSendTyped)(id self, SEL _cmd, IIViewDeckController* foo, BOOL animated) = (void*)objc_msgSend;
//            
//            if (self.delegate && [self.delegate respondsToSelector:selector]) 
//                ok = ok & objc_msgSendTyped(self.delegate, selector, self, animated);
//            
//            for (UIViewController* controller in self.controllers) {
//                // check controller first
//                if ([controller respondsToSelector:selector] && (id)controller != (id)self.delegate) 
//                    ok = ok & objc_msgSendTyped(controller, selector, self, animated);
//                // if that fails, check if it's a navigation controller and use the top controller
//                else if ([controller isKindOfClass:[UINavigationController class]]) {
//                    UIViewController* topController = ((UINavigationController*)controller).topViewController;
//                    if ([topController respondsToSelector:selector] && (id)topController != (id)self.delegate) 
//                        ok = ok & objc_msgSendTyped(topController, selector, self, animated);
//                }
//            }
//            
//            return ok;
//        }

//        private void performDelegate(SEL selector, bool animated) 
//        {
//            // used typed message send to properly pass values
//            void (*objc_msgSendTyped)(id self, SEL _cmd, IIViewDeckController* foo, BOOL animated) = (void*)objc_msgSend;
//
//            if (self.delegate && [self.delegate respondsToSelector:selector]) 
//                objc_msgSendTyped(self.delegate, selector, self, animated);
//            
//            for (UIViewController* controller in self.controllers) {
//                // check controller first
//                if ([controller respondsToSelector:selector] && (id)controller != (id)self.delegate) 
//                    objc_msgSendTyped(controller, selector, self, animated);
//                // if that fails, check if it's a navigation controller and use the top controller
//                else if ([controller isKindOfClass:[UINavigationController class]]) {
//                    UIViewController* topController = ((UINavigationController*)controller).topViewController;
//                    if ([topController respondsToSelector:selector] && (id)topController != (id)self.delegate) 
//                        objc_msgSendTyped(topController, selector, self, animated);
//                }
//            }
//        }

//        private void performOffsetDelegate(SEL selector, float offset) 
//        {
//            void (*objc_msgSendTyped)(id self, SEL _cmd, IIViewDeckController* foo, CGFloat offset) = (void*)objc_msgSend;
//            if (self.delegate && [self.delegate respondsToSelector:selector]) 
//                objc_msgSendTyped(self.delegate, selector, self, offset);
//            
//            for (UIViewController* controller in self.controllers) {
//                // check controller first
//                if ([controller respondsToSelector:selector] && (id)controller != (id)self.delegate) 
//                    objc_msgSendTyped(controller, selector, self, offset);
//                
//                // if that fails, check if it's a navigation controller and use the top controller
//                else if ([controller isKindOfClass:[UINavigationController class]]) {
//                    UIViewController* topController = ((UINavigationController*)controller).topViewController;
//                    if ([topController respondsToSelector:selector] && (id)topController != (id)self.delegate) 
//                        objc_msgSendTyped(topController, selector, self, offset);
//                }
//            }
//        }


       // #pragma mark - Properties


        private string __title;
        public override string Title
        {
            get
            {
                return this.centerController.Title;
            }

            set
            {
                if (this.__title != value)
                {
                    this.__title = value;
                    base.Title = value;
                    this.centerController.Title = value;
                }
            }
        }

         // property setter
        private void setPanningMode(PanningMode panningMode)
        {
            if (this.viewAppeared) 
            {
                this.removePanners();
                this.panningMode = panningMode;
                this.addPanners();
            }
            else
            {
                this.panningMode = panningMode;
            }
        }

          // property setter
        private void setPanningView(UIView panningView) 
        {
            if (this.panningView != panningView) 
            {
                // todo: dispose ??
                //II_RELEASE(_panningView);
                this.panningView = panningView;
                //II_RETAIN(_panningView);
                
                if (this.viewAppeared && this.panningMode == PanningMode.PanningViewPanning)
                {
                    this.addPanners();
                }
            }
        }

          // property setter
        private void setNavigationControllerBehavior(NavigationControllerBehavior navigationControllerBehavior) 
        {
            if (!this.viewAppeared) 
            {
                this.navigationControllerBehavior = navigationControllerBehavior;
            }
            else 
            {
                throw new InvalidOperationException("Cannot set navigationcontroller behavior when the view deck is already showing.");
            }
        }

        // todo: is controllerStore a ref parametsr
        private void applySideController(ref UIViewController controllerStore, UIViewController newController, UIViewController otherController, 
                                             NSAction clearOtherController) 
        {
            //void(^beforeBlock)(UIViewController* controller) = ^(UIViewController* controller){};
            //void(^afterBlock)(UIViewController* controller, BOOL left) = ^(UIViewController* controller, BOOL left){};

            Action<UIViewController> beforeBlock = (x) => {};
            Action<UIViewController, bool> afterBlock = (x, y) => {};

            if (this.viewAppeared) 
            {
                beforeBlock = (controller) => 
                {
//                    controller.vdc_viewWillDisappear(false);
                    controller.View.RemoveFromSuperview();
//                    controller.vdc_viewDidDisappear(false);
                };

                afterBlock = (controller, left) => 
                {
//                    controller.vdc_viewWillAppear(false);
                    controller.View.Hidden = left ? this.slidingControllerView.Frame.Location.X <= 0 : this.slidingControllerView.Frame.Location.X >= 0;
                    controller.View.Frame = this.referenceBounds;
                    controller.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                    if (this.slidingController != null)
                    {
                        this.referenceView.InsertSubviewBelow(controller.View, this.slidingControllerView);
                    }
                    else
                    {
                        this.referenceView.AddSubview(controller.View);
                    }

//                    controller.vdc_viewDidAppear(false);
                };
            }
            
            // start the transition
            if (controllerStore != null) 
            {
                controllerStore.WillMoveToParentViewController(null);
                if (newController == this.centerController) this.centerController = null;
                if (newController == otherController && clearOtherController != null) clearOtherController();

                beforeBlock(controllerStore);

//                controllerStore.setViewDeckController(null);
                controllerStore.RemoveFromParentViewController();
                controllerStore.DidMoveToParentViewController(null);
            }
            
            // make the switch
            if (controllerStore != newController) 
            {
                // todo: dispose II_RELEASE(*controllerStore);
                controllerStore = newController;
                //II_RETAIN(*controllerStore);
            }
            
            if (controllerStore != null) 
            {
                newController.WillMoveToParentViewController(null);
                newController.RemoveFromParentViewController();
                newController.DidMoveToParentViewController(null);
                
                // and finish the transition
                UIViewController parentController = (this.referenceView == this.View) ? this : this.GetGrandParent();
                if (parentController != null)
                {
                    parentController.AddChildViewController(controllerStore);
                }

//                controllerStore.setViewDeckController(this);

                afterBlock(controllerStore, controllerStore == this.leftController);

                controllerStore.DidMoveToParentViewController(parentController);
            }
        }

        private UIViewController GetGrandParent()
        {
            if (this.ParentViewController != null)
            {
                return this.ParentViewController.ParentViewController;
            }

            return null;
        }

        private void reapplySideController(UIViewController controllerStore) 
        {
            this.applySideController(ref controllerStore, controllerStore, null, null);
        }

        // property setter
        private void setLeftController(UIViewController leftController) 
        {
            if (this.leftController == leftController) 
            {
                return;
            }

            this.applySideController(ref this.leftController, leftController, this.rightController, () => { this.rightController = null; });
        }

        // property setter
        private void setRightController(UIViewController rightController)
        {
            if (this.rightController == rightController) 
            {
                return;
            }

            this.applySideController(ref this.rightController, rightController, this.leftController, () => { this.leftController = null; });
        }


        // property setter
        private void setCenterController(UIViewController centerController) 
        {
            if (this.centerController == centerController) return;
            
            //void(^beforeBlock)(UIViewController* controller) = ^(UIViewController* controller){};
            //void(^afterBlock)(UIViewController* controller) = ^(UIViewController* controller){};
            Action<UIViewController> beforeBlock = (x) => {};
            Action<UIViewController> afterBlock = (x) => {};


            RectangleF currentFrame = this.referenceBounds;

            if (this.viewAppeared) 
            {
                beforeBlock = (controller) => 
                {
// todo:                    controller.vdc_viewWillDisappear(false);
                    this.restoreShadowToSlidingView();
                    this.removePanners();
                    controller.View.RemoveFromSuperview();
// todo:                    controller.vdc_viewDidDisappear(false);
                    this.centerView.RemoveFromSuperview();
                };

                afterBlock = (controller) => 
                {
                    this.View.AddSubview(this.centerView);
// todo:                    controller.vdc_viewWillAppear(false);

                    UINavigationController navController = centerController.GetType().IsSubclassOf(typeof(UINavigationController)) 
                    ? (UINavigationController)centerController 
                    : null;

                    bool barHidden = false;
                    if (navController != null && !navController.NavigationBarHidden) 
                    {
                        barHidden = true;
                        navController.NavigationBarHidden = true;
                    }
                    
                    this.setSlidingAndReferenceViews();
                    controller.View.Frame = currentFrame;
                    controller.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                    controller.View.Hidden = false;
                    this.centerView.AddSubview(controller.View);
                    
                    if (barHidden) 
                        navController.NavigationBarHidden = false;
                    
                    this.addPanners();
                    this.applyShadowToSlidingView();
// todo:                    controller.vdc_viewDidAppear(false);
                };
            }
            
            // start the transition
            if (this.centerController != null) 
            {
                currentFrame = this.centerController.View.Frame;
                this.centerController.WillMoveToParentViewController(null);

                if (centerController == this.leftController) this.leftController = null;
                if (centerController == this.rightController) this.rightController = null;


                beforeBlock(this.centerController);

                try 
                {
                    this.centerController.RemoveObserver(this, new NSString("title"));
                    if (this.automaticallyUpdateTabBarItems) 
                    {
                        this.centerController.RemoveObserver(this, new NSString("tabBarItem.title"));
                        this.centerController.RemoveObserver(this, new NSString("tabBarItem.image"));
                        this.centerController.RemoveObserver(this, new NSString("hidesBottomBarWhenPushed"));
                    }
                }
                catch (Exception ex) 
                {
                    // gobble
                }

// todo:                this.centerController.setViewDeckController(null);
                this.centerController.RemoveFromParentViewController();

                
                this.centerController.DidMoveToParentViewController(null);
                // todo: dispose ? II_RELEASE(_centerController);
            }
            
            // make the switch
            this.centerController = centerController;
            
            if (this.centerController != null) 
            {
                // and finish the transition
                //II_RETAIN(_centerController);
                this.AddChildViewController(this.centerController);

// todo:                this.centerController.setViewDeckController(this);
                this.centerController.AddObserver(this, new NSString("title"), 0, IntPtr.Zero);

                this.Title = this.centerController.Title;

                if (this.automaticallyUpdateTabBarItems) 
                {
                    this.centerController.AddObserver(this, new NSString("tabBarItem.title"), 0, IntPtr.Zero);
                    this.centerController.AddObserver(this, new NSString("tabBarItem.image"), 0, IntPtr.Zero);
                    this.centerController.AddObserver(this, new NSString("hidesBottomBarWhenPushed"), 0, IntPtr.Zero);
                    
                    this.TabBarItem.Title = this.centerController.TabBarItem.Title;
                    this.TabBarItem.Image = this.centerController.TabBarItem.Image;
                    this.HidesBottomBarWhenPushed = this.centerController.HidesBottomBarWhenPushed;
                }
                
                afterBlock(this.centerController);

                this.centerController.DidMoveToParentViewController(this);
            }    
        }

        private void setAutomaticallyUpdateTabBarItems(bool automaticallyUpdateTabBarItems) 
        {
//            if (_automaticallyUpdateTabBarItems) {
//                @try {
//                    [_centerController removeObserver:self forKeyPath:@"tabBarItem.title"];
//                    [_centerController removeObserver:self forKeyPath:@"tabBarItem.image"];
//                    [_centerController removeObserver:self forKeyPath:@"hidesBottomBarWhenPushed"];
//                }
//                @catch (NSException *exception) {}
//            }
//            
//            _automaticallyUpdateTabBarItems = automaticallyUpdateTabBarItems;
//
//            if (_automaticallyUpdateTabBarItems) {
//                [_centerController addObserver:self forKeyPath:@"tabBarItem.title" options:0 context:nil];
//                [_centerController addObserver:self forKeyPath:@"tabBarItem.image" options:0 context:nil];
//                [_centerController addObserver:self forKeyPath:@"hidesBottomBarWhenPushed" options:0 context:nil];
//                self.tabBarItem.title = _centerController.tabBarItem.title;
//                self.tabBarItem.image = _centerController.tabBarItem.image;
//            }
        }


        private bool setSlidingAndReferenceViews() 
        {
            if (this.NavigationController != null && this.navigationControllerBehavior == NavigationControllerBehavior.Integrated) 
            {
                if (this.NavigationController.View.Superview != null) 
                {
                    this.slidingController = this.NavigationController;
                    this.referenceView = this.NavigationController.View.Superview;
                    return true;
                }
            }
            else 
            {
                this.slidingController = this.centerController;
                this.referenceView = this.View;
                return true;
            }
            
            return false;
        }

        private UIView slidingControllerView 
        {
            get
            {
                if (this.NavigationController != null && this.navigationControllerBehavior == NavigationControllerBehavior.Integrated) 
                {
                    return this.slidingController.View;
                }
                else {
                    return this.centerView;
                }
            }
        }

        //#pragma mark - observation

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
//            if (object == _centerController) {
//                if ([@"tabBarItem.title" isEqualToString:keyPath]) {
//                    self.tabBarItem.title = _centerController.tabBarItem.title;
//                    return;
//                }
//                
//                if ([@"tabBarItem.image" isEqualToString:keyPath]) {
//                    self.tabBarItem.image = _centerController.tabBarItem.image;
//                    return;
//                }
//
//                if ([@"hidesBottomBarWhenPushed" isEqualToString:keyPath]) {
//                    self.hidesBottomBarWhenPushed = _centerController.hidesBottomBarWhenPushed;
//                    self.tabBarController.hidesBottomBarWhenPushed = _centerController.hidesBottomBarWhenPushed;
//                    return;
//                }
//            }
//
//            if ([@"title" isEqualToString:keyPath]) {
//                if (!II_STRING_EQUAL([super title], self.centerController.title)) {
//                    self.title = self.centerController.title;
//                }
//                return;
//            }
//            
//            if ([keyPath isEqualToString:@"bounds"]) {
//                CGFloat offset = self.slidingControllerView.Frame.Location.X;
//                [self setSlidingFrameForOffset:offset];
//                self.slidingControllerView.layer.shadowPath = [UIBezierPath bezierPathWithRect:self.referenceBounds].CGPath;
//                UINavigationController* navController = [self.centerController isKindOfClass:[UINavigationController class]] 
//                ? (UINavigationController*)self.centerController 
//                : nil;
//                if (navController != nil && !navController.navigationBarHidden) {
//                    navController.navigationBarHidden = YES;
//                    navController.navigationBarHidden = NO;
//                }
//                return;
//            }
        }

       // #pragma mark - Shadow

        private void restoreShadowToSlidingView() 
        {
//            UIView* shadowedView = self.slidingControllerView;
//            if (!shadowedView) return;
//            
//            shadowedView.layer.shadowRadius = self.LocationalShadowRadius;
//            shadowedView.layer.shadowOpacity = self.LocationalShadowOpacity;
//            shadowedView.layer.shadowColor = [self.LocationalShadowColor CGColor]; 
//            shadowedView.layer.shadowOffset = self.LocationalShadowOffset;
//            shadowedView.layer.shadowPath = [self.LocationalShadowPath CGPath];
        }

        private void applyShadowToSlidingView() 
        {
            UIView shadowedView = this.slidingControllerView;
            if (shadowedView == null) return;
            
            this.originalShadowRadius = shadowedView.Layer.ShadowRadius;
            this.originalShadowOpacity = shadowedView.Layer.ShadowOpacity;
            this.originalShadowColor = shadowedView.Layer.ShadowColor != null ? UIColor.FromCGColor(this.slidingControllerView.Layer.ShadowColor) : null;
            this.originalShadowOffset = shadowedView.Layer.ShadowOffset;
//            this.originalShadowPath = shadowedView.Layer.ShadowPath  != null  ? UIBezierPath.FromPath(this.slidingControllerView.Layer.ShadowPath) : null;
            
//            if ([this.delegate respondsToSelector:@selector(viewDeckController:applyShadow:withBounds:)]) 
//            {
//                [this.delegate viewDeckController:this applyShadow:shadowedView.layer withBounds:self.referenceBounds];
//            }
//            else 
            {
                shadowedView.Layer.MasksToBounds = false;
                shadowedView.Layer.ShadowRadius = 10;
                shadowedView.Layer.ShadowOpacity = 0.5f;
                shadowedView.Layer.ShadowColor = UIColor.Black.CGColor;
                shadowedView.Layer.ShadowOffset = SizeF.Empty;
                shadowedView.Layer.ShadowPath = UIBezierPath.FromRect(shadowedView.Bounds).CGPath;
            }
        }

         

    }


}

