using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;

using Server.Security;
using MessageTypes;
using System.Xml.Serialization;
using System.IO;

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
		private List<OnlineUser> onlineUsers;

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
					Task t = ProcessConnection(client);
					activeTasks.Add(t);

					// remove completed tasks
					activeTasks.RemoveAll(x => x.IsCompleted == true);
				}
				catch (ObjectDisposedException)
				{
					// TODO logger
					Console.WriteLine("logger-___-");

					break;
				}
				catch (SocketException)
				{
					// TODO logger
					Console.WriteLine("logger-___-");

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
		private async Task ProcessConnection(TcpClient client)
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

					// recieve user's login and password
					XmlSerializer messageSerializer = new XmlSerializer(typeof(RequestMessage));

					byte[] buffer = new byte[client.ReceiveBufferSize];
					int length = sslStream.Read(buffer, 0, buffer.Length);
					MemoryStream ms = new MemoryStream(buffer, 0, length);

					RequestMessage message = (RequestMessage)messageSerializer.Deserialize(ms);

					// try login/register
					ProcessData(client, sslStream, message);
				}
				catch
				{
					// TODO logger
					Console.WriteLine("logger-___-");
				}
			});
		}

		/// <summary>
		/// Process clients message. Send response to client about required operation.
		/// </summary>
		/// <param name="client">tcp client.</param>
		/// <param name="sslStream">connected client's ssl stream.</param>
		/// <param name="message">client's message.</param>
		private void ProcessData(TcpClient client, SslStream sslStream, RequestMessage message)
		{
			bool isLoggedIn = false;
			ResponseMessage response;

			// login
			if (message is LoginRequestMessage)
			{
				LoginRequestMessage req = message as LoginRequestMessage;

				if (string.IsNullOrEmpty(req.login) ||
					string.IsNullOrEmpty(req.password) ||
					req.login.Length > 30)
				{
					response = new LoginResponseMessage { response = LoginRegisterResponse.ERROR };
				}
				else
				{
					int id;

					if (Login(req.login, req.password, out id))
					{
						// user is online
						OnlineUser user = new OnlineUser(id, req.login, client, sslStream);
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
			}
			// register
			else if (message is RegisterRequestMessage)
			{
				RegisterRequestMessage req = message as RegisterRequestMessage;

				if (string.IsNullOrEmpty(req.login) ||
					string.IsNullOrEmpty(req.password) ||
					req.login.Length > 30)
				{
					response = new RegisterResponseMessage { response = LoginRegisterResponse.ERROR };
				}
				else
				{
					if (Register(req.login, req.password))
						response = new RegisterResponseMessage { response = LoginRegisterResponse.SUCCESS };
					else
						response = new RegisterResponseMessage { response = LoginRegisterResponse.FAIL };
				}
			}
			// error
			else
			{
				response = null;
			}

			try
			{
				// response to client
				XmlSerializer responseSerializer = new XmlSerializer(typeof(ResponseMessage));
				responseSerializer.Serialize(sslStream, response);
			}
			catch
			{
				// TODO logger
				Console.WriteLine("logger-___-");
			}
			finally
			{
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
		}

		/// <summary>
		/// Do login.
		/// </summary>
		/// <param name="_login">user's login.</param>
		/// <param name="_password">user's password.</param>
		/// <param name="_id">client's id in db.</param>
		/// <returns>true, if operation had success.</returns>
		private bool Login(string _login, string _password, out int _id)
		{
			using (LinqToDatabaseDataContext DBcontext = new LinqToDatabaseDataContext())
			{
				// get user
				var data =
					from user in DBcontext.Users
					where user.login == _login
					select user;

				if (data.Any())
				{
					foreach (User user in data)
					{
						if (PasswordHash.PasswordHash.ValidatePassword(_password, user.password))
						{
							_id = user.user_id;
							return true;
						}
						else
						{
							_id = 0;
							return false;
						}
					}

					_id = 0;
					return false; // something wrong
				}
				else
				{
					// user not registered
					_id = 0;
					return false;
				}
			}
		}

		/// <summary>
		/// Do registration.
		/// </summary>
		/// <param name="_login">user's login.</param>
		/// <param name="_password">user's password.</param>
		/// <returns>true, if operation had success.</returns>
		private bool Register(string _login, string _password)
		{
			using (LinqToDatabaseDataContext DBcontext = new LinqToDatabaseDataContext())
			{
				// if login already exist
				var data =
					from user in DBcontext.Users
					where user.login == _login
					select user;

				if (data.Any())
				{
					// login already exist
					return false;
				}
				else
				{
					// new user
					User newUser = new User
					{
						login = _login,
						password = PasswordHash.PasswordHash.CreateHash(_password)
					};
					DBcontext.Users.InsertOnSubmit(newUser);

					try
					{
						DBcontext.SubmitChanges();
						Console.WriteLine(" - new user: {0}", _login);
						return true;
					}
					catch
					{
						// TODO logger
						return false;
					}
				}
			}
		}

		/// <summary>
		/// Listen to user's messages.
		/// </summary>
		/// <param name="user">user.</param>
		private void OnlineUserListener(OnlineUser user)
		{
			XmlSerializer requestSerializer = new XmlSerializer(typeof(RequestMessage));
			XmlSerializer responseSerializer = new XmlSerializer(typeof(ResponseMessage));

			try
			{
				while (true)
				{
					byte[] buffer = new byte[user.client.ReceiveBufferSize];
					int length = user.sslStream.Read(buffer, 0, buffer.Length);
					MemoryStream ms = new MemoryStream(buffer, 0, length);
					// user's incoming message
					RequestMessage message = (RequestMessage)requestSerializer.Deserialize(ms);

					// process message
					if (message is LogoutRequestMessage)
					{
						// response to user
						responseSerializer.Serialize(user.sslStream, new LogoutResponseMessage());

						onlineUsers.Remove(user);
						user.Dispose();
						break;
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
	}
}
