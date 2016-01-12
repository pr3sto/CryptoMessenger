using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Server.SslStuff;

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

	public abstract class BasicClientHandler
	{
		// port number
		private int port;
		// tcp listener
		private TcpListener listener;
		// token for cancel listening
		CancellationTokenSource cts;
		// active tasks
		List<Task> activeTasks;
		// is server started listening to clients
		public bool IsStarted { get; private set; }

		/// <summary>
		/// Initialize Handler, that listen to clients.
		/// </summary>
		public BasicClientHandler(int port)
		{
			this.port = port;
			activeTasks = new List<Task>();
			IsStarted = false;
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

			Console.WriteLine("{0} is now listening.", GetType().Name);

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
					Console.WriteLine("logger-___-");

					break;
				}
				catch (SocketException)
				{
					// TODO logger
					Console.WriteLine("logger-___-");

					listener = null;
					listener = TcpListener.Create(port);
					listener.Start();
				}
			}

			// wait for finishing processing clients
			Task.WaitAll(activeTasks.ToArray());

			Console.WriteLine("{0} is now stopped.", GetType().Name);
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
			Console.WriteLine("client connected. ip {0}", ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());

			await Task.Run(() =>
			{
				try
				{
					RemoteCertificateValidationCallback validationCallback =
						new RemoteCertificateValidationCallback(CertificateCallback.ClientValidationCallback);

					LocalCertificateSelectionCallback selectionCallback =
						new LocalCertificateSelectionCallback(CertificateCallback.ServerCertificateSelectionCallback);

					SslStream sslStream = new SslStream(client.GetStream(), true, 
						validationCallback, selectionCallback, EncryptionPolicy.RequireEncryption);

					SslHandshake.ServerSideHandshake(sslStream);

					string clientData = RecieveDataFromClient(sslStream, client.ReceiveBufferSize);
					ProcessData(sslStream, clientData);
				}
				catch (ClientConnectionException)
				{
					// TODO logger
					Console.WriteLine("logger-___-");
				}
				catch (ClientMessageException)
				{
					// TODO logger
					Console.WriteLine("logger-___-");
				}
				finally
				{
					Console.WriteLine("client disconnected. ip {0}", ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
					client.Close();
				}
			});
		}

		/// <summary>
		/// Recieve string from clients stream.
		/// </summary>
		/// <param name="client">Connected client.</param>
		/// <returns>Recieved string.</returns>
		/// <exception cref="ClientConnectionException"></exception>
		/// <exception cref="ClientMessageException"></exception>
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
				// data from client in bytes
				Array.Resize<byte>(ref bytes, bytesRead);
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
		/// <param name="client">Connected client.</param>
		/// <param name="data">Clients message.</param>
		private void ProcessData(SslStream sslStream, string data)
		{
			string response;
			string[] login_password = data.Split(' ');

			if (login_password.Length != 2 ||
				string.IsNullOrEmpty(login_password[0]) ||
				string.IsNullOrEmpty(login_password[1]) ||
				!DoRequiredOperation(login_password))
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
				Console.WriteLine("logger-___-");
			}
		}

		/// <summary>
		/// Send response message to client.
		/// </summary>
		/// <param name="client">Connected client.</param>
		/// <param name="response">Response message.</param>
		/// <exception cref="ClientConnectionException"></exception>
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
		/// <param name="login_password">Array of users login and password.</param>
		/// <returns>true, if operation had success.</returns>
		protected abstract bool DoRequiredOperation(string[] login_password);
	}
}
