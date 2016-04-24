using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Security.Cryptography.X509Certificates;

using MessageProtocol;
using MessageProtocol.MessageTypes;

namespace CryptoMessenger.Models
{
	/// <summary>
	/// Client side code.
	/// </summary>
	public class Client : INotifyPropertyChanged
	{
		// is client logged in
		private bool _isLoggedIn = false;

		// server's port 
		private int port;
		// servers ip addres
		private string ip;
		// client
		private MpClient client;

		/// <summary>
		/// Client's name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Notify about connection breaks.
		/// </summary>
		public event Action ConnectionBreaks;

		/// <summary>
		/// User conversations.
		/// </summary>
		public Conversations Conversations { get; private set; }


		#region Data that comes from server

		private string[] _friendsList;
		/// <summary>
		/// Array of friends.
		/// </summary>
		public string[] FriendsList
		{
			get { return _friendsList; }
			private set
			{
				_friendsList = value;
				RaisePropertyChanged(nameof(FriendsList));
			}
		}
		private string[] _searchUsersList;
		/// <summary>
		/// Array of all users.
		/// </summary>
		public string[] SearchUsersList
		{
			get { return _searchUsersList; }
			private set
			{
				_searchUsersList = value;
				RaisePropertyChanged(nameof(SearchUsersList));
			}
		}
		private string[] _incomeRequestsList;
		/// <summary>
		/// Array of income requests.
		/// </summary>
		public string[] IncomeRequestsList
		{
			get { return _incomeRequestsList; }
			private set
			{
				_incomeRequestsList = value;
				RaisePropertyChanged(nameof(IncomeRequestsList));
			}
		}
		private string[] _outcomeRequestsList;
		/// <summary>
		/// Array of outcome requests.
		/// </summary>
		public string[] OutcomeRequestsList
		{
			get { return _outcomeRequestsList; }
			private set
			{
				_outcomeRequestsList = value;
				RaisePropertyChanged(nameof(OutcomeRequestsList));
			}
		}

		/// <summary>
		/// Notify about data from server comes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		private void RaisePropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion


		public Client()
		{
			// get data from connection.cfg
			XDocument doc = XDocument.Load("connection.config");

			var _ip = doc.Descendants("ip");
			var _port = doc.Descendants("port");

			ip = "";
			foreach (var i in _ip) ip = i.Value;
	
			port = 0;
			foreach (var i in _port) port = int.Parse(i.Value);

			FriendsList = null;
			SearchUsersList = null;
			IncomeRequestsList = null;
			OutcomeRequestsList = null;
			
			client = new MpClient();
			Name = null;
			Conversations = new Conversations();

			this.ConnectionBreaks += () => { _isLoggedIn = false; };
		}

		/// <summary>
		/// Try to login into user account.
		/// </summary>
		/// <param name="login">users login.</param>
		/// <param name="password">users password.</param>
		/// <returns>server's response.</returns>
		/// <exception cref="ConnectionInterruptedException"></exception>
		/// <exception cref="CertificateException"></exception>
		public async Task<LoginRegisterResponse> Login(string login, string password)
		{
			if (_isLoggedIn) return LoginRegisterResponse.ERROR;

			// certificate
			X509Certificate2 cert = SslTools.CreateCertificate(typeof(Client), "CryptoMessenger.Certificate.cert.pfx");

			await client.ConnectWithTimeoutAsync(ip, port, cert, 5000);

			LoginRegisterResponseMessage serverResp;

			try
			{
				// send login request
				client.SendMessage(new LoginRequestMessage
				{
					login = login,
					password = password
				});

				// async wait for server's response
				serverResp = (LoginRegisterResponseMessage)await client.ReceiveMessageAsync();
			}
			catch (ConnectionInterruptedException)
			{
				client.Close();
				throw;
			}

			// dont disconnect if login success
			if (LoginRegisterResponse.SUCCESS.Equals(serverResp.response))
			{
				_isLoggedIn = true;
				Name = login;
			}
			else
			{
				client.Close();
			}

			return serverResp.response;
		}

		/// <summary>
		/// Try to register client on the server.
		/// </summary>
		/// <param name="_login">users login.</param>
		/// <param name="_password">users password.</param>
		/// <returns>server's response.</returns>
		/// <exception cref="ConnectionInterruptedException"></exception>
		/// <exception cref="CertificateException"></exception>
		public async Task<LoginRegisterResponse> Register(string _login, string _password)
		{
			if (_isLoggedIn) return LoginRegisterResponse.ERROR;

			// certificate
			X509Certificate2 cert = SslTools.CreateCertificate(typeof(Client), "CryptoMessenger.Certificate.cert.pfx");

			await client.ConnectWithTimeoutAsync(ip, port, cert, 5000);

			LoginRegisterResponseMessage serverResp;

			try
			{
				// send register request
				client.SendMessage(new RegisterRequestMessage
				{
					login = _login,
					password = _password
				});

				// async wait for server's response
				serverResp = (LoginRegisterResponseMessage)await client.ReceiveMessageAsync();
			}
			catch (ConnectionInterruptedException)
			{
				throw;
			}
			finally
			{
				client.Close();
			}

			return serverResp.response;
		}

		/// <summary>
		/// Logout from server.
		/// </summary>
		public void Logout()
		{
			if (!_isLoggedIn) return;

			_isLoggedIn = false;

			Name = null;
			FriendsList = null;
			SearchUsersList = null;
			IncomeRequestsList = null;
			OutcomeRequestsList = null;

			Conversations = new Conversations();

			try
			{
				client.SendMessage(new LogoutRequestMessage());
			}
			catch (ConnectionInterruptedException)
			{
				// dont mind because we exit
			}
			finally
			{
				try
				{
					client.Close();
				}
				catch (ConnectionInterruptedException)
				{
					// dont mind because we exit
				}
			}
		}


		/// <summary>
		/// Notify about reply from server comes.
		/// <param name="interlocutor">interlocutor.</param>
		/// <param name="reply">new reply.</param>
		/// </summary>
		public event Action<string, ConversationReply> NewReplyComes;
		/// <summary>
		/// Notify about reply from server comes.
		/// <param name="interlocutor">interlocutor.</param>
		/// <param name="reply">new reply.</param>
		/// </summary>
		public event Action<string, ConversationReply> OldReplyComes;


		/// <summary>
		/// Listen for messages from server.
		/// </summary>
		/// <param name="//form">//form to update when message come.</param>
		public async void Listen()
		{
			if (!_isLoggedIn) return;
			
			// message from server
			Message message;

			while (true)
			{
				try
				{
					// wait for message
					message = await client.ReceiveMessageAsync();
				}
				catch (ConnectionInterruptedException)
				{
					if (_isLoggedIn)
						ConnectionBreaks();

					return;
				}

				// handle message
				if (message is AllUsersMessage)
				{
					SearchUsersList = ((AllUsersMessage)message).users;
				}
				else if (message is FriendsMessage)
				{
					FriendsList = ((FriendsMessage)message).friends;					
				}
				else if (message is IncomeFriendshipRequestsMessage)
				{
					IncomeRequestsList = ((IncomeFriendshipRequestsMessage)message).logins;
				}
				else if (message is OutcomeFriendshipRequestsMessage)
				{
					OutcomeRequestsList = ((OutcomeFriendshipRequestsMessage)message).logins;
				}
				else if (message is NewReplyMessage)
				{
					Conversations.AddReply(
						((NewReplyMessage)message).interlocutor,
						new ConversationReply
						{
							Author = ((NewReplyMessage)message).reply_author,
							Time = ((NewReplyMessage)message).reply_time,
							Text = ((NewReplyMessage)message).reply_text
						}
					);
					NewReplyComes(
						((NewReplyMessage)message).interlocutor,
						new ConversationReply
						{
							Author = ((NewReplyMessage)message).reply_author,
							Time = ((NewReplyMessage)message).reply_time,
							Text = ((NewReplyMessage)message).reply_text
						});
				}
				else if (message is OldReplyMessage)
				{
					Conversations.InsertReplyToTop(
						((OldReplyMessage)message).interlocutor,
						new ConversationReply
						{
							Author = ((OldReplyMessage)message).reply_author,
							Time = ((OldReplyMessage)message).reply_time,
							Text = ((OldReplyMessage)message).reply_text
						}
					);
					OldReplyComes(
						((OldReplyMessage)message).interlocutor,
						new ConversationReply
						{
							Author = ((OldReplyMessage)message).reply_author,
							Time = ((OldReplyMessage)message).reply_time,
							Text = ((OldReplyMessage)message).reply_text
						});
				}
			}
		}

		#region Server requests

		/// <summary>
		/// Sends message to server (and re-send if needs).
		/// </summary>
		/// <param name="message">message to send.</param>
		private void SendMessage(Message message)
		{
			if (!_isLoggedIn) return;

			try
			{
				client.SendMessage(message);
			}
			catch (ConnectionInterruptedException)
			{
				try
				{
					// try again
					client.SendMessage(message);
				}
				catch (ConnectionInterruptedException)
				{
					// fail two times -> something wrong with connection
					Logout();
					ConnectionBreaks();
				}
			}
		}

		/// <summary>
		/// Get array of all users from server;
		/// listener should receive response message.
		/// </summary>
		public void GetAllUsers()
		{
			SendMessage(new GetAllUsersMessage());
		}

		/// <summary>
		/// Get array of friends from server;
		/// listener should receive response message.
		/// </summary>
		public void GetFriends()
		{
			// dont ask server if we have
			if (FriendsList != null)
			{
				RaisePropertyChanged(nameof(FriendsList));
				return;
			}

			SendMessage(new GetFriendsMessage());
		}

		/// <summary>
		/// Get array of income friendship requests from server;
		/// listener should receive response message.
		/// </summary>
		public void GetIncomeFriendshipRequests()
		{
			// dont ask server if we have
			if (IncomeRequestsList != null)
			{
				RaisePropertyChanged(nameof(IncomeRequestsList));
				return;
			}

			SendMessage(new GetIncomeFriendshipRequestsMessage());
		}

		/// <summary>
		/// Get array of outcome friendship requests from server;
		/// listener should receive response message.
		/// </summary>
		public void GetOutcomeFriendshipRequests()
		{
			// dont ask server if we have
			if (OutcomeRequestsList != null)
			{
				RaisePropertyChanged(nameof(OutcomeRequestsList));
				return;
			}

			SendMessage(new GetOutcomeFriendshipRequestsMessage());
		}

		/// <summary>
		/// Send friendship request to server.
		/// </summary>
		/// <param name="login">login of needed user.</param>
		public void SendFriendshipRequest(string login)
		{
			SendMessage(new FriendshipRequestMessage
			{
				login_of_needed_user = login
			});
		}

		/// <summary>
		/// Send message to server about cancellation of friendship request.
		/// </summary>
		/// <param name="login">needed friend login.</param>
		public void CancelFriendshipRequest(string login)
		{
			SendMessage(new FriendActionMessage
			{
				friends_login = login,
				action = ActionsWithFriend.CANCEL_FRIENDSHIP_REQUEST
			});
		}
		
		/// <summary>
		/// Send to server request about accepting friendship request.
		/// </summary>
		/// <param name="login">accepted friend login.</param>
		public void AcceptFriendshipRequest(string login)
		{
			SendMessage(new FriendActionMessage
			{
				friends_login = login,
				action = ActionsWithFriend.ACCEPT_FRIENDSHIP
			});
		}

		/// <summary>
		/// Send to server request about rejecting friendship request.
		/// </summary>
		/// <param name="login">rejected user login.</param>
		public void RejectFriendshipRequest(string login)
		{
			SendMessage(new FriendActionMessage
			{
				friends_login = login,
				action = ActionsWithFriend.REJECT_FRIENDSHIP
			});
		}

		/// <summary>
		/// Send to server request about removing friend from friends.
		/// </summary>
		/// <param name="login">friend's login.</param>
		public void RemoveFriend(string login)
		{
			SendMessage(new FriendActionMessage
			{
				friends_login = login,
				action = ActionsWithFriend.REMOVE_FROM_FRIENDS
			});
		}

		/// <summary>
		/// Send conversation reply to server.
		/// </summary>
		/// <param name="receiver">receiver's login.</param>
		/// <param name="text">text of reply.</param>
		public void SendReply(string receiver, string text)
		{
			SendMessage(new NewReplyMessage
			{
				interlocutor = receiver,
				reply_text = text
			});
		}

		/// <summary>
		/// Get conversation with interlocutor from server;
		/// listener should receive response message.
		/// </summary>
		/// <param name="interlocutor">interlocutor.</param>
		public void GetConversation(string interlocutor)
		{
			SendMessage(new GetConversationMessage
			{
				interlocutor = interlocutor
			});
		}

		#endregion
	}
}
