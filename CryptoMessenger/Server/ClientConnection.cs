using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Xml.Serialization;

using MessageTypes;

namespace Server
{
	class ClientConnection
	{
		/// <summary>
		/// Send message to client.
		/// </summary>
		/// <param name="sslStream">stream with client.</param>
		/// <param name="message">client's message.</param>
		public static void SendMessage(SslStream sslStream, ResponseMessage message)
		{
			try
			{
				XmlSerializer responseSerializer = new XmlSerializer(typeof(ResponseMessage));
				responseSerializer.Serialize(sslStream, message);
			}
			catch
			{
				// TODO logger
			}
		}

		/// <summary>
		/// Reseive request from client.
		/// </summary>
		/// <param name="client">tcp client.</param>
		/// <param name="sslStream">stream with client.</param>
		/// <returns>client's message.</returns>
		public static RequestMessage ReceiveMessage(TcpClient client, SslStream sslStream)
		{
			XmlSerializer requestSerializer = new XmlSerializer(typeof(RequestMessage));

			byte[] buffer = new byte[client.ReceiveBufferSize];
			int length = sslStream.Read(buffer, 0, buffer.Length);
			MemoryStream ms = new MemoryStream(buffer, 0, length);

			return (RequestMessage)requestSerializer.Deserialize(ms);
		}
	}
}
