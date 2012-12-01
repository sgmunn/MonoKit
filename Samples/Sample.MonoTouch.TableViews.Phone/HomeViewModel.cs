//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="HomeViewModel.cs" company="sgmunn">
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

namespace Sample.TableViews
{
    using System;
    using MonoKit.Controls;
    using MonoKit.ViewModels;
    using MonoTouch.UIKit;

    public class HomeViewModel : RootViewModel
    {
        private readonly INavigationService navigation;
        
        public HomeViewModel(INavigationService navigation)
        {
            this.navigation = navigation;
            this.Load();
        }
        
        private void Load()
        {
            var section1 = new SectionViewModel()
            {
                Header = "TableView",
                Items = 
                {
                    new SimpleElement("TableView Samples") { 
                        Command = new DelegateCommand(() => this.navigation.NavigateTo(new HomeViewModelX(this.navigation))) 
                    },
                    new SimpleElement("CollectionView Samples") { 
                        Command = new DelegateCommand(() => this.navigation.NavigateTo(new CollectionViewSampleViewModel())) 
                    },
                }
            };

            this.Sections.Add(section1);
        }
    }

    public class CollectionViewSampleViewModel : RootViewModel
    {
        public CollectionViewSampleViewModel()
        {
            this.Load();
        }

        private void Load()
        {
            var section1 = new SectionViewModel()
            {
                Header = "CollectionView",
                Items = 
                {
                    new SimpleElement("Add") { 
                        Command = new DelegateCommand(() => this.Sections[0].Items.Add("Added")) 
                    },
                    new SimpleElement("Remove") { 
                        Command = new DelegateCommand(() => this.Sections[0].Items.RemoveAt(this.Sections[0].Items.Count-1)) 
                    },
                    new SimpleElement("Add") { 
                        Command = new DelegateCommand(() => this.Sections.Add(new SectionViewModel())) 
                    },
                    new SimpleElement("Add") { 
                        Command = new DelegateCommand(() => this.Sections[1].Items.Add("Added")) 
                    },
                    "hello",
                    123,
                }
            };
            
            this.Sections.Add(section1);
            this.Sections.Add(new SectionViewModel() { Items = {""}});
        }
    }



    public class RandomObject
    {
        ~RandomObject()
        {
            Console.WriteLine("~RandomObject");
        }
    }

    public class ButtonViewModel : ViewModelBase, ICommand
    {
        private RootViewModel parent;
        private bool doAdd;

        public ButtonViewModel(RootViewModel parent, bool doAdd)
        {
            this.parent = parent;
            this.doAdd = doAdd;
        }

        ~ButtonViewModel()
        {
            Console.WriteLine("~ButtonViewModel");
        }


        public override void Execute()
        {
            if (this.doAdd)
            {
                this.parent.Sections[0].Items.Add("new item");
            }
            else
            {
                this.parent.Sections[0].Items.RemoveAt(this.parent.Sections[0].Items.Count-1);
            }
        }

        public override bool GetCanExecute()
        {
            return true;
        }
    }

    public class HomeViewModelX : RootViewModel
    {
        private readonly INavigationService navigation;

        public HomeViewModelX(INavigationService navigation)
        {
            this.navigation = navigation;
            this.Load();
        }

        private void Load()
        {
            var section1 = new SectionViewModel()
            {
                Header = "Section 1",
                //Footer = new LabelledCheckbox() { Text = "Footer", Value = true },
                Items = 
                {
                    new ButtonViewModel(this, true),
                    new ButtonViewModel(this, false),
                    "item A 1",
                    new LabelledStringInputElement() { Text = "Input 1",  },
                    123,
                    new LabelledStringInputElement() { Text = "Input 2",  },
                    new LabelledDateTimeInputElement() { Text = "",  },
                    new LabelledDecimalInputElement() { Text = "Amt",  },
                    new LabelledCheckboxElement() { Text = "Checkbox", Value = false },
                    new LabelledBooleanElement() { Text = "Boolean", Value = false },
                }
            };
            
            
            
            this.Sections.Add(section1);
            this.Sections.Add(new SectionViewModel() { Footer = "section 2" });
            this.Sections.Add(new SectionViewModel() { Header = "Section 3" });



            // this root model is where we would push another tvc
            // is this a built-in thing or something that you wire up
            // subSection.Command = this.NavigateTo();
            // i think in this case I prefer to wire up the command myself
            // that means that ICommand needs to execute a delegate
            // IDelegateCommand - delegate Command;




            var subSection = new RootViewModel()
            {
                Title = "Sub-Section",
                Sections = {
                    new SectionViewModel()
                    {
                        Header = "hello",
                        Items = {
                            "world",
                            new RandomObject(),
                        }
                    }
                },
            };

            subSection.Command = new DelegateCommand(() => this.navigation.NavigateTo(subSection));

            subSection.Sections[0].Items.Add(new ButtonViewModel(subSection, true));
            
            this[1].Items.Add(subSection);
            

            for (int i = 0; i < 100; i++)
            {
                this.Sections[2].Items.Add("item B "+i.ToString());
            }
        }

    
    }
    
    public class ButtonViewModel1 : ViewModelBase, ICommand
    {
        private AlternateHomeViewModel parent;
        private bool doAdd;
        
        public ButtonViewModel1(AlternateHomeViewModel parent, bool doAdd)
        {
            this.parent = parent;
            this.doAdd = doAdd;
        }
        
        ~ButtonViewModel1()
        {
            Console.WriteLine("~ButtonViewModel_1");
        }
        
        
        public override void Execute()
        {
            if (this.doAdd)
            {
                var x = "new item";
                this.parent.Stuff.Add(x);
                this.parent.NotifyCollectionChange(NotifyCollectionChangedAction.Add, x, this.parent.Stuff.Count - 1);
            }
            else
            {
                this.parent.Stuff.RemoveAt(this.parent.Stuff.Count-1);
            }
        }
        
        public override bool GetCanExecute()
        {
            return true;
        }
    }

    // do we have to hard code the binding for this viewmodel is we want to use this over section or sectionroot ??
    public class AlternateHomeViewModel : ViewModelBase, IEnumerable, INotifyCollectionChanged
    {
        public AlternateHomeViewModel()
        {
            this.Stuff = new List<object>();
            this.Stuff.Add(new ButtonViewModel1(this, true));
            this.Stuff.Add(new ButtonViewModel1(this, false));
            this.Stuff.Add("11111");
            this.Stuff.Add("22222");
            this.Stuff.Add("33333");
            //this.Header = "1111";
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public List<object> Stuff { get; private set; }




        public IEnumerator GetEnumerator()
        {
            return this.Stuff.GetEnumerator();
        }

        public void NotifyCollectionChange(NotifyCollectionChangedAction action, object item, int index)
        {
            var change = this.CollectionChanged;
            if (change != null)
            {
                change(this, new NotifyCollectionChangedEventArgs(action, item, index));
            }
        }
    }
}
