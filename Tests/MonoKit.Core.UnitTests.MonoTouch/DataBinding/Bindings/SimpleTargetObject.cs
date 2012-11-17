
using System;
using System.ComponentModel;

namespace MonoKit.Core.UnitTests.Bindings
{
    public class SimpleTargetObject : INotifyPropertyChanged
    {
        private string propertyA;

        public event PropertyChangedEventHandler PropertyChanged; 

        public string PropertyA
        {
            get
            {
                return this.propertyA;
            }

            set
            {
                if (value != this.propertyA)
                {
                    this.propertyA = value;
                    this.NotifyPropertyChanged("PropertyA");
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
