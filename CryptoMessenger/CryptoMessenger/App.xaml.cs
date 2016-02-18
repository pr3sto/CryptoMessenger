﻿using System.Windows;
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
			MainViewModel viewModel = new MainViewModel();
			view.DataContext = viewModel;
			view.Show();
		}
	}
}