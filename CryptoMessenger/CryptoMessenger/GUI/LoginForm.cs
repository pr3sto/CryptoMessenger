using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

using CryptoMessenger.Stuff;
using CryptoMessenger.ClientServerCommunication;

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
            if (userName.Text.Equals(""))
            {
				namePanelBorderColor = Properties.Settings.Default.AlertColor;
				userNamePanel.Refresh();

				notificationLabel.ForeColor = Properties.Settings.Default.AlertColor;
				notificationLabel.Text = "НЕКОРРЕКТНЫЕ ДАННЫЕ";
			}
            if (userPassword.Text.Equals(""))
            {
				passPanelBorderColor = Properties.Settings.Default.AlertColor;
				userPasswordPanel.Refresh();

				notificationLabel.ForeColor = Properties.Settings.Default.AlertColor;
				notificationLabel.Text = "НЕКОРРЕКТНЫЕ ДАННЫЕ";
			}
            if (!userName.Text.Equals("") && !userPassword.Text.Equals(""))
            {
				loginButton.Enabled = false;
				registerButton.Enabled = false;
				userName.Enabled = false;
				userPassword.Enabled = false;
				showPasswordCheckBox.Enabled = false;

				// try login
				notificationLabel.Text = "ВХОД...";
				bool success;
				try
				{
					success = await client.Login(userName.Text, userPassword.Text);
				}
				catch (ServerConnectionException)
				{
					notificationLabel.ForeColor = Properties.Settings.Default.AlertColor;
					notificationLabel.Text = "ОШИБКА СОЕДИНЕНИЯ С СЕРВЕРОМ";

					loginButton.Enabled = true;
					registerButton.Enabled = true;
					userName.Enabled = true;
					userPassword.Enabled = true;
					showPasswordCheckBox.Enabled = true;
					ActiveControl = loginButton;

					return;
				}

				if (success)
				{
					userPassword.Text = null;
					Hide();

					mainForm = new MainForm(this, userName.Text);
					mainForm.ShowDialog();
					mainForm.Close();

					// free memory
					mainForm = null;
					GC.Collect(); // ouch
					GC.WaitForPendingFinalizers();

					notificationLabel.Text = "ПОЖАЛУЙСТА, ВОЙДИТЕ ИЛИ ЗАРЕГИСТРИРУЙТЕСЬ";
					Show();
				}
				else
				{
					notificationLabel.ForeColor = Properties.Settings.Default.AlertColor;
					notificationLabel.Text = "НЕ УДАЕТСЯ ВОЙТИ. ПРОВЕРЬТЕ ПРАВИЛЬНОСТЬ ДАННЫХ";

					namePanelBorderColor = Properties.Settings.Default.AlertColor;
					passPanelBorderColor = Properties.Settings.Default.AlertColor;
				}

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


        // register
        private async void registerButton_Click(object sender, EventArgs e)
        {
            if (userName.Text.Equals(""))
            {
                namePanelBorderColor = Properties.Settings.Default.AlertColor;
                userNamePanel.Refresh();

				notificationLabel.ForeColor = Properties.Settings.Default.AlertColor;
				notificationLabel.Text = "НЕКОРРЕКТНЫЕ ДАННЫЕ";
            }
            if (userPassword.Text.Equals(""))
            {
                passPanelBorderColor = Properties.Settings.Default.AlertColor;
                userPasswordPanel.Refresh();

				notificationLabel.ForeColor = Properties.Settings.Default.AlertColor;
				notificationLabel.Text = "НЕКОРРЕКТНЫЕ ДАННЫЕ";
			}
            if (!userName.Text.Equals("") && !userPassword.Text.Equals(""))
            {
				loginButton.Enabled = false;
				registerButton.Enabled = false;
				userName.Enabled = false;
				userPassword.Enabled = false;
				showPasswordCheckBox.Enabled = false;

				// try register
				notificationLabel.Text = "РЕГИСТРАЦИЯ...";
				bool success;
				try
				{
					success = await client.Register(userName.Text, userPassword.Text);
				}
				catch (ServerConnectionException)
				{
					notificationLabel.ForeColor = Properties.Settings.Default.AlertColor;
					notificationLabel.Text = "ОШИБКА СОЕДИНЕНИЯ С СЕРВЕРОМ";
					
					loginButton.Enabled = true;
					registerButton.Enabled = true;
					userName.Enabled = true;
					userPassword.Enabled = true;
					showPasswordCheckBox.Enabled = true;
					ActiveControl = loginButton;

					return;
				}

				if (success)
				{
					notificationLabel.ForeColor = Properties.Settings.Default.SuccessColor;
					notificationLabel.Text = "ВЫ УСПЕШНО ЗАРЕГИСТРИРОВАЛИСЬ";

					namePanelBorderColor = Properties.Settings.Default.SuccessColor;
					passPanelBorderColor = Properties.Settings.Default.SuccessColor;
				}
				else
				{
					notificationLabel.ForeColor = Properties.Settings.Default.AlertColor;
					notificationLabel.Text = "РЕГИСТРАЦИЯ НЕ УСПЕШНА";

					namePanelBorderColor = Properties.Settings.Default.AlertColor;
					passPanelBorderColor = Properties.Settings.Default.AlertColor;
				}

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
}
