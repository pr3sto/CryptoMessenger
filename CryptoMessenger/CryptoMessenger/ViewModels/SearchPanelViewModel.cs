using System.Collections.ObjectModel;
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

		public User(Client client, string name)
		{
			this.client = client;
			Name = name;
		}

		// user's name.
		public string Name { get; }

		// add to friends
		private DelegateCommand addToFriendsCommand;
		public ICommand AddToFriendsCommand
		{
			get
			{
				if (addToFriendsCommand == null)
				{
					addToFriendsCommand = new DelegateCommand(delegate
					{ client.SendFriendshipRequest(Name); });
				}
				return addToFriendsCommand;
			}
		}
	}

	/// <summary>
	/// View model for search panel (mvvm pattern).
	/// </summary>
	class SearchPanelViewModel : ViewModelBase, IWindowPanel
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
				UsersList = new ObservableCollection<User>();
				foreach (var name in client.SearchUsersList)
					UsersList.Add(new User(client, name));
			}
		}

		#region Properties

		// users list
		private ObservableCollection<User> usersList;
		public ObservableCollection<User> UsersList
		{
			get { return usersList; }
			set
			{
				usersList = value;
				OnPropertyChanged(nameof(UsersList));
			}
		}

		#endregion
	}
}
