using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Reflection;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CryptoMessenger.ClientServerCommunication
{
	class SslHandshake
	{
		/// <summary>
		/// Perform the client handshake
		/// </summary>
		public static void ClientSideHandshake(SslStream sslStream, string targetHost)
		{


			byte[] embeddedCert;
			Assembly thisAssembly = Assembly.GetAssembly(typeof(SslHandshake));
			using (Stream certStream = thisAssembly.GetManifestResourceStream("CryptoMessenger.Certificate.cert.pfx"))
			{
				embeddedCert = new byte[certStream.Length];
				certStream.Read(embeddedCert, 0, (int)certStream.Length);
			}

			X509Certificate2 certificate = new X509Certificate2(embeddedCert, "", X509KeyStorageFlags.MachineKeySet);
			X509CertificateCollection ccertificateCollection = new X509CertificateCollection();
			ccertificateCollection.Add(certificate);


			SslProtocols sslProtocol = SslProtocols.Tls;
			bool checkCertificateRevocation = true;
			sslStream.AuthenticateAsClient(targetHost, ccertificateCollection, sslProtocol, checkCertificateRevocation);
		}
	}
}
