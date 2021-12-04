using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MunicipalityApp.Converters
{
    public class BoolToStringConverter : IValueConverter
    {
        public string TrueString { get; set; }
        public string FalseString { get; set; }

        private object Value { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                Value = value;
                if (value is bool)
                {
                    return ((bool)value) ? TrueString : FalseString;
                }
            }
            catch { }

            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Value;
        }
    }
}
