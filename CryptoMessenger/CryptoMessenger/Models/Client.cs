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
	public class Client
	{
		// is we logged in
		private bool _isLoggedIn = false;
		// is we now log out
		private bool _isLogOut = false;

		// server's port 
		private int port;
		// servers ip addres
		private string ip;
		// client
		private MpClient client;

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

			//client
			client = new MpClient();
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
				_isLogOut = false;
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
			_isLogOut = true;

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
					//if (!_isLogOut)
						//form.CloseEmergency();

					return;
				}

				// handle message
				if (message is AllUsersMessage)
				{
					//form.UpdateAllUsersList(((AllUsersMessage)message).users);
				}
				else if (message is FriendsMessage)
				{
					//form.UpdateFriendsList(((FriendsMessage)message).friends);
				}
				else if (message is IncomeFriendshipRequestsMessage)
				{
					//form.UpdateIncomeFriendshipRequests(((IncomeFriendshipRequestsMessage)message).logins);
				}
				else if (message is OutcomeFriendshipRequestsMessage)
				{
					//form.UpdateOutcomeFriendshipRequests(((OutcomeFriendshipRequestsMessage)message).logins);
				}
				else if (message is ReplyMessage)
				{
					//form.UpdateConversations(
					//	((ReplyMessage)message).interlocutor, 
					//	new ConversationReply
					//	(
					//		((ReplyMessage)message).reply_author,
					//		((ReplyMessage)message).reply_time,
					//		((ReplyMessage)message).reply_text
					//	)
					//);
				}
			}
		}

		/// <summary>
		/// Get array of all users from server;
		/// listener should receive response message.
		/// </summary>
		public void GetAllUsers()
		{
			if (!_isLoggedIn) return;

			try
			{
				client.SendMessage(new GetAllUsersMessage());
			}
			catch (ConnectionInterruptedException)
			{
				try
				{
					// try again
					client.SendMessage(new GetAllUsersMessage());
				}
				catch (ConnectionInterruptedException)
				{
					// fail two times -> something wrong with connection
					//form.CloseEmergency();
				}
			}
		}

		/// <summary>
		/// Get array of friends from server;
		/// listener should receive response message.
		/// </summary>
		public void GetFriends()
		{
			if (!_isLoggedIn) return;

			try
			{
				client.SendMessage(new GetFriendsMessage());
			}
			catch (ConnectionInterruptedException)
			{
				try
				{
					// try again
					client.SendMessage(new GetFriendsMessage());
				}
				catch (ConnectionInterruptedException)
				{
					// fail two times -> something wrong with connection
					//form.CloseEmergency();
				}
			}
		}

		/// <summary>
		/// Get array of income friendship requests from server;
		/// listener should receive response message.
		/// </summary>
		public void GetIncomeFriendshipRequests()
		{
			if (!_isLoggedIn) return;

			try
			{
				client.SendMessage(new GetIncomeFriendshipRequestsMessage());
			}
			catch (ConnectionInterruptedException)
			{
				try
				{
					// try again
					client.SendMessage(new GetIncomeFriendshipRequestsMessage());
				}
				catch (ConnectionInterruptedException)
				{
					// fail two times -> something wrong with connection
					//form.CloseEmergency();
				}
			}
		}

		/// <summary>
		/// Get array of outcome friendship requests from server;
		/// listener should receive response message.
		/// </summary>
		public void GetOutcomeFriendshipRequests()
		{
			if (!_isLoggedIn) return;

			try
			{
				client.SendMessage(new GetOutcomeFriendshipRequestsMessage());
			}
			catch (ConnectionInterruptedException)
			{
				try
				{
					// try again
					client.SendMessage(new GetOutcomeFriendshipRequestsMessage());
				}
				catch (ConnectionInterruptedException)
				{
					// fail two times -> something wrong with connection
					//form.CloseEmergency();
				}
			}
		}

		/// <summary>
		/// Send friendship request to server.
		/// </summary>
		/// <param name="login">login of needed user.</param>
		public void SendFriendshipRequest(string login)
		{
			if (!_isLoggedIn) return;

			try
			{
				client.SendMessage(new FriendshipRequestMessage
				{
					login_of_needed_user = login
				});
			}
			catch (ConnectionInterruptedException)
			{
				try
				{
					// try again
					client.SendMessage(new FriendshipRequestMessage
					{
						login_of_needed_user = login
					});
				}
				catch (ConnectionInterruptedException)
				{
					// fail two times -> something wrong with connection
					//form.CloseEmergency();
				}
			}
		}

		/// <summary>
		/// Send message to server about cancellation of friendship request.
		/// </summary>
		/// <param name="login">needed friend login.</param>
		public void CancelFriendshipRequest(string login)
		{
			if (!_isLoggedIn) return;

			try
			{
				client.SendMessage(new FriendActionMessage
				{
					friends_login = login,
					action = ActionsWithFriend.CANCEL_FRIENDSHIP_REQUEST
				});
			}
			catch (ConnectionInterruptedException)
			{
				try
				{
					// try again
					client.SendMessage(new FriendActionMessage
					{
						friends_login = login,
						action = ActionsWithFriend.CANCEL_FRIENDSHIP_REQUEST
					});
				}
				catch (ConnectionInterruptedException)
				{
					// fail two times -> something wrong with connection
					//form.CloseEmergency();
				}
			}
		}
		
		/// <summary>
		/// Send to server request about accepting friendship request.
		/// </summary>
		/// <param name="login">accepted friend login.</param>
		public void AcceptFriendshipRequest(string login)
		{
			if (!_isLoggedIn) return;

			try
			{
				client.SendMessage(new FriendActionMessage
				{
					friends_login = login,
					action = ActionsWithFriend.ACCEPT_FRIENDSHIP
				});
			}
			catch (ConnectionInterruptedException)
			{
				try
				{
					// try again
					client.SendMessage(new FriendActionMessage
					{
						friends_login = login,
						action = ActionsWithFriend.ACCEPT_FRIENDSHIP
					});
				}
				catch (ConnectionInterruptedException)
				{
					// fail two times -> something wrong with connection
					//form.CloseEmergency();
				}
			}
		}

		/// <summary>
		/// Send to server request about rejecting friendship request.
		/// </summary>
		/// <param name="login">rejected user login.</param>
		public void RejectFriendshipRequest(string login)
		{
			if (!_isLoggedIn) return;

			try
			{
				client.SendMessage(new FriendActionMessage
				{
					friends_login = login,
					action = ActionsWithFriend.REJECT_FRIENDSHIP
				});
			}
			catch (ConnectionInterruptedException)
			{
				try
				{
					// try again
					client.SendMessage(new FriendActionMessage
					{
						friends_login = login,
						action = ActionsWithFriend.REJECT_FRIENDSHIP
					});
				}
				catch (ConnectionInterruptedException)
				{
					// fail two times -> something wrong with connection
					//form.CloseEmergency();
				}
			}
		}

		/// <summary>
		/// Send to server request about removing friend from friends.
		/// </summary>
		/// <param name="login">friend's login.</param>
		public void RemoveFriend(string login)
		{
			if (!_isLoggedIn) return;

			try
			{
				client.SendMessage(new FriendActionMessage
				{
					friends_login = login,
					action = ActionsWithFriend.REMOVE_FROM_FRIENDS
				});
			}
			catch (ConnectionInterruptedException)
			{
				try
				{
					// try again
					client.SendMessage(new FriendActionMessage
					{
						friends_login = login,
						action = ActionsWithFriend.REMOVE_FROM_FRIENDS
					});
				}
				catch (ConnectionInterruptedException)
				{
					// fail two times -> something wrong with connection
					//form.CloseEmergency();
				}
			}
		}

		/// <summary>
		/// Send conversation reply to server.
		/// </summary>
		/// <param name="receiver">receiver's login.</param>
		/// <param name="text">text of reply.</param>
		public void SendReply(string receiver, string text)
		{
			if (!_isLoggedIn) return;

			try
			{
				client.SendMessage(new ReplyMessage
				{
					interlocutor = receiver,
					reply_text = text
				});
			}
			catch (ConnectionInterruptedException)
			{
				try
				{
					// try again
					client.SendMessage(new ReplyMessage
					{
						interlocutor = receiver,
						reply_text = text
					});
				}
				catch (ConnectionInterruptedException)
				{
					// fail two times -> something wrong with connection
					//form.CloseEmergency();
				}
			}
		}

		/// <summary>
		/// Get conversation with interlocutor from server;
		/// listener should receive response message.
		/// </summary>
		/// <param name="interlocutor">interlocutor.</param>
		public void GetConversation(string interlocutor)
		{
			if (!_isLoggedIn) return;

			try
			{
				client.SendMessage(new GetConversationMessage
				{
					interlocutor = interlocutor
				});
			}
			catch (ConnectionInterruptedException)
			{
				try
				{
					// try again
					client.SendMessage(new GetConversationMessage
					{
						interlocutor = interlocutor
					});
				}
				catch (ConnectionInterruptedException)
				{
					// fail two times -> something wrong with connection
					//form.CloseEmergency();
				}
			}
		}
	}
}
