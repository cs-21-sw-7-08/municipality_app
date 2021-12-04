using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MunicipalityApp.Converters
{
    public class DisabledColorConverter : IMultiValueConverter
    {
        private byte GetColorValue(int value)
        {
            if (value - 0x30 < 0)
                return 0;
            else
                return (byte)(value - 0x30);
        }

        private Color CalculatePressedColor(Color color)
        {
            color.R = GetColorValue(color.R);
            color.G = GetColorValue(color.G);
            color.B = GetColorValue(color.B);
            return color;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is false)
            {
                if (values[1] is SolidColorBrush)
                {
                    var solidColorBrush = values[1] as SolidColorBrush;
                    var tempSolidColorBrush = new SolidColorBrush()
                    {
                        Color = CalculatePressedColor(solidColorBrush.Color)
                    };
                    return tempSolidColorBrush;
                }
                else if (values[1] is LinearGradientBrush)
                {
                    var gradientBrush = values[1] as LinearGradientBrush;
                    var tempGradientBrush = new LinearGradientBrush()
                    {
                        StartPoint = gradientBrush.StartPoint,
                        EndPoint = gradientBrush.EndPoint
                    };
                    foreach (var obj in gradientBrush.GradientStops)
                    {
                        var gradientStop = new GradientStop()
                        {
                            Color = CalculatePressedColor(obj.Color),
                            Offset = obj.Offset
                        };
                        tempGradientBrush.GradientStops.Add(gradientStop);
                    }
                    return tempGradientBrush;
                }
            }
            else
            {
                return values[1];
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
