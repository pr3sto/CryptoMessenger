using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

using Shadow;

namespace CryptoMessenger
{
    public partial class LoginForm : Form
    {		
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
        }


		// login
		private void LoginForm_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == Convert.ToChar(Keys.Return))
				loginButton.PerformClick();
		}
		private void loginButton_Click(object sender, EventArgs e)
        {
            if (userName.Text.Equals(""))
            {
                namePanelBorderColor = Properties.Settings.Default.AlertColor;
                userNamePanel.Refresh();

                incorrectName.ForeColor = Color.Red;
                incorrectName.Text = "ИМЯ НЕ УКАЗАНО";
            }
            if (userPassword.Text.Equals(""))
            {
                passPanelBorderColor = Properties.Settings.Default.AlertColor;
                userPasswordPanel.Refresh();

                incorrectPassword.ForeColor = Color.Red;
                incorrectPassword.Text = "ПАРОЛЬ НЕ УКАЗАН";
            }
            if (!userName.Text.Equals("") && !userPassword.Text.Equals(""))
            {
                Hide();
                userPassword.Text = null;

				mainForm = new MainForm(this, userName.Text);
				mainForm.ShowDialog();
				mainForm.Close();

				// free memory
				mainForm = null;
				GC.Collect(); // ouch
				GC.WaitForPendingFinalizers();

				Show();
            }
        }


        // register
        private void registerButton_Click(object sender, EventArgs e)
        {
            if (userName.Text.Equals(""))
            {
                namePanelBorderColor = Properties.Settings.Default.AlertColor;
                userNamePanel.Refresh();

                incorrectName.ForeColor = Color.Red;
                incorrectName.Text = "ИМЯ НЕ УКАЗАНО";
            }
            if (userPassword.Text.Equals(""))
            {
                passPanelBorderColor = Properties.Settings.Default.AlertColor;
                userPasswordPanel.Refresh();

                incorrectPassword.ForeColor = Color.Red;
                incorrectPassword.Text = "ПАРОЛЬ НЕ УКАЗАН";
            }
            if (!userName.Text.Equals("") && !userPassword.Text.Equals(""))
            {
            }
        }
	}
}
