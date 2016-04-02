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

		public Friend(Client client)
		{
			this.client = client;
		}		

		// friend's name.
		public string Name { get; set; }

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
	class FriendsPanelViewModel : ViewModelBase, ILeftPanel
	{
		private Client client;

		public FriendsPanelViewModel(Client client)
		{
			this.client = client;
			client.PropertyChanged += FriendsListChanged;

			FriendsList = null;

			// get friends when panel loads
			client.GetFriends();
		}

		// update FriendsList when property in client changed
		private void FriendsListChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(client.FriendsList))
			{
				List<Friend> friends = new List<Friend>();
				foreach (var s in client.FriendsList)
				{
					Friend f = new Friend(client);
					f.Name = s;
					friends.Add(f);
				}
				FriendsList = friends.ToArray();
			}
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

		#endregion
	}
}
