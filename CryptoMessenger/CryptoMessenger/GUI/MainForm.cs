using System;
using System.Windows.Forms;

using CryptoMessenger.Net;

namespace CryptoMessenger.GUI
{
	public enum UsersPanels { REQUESTS, FRIENDS, SEARCH }; 

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
		// selected panel
		private UsersPanels selectedPanel;

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

			selectedPanel = UsersPanels.FRIENDS;
        }

		// update all users list box
		public void UpdateAllUsersList(string[] users)
		{
			if (UsersPanels.SEARCH.Equals(selectedPanel))
			{
				allUsersListBox.Items.Clear();
				if (users != null) allUsersListBox.Items.AddRange(users);
				allUsersListBox.Update();

				loadingLabel.Visible = false;
				allUsersPanel.Visible = true;
			}
		}

		// update friends list box
		public void UpdateFriendsList(string[] friends)
		{
			if (UsersPanels.FRIENDS.Equals(selectedPanel))
			{
				friendsListBox.Items.Clear();
				if (friends != null) friendsListBox.Items.AddRange(friends);
				friendsListBox.Update();

				loadingLabel.Visible = false;
				friendsPanel.Visible = true;
			}
		}

		// update friendship requests list box
		public void UpdateFriendshipRequests(string[] income, string[] outcome)
		{
			if (UsersPanels.REQUESTS.Equals(selectedPanel))
			{
				incomeFriendshipRequestsListBox.Items.Clear();
				if (income != null) incomeFriendshipRequestsListBox.Items.AddRange(income);
				incomeFriendshipRequestsListBox.Update();

				outcomeFriendshipRequestsListBox.Items.Clear();
				if (outcome != null) outcomeFriendshipRequestsListBox.Items.AddRange(outcome);
				outcomeFriendshipRequestsListBox.Update();

				loadingLabel.Visible = false;
				friendshipRequestsPanel.Visible = true;
			}
		}

		// send request to server to update info
		private async void UpdateUserPanel(object sender, EventArgs e)
		{
			Label label = (Label)sender;

			if ("friendsLabel".Equals(label.Name))
			{
				allUsersPanel.Visible = false;
				friendsPanel.Visible = false;
				friendshipRequestsPanel.Visible = false;

				loadingLabel.Visible = true;

				await client.GetFriends();
			}
			else if ("searchLabel".Equals(label.Name))
			{
				allUsersPanel.Visible = false;
				friendsPanel.Visible = false;
				friendshipRequestsPanel.Visible = false;

				loadingLabel.Visible = true;

				await client.GetAllUsers();
			}
			else if ("friendshipRequestsLabel".Equals(label.Name))
			{
				allUsersPanel.Visible = false;
				friendsPanel.Visible = false;
				friendshipRequestsPanel.Visible = false;

				loadingLabel.Visible = true;

				await client.GetFriendshipRequests();
			}
		}

		// remove user from friends
		private async void removeFriendButton_Click(object sender, EventArgs e)
		{
			if (friendsListBox.SelectedItem != null)
			{
				await client.RemoveFriend(friendsListBox.SelectedItem.ToString());
			}
		}

		// send request for friendship
		private async void addFriendButton_Click(object sender, EventArgs e)
		{
			if (allUsersListBox.SelectedItem != null)
			{
				await client.SendFriendshipRequest(allUsersListBox.SelectedItem.ToString());
			}
		}
		
		// switch between income and outcome requests
		private void incomeRequestsListBox_Enter(object sender, EventArgs e)
		{
			outcomeFriendshipRequestsListBox.ClearSelected();
			cancelFriendshipRequestButton.Visible = false;
			acceptFriendshipButton.Visible = true;
			rejectFriendshipButton.Visible = true;
		}

		// switch between income and outcome requests
		private void outcomeRequestsListBox_Enter(object sender, EventArgs e)
		{
			incomeFriendshipRequestsListBox.ClearSelected();
			cancelFriendshipRequestButton.Visible = true;
			acceptFriendshipButton.Visible = false;
			rejectFriendshipButton.Visible = false;
		}

		// cancel friendship request
		private async void cancelFriendshipRequestButton_Click(object sender, EventArgs e)
		{
			if (outcomeFriendshipRequestsListBox.SelectedItem != null)
			{
				await client.CancelFriendshipRequest(outcomeFriendshipRequestsListBox.SelectedItem.ToString());
			}
		}

		// accept friendship
		private async void acceptFriendshipButton_Click(object sender, EventArgs e)
		{
			if (incomeFriendshipRequestsListBox.SelectedItem != null)
			{
				await client.AcceptFriendshipRequest(incomeFriendshipRequestsListBox.SelectedItem.ToString());
			}
		}
		
		// reject friendship
		private async void rejectFriendshipButton_Click(object sender, EventArgs e)
		{
			if (incomeFriendshipRequestsListBox.SelectedItem != null)
			{
				await client.RejectFriendshipRequest(incomeFriendshipRequestsListBox.SelectedItem.ToString());
			}
		}

		// select friend and change chat
		private void friendsListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			activeTalkLabel.Text = friendsListBox.Text;
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
			await client.Logout();
		}
	}
}
