using System;
using System.IO;
using System.Net.Security;
using System.Threading.Tasks;
using System.Xml.Serialization;

using MessageProtocol.MessageTypes;

namespace MessageProtocol.Client
{
	/// <summary>
	/// Connection problems.
	/// </summary>
	[Serializable]
	public class ConnectionInterruptedException : Exception
	{
		public ConnectionInterruptedException()
		{
		}

		public ConnectionInterruptedException(string message)
			: base(message)
		{
		}

		public ConnectionInterruptedException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}

	/// <summary>
	/// Message protocol client for sending and receiving messages.
	/// </summary>
	public static class MpClient
	{
		private static readonly int ChunkSize = 1024;

		/// <summary>
		/// Send message over ssl stream.
		/// </summary>
		/// <param name="sslStream">ssl stream.</param>
		/// <param name="message">message to send.</param>
		/// <exception cref="ConnectionInterruptedException"></exception>
		public static void SendMessage(SslStream sslStream, Message message)
		{
			try
			{
				int num_of_chunks = 0;
				var serializer = new XmlSerializer(typeof(Message));

				// number of chunks
				using (var stream = new MemoryStream())
				{
					serializer.Serialize(stream, message);
					num_of_chunks = (int)stream.Length / ChunkSize + 1;
				}

				// send number
				sslStream.Write(BitConverter.GetBytes(num_of_chunks));
				//send message
				serializer.Serialize(sslStream, message);
			}
			catch
			{
				throw new ConnectionInterruptedException();
			}
			
		}

		/// <summary>
		/// Reseive message from ssl stream.
		/// </summary>
		/// <param name="sslStream">ssl stream.</param>
		/// <returns>received message.</returns>
		/// <exception cref="ConnectionInterruptedException"></exception>
		public static Message ReceiveMessage(SslStream sslStream)
		{
			try
			{
				byte[] buffer = new byte[32];
				int length = sslStream.Read(buffer, 0, buffer.Length);

				// number of chunks
				Array.Resize(ref buffer, length);
				int num_of_chunks = BitConverter.ToInt32(buffer, 0);

				// read data
				buffer = new byte[num_of_chunks * ChunkSize];
				length = 0;
				for (int i = 0; i < num_of_chunks; i++)
					length += sslStream.Read(buffer, i * ChunkSize, ChunkSize);

				// deserialize message
				var serializer = new XmlSerializer(typeof(Message));
				var ms = new MemoryStream(buffer, 0, length);
				return (Message)serializer.Deserialize(ms);
			}
			catch
			{
				throw new ConnectionInterruptedException();
			}
		}

		/// <summary>
		/// Reseive message from ssl stream asynchronously.
		/// </summary>
		/// <param name="sslStream">ssl stream.</param>
		/// <returns>received message.</returns>
		/// <exception cref="ConnectionInterruptedException"></exception>
		public static async Task<Message> ReceiveMessageAsync(SslStream sslStream)
		{
			return await Task.Run(() =>
			{
				return ReceiveMessage(sslStream);
			});
		}
	}
}
