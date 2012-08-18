//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="Commands.cs" company="sgmunn">
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

namespace MonoKitSample.Domain
{
    using System;
    using MonoKit.Data;
    using MonoKit.Domain;
    
    public class CommandBase : ICommand
    {
        public CommandBase()
        {
            this.CommandId = Guid.NewGuid();
        }

        public IUniqueIdentity AggregateId { get; set; }
        
        public Guid CommandId { get; set; }
    }

    public class TestCommand1 : CommandBase
    {
        public string Name { get; set; }
    }

    public class TestCommand2 : CommandBase
    {
        public string Description { get; set; }
 
        public decimal Amount { get; set; }
   }
}

