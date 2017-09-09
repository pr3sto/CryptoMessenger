using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

using CryptoMessenger.Commands;
using CryptoMessenger.Models;

namespace CryptoMessenger.ViewModels
{
	/// <summary>
	/// Notification item.
	/// </summary>
	class NotificationItem
	{
		public NotificationItem(string text, DateTime time)
		{
			Text = text;
			Time = time;
		}

		public string Text { get; set; }
		public DateTime Time { get; set; }
	}

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

			Notification = Properties.Resources.NotificationListPlaceholder;
			NotificationList = new ObservableCollection<NotificationItem>();
			IsNotificationOpen = false;
			UnreadNotificationCount = 0;

			Login = client.Name;
			IsOnline = true;

			// client's notification events
			client.FriendshipAccepted += async delegate (string login, DateTime time)
			{
				string notification = login + " " + Properties.Resources.FriendshipAcceptedNotificationText;
				await addNotification(notification, time);
			};

			client.FriendshipRejected += async delegate(string login, DateTime time)
			{
				string notification = login + " " + Properties.Resources.FriendshipRejectedNotificationText;
				await addNotification(notification, time);
			};

			client.NewFriendshipRequest += async delegate(string login, DateTime time)
			{
				string notification = login + " " + Properties.Resources.NewFriendshipRequestNotificationText;
				await addNotification(notification, time);
			};

			client.FriendshipRequestCancelled += async delegate(string login, DateTime time)
			{
				string notification = login + " " + Properties.Resources.FriendshipRequestCancelledNotificationText;
				await addNotification(notification, time);
			};

			client.RemovedFromeFriends += async delegate(string login, DateTime time)
			{
				string notification = login + " " + Properties.Resources.FriendRemovedNotificationText;
				await addNotification(notification, time);
			};

			client.ConnectionBreaks += delegate { IsOnline = false; };

			RequestsButtonSelected = false;
			FriendsButtonSelected = true;
			SearchButtonSelected = false;

			WindowPanel = new FriendsPanelViewModel(client);

			// get notifications when view loads
			client.GetNotifications();
		}

		/// <summary>
		/// Add notification and do other ui stuff.
		/// </summary>
		/// <param name="notification">notification.</param>
		private async Task addNotification(string notification, DateTime time)
		{
			NotificationList.Insert(0, new NotificationItem(notification, time));
			if (!IsNotificationOpen) UnreadNotificationCount++;
			Notification = notification;

			await Task.Run(() =>
			{
				System.Threading.Thread.Sleep(Properties.Settings.Default.HideNotificationDelayMsec);
				Notification = Properties.Resources.NotificationListPlaceholder;
			});
		}

		/// <summary>
		/// Fire event when logout.
		/// </summary>
		public event Action Logout;

		#region Properties

		// notification
		private string notification;
		public string Notification
		{
			get { return notification; }
			set
			{
				notification = value;
				OnPropertyChanged(nameof(Notification));
			}
		}

		// unread notification count
		private int unreadNotificationCount;
		public int UnreadNotificationCount
		{
			get { return unreadNotificationCount; }
			set
			{
				unreadNotificationCount = value;
				OnPropertyChanged(nameof(UnreadNotificationCount));
			}
		}

		// is notification list showed
		private bool isNotificationOpen;
		public bool IsNotificationOpen
		{
			get { return isNotificationOpen; }
			set
			{
				isNotificationOpen = value;
				OnPropertyChanged(nameof(IsNotificationOpen));
			}
		}

		// notifications list
		private ObservableCollection<NotificationItem> notificationList;
		public ObservableCollection<NotificationItem> NotificationList
		{
			get { return notificationList; }
			set
			{
				notificationList = value;
				OnPropertyChanged(nameof(NotificationList));
			}
		}

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

		// open notification
		private DelegateCommand openNotificationCommand;
		public ICommand OpenNotificationCommand
		{
			get
			{
				if (openNotificationCommand == null)
				{
					openNotificationCommand = new DelegateCommand(delegate
					{
						IsNotificationOpen = true;
						if (IsNotificationOpen)
							UnreadNotificationCount = 0;
					});
				}
				return openNotificationCommand;
			}
		}

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
						Logout?.Invoke();
					});
				}
				return logoutCommand;
			}
		}

		#endregion
	}
}
