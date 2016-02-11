using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

using Server.Database;

using MessageProtocol;
using MessageProtocol.MessageTypes;

namespace Server
{
	/// <summary>
	/// Server side code.
	/// </summary>
	class Server
	{
		// port number
		private int port;
		// message protocol listener
		private MpListener listener;
		// active tasks of handling connections
		private List<Task> activeTasks;
		// is server started listening to clients
		private bool isStarted;

		// server's certificate
		X509Certificate2 certificate;

		// handler of online users
		private OnlineUsersHandler usersHandler;


		public Server(int port)
		{
			this.port = port;
			activeTasks = new List<Task>();
			usersHandler = new OnlineUsersHandler();
			isStarted = false;

			try
			{
				certificate = SslTools.CreateCertificate(typeof(Server), "Server.Certificate.cert.pfx");
			}
			catch (CertificateException)
			{
				// TODO logger
			}
		}

		/// <summary>
		/// Start listen to clients and process connected clients.
		/// </summary>
		public async void Start()
		{
			if (isStarted) return;
			if (certificate == null) return;

			listener = new MpListener(port, certificate);
			listener.Start();
			isStarted = true;

			Console.WriteLine("{0}: Server is now listening.", DateTime.Now);

			while (true)
			{
				try
				{
					MpClient client = await listener.AcceptMpClientAsync();
					Task t = HandleClient(client);
					activeTasks.Add(t);

					// remove completed tasks
					activeTasks.RemoveAll(x => x.IsCompleted == true);
				}
				catch (ConnectionInterruptedException)
				{
					continue;
				}
				catch (SocketException)
				{
					// TODO logger
					listener.Stop();
					listener = null;
					listener = new MpListener(port, certificate);
					listener.Start();
				}
				catch (ObjectDisposedException)
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
		private async Task HandleClient(MpClient client)
		{
			Console.WriteLine("{0}: Client connected. ip {1}", DateTime.Now,
				((IPEndPoint)client.tcpClient.Client.RemoteEndPoint).Address.ToString());

			await Task.Run(() =>
			{
				try
				{
					// recieve client's message
					Message message = client.ReceiveMessage();

					// login / register
					if (message != null)
					{
						if (message is LoginRequestMessage)
							LoginClient(client, (LoginRequestMessage)message);
						else if (message is RegisterRequestMessage)
							RegisterClient(client, (RegisterRequestMessage)message);
					}
				}
				catch (ConnectionInterruptedException)
				{
					// TODO logger
				}
			});
		}

		/// <summary>
		/// Try to login client. Send response about result.
		/// </summary>
		/// <param name="client">client.</param>
		/// <param name="message">client's message.</param>
		private void LoginClient(MpClient client, LoginRequestMessage message)
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
					OnlineUser user = new OnlineUser(id, message.login, client);
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
				client.SendMessage(response);
			}
			catch (ConnectionInterruptedException)
			{
				// TODO logger
			}

			// close connection with client if client not logged in
			if (!isLoggedIn)
			{
				Console.WriteLine("{0}: Client disconnected. ip {1}", DateTime.Now,
					((IPEndPoint)client.tcpClient.Client.RemoteEndPoint).Address.ToString());

				try
				{
					client.Close();
				}
				catch
				{
					// TODO logger
				}
			}
		}

		/// <summary>
		/// Try to register client. Send response about result.
		/// </summary>
		/// <param name="client">client.</param>
		/// <param name="message">client's message.</param>
		private void RegisterClient(MpClient client, RegisterRequestMessage message)
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
				client.SendMessage(response);
			}
			catch (ConnectionInterruptedException)
			{
				// TODO logger
			}
			
			// close connection
			Console.WriteLine("{0}: Client disconnected. ip {1}", DateTime.Now,
					((IPEndPoint)client.tcpClient.Client.RemoteEndPoint).Address.ToString());

			try
			{
				client.Close();
			}
			catch
			{
				// TODO logger
			}
		}
	}
}
