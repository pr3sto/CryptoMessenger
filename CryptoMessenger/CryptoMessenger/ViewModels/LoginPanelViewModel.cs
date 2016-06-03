using System;
using System.Windows.Input;
using System.Windows.Controls;

using CryptoMessenger.Commands;
using CryptoMessenger.Models;

using MessageProtocol.MessageTypes;
using MessageProtocol;

namespace CryptoMessenger.ViewModels
{
	/// <summary>
	/// View model for login panel (mvvm pattern).
	/// </summary>
    class LoginPanelViewModel : ViewModelBase, IWindowPanel
	{
		private Client client;

		public LoginPanelViewModel(Client client)
        {
			this.client = client;
			Notification = Properties.Resources.WelcomeNotification;
			IsLoading = false;
			IsClosing = false;
		}

		/// <summary>
		/// Fire event when login success.
		/// </summary>
		public event Action LoginSuccess;

		#region Properties

		// is something loading 
		private bool isLoading;
		public bool IsLoading
		{
			get { return isLoading; }
			set
			{
				isLoading = value;
				OnPropertyChanged(nameof(IsLoading));
			}
		}

		// is login panel closing 
		private bool isClosing;
		public bool IsClosing
		{
			get { return isClosing; }
			set
			{
				isClosing = value;
				OnPropertyChanged(nameof(IsClosing));
			}
		}

		// notification textblock
		private string notification;
		public string Notification
		{
			get { return notification; }
			set
			{
				notification = value;
				OnPropertyChanged(nameof(Notification));
			}
		}

		// user login
		private string login;
		public string Login
		{
			get { return login; }
			set
			{
				login = value;
				OnPropertyChanged(nameof(Login));
			}
		}

		// is user login incorrect
		private bool isLoginIncorrect;
		public bool IsLoginIncorrect
		{
			get { return isLoginIncorrect; }
			set
			{
				isLoginIncorrect = value;
				OnPropertyChanged(nameof(IsLoginIncorrect));
			}
		}

		// is user login correct
		private bool isLoginCorrect;
		public bool IsLoginCorrect
		{
			get { return isLoginCorrect; }
			set
			{
				isLoginCorrect = value;
				OnPropertyChanged(nameof(IsLoginCorrect));
			}
		}

		// is user password incorrect
		private bool isPasswordIncorrect;
		public bool IsPasswordIncorrect
		{
			get { return isPasswordIncorrect; }
			set
			{
				isPasswordIncorrect = value;
				OnPropertyChanged(nameof(IsPasswordIncorrect));
			}
		}

		// is user password correct
		private bool isPasswordCorrect;
		public bool IsPasswordCorrect
		{
			get { return isPasswordCorrect; }
			set
			{
				isPasswordCorrect = value;
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
			string password = ((PasswordBox)passwordBox).Password;

			SetDefaultColors();

			if (IsUserDataCorrect(password))
			{
				IsLoading = true;
				
				Notification = Properties.Resources.LoginNotification;

				// try login
				LoginRegisterResponse response;
				try
				{
					response = await client.Login(Login, password);
				}
				catch (ConnectionInterruptedException)
				{
					Notification = Properties.Resources.ConnectionErrorNotification;
					isColorsChanged = true;
					
					IsLoading = false;
					return;
				}
				catch (CertificateException)
				{
					Notification = Properties.Resources.CertificateErrorNotification;
					isColorsChanged = true;

					IsLoading = false;
					return;
				}

				if (LoginRegisterResponse.Success.Equals(response))
				{
					IsClosing = true;
					// wait for animation
					await System.Threading.Tasks.Task.Run(() => System.Threading.Thread.Sleep(1700));

					LoginSuccess?.Invoke();
					Notification = Properties.Resources.WelcomeNotification;
				}
				else if (LoginRegisterResponse.Fail.Equals(response))
				{
					Notification = Properties.Resources.LoginErrorNotification;
					isColorsChanged = true;
					IsLoginIncorrect = true;
					IsPasswordIncorrect = true;
				}
				else if (LoginRegisterResponse.Error.Equals(response))
				{
					Notification = Properties.Resources.UnknownErrorNotification;
					isColorsChanged = true;
				}
				else if (LoginRegisterResponse.AlreadyLogin.Equals(response))
				{
					Notification = Properties.Resources.AlreadyLoginNotification;
					isColorsChanged = true;
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
			string password = ((PasswordBox)passwordBox).Password;
		
			SetDefaultColors();

			if (IsUserDataCorrect(password))
			{
				IsLoading = true;
				
				Notification = Properties.Resources.RegistrationNotification;

				// try register
				LoginRegisterResponse response;
				try
				{
					response = await client.Register(Login, password);
				}
				catch (ConnectionInterruptedException)
				{
					Notification = Properties.Resources.ConnectionErrorNotification;
					isColorsChanged = true;
					
					IsLoading = false;
					return;
				}
				catch (CertificateException)
				{
					Notification = Properties.Resources.CertificateErrorNotification;
					isColorsChanged = true;
					
					IsLoading = false;
					return;
				}

				if (LoginRegisterResponse.Success.Equals(response))
				{
					Notification = Properties.Resources.RegistrationSuccessNotification;
					isColorsChanged = true;
					IsLoginCorrect = true;
					IsPasswordCorrect = true;
				}
				else if (LoginRegisterResponse.Fail.Equals(response))
				{
					Notification = Properties.Resources.RegistrationErrorNotification;
					isColorsChanged = true;
					IsLoginIncorrect = true;
					IsPasswordIncorrect = true;
				}
				else if (LoginRegisterResponse.Error.Equals(response))
				{
					Notification = Properties.Resources.UnknownErrorNotification;
					isColorsChanged = true;
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

		// open hyperlink in browser
		private DelegateCommand openHyperlinkCommand;
		public ICommand OpenHyperlinkCommand
		{
			get
			{
				if (openHyperlinkCommand == null)
				{
					openHyperlinkCommand = new DelegateCommand(delegate
					{
						System.Diagnostics.Process.Start(Properties.Resources.CreditsHyperlink);
					});
				}
				return openHyperlinkCommand;
			}
		}

		#endregion

		// set default colors
		bool isColorsChanged = false;
		private void SetDefaultColors()
		{
			if (isColorsChanged)
			{
				Notification = Properties.Resources.WelcomeNotification;
				isColorsChanged = false;

				IsLoginIncorrect = false;
				IsPasswordIncorrect = false;

				IsLoginCorrect = false;
				IsPasswordCorrect = false;
			}
		}

		// check textboxes
		private bool IsUserDataCorrect(string Password)
		{
			bool isDataCorrect = true;

			// empty login
			if (string.IsNullOrEmpty(Login))
			{
				isColorsChanged = true;
				IsLoginIncorrect = true;
				isDataCorrect = false;
			}
			// empty password
			if (string.IsNullOrEmpty(Password))
			{
				isColorsChanged = true;
				IsPasswordIncorrect = true;
				isDataCorrect = false;
			}

			if (isDataCorrect == false)
			{
				Notification = Properties.Resources.EmptyDataNotification;
				return isDataCorrect;
			}

			// incorrect login
			if (!string.IsNullOrEmpty(Login) &&
				!System.Text.RegularExpressions.Regex.IsMatch(Login, @"^[a-zA-Z0-9]+$"))
			{
				Notification = Properties.Resources.IncorrectDataNotification;
				isColorsChanged = true;
				IsLoginIncorrect = true;
				isDataCorrect = false;
			}

			// incorrect password
			if (!string.IsNullOrEmpty(Password) &&
				!System.Text.RegularExpressions.Regex.IsMatch(Password, @"^[a-zA-Z0-9]+$"))
			{
				Notification = Properties.Resources.IncorrectDataNotification;
				isColorsChanged = true;
				IsPasswordIncorrect = true;
				isDataCorrect = false;
			}
			
			return isDataCorrect;
		}
	}
}
