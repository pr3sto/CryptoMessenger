using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using Server.Security;
using MessageTypes;
using System.Xml.Serialization;

namespace Server
{
	class Server
	{
		// port number
		private int port;
		// tcp listener
		private TcpListener listener;
		// token for cancel listening
		private CancellationTokenSource cts;
		// active tasks
		private List<Task> activeTasks;
		// is server started listening to clients
		public bool IsStarted { get; private set; }

		/// <summary>
		/// Initialize server, that listen to clients.
		/// </summary>
		public Server(int port)
		{
			this.port = port;
			activeTasks = new List<Task>();
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
			cts = new CancellationTokenSource();
			IsStarted = true;

			Console.WriteLine("{0} is now listening.", port);

			while (!cts.IsCancellationRequested)
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

			Console.WriteLine("{0} is now stopped.", port);
			cts.Dispose();
			IsStarted = false;
		}

		/// <summary>
		/// Stop accepting clients.
		/// </summary>
		public void Stop()
		{
			if (!IsStarted) return;

			listener.Stop();
			listener = null;
			cts.Cancel();
		}

		/// <summary>
		/// Process connected client.
		/// </summary>
		/// <param name="client">Connected client.</param>
		private async Task ProcessConnection(TcpClient client)
		{
			Console.WriteLine("{0}: client connected. ip {1}", port,
				((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());

			await Task.Run(() =>
			{
				try
				{
					using (SslStream sslStream = new SslStream(client.GetStream(), true,
						SslStuff.ClientValidationCallback,
						SslStuff.ServerCertificateSelectionCallback,
						EncryptionPolicy.RequireEncryption))
					{
						// handshake
						SslStuff.ServerSideHandshake(sslStream);

						XmlSerializer messageSerializer = new XmlSerializer(typeof(LoginRegisterMessage));

						// recieve user's login and password
						LoginRegisterMessage message = (LoginRegisterMessage)messageSerializer.Deserialize(sslStream);

						// try login/register
						ProcessData(client, sslStream, message);
					}
				}
				catch
				{
					// TODO logger
					Console.WriteLine("logger-___-");
				}
				finally
				{
					Console.WriteLine("{0}: client disconnected. ip {1}", port,
						((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
					client.Close();
				}
			});
		}

		/// <summary>
		/// Process clients message. Send response to client about required operation.
		/// </summary>
		/// <param name="client">Tcp client.</param>
		/// <param name="sslStream">connected client's ssl stream.</param>
		/// <param name="message">Client's message.</param>
		private void ProcessData(TcpClient client, SslStream sslStream, LoginRegisterMessage message)
		{
			LoginRegisterResponseMessage response;
			
			if (string.IsNullOrEmpty(message.login) ||
				string.IsNullOrEmpty(message.password) ||
				message.login.Length > 30)
			{
				response = new LoginRegisterResponseMessage { response = LoginRegisterResponse.ERROR };
			}
			else if (MessageType.LOGIN.Equals(message.type))
			{
				if (Login(message.login, message.password))
					response = new LoginRegisterResponseMessage { response = LoginRegisterResponse.SUCCESS };
				else
					response = new LoginRegisterResponseMessage { response = LoginRegisterResponse.FAIL };
			}
			else if (MessageType.REGISTER.Equals(message.type))
			{
				if (Register(message.login, message.password))
					response = new LoginRegisterResponseMessage { response = LoginRegisterResponse.SUCCESS };
				else
					response = new LoginRegisterResponseMessage { response = LoginRegisterResponse.FAIL };
			}
			else
			{
				response = new LoginRegisterResponseMessage { response = LoginRegisterResponse.ERROR };
			}

			try
			{
				XmlSerializer responseSerializer = new XmlSerializer(typeof(LoginRegisterResponseMessage));
				responseSerializer.Serialize(sslStream, response);
				client.Client.Shutdown(SocketShutdown.Send);
			}
			catch
			{
				// TODO logger
				Console.WriteLine("logger-___-");
			}
		}

		/// <summary>
		/// Do login.
		/// </summary>
		/// <param name="_login">user's login.</param>
		/// <param name="_password">user's password.</param>
		/// <returns>true, if operation had success.</returns>
		private bool Login(string _login, string _password)
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
							return true;
						else
							return false;
					}

					return false; // something wrong
				}
				else
				{
					// user not registered
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
						Console.WriteLine("New user: {0}", _login);
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
	}
}
