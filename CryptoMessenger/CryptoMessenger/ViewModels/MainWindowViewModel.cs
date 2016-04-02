using System.Windows.Input;

using CryptoMessenger.Commands;
using CryptoMessenger.Models;

namespace CryptoMessenger.ViewModels
{
	/// <summary>
	/// View model for main window (mvvm pattern).
	/// </summary>
	class MainWindowViewModel : ViewModelBase
	{
		private Client client;

		public MainWindowViewModel(Client client, string login)
		{
			this.client = client;
			client.Listen();

			WindowTitle = login;

			RequestsButtonSelected = false;
			FriendsButtonSelected = true;
			SearchButtonSelected = false;

			LeftPanel = new FriendsPanelViewModel(client);
		}

		#region Properties

		// window title
		private string _windowTitle;
		public string WindowTitle
		{
			get { return _windowTitle; }
			set
			{
				_windowTitle = value;
				OnPropertyChanged(nameof(WindowTitle));
			}
		}

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

		// left panel
		private ILeftPanel _leftPanel;
		public ILeftPanel LeftPanel
		{
			get { return _leftPanel; }
			set
			{
				_leftPanel = value;
				OnPropertyChanged(nameof(LeftPanel));
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

			LeftPanel = new RequestsPanelViewModel(client);
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

			LeftPanel = new FriendsPanelViewModel(client);
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

			LeftPanel = new SearchPanelViewModel(client);
		}

		#endregion
	}
}
