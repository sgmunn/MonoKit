namespace MonoKit
{
    using System;
    using System.Linq;
    using MonoTouch.UIKit;
    using MonoTouch.Foundation;
    using MonoTouch.ObjCRuntime;

    /// <summary>
    /// Defines extension methods on UIViewControllers
    /// </summary>
    public static class UIViewControllerExtensions
    {
        /// <summary>
        /// Loads a view from a nib file in the main bundle
        /// </summary>
        /// <returns>
        /// The loaded view
        /// </returns>
        /// <param name='controller'>
        /// The view controller to be the owner of the view
        /// </param>
        /// <typeparam name='T'>
        /// The type of the view to load
        /// </typeparam>
        public static T LoadViewFromNib<T>(this UIViewController controller) where T: UIView
        {
            var viewType = typeof(T);
            var registrations = viewType.GetCustomAttributes(typeof(RegisterAttribute), false);
            
            if (registrations.Count() == 0)
            {
                throw new InvalidOperationException("You cannot load a view from a nib that has not been registered");
            }
            
            var registration = (RegisterAttribute)registrations[0];
            
            var handle = NSBundle.MainBundle.LoadNib(registration.Name, controller, null).ValueAt(0);
                 
            return Runtime.GetNSObject(handle) as T;
        }
    }
}

