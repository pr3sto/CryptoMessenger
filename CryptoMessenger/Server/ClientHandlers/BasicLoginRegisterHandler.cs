using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Server.Security;

namespace Server
{
	[Serializable]
	class ClientConnectionException : Exception
	{
		public ClientConnectionException()
		{
		}

		public ClientConnectionException(string message)
			: base(message)
		{
		}

		public ClientConnectionException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}

	[Serializable]
	class ClientMessageException : Exception
	{
		public ClientMessageException()
		{
		}

		public ClientMessageException(string message)
			: base(message)
		{
		}

		public ClientMessageException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}

	public abstract class BasicLoginRegisterHandler : IDisposable
	{
		// port number
		private int port;
		// tcp listener
		private TcpListener listener;
		// token for cancel listening
		private CancellationTokenSource cts;
		// active tasks
		private List<Task> activeTasks;

		// database context
		protected LinqToDatabaseDataContext DBcontext;

		// is server started listening to clients
		public bool IsStarted { get; private set; }
		

		/// <summary>
		/// Initialize Handler, that listen to clients.
		/// </summary>
		public BasicLoginRegisterHandler(int port)
		{
			this.port = port;
			activeTasks = new List<Task>();
			DBcontext = new LinqToDatabaseDataContext();
			IsStarted = false;
		}

		/// <summary>
		/// Destructor.
		/// </summary>
		~BasicLoginRegisterHandler()
		{
			// Finalizer calls Dispose(false)
			Dispose(false);
		}

		/// <summary>
		/// Dispose members.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Free managed resources.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (DBcontext != null)
				{
					DBcontext.Dispose();
					DBcontext = null;
				}
			}
		}

		/// <summary>
		/// Start listen to clients and process connected clients.
		/// </summary>
		public async void StartAcceptClients()
		{
			if (IsStarted) return;

			listener = TcpListener.Create(port);
			listener.Start();
			cts = new CancellationTokenSource();
			IsStarted = true;

			ConsoleWriter.WriteLine("{0} is now listening.", GetType().Name);

			while (!cts.IsCancellationRequested)
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
					ConsoleWriter.WriteLine("logger-___-");

					break;
				}
				catch (SocketException)
				{
					// TODO logger
					ConsoleWriter.WriteLine("logger-___-");

					listener = null;
					listener = TcpListener.Create(port);
					listener.Start();
				}
			}

			// wait for finishing process clients
			Task.WaitAll(activeTasks.ToArray());

			ConsoleWriter.WriteLine("{0} is now stopped.", GetType().Name);
			cts.Dispose();
			IsStarted = false;
		}

		/// <summary>
		/// Stop accepting clients.
		/// </summary>
		public void StopAcceptClients()
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
		private async Task ProcessClient(TcpClient client)
		{
			ConsoleWriter.WriteLine("client connected. ip {0}", 
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

						// recieve user's login and password
						string clientData = RecieveDataFromClient(sslStream, client.ReceiveBufferSize);

						// try login/register
						ProcessData(sslStream, clientData);
					}
				}
				catch (ClientConnectionException)
				{
					// TODO logger
					ConsoleWriter.WriteLine("logger-___-");
				}
				catch (ClientMessageException)
				{
					// TODO logger
					ConsoleWriter.WriteLine("logger-___-");
				}
				finally
				{
					ConsoleWriter.WriteLine("client disconnected. ip {0}", 
						((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
					client.Close();
				}
			});
		}

		/// <summary>
		/// Recieve string from clients stream.
		/// </summary>
		/// <param name="sslStream">connected client's ssl stream.</param>
		/// <param name="receiveBufferSize">client's receive buffer size</param>
		/// <returns>recieved string.</returns>
		/// <exception cref="ClientConnectionException">can't read from client's stream.</exception>
		/// <exception cref="ClientMessageException">client's message is incorrect</exception>
		private string RecieveDataFromClient(SslStream sslStream, int receiveBufferSize)
		{
			byte[] bytes;  // bytes from client stream
			int bytesRead; // number of bytes

			try
			{
				bytes = new byte[receiveBufferSize];
				bytesRead = sslStream.Read(bytes, 0, receiveBufferSize);
			}
			catch
			{
				throw new ClientConnectionException();
			}

			string data;

			try
			{
				Array.Resize(ref bytes, bytesRead);
				data = Encoding.UTF8.GetString(bytes);
			}
			catch
			{
				throw new ClientMessageException();
			}

			return data;
		}

		/// <summary>
		/// Process clients message. Send response to client about required operation.
		/// </summary>
		/// <param name="sslStream">connected client's ssl stream.</param>
		/// <param name="data">Clients message.</param>
		private void ProcessData(SslStream sslStream, string data)
		{
			string response;
			string[] login_password = data.Split(' ');

			if (login_password.Length != 2 ||
				string.IsNullOrEmpty(login_password[0]) ||
				string.IsNullOrEmpty(login_password[1]) ||
				login_password[0].Length > 30 ||
				!DoRequiredOperation(login_password[0], login_password[1]))
			{
				response = "ERROR";
			}
			else
			{
				response = "OK";
			}

			try
			{
				SendResponseToClient(sslStream, response);
			}
			catch (ClientConnectionException)
			{
				// TODO logger
				ConsoleWriter.WriteLine("logger-___-");
			}
		}

		/// <summary>
		/// Send response message to client.
		/// </summary>
		/// <param name="sslStream">connected client's ssl stream.</param>
		/// <param name="response">Response message.</param>
		/// <exception cref="ClientConnectionException">can't write to client's stream.</exception>
		private void SendResponseToClient(SslStream sslStream, string response)
		{
			if (string.IsNullOrEmpty(response)) response = "ERROR";
			byte[] response_bytes = Encoding.UTF8.GetBytes(response);

			try
			{
				sslStream.Write(response_bytes, 0, response_bytes.Length);
			}
			catch
			{
				throw new ClientConnectionException();
			}
		}

		/// <summary>
		/// Do required operation (login or register).
		/// </summary>
		/// <param name="_login">user's login.</param>
		/// <param name="_password">user's password.</param>
		/// <returns>true, if operation had success.</returns>
		protected abstract bool DoRequiredOperation(string _login, string _password);
	}
}
