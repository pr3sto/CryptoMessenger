using System.Windows;
using System.Windows.Controls;

namespace CryptoMessenger.Views.Extensions
{
	/// <summary>
	/// Class for attached properties.
	/// </summary>
	public class Extensions
	{
		// IsDataIncorrect property
		public static readonly DependencyProperty IsDataIncorrectProperty =
			DependencyProperty.RegisterAttached("IsDataIncorrect", typeof(bool), typeof(Extensions), new PropertyMetadata(false));
		public static void SetIsDataIncorrect(UIElement element, bool value)
		{
			element.SetValue(IsDataIncorrectProperty, value);
		}
		public static bool GetIsDataIncorrect(UIElement element)
		{
			return (bool)element.GetValue(IsDataIncorrectProperty);
		}


		// IsDataCorrect property
		public static readonly DependencyProperty IsDataCorrectProperty =
			DependencyProperty.RegisterAttached("IsDataCorrect", typeof(bool), typeof(Extensions), new PropertyMetadata(false));
		public static void SetIsDataCorrect(UIElement element, bool value)
		{
			element.SetValue(IsDataCorrectProperty, value);
		}
		public static bool GetIsDataCorrect(UIElement element)
		{
			return (bool)element.GetValue(IsDataCorrectProperty);
		}


		// for placeholders

		public static readonly DependencyProperty IsMonitoringProperty =
			DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(Extensions), new UIPropertyMetadata(false, OnIsMonitoringChanged));
		public static bool GetIsMonitoring(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsMonitoringProperty);
		}
		public static void SetIsMonitoring(DependencyObject obj, bool value)
		{
			obj.SetValue(IsMonitoringProperty, value);
		}

		// passwordbox placeholder

		public static readonly DependencyProperty PasswordLengthProperty =
			DependencyProperty.RegisterAttached("PasswordLength", typeof(int), typeof(Extensions), new UIPropertyMetadata(0));
		public static int GetPasswordLength(DependencyObject obj)
		{
			return (int)obj.GetValue(PasswordLengthProperty);
		}
		public static void SetPasswordLength(DependencyObject obj, int value)
		{
			obj.SetValue(PasswordLengthProperty, value);
		}		

		// textbox placeholder

		public static readonly DependencyProperty TextLengthProperty =
			DependencyProperty.RegisterAttached("TextLength", typeof(int), typeof(Extensions), new UIPropertyMetadata(0));
		public static int GetTextLength(DependencyObject obj)
		{
			return (int)obj.GetValue(TextLengthProperty);
		}
		public static void SetTextLength(DependencyObject obj, int value)
		{
			obj.SetValue(TextLengthProperty, value);
		}

		private static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is PasswordBox)
			{
				var pb = d as PasswordBox;
				if (pb == null)
				{
					return;
				}
				if ((bool)e.NewValue)
				{
					pb.PasswordChanged += PasswordChanged;
				}
				else
				{
					pb.PasswordChanged -= PasswordChanged;
				}
			}
			else if (d is TextBox)
			{
				var tb = d as TextBox;
				if (tb == null)
				{
					return;
				}
				if ((bool)e.NewValue)
				{
					tb.TextChanged += TextChanged;
				}
				else
				{
					tb.TextChanged -= TextChanged;
				}
			}
		}

		static void PasswordChanged(object sender, RoutedEventArgs e)
		{
			var pb = sender as PasswordBox;
			if (pb == null)
			{
				return;
			}
			SetPasswordLength(pb, pb.Password.Length);
		}

		static void TextChanged(object sender, RoutedEventArgs e)
		{
			var tb = sender as TextBox;
			if (tb == null)
			{
				return;
			}
			SetTextLength(tb, tb.Text.Length);
		}
	}
}
