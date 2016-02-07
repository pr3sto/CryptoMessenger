using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

using MessageTypes;

namespace Server
{
	/// <summary>
	/// Contains methods to communicate with client.
	/// </summary>
	class ClientConnection
	{
		/// <summary>
		/// Send message to client.
		/// </summary>
		/// <param name="sslStream">stream with client.</param>
		/// <param name="message">client's message.</param>
		public static void SendMessage(SslStream sslStream, Message message)
		{
			try
			{
				using (var stream = new MemoryStream())
				{
					var requestSerializer = new XmlSerializer(typeof(Message));
					requestSerializer.Serialize(stream, message);

					// number of 1024b chunks
					int count = (int)stream.Length / 1024 + 1;

					// send count
					sslStream.Write(BitConverter.GetBytes(count));
					//send message
					requestSerializer.Serialize(sslStream, message);
				}
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
		public static Message ReceiveMessage(TcpClient client, SslStream sslStream)
		{
			var requestSerializer = new XmlSerializer(typeof(Message));

			byte[] buffer = new byte[1000000];
			int length = sslStream.Read(buffer, 0, buffer.Length);

			// number of 1024b chunks
			Array.Resize(ref buffer, length);
			int count = BitConverter.ToInt32(buffer, 0);

			// read data
			buffer = new byte[1000000];
			length = 0;
			for (int i = 0; i < count; i++)
				length += sslStream.Read(buffer, i * 1024, 1024);

			// deserialize message
			var ms = new MemoryStream(buffer, 0, length);
			return (Message)requestSerializer.Deserialize(ms);
		}
	}
}
