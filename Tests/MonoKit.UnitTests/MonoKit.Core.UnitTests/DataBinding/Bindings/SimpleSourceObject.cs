using System;
using System.ComponentModel;

namespace MonoKit.Core.UnitTests.Bindings
{
    public class SimpleSourceObject : INotifyPropertyChanged
    {
        private string property1;
        private bool property2;

        public event PropertyChangedEventHandler PropertyChanged; 
        
        public string Property1
        {
            get
            {
                return this.property1;
            }
            
            set
            {
                if (value != this.property1)
                {
                    this.property1 = value;
                    this.NotifyPropertyChanged("Property1");
                }
            }
        }
        
        public bool Property2
        {
            get
            {
                return this.property2;
            }
            
            set
            {
                if (value != this.property2)
                {
                    this.property2 = value;
                    this.NotifyPropertyChanged("Property2");
                }
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
