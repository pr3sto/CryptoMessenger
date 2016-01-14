using System;
using System.IO;
using System.Net.Security;
using System.Reflection;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Server.Security
{
	[Serializable]
	class ServerCertificateException : Exception
	{
		public ServerCertificateException()
		{
		}

		public ServerCertificateException(string message)
			: base(message)
		{
		}

		public ServerCertificateException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}

	class SslStuff
	{
		/// <summary>
		/// Callback for the verification of the client certificate.
		/// </summary>
		/// <param name="sender">sender of this callback.</param>
		/// <param name="certificate">client's certificate.</param>
		/// <param name="chain">certificate build chain.</param>
		/// <param name="sslPolicyErrors">ssl errors.</param>
		/// <returns>true, if client has been validated.</returns>
		public static bool ClientValidationCallback(object sender,
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
		/// <param name="remoteCertificate">client's certificate.</param>
		/// <param name="acceptableIssuers">acceptable issuers for remote client.</param>
		/// <returns></returns>
		public static X509Certificate ServerCertificateSelectionCallback(object sender, 
			string targetHost, X509CertificateCollection localCertificates, 
			X509Certificate remoteCertificate, string[] acceptableIssuers)
		{
			return localCertificates[0];
		}

		/// <summary>
		/// Perform the server handshake.
		/// </summary>
		/// <param name="sslStream">ssl stream with client.</param>
		/// <exception cref="ServerCertificateException">can't get local certificate.</exception>
		public static void ServerSideHandshake(SslStream sslStream)
		{
			X509Certificate2 certificate;

			try
			{
				// get certificate from cert.pfx
				byte[] embeddedCert;
				Assembly thisAssembly = Assembly.GetAssembly(typeof(SslStuff));
				using (Stream certStream = thisAssembly.GetManifestResourceStream("Server.Certificate.cert.pfx"))
				{
					embeddedCert = new byte[certStream.Length];
					certStream.Read(embeddedCert, 0, (int)certStream.Length);
				}
				certificate = new X509Certificate2(embeddedCert, "", X509KeyStorageFlags.MachineKeySet);
			}
			catch
			{
				throw new ServerCertificateException();
			}

			// 'handshake'
			sslStream.AuthenticateAsServer(certificate, true, SslProtocols.Tls, true);
		}
	}
}
