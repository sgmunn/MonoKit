namespace MonoKit.DataBinding
{
    using System;
 
    /// <summary>
    /// Boolean to string converter.
    /// </summary>
    public class BooleanToStringConverter : IValueConverter
    {
        public BooleanToStringConverter()
        {
        }

        public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToBoolean((string)value);
        }
    }
    
    public class DecimalToStringConverter : IValueConverter
    {
        public DecimalToStringConverter()
        {
        }

        public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToDecimal((string)value);
        }
    }
}

