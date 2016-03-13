using System.Windows;
using CryptoMessenger.ViewModels;
using CryptoMessenger.Views;

namespace CryptoMessenger
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		// load view on startup
		private void OnStartup(object sender, StartupEventArgs e)
		{
			LoginWindow view = new LoginWindow();
			LoginWindowViewModel viewModel = new LoginWindowViewModel();
			view.DataContext = viewModel;
			view.Show();

			//MainWindow view = new MainWindow();
			//MainWindowViewModel viewModel = new MainWindowViewModel(null, "ololsha");
			//view.DataContext = viewModel;
			//view.Show();
		}
	}
}
