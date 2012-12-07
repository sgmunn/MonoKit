//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="TableViewSamples.cs" company="sgmunn">
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

using MonoKit;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using MonoKit.ViewModels.Elements;
using MonoKit.Runtime;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Sample.TableViews
{
    using System;
    using MonoKit.Controls;
    using MonoKit.ViewModels;
    using MonoTouch.UIKit;


    /*
     * things to demo
     * adding an item to a section
     * removing an item
     * adding a section
     * removing a section
     * 
     * binding to a root view model
     * binding to a section view model
     * binding to an enumerable
     * binding to a custom view model
     * 
     * sampler of elements, string, integer, date etc
     * sampler of editing elements as well
     * 
     * binding collection views to the same view models as the table view models
     * /
     */




    public class TableViewSamples : RootViewModel
    {
        private readonly INavigationService navigation;

        public TableViewSamples(INavigationService navigation)
        {
            this.navigation = navigation;
            this.Load();
        }

        private void Load()
        {
            this.Title = "Samples";

            this.Sections.Add(new SectionViewModel() {
                Header = "Different View Models",
                Items = {
                    new NavigatingViewModel(this.navigation, "Root", () => new TestRootViewModel()),
                    new NavigatingViewModel(this.navigation, "Section", () => new TestSectionViewModel()),
                    new NavigatingViewModel(this.navigation, "Enumerable", () => new TestEnumerableViewModel()),
                    new NavigatingViewModel(this.navigation, "Custom", () => new TestCustomViewModel()),
                }
            });

            this.Sections.Add(new SectionViewModel() {
                Header = "Sampler",
                Items = {
                    new NavigationRootViewModel(this.navigation)
                    {
                        Title = "Read-Only",
                        Sections = {
                            new SectionViewModel()
                            {
                                Items = {
                                    new LabelledStringInputElement() { Text = "Input 1",  },
                                    new LabelledStringInputElement() { Text = "Input 2",  },
                                    new LabelledDateTimeInputElement() { Text = "",  },
                                    new LabelledDecimalInputElement() { Text = "Amt",  },
                                    new LabelledCheckboxElement() { Text = "Checkbox", Value = false },
                                    new LabelledBooleanElement() { Text = "Boolean", Value = false },
                                }
                            }
                        },
                    },
                    new NavigationRootViewModel(this.navigation)
                    {
                        Title = "Editable",
                        Sections = {
                            new SectionViewModel()
                            {
                                Items = {
                                    new LabelledStringInputElement() { Text = "Input 1",  },
                                    new LabelledStringInputElement() { Text = "Input 2",  },
                                    new LabelledDateTimeInputElement() { Text = "",  },
                                    new LabelledDecimalInputElement() { Text = "Amt",  },
                                    new LabelledCheckboxElement() { Text = "Checkbox", Value = false },
                                    new LabelledBooleanElement() { Text = "Boolean", Value = false },
                                }
                            }
                        },
                    },
                }
            });
        }
    }

    public class TestRootViewModel : RootViewModel
    {
        public TestRootViewModel()
        {
            this.Title = "Root";

            this.Sections.Add(new SectionViewModel() {
                Header = "Header",
                Footer = "Footer",
                Items = {
                    new SimpleElement("Add Item") {Command = new DelegateCommand(() => this.Sections[0].Items.Add("New Item"))},
                    new SimpleElement("Remove Item") {Command = new DelegateCommand(() => this.Sections[0].Items.RemoveAt(this.Sections[0].Items.Count-1))},
                    new SimpleElement("Add Section") {Command = new DelegateCommand(() => this.Sections.Add(new SectionViewModel() { Header = "New"}))},
                    new SimpleElement("Remove Section") {Command = new DelegateCommand(() => this.Sections.RemoveAt(this.Sections.Count-1))},
                    new SimpleElement("Add Item - last Section") {Command = new DelegateCommand(() => this.Sections[this.Sections.Count-1].Items.Add("An Item"))},
                    "one",
                    "two",
                    3,
                    "four",
                }
            });
        }
    }

    public class TestSectionViewModel : SectionViewModel
    {
        public TestSectionViewModel()
        {
            this.Header = "Header";
            this.Footer = "Footer";
            this.Items.Add(new SimpleElement("Add Item") {Command = new DelegateCommand(() => this.Items.Add("New Item"))});
            this.Items.Add(new SimpleElement("Remove Item") {Command = new DelegateCommand(() => this.Items.RemoveAt(this.Items.Count-1))});
            this.Items.Add("one");
            this.Items.Add("two");
            this.Items.Add(3);
            this.Items.Add("four");
        }
    }

    public class TestEnumerableViewModel : ObservableCollection<object>, IViewModel
    {
        public TestEnumerableViewModel()
        {
            this.Add(new SimpleElement("Add Item") {Command = new DelegateCommand(() => this.Add("New Item"))});
            this.Add(new SimpleElement("Remove Item") {Command = new DelegateCommand(() => this.RemoveAt(this.Count-1))});
            this.Add("one");
            this.Add("two");
            this.Add(3);
            this.Add("four");
        }

        public void Dispose()
        {
            this.Clear();
        }
    }

    public class TestCustomViewModel : IViewModel
    {
        public TestCustomViewModel()
        {
            this.Property1 = "Hello";
            this.Property2 = "World";
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        public string Property1 { get; set; }

        public string Property2 { get; set; }

        public void Dispose()
        {
        }
    }

}
