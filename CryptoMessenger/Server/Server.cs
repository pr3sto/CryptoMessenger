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
		// logger
		private static readonly log4net.ILog log = LogHelper.GetLogger();

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
			catch (CertificateException e)
			{
				log.Info("Can't create certificate. Server will not start.");
				log.Error(e);
			}
		}

		/// <summary>
		/// Start listen to clients and process connected clients.
		/// </summary>
		public async void Start()
		{
			#region Test code

			DBoperations.Register("13", "13");
			for (int i = 0; i < 10; i++)
			{
				string login = "user" + i.ToString();
				DBoperations.Register(login, login);
			}
			for (int i = 10; i < 20; i++)
			{
				string login = "user" + i.ToString();
				DBoperations.Register(login, login);
				DBoperations.SetFriendship(false, DBoperations.GetUserId(login), DBoperations.GetUserId("13"));
			}
			for (int i = 20; i < 30; i++)
			{
				string login = "user" + i.ToString();
				DBoperations.Register(login, login);
				DBoperations.SetFriendship(false, DBoperations.GetUserId("13"), DBoperations.GetUserId(login));
			}
			for (int i = 30; i < 40; i++)
			{
				string login = "user" + i.ToString();
				DBoperations.Register(login, login);
				DBoperations.SetFriendship(true, DBoperations.GetUserId("13"), DBoperations.GetUserId(login));
			}

			#endregion

			if (isStarted) return;
			if (certificate == null) return;

			listener = new MpListener(port, certificate);
			listener.Start();
			isStarted = true;

			log.Info("Server is now listening.");

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
				catch (ConnectionInterruptedException e)
				{
					// connection with client broke
					// nothing critical, just continue
					log.Error(e);
					continue;
				}
				catch (SocketException e)
				{
					log.Info("Socket exception. Trying to restart listening.");
					log.Error(e);

					listener.Stop();
					listener = null;
					listener = new MpListener(port, certificate);
					listener.Start();
				}
				catch (ObjectDisposedException)
				{
					// the way we stop server
					break;
				}
			}

			// wait for finishing process clients
			Task.WaitAll(activeTasks.ToArray());

			log.Info("Server is now stopped listening.");
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
			log.Info(string.Format("Client connected. ip {0}",
				((IPEndPoint)client.tcpClient.Client.RemoteEndPoint).Address.ToString()));

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
				catch (ConnectionInterruptedException e)
				{
					log.Error(e);

					log.Info(string.Format("Client disconnected. ip {0}",
						((IPEndPoint)client.tcpClient.Client.RemoteEndPoint).Address.ToString()));
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
					log.Info(string.Format("Client login: {0}", message.login));

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
			catch (ConnectionInterruptedException e)
			{
				log.Error(e);
			}

			// close connection with client if client not logged in
			if (!isLoggedIn)
			{
				log.Info(string.Format("Client disconnected. ip {0}",
					((IPEndPoint)client.tcpClient.Client.RemoteEndPoint).Address.ToString()));

				try
				{
					client.Close();
				}
				catch (ConnectionInterruptedException e)
				{
					log.Error(e);
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
					log.Info(string.Format("Client registered: {0}", message.login));

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
			catch (ConnectionInterruptedException e)
			{
				log.Error(e);
			}

			// close connection
			log.Info(string.Format("Client disconnected. ip {0}",
					((IPEndPoint)client.tcpClient.Client.RemoteEndPoint).Address.ToString()));

			try
			{
				client.Close();
			}
			catch (ConnectionInterruptedException e)
			{
				log.Error(e);
			}
		}
	}
}
