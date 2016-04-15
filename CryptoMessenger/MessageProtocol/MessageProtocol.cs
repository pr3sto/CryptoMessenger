using System;
using System.IO;
using System.IO.Compression;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml.Serialization;

using MessageProtocol.MessageTypes;

namespace MessageProtocol
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
	public class MpClient
	{
		/// <summary>
		/// Underlying tcp client.
		/// </summary>
		public TcpClient tcpClient { get; set; }

		/// <summary>
		/// Ssl stream with connected remote host.
		/// </summary>
		public SslStream sslStream { get; set; }

		/// <summary>
		/// Connects the client to a remote host using the specified 
		/// IP address and port number as an asynchronous operation.
		/// </summary>
		/// <param name="host">host.</param>
		/// <param name="port">port.</param>
		/// <param name="certificate">certificate.</param>
		/// <exception cref="ConnectionInterruptedException"></exception>
		public async Task ConnectAsync(string host, int port, X509Certificate2 certificate)
		{
			try
			{
				// connect via tcp
				tcpClient = new TcpClient();
				await tcpClient.ConnectAsync(host, port);

				await Task.Run(() =>
				{
					// create ssl stream
					sslStream = new SslStream(tcpClient.GetStream(), true,
						Ssl.ServerValidationCallback,
						Ssl.ClientCertificateSelectionCallback,
						EncryptionPolicy.RequireEncryption);

					// handshake
					Ssl.ClientSideHandshake(certificate, sslStream, host);
				});
			}
			catch (CertificateException e)
			{
				throw new ConnectionInterruptedException("Connection failed. Reason: " + e.Message);
			}
			catch
			{
				throw new ConnectionInterruptedException("Connection failed.");
			}
		}

		/// <summary>
		/// Connects the client to a remote host using the specified 
		/// IP address and port number as an asynchronous operation.
		/// </summary>
		/// <param name="host">host.</param>
		/// <param name="port">port.</param>
		/// <param name="certificate">certificate.</param>
		/// <param name="milisecondsTimeout">wait for connection during this period of time.</param>
		/// <exception cref="ConnectionInterruptedException"></exception>
		public async Task ConnectWithTimeoutAsync(string host, int port, X509Certificate2 certificate, int milisecondsTimeout)
		{
			try
			{
				// connect via tcp
				tcpClient = new TcpClient();

				await Task.Run(() =>
				{
					// timeout for waiting connection
					if (!tcpClient.ConnectAsync(host, port).Wait(milisecondsTimeout))
						throw new TimeoutException();
				});

				await Task.Run(() =>
				{
					// create ssl stream
					sslStream = new SslStream(tcpClient.GetStream(), true,
						Ssl.ServerValidationCallback,
						Ssl.ClientCertificateSelectionCallback,
						EncryptionPolicy.RequireEncryption);

					// handshake
					Ssl.ClientSideHandshake(certificate, sslStream, host);
				});
			}
			catch (TimeoutException)
			{
				throw new ConnectionInterruptedException("Connection failed. Reason: timeout.");
			}
			catch (CertificateException e)
			{
				throw new ConnectionInterruptedException("Connection failed. Reason: " + e.Message);
			}
			catch
			{
				throw new ConnectionInterruptedException("Connection failed.");
			}
		}

		/// <summary>
		/// Disposes this MpClient instance and requests that the underlying ssl connection be closed.
		/// </summary>
		/// <exception cref="ConnectionInterruptedException"></exception>
		public void Close()
		{
			try
			{
				tcpClient.Client.Shutdown(SocketShutdown.Both);
			}
			catch
			{
				throw new ConnectionInterruptedException("Error closing socket.");
			}
			finally
			{
				sslStream.Dispose();
				tcpClient.Close();
			}
		}

		/// <summary>
		/// Send message over ssl stream.
		/// </summary>
		/// <param name="message">message to send.</param>
		/// <exception cref="ConnectionInterruptedException"></exception>
		public void SendMessage(Message message)
		{
			try
			{
				using (var stream = new MemoryStream())
				{
					// compress data
					using (var deflateStream = new DeflateStream(stream, CompressionLevel.Optimal))
					{
						var serializer = new XmlSerializer(typeof(Message));
						serializer.Serialize(deflateStream, message);
					}

					byte[] BinaryData = stream.ToArray();
					byte[] DataLength = BitConverter.GetBytes(BinaryData.Length);

					// send number of bytes in message
					sslStream.Write(DataLength);
					// send message
					sslStream.Write(BinaryData);
				}	
			}
			catch
			{
				throw new ConnectionInterruptedException("Error sending data.");
			}
			
		}

		/// <summary>
		/// Reseive message.
		/// </summary>
		/// <returns>received message.</returns>
		/// <exception cref="ConnectionInterruptedException"></exception>
		public Message ReceiveMessage()
		{
			try
			{
				byte[] buffer = new byte[4];
				for (int i = 0; i < 4; i++)
					buffer[i] = (byte)sslStream.ReadByte();

				// number of bytes in message
				int num_of_bytes = BitConverter.ToInt32(buffer, 0);

				// read data
				buffer = new byte[num_of_bytes];
				for (int i = 0; i < num_of_bytes; i++)
					buffer[i] = (byte)sslStream.ReadByte();

				// deserialize message
				using (var stream = new MemoryStream(buffer))
				{
					using (var deflateStream = new DeflateStream(stream, CompressionMode.Decompress))
					{
						var serializer = new XmlSerializer(typeof(Message));
						return (Message)serializer.Deserialize(deflateStream);
					}
				}
			}
			catch
			{
				throw new ConnectionInterruptedException("Error receiving data.");
			}
		}

		/// <summary>
		/// Reseive message asynchronously.
		/// </summary>
		/// <returns>received message.</returns>
		/// <exception cref="ConnectionInterruptedException"></exception>
		public async Task<Message> ReceiveMessageAsync()
		{
			return await Task.Run(() =>
			{
				return ReceiveMessage();
			});
		}
	}

	/// <summary>
	/// Listens for connections from MP network clients.
	/// </summary>
	public class MpListener
	{
		private TcpListener listener;
		private X509Certificate2 certificate;

		public MpListener(int port, X509Certificate2 certificate)
		{
			listener = TcpListener.Create(port);
			this.certificate = certificate;
		}

		/// <summary>
		/// Starts listening for incoming connection requests.
		/// </summary>
		/// <exception cref="SocketException"></exception>
		public void Start()
		{
			listener.Start();
		}

		/// <summary>
		/// Closes the listener.
		/// </summary>
		/// <exception cref="SocketException"></exception>
		public void Stop()
		{
			listener.Stop();
			listener = null;
		}

		/// <summary>
		/// Accepts a pending connection request as an asynchronous operation. 
		/// </summary>
		/// <exception cref="SocketException"></exception>
		/// <exception cref="ConnectionInterruptedException"></exception>
		public async Task<MpClient> AcceptMpClientAsync()
		{
			// accept tcp client
			TcpClient tcpClient = await listener.AcceptTcpClientAsync();

			return await Task.Run(() =>
			{
				try
				{
					// create ssl stream with client
					SslStream sslStream = new SslStream(tcpClient.GetStream(), true,
						Ssl.ClientValidationCallback,
						Ssl.ServerCertificateSelectionCallback,
						EncryptionPolicy.RequireEncryption);

					// handshake
					Ssl.ServerSideHandshake(certificate, sslStream);

					// create MP client
					MpClient mpClient = new MpClient();
					mpClient.tcpClient = tcpClient;
					mpClient.sslStream = sslStream;

					return mpClient;
				}
				catch (CertificateException e)
				{
					throw new ConnectionInterruptedException("Connection failed. Reason: " + e.Message);
				}
				catch
				{
					throw new ConnectionInterruptedException("Connection failed.");
				}
			});
		}
	}
}
