using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using MessageTypes;

namespace CryptoMessenger.Net
{
	[Serializable]
	class ServerConnectionException : Exception
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

	class Client
	{
		// server's port 
		private int port;
		// servers ip addres
		private string ip;
		// client
		private TcpClient client;

		/// <summary>
		/// Initialize Client.
		/// </summary>
		public Client()
		{
			// get data from connection.cfg
			XDocument doc = XDocument.Load("connection.cfg");

			var _ip = doc.Descendants("ip");
			var _port = doc.Descendants("port");

			ip = "";
			foreach (var i in _ip) ip = i.Value;

			port = 0;
			foreach (var i in _port) port = int.Parse(i.Value);
		}

		/// <summary>
		/// Try to login into user account.
		/// </summary>
		/// <param name="_login">users login.</param>
		/// <param name="_password">users password.</param>
		/// <returns>server's response.</returns>
		public async Task<LoginRegisterResponseMessage> Login(string _login, string _password)
		{
			LoginRegisterMessage message = new LoginRegisterMessage
			{
				type = MessageType.LOGIN,
				login = _login,
				password = _password
			};

			return await SendMessageToServerAndRecieveResponse(message);
		}

		/// <summary>
		/// Try to register client on the server.
		/// </summary>
		/// <param name="_login">users login.</param>
		/// <param name="_password">users password.</param>
		/// <returns>server's response.</returns>
		public async Task<LoginRegisterResponseMessage> Register(string _login, string _password)
		{
			LoginRegisterMessage message = new LoginRegisterMessage
			{
				type = MessageType.REGISTER,
				login = _login,
				password = _password
			};

			return await SendMessageToServerAndRecieveResponse(message);
		}

		/// <summary>
		/// Send message to server;
		/// resieve response from server.
		/// </summary>
		/// <param name="message">message with user's data.</param>
		/// <returns>server's response.</returns>
		/// <exception cref="ServerConnectionException">connection problems.</exception>
		/// <exception cref="ClientCertificateException">can't get local certificate.</exception>
		private async Task<LoginRegisterResponseMessage> SendMessageToServerAndRecieveResponse(LoginRegisterMessage message)
		{
			try
			{
				client = new TcpClient();

				// timeout for waiting connection
				if (!client.ConnectAsync(ip, port).Wait(5000))
					throw new TimeoutException();

				// asynchronous communicate with server
				return await Task.Run(() => { 

					using (SslStream sslStream = new SslStream(client.GetStream(), true,
						SslStuff.ServerValidationCallback, SslStuff.ClientCertificateSelectionCallback,
						EncryptionPolicy.RequireEncryption))
					{
						// handshake
						SslStuff.ClientSideHandshake(sslStream, ip);

						XmlSerializer messageSerializer = new XmlSerializer(typeof(LoginRegisterMessage));
						XmlSerializer responseSerializer = new XmlSerializer(typeof(LoginRegisterResponseMessage));

						// send login and password to server
						messageSerializer.Serialize(sslStream, message);
						client.Client.Shutdown(SocketShutdown.Send);

						// server response
						return (LoginRegisterResponseMessage)responseSerializer.Deserialize(sslStream);
					}

				});
			}
			catch (ClientCertificateException)
			{
				throw;
			}
			catch
			{
				throw new ServerConnectionException();
			}
			finally
			{
				client.Close();
			}
		}
	}
}
