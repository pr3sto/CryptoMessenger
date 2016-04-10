using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

using CryptoMessenger.Commands;
using CryptoMessenger.Models;

namespace CryptoMessenger.ViewModels
{
	/// <summary>
	/// User for ItemSource of ListBox.
	/// </summary>
	class User
	{
		private Client client;

		public User(Client client)
		{
			this.client = client;
		}

		// user's name.
		public string Name { get; set; }

		// add to friends
		private DelegateCommand addToFriendsCommand;
		public ICommand AddToFriendsCommand
		{
			get
			{
				if (addToFriendsCommand == null)
				{
					addToFriendsCommand = new DelegateCommand(AddToFriends);
				}
				return addToFriendsCommand;
			}
		}
		private void AddToFriends()
		{
			client.SendFriendshipRequest(Name);
		}
	}

	/// <summary>
	/// View model for search panel (mvvm pattern).
	/// </summary>
	class SearchPanelViewModel : ViewModelBase, IMainWindowPanel
	{
		private Client client;

		public SearchPanelViewModel(Client client)
		{
			this.client = client;
			client.PropertyChanged += UsersListChanged;

			UsersList = null;

			// get users when panel loads
			client.GetAllUsers();
		}

		// update UsersList when property in client changed
		private void UsersListChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(client.SearchUsersList) && client.SearchUsersList != null)
			{
				List<User> users = new List<User>();
				foreach (var s in client.SearchUsersList)
				{
					User u = new User(client);
					u.Name = s;
					users.Add(u);
				}
				UsersList = users.ToArray();
			}
		}

		#region Properties

		// users list
		private User[] _usersList;
		public User[] UsersList
		{
			get { return _usersList; }
			set
			{
				_usersList = value;
				OnPropertyChanged(nameof(UsersList));
			}
		}

		#endregion
	}
}
