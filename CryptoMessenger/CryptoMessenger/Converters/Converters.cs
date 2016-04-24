﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace CryptoMessenger.Converters
{
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