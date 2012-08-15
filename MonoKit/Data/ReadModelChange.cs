//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ReadModelChange.cs" company="sgmunn">
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

namespace MonoKit.Data
{
    using System;
 
    public class ReadModelChange : IReadModelChange
    {
        public ReadModelChange(IIdentifiableObject item)
        {
            this.Item = item;
            this.Identity = item.Identity;
            this.Deleted = false;
        }

        public ReadModelChange(IIdentity id)
        {
            this.Identity = id;
            this.Deleted = true;
        }

        public ReadModelChange(IIdentifiableObject item, bool deleted)
        {
            this.Item = item;
            this.Identity = item.Identity;
            this.Deleted = deleted;
        }

        public ReadModelChange(IIdentity id, bool deleted)
        {
            this.Identity = id;
            this.Deleted = deleted;
        }

        public IIdentity Identity { get; private set; }

        public IIdentifiableObject Item { get; private set; }

        public bool Deleted { get; private set; }
    }
}
