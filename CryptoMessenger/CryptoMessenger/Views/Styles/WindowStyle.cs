using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Linq;

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

		internal enum Theme { Light, Dark };
		Theme currentTheme = Theme.Dark;

		private const string LightThemePath = "Views/Themes/LightTheme.xaml";
		private const string DarkThemePath = "Views/Themes/DarkTheme.xaml";


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

				// transform for animation
				ScaleTransform myScaleTransform = new ScaleTransform(1, 1, 375, 250);
				window.RegisterName("MyAnimatedScaleTransform", myScaleTransform);
				window.RenderTransform = myScaleTransform;

				hWnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
				System.Windows.Interop.HwndSource.FromHwnd(hWnd).AddHook(WindowProc);
			}
		}

		// change theme
		void ThemeButton_Click(object sender, RoutedEventArgs e)
		{
			// opacity animation
			var opacityDownAnim = new DoubleAnimation(0, TimeSpan.FromMilliseconds(300));
			var opacityUpAnim = new DoubleAnimation(1, TimeSpan.FromMilliseconds(300));

			// scale animation
			SineEase easingFunction = new SineEase();
			easingFunction.EasingMode = EasingMode.EaseIn;
			var scaleDownAnim = new DoubleAnimation(0.001, TimeSpan.FromMilliseconds(300));
			scaleDownAnim.EasingFunction = easingFunction;
			var scaleUpAnim = new DoubleAnimation(1, TimeSpan.FromMilliseconds(300));
			scaleUpAnim.EasingFunction = easingFunction;

			// storyboard
			var storyboard = new Storyboard();

			Storyboard.SetTargetProperty(opacityDownAnim, new PropertyPath(UIElement.OpacityProperty));
			Storyboard.SetTargetName(scaleDownAnim, "MyAnimatedScaleTransform");
			Storyboard.SetTargetProperty(scaleDownAnim, new PropertyPath(ScaleTransform.ScaleYProperty));

			Storyboard.SetTargetProperty(opacityUpAnim, new PropertyPath(UIElement.OpacityProperty));
			Storyboard.SetTargetName(scaleUpAnim, "MyAnimatedScaleTransform");
			Storyboard.SetTargetProperty(scaleUpAnim, new PropertyPath(ScaleTransform.ScaleYProperty));

			storyboard.Children.Add(opacityDownAnim);
			storyboard.Children.Add(scaleDownAnim);

			storyboard.Completed += delegate
			{
				ChangeTheme();
				storyboard = new Storyboard();
				storyboard.Children.Add(opacityUpAnim);
				storyboard.Children.Add(scaleUpAnim);
				window.BeginStoryboard(storyboard);
			};

			window.BeginStoryboard(storyboard);
		}

		void ChangeTheme()
		{
			if (currentTheme == Theme.Dark)
			{
				// remove old theme
				Application.Current.Resources.MergedDictionaries.Remove(
					Application.Current.Resources.MergedDictionaries.FirstOrDefault(
						x => x.Source.OriginalString.Equals(DarkThemePath)
						)
					);

				// apply new theme
				var newTheme = new ResourceDictionary();
				newTheme.Source = new Uri(LightThemePath, UriKind.Relative);
				Application.Current.Resources.MergedDictionaries.Add(newTheme);

				currentTheme = Theme.Light;
			}
			else
			{
				// remove old theme
				Application.Current.Resources.MergedDictionaries.Remove(
					Application.Current.Resources.MergedDictionaries.FirstOrDefault(
						x => x.Source.OriginalString.Equals(LightThemePath)
						)
					);

				// apply new theme
				var newTheme = new ResourceDictionary();
				newTheme.Source = new Uri(DarkThemePath, UriKind.Relative);
				Application.Current.Resources.MergedDictionaries.Add(newTheme);

				currentTheme = Theme.Dark;
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
