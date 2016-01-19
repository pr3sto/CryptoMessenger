using System;
using System.IO;
using System.Net.Security;
using System.Reflection;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace CryptoMessenger.Net
{
	/// <summary>
	/// Problems with local certificate.
	/// </summary>
	[Serializable]
	class ClientCertificateException : Exception
	{
		public ClientCertificateException()
		{
		}

		public ClientCertificateException(string message)
			: base(message)
		{
		}

		public ClientCertificateException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}

	class SslStuff
	{
		/// <summary>
		/// Callback for the verification of the server certificate.
		/// </summary>
		/// <param name="sender">sender of this callback.</param>
		/// <param name="certificate">server's certificate.</param>
		/// <param name="chain">certificate build chain.</param>
		/// <param name="sslPolicyErrors">ssl errors.</param>
		/// <returns>true, if server has been validated.</returns>
		public static bool ServerValidationCallback(object sender, 
			X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			// TODO Add cert check
			return true;
		}


		/// <summary>
		/// Certificate selection callback.
		/// </summary>
		/// <param name="sender">sender of this callback.</param>
		/// <param name="targetHost">server's ip address.</param>
		/// <param name="localCertificates">local certificates clooection.</param>
		/// <param name="remoteCertificate">server's certificate.</param>
		/// <param name="acceptableIssuers">acceptable issuers for remote host.</param>
		/// <returns></returns>
		public static X509Certificate ClientCertificateSelectionCallback(object sender, 
			string targetHost, X509CertificateCollection localCertificates, 
			X509Certificate remoteCertificate, string[] acceptableIssuers)
		{
			return localCertificates[0];
		}

		/// <summary>
		/// Perform the client handshake.
		/// </summary>
		/// <param name="sslStream">ssl stream with server.</param>
		/// <param name="targetHost">server's ip address.</param>
		/// <exception cref="ClientCertificateException">can't get local certificate.</exception>
		public static void ClientSideHandshake(SslStream sslStream, string targetHost)
		{
			X509CertificateCollection ccertificateCollection = new X509CertificateCollection();

			try
			{
				// get certificate from cert.pfx
				byte[] embeddedCert;
				Assembly thisAssembly = Assembly.GetAssembly(typeof(Client));
				using (Stream certStream = thisAssembly.GetManifestResourceStream("CryptoMessenger.Certificate.cert.pfx"))
				{
					embeddedCert = new byte[certStream.Length];
					certStream.Read(embeddedCert, 0, (int)certStream.Length);
				}
				X509Certificate2 certificate = new X509Certificate2(embeddedCert, "", X509KeyStorageFlags.MachineKeySet);
				ccertificateCollection.Add(certificate);
			}
			catch
			{
				throw new ClientCertificateException();
			}

			// 'handshake'
			sslStream.AuthenticateAsClient(targetHost, ccertificateCollection, SslProtocols.Tls, true);
		}
	}
}
