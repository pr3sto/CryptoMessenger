using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

using MessageTypes;

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
		/// Connect to server.
		/// </summary>
		/// <exception cref="ServerConnectionException">connection problems.</exception>
		/// <exception cref="ClientCertificateException">can't get local certificate.</exception>
		private void Connect()
		{
			try
			{
				client = new TcpClient();

				// timeout for waiting connection
				if (!client.ConnectAsync(ip, port).Wait(5000))
					throw new TimeoutException();

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
		/// Listen for messages from server.
		/// </summary>
		public async void Listen()
		{
			XmlSerializer requestSerializer = new XmlSerializer(typeof(RequestMessage));
			XmlSerializer responseSerializer = new XmlSerializer(typeof(ResponseMessage));

			await Task.Run(() =>
			{
				try
				{
					while (true)
					{
						byte[] buffer = new byte[client.ReceiveBufferSize];
						int length = sslStream.Read(buffer, 0, buffer.Length);
						MemoryStream ms = new MemoryStream(buffer, 0, length);
						// server's incoming message
						// RequestMessage message = (RequestMessage)requestSerializer.Deserialize(ms);
					}
				}
				catch
				{
					
				}
			});
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
			Connect();

			LoginRequestMessage message = new LoginRequestMessage
			{
				login = _login,
				password = _password
			};
			LoginResponseMessage serverResp = (LoginResponseMessage) await SendMessage(message);

			// don't disconnect if login success
			if (!LoginRegisterResponse.SUCCESS.Equals(serverResp.response))
				Disconnect();

			return serverResp.response;
		}

		/// <summary>
		/// Logout from server.
		/// </summary>
		/// <exception cref="ServerConnectionException">connection problems.</exception>
		public async Task Logout()
		{
			await SendMessage(new LogoutRequestMessage());
			Disconnect();
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
			Connect();

			RegisterRequestMessage message = new RegisterRequestMessage
			{
				login = _login,
				password = _password
			};
			RegisterResponseMessage serverResp = (RegisterResponseMessage)await SendMessage(message);
			
			Disconnect();

			return serverResp.response;
		}

		/// <summary>
		/// Send message to server;
		/// resieve response from server.
		/// </summary>
		/// <param name="message">message with user's data.</param>
		/// <returns>server's response.</returns>
		/// <exception cref="ServerConnectionException">connection problems.</exception>
		/// <exception cref="ClientCertificateException">can't get local certificate.</exception>
		private async Task<ResponseMessage> SendMessage(RequestMessage message)
		{
			// asynchronous communicate with server
			return await Task.Run(() => {

				try
				{
					XmlSerializer requestSerializer = new XmlSerializer(typeof(RequestMessage));
					XmlSerializer responseSerializer = new XmlSerializer(typeof(ResponseMessage));

					// send login and password to server
					requestSerializer.Serialize(sslStream, message);

					// server response
					byte[] buffer = new byte[client.ReceiveBufferSize];
					int length = sslStream.Read(buffer, 0, buffer.Length);
					MemoryStream ms = new MemoryStream(buffer, 0, length);

					return (ResponseMessage)responseSerializer.Deserialize(ms);
				}
				catch
				{
					throw new ServerConnectionException();
				}

			});
		}
	}
}
