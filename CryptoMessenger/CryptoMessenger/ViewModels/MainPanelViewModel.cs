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

			RequestsButtonSelected = false;
			FriendsButtonSelected = true;
			SearchButtonSelected = false;

			WindowPanel = new FriendsPanelViewModel(client);
		}

		#region Properties

		// requests button selected
		private bool _requestsButtonSelected;
		public bool RequestsButtonSelected
		{
			get { return _requestsButtonSelected; }
			set
			{
				_requestsButtonSelected = value;
				OnPropertyChanged(nameof(RequestsButtonSelected));
			}
		}

		// friends button selected
		private bool _friendsButtonSelected;
		public bool FriendsButtonSelected
		{
			get { return _friendsButtonSelected; }
			set
			{
				_friendsButtonSelected = value;
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
		private IWindowPanel _windowPanel;
		public IWindowPanel WindowPanel
		{
			get { return _windowPanel; }
			set
			{
				_windowPanel = value;
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

		#endregion
	}
}
