using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CryptoMessenger.ClientServerCommunication
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
		/// Send login and password to server.
		/// Resieve response from server.
		/// </summary>
		/// <param name="login">users login.</param>
		/// <param name="password">users password.</param>
		/// <param name="port">port number.</param>
		/// <returns>true, if servers response is 'OK'.</returns>
		/// <exception cref="ServerConnectionException"></exception>
		private async Task<bool> SendDataToServerAndRecieveResponse(string login, string password, int port)
		{
			// response from server
			string response;

			try
			{
				client = new TcpClient();
				client.Connect(ip, port);
			
				string data = login + " " + password;

				// send login and password to server
				byte[] bytes = Encoding.UTF8.GetBytes(data);
				await client.GetStream().WriteAsync(bytes, 0, bytes.Length);

				// server response
				bytes = new byte[client.ReceiveBufferSize];
				int bytesRead = await client.GetStream().ReadAsync(bytes, 0, client.ReceiveBufferSize);
				byte[] response_bytes = new byte[bytesRead];
				Array.Copy(bytes, response_bytes, bytesRead);
				response = Encoding.UTF8.GetString(response_bytes);

				client.Close();
			}
			catch
			{
				throw new ServerConnectionException();
			}

			// process response
			if ("OK".Equals(response))
				return true;
			else if ("ERROR".Equals(response))
				return false;

			return false;
		}
	}
}
