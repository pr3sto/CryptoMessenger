using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
		// port number for login
		private int loginPort;
		// port number for register
		private int registerPort;
		// servers ip addres
		private string ip;
		// client
		private TcpClient client;

		/// <summary>
		/// Initialize Client.
		/// </summary>
		public Client()
		{
			// get data from config.cfg
			XDocument doc = XDocument.Load("config.cfg");

			var _ip = doc.Descendants("ip");
			var _logPort = doc.Descendants("loginPort");
			var _regPort = doc.Descendants("registerPort");

			ip = "";
			foreach (var i in _ip) ip = i.Value;

			loginPort = 0;
			foreach (var i in _logPort) loginPort = int.Parse(i.Value);

			registerPort = 0;
			foreach (var i in _regPort) registerPort = int.Parse(i.Value);
		}

		/// <summary>
		/// Try to login into user account.
		/// </summary>
		/// <param name="login">users login.</param>
		/// <param name="password">users password.</param>
		/// <returns>true, if login success.</returns>
		public async Task<bool> Login(string login, string password)
		{
			return await SendDataToServerAndRecieveResponse(login, password, loginPort);
		}

		/// <summary>
		/// Try to register client on the server.
		/// </summary>
		/// <param name="login">users login.</param>
		/// <param name="password">users password.</param>
		/// <returns>true, if registration success.</returns>
		public async Task<bool> Register(string login, string password)
		{
			return await SendDataToServerAndRecieveResponse(login, password, registerPort);
		}

		/// <summary>
		/// Send login and password to server;
		/// resieve response from server.
		/// </summary>
		/// <param name="login">users login.</param>
		/// <param name="password">users password.</param>
		/// <param name="port">port number.</param>
		/// <returns>true, if servers response is 'OK'.</returns>
		/// <exception cref="ServerConnectionException">Connection problems.</exception>
		/// <exception cref="ClientCertificateException">Can't get local certificate.</exception>
		private async Task<bool> SendDataToServerAndRecieveResponse(string login, string password, int port)
		{
			// response from server
			string response;

			try
			{
				// try to connect to server during 5 seconds
				using (client = new TcpClient())
				{
					IAsyncResult ar = client.BeginConnect(ip, port, null, null);
					System.Threading.WaitHandle wh = ar.AsyncWaitHandle;
					try
					{
						if (!ar.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5), false))
						{
							client.Close();
							throw new TimeoutException();
						}

						client.EndConnect(ar);
					}
					finally
					{
						wh.Close();
					}
				}

				string data = login + " " + password;

				SslStream sslStream = new SslStream(client.GetStream(), true,
					SslStuff.ServerValidationCallback, SslStuff.ClientCertificateSelectionCallback, 
					EncryptionPolicy.RequireEncryption);

				// handshake
				SslStuff.ClientSideHandshake(sslStream, ip);

				// send login and password to server
				byte[] bytes = Encoding.UTF8.GetBytes(data);
				await sslStream.WriteAsync(bytes, 0, bytes.Length);

				// server response
				bytes = new byte[client.ReceiveBufferSize];
				int bytesRead = await sslStream.ReadAsync(bytes, 0, client.ReceiveBufferSize);
				Array.Resize(ref bytes, bytesRead);
				response = Encoding.UTF8.GetString(bytes);
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

			// process response
			if ("OK".Equals(response))
				return true;
			else 
				return false;
		}
	}
}
