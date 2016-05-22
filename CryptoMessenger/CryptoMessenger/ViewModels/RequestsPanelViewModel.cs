using System.Linq;
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

		// name of user that send this request
		public string Name { get; }

		public IncomeRequest(Client client, string name)
		{
			this.client = client;
			Name = name;
		}

		// accept request
		private DelegateCommand acceptRequestCommand;
		public ICommand AcceptRequestCommand
		{
			get
			{
				if (acceptRequestCommand == null)
				{
					acceptRequestCommand = new DelegateCommand(delegate 
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
					rejectRequestCommand = new DelegateCommand(delegate
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

		// name of user
		public string Name { get; }

		public OutcomeRequest(Client client, string name)
		{
			this.client = client;
			Name = name;
		}

		// cancel request
		private DelegateCommand cancelRequestCommand;
		public ICommand CancelRequestCommand
		{
			get
			{
				if (cancelRequestCommand == null)
				{
					cancelRequestCommand = new DelegateCommand(delegate
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

			IncomeReqsList = new ObservableCollection<IncomeRequest>();
			OutcomeReqsList = new ObservableCollection<OutcomeRequest>();

			client.FriendshipAccepted += async delegate (string login)
			{
				await System.Threading.Tasks.Task.Run(() => System.Threading.Thread.Sleep(1000));
				OutcomeReqsList.Remove(OutcomeReqsList.FirstOrDefault(x => x.Name.Equals(login)));
			};
			client.FriendshipRejected += async delegate (string login)
			{
				await System.Threading.Tasks.Task.Run(() => System.Threading.Thread.Sleep(1000));
				OutcomeReqsList.Remove(OutcomeReqsList.FirstOrDefault(x => x.Name.Equals(login)));
			};
			client.NewFriendshipRequest += async delegate (string login)
			{
				await System.Threading.Tasks.Task.Run(() => System.Threading.Thread.Sleep(1000));
				IncomeReqsList.Add(new IncomeRequest(client, login));
			};
			client.FriendshipRequestCancelled += async delegate (string login)
			{
				await System.Threading.Tasks.Task.Run(() => System.Threading.Thread.Sleep(1000));
				IncomeReqsList.Remove(IncomeReqsList.FirstOrDefault(x => x.Name.Equals(login)));
			};

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
		private ObservableCollection<IncomeRequest> incomeReqsList;
		public ObservableCollection<IncomeRequest> IncomeReqsList
		{
			get { return incomeReqsList; }
			set
			{
				incomeReqsList = value;
				OnPropertyChanged(nameof(IncomeReqsList));
			}
		}

		// outcome requests list
		private ObservableCollection<OutcomeRequest> outcomeReqsList;
		public ObservableCollection<OutcomeRequest> OutcomeReqsList
		{
			get { return outcomeReqsList; }
			set
			{
				outcomeReqsList = value;
				OnPropertyChanged(nameof(OutcomeReqsList));
			}
		}

		#endregion
	}
}

