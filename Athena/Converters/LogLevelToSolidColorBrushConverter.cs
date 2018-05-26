using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Zeus.Log;

namespace Athena.Converters
{
    /// <summary>
    /// Handle conversion between <see cref="LogLevels"/> and <see cref="SolidColorBrush"/> types.
    /// </summary>
    public class LogLevelToSolidColorBrushConverter : IValueConverter
    {
        #region IValueConverter interface

        /// <summary>
        /// Converts from a <see cref="LogLevels"/> value to a <see cref="SolidColorBrush"/> value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Return a <see cref="SolidColorBrush"/> value according with the given <paramref name="value"/>.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch((LogLevels)value)
            {
                case LogLevels.Error:
                case LogLevels.Fatal:
                    return new SolidColorBrush((Color)Application.Current.FindResource("RedColor"));
                case LogLevels.Warning:
                    return new SolidColorBrush((Color)Application.Current.FindResource("YellowColor"));
                case LogLevels.Success:
                    return new SolidColorBrush((Color)Application.Current.FindResource("GreenColor"));
                case LogLevels.Debug:
                case LogLevels.Trace:
                    return new SolidColorBrush((Color)Application.Current.FindResource("BlueColor"));
                default:
                    return DependencyProperty.UnsetValue;
            }
        }
        /// <summary>
        /// Converts from a <see cref="SolidColorBrush"/> value to a <see cref="LogLevels"/> value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Return a <see cref="LogLevels"/> value according with the given <paramref name="value"/>.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
