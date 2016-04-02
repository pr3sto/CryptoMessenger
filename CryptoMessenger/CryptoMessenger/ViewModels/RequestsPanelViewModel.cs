using System.Collections.Generic;
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

		public IncomeRequest(Client client)
		{
			this.client = client;
		}

		// name of user that send this request
		public string Name { get; set; }

		// accept request
		private DelegateCommand acceptRequestCommand;
		public ICommand AcceptRequestCommand
		{
			get
			{
				if (acceptRequestCommand == null)
				{
					acceptRequestCommand = new DelegateCommand(AcceptRequest);
				}
				return acceptRequestCommand;
			}
		}
		private void AcceptRequest()
		{
			client.AcceptFriendshipRequest(Name);
		}

		// reject request
		private DelegateCommand rejectRequestCommand;
		public ICommand RejectRequestCommand
		{
			get
			{
				if (rejectRequestCommand == null)
				{
					rejectRequestCommand = new DelegateCommand(RejectRequest);
				}
				return rejectRequestCommand;
			}
		}
		private void RejectRequest()
		{
			client.RejectFriendshipRequest(Name);
		}
	}

	/// <summary>
	/// Outcome request for ItemSource of ListBox.
	/// </summary>
	class OutcomeRequest
	{
		private Client client;

		public OutcomeRequest(Client client)
		{
			this.client = client;
		}

		// name of user
		public string Name { get; set; }

		// cancel request
		private DelegateCommand cancelRequestCommand;
		public ICommand CancelRequestCommand
		{
			get
			{
				if (cancelRequestCommand == null)
				{
					cancelRequestCommand = new DelegateCommand(CancelRequest);
				}
				return cancelRequestCommand;
			}
		}
		private void CancelRequest()
		{
			client.CancelFriendshipRequest(Name);
		}
	}

	/// <summary>
	/// View model for requests panel (mvvm pattern).
	/// </summary>
	class RequestsPanelViewModel : ViewModelBase, ILeftPanel
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
			if (e.PropertyName == nameof(client.IncomeRequestsList))
			{
				List<IncomeRequest> reqs = new List<IncomeRequest>();
				foreach (var s in client.IncomeRequestsList)
				{
					IncomeRequest r = new IncomeRequest(client);
					r.Name = s;
					reqs.Add(r);
				}
				IncomeReqsList = reqs.ToArray();
			}
			else if (e.PropertyName == nameof(client.OutcomeRequestsList))
			{
				List<OutcomeRequest> reqs = new List<OutcomeRequest>();
				foreach (var s in client.OutcomeRequestsList)
				{
					OutcomeRequest r = new OutcomeRequest(client);
					r.Name = s;
					reqs.Add(r);
				}
				OutcomeReqsList = reqs.ToArray();
			}
		}

		#region Properties

		// income requests list
		private IncomeRequest[] _incomeReqsList;
		public IncomeRequest[] IncomeReqsList
		{
			get { return _incomeReqsList; }
			set
			{
				_incomeReqsList = value;
				OnPropertyChanged(nameof(IncomeReqsList));
			}
		}

		// outcome requests list
		private OutcomeRequest[] _outcomeReqsList;
		public OutcomeRequest[] OutcomeReqsList
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

