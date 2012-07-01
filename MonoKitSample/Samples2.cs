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
using MonoTouch.UIKit;
using MonoKit.UI;
using MonoKit.UI.Elements;

namespace MonoKitSample
{
    public class Samples2 : Samples
    {

        public Samples2(UINavigationController rootController) : base(rootController)
        {
        }
        
        public void SetupContent1(TableViewSource source)
        {
            var section1 = new TableViewSection(source);

            section1.Add(new DisclosureElement("Element Samples") { Command = this.GotoElementSamples });
            section1.Add(new DisclosureElement("Binding Samples") { Command = this.GotoBindingSamples });
            section1.Add(new DisclosureElement("Large Data Sample") { Command = this.GotoLargeDataSample });
            section1.Add(new DisclosureElement("Bind to Single Object") { Command = this.GotoBindToSingleObject });
        }
                
        public void SetupContent2(TableViewSource source)
        {
            var section2 = new TableViewSection(source);

            section2.Add(new DisclosureElement("Tasks Test") { Command = this.DoTaskTest });
            section2.Add(new DisclosureElement("Minion Test") { Command = this.DoMinionTest });
            section2.Add(new DisclosureElement("Event Sourced Domain Test") { Command = this.DoDomainTest1 });
            section2.Add(new DisclosureElement("Snapshot Domain Test") { Command = this.DoDomainTest2 });
            section2.Add(new DisclosureElement("SQLite Admin Test") { Command = this.DoSqliteTest });
            section2.Add(new DisclosureElement("Custom Control") { Command = this.GotoCustomControl });
            section2.Add(new DisclosureElement("GC Tests") { Command = this.GotoGCTests });
            
            var section3 = new TableViewSection(source);
            
            section3.Header = "Utils";
            
            section3.Add(new StringElement("GC.Collect") { Command = this.DoGCCollect });
        }
                
        public void SetupContent3(TableViewSource source)
        {
            var section3 = new TableViewSection(source);

            section3.Add(new StringElement("GC.Collect") { Command = this.DoGCCollect });
        }

    }
}

