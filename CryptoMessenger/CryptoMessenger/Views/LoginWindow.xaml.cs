using System.Windows;
using System.Windows.Media;

namespace CryptoMessenger.Views
{
	/// <summary>
	/// Interaction logic for LoginWindow.xaml
	/// </summary>
	public partial class LoginWindow : Window
	{
		public LoginWindow()
		{
			InitializeComponent();

			// window colors
			Resources["WindowContentBackground1Brush"] = new SolidColorBrush(Color.FromRgb(76, 71, 67));
			Resources["WindowContentBackground2Brush"] = new SolidColorBrush(Color.FromRgb(129, 129, 129));
			Resources["WindowContentBackground3Brush"] = new SolidColorBrush(Color.FromRgb(244, 240, 228));
			Resources["WindowBorderBrush"] = new SolidColorBrush(Color.FromRgb(237, 87, 132));
			Resources["BorderShadowColor"] = Color.FromRgb(237, 87, 132);

			// textbox colors
			Resources["TextBoxBorderBrush"] = new SolidColorBrush(Color.FromRgb(129, 129, 129));
			Resources["FocusedTextBoxBorderBrush"] = new SolidColorBrush(Color.FromRgb(237, 87, 132));
			Resources["SelectionBrush"] = new SolidColorBrush(Color.FromRgb(100, 100, 100));

			// button colors
			Resources["ButtonBackgroundNormalBrush"] = new SolidColorBrush(Color.FromRgb(129, 129, 129));
			Resources["ButtonBackgroundHoverBrush"] = new SolidColorBrush(Color.FromRgb(149, 149, 144));
			Resources["ButtonForegroundPressedBrush"] = new SolidColorBrush(Color.FromRgb(237, 87, 132));

			// checkbox colors
			Resources["CheckBoxBorderBrush"] = new SolidColorBrush(Color.FromRgb(129, 129, 129));
			Resources["CheckBoxCheckBrush"] = new SolidColorBrush(Color.FromRgb(237, 87, 132));
		}
	}
}
