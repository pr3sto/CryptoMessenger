using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Xml.Serialization;

using Server.Security;
using Server.Database;
using MessageTypes;

namespace Server
{
	class Server
	{
		// port number
		private int port;
		// tcp listener
		private TcpListener listener;
		// active tasks
		private List<Task> activeTasks;
		// is server started listening to clients
		private bool IsStarted;

		// list of online users
		public List<OnlineUser> onlineUsers;

		/// <summary>
		/// Initialize server, that listen to clients.
		/// </summary>
		public Server(int port)
		{
			this.port = port;
			activeTasks = new List<Task>();
			onlineUsers = new List<OnlineUser>();
			IsStarted = false;
		}

		/// <summary>
		/// Start listen to clients and process connected clients.
		/// </summary>
		public async void Start()
		{
			if (IsStarted) return;

			listener = TcpListener.Create(port);
			listener.Start();
			IsStarted = true;

			Console.WriteLine("Server is now listening.");

			while (true)
			{
				try
				{
					var client = await listener.AcceptTcpClientAsync();
					Task t = ProcessClient(client);
					activeTasks.Add(t);

					// remove completed tasks
					activeTasks.RemoveAll(x => x.IsCompleted == true);
				}
				catch (ObjectDisposedException)
				{
					// TODO logger
					break;
				}
				catch (SocketException)
				{
					// TODO logger
					listener.Stop();
					listener = null;
					listener = TcpListener.Create(port);
					listener.Start();
				}
			}

			// wait for finishing process clients
			Task.WaitAll(activeTasks.ToArray());

			Console.WriteLine("Server is now stopped listening.");
			IsStarted = false;
		}

		/// <summary>
		/// Stop accepting clients.
		/// </summary>
		public void Stop()
		{
			if (!IsStarted) return;

			onlineUsers.ForEach((x) => { x.Dispose(); });
			onlineUsers.Clear();

			listener.Stop();
			listener = null;
		}

		/// <summary>
		/// Process connected client.
		/// </summary>
		/// <param name="client">Connected client.</param>
		private async Task ProcessClient(TcpClient client)
		{
			Console.WriteLine(" - client connected. ip {0}",
				((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());

			await Task.Run(() =>
			{
				try
				{
					SslStream sslStream = new SslStream(client.GetStream(), true,
						SslStuff.ClientValidationCallback,
						SslStuff.ServerCertificateSelectionCallback,
						EncryptionPolicy.RequireEncryption);
					
					// handshake
					SslStuff.ServerSideHandshake(sslStream);

					// recieve user's message
					RequestMessage message = ReceiveMessage(client, sslStream);

					// login / register
					if (message != null)
					{
						if (message is LoginRequestMessage)
							LoginClient(client, sslStream, (LoginRequestMessage)message);
						else if (message is RegisterRequestMessage)
							RegisterClient(client, sslStream, (RegisterRequestMessage)message);
					}
				}
				catch
				{
					// TODO logger
				}
			});
		}

		/// <summary>
		/// Try to login client. Send response about result.
		/// </summary>
		/// <param name="client">tcp client.</param>
		/// <param name="sslStream">connected client's ssl stream.</param>
		/// <param name="message">client's message.</param>
		private void LoginClient(TcpClient client, SslStream sslStream, LoginRequestMessage message)
		{
			bool isLoggedIn = false;
			ResponseMessage response;

			if (string.IsNullOrEmpty(message.login) ||
				string.IsNullOrEmpty(message.password) ||
				message.login.Length > 30)
			{
				response = new LoginResponseMessage { response = LoginRegisterResponse.ERROR };
			}
			else
			{
				int id;

				if (DBoperations.Login(message.login, message.password, out id))
				{
					// user is online
					OnlineUser user = new OnlineUser(id, message.login, client, sslStream);
					onlineUsers.Add(user);
					// listen this user
					Task.Run(() => OnlineUserListener(user));
					isLoggedIn = true;
					response = new LoginResponseMessage { response = LoginRegisterResponse.SUCCESS };
				}
				else
				{
					response = new LoginResponseMessage { response = LoginRegisterResponse.FAIL };
				}
			}

			// response to client
			SendMessage(sslStream, response);
				
			// close connection with client if client not logged in
			if (!isLoggedIn)
			{
				Console.WriteLine(" - client disconnected. ip {0}",
					((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());

				client.Client.Shutdown(SocketShutdown.Both);
				sslStream.Dispose();
				client.Close();
			}
			
		}

		/// <summary>
		/// Try to register client. Send response about result.
		/// </summary>
		/// <param name="client">tcp client.</param>
		/// <param name="sslStream">connected client's ssl stream.</param>
		/// <param name="message">client's message.</param>
		private void RegisterClient(TcpClient client, SslStream sslStream, RegisterRequestMessage message)
		{
			ResponseMessage response;

			if (string.IsNullOrEmpty(message.login) ||
				string.IsNullOrEmpty(message.password) ||
				message.login.Length > 30)
			{
				response = new RegisterResponseMessage { response = LoginRegisterResponse.ERROR };
			}
			else
			{
				if (DBoperations.Register(message.login, message.password))
					response = new RegisterResponseMessage { response = LoginRegisterResponse.SUCCESS };
				else
					response = new RegisterResponseMessage { response = LoginRegisterResponse.FAIL };
			}

			// response to client
			SendMessage(sslStream, response);

			// close connection
			Console.WriteLine(" - client disconnected. ip {0}",
				((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());

			client.Client.Shutdown(SocketShutdown.Both);
			sslStream.Dispose();
			client.Close();
		}

		/// <summary>
		/// Listen to user's messages.
		/// </summary>
		/// <param name="user">user.</param>
		private void OnlineUserListener(OnlineUser user)
		{
			try
			{
				while (true)
				{
					// user's incoming message
					RequestMessage message = ReceiveMessage(user.client, user.sslStream);

					// process message
					if (message is LogoutRequestMessage)
					{
						onlineUsers.Remove(user);
						user.Dispose();
						break;
					}
					else if (message is GetAllUsersRequestMessage)
					{
						GetAllUsersResponseMessage response = new GetAllUsersResponseMessage
						{
							users = DBoperations.GetAllUsers()
						};
						SendMessage(user.sslStream, response);
					}
					else if (message is GetFriendsRequestMessage)
					{
						GetFriendsResponseMessage response = new GetFriendsResponseMessage
						{
							friends = DBoperations.GetFriends(user.login)
						};
						SendMessage(user.sslStream, response);
					}
					else if (message is GetFriendshipReqsRequestMessage)
					{
						GetFriendsReqsResponseMessage response = new GetFriendsReqsResponseMessage
						{
							income_requests = DBoperations.GetIncomeFriendshipRequests(user.login),
							outcome_requests = DBoperations.GetOutcomeFriendshipRequests(user.login)
						};
						SendMessage(user.sslStream, response);
					}
					else if (message is FriendshipReqRequestMessage)
					{
						string login = ((FriendshipReqRequestMessage)message).login_of_needed_user;
						// set friendship
						DBoperations.SetFriendship(false, user.login, login);
						// send notification if user online
						OnlineUser friend = GetOnlineUser(login);
						if (friend != null)
						{ 
							GetFriendsReqsResponseMessage response = new GetFriendsReqsResponseMessage
							{
								income_requests = DBoperations.GetIncomeFriendshipRequests(login),
								outcome_requests = DBoperations.GetOutcomeFriendshipRequests(login)
							};
							SendMessage(friend.sslStream, response);
						}
					}
					else if (message is FriendActionRequestMessage)
					{
						FriendActionRequestMessage msg = (FriendActionRequestMessage)message;

						if (ActionsWithFriend.CANCEL_FRIENDSHIP_REQUEST.Equals(msg.action))
						{
							DBoperations.RemoveFriendshipRequest(user.login, msg.friends_login);
						}
						else if (ActionsWithFriend.ACCEPT_FRIENDSHIP.Equals(msg.action))
						{
							DBoperations.SetFriendship(true, msg.friends_login, user.login);
						}
						else if (ActionsWithFriend.REJECT_FRIENDSHIP.Equals(msg.action))
						{
							DBoperations.RemoveFriendshipRequest(msg.friends_login, user.login);
						}
						else if (ActionsWithFriend.REMOVE_FROM_FRIENDS.Equals(msg.action))
						{
							DBoperations.RemoveFriend(user.login, msg.friends_login);
						}
					}
				}
			}
			catch
			{
				// client disconnected in a bad way
				if (onlineUsers.Contains(user))
				{
					onlineUsers.Remove(user);
					user.Dispose();
				}
				// or server has been stopped
			}
		}

		/// <summary>
		/// Get online user.
		/// </summary>
		/// <param name="login">user's login.</param>
		/// <returns>user, if he online; otherwise - null.</returns>
		private OnlineUser GetOnlineUser(string login)
		{
			return onlineUsers.Find(x => x.login == login);
		}

		/// <summary>
		/// Send message to client.
		/// </summary>
		/// <param name="sslStream">stream with client.</param>
		/// <param name="message">client's message.</param>
		private void SendMessage(SslStream sslStream, ResponseMessage message)
		{
			try
			{
				XmlSerializer responseSerializer = new XmlSerializer(typeof(ResponseMessage));
				responseSerializer.Serialize(sslStream, message);
			}
			catch
			{
				// TODO logger
			}
		}

		/// <summary>
		/// Reseive request from client.
		/// </summary>
		/// <param name="client">tcp client.</param>
		/// <param name="sslStream">stream with client.</param>
		/// <returns>client's message.</returns>
		private RequestMessage ReceiveMessage(TcpClient client, SslStream sslStream)
		{
			XmlSerializer requestSerializer = new XmlSerializer(typeof(RequestMessage));

			byte[] buffer = new byte[client.ReceiveBufferSize];
			int length = sslStream.Read(buffer, 0, buffer.Length);
			MemoryStream ms = new MemoryStream(buffer, 0, length);

			return (RequestMessage)requestSerializer.Deserialize(ms);
		}
	}
}
