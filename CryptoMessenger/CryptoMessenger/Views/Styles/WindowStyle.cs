using System;
using System.Windows;
using System.Windows.Input;

namespace CryptoMessenger.Views.Styles
{
	internal static class LocalExtensions
	{
		// find window
		public static void ForWindowFromTemplate(this object templateFrameworkElement, Action<Window> action)
		{
			Window window = ((FrameworkElement)templateFrameworkElement).TemplatedParent as Window;
			if (window != null) action(window);
		}
	}

	public partial class WindowStyle
	{
		// close
		void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			sender.ForWindowFromTemplate(w => w.Close());
		}

		// minimize
		void MinimizeButton_Click(object sender, RoutedEventArgs e)
		{
			sender.ForWindowFromTemplate(w => w.WindowState = WindowState.Minimized);
		}

		// drag window
		void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton.Equals(MouseButtonState.Pressed))
				sender.ForWindowFromTemplate(w => w.DragMove());
		}
	}
}
