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
			MainWindow view = new MainWindow();
			MainWindowViewModel viewModel = new MainWindowViewModel();
			view.DataContext = viewModel;
			view.Show();
		}
	}
}
