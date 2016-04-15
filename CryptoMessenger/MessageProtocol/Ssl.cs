using System;
using System.IO;
using System.Net.Security;
using System.Reflection;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace MessageProtocol
{
	/// <summary>
	/// Problems with certificate.
	/// </summary>
	[Serializable]
	public class CertificateException : Exception
	{
		public CertificateException()
		{
		}

		public CertificateException(string message)
			: base(message)
		{
		}

		public CertificateException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}


	/// <summary>
	/// Things needed for organizing ssl stream 
	/// between client and server.
	/// </summary>
	static class Ssl
	{
		#region Client

		/// <summary>
		/// Callback for the verification of the server certificate.
		/// </summary>
		/// <param name="sender">sender of this callback.</param>
		/// <param name="certificate">server's certificate.</param>
		/// <param name="chain">certificate build chain.</param>
		/// <param name="sslPolicyErrors">ssl errors.</param>
		/// <returns>true if server has been validated; otherwise, false.</returns>
		public static bool ServerValidationCallback(object sender,
			X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			// because certificate is self-signed
			return true;
		}


		/// <summary>
		/// Certificate selection callback.
		/// </summary>
		/// <param name="sender">sender of this callback.</param>
		/// <param name="targetHost">server's ip address.</param>
		/// <param name="localCertificates">local certificates collection.</param>
		/// <param name="remoteCertificate">server's certificate.</param>
		/// <param name="acceptableIssuers">acceptable issuers for remote host.</param>
		/// <returns>certificate.</returns>
		/// <exception cref="CertificateException"></exception>
		public static X509Certificate ClientCertificateSelectionCallback(object sender,
			string targetHost, X509CertificateCollection localCertificates,
			X509Certificate remoteCertificate, string[] acceptableIssuers)
		{
			if (localCertificates.Count > 0)
				return localCertificates[0];
			else
				throw new CertificateException("Certificate not found in local certificates collection.");
		}

		/// <summary>
		/// Perform the client handshake.
		/// </summary>
		/// <param name="certificate">client's certificate.</param>
		/// <param name="sslStream">ssl stream with server.</param>
		/// <param name="targetHost">server's ip address.</param>
		public static void ClientSideHandshake(X509Certificate2 certificate, SslStream sslStream, string targetHost)
		{
			var certificateCollection = new X509Certificate2Collection();
			certificateCollection.Add(certificate);

			// 'handshake'
			sslStream.AuthenticateAsClient(targetHost, certificateCollection, SslProtocols.Tls, true);
		}

		#endregion

		#region Server

		/// <summary>
		/// Callback for the verification of the client certificate.
		/// </summary>
		/// <param name="sender">sender of this callback.</param>
		/// <param name="certificate">client's certificate.</param>
		/// <param name="chain">certificate build chain.</param>
		/// <param name="sslPolicyErrors">ssl errors.</param>
		/// <returns>true if client has been validated; otherwise, false.</returns>
		public static bool ClientValidationCallback(object sender,
			X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			// because certificate is self-signed
			return true;
		}

		/// <summary>
		/// Certificate selection callback.
		/// </summary>
		/// <param name="sender">sender of this callback.</param>
		/// <param name="targetHost">server's ip address.</param>
		/// <param name="localCertificates">local certificates collection.</param>
		/// <param name="remoteCertificate">client's certificate.</param>
		/// <param name="acceptableIssuers">acceptable issuers for remote client.</param>
		/// <returns>certificate.</returns>
		/// /// <exception cref="CertificateException"></exception>
		public static X509Certificate ServerCertificateSelectionCallback(object sender,
			string targetHost, X509CertificateCollection localCertificates,
			X509Certificate remoteCertificate, string[] acceptableIssuers)
		{
			if (localCertificates.Count > 0)
				return localCertificates[0];
			else
				throw new CertificateException("Certificate not found in local certificates collection.");
		}

		/// <summary>
		/// Perform the server handshake.
		/// </summary>
		/// <param name="certificate">server's certificate.</param>
		/// <param name="sslStream">ssl stream with client.</param>
		public static void ServerSideHandshake(X509Certificate2 certificate, SslStream sslStream)
		{
			sslStream.AuthenticateAsServer(certificate, true, SslProtocols.Tls, true);
		}

		#endregion
	}

	
	public static class SslTools
	{
		/// <summary>
		/// Create certificate from resource.
		/// </summary>
		/// <param name="type">An object representing a class in the assembly.</param>
		/// <param name="pathToResource">Path to resource with certificate.</param>
		/// <returns>cdertificate.</returns>
		/// <exception cref="CertificateException"></exception>
		public static X509Certificate2 CreateCertificate(Type type, string pathToResource)
		{
			try
			{
				byte[] embeddedCert;
				var thisAssembly = Assembly.GetAssembly(type);

				using (Stream certStream = thisAssembly.GetManifestResourceStream(pathToResource))
				{
					embeddedCert = new byte[certStream.Length];
					certStream.Read(embeddedCert, 0, (int)certStream.Length);

					return new X509Certificate2(embeddedCert, "", X509KeyStorageFlags.MachineKeySet);
				}
			}
			catch
			{
				throw new CertificateException("Can't create certificate from embeded resource.");
			}
		}
	}
}
