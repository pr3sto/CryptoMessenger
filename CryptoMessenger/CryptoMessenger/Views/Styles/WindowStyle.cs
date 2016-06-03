using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CryptoMessenger.Views.Styles
{
	// windows style code
	public partial class WindowStyle
	{
		internal class ApiCodes
		{
			public const int SC_RESTORE = 0xF120;
			public const int SC_MINIMIZE = 0xF020;
			public const int WM_SYSCOMMAND = 0x0112;
		}

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

		private IntPtr hWnd;
		private Window window;

		private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == ApiCodes.WM_SYSCOMMAND)
			{
				// minimize actions
				if (wParam.ToInt32() == ApiCodes.SC_MINIMIZE)
				{
					SineEase easingFunction = new SineEase();
					easingFunction.EasingMode = EasingMode.EaseIn;

					var anim = new DoubleAnimation(0, TimeSpan.FromMilliseconds(300));
					anim.EasingFunction = easingFunction;
					anim.Completed += delegate { window.WindowState = WindowState.Minimized; };
					window.BeginAnimation(UIElement.OpacityProperty, anim);

					handled = true;
				}
				// restore actions
				else if (wParam.ToInt32() == ApiCodes.SC_RESTORE)
				{
					SineEase easingFunction = new SineEase();
					easingFunction.EasingMode = EasingMode.EaseIn;

					window.WindowState = WindowState.Normal;
					var anim = new DoubleAnimation(1, TimeSpan.FromMilliseconds(300));
					anim.EasingFunction = easingFunction;
					window.BeginAnimation(UIElement.OpacityProperty, anim);

					handled = true;
				}
			}
			return IntPtr.Zero;
		}

		// window loaded
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Window _window = sender as Window;
			if (_window != null)
			{
				window = _window;
				hWnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
				System.Windows.Interop.HwndSource.FromHwnd(hWnd).AddHook(WindowProc);
			}
		}

		// minimize
		void MinimizeButton_Click(object sender, RoutedEventArgs e)
		{
			SendMessage(hWnd, ApiCodes.WM_SYSCOMMAND, new IntPtr(ApiCodes.SC_MINIMIZE), IntPtr.Zero);
		}

		// close
		void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			// opacity animation
			var opacityAnim = new DoubleAnimation(0, TimeSpan.FromMilliseconds(600));

			// scale Y animation
			SineEase easingFunction = new SineEase();
			easingFunction.EasingMode = EasingMode.EaseIn;
			var sclaeYAnim = new DoubleAnimation(0.001, TimeSpan.FromMilliseconds(300));
			sclaeYAnim.EasingFunction = easingFunction;

			// scale X animation
			var scaleXAnim = new DoubleAnimation(0, TimeSpan.FromMilliseconds(200));
			scaleXAnim.BeginTime = TimeSpan.FromMilliseconds(300);

			// animated transform
			ScaleTransform myScaleTransform = new ScaleTransform(1, 1, 375, 250);
			window.RegisterName("MyAnimatedScaleTransform", myScaleTransform);
			window.RenderTransform = myScaleTransform;

			// storyboard
			var storyboard = new Storyboard();

			Storyboard.SetTargetProperty(opacityAnim, new PropertyPath(UIElement.OpacityProperty));
			Storyboard.SetTargetName(sclaeYAnim, "MyAnimatedScaleTransform");
			Storyboard.SetTargetProperty(sclaeYAnim, new PropertyPath(ScaleTransform.ScaleYProperty));
			Storyboard.SetTargetName(scaleXAnim, "MyAnimatedScaleTransform");
			Storyboard.SetTargetProperty(scaleXAnim, new PropertyPath(ScaleTransform.ScaleXProperty));

			storyboard.Children.Add(opacityAnim);
			storyboard.Children.Add(sclaeYAnim);
			storyboard.Children.Add(scaleXAnim);

			storyboard.Completed += delegate { window.Close(); };

			window.BeginStoryboard(storyboard);
		}

		// drag window
		void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton.Equals(MouseButtonState.Pressed))
			{
				window.DragMove();
			}
		}
	}
}
