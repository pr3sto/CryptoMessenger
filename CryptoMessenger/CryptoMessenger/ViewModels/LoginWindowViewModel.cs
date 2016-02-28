using System;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;

using CryptoMessenger.Commands;
using CryptoMessenger.Models;

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

			NotificationBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
			Notification = Properties.Resources.STANDART_NOTIFICATION;
			
			IsInterfaceEnabled = true;
		}

		#region Properties

		// cursor
		private Cursor _windowCursor;
		public Cursor WindowCursor
		{
			get { return _windowCursor; }
			set
			{
				_windowCursor = value;
				OnPropertyChanged("WindowCursor");
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
				OnPropertyChanged("Notification");
			}
		}

		// notification textblock brush
		private SolidColorBrush _notificationBrush;
		public SolidColorBrush NotificationBrush
		{
			get { return _notificationBrush; }
			set
			{
				_notificationBrush = value;
				OnPropertyChanged("NotificationBrush");
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
				OnPropertyChanged("Login");
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
				OnPropertyChanged("IsLoginIncorrect");
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
				OnPropertyChanged("IsLoginCorrect");
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
				OnPropertyChanged("IsPasswordIncorrect");
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
				OnPropertyChanged("IsPasswordCorrect");
			}
		}

		// is interface enabled
		private bool _isInterfaceEnabled;
		public bool IsInterfaceEnabled
		{
			get { return _isInterfaceEnabled; }
			set
			{
				_isInterfaceEnabled = value;
				OnPropertyChanged("IsInterfaceEnabled");
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
				IsInterfaceEnabled = false;
				WindowCursor = Cursors.Wait;
				
				Notification = Properties.Resources.LOGIN_NOTIFICATION;

				// try login
				LoginRegisterResponse response;
				try
				{
					response = await client.Login(Login, Password);
				}
				catch (ConnectionInterruptedException)
				{
					NotificationBrush = new SolidColorBrush(Properties.Settings.Default.AlertColor);
					Notification = Properties.Resources.SERVER_CONNECTION_ERROR_NOTIFICATION;
					_isColorsChanged = true;

					IsInterfaceEnabled = true;
					WindowCursor = null;
					return;
				}
				catch (CertificateException)
				{
					NotificationBrush = new SolidColorBrush(Properties.Settings.Default.AlertColor);
					Notification = Properties.Resources.CERTIFICATE_ERROR_NOTIFICATION;
					_isColorsChanged = true;

					IsInterfaceEnabled = true;
					WindowCursor = null;
					return;
				}

				if (LoginRegisterResponse.SUCCESS.Equals(response))
				{
					//Hide();

					//mainForm = new MainForm(this, client, userNameTextBox.Text);
					//mainForm.ShowDialog();
					//mainForm.Close();

					// free memory
					//mainForm = null;
					GC.Collect(); // ouch...
					GC.WaitForPendingFinalizers();

					Notification = Properties.Resources.STANDART_NOTIFICATION;
					//Show();
				}
				else if (LoginRegisterResponse.FAIL.Equals(response))
				{
					NotificationBrush = new SolidColorBrush(Properties.Settings.Default.AlertColor);
					Notification = Properties.Resources.LOGIN_ERROR_NOTIFICATION;
					_isColorsChanged = true;
					IsLoginIncorrect = true;
					IsPasswordIncorrect = true;
				}
				else if (LoginRegisterResponse.ERROR.Equals(response))
				{
					NotificationBrush = new SolidColorBrush(Properties.Settings.Default.AlertColor);
					Notification = Properties.Resources.UNKNOWN_ERROR;
					_isColorsChanged = true;
				}
				else if (LoginRegisterResponse.ALREADY_LOGIN.Equals(response))
				{
					NotificationBrush = new SolidColorBrush(Properties.Settings.Default.AlertColor);
					Notification = Properties.Resources.ALREADY_LOGIN_NOTIFICATION;
					_isColorsChanged = true;
				}

				IsInterfaceEnabled = true;
				WindowCursor = null;
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
				IsInterfaceEnabled = false;
				WindowCursor = Cursors.Wait;
				
				Notification = Properties.Resources.REGISTRATION_NOTIFICATION;

				// try register
				LoginRegisterResponse response;
				try
				{
					response = await client.Register(Login, Password);
				}
				catch (ConnectionInterruptedException)
				{
					NotificationBrush = new SolidColorBrush(Properties.Settings.Default.AlertColor);
					Notification = Properties.Resources.SERVER_CONNECTION_ERROR_NOTIFICATION;
					_isColorsChanged = true;

					IsInterfaceEnabled = true;
					WindowCursor = null;
					return;
				}
				catch (CertificateException)
				{
					NotificationBrush = new SolidColorBrush(Properties.Settings.Default.AlertColor);
					Notification = Properties.Resources.CERTIFICATE_ERROR_NOTIFICATION;
					_isColorsChanged = true;

					IsInterfaceEnabled = true;
					WindowCursor = null;
					return;
				}

				if (LoginRegisterResponse.SUCCESS.Equals(response))
				{
					NotificationBrush = new SolidColorBrush(Properties.Settings.Default.SuccessColor);
					Notification = Properties.Resources.REGISTRATION_SUCCESS_NOTIFICATION;
					_isColorsChanged = true;
					IsLoginCorrect = true;
					IsPasswordCorrect = true;
				}
				else if (LoginRegisterResponse.FAIL.Equals(response))
				{
					NotificationBrush = new SolidColorBrush(Properties.Settings.Default.AlertColor);
					Notification = Properties.Resources.REGISTRATION_ERROR_NOTIFICATION;
					_isColorsChanged = true;
					IsLoginIncorrect = true;
					IsPasswordIncorrect = true;
				}
				else if (LoginRegisterResponse.ERROR.Equals(response))
				{
					NotificationBrush = new SolidColorBrush(Properties.Settings.Default.AlertColor);
					Notification = Properties.Resources.UNKNOWN_ERROR;
					_isColorsChanged = true;
				}

				IsInterfaceEnabled = true;
				WindowCursor = null;
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
				NotificationBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
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
				NotificationBrush = new SolidColorBrush(Properties.Settings.Default.AlertColor);
				Notification = Properties.Resources.INCORRECT_DATA_NOTIFICATION;
				_isColorsChanged = true;
				IsLoginIncorrect = true;

				ret = false;
			}
			if (string.IsNullOrEmpty(Password))
			{
				NotificationBrush = new SolidColorBrush(Properties.Settings.Default.AlertColor);
				Notification = Properties.Resources.INCORRECT_DATA_NOTIFICATION;
				_isColorsChanged = true;
				IsPasswordIncorrect = true;

				ret = false;
			}

			return ret;
		}
	}
}
