// --------------------------------------------------------------------------------------------------------------------
// <copyright file=".cs" company="sgmunn">
//   (c) sgmunn 2012  
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//   the Software.
// 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//   THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//   IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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

