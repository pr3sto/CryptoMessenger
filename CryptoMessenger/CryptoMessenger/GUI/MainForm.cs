using System;
using System.Windows.Forms;

using Shadow;

namespace CryptoMessenger
{
    public partial class MainForm : Form
    {
		// user name
		private string login;

		// parent form
		LoginForm loginForm;

        // shadow around window
        private Dropshadow shadow;

        public MainForm(LoginForm parent, string login)
        {
            InitializeComponent();

			loginForm = parent;
			this.login = login;

			// create shadow, set shadow params
			shadow = new Dropshadow(this)
            {
                ShadowBlur = 6,
                ShadowSpread = -3,
                ShadowColor = Properties.Settings.Default.MainFirstColor
			};
            shadow.RefreshShadow();
            shadow.UpdateLocation();
        }


		// select friend and change chat
		private void friendsListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			activeTalkTitle.Text = friendsListBox.Text;
		}


		// send message
		private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == Convert.ToChar(Keys.Return) & ModifierKeys != Keys.Shift & message.Focused)
				sendButton.PerformClick();
		}
		private void sendButton_Click(object sender, EventArgs e)
		{
		}
	}
}
