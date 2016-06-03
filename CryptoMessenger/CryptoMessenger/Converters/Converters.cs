using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace CryptoMessenger.Converters
{
	/// <summary>
	/// Converter that convert background color to foreground.
	/// </summary>
	[ValueConversion(typeof(SolidColorBrush), typeof(SolidColorBrush))]
	public class ForegroundColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var b = (SolidColorBrush)value;
			var color = System.Drawing.Color.FromArgb(b.Color.A, b.Color.R, b.Color.G, b.Color.B);
			float brightness = color.GetBrightness();

			if (brightness < 0.2f)
				return Properties.Settings.Default.SecondaryFirstBrush;
			else if (brightness < 0.5f)
				return Properties.Settings.Default.SecondarySecondBrush;
			else if (brightness < 0.8f)
				return Properties.Settings.Default.MainSecondBrush;
			else
				return Properties.Settings.Default.MainFirstBrush;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}

	/// <summary>
	/// Converter that inverse boolean.
	/// </summary>
	[ValueConversion(typeof(bool), typeof(bool))]
	public class InverseBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return !(bool)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}

	/// <summary>
	/// Convert string emptyness to boolean.
	/// </summary>
	[ValueConversion(typeof(string), typeof(bool))]
	public class StringEmptynessToBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return !string.IsNullOrEmpty((string)value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}

	/// <summary>
	/// Group converters to use like one.
	/// </summary>
	public class ValueConverterGroup : List<IValueConverter>, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return this.Aggregate(value, (current, converter) => 
				converter.Convert(current, targetType, parameter, culture));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
