using System;
using System.Windows.Input;
using System.Windows.Controls;

using CryptoMessenger.Commands;
using CryptoMessenger.Models;
using CryptoMessenger.Views;

using MessageProtocol.MessageTypes;
using MessageProtocol;

namespace CryptoMessenger.ViewModels
{
	/// <summary>
	/// View model for login window (mvvm pattern).
	/// </summary>
    class LoginWindowViewModel : ViewModelBase
    {
		private Client client;

		public LoginWindowViewModel()
        {
			client = new Client();

			IsWindowVisible = true;
			Notification = Properties.Resources.STANDART_NOTIFICATION;
			IsLoading = false;
		}

		#region Properties

		// is something loading visibility
		private bool _isLoading;
		public bool IsLoading
		{
			get { return _isLoading; }
			set
			{
				_isLoading = value;
				OnPropertyChanged(nameof(IsLoading));
			}
		}

		// window visibility
		private bool _isWindowVisible;
		public bool IsWindowVisible
		{
			get { return _isWindowVisible; }
			set
			{
				_isWindowVisible = value;
				OnPropertyChanged(nameof(IsWindowVisible));
			}
		}

		// notification textblock
		private string _notification;
		public string Notification
		{
			get { return _notification; }
			set
			{
				_notification = value;
				OnPropertyChanged(nameof(Notification));
			}
		}

		// user login
		private string _login;
		public string Login
		{
			get { return _login; }
			set
			{
				_login = value;
				OnPropertyChanged(nameof(Login));
			}
		}

		// is user login incorrect
		private bool _isLoginIncorrect;
		public bool IsLoginIncorrect
		{
			get { return _isLoginIncorrect; }
			set
			{
				_isLoginIncorrect = value;
				OnPropertyChanged(nameof(IsLoginIncorrect));
			}
		}

		// is user login correct
		private bool _isLoginCorrect;
		public bool IsLoginCorrect
		{
			get { return _isLoginCorrect; }
			set
			{
				_isLoginCorrect = value;
				OnPropertyChanged(nameof(IsLoginCorrect));
			}
		}

		// is user password incorrect
		private bool _isPasswordIncorrect;
		public bool IsPasswordIncorrect
		{
			get { return _isPasswordIncorrect; }
			set
			{
				_isPasswordIncorrect = value;
				OnPropertyChanged(nameof(IsPasswordIncorrect));
			}
		}

		// is user password correct
		private bool _isPasswordCorrect;
		public bool IsPasswordCorrect
		{
			get { return _isPasswordCorrect; }
			set
			{
				_isPasswordCorrect = value;
				OnPropertyChanged(nameof(IsPasswordCorrect));
			}
		}

		#endregion

		#region Commands

		// login
		private DelegateCommand<object> loginCommand;
		public ICommand LoginCommand
		{
			get
			{
				if (loginCommand == null)
				{
					loginCommand = new DelegateCommand<object>(DoLogin);
				}
				return loginCommand;
			}
		}
		private async void DoLogin(object passwordBox)
		{
			PasswordBox pb = (PasswordBox)passwordBox;
			string Password = pb.Password;

			SetDefaultColors();

			if (IsUserDataCorrect(Password))
			{
				IsLoading = true;
				
				Notification = Properties.Resources.LOGIN_NOTIFICATION;

				// try login
				LoginRegisterResponse response;
				try
				{
					response = await client.Login(Login, Password);
				}
				catch (ConnectionInterruptedException)
				{
					Notification = Properties.Resources.SERVER_CONNECTION_ERROR_NOTIFICATION;
					_isColorsChanged = true;
					
					IsLoading = false;
					return;
				}
				catch (CertificateException)
				{
					Notification = Properties.Resources.CERTIFICATE_ERROR_NOTIFICATION;
					_isColorsChanged = true;

					IsLoading = false;
					return;
				}

				if (LoginRegisterResponse.SUCCESS.Equals(response))
				{
					IsWindowVisible = false;

					// open main window
					MainWindow view = new MainWindow();
					MainWindowViewModel viewModel = new MainWindowViewModel(client, Login);
					view.DataContext = viewModel;
					view.ShowDialog();
					// close main window
					client.Logout();
					view.Close();	

					// free memory
					GC.Collect(); // ouch...
					GC.WaitForPendingFinalizers();

					Notification = Properties.Resources.STANDART_NOTIFICATION;
					IsWindowVisible = true;
				}
				else if (LoginRegisterResponse.FAIL.Equals(response))
				{
					Notification = Properties.Resources.LOGIN_ERROR_NOTIFICATION;
					_isColorsChanged = true;
					IsLoginIncorrect = true;
					IsPasswordIncorrect = true;
				}
				else if (LoginRegisterResponse.ERROR.Equals(response))
				{
					Notification = Properties.Resources.UNKNOWN_ERROR;
					_isColorsChanged = true;
				}
				else if (LoginRegisterResponse.ALREADY_LOGIN.Equals(response))
				{
					Notification = Properties.Resources.ALREADY_LOGIN_NOTIFICATION;
					_isColorsChanged = true;
				}
				
				IsLoading = false;
			}
		}		

		// register
		private DelegateCommand<object> registerCommand;
		public ICommand RegisterCommand
		{
			get
			{
				if (registerCommand == null)
				{
					registerCommand = new DelegateCommand<object>(DoRegister);
				}
				return registerCommand;
			}
		}
		private async void DoRegister(object passwordBox)
		{
			PasswordBox pb = (PasswordBox)passwordBox;
			string Password = pb.Password;
		
			SetDefaultColors();

			if (IsUserDataCorrect(Password))
			{
				IsLoading = true;
				
				Notification = Properties.Resources.REGISTRATION_NOTIFICATION;

				// try register
				LoginRegisterResponse response;
				try
				{
					response = await client.Register(Login, Password);
				}
				catch (ConnectionInterruptedException)
				{
					Notification = Properties.Resources.SERVER_CONNECTION_ERROR_NOTIFICATION;
					_isColorsChanged = true;
					
					IsLoading = false;
					return;
				}
				catch (CertificateException)
				{
					Notification = Properties.Resources.CERTIFICATE_ERROR_NOTIFICATION;
					_isColorsChanged = true;
					
					IsLoading = false;
					return;
				}

				if (LoginRegisterResponse.SUCCESS.Equals(response))
				{
					Notification = Properties.Resources.REGISTRATION_SUCCESS_NOTIFICATION;
					_isColorsChanged = true;
					IsLoginCorrect = true;
					IsPasswordCorrect = true;
				}
				else if (LoginRegisterResponse.FAIL.Equals(response))
				{
					Notification = Properties.Resources.REGISTRATION_ERROR_NOTIFICATION;
					_isColorsChanged = true;
					IsLoginIncorrect = true;
					IsPasswordIncorrect = true;
				}
				else if (LoginRegisterResponse.ERROR.Equals(response))
				{
					Notification = Properties.Resources.UNKNOWN_ERROR;
					_isColorsChanged = true;
				}
				
				IsLoading = false;
			}
		}

		// textbox got focus
		private DelegateCommand textBoxGotFocusCommand;
		public ICommand TextBoxGotFocusCommand
		{
			get
			{
				if (textBoxGotFocusCommand == null)
				{
					textBoxGotFocusCommand = new DelegateCommand(SetDefaultColors);
				}
				return textBoxGotFocusCommand;
			}
		}

		#endregion

		// set default colors
		bool _isColorsChanged = false;
		private void SetDefaultColors()
		{
			if (_isColorsChanged)
			{
				Notification = Properties.Resources.STANDART_NOTIFICATION;
				_isColorsChanged = false;

				IsLoginIncorrect = false;
				IsPasswordIncorrect = false;

				IsLoginCorrect = false;
				IsPasswordCorrect = false;
			}
		}

		// check textboxes
		private bool IsUserDataCorrect(string Password)
		{
			bool ret = true;

			if (string.IsNullOrEmpty(Login))
			{
				Notification = Properties.Resources.INCORRECT_DATA_NOTIFICATION;
				_isColorsChanged = true;
				IsLoginIncorrect = true;

				ret = false;
			}
			if (string.IsNullOrEmpty(Password))
			{
				Notification = Properties.Resources.INCORRECT_DATA_NOTIFICATION;
				_isColorsChanged = true;
				IsPasswordIncorrect = true;

				ret = false;
			}

			return ret;
		}
	}
}
