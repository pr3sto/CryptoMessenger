using System;
using System.Windows.Forms;

using CryptoMessenger.Net;
using CryptoMessenger.Stuff;

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

		// whitch panel selected now
		private UsersPanels selectedPanel = UsersPanels.FRIENDS;

		// 'cache' 
		// We dont ask server for it if we have it in cache;
		// server sends it to us if something changes.
		private string[] cache_friends= null;
		private string[] cache_income_reqs = null;
		private string[] cache_outcome_reqs = null;

		// conversations of user
		private Conversations conversations = new Conversations();

		// can i send reply or not
		private bool CanSendReply = false;


		public MainForm(LoginForm parent, Client client, string login)
        {
            InitializeComponent();

			loginForm = parent;
			this.login = login;
			this.client = client;

			// create shadow, set shadow params
			shadow = new Dropshadow(this)
            {
                ShadowBlur = 6,
                ShadowSpread = -3,
                ShadowColor = Properties.Settings.Default.MainFirstColor
			};
            shadow.RefreshShadow();
            shadow.UpdateLocation();

			// start listeninig for messages from server
			this.client.Listen(this);
		}

		// logout when form closing
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			client.Logout();
		}

		#region Update users panels

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
				if (cache_income_reqs == null)
					client.GetIncomeFriendshipRequests();
				else
					UpdateIncomeFriendshipRequests(cache_income_reqs);

				// if we dont have it in cache ask server for it
				if (cache_outcome_reqs == null)
					client.GetOutcomeFriendshipRequests();
				else
					UpdateOutcomeFriendshipRequests(cache_outcome_reqs);
			}
		}

		#endregion

		#region Update form components

		// update all users list box
		public void UpdateAllUsersList(string[] users)
		{
			if (UsersPanels.SEARCH.Equals(selectedPanel))
			{
				allUsersListBox.Items.Clear();
				if (users != null) allUsersListBox.Items.AddRange(users);
				allUsersListBox.Update();

				AllUsersListBoxSelectedChanged(null, EventArgs.Empty);

				loadingLabel.Visible = false;
				allUsersPanel.Visible = true;
			}
		}

		// update friends list box
		public void UpdateFriendsList(string[] friends)
		{
			cache_friends = friends;

			if (UsersPanels.FRIENDS.Equals(selectedPanel))
			{
				friendsListBox.Items.Clear();
				if (friends != null) friendsListBox.Items.AddRange(friends);
				friendsListBox.Update();

				FriendsListBoxSelectedChanged(null, EventArgs.Empty);

				loadingLabel.Visible = false;
				friendsPanel.Visible = true;
			}
		}

		// update income friendship requests list box
		public void UpdateIncomeFriendshipRequests(string[] income)
		{
			cache_income_reqs = income;

			if (UsersPanels.REQUESTS.Equals(selectedPanel))
			{
				incomeFriendshipRequestsListBox.Items.Clear();
				if (income != null) incomeFriendshipRequestsListBox.Items.AddRange(income);
				incomeFriendshipRequestsListBox.Update();

				loadingLabel.Visible = false;
				friendshipRequestsPanel.Visible = true;
			}
		}

		// update outcome friendship requests list box
		public void UpdateOutcomeFriendshipRequests(string[] outcome)
		{
			cache_outcome_reqs = outcome;

			if (UsersPanels.REQUESTS.Equals(selectedPanel))
			{
				outcomeFriendshipRequestsListBox.Items.Clear();
				if (outcome != null) outcomeFriendshipRequestsListBox.Items.AddRange(outcome);
				outcomeFriendshipRequestsListBox.Update();

				OutcomeRequestsListBoxesSelectedChanged(null, EventArgs.Empty);

				loadingLabel.Visible = false;
				friendshipRequestsPanel.Visible = true;
			}
		}

		// add reply and show conversation if it selected
		public void UpdateConversations(string interlocutor, ConversationReply reply)
		{
			conversations.AddReply(interlocutor, reply);
			ShowConversation(interlocutor);
		}

		#endregion

		#region Users panels buttons

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

		#endregion

		#region Conversation actions

		// show all replies with interlocutor
		public void ShowConversation(string interlocutor)
		{
			if (!string.IsNullOrEmpty(interlocutor) && interlocutor.Equals(activeTalkLabel.Text))
			{
				Conversation conversation = conversations.GetConversation(interlocutor);

				if (conversation != null)
				{
					conversationHtmlPanel.Text = "";

					foreach (var reply in conversation.GetArrayOfReplies())
					{
						conversationHtmlPanel.Text += "<html><b>" + reply.author +
							"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + reply.time +
							"</b><br>" + reply.text + "</html>";
					}
				}
			}
		}

		// get conversation from server if we dont have it
		private void GetOrShowConversation(string interlocutor)
		{
			if (!conversations.Contains(interlocutor))
			{
				conversations.AddConversation(new Conversation(interlocutor));
				client.GetConversation(interlocutor);
			}
			else
				ShowConversation(interlocutor);
		}

		// send reply
		private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == Convert.ToChar(Keys.Return) & ModifierKeys != Keys.Shift & replyTextfield.Focused)
				sendReplyButton.PerformClick();
		}
		private void sendReplyButton_Click(object sender, EventArgs e)
		{
			if (CanSendReply)
				client.SendReply(activeTalkLabel.Text, replyTextfield.Text);
		}

		#endregion

		#region Close form emergency 

		public void CloseEmergency()
		{
			Close();
		}

		#endregion
	}
}
