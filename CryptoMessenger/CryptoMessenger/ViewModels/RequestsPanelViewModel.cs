using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

using CryptoMessenger.Commands;
using CryptoMessenger.Models;

namespace CryptoMessenger.ViewModels
{
	/// <summary>
	/// Income request for ItemSource of ListBox.
	/// </summary>
	class IncomeRequest
	{
		private Client client;

		public IncomeRequest(Client client, string name)
		{
			this.client = client;
			Name = name;
		}

		// name of user that send this request
		public string Name { get; }

		// accept request
		private DelegateCommand acceptRequestCommand;
		public ICommand AcceptRequestCommand
		{
			get
			{
				if (acceptRequestCommand == null)
				{
					acceptRequestCommand = new DelegateCommand(() => 
					{ client.AcceptFriendshipRequest(Name); });
				}
				return acceptRequestCommand;
			}
		}

		// reject request
		private DelegateCommand rejectRequestCommand;
		public ICommand RejectRequestCommand
		{
			get
			{
				if (rejectRequestCommand == null)
				{
					rejectRequestCommand = new DelegateCommand(() => 
					{ client.RejectFriendshipRequest(Name); });
				}
				return rejectRequestCommand;
			}
		}
	}

	/// <summary>
	/// Outcome request for ItemSource of ListBox.
	/// </summary>
	class OutcomeRequest
	{
		private Client client;

		public OutcomeRequest(Client client, string name)
		{
			this.client = client;
			Name = name;
		}

		// name of user
		public string Name { get; }

		// cancel request
		private DelegateCommand cancelRequestCommand;
		public ICommand CancelRequestCommand
		{
			get
			{
				if (cancelRequestCommand == null)
				{
					cancelRequestCommand = new DelegateCommand(() =>
					{ client.CancelFriendshipRequest(Name); });
				}
				return cancelRequestCommand;
			}
		}
	}

	/// <summary>
	/// View model for requests panel (mvvm pattern).
	/// </summary>
	class RequestsPanelViewModel : ViewModelBase, IWindowPanel
	{
		private Client client;

		public RequestsPanelViewModel(Client client)
		{
			this.client = client;
			client.PropertyChanged += ReqsListChanged;

			IncomeReqsList = null;
			OutcomeReqsList = null;

			// get requests when panel loads
			client.GetIncomeFriendshipRequests();
			client.GetOutcomeFriendshipRequests();
		}

		// update requests lists when property in client changed
		private void ReqsListChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(client.IncomeRequestsList) && client.IncomeRequestsList != null)
			{
				IncomeReqsList = new ObservableCollection<IncomeRequest>();
				foreach (var name in client.IncomeRequestsList)
					IncomeReqsList.Add(new IncomeRequest(client, name));
			}
			else if (e.PropertyName == nameof(client.OutcomeRequestsList) && client.OutcomeRequestsList != null)
			{
				OutcomeReqsList = new ObservableCollection<OutcomeRequest>();
				foreach (var name in client.OutcomeRequestsList)
					OutcomeReqsList.Add(new OutcomeRequest(client, name));
			}
		}

		#region Properties

		// income requests list
		private ObservableCollection<IncomeRequest> _incomeReqsList;
		public ObservableCollection<IncomeRequest> IncomeReqsList
		{
			get { return _incomeReqsList; }
			set
			{
				_incomeReqsList = value;
				OnPropertyChanged(nameof(IncomeReqsList));
			}
		}

		// outcome requests list
		private ObservableCollection<OutcomeRequest> _outcomeReqsList;
		public ObservableCollection<OutcomeRequest> OutcomeReqsList
		{
			get { return _outcomeReqsList; }
			set
			{
				_outcomeReqsList = value;
				OnPropertyChanged(nameof(OutcomeReqsList));
			}
		}

		#endregion
	}
}

