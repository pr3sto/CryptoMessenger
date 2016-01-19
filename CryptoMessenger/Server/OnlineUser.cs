using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;

namespace Server
{
	/// <summary>
	/// Online user data.
	/// </summary>
	class OnlineUser : IDisposable
	{
		private bool disposed;

		// client's id in db
		public int id { get; private set; }
		// client's login
		public string login { get; private set; }

		// client
		public TcpClient client { get; private set; }
		// ssl stream with connected client
		public SslStream sslStream { get; private set; }

		/// <summary>
		/// Create instance of user.
		/// </summary>
		/// <param name="id">client's id in db.</param>
		/// <param name="login">client's login.</param>
		/// <param name="client">tcp client.</param>
		/// <param name="sslStream">ssl stream with connected client.</param>
		public OnlineUser(int id, string login, TcpClient client, SslStream sslStream)
		{
			this.id = id;
			this.login = login;
			this.client = client;
			this.sslStream = sslStream;
		}

		public void Dispose()
		{
			if (disposed) return;

			Console.WriteLine(" - client disconnected. ip {0}",
				((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());

			try
			{
				client.Client.Shutdown(SocketShutdown.Both);
			}
			catch
			{
				// TODO logger
				Console.WriteLine("logger-___-");
			}
			finally
			{
				sslStream.Dispose();
				client.Close();
				disposed = true;
			}
		}
	}
}
