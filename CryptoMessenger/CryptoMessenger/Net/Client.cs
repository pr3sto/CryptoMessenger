using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

using CryptoMessenger.GUI;
using MessageTypes;
using ConversationTypes;

namespace CryptoMessenger.Net
{
	/// <summary>
	/// Connection problems.
	/// </summary>
	[Serializable]
	public class ServerConnectionException : Exception
	{
		public ServerConnectionException()
		{
		}

		public ServerConnectionException(string message)
			: base(message)
		{
		}

		public ServerConnectionException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}

	/// <summary>
	/// Client side code.
	/// </summary>
	public class Client
	{
		// server's port 
		private int port;
		// servers ip addres
		private string ip;
		// client
		private TcpClient client;
		// ssl stream with server
		private SslStream sslStream;
		
		// form to update
		private MainForm form;

		/// <summary>
		/// Initialize Client.
		/// </summary>
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
		}

		/// <summary>
		/// Listen for messages from server.
		/// </summary>
		/// <param name="_form">form to update when message come.</param>
		public async void Listen(MainForm _form)
		{
			form = _form;
			try
			{
				while (true)
				{
					// wait for message
					Message message = await ReceiveMessage();

					// handle message
					if (message is AllUsersMessage)
					{
						string[] users = ((AllUsersMessage)message).users;
						form.UpdateAllUsersList(users);
					}
					else if (message is FriendsMessage)
					{
						form.cache_friends = ((FriendsMessage)message).friends;
						form.UpdateFriendsList(form.cache_friends);
					}
					else if (message is IncomeFriendshipRequestsMessage)
					{
						form.cache_income_reqs = ((IncomeFriendshipRequestsMessage)message).logins;
						form.UpdateIncomeFriendshipRequests(form.cache_income_reqs);
					}
					else if (message is OutcomeFriendshipRequestsMessage)
					{
						form.cache_outcome_reqs = ((OutcomeFriendshipRequestsMessage)message).logins;
						form.UpdateOutcomeFriendshipRequests(form.cache_outcome_reqs);
					}
					else if (message is ConversationMessage)
					{
						form.conversations.AddConversation(((ConversationMessage)message).conversation);
					}
					else if (message is ReplyMessage)
					{
						form.conversations.AddReply(((ReplyMessage)message).interlocutor, ((ReplyMessage)message).reply);
					}
				}
			}
			catch (ServerConnectionException)
			{
				// disconnect from server
				// TODO do something -__-
			}
		}

		/// <summary>
		/// Try to login into user account.
		/// </summary>
		/// <param name="_login">users login.</param>
		/// <param name="_password">users password.</param>
		/// <returns>server's response.</returns>
		/// <exception cref="ServerConnectionException">connection problems.</exception>
		/// <exception cref="ClientCertificateException">can't get local certificate.</exception>
		public async Task<LoginRegisterResponse> Login(string _login, string _password)
		{
			await Connect();

			var message = new LoginRequestMessage
			{
				login = _login,
				password = _password
			};

			SendMessage(message);
			// async wait for server's response
			var serverResp = (LoginRegisterResponseMessage)await ReceiveMessage();

			// don't disconnect if login success
			if (!LoginRegisterResponse.SUCCESS.Equals(serverResp.response))
				Disconnect();

			return serverResp.response;
		}

		/// <summary>
		/// Logout from server.
		/// </summary>
		/// <exception cref="ServerConnectionException">connection problems.</exception>
		public void Logout()
		{
			SendMessage(new LogoutRequestMessage());
			try
			{
				Disconnect();
			}
			catch
			{
				// dont mind because we close application
			}
		}

		/// <summary>
		/// Try to register client on the server.
		/// </summary>
		/// <param name="_login">users login.</param>
		/// <param name="_password">users password.</param>
		/// <returns>server's response.</returns>
		/// <exception cref="ServerConnectionException">connection problems.</exception>
		/// <exception cref="ClientCertificateException">can't get local certificate.</exception>
		public async Task<LoginRegisterResponse> Register(string _login, string _password)
		{
			await Connect();

			var message = new RegisterRequestMessage
			{
				login = _login,
				password = _password
			};

			SendMessage(message);
			// async wait for server's response
			var serverResp = (LoginRegisterResponseMessage)await ReceiveMessage();
			
			Disconnect();

			return serverResp.response;
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
			SendMessage(new GetFriendsMessage());
		}

		/// <summary>
		/// Get array of income friendship requests from server;
		/// listener should receive response message.
		/// </summary>
		public void GetIncomeFriendshipRequests()
		{
			SendMessage(new GetIncomeFriendshipRequestsMessage());
		}

		/// <summary>
		/// Get array of outcome friendship requests from server;
		/// listener should receive response message.
		/// </summary>
		public void GetOutcomeFriendshipRequests()
		{
			SendMessage(new GetOutcomeFriendshipRequestsMessage());
		}

		/// <summary>
		/// Send friendship request to server.
		/// </summary>
		/// <param name="login">login of needed user.</param>
		public void SendFriendshipRequest(string login)
		{
			SendMessage(new FriendshipRequestMessage { login_of_needed_user = login });
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
			SendMessage(new ReplyMessage
			{
				interlocutor = receiver,
				reply = new ConversationReply
				{
					text = text
				}
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

		#region Communication with server

		/// <summary>
		/// Connect to server.
		/// </summary>
		/// <exception cref="ServerConnectionException">connection problems.</exception>
		/// <exception cref="ClientCertificateException">can't get local certificate.</exception>
		private async Task Connect()
		{
			try
			{
				client = new TcpClient();
				
				await Task.Run(()=>
				{
					// timeout for waiting connection
					if (!client.ConnectAsync(ip, port).Wait(5000))
						throw new TimeoutException();
				});

				sslStream = new SslStream(client.GetStream(), true,
					SslStuff.ServerValidationCallback,
					SslStuff.ClientCertificateSelectionCallback,
					EncryptionPolicy.RequireEncryption);

				// handshake
				SslStuff.ClientSideHandshake(sslStream, ip);
			}
			catch (ClientCertificateException)
			{
				throw;
			}
			catch
			{
				throw new ServerConnectionException();
			}
		}

		/// <summary>
		/// Disconnect from server.
		/// </summary>
		/// <exception cref="ServerConnectionException">connection problems.</exception>
		private void Disconnect()
		{
			try
			{
				client.Client.Shutdown(SocketShutdown.Both);
			}
			catch
			{
				throw new ServerConnectionException();
			}
			finally
			{
				sslStream.Dispose();
				client.Close();
			}
		}

		/// <summary>
		/// Send message to server.
		/// </summary>
		/// <param name="message">user's message.</param>
		private void SendMessage(Message message)
		{
			try
			{
				var requestSerializer = new XmlSerializer(typeof(Message));
				requestSerializer.Serialize(sslStream, message);
			}
			catch
			{
			}
		}

		/// <summary>
		/// Reseive response from server.
		/// </summary>
		/// <returns>server's response.</returns>
		/// <exception cref="ServerConnectionException">connection problems.</exception>
		private async Task<Message> ReceiveMessage()
		{
			// asynchronous communicate with server
			return await Task.Run(() => {

				try
				{
					var responseSerializer = new XmlSerializer(typeof(Message));

					byte[] buffer = new byte[client.ReceiveBufferSize];
					int length = sslStream.Read(buffer, 0, buffer.Length);
					var ms = new MemoryStream(buffer, 0, length);

					return (Message)responseSerializer.Deserialize(ms);
				}
				catch
				{
					throw new ServerConnectionException();
				}

			});
		}

		#endregion
	}
}
