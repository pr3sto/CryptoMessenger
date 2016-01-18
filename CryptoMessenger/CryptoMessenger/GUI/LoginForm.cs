using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

using CryptoMessenger.Stuff;
using CryptoMessenger.Net;
using MessageTypes;

namespace CryptoMessenger.GUI
{
    public partial class LoginForm : Form
    {
		// cleint 
		Client client;

		// main form, opens when user login
		private MainForm mainForm;

        // shadow around window
        private Dropshadow shadow;

		// fonts
		private PrivateFontCollection fonts = new PrivateFontCollection();
		public Font NeueFont15;
		public Font NeueFont10;

		// Violates rule: MovePInvokesToNativeMethodsClass
		internal class NativeMethods
		{
			// for textbox
			internal const int EM_SETCUEBANNER = 0x1501;
			[System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
			internal static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam,
				[System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]string lParam);
		}

		public LoginForm()
        {
            InitializeComponent();

			// create fonts
			NeueFont15 = FontFactory.CreateFont(fonts, Properties.Resources.Neue, 15.5F);
			NeueFont10 = FontFactory.CreateFont(fonts, Properties.Resources.Neue, 10.0F);

			// create shadow, set shadow params
			shadow = new Dropshadow(this)
            {
                ShadowBlur = 6,
                ShadowSpread = -3,
                ShadowColor = Properties.Settings.Default.LoginFirstColor
			};
            shadow.RefreshShadow();
            shadow.UpdateLocation();

			client = new Client();
		}


		// login
		private void LoginForm_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == Convert.ToChar(Keys.Return))
				loginButton.PerformClick();
		}
		private async void loginButton_Click(object sender, EventArgs e)
        {
            if (IsUserDataCorrect())
			{
				DisableInterface();

				// try login
				notificationLabel.Text = "";
				notificationLabel.Text = Properties.Resources.LOGIN_NOTIFICATION;
				LoginRegisterResponseMessage response;
				try
				{
					response = await client.Login(userName.Text, userPassword.Text);
				}
				catch (ServerConnectionException)
				{
					notificationLabel.ForeColor = Properties.Settings.Default.AlertColor;
					notificationLabel.Text = Properties.Resources.SERVER_CONNECTION_ERROR_NOTIFICATION;

					EnableInterface();
					return;
				}
				catch (ClientCertificateException)
				{
					notificationLabel.ForeColor = Properties.Settings.Default.AlertColor;
					notificationLabel.Text = Properties.Resources.CERTIFICATE_ERROR_NOTIFICATION;

					EnableInterface();
					return;
				}

				if (LoginRegisterResponse.SUCCESS.Equals(response.response))
				{
					userPassword.Text = null;
					Hide();

					mainForm = new MainForm(this, userName.Text);
					mainForm.ShowDialog();
					mainForm.Close();

					// free memory
					mainForm = null;
					GC.Collect(); // ouch...
					GC.WaitForPendingFinalizers();

					notificationLabel.Text = Properties.Resources.STANDART_NOTIFICATION;
					Show();
				}
				else if (LoginRegisterResponse.FAIL.Equals(response.response))
				{
					notificationLabel.ForeColor = Properties.Settings.Default.AlertColor;
					notificationLabel.Text = Properties.Resources.LOGIN_ERROR_NOTIFICATION;

					namePanelBorderColor = Properties.Settings.Default.AlertColor;
					passPanelBorderColor = Properties.Settings.Default.AlertColor;
				}
				else if (LoginRegisterResponse.ERROR.Equals(response.response))
				{
					notificationLabel.ForeColor = Properties.Settings.Default.AlertColor;
					notificationLabel.Text = Properties.Resources.UNKNOWN_ERROR;

					EnableInterface();
					return;
				}

				EnableInterface();
			}
        }


        // register
        private async void registerButton_Click(object sender, EventArgs e)
        {  
            if (IsUserDataCorrect())
            {
				DisableInterface();

				// try register
				notificationLabel.Text = "";
				notificationLabel.Text = Properties.Resources.REGISTRATION_NOTIFICATION;
				LoginRegisterResponseMessage response;
				try
				{
					response = await client.Register(userName.Text, userPassword.Text);
				}
				catch (ServerConnectionException)
				{
					notificationLabel.ForeColor = Properties.Settings.Default.AlertColor;
					notificationLabel.Text = Properties.Resources.SERVER_CONNECTION_ERROR_NOTIFICATION;

					EnableInterface();
					return;
				}
				catch (ClientCertificateException)
				{
					notificationLabel.ForeColor = Properties.Settings.Default.AlertColor;
					notificationLabel.Text = Properties.Resources.CERTIFICATE_ERROR_NOTIFICATION;

					EnableInterface();
					return;
				}

				if (LoginRegisterResponse.SUCCESS.Equals(response.response))
				{
					notificationLabel.ForeColor = Properties.Settings.Default.SuccessColor;
					notificationLabel.Text = Properties.Resources.REGISTRATION_SUCCESS_NOTIFICATION;

					namePanelBorderColor = Properties.Settings.Default.SuccessColor;
					passPanelBorderColor = Properties.Settings.Default.SuccessColor;
				}
				else if (LoginRegisterResponse.FAIL.Equals(response.response))
				{
					notificationLabel.ForeColor = Properties.Settings.Default.AlertColor;
					notificationLabel.Text = Properties.Resources.REGISTRATION_ERROR_NOTIFICATION;

					namePanelBorderColor = Properties.Settings.Default.AlertColor;
					passPanelBorderColor = Properties.Settings.Default.AlertColor;
				}
				else if (LoginRegisterResponse.ERROR.Equals(response.response))
				{
					notificationLabel.ForeColor = Properties.Settings.Default.AlertColor;
					notificationLabel.Text = Properties.Resources.UNKNOWN_ERROR;

					EnableInterface();
					return;
				}

				EnableInterface();
			}
        }

		// check textfields
		private bool IsUserDataCorrect()
		{
			bool ret = true;

			if (string.IsNullOrEmpty(userName.Text))
			{
				namePanelBorderColor = Properties.Settings.Default.AlertColor;
				userNamePanel.Refresh();

				notificationLabel.ForeColor = Properties.Settings.Default.AlertColor;
				notificationLabel.Text = Properties.Resources.INCORRECT_DATA_NOTIFICATION;

				ret = false;
			}
			if (string.IsNullOrEmpty(userPassword.Text))
			{
				passPanelBorderColor = Properties.Settings.Default.AlertColor;
				userPasswordPanel.Refresh();

				notificationLabel.ForeColor = Properties.Settings.Default.AlertColor;
				notificationLabel.Text = Properties.Resources.INCORRECT_DATA_NOTIFICATION;

				ret = false;
			}

			return ret;
		}

		// disable components when waiting for login/register
		private void DisableInterface()
		{
			loginButton.Enabled = false;
			registerButton.Enabled = false;
			userName.Enabled = false;
			userPassword.Enabled = false;
			showPasswordCheckBox.Enabled = false;
		}

		// enable components after login/register
		private void EnableInterface()
		{
			userNamePanel.Refresh();
			userPasswordPanel.Refresh();
			loginButton.Enabled = true;
			registerButton.Enabled = true;
			userName.Enabled = true;
			userPassword.Enabled = true;
			showPasswordCheckBox.Enabled = true;
			ActiveControl = loginButton;
		}
	}
}
