using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;

using Server.Security;
using Server.Database;

using MessageProtocol.MessageTypes;
using MessageProtocol.Client;

namespace Server
{
	/// <summary>
	/// Server side code.
	/// </summary>
	class Server
	{
		// port number
		private int port;
		// tcp listener
		private TcpListener listener;
		// active tasks of handling connections
		private List<Task> activeTasks;
		// is server started listening to clients
		private bool isStarted;

		// handler of online users
		private OnlineUsersHandler usersHandler;

		/// <summary>
		/// Initialize server, that listen to clients.
		/// </summary>
		public Server(int port)
		{
			this.port = port;
			activeTasks = new List<Task>();
			usersHandler = new OnlineUsersHandler();
			isStarted = false;
		}

		/// <summary>
		/// Start listen to clients and process connected clients.
		/// </summary>
		public async void Start()
		{
			if (isStarted) return;

			listener = TcpListener.Create(port);
			listener.Start();
			isStarted = true;

			Console.WriteLine("{0}: Server is now listening.", DateTime.Now);

			while (true)
			{
				try
				{
					var client = await listener.AcceptTcpClientAsync();
					Task t = HandleClient(client);
					activeTasks.Add(t);

					// remove completed tasks
					activeTasks.RemoveAll(x => x.IsCompleted == true);
				}
				catch (SocketException)
				{
					// TODO logger
					listener.Stop();
					listener = null;
					listener = TcpListener.Create(port);
					listener.Start();
				}
				catch
				{
					// TODO logger
					break;
				}
			}

			// wait for finishing process clients
			Task.WaitAll(activeTasks.ToArray());

			Console.WriteLine("{0}: Server is now stopped listening.", DateTime.Now);
			isStarted = false;
		}

		/// <summary>
		/// Stop accepting clients.
		/// </summary>
		public void Stop()
		{
			if (!isStarted) return;

			usersHandler.Dispose();
			listener.Stop();
			listener = null;
		}

		/// <summary>
		/// Handle client.
		/// </summary>
		/// <param name="client">Connected client.</param>
		private async Task HandleClient(TcpClient client)
		{
			Console.WriteLine("{0}: Client connected. ip {1}", DateTime.Now,
				((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());

			await Task.Run(() =>
			{
				try
				{
					// create ssl stream with client
					SslStream sslStream = new SslStream(client.GetStream(), true,
						SslStuff.ClientValidationCallback,
						SslStuff.ServerCertificateSelectionCallback,
						EncryptionPolicy.RequireEncryption);
					
					// handshake
					SslStuff.ServerSideHandshake(sslStream);

					// recieve client's message
					Message message = MpClient.ReceiveMessage(sslStream);

					// login / register
					if (message != null)
					{
						if (message is LoginRequestMessage)
							LoginClient(client, sslStream, (LoginRequestMessage)message);
						else if (message is RegisterRequestMessage)
							RegisterClient(client, sslStream, (RegisterRequestMessage)message);
					}
				}
				catch (ServerCertificateException)
				{
					// TODO logger
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
			Message response;

			if (string.IsNullOrEmpty(message.login) ||
				string.IsNullOrEmpty(message.password) ||
				message.login.Length > 30)
			{
				response = new LoginRegisterResponseMessage { response = LoginRegisterResponse.ERROR };
			}
			else if (usersHandler.GetOnlineUser(message.login) != null)
			{
				response = new LoginRegisterResponseMessage { response = LoginRegisterResponse.ALREADY_LOGIN };
			}
			else
			{
				int id; // client's id in db
				if (DBoperations.Login(message.login, message.password, out id))
				{
					Console.WriteLine("{0}: Client login: {1}", DateTime.Now, message.login);

					// user is online
					OnlineUser user = new OnlineUser(id, message.login, client, sslStream);
					usersHandler.AddUser(user);
					isLoggedIn = true;
					response = new LoginRegisterResponseMessage { response = LoginRegisterResponse.SUCCESS };
				}
				else
				{
					response = new LoginRegisterResponseMessage { response = LoginRegisterResponse.FAIL };
				}
			}

			try
			{
				// response to client
				MpClient.SendMessage(sslStream, response);
			}
			catch (ConnectionInterruptedException)
			{
				// TODO logger
			}

			// close connection with client if client not logged in
			if (!isLoggedIn)
			{
				Console.WriteLine("{0}: Client disconnected. ip {1}", DateTime.Now,
					((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());

				try
				{
					client.Client.Shutdown(SocketShutdown.Both);
				}
				catch
				{
					// TODO logger
				}
				finally
				{
					sslStream.Dispose();
					client.Close();
				}
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
			Message response;

			if (string.IsNullOrEmpty(message.login) ||
				string.IsNullOrEmpty(message.password) ||
				message.login.Length > 30)
			{
				response = new LoginRegisterResponseMessage { response = LoginRegisterResponse.ERROR };
			}
			else
			{
				if (DBoperations.Register(message.login, message.password))
				{
					Console.WriteLine("{0}: New client: {1}", DateTime.Now, message.login);

					response = new LoginRegisterResponseMessage { response = LoginRegisterResponse.SUCCESS };
				}
				else
				{
					response = new LoginRegisterResponseMessage { response = LoginRegisterResponse.FAIL };
				}
			}

			try
			{
				// response to client
				MpClient.SendMessage(sslStream, response);
			}
			catch (ConnectionInterruptedException)
			{
				// TODO logger
			}
			
			// close connection
			Console.WriteLine("{0}: Client disconnected. ip {1}", DateTime.Now,
					((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());

			try
			{
				client.Client.Shutdown(SocketShutdown.Both);
			}
			catch
			{
				// TODO logger
			}
			finally
			{
				sslStream.Dispose();
				client.Close();
			}
		}
	}
}
