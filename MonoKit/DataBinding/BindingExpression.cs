namespace MonoKit.DataBinding
{
    using System;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Globalization;
    
    // todo: this class needs some performance work.
    // when values are updated it takes a while to pass that value to the target
    // could this be faster with passed in handler for updating properties in the binding definition
    
    /// <summary>
    /// An instance of a Binding between two objects.
    /// </summary>
    public sealed class BindingExpression : IDisposable
    {
        /// <summary>
        /// The target object that is bound
        /// </summary>
        private WeakReference target;
        
        /// <summary>
        /// The source object that is bound.
        /// </summary>
        private WeakReference source;
  
        /// <summary>
        /// Indicates that the expression has been disposed.
        /// </summary>
        private bool disposed = false;
        
        /// <summary>
        /// The property info of the target objects property.
        /// </summary>
        private PropertyInfo targetPropertyInfo;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoKit.DataBinding.BindingExpression"/> class.
        /// </summary>
        /// <param name='target'>
        /// The target object to bind to.
        /// </param>
        /// <param name='targetProperty'>
        /// The name of the target property.
        /// </param>
        /// <param name='source'>
        /// The source object to bind to.
        /// </param>
        /// <param name='binding'>
        /// The binding for the source object.
        /// </param>
        public BindingExpression(object target, string targetProperty, object source, Binding binding)
        {
            this.Initialize(target, targetProperty, source, binding);
        }
        
        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="MonoKit.DataBinding.BindingExpression"/> is reclaimed by garbage collection.
        /// </summary>
        ~BindingExpression()
        {
            Dispose(false);
        }
        
        /// <summary>
        /// Gets the target of the binding
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        public object Target
        {
            get
            {
                return this.target.Target;
            }
        }
        
        /// <summary>
        /// Gets the name of the target property.
        /// </summary>
        /// <value>
        /// The target property.
        /// </value>
        public string TargetProperty
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Gets the source of the binding.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public object Source
        {
            get
            {
                return this.source.Target;
            }
        }
        
        /// <summary>
        /// Gets the binding for Source.
        /// </summary>
        /// <value>
        /// The binding.
        /// </value>
        public Binding Binding
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Updates the target object from the source object.
        /// </summary>
        public void UpdateTarget()
        {
            if (this.Binding.Mode == BindingMode.OneWayToSource)
            {
                return;
            }
            
            var target = this.Target;
            if (target != null)
            {
                var sourceValue = this.Binding.GetSourceValue(this.Source, this.Target.GetType());

                BindingExpression.SetValue(target, this.targetPropertyInfo, sourceValue);
            }
        }
        
        /// <summary>
        /// Updates the source object from the Target object.
        /// </summary>
        public void UpdateSource()
        {
            if (this.Binding.Mode == BindingMode.OneWay)
            {
                return;
            }
            
            var target = this.Target;
            if (target != null)
            {
                var targetValue = BindingExpression.GetValue(this.Target, this.targetPropertyInfo);
                this.Binding.UpdateSourceValue(this.Source, targetValue);
            }            
        }
        
        /// <summary>
        /// Releases all resource used by the <see cref="MonoKit.DataBinding.BindingExpression"/> object.
        /// </summary>
        /// <remarks>
        /// Call <see cref="Dispose"/> when you are finished using the
        /// <see cref="MonoKit.DataBinding.BindingExpression"/>. The <see cref="Dispose"/> method leaves the
        /// <see cref="MonoKit.DataBinding.BindingExpression"/> in an unusable state. After calling
        /// <see cref="Dispose"/>, you must release all references to the
        /// <see cref="MonoKit.DataBinding.BindingExpression"/> so the garbage collector can reclaim the memory that the
        /// <see cref="MonoKit.DataBinding.BindingExpression"/> was occupying.
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// Disposes the resources used by this object.
        /// </summary>
        /// <param name='disposing'>
        /// Disposing.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.Binding.Dispose();
                    this.UnregisterForChanges();
                }
                
                disposed = true;
            }
        }
        
        /// <summary>
        /// Unregisters for changes in the source and target objects.
        /// </summary>
        private void UnregisterForChanges()
        {
            var s = this.Source;
            INotifyPropertyChanged inpc = s as INotifyPropertyChanged;
            if (inpc != null)
            {
                inpc.PropertyChanged -= HandleSourcePropertyChanged;
            }

            var t = this.Target;
            inpc = t as INotifyPropertyChanged;
            if (inpc != null)
            {
                inpc.PropertyChanged -= HandleTargetPropertyChanged;
            }
        }
        
        /// <summary>
        /// Initialize the binding instance.
        /// </summary>
        /// <param name='target'>
        /// The target object to bind to.
        /// </param>
        /// <param name='targetProperty'>
        /// The name of the Target property.
        /// </param>
        /// <param name='source'>
        /// The Source object to bind to.
        /// </param>
        /// <param name='binding'>
        /// The source Binding.
        /// </param>
        private void Initialize(object target, string targetProperty, object source, Binding binding)
        {
            this.target = new WeakReference(target);
            this.source = new WeakReference(source);
            this.TargetProperty = targetProperty;
            this.Binding = binding;

            this.RegisterForPropertyChangesOnSource();
            this.RegisterForPropertyChangesOnTarget();
            
            this.targetPropertyInfo = target.GetPropertyInfo(this.TargetProperty);
            
            this.UpdateTarget();
        }
        
        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <returns>
        /// The converted value.
        /// </returns>
        /// <param name='value'>
        /// The Value to convert.
        /// </param>
        /// <param name='propertyInfo'>
        /// The PropertyInfo that describes the target type of the conversion.
        /// </param>
        private object ConvertValue(object value, PropertyInfo propertyInfo)
        {
            object convertedValue = value;
            
            if (this.Binding.Converter != null)
            {
                convertedValue = Binding.Converter.Convert(value, propertyInfo.PropertyType, Binding.ConverterParameter, CultureInfo.CurrentUICulture);
            }
            
            return convertedValue;
        }
        
        /// <summary>
        /// Gets the value of an object with the given property info.
        /// </summary>
        /// <returns>
        /// The value.
        /// </returns>
        /// <param name='o'>
        /// The object to get the value from
        /// </param>
        /// <param name='propertyInfo'>
        /// The Property info to get.
        /// </param>
        private static object GetValue(object o, PropertyInfo propertyInfo)
        {
            if (o != null && propertyInfo != null)
            {
                return propertyInfo.GetValue(o, null);
            }
            
            return null;
        }
        
        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name='o'>
        /// The object whose value to set.
        /// </param>
        /// <param name='propertyInfo'>
        /// The Property info that describes which value to set.
        /// </param>
        /// <param name='newValue'>
        /// The New value of the property.
        /// </param>
        private static void SetValue(object o, PropertyInfo propertyInfo, object newValue)
        {
            if (o != null && propertyInfo != null && propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(o, newValue, null);
            }
        }
  
        /// <summary>
        /// Registers for property changes on the source object.  The source object must implement INotifyPropertyChanged.
        /// </summary>
        private void RegisterForPropertyChangesOnSource()
        {
            var s = this.Source;
            var inpc = s as INotifyPropertyChanged;
            
            if (inpc != null)
            {
                inpc.PropertyChanged -= HandleSourcePropertyChanged;
                inpc.PropertyChanged += HandleSourcePropertyChanged;
            }
        }
        
        /// <summary>
        /// Registers for property changes on the target object.  The target object must implement INotifyPropertyChanged.
        /// </summary>
        private void RegisterForPropertyChangesOnTarget()
        {
            var t = this.Target;
            var inpc = t as INotifyPropertyChanged;
            if (inpc != null)
            {
                inpc.PropertyChanged -= HandleTargetPropertyChanged;
                inpc.PropertyChanged += HandleTargetPropertyChanged;
            }
        }
  
        /// <summary>
        /// Handles changes in the source object.
        /// </summary>
        /// <param name='sender'>
        /// Sender.
        /// </param>
        /// <param name='e'>
        /// E.
        /// </param>
        private void HandleSourcePropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName.Equals(this.Binding.PropertyName))
            {
                this.UpdateTarget();
            }
        }

        /// <summary>
        /// Handles changes in the target object.
        /// </summary>
        /// <param name='sender'>
        /// Sender.
        /// </param>
        /// <param name='e'>
        /// E.
        /// </param>
        private void HandleTargetPropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName.Equals(this.TargetProperty))
            {
                this.UpdateSource();
            }
        }
    }
}

