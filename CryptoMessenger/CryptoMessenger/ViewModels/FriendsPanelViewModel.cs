using System;
using System.Linq;
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
	class Friend : INotifyPropertyChanged
	{
		private Client client;

		// friend's name.
		public string Name { get; }

		// friends list
		private bool isOnline;
		public bool IsOnline
		{
			get { return isOnline; }
			set
			{
				isOnline = value;
				OnPropertyChanged(nameof(IsOnline));
			}
		}

		public Friend(Client client, string name, bool isOnline)
		{
			this.client = client;
			Name = name;
			IsOnline = isOnline;
		}		

		// remove friend
		private DelegateCommand removeFriendCommand;
		public ICommand RemoveFriendCommand
		{
			get
			{
				if (removeFriendCommand == null)
				{
					removeFriendCommand = new DelegateCommand(delegate
					{ client.RemoveFriend(Name); });
				}
				return removeFriendCommand;
			}
		}

		// INotifyPropertyChanged implementation
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	/// <summary>
	/// Reply in conversation.
	/// </summary>
	class Reply
	{
		public Reply(ConversationReply reply, string myLogin)
		{
			Author = reply.Author;
			Time = reply.Time;
			Text = reply.Text;
			IsMyReply = reply.Author == myLogin;
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
			client.FriendGoOnline += FriendGoOnline;
			client.FriendGoOffline += FriendGoOffline;
			client.FriendshipAccepted += FriendshipAccepted;
			client.RemovedFromeFriends += RemovedFromeFriends;

			FriendsList = new ObservableCollection<Friend>();
			RepliesList = new ObservableCollection<Reply>();

			IsDialogVisible = false;

			// get friends when panel loads
			client.GetFriends();
		}

		// update FriendsList when property in client changed
		private void FriendsListChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(client.OfflineFriendsList) && client.OfflineFriendsList != null)
			{
				FriendsList = new ObservableCollection<Friend>();

				foreach (var name in client.OnlineFriendsList)
					FriendsList.Add(new Friend(client, name, true));

				foreach (var name in client.OfflineFriendsList)
					FriendsList.Add(new Friend(client, name, false));
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

		// update friend status
		private void FriendGoOnline(string login)
		{
			if (!string.IsNullOrEmpty(login))
			{
				Friend f = FriendsList.FirstOrDefault(x => x.Name.Equals(login));

				if (f != null)
					f.IsOnline = true;
			}
		}

		// update friend status
		private void FriendGoOffline(string login)
		{
			if (!string.IsNullOrEmpty(login))
			{
				Friend f = FriendsList.FirstOrDefault(x => x.Name.Equals(login));

				if (f != null)
					f.IsOnline = false;
			}
		}

		// your request for friendship accepted
		private async void FriendshipAccepted(string login, DateTime time)
		{
			await System.Threading.Tasks.Task.Run(() => 
			System.Threading.Thread.Sleep(Properties.Settings.Default.ActionDelayMsec));
			if (FriendsList.FirstOrDefault(x => x.Name.Equals(login)) == null)
				FriendsList.Add(new Friend(client, login, true));
		}

		// you have ben removed from friends
		private async void RemovedFromeFriends(string login, DateTime time)
		{
			await System.Threading.Tasks.Task.Run(() => 
			System.Threading.Thread.Sleep(Properties.Settings.Default.ActionDelayMsec));
			FriendsList.Remove(FriendsList.FirstOrDefault(x => x.Name.Equals(login)));
		}

		#region Properties

		// friends list
		private ObservableCollection<Friend> friendsList;
		public ObservableCollection<Friend> FriendsList
		{
			get { return friendsList; }
			set
			{
				friendsList = value;
				OnPropertyChanged(nameof(FriendsList));
			}
		}

		// replies list
		private ObservableCollection<Reply> repliesList;
		public ObservableCollection<Reply> RepliesList
		{
			get { return repliesList; }
			set
			{
				repliesList = value;
				OnPropertyChanged(nameof(RepliesList));
			}
		}

		// message text
		private string messageText;
		public string MessageText
		{
			get { return messageText; }
			set
			{
				messageText = value;
				OnPropertyChanged(nameof(MessageText));
			}
		}

		// selected friend
		private Friend selectedFriend;
		public Friend SelectedFriend
		{
			get { return selectedFriend; }
			set
			{
				selectedFriend = value;
				IsDialogVisible = (selectedFriend != null);

				if (selectedFriend != null)
				{
					if (!client.Conversations.Contains(selectedFriend.Name))
					{
						client.GetConversation(selectedFriend.Name);
						RepliesList = new ObservableCollection<Reply>();
					}
					else
					{
						List<ConversationReply> repliesTmp = client.Conversations.GetConversation(selectedFriend.Name)?.replies;
						List<Reply> replies = new List<Reply>();

						if (repliesTmp != null)
							foreach (var r in repliesTmp)
								replies.Add(new Reply(r, client.Name));
						
						RepliesList = new ObservableCollection<Reply>(replies);
					}
				}

				OnPropertyChanged(nameof(SelectedFriend));
			}
		}

		// is dialg visible
		private bool isDialogVisible;
		public bool IsDialogVisible
		{
			get { return isDialogVisible; }
			set
			{
				isDialogVisible = value;
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
						var oldClipboard = System.Windows.Clipboard.GetText();

						System.Windows.Controls.TextBox textbox = t as System.Windows.Controls.TextBox;
						System.Windows.Clipboard.SetText(Environment.NewLine);
						textbox?.Paste();

						System.Windows.Clipboard.SetText(oldClipboard);
					});
				}
				return messageShiftEnterCommand;
			}
		}

		#endregion
	}
}
