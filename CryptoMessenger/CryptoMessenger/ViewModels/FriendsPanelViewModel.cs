using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

using CryptoMessenger.Commands;
using CryptoMessenger.Models;

namespace CryptoMessenger.ViewModels
{
	/// <summary>
	/// Friend for ItemSource of ListBox.
	/// </summary>
	class Friend
	{
		private Client client;

		public Friend(Client client, string name)
		{
			this.client = client;
			Name = name;
		}		

		// friend's name.
		public string Name { get; }

		// remove friend
		private DelegateCommand removeFriendCommand;
		public ICommand RemoveFriendCommand
		{
			get
			{
				if (removeFriendCommand == null)
				{
					removeFriendCommand = new DelegateCommand(RemoveFriend);
				}
				return removeFriendCommand;
			}
		}
		private void RemoveFriend()
		{
			client.RemoveFriend(Name);
		}
	}

	/// <summary>
	/// View model for friends panel (mvvm pattern).
	/// </summary>
	class FriendsPanelViewModel : ViewModelBase, IMainWindowPanel
	{
		private Client client;

		public FriendsPanelViewModel(Client client)
		{
			this.client = client;
			client.PropertyChanged += FriendsListChanged;
			client.ReplyComes += NewMessage;

			FriendsList = null;
			RepliesList = null;

			IsFriendSelected = false;

			// get friends when panel loads
			client.GetFriends();
		}

		// update FriendsList when property in client changed
		private void FriendsListChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(client.FriendsList) && client.FriendsList != null)
			{
				List<Friend> friends = new List<Friend>();
				foreach (var name in client.FriendsList)
					friends.Add(new Friend(client, name));
				
				FriendsList = friends.ToArray();
			}
		}

		// update conversations
		private void NewMessage(object sender, string login)
		{
			if (SelectedFriend != null  && login == SelectedFriend.Name)
				RepliesList = client.Conversations.GetConversation(SelectedFriend.Name)?.ToArrayOfReplies();
		}

		#region Properties

		// friends list
		private Friend[] _friendsList;
		public Friend[] FriendsList
		{
			get { return _friendsList; }
			set
			{
				_friendsList = value;
				OnPropertyChanged(nameof(FriendsList));
			}
		}

		// replies list
		private ConversationReply[] _repliesList;
		public ConversationReply[] RepliesList
		{
			get { return _repliesList; }
			set
			{
				_repliesList = value;
				OnPropertyChanged(nameof(RepliesList));
			}
		}

		// message text
		private string _messageText;
		public string MessageText
		{
			get { return _messageText; }
			set
			{
				_messageText = value;
				OnPropertyChanged(nameof(MessageText));
			}
		}

		// selected friend
		private Friend _selectedFriend;
		public Friend SelectedFriend
		{
			get { return _selectedFriend; }
			set
			{
				_selectedFriend = value;
				IsFriendSelected = (_selectedFriend != null);

				if (_selectedFriend != null)
				{
					if (!client.Conversations.Contains(_selectedFriend.Name))
					{
						client.GetConversation(_selectedFriend.Name);
						RepliesList = null;
					}
					else
						RepliesList = client.Conversations.GetConversation(_selectedFriend.Name)?.ToArrayOfReplies();
				}

				OnPropertyChanged(nameof(SelectedFriend));
			}
		}

		// is dialg visible
		private bool _isDialogVisible;
		public bool IsFriendSelected
		{
			get { return _isDialogVisible; }
			set
			{
				_isDialogVisible = value;
				OnPropertyChanged(nameof(IsFriendSelected));
			}
		}

		#endregion

		#region Commands

		// send message
		private DelegateCommand sendCommand;
		public ICommand SendCommand
		{
			get
			{
				if (sendCommand == null)
				{
					sendCommand = new DelegateCommand(DoSend);
				}
				return sendCommand;
			}
		}
		private void DoSend()
		{
			if (SelectedFriend != null)
			{
				client.SendReply(SelectedFriend.Name, MessageText);
				MessageText = null;
			}
		}

		#endregion
	}
}
