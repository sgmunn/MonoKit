namespace MonoKit.DataBinding
{
    using System;

    public static class DataContextAttachedProperty
    {
        public static AttachedProperty DataContextProperty
        {
            get
            {
                return AttachedProperty.Register("DataContext", typeof(object), typeof(object), new AttachedPropertyMetadata(DataContextChanged));
            }
        }
        
        public static object GetDataContext(this object owner)
        {
            return owner.GetValue(DataContextAttachedProperty.DataContextProperty);
        }
        
        public static void SetDataContext(this object owner, object value)
        {
            owner.SetValue(DataContextAttachedProperty.DataContextProperty, value);
        }
        
        public static void DataContextChanged(object target, AttachedPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                // bind "target" to new value
                // target.Bind(e.NewValue, --template--)
                
                // how do we get the binding information for "target" in an easy manner
                // in xaml this would be defined in the view.  perhaps we do the same
                // by either using attributes on target or having the view register
                // perhaps the view can register define a way of binding
            }
        }
    }
}

