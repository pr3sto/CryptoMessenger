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

		private void ShowMainPanel()
		{
			mainPanelViewModel = new MainPanelViewModel(client);
			mainPanelViewModel.Logout += ShowLoginPanel;
			MainWindowPanel = mainPanelViewModel;
		}

		private void ShowLoginPanel()
		{
			loginPanelViewModel = new LoginPanelViewModel(client);
			loginPanelViewModel.LoginSuccess += ShowMainPanel;
			MainWindowPanel = loginPanelViewModel;
		}

		#region Properties

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

		// logout
		private DelegateCommand logoutCommand;
		public ICommand LogoutCommand
		{
			get
			{
				if (logoutCommand == null)
				{
					logoutCommand = new DelegateCommand(() => 
					{
						client.Logout();
						ShowWarning = false;
						ShowLoginPanel();
					});
				}
				return logoutCommand;
			}
		}

		// do before closing
		private DelegateCommand windowClosingCommand;
		public ICommand WindowClosingCommand
		{
			get
			{
				if (windowClosingCommand == null)
				{
					windowClosingCommand = new DelegateCommand(() =>
					{ client.Logout(); });
				}
				return windowClosingCommand;
			}
		}

		#endregion
	}
}
