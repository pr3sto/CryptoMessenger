using System.Windows.Input;

using CryptoMessenger.Commands;
using CryptoMessenger.Models;

namespace CryptoMessenger.ViewModels
{
	/// <summary>
	/// View model for main window (mvvm pattern).
	/// </summary>
	class MainWindowViewModel : ViewModelBase
	{
		private Client client;

		// window main panels
		private LoginPanelViewModel loginPanelViewModel;
		private MainPanelViewModel mainPanelViewModel;

		public MainWindowViewModel()
		{
			client = new Client();
			ShowWarning = false;
			client.ConnectionBreaks += () => { ShowWarning = true; };

			ShowLoginPanel();
		}

		private void ShowMainPanel(string login)
		{
			mainPanelViewModel = new MainPanelViewModel(client);
			MainWindowPanel = mainPanelViewModel;
			WindowTitle = login;
		}

		private void ShowLoginPanel()
		{
			loginPanelViewModel = new LoginPanelViewModel(client);
			loginPanelViewModel.LoginSuccess += ShowMainPanel;
			MainWindowPanel = loginPanelViewModel;
			WindowTitle = Properties.Resources.APP_NAME;
		}

		#region Properties

		// window title
		private string _windowTitle;
		public string WindowTitle
		{
			get { return _windowTitle; }
			set
			{
				_windowTitle = value;
				OnPropertyChanged(nameof(WindowTitle));
			}
		}

		// warning 
		private bool _showWarning;
		public bool ShowWarning
		{
			get { return _showWarning; }
			set
			{
				_showWarning = value;
				OnPropertyChanged(nameof(ShowWarning));
			}
		}

		// panel
		private IWindowPanel _mainWindowPanel;
		public IWindowPanel MainWindowPanel
		{
			get { return _mainWindowPanel; }
			set
			{
				_mainWindowPanel = value;
				OnPropertyChanged(nameof(MainWindowPanel));
			}
		}

		#endregion

		#region Commands

		// hide warning
		private DelegateCommand hideWarningCommand;
		public ICommand HideWarningCommand
		{
			get
			{
				if (hideWarningCommand == null)
				{
					hideWarningCommand = new DelegateCommand(() => { ShowWarning = false; });
				}
				return hideWarningCommand;
			}
		}

		// exit
		private DelegateCommand exitCommand;
		public ICommand ExitCommand
		{
			get
			{
				if (exitCommand == null)
				{
					exitCommand = new DelegateCommand(() => 
					{
						client.Logout();
						ShowWarning = false;
						ShowLoginPanel();
					});
				}
				return exitCommand;
			}
		}

		#endregion
	}
}
