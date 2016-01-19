using System;
using System.Windows.Forms;

using CryptoMessenger.Net;

namespace CryptoMessenger.GUI
{
    public partial class MainForm : Form
    {
		// cleint 
		private Client client;
		// user name
		private string login;
		// parent form
		private LoginForm loginForm;

        // shadow around window
        private Dropshadow shadow;

        public MainForm(LoginForm parent, Client client, string login)
        {
            InitializeComponent();

			loginForm = parent;
			this.login = login;
			this.client = client;
			// listen for messages from server
			this.client.Listen();

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

		// logout when form closing
		private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				await client.Logout();
			}
			catch
			{
				// dont mind because we close app
			}
		}
	}
}
