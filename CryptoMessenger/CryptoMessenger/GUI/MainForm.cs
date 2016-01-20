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
			// start listeninig for messages from server
			this.client.Listen(this);

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

		// update users
		public void UpdateUsersList(string[] users)
		{
			usersListBox.Items.Clear();
			usersListBox.Items.AddRange(users);
			usersListBox.Update();

			loadingLabel.Visible = false;
			usersListBox.Visible = true;
		}

		// show users
		private async void Click_ShowUsers(object sender, EventArgs e)
		{
			Label label = (Label)sender;

			if ("friendsTitle".Equals(label.Name))
			{
			}
			else if ("searchTitle".Equals(label.Name))
			{
				usersListBox.Visible = false;
				loadingLabel.Visible = true;
				await client.GetAllUsers();
			}
		}

		// select friend and change chat
		private void friendsListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (showChat)
			{
				activeTalkTitle.Text = usersListBox.Text;
			}
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
			catch (ServerConnectionException)
			{
				// dont mind because we close app
			}
		}
	}
}
