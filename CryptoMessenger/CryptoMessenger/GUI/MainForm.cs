﻿using System;
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

		// 'cache' 
		// We dont ask server for it if we have in cache;
		// server sends it to us if something changes.
		public string[] cache_friends = null;
		public string[] cache_income_reqs = null;
		public string[] cache_outcome_reqs = null;

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

		// send request to server to update listboxes
		private void UpdateUserPanel(object sender, EventArgs e)
		{
			Label label = (Label)sender;

			if ("friendsLabel".Equals(label.Name))
			{
				allUsersPanel.Visible = false;
				friendsPanel.Visible = false;
				friendshipRequestsPanel.Visible = false;

				loadingLabel.Visible = true;

				// if we dont have it in cache ask server for it
				if (cache_friends == null)
					client.GetFriends();
				else
					UpdateFriendsList(cache_friends);
			}
			else if ("searchLabel".Equals(label.Name))
			{
				allUsersPanel.Visible = false;
				friendsPanel.Visible = false;
				friendshipRequestsPanel.Visible = false;

				loadingLabel.Visible = true;

				client.GetAllUsers();
			}
			else if ("friendshipRequestsLabel".Equals(label.Name))
			{
				allUsersPanel.Visible = false;
				friendsPanel.Visible = false;
				friendshipRequestsPanel.Visible = false;

				loadingLabel.Visible = true;

				// if we dont have it in cache ask server for it
				if (cache_income_reqs == null && cache_outcome_reqs == null)
					client.GetFriendshipRequests();
				else
					UpdateFriendshipRequests(cache_income_reqs, cache_outcome_reqs);
				
			}
		}

		// send request for friendship
		private void addFriendButton_Click(object sender, EventArgs e)
		{
			if (allUsersListBox.SelectedItem != null)
			{
				addFriendButton.Enabled = false;
				addFriendButton.Text = "ЗАЯВКА ОТПРАВЛЕНА";
				client.SendFriendshipRequest(allUsersListBox.SelectedItem.ToString());
			}
		}

		// remove user from friends
		private void removeFriendButton_Click(object sender, EventArgs e)
		{
			if (friendsListBox.SelectedItem != null)
			{
				removeFriendButton.Enabled = false;
				removeFriendButton.Text = "ПОЛЬЗОВАТЕЛЬ УДАЛЕН";
				client.RemoveFriend(friendsListBox.SelectedItem.ToString());
			}
		}

		// cancel friendship request
		private void cancelFriendshipRequestButton_Click(object sender, EventArgs e)
		{
			if (outcomeFriendshipRequestsListBox.SelectedItem != null)
			{
				cancelFriendshipRequestButton.Enabled = false;
				cancelFriendshipRequestButton.Text = "ЗАЯВКА ОТМЕНЕНА";
				client.CancelFriendshipRequest(outcomeFriendshipRequestsListBox.SelectedItem.ToString());
			}
		}

		// accept friendship
		private void acceptFriendshipButton_Click(object sender, EventArgs e)
		{
			if (incomeFriendshipRequestsListBox.SelectedItem != null)
			{
				acceptFriendshipButton.Visible = false;
				rejectFriendshipButton.Visible = false;

				cancelFriendshipRequestButton.Visible = true;
				cancelFriendshipRequestButton.Enabled = false;
				cancelFriendshipRequestButton.Text = "ЗАЯВКА ПРИНЯТА";

				client.AcceptFriendshipRequest(incomeFriendshipRequestsListBox.SelectedItem.ToString());
			}
		}
		
		// reject friendship
		private void rejectFriendshipButton_Click(object sender, EventArgs e)
		{
			if (incomeFriendshipRequestsListBox.SelectedItem != null)
			{
				acceptFriendshipButton.Visible = false;
				rejectFriendshipButton.Visible = false;

				cancelFriendshipRequestButton.Visible = true;
				cancelFriendshipRequestButton.Enabled = false;
				cancelFriendshipRequestButton.Text = "ЗАЯВКА ОТКЛОНЕНА";

				client.RejectFriendshipRequest(incomeFriendshipRequestsListBox.SelectedItem.ToString());
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
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			client.Logout();
		}
	}
}
