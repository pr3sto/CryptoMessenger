using System;
using System.Collections.Generic;
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
	/// Reply in conversation.
	/// </summary>
	class Reply
	{
		public Reply(ConversationReply r, string myLogin)
		{
			Author = r.Author;
			Time = r.Time;
			Text = r.Text;
			IsMyReply = r.Author == myLogin;
		}

		public string Author { get; set; }
		public DateTime Time { get; set; }
		public string Text { get; set; }
		public bool IsMyReply { get; set; }
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
			RepliesList = new ObservableCollection<Reply>();

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
				RepliesList.Add(new Reply(reply, client.Name));
		}

		// update conversations
		private void OldReply(string interlocutor, ConversationReply reply)
		{
			if (SelectedFriend != null && interlocutor == SelectedFriend.Name)
				RepliesList.Insert(0, new Reply(reply, client.Name));
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
		private ObservableCollection<Reply> _repliesList;
		public ObservableCollection<Reply> RepliesList
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
						RepliesList = new ObservableCollection<Reply>();
					}
					else
					{
						List<ConversationReply> replies_tmp = client.Conversations.GetConversation(_selectedFriend.Name)?.replies;
						List<Reply> replies = new List<Reply>();

						if (replies_tmp != null)
							foreach (var r in replies_tmp)
								replies.Add(new Reply(r, client.Name));
						
						RepliesList = new ObservableCollection<Reply>(replies);
					}
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

		// on press enter
		private DelegateCommand messageEnterCommand;
		public ICommand MessageEnterCommand
		{
			get
			{
				if (messageEnterCommand == null)
				{
					messageEnterCommand = new DelegateCommand(DoSend);
				}
				return messageEnterCommand;
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

		// on press shift + enter
		private DelegateCommand<object> messageShiftEnterCommand;
		public ICommand MessageShiftEnterCommand
		{
			get
			{
				if (messageShiftEnterCommand == null)
				{
					messageShiftEnterCommand = new DelegateCommand<object>((t) =>
					{
						System.Windows.Controls.TextBox textbox = t as System.Windows.Controls.TextBox;
						System.Windows.Clipboard.SetText(Environment.NewLine);
						textbox?.Paste();
					});
				}
				return messageShiftEnterCommand;
			}
		}

		#endregion
	}
}
