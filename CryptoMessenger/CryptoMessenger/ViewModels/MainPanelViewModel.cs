using System;
using System.Windows.Input;

using CryptoMessenger.Commands;
using CryptoMessenger.Models;

namespace CryptoMessenger.ViewModels
{
	/// <summary>
	/// View model for main panel (mvvm pattern).
	/// </summary>
	class MainPanelViewModel : ViewModelBase, IWindowPanel
	{
		private Client client;

		public MainPanelViewModel(Client client)
		{
			this.client = client;
			client.Listen();

			Login = client.Name;
			IsOnline = true;

			client.ConnectionBreaks += delegate { IsOnline = false; };

			RequestsButtonSelected = false;
			FriendsButtonSelected = true;
			SearchButtonSelected = false;

			WindowPanel = new FriendsPanelViewModel(client);
		}

		/// <summary>
		/// Fire event when logout.
		/// </summary>
		public event Action Logout;

		#region Properties

		// account login
		private string login;
		public string Login
		{
			get { return login; }
			set
			{
				login = value;
				OnPropertyChanged(nameof(Login));
			}
		}

		// account status
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

		// requests button selected
		private bool requestsButtonSelected;
		public bool RequestsButtonSelected
		{
			get { return requestsButtonSelected; }
			set
			{
				requestsButtonSelected = value;
				OnPropertyChanged(nameof(RequestsButtonSelected));
			}
		}

		// friends button selected
		private bool friendsButtonSelected;
		public bool FriendsButtonSelected
		{
			get { return friendsButtonSelected; }
			set
			{
				friendsButtonSelected = value;
				OnPropertyChanged(nameof(FriendsButtonSelected));
			}
		}

		// search button selected
		private bool _searchButtonSelected;
		public bool SearchButtonSelected
		{
			get { return _searchButtonSelected; }
			set
			{
				_searchButtonSelected = value;
				OnPropertyChanged(nameof(SearchButtonSelected));
			}
		}

		// panel
		private IWindowPanel windowPanel;
		public IWindowPanel WindowPanel
		{
			get { return windowPanel; }
			set
			{
				windowPanel = value;
				OnPropertyChanged(nameof(WindowPanel));
			}
		}
		
		#endregion

		#region Commands

		// requests textblock clicked
		private DelegateCommand requestsTextBlockClickedCommand;
		public ICommand RequestsTextBlockClickedCommand
		{
			get
			{
				if (requestsTextBlockClickedCommand == null)
				{
					requestsTextBlockClickedCommand = new DelegateCommand(SwitchToRequests);
				}
				return requestsTextBlockClickedCommand;
			}
		}
		private void SwitchToRequests()
		{
			RequestsButtonSelected = true;
			FriendsButtonSelected = false;
			SearchButtonSelected = false;

			WindowPanel = new RequestsPanelViewModel(client);
		}

		// requests textblock clicked
		private DelegateCommand friendsTextBlockClickedCommand;
		public ICommand FriendsTextBlockClickedCommand
		{
			get
			{
				if (friendsTextBlockClickedCommand == null)
				{
					friendsTextBlockClickedCommand = new DelegateCommand(SwitchToFriends);
				}
				return friendsTextBlockClickedCommand;
			}
		}
		private void SwitchToFriends()
		{
			RequestsButtonSelected = false;
			FriendsButtonSelected = true;
			SearchButtonSelected = false;

			WindowPanel = new FriendsPanelViewModel(client);
		}

		// requests textblock clicked
		private DelegateCommand searchTextBlockClickedCommand;
		public ICommand SearchTextBlockClickedCommand
		{
			get
			{
				if (searchTextBlockClickedCommand == null)
				{
					searchTextBlockClickedCommand = new DelegateCommand(SwitchToSearch);
				}
				return searchTextBlockClickedCommand;
			}
		}
		private void SwitchToSearch()
		{
			RequestsButtonSelected = false;
			FriendsButtonSelected = false;
			SearchButtonSelected = true;

			WindowPanel = new SearchPanelViewModel(client);
		}

		// logout
		private DelegateCommand logoutCommand;
		public ICommand LogoutCommand
		{
			get
			{
				if (logoutCommand == null)
				{
					logoutCommand = new DelegateCommand(delegate
					{
						client.Logout();
						Logout();
					});
				}
				return logoutCommand;
			}
		}

		#endregion
	}
}
