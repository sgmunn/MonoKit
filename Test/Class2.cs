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
    public enum ViewDeckPanningMode
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

    public enum ViewDeckNavigationControllerBehavior
    {
        Contained,
        Integrated
    }

    public enum ViewDeckRotationBehavior
    {
        KeepsLedgeSizes,
        KeepsViewSizes
    }

    public class ViewDeckController : UIViewController
    {
        private readonly List<UIGestureRecognizer> panners;

        private CenterHiddenInteractivity centerHiddenInteractivity;
        private bool viewAppeared;
        private bool resizesCenterView;

        private UIViewController slidingController;

        private float originalShadowRadius;
        private SizeF originalShadowOffset;
        private UIColor originalShadowColor;
        private float originalShadowOpacity;

        private UIView referenceView;
        private UIBezierPath originalShadowPath;
        private UIView centerView;
        private UIButton centerTapper;

        private float offset;
        private float preRotationWidth;
        private float preRotationCenterWidth;
        private float leftWidth;
        private float rightWidth;
        private float panOrigin;


        private UIViewController _centerController;
        private UIViewController _leftController;
        private UIViewController _rightController;
        private float _rightLedge;
        private float _leftLedge;
        private ViewDeckNavigationControllerBehavior _navigationControllerBehavior;
        private ViewDeckPanningMode _panningMode;
        private UIView _panningView;
        private float _maxLedge;
        private bool _automaticallyUpdateTabBarItems;



        public ViewDeckController(UIViewController centerController)
        {
            this.panners = new List<UIGestureRecognizer>();
            this.Enabled = true;
            this.Elastic = true;

// ??             this.originalShadowColor = UIColor.Clear;

            this.RotationBehavior = ViewDeckRotationBehavior.KeepsLedgeSizes;

            this.PanningMode = ViewDeckPanningMode.FullViewPanning;
            this.centerHiddenInteractivity = CenterHiddenInteractivity.UserInteractive;

            this.LeftLedge = 44;
            this.RightLedge = 44;
        
            this.CenterController = centerController;
        }


        public ViewDeckController(UIViewController centerController, UIViewController leftController) : this(centerController)
        {
            this.LeftController = leftController;
        }

        public ViewDeckController(UIViewController centerController, UIViewController leftController, UIViewController rightController) : this(centerController)
        {
            this.LeftController = leftController;
            this.RightController = rightController;
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


        #region Public Properties

        /// <summary>
        /// </summary>
        public UIViewController CenterController
        {
            get
            {
                return this._centerController;
            }

            set
            {
                this.SetCenterController(value);
            }
        }

        /// <summary>
        /// </summary>
        public UIViewController LeftController
        {
            get
            {
                return this._leftController;
            }

            set
            {
                if (this.LeftController == value) 
                {
                    return;
                }

                this.ApplySideController(ref this._leftController, value, this.RightController, () => { this.RightController = null; });
            }
        }

        /// <summary>
        /// </summary>
        public UIViewController RightController
        {
            get
            {
                return this._rightController;
            }

            set
            {
                if (this.RightController == value) 
                {
                    return;
                }

                this.ApplySideController(ref this._rightController, value, this.LeftController, () => { this.LeftController = null; });
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the view deck is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the deck can be stretched past the ledges.
        /// </summary>
        public bool Elastic { get; set; }

        /// <summary>
        /// </summary>
        public float RightLedge
        {
            get
            {
                return this._rightLedge;
            }

            set
            {
                this.SetRightLedge(value);
            }
        }

        /// <summary>
        /// </summary>
        public float LeftLedge
        {
            get
            {
                return this._leftLedge;
            }

            set
            {
                this.SetLeftLedge(value);
            }
        }
        
        /// <summary>
        /// </summary>
        public float MaxLedge
        {
            get
            {
                return this._maxLedge;
            }

            set
            {
                this._maxLedge = value;

                if (this.LeftController != null && this.RightController != null) 
                {
                    Console.WriteLine("ViewDeckController: warning: setting maxLedge with 2 side controllers. Value will be ignored.");
                    return;
                }
                
                if (this.LeftController != null && this.LeftLedge > this.MaxLedge) 
                {
                    this.LeftLedge = value;
                }
                else if (this.RightController != null && this.RightLedge > this.MaxLedge) 
                {
                    this.RightLedge = value;
                }
                
                this.SetSlidingFrameForOffset(this.offset);
            }
        }

        /// <summary>
        /// </summary>
        public ViewDeckNavigationControllerBehavior NavigationControllerBehavior
        {
            get
            {
                return this._navigationControllerBehavior;
            }

            set
            {
                if (this.viewAppeared) 
                {
                    throw new InvalidOperationException("Cannot set navigationcontroller behavior when the view deck is already showing.");
                }

                this._navigationControllerBehavior = value;
            }
        }

        /// <summary>
        /// </summary>
        public ViewDeckPanningMode PanningMode
        {
            get
            {
                return this._panningMode;
            }

            set
            {
                if (this.viewAppeared) 
                {
                    this.RemovePanners();
                    this._panningMode = value;
                    this.AddPanners();
                }
                else
                {
                    this._panningMode = value;
                }

            }
        }

        /// <summary>
        /// </summary>
        public UIView PanningView
        {
            get
            {
                return this._panningView;
            }

            set
            {
                if (this._panningView != value) 
                {
                    // todo: dispose ??
                    //II_RELEASE(_panningView);
                    this._panningView = value;
                    //II_RETAIN(_panningView);
                    
                    if (this.viewAppeared && this.PanningMode == ViewDeckPanningMode.PanningViewPanning)
                    {
                        this.AddPanners();
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        public ViewDeckRotationBehavior RotationBehavior { get; set; }

        /// <summary>
        /// </summary>
        public bool AutomaticallyUpdateTabBarItems
        {
            get
            {
                return this._automaticallyUpdateTabBarItems;
            }

            set
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
                this._automaticallyUpdateTabBarItems = value;
            }
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// </summary>
        private UIView SlidingControllerView 
        {
            get
            {
                if (this.NavigationController != null && this.NavigationControllerBehavior == ViewDeckNavigationControllerBehavior.Integrated) 
                {
                    return this.slidingController.View;
                }
                else 
                {
                    return this.centerView;
                }
            }
        }
        
        /// <summary>
        /// </summary>
        private bool LeftControllerIsClosed 
        {
            get
            {
                return this.LeftController == null || this.SlidingControllerView.Frame.GetMinX() <= 0;
            }
        }

        /// <summary>
        /// </summary>
        private bool RightControllerIsClosed 
        {
            get
            {
                return this.RightController == null || this.SlidingControllerView.Frame.GetMaxX() >= this.ReferenceBounds.Size.Width;
            }
        }

        /// <summary>
        /// todo: unused
        /// </summary>
        private bool LeftControllerIsOpen 
        {
            get
            {
                return this.LeftController != null && this.SlidingControllerView.Frame.GetMinX() < this.ReferenceBounds.Size.Width 
                    && this.SlidingControllerView.Frame.GetMinX() >= this.RightLedge;
            }
        }

        /// <summary>
        /// todo: unused
        /// </summary>
        private bool RightControllerIsOpen 
        {
            get
            {
                return this.RightController != null && this.SlidingControllerView.Frame.GetMaxX() < this.ReferenceBounds.Size.Width 
                    && this.SlidingControllerView.Frame.GetMaxX() >= this.LeftLedge;
            }
        }

        private RectangleF ReferenceBounds
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

        private float RelativeStatusBarHeight
        {
            get
            {
                if (!this.referenceView.GetType().IsSubclassOf(typeof(UIWindow)))
                {
                    return 0;
                }   

                return this.StatusBarHeight;
            }
        }

        private float StatusBarHeight 
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

        private RectangleF CenterViewBounds 
        {
            get
            {
                if (this.NavigationControllerBehavior == ViewDeckNavigationControllerBehavior.Contained)
                    return this.ReferenceBounds;
            
                return II_RectangleFShrink(this.ReferenceBounds, 
                                           0, 
                                           this.RelativeStatusBarHeight + 
                                           (this.NavigationController.NavigationBarHidden ? 0 : this.NavigationController.NavigationBar.Frame.Size.Height));
            }
        }

        private static RectangleF II_RectangleFOffsetTopAndShrink(RectangleF rect, float offset)
        {
            return new RectangleF(rect.X, rect.Y, rect.Width, rect.Height - offset);
        }

        private RectangleF SideViewBounds 
        {
            get
            {
                if (this.NavigationControllerBehavior == ViewDeckNavigationControllerBehavior.Contained)
                    return this.ReferenceBounds;
            
                return II_RectangleFOffsetTopAndShrink(this.ReferenceBounds, this.RelativeStatusBarHeight);
            }
        }

        #endregion

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

        // todo: rename to dispose
        private void Dealloc()
        {
            this.CleanUp();
            
//            this.centerController.viewDeckController = null;
            this.CenterController = null;

            if (this.LeftController != null)
            {
//                this.leftController.viewDeckController = null;
                this.LeftController = null;
            }

            if (this.RightController != null)
            {
//                this.rightController.viewDeckController = null;
                this.RightController = null;
            }

            this.panners.Clear();
        }


        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            this.CenterController.DidReceiveMemoryWarning();

            if (this.LeftController != null)
            {
                this.LeftController.DidReceiveMemoryWarning();
            }

            if (this.RightController != null)
            {
                this.RightController.DidReceiveMemoryWarning();
            }
        }

        public IEnumerable<UIViewController> Controllers()
        {
            yield return this.CenterController;

            if (this.LeftController != null)
            {
                yield return this.LeftController;
            }

            if (this.RightController != null)
            {
                yield return this.RightController;
            }
        }

        private float LimitOffset(float offset) 
        {
            if (this.LeftController != null && this.RightController != null) 
                return offset;
            
            if (this.LeftController != null && this.MaxLedge > 0) 
            {
                float left = this.ReferenceBounds.Size.Width - this.MaxLedge;
                offset = Math.Max(offset, left);
            }
            else if (this.RightController != null && this.MaxLedge > 0) 
            {
                float right = this.MaxLedge - this.ReferenceBounds.Size.Width;
                offset = Math.Min(offset, right);
            }
            
            return offset;
        }

        private RectangleF SlidingRectForOffset(float offset) 
        {
            offset = this.LimitOffset(offset);

            var sz = this.SlidingSizeForOffset(offset);

            return new RectangleF(this.resizesCenterView && offset < 0 ? 0 : offset, 0, sz.Width, sz.Height);
        }

        private SizeF SlidingSizeForOffset(float offset) 
        {
            if (!this.resizesCenterView) 
                return this.ReferenceBounds.Size;
            
            offset = this.LimitOffset(offset);

            if (offset < 0) 
                return new SizeF(this.CenterViewBounds.Size.Width + offset, this.CenterViewBounds.Size.Height);
            
            return new SizeF(this.CenterViewBounds.Size.Width - offset, this.CenterViewBounds.Size.Height);
        }

        private void SetSlidingFrameForOffset(float offset) 
        {
            this.offset = this.LimitOffset(offset);
            this.SlidingControllerView.Frame = this.SlidingRectForOffset(offset);

//delegate            this.performOffsetDelegate(@selector(viewDeckController:slideOffsetChanged:), this.offset);
        }

        private void HideAppropriateSideViews() 
        {
            this.LeftController.View.Hidden = this.SlidingControllerView.Frame.GetMinX() <= 0;

            this.RightController.View.Hidden = this.SlidingControllerView.Frame.GetMaxX() >= this.ReferenceBounds.Size.Width;
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
                this.CenterController.View.RemoveFromSuperview();
                this.centerView.AddSubview(this.CenterController.View);
                this.LeftController.View.RemoveFromSuperview();
                this.referenceView.InsertSubviewBelow(this.LeftController.View, this.SlidingControllerView);

                this.RightController.View.RemoveFromSuperview();
                this.referenceView.InsertSubviewBelow(this.RightController.View, this.SlidingControllerView);
                
                this.ReapplySideController(this.LeftController);
                this.ReapplySideController(this.RightController);
                
                this.SetSlidingFrameForOffset(this.offset);
                this.SlidingControllerView.Hidden = false;
                
                this.centerView.Frame = this.CenterViewBounds;
                this.CenterController.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                this.CenterController.View.Frame = this.centerView.Bounds;
                
                this.LeftController.View.Frame = this.SideViewBounds;
                this.LeftController.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                
                this.RightController.View.Frame = this.SideViewBounds;
                this.RightController.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

                this.ApplyShadowToSlidingView();
            };

            if (this.SetSlidingAndReferenceViews()) 
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
            
            this.AddPanners();
            
            if (this.SlidingControllerView.Frame.Location.X == 0.0f) 
            {
                this.CenterViewVisible();
            }
            else
            {
                this.CenterViewHidden();
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
            
            this.RemovePanners();
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
            this.preRotationWidth = this.ReferenceBounds.Size.Width;
            this.preRotationCenterWidth = this.CenterViewBounds.Size.Width;//todo: was - this.centerView.Bounds.Size.Width;
            
            if (this.RotationBehavior == ViewDeckRotationBehavior.KeepsViewSizes) 
           {
                this.leftWidth = this.LeftController.View.Frame.Size.Width;
                this.rightWidth = this.RightController.View.Frame.Size.Width;
            }
            
            bool should = true;
            if (this.CenterController != null)
                {
                should = this.CenterController.ShouldAutorotateToInterfaceOrientation(toInterfaceOrientation);
          }

            return should;
        }

         public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);
            
//            this.relayAppearanceMethod:^(UIViewController *controller) {
//                [controller willAnimateRotationToInterfaceOrientation:toInterfaceOrientation duration:duration);
//            });
            
            this.ArrangeViewsAfterRotation();
        }

         public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillRotate(toInterfaceOrientation, duration);
            this.RestoreShadowToSlidingView();
            
//            this.relayAppearanceMethod:^(UIViewController *controller) {
//                [controller willRotateToInterfaceOrientation:toInterfaceOrientation duration:duration);
//            });
        }

         public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);
            this.ApplyShadowToSlidingView();
            
//            this.relayAppearanceMethod:^(UIViewController *controller) {
//                [controller didRotateFromInterfaceOrientation:fromInterfaceOrientation);
//            });
        }

        private void ArrangeViewsAfterRotation() 
        {
            if (this.preRotationWidth <= 0) return;
            
            float offset = this.SlidingControllerView.Frame.Location.X;

            if (this.resizesCenterView != null && offset == 0) 
            {
                offset = offset + (this.preRotationCenterWidth - this.preRotationWidth);
            }
            
            if (this.RotationBehavior == ViewDeckRotationBehavior.KeepsLedgeSizes) 
            {
                if (offset > 0) 
                {
                    offset = this.ReferenceBounds.Size.Width - this.preRotationWidth + offset;
                }
                else if (offset < 0) 
                {
                    offset = offset + this.preRotationWidth - this.ReferenceBounds.Size.Width;
                }
            }
            else 
            {
                this.LeftLedge = this.LeftLedge + this.ReferenceBounds.Size.Width - this.preRotationWidth; 
                this.RightLedge = this.RightLedge + this.ReferenceBounds.Size.Width - this.preRotationWidth; 
                this.MaxLedge = this.MaxLedge + this.ReferenceBounds.Size.Width - this.preRotationWidth; 
            }

            this.SetSlidingFrameForOffset(offset);
            
            this.preRotationWidth = 0;
        }


        private void ShowCenterView() 
        {
            this.ShowCenterView(true);
        }

        private void ShowCenterView(bool animated) 
        {
            this.ShowCenterView(animated, null);
        }

        private void ShowCenterView(bool animated, Action<ViewDeckController> completed)
        {
            bool mustRunCompletion = completed != null;

            if (this.LeftController != null&& !this.LeftController.View.Hidden) 
            {
                this.CloseLeftViewAnimated(animated, completed);
                mustRunCompletion = false;
            }
            
            if (this.RightController != null && !this.RightController.View.Hidden) 
            {
                this.CloseRightViewAnimated(animated, completed);
                mustRunCompletion = false;
            }
            
            if (mustRunCompletion)
                completed(this);
        }

        public bool ToggleLeftView() 
        {
            return this.ToggleLeftViewAnimated(true);
        }

        public bool OpenLeftView() 
        {
            return this.OpenLeftViewAnimated(true);
        }

        public bool CloseLeftView()
        {
            return this.CloseLeftViewAnimated(true);
        }

        public bool ToggleLeftViewAnimated(bool animated)
        {
            return this.ToggleLeftViewAnimated(animated, null);
        }

        public bool ToggleLeftViewAnimated(bool animated, Action<ViewDeckController> completed)
        {
            if (this.LeftControllerIsClosed) 
            {
                return this.OpenLeftViewAnimated(animated, completed);
            }
            else
            {
                return this.CloseLeftViewAnimated(animated, completed);
            }
        }

        public bool OpenLeftViewAnimated(bool animated) 
        {
            return this.OpenLeftViewAnimated(animated, null);
        }

        public bool OpenLeftViewAnimated(bool animated, Action<ViewDeckController> completed)
        {
            return this.OpenLeftViewAnimated(animated, UIViewAnimationOptions.CurveEaseInOut, true, completed);
        }

        private bool OpenLeftViewAnimated(bool animated, bool callDelegate, Action<ViewDeckController> completed)
        {
            return this.OpenLeftViewAnimated(animated, UIViewAnimationOptions.CurveEaseInOut, callDelegate, completed);
        }

        private bool OpenLeftViewAnimated(bool animated, UIViewAnimationOptions options, bool callDelegate, Action<ViewDeckController> completed)
        {
            if (this.LeftController == null || II_FLOAT_EQUAL(this.SlidingControllerView.Frame.GetMinX(), this.LeftLedge)) return true;


            // check the delegate to allow opening
//delegate            if (callDelegate && !this.checkDelegate:@selector(viewDeckControllerWillOpenLeftView:animated:) animated:animated]) return false;

            // also close the right view if it's open. Since the delegate can cancel the close, check the result.
//delegate            if (callDelegate && !this.closeRightViewAnimated(animated, options, callDelegate, completed]) return false;
            
            UIView.Animate(OPEN_SLIDE_DURATION(animated), 0, options | UIViewAnimationOptions.LayoutSubviews | UIViewAnimationOptions.BeginFromCurrentState, () =>
                           {
                this.LeftController.View.Hidden = false;
                this.SetSlidingFrameForOffset(this.ReferenceBounds.Size.Width - this.LeftLedge);
                this.CenterViewHidden();
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

        public bool OpenLeftViewBouncing(Action<ViewDeckController> bounced)
        {
            return this.OpenLeftViewBouncing(bounced, null);
        }

        public bool OpenLeftViewBouncing(Action<ViewDeckController> bounced, Action<ViewDeckController>completed) 
        {
            return this.OpenLeftViewBouncing(bounced, true, completed);
        }

        private bool OpenLeftViewBouncing(Action<ViewDeckController> bounced, bool callDelegate, Action<ViewDeckController> completed) 
        {
            return this.OpenLeftViewBouncing(bounced, UIViewAnimationOptions.CurveEaseInOut, true, completed);
        }

        private bool OpenLeftViewBouncing(Action<ViewDeckController> bounced, UIViewAnimationOptions options, bool callDelegate, Action<ViewDeckController> completed)
        {
            if (this.LeftController == null || II_FLOAT_EQUAL(this.SlidingControllerView.Frame.GetMinX(), this.LeftLedge)) return true;
            
            // check the delegate to allow opening
//            if (callDelegate && !this.checkDelegate:@selector(viewDeckControllerWillOpenLeftView:animated:) animated:YES]) return false;

            // also close the right view if it's open. Since the delegate can cancel the close, check the result.
//            if (callDelegate && !this.closeRightViewAnimated:YES options:options callDelegate:callDelegate completion:completed]) return false;
            
            // first open the view completely, run the block (to allow changes)
            UIView.Animate(OPEN_SLIDE_DURATION(true), 0, UIViewAnimationOptions.CurveEaseIn | UIViewAnimationOptions.LayoutSubviews, () =>
            {
                this.LeftController.View.Hidden = false;
                this.SetSlidingFrameForOffset(this.ReferenceBounds.Size.Width);
            }, () => {
                // run block if it's defined
                if (bounced != null) bounced(this);
                this.CenterViewHidden();
                
                // now slide the view back to the ledge position
                UIView.Animate(OPEN_SLIDE_DURATION(true), 0, options | UIViewAnimationOptions.LayoutSubviews | UIViewAnimationOptions.BeginFromCurrentState,
                               () => {
                    this.SetSlidingFrameForOffset(this.ReferenceBounds.Size.Width - this.LeftLedge);
                }, () => {
                    if (completed != null) completed(this);
//                    if (callDelegate) this.performDelegate:@selector(viewDeckControllerDidOpenLeftView:animated:) animated:YES);
                });
            });
            
            return true;
        }

        public bool CloseLeftViewAnimated(bool animated) 
        {
            return this.CloseLeftViewAnimated(animated, null);
        }

        public bool CloseLeftViewAnimated(bool animated, Action<ViewDeckController> completed)
        {
            return this.CloseLeftViewAnimated(animated,true, completed);
        }

        private bool CloseLeftViewAnimated(bool animated, bool callDelegate, Action<ViewDeckController> completed) 
        {
            return this.CloseLeftViewAnimated(animated, UIViewAnimationOptions.CurveEaseInOut, callDelegate, completed);
        }

        private bool CloseLeftViewAnimated(bool animated, UIViewAnimationOptions options, bool callDelegate, Action<ViewDeckController> completed) 
        {
            if (this.LeftControllerIsClosed) return true;
            
            // check the delegate to allow closing
//            if (callDelegate && !this.checkDelegate:@selector(viewDeckControllerWillCloseLeftView:animated:) animated:animated]) return NO;
            
            UIView.Animate(CLOSE_SLIDE_DURATION(animated), 0, options | UIViewAnimationOptions.LayoutSubviews, () => {
                this.SetSlidingFrameForOffset(0);
                this.CenterViewVisible();
            }, () =>  {
                this.HideAppropriateSideViews();
                if (completed != null) completed(this);
                if (callDelegate) 
                {
//                    this.performDelegate:@selector(viewDeckControllerDidCloseLeftView:animated:) animated:animated);
//                    this.performDelegate:@selector(viewDeckControllerDidShowCenterView:animated:) animated:animated);
                }
            });
            
            return true;
        }

        public bool CloseLeftViewBouncing(Action<ViewDeckController> bounced) 
        {
            return this.CloseLeftViewBouncing(bounced, null);
        }

        public bool CloseLeftViewBouncing(Action<ViewDeckController> bounced, Action<ViewDeckController> completed) 
        {
            return this.CloseLeftViewBouncing(bounced, true, completed);
        }

        private bool CloseLeftViewBouncing(Action<ViewDeckController> bounced, bool callDelegate, Action<ViewDeckController> completed) 
        {
            if (this.LeftControllerIsClosed) return true;
            
            // check the delegate to allow closing
//            if (callDelegate && !this.checkDelegate:@selector(viewDeckControllerWillCloseLeftView:animated:) animated:YES]) return NO;
            
            // first open the view completely, run the block (to allow changes) and close it again.
            UIView.Animate(OPEN_SLIDE_DURATION(true), 0, UIViewAnimationOptions.CurveEaseIn | UIViewAnimationOptions.LayoutSubviews,
                           () => 
                           {
                this.SetSlidingFrameForOffset(this.ReferenceBounds.Size.Width);
            }, () => 
            {
                // run block if it's defined
                if (bounced != null) bounced(this);

//                if (callDelegate && this.delegate && [this.delegate respondsToSelector:@selector(viewDeckController:didBounceWithClosingController:)]) 
//                    [this.delegate viewDeckController:self didBounceWithClosingController:this.leftController);
                
                UIView.Animate(CLOSE_SLIDE_DURATION(true), 0, UIViewAnimationOptions.CurveEaseOut | UIViewAnimationOptions.LayoutSubviews, () => {
                    this.SetSlidingFrameForOffset(0);
                    this.CenterViewVisible();
                } , () => {
                    this.HideAppropriateSideViews();
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


        public bool ToggleRightView() 
        {
            return this.ToggleRightViewAnimated(true);
        }

        public bool OpenRightView() 
        {
            return this.OpenRightViewAnimated(true);
        }

        public bool CloseRightView() 
        {
            return this.CloseRightViewAnimated(true);
        }

        public bool ToggleRightViewAnimated(bool animated)
        {
            return this.ToggleRightViewAnimated(animated, null);
        }

        public bool ToggleRightViewAnimated(bool animated, Action<ViewDeckController> completed) 
        {
            if (this.RightControllerIsClosed) 
                {
                return this.OpenRightViewAnimated(animated, completed);
                }
                else
                {
                    return this.CloseRightViewAnimated(animated, completed);
                }
        }

        public bool OpenRightViewAnimated(bool animated)
        {
            return this.OpenRightViewAnimated(animated, null);
        }

        public bool OpenRightViewAnimated(bool animated, Action<ViewDeckController> completed) 
        {
            return this.OpenRightViewAnimated(animated, UIViewAnimationOptions.CurveEaseInOut,true, completed);
        }

        private bool OpenRightViewAnimated(bool animated, bool callDelegate, Action<ViewDeckController> completed)
        {
            return this.OpenRightViewAnimated(animated, UIViewAnimationOptions.CurveEaseInOut, callDelegate, completed);
        }

        private bool OpenRightViewAnimated(bool animated, UIViewAnimationOptions options, bool callDelegate, Action<ViewDeckController> completed)
        {
            if (this.RightController == null || II_FLOAT_EQUAL(this.SlidingControllerView.Frame.GetMaxX(), this.RightLedge)) return true;
            
            // check the delegate to allow opening
//            if (callDelegate && !this.checkDelegate:@selector(viewDeckControllerWillOpenRightView:animated:) animated:animated]) return NO;

            // also close the left view if it's open. Since the delegate can cancel the close, check the result.
//            if (callDelegate && !this.closeLeftViewAnimated:animated options:options callDelegate:callDelegate completion:completed]) return NO;
            
            UIView.Animate(OPEN_SLIDE_DURATION(animated), 0, options | UIViewAnimationOptions.LayoutSubviews, () => {
                this.RightController.View.Hidden = false;
                this.SetSlidingFrameForOffset(this.RightLedge - this.ReferenceBounds.Size.Width);
                this.CenterViewHidden();
            }, () => {
                if (completed != null) completed(this);
//                if (callDelegate) this.performDelegate:@selector(viewDeckControllerDidOpenRightView:animated:) animated:animated);
            });

            return true;
        }

        public bool OpenRightViewBouncing(Action<ViewDeckController> bounced) 
        {
            return this.OpenRightViewBouncing(bounced, null);
        }

        public bool OpenRightViewBouncing(Action<ViewDeckController> bounced, Action<ViewDeckController> completed) 
        {
            return this.OpenRightViewBouncing(bounced, true, completed);
        }

        private bool OpenRightViewBouncing(Action<ViewDeckController> bounced, bool callDelegate, Action<ViewDeckController> completed)
        {
            return this.OpenRightViewBouncing(bounced, UIViewAnimationOptions.CurveEaseInOut, true, completed);
        }

        private bool OpenRightViewBouncing(Action<ViewDeckController> bounced, UIViewAnimationOptions options, bool callDelegate, Action<ViewDeckController> completed)
        {
            if (this.RightController == null || II_FLOAT_EQUAL(this.SlidingControllerView.Frame.GetMinX(), this.RightLedge)) return true;
            
            // check the delegate to allow opening
//            if (callDelegate && !this.checkDelegate:@selector(viewDeckControllerWillOpenRightView:animated:) animated:YES]) return NO;

            // also close the right view if it's open. Since the delegate can cancel the close, check the result.
//            if (callDelegate && !this.closeLeftViewAnimated:YES options:options callDelegate:callDelegate completion:completed]) return NO;
            
            // first open the view completely, run the block (to allow changes)
            UIView.Animate(OPEN_SLIDE_DURATION(true), 0, UIViewAnimationOptions.CurveEaseIn | UIViewAnimationOptions.LayoutSubviews, () => {
                this.RightController.View.Hidden = false;
                this.SetSlidingFrameForOffset(-this.ReferenceBounds.Size.Width);
            }, () =>  {
                // run block if it's defined
                if (bounced != null) bounced(this);
                this.CenterViewHidden();
                
                // now slide the view back to the ledge position
                UIView.Animate(OPEN_SLIDE_DURATION(true), 0, options | UIViewAnimationOptions.LayoutSubviews | UIViewAnimationOptions.BeginFromCurrentState, () => {
                    this.SetSlidingFrameForOffset(this.RightLedge - this.ReferenceBounds.Size.Width);
                }, () => {
                    if (completed != null) completed(this);
//                    if (callDelegate) this.performDelegate:@selector(viewDeckControllerDidOpenRightView:animated:) animated:YES);
                });
            });
            
            return true;
        }

        public bool CloseRightViewAnimated(bool animated)
        {
            return this.CloseRightViewAnimated(animated, null);
        }

        public bool CloseRightViewAnimated(bool animated, Action<ViewDeckController> completed)
        {
            return this.CloseRightViewAnimated(animated, UIViewAnimationOptions.CurveEaseInOut, true, completed);
        }

        private bool CloseRightViewAnimated(bool animated, bool callDelegate, Action<ViewDeckController> completed)
        {
            return this.CloseRightViewAnimated(animated, UIViewAnimationOptions.CurveEaseInOut, callDelegate, completed);
        }

        private bool CloseRightViewAnimated(bool animated, UIViewAnimationOptions options, bool callDelegate, Action<ViewDeckController> completed) 
        {
            if (this.RightControllerIsClosed) return true;
            
            // check the delegate to allow closing
//            if (callDelegate && !this.checkDelegate:@selector(viewDeckControllerWillCloseRightView:animated:) animated:animated]) return NO;
            
            UIView.Animate(CLOSE_SLIDE_DURATION(animated), 0, options | UIViewAnimationOptions.LayoutSubviews, () => {
                this.SetSlidingFrameForOffset(0);
                this.CenterViewVisible();
            }, () => {
                if (completed != null) completed(this);
                this.HideAppropriateSideViews();
                if (callDelegate) {
//                    this.performDelegate:@selector(viewDeckControllerDidCloseRightView:animated:) animated:animated);
//                    this.performDelegate:@selector(viewDeckControllerDidShowCenterView:animated:) animated:animated);
                }
            });
            
            return true;
        }

        public bool CloseRightViewBouncing(Action<ViewDeckController> bounced) 
        {
            return this.CloseRightViewBouncing(bounced, null);
        }

        public bool CloseRightViewBouncing(Action<ViewDeckController> bounced, Action<ViewDeckController> completed) 
        {
            return this.CloseRightViewBouncing(bounced, true, completed);
        }

        private bool CloseRightViewBouncing(Action<ViewDeckController> bounced, bool callDelegate, Action<ViewDeckController> completed) 
        {
            if (this.RightControllerIsClosed) return true;
            
            // check the delegate to allow closing
//            if (callDelegate && !this.checkDelegate:@selector(viewDeckControllerWillCloseRightView:animated:) animated:YES]) return NO;
            
            UIView.Animate(OPEN_SLIDE_DURATION(true), 0,  UIViewAnimationOptions.CurveEaseIn | UIViewAnimationOptions.LayoutSubviews, () => {
                this.SetSlidingFrameForOffset(-this.ReferenceBounds.Size.Width);
            }, () =>  {
                if (bounced != null) bounced(this);
//                if (callDelegate && this.delegate && [this.delegate respondsToSelector:@selector(viewDeckController:didBounceWithClosingController:)]) 
//                    [this.delegate viewDeckController:self didBounceWithClosingController:this.rightController);
                
                UIView.Animate(CLOSE_SLIDE_DURATION(true), 0, UIViewAnimationOptions.CurveEaseOut | UIViewAnimationOptions.LayoutSubviews, () => {
                    this.SetSlidingFrameForOffset(0);
                    this.CenterViewVisible();
                }, () =>  {
                    this.HideAppropriateSideViews();
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

        public void RightViewPushViewControllerOverCenterController(UIViewController controller) 
        {
            Debug.Assert(this.CenterController.GetType().IsSubclassOf(typeof(UINavigationController)), "cannot rightViewPushViewControllerOverCenterView when center controller is not a navigation controller");

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
            
            this.CloseRightViewAnimated(true);

            UINavigationController navController = ((UINavigationController)this.CenterController);
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

        private void RelayAppearanceMethod(Action<UIViewController> relay, bool forced) 
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

        private void RelayAppearanceMethod(Action<UIViewController> relay)
        {
//            [self relayAppearanceMethod:relay forced:NO];
        }

        //#pragma mark - center view hidden stuff

        private void CenterViewVisible()
        {
            this.RemovePanners();
            if (this.centerTapper != null) 
            {
// todo:                this.centerTapper.RemoveTarget(this, @selector(centerTapped), UIControlEventTouchUpInside);
                this.centerTapper.RemoveFromSuperview();
            }

            this.centerTapper = null;
            this.AddPanners();
        }

        private void CenterViewHidden() 
        {
            if (this.centerHiddenInteractivity == CenterHiddenInteractivity.UserInteractive) 
                return;
            
            this.RemovePanners();

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
            this.AddPanners();
        }

        [Export("centerTapped")]
        private void CenterTapped() 
        {
            // todo: handle additinal cases better
            if (this.centerHiddenInteractivity != CenterHiddenInteractivity.UserInteractive) 
            {
                if (this.LeftController != null && this.SlidingControllerView.Frame.GetMinX() > 0) 
                {
                    if (this.centerHiddenInteractivity == CenterHiddenInteractivity.NotUserInteractiveWithTapToClose) 
                    {
                        this.CloseLeftView();
                    }
                    else
                    {
                        this.CloseLeftViewBouncing(null);
                    }
                }

                if (this.RightController != null && this.SlidingControllerView.Frame.GetMinX() < 0) 
                {
                    if (this.centerHiddenInteractivity == CenterHiddenInteractivity.NotUserInteractiveWithTapToClose) 
                    {
                        this.CloseRightView();
                    }
                    else
                    {
                        this.CloseRightViewBouncing(null);
                    }
                }
                
            }
        }

        //#pragma mark - Panning

        [Export("gestureRecognizerShouldBegin:")]
        private bool GestureRecognizerShouldBegin(UIGestureRecognizer gestureRecognizer)
        {
            float px = this.SlidingControllerView.Frame.Location.X;
            if (px != 0) return true;
                
            float x = this.LocationOfPanner((UIPanGestureRecognizer)gestureRecognizer);
            bool ok =  true;

            if (x > 0) 
            {
// todo                ok = [self checkDelegate:@selector(viewDeckControllerWillOpenLeftView:animated:) animated:NO];
                if (!ok)
                    this.CloseLeftViewAnimated(false);
            }
            else if (x < 0) 
            {
// todo                ok = [self checkDelegate:@selector(viewDeckControllerWillOpenRightView:animated:) animated:NO];
                if (!ok)
                    this.CloseRightViewAnimated(false);
            }
            
            return ok;
        }

        [Export("gestureRecognizer:shouldReceiveTouch:")]
        private bool GestureRecognizer(UIGestureRecognizer gestureRecognizer, UITouch touch) 
        {
            this.panOrigin = this.SlidingControllerView.Frame.Location.X;
            return true;
        }

        private float LocationOfPanner(UIPanGestureRecognizer panner) 
        {
            PointF pan = panner.TranslationInView(this.referenceView);
            float x = pan.X + this.panOrigin;
            
            if (this.LeftController == null) x = Math.Min(0, x);

            if (this.RightController == null) x = Math.Max(0, x);
            
            float w = this.ReferenceBounds.Size.Width;
            float lx = Math.Max(Math.Min(x, w - this.LeftLedge), -w + this.RightLedge);
            
            if (this.Elastic) 
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
            
            return this.LimitOffset(x);
        }

        [Export("panned:")]
        private void Panned(UIPanGestureRecognizer panner) 
        {
            if (!this.Enabled) return;

            float px = this.SlidingControllerView.Frame.Location.X;
            float x = this.LocationOfPanner(panner);
            float w = this.ReferenceBounds.Size.Width;

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
                        this.CloseRightViewAnimated(false);
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
                        this.CloseLeftViewAnimated(false);
                        return;
                    }
                }
            }
            
            this.SetSlidingFrameForOffset(x);
            
            bool rightWasHidden = this.RightController.View.Hidden;
            bool leftWasHidden = this.LeftController.View.Hidden;
            
            // todo: [self performOffsetDelegate:@selector(viewDeckController:didPanToOffset:) offset:x];
            
            if (panner.State == UIGestureRecognizerState.Ended) 
            {
                if (this.SlidingControllerView.Frame.Location.X == 0.0f) 
                {
                    this.CenterViewVisible();
                }
                else
                {
                    this.CenterViewHidden();
                }

                float lw3 = (w - this.LeftLedge) / 3.0f;
                float rw3 = (w - this.RightLedge) / 3.0f;
                float velocity = panner.VelocityInView(this.referenceView).X;

                if (Math.Abs(velocity) < 500) 
                {
                    // small velocity, no movement
                    if (x >= w - this.LeftLedge - lw3) 
                    {
                        this.OpenLeftViewAnimated(true, UIViewAnimationOptions.CurveEaseOut, false, null);
                    }
                    else if (x <= this.RightLedge + rw3 - w) 
                    {
                        this.OpenRightViewAnimated(true, UIViewAnimationOptions.CurveEaseOut, false, null);
                    }
                    else
                    {
                        this.ShowCenterView(true);
                    }
                }
                else if (velocity < 0) 
                {
                    // swipe to the left
                    if (x < 0) 
                    {
                        this.OpenRightViewAnimated(true, UIViewAnimationOptions.CurveEaseOut, true, null);
                    }
                    else 
                    {
                        this.ShowCenterView(true);
                    }
                }
                else if (velocity > 0) 
                {
                    // swipe to the right
                    if (x > 0) 
                    {
                        this.OpenLeftViewAnimated(true, UIViewAnimationOptions.CurveEaseOut, true, null);
                    }
                    else 
                    {
                        this.ShowCenterView(true);
                    }
                }
            }
            else
            {
                this.HideAppropriateSideViews();
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


        private void AddPanner(UIView view) 
        {
            if (view == null) return;

            UIPanGestureRecognizer panner = new UIPanGestureRecognizer(this, new Selector("panned:"));

            panner.CancelsTouchesInView = true;
            panner.WeakDelegate = this;

            this.View.AddGestureRecognizer(panner);
            this.panners.Add(panner);
        }


        private void AddPanners() 
        {
            this.RemovePanners();
            
            switch (this.PanningMode) 
            {
                case ViewDeckPanningMode.NoPanning: 
                    break;
                    
                case ViewDeckPanningMode.FullViewPanning:
                    this.AddPanner(this.SlidingControllerView);

                    // also add to disabled center
                    if (this.centerTapper != null)
                        this.AddPanner(this.centerTapper);

                    // also add to navigationbar if present
                    if (this.NavigationController != null && !this.NavigationController.NavigationBarHidden) 
                        this.AddPanner(this.NavigationController.NavigationBar);

                    break;
                    
                case ViewDeckPanningMode.NavigationBarPanning:
                    if (this.NavigationController != null && !this.NavigationController.NavigationBarHidden) 
                    {
                        this.AddPanner(this.NavigationController.NavigationBar);
                    }
                    
                    if (this.CenterController.NavigationController != null && !this.CenterController.NavigationController.NavigationBarHidden) 
                    {
                        this.AddPanner(this.CenterController.NavigationController.NavigationBar);
                    }
                    
                    if (this.CenterController.GetType().IsSubclassOf(typeof(UINavigationController)) && !((UINavigationController)this.CenterController).NavigationBarHidden) 
                    {
                        this.AddPanner(((UINavigationController)this.CenterController).NavigationBar);
                    }

                    break;
                case ViewDeckPanningMode.PanningViewPanning:
                    if (this.PanningView != null) 
                    {
                        this.AddPanner(this.PanningView);
                    }

                    break;
            }
        }


        private void RemovePanners() 
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


        private void ApplySideController(ref UIViewController controllerStore, UIViewController newController, UIViewController otherController, 
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
                    controller.View.Hidden = left ? this.SlidingControllerView.Frame.Location.X <= 0 : this.SlidingControllerView.Frame.Location.X >= 0;
                    controller.View.Frame = this.ReferenceBounds;
                    controller.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                    if (this.slidingController != null)
                    {
                        this.referenceView.InsertSubviewBelow(controller.View, this.SlidingControllerView);
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
                if (newController == this.CenterController) 
                {
                    this.CenterController = null;
                }

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

                afterBlock(controllerStore, controllerStore == this.LeftController);

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

        private void ReapplySideController(UIViewController controllerStore) 
        {
            this.ApplySideController(ref controllerStore, controllerStore, null, null);
        }


        #region Property Setters

        /// <summary>
        /// Set the center controller
        /// </summary>
        private void SetCenterController(UIViewController centerController) 
        {
            if (this.CenterController == centerController) 
            {
                return;
            }

            Action<UIViewController> beforeBlock = (x) => {};
            Action<UIViewController> afterBlock = (x) => {};

            var currentFrame = this.ReferenceBounds;

            if (this.viewAppeared) 
            {
                beforeBlock = (controller) => 
                {
// todo:                    controller.vdc_viewWillDisappear(false);
                    this.RestoreShadowToSlidingView();
                    this.RemovePanners();
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
                    
                    this.SetSlidingAndReferenceViews();
                    controller.View.Frame = currentFrame;
                    controller.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                    controller.View.Hidden = false;
                    this.centerView.AddSubview(controller.View);
                    
                    if (barHidden) 
                        navController.NavigationBarHidden = false;
                    
                    this.AddPanners();
                    this.ApplyShadowToSlidingView();
// todo:                    controller.vdc_viewDidAppear(false);
                };
            }
            
            // start the transition
            if (this.CenterController != null) 
            {
                currentFrame = this.CenterController.View.Frame;
                this.CenterController.WillMoveToParentViewController(null);

                if (centerController == this.LeftController) this.LeftController = null;
                if (centerController == this.RightController) this.RightController = null;


                beforeBlock(this.CenterController);

                try 
                {
                    this.CenterController.RemoveObserver(this, new NSString("title"));
                    if (this.AutomaticallyUpdateTabBarItems) 
                    {
                        this.CenterController.RemoveObserver(this, new NSString("tabBarItem.title"));
                        this.CenterController.RemoveObserver(this, new NSString("tabBarItem.image"));
                        this.CenterController.RemoveObserver(this, new NSString("hidesBottomBarWhenPushed"));
                    }
                }
                catch (Exception ex) 
                {
                    // gobble
                }

// todo:                this.centerController.setViewDeckController(null);
                this.CenterController.RemoveFromParentViewController();

                
                this.CenterController.DidMoveToParentViewController(null);
                // todo: dispose ? II_RELEASE(_centerController);
            }
            
            // make the switch
            this._centerController = centerController;
            
            if (this.CenterController != null) 
            {
                // and finish the transition
                //II_RETAIN(_centerController);
                this.AddChildViewController(this.CenterController);

// todo:                this.centerController.setViewDeckController(this);
                this.CenterController.AddObserver(this, new NSString("title"), 0, IntPtr.Zero);

                this.Title = this.CenterController.Title;

                if (this.AutomaticallyUpdateTabBarItems) 
                {
                    this.CenterController.AddObserver(this, new NSString("tabBarItem.title"), 0, IntPtr.Zero);
                    this.CenterController.AddObserver(this, new NSString("tabBarItem.image"), 0, IntPtr.Zero);
                    this.CenterController.AddObserver(this, new NSString("hidesBottomBarWhenPushed"), 0, IntPtr.Zero);
                    
                    this.TabBarItem.Title = this.CenterController.TabBarItem.Title;
                    this.TabBarItem.Image = this.CenterController.TabBarItem.Image;
                    this.HidesBottomBarWhenPushed = this.CenterController.HidesBottomBarWhenPushed;
                }
                
                afterBlock(this.CenterController);

                this.CenterController.DidMoveToParentViewController(this);
            }    
        }

        /// <summary>
        /// </summary>
        private void SetRightLedge(float rightLedge) 
        {
            // Compute the final ledge in two steps. This prevents a strange bug where
            // nesting MAX(X, MIN(Y, Z)) with miniscule referenceBounds returns a bogus near-zero value.

            float minLedge = Math.Min(this.ReferenceBounds.Size.Width, rightLedge);
            rightLedge = Math.Max(rightLedge, minLedge);

            if (this.viewAppeared && II_FLOAT_EQUAL(this.SlidingControllerView.Frame.Location.X, this.RightLedge - this.ReferenceBounds.Size.Width)) 
            {
                if (rightLedge < this.RightLedge) 
                {
                    UIView.Animate(CLOSE_SLIDE_DURATION(true), () =>
                    {
                        this.SetSlidingFrameForOffset(rightLedge - this.ReferenceBounds.Size.Width);
                    });
                }
                else if (rightLedge > this.RightLedge) 
                {
                    UIView.Animate(OPEN_SLIDE_DURATION(true),() =>
                    {
                        this.SetSlidingFrameForOffset(rightLedge - this.ReferenceBounds.Size.Width);
                    });
                }
            }

            this._rightLedge = rightLedge;
        }

        /// <summary>
        /// </summary>
        private void SetRightLedge(float rightLedge, Action<bool> completion)
        {
            // Compute the final ledge in two steps. This prevents a strange bug where
            // nesting MAX(X, MIN(Y, Z)) with miniscule referenceBounds returns a bogus near-zero value.

            float minLedge = Math.Min(this.ReferenceBounds.Size.Width, rightLedge);
            rightLedge = Math.Max(rightLedge, minLedge);

            if (this.viewAppeared && II_FLOAT_EQUAL(this.SlidingControllerView.Frame.Location.X, this.RightLedge - this.ReferenceBounds.Size.Width)) 
            {
                if (rightLedge < this.RightLedge) 
                {
                    UIView.Animate(CLOSE_SLIDE_DURATION(true), () =>
                    {
                        this.SetSlidingFrameForOffset(rightLedge - this.ReferenceBounds.Size.Width);
                    }, () => completion(true));
                }
                else if (rightLedge > this.RightLedge) 
                {
                    UIView.Animate(OPEN_SLIDE_DURATION(true),() =>
                    {
                        this.SetSlidingFrameForOffset(rightLedge - this.ReferenceBounds.Size.Width);
                    }, () => completion(true));
                }
            }

            this._rightLedge = rightLedge;
        }

        /// <summary>
        /// </summary>
        private void SetLeftLedge(float leftLedge) 
        {
            // Compute the final ledge in two steps. This prevents a strange bug where
            // nesting MAX(X, MIN(Y, Z)) with miniscule referenceBounds returns a bogus near-zero value.

            float minLedge = Math.Min(this.ReferenceBounds.Size.Width, leftLedge);
            leftLedge = Math.Max(leftLedge, minLedge);

            if (this.viewAppeared && II_FLOAT_EQUAL(this.SlidingControllerView.Frame.Location.X, this.ReferenceBounds.Size.Width - this.LeftLedge)) 
            {
                if (leftLedge < this.LeftLedge) 
                {
                    UIView.Animate(CLOSE_SLIDE_DURATION(true), () =>
                    {
                        this.SetSlidingFrameForOffset(this.ReferenceBounds.Size.Width - leftLedge);
                    });
                }
                else if (leftLedge > this.LeftLedge) 
                {
                    UIView.Animate(OPEN_SLIDE_DURATION(true),() =>
                   {
                        this.SetSlidingFrameForOffset(this.ReferenceBounds.Size.Width - leftLedge);
                    });
                }
            }

            this._leftLedge = leftLedge;
        }

        /// <summary>
        /// </summary>
        private void SetLeftLedge(float leftLedge, Action<bool> completion)
        {
            // Compute the final ledge in two steps. This prevents a strange bug where
            // nesting MAX(X, MIN(Y, Z)) with miniscule referenceBounds returns a bogus near-zero value.

            float minLedge = Math.Min(this.ReferenceBounds.Size.Width, leftLedge);
            leftLedge = Math.Max(leftLedge, minLedge);

            if (this.viewAppeared && II_FLOAT_EQUAL(this.SlidingControllerView.Frame.Location.X, this.ReferenceBounds.Size.Width - this.LeftLedge)) 
            {
                if (leftLedge < this.LeftLedge) 
                {
                    UIView.Animate(CLOSE_SLIDE_DURATION(true), () =>
                    {
                        this.SetSlidingFrameForOffset(this.ReferenceBounds.Size.Width - leftLedge);
                    }, () => completion(true));
                }
                else if (leftLedge > this.LeftLedge) {
                    UIView.Animate(OPEN_SLIDE_DURATION(true),() =>
                    {
                        this.SetSlidingFrameForOffset(this.ReferenceBounds.Size.Width - leftLedge);
                    }, () => completion(true));
                }
            }

            this._leftLedge = leftLedge;
        }

        private string __title;
        public override string Title
        {
            get
            {
                return this.CenterController.Title;
            }

            set
            {
                if (this.__title != value)
                {
                    this.__title = value;
                    base.Title = value;
                    this.CenterController.Title = value;
                }
            }
        }


        #endregion

        private bool SetSlidingAndReferenceViews() 
        {
            if (this.NavigationController != null && this.NavigationControllerBehavior == ViewDeckNavigationControllerBehavior.Integrated) 
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
                this.slidingController = this.CenterController;
                this.referenceView = this.View;
                return true;
            }
            
            return false;
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

        private void RestoreShadowToSlidingView() 
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

        private void ApplyShadowToSlidingView() 
        {
            UIView shadowedView = this.SlidingControllerView;
            if (shadowedView == null) return;
            
            this.originalShadowRadius = shadowedView.Layer.ShadowRadius;
            this.originalShadowOpacity = shadowedView.Layer.ShadowOpacity;
            this.originalShadowColor = shadowedView.Layer.ShadowColor != null ? UIColor.FromCGColor(this.SlidingControllerView.Layer.ShadowColor) : null;
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

