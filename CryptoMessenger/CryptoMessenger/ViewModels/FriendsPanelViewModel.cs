using System.Collections.ObjectModel;
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
					removeFriendCommand = new DelegateCommand(() =>
					{ client.RemoveFriend(Name); });
				}
				return removeFriendCommand;
			}
		}
	}

	/// <summary>
	/// View model for friends panel (mvvm pattern).
	/// </summary>
	class FriendsPanelViewModel : ViewModelBase, IWindowPanel
	{
		private Client client;

		public FriendsPanelViewModel(Client client)
		{
			this.client = client;
			client.PropertyChanged += FriendsListChanged;
			client.NewReplyComes += NewReply;
			client.OldReplyComes += OldReply;

			FriendsList = null;
			RepliesList = new ObservableCollection<ConversationReply>();

			IsDialogVisible = false;

			// get friends when panel loads
			client.GetFriends();
		}

		// update FriendsList when property in client changed
		private void FriendsListChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(client.FriendsList) && client.FriendsList != null)
			{
				FriendsList = new ObservableCollection<Friend>();
				foreach (var name in client.FriendsList)
					FriendsList.Add(new Friend(client, name));
				
			}
		}

		// update conversations
		private void NewReply(string interlocutor, ConversationReply reply)
		{
			if (SelectedFriend != null && interlocutor == SelectedFriend.Name)
				RepliesList.Add(reply);
		}

		// update conversations
		private void OldReply(string interlocutor, ConversationReply reply)
		{
			if (SelectedFriend != null && interlocutor == SelectedFriend.Name)
				RepliesList.Insert(0, reply);
		}

		#region Properties

		// friends list
		private ObservableCollection<Friend> _friendsList;
		public ObservableCollection<Friend> FriendsList
		{
			get { return _friendsList; }
			set
			{
				_friendsList = value;
				OnPropertyChanged(nameof(FriendsList));
			}
		}

		// replies list
		private ObservableCollection<ConversationReply> _repliesList;
		public ObservableCollection<ConversationReply> RepliesList
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
				IsDialogVisible = (_selectedFriend != null);

				if (_selectedFriend != null)
				{
					if (!client.Conversations.Contains(_selectedFriend.Name))
					{
						client.GetConversation(_selectedFriend.Name);
						RepliesList = new ObservableCollection<ConversationReply>();
					}
					else
						RepliesList = new ObservableCollection<ConversationReply>(
							client.Conversations.GetConversation(_selectedFriend.Name)?.replies);
				}

				OnPropertyChanged(nameof(SelectedFriend));
			}
		}

		// is dialg visible
		private bool _isDialogVisible;
		public bool IsDialogVisible
		{
			get { return _isDialogVisible; }
			set
			{
				_isDialogVisible = value;
				OnPropertyChanged(nameof(IsDialogVisible));
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
			if (SelectedFriend != null && !string.IsNullOrEmpty(MessageText))
			{
				client.SendReply(SelectedFriend.Name, MessageText);
				MessageText = null;
			}
		}

		#endregion
	}
}
