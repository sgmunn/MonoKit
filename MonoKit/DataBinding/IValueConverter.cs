namespace MonoKit.DataBinding
{
    using System;
    using System.Globalization;
 
    /// <summary>
    /// Defines an interface for converting a value to another type and back.
    /// </summary>
    public interface IValueConverter
    {
        object Convert(object value, Type targetType, object parameter, CultureInfo culture);
        object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
    }
}

