//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="SectionItemsCollectionChangedEventArgs.cs" company="sgmunn">
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

namespace MonoKit.Controls
{
    using System;
    using System.Collections.Specialized;
    using MonoKit.ViewModels;

    /// <summary>
    /// A wrapper for an NotifyCollectionChangedEventArgs event that passes in the proxy of the sender.
    /// This maps our Source.Root[] to the sender of the event so that tables and collections can update the 
    /// correct section when items are added or removed from the view model
    /// </summary> 
    public class SectionItemsCollectionChangedEventArgs : NotifyCollectionChangedEventArgs
    {
        public SectionItemsCollectionChangedEventArgs(ISection section, NotifyCollectionChangedEventArgs args) 
            : base(NotifyCollectionChangedAction.Reset)
        {
            this.Section = section;
            this.InnerArgs = args;
        }

        public ISection Section { get; private set; }

        public NotifyCollectionChangedEventArgs InnerArgs { get; private set; }
    }
}
