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

namespace Server.SslStuff
{
	class SslHandshake
	{
		/// <summary>
		/// Perform the server handshake
		/// </summary>
		public static void ServerSideHandshake(SslStream sslStream)
		{


			byte[] embeddedCert;
			Assembly thisAssembly = Assembly.GetAssembly(typeof(SslHandshake));
			using (Stream certStream = thisAssembly.GetManifestResourceStream("Server.Certificate.cert.pfx"))
			{
				embeddedCert = new byte[certStream.Length];
				certStream.Read(embeddedCert, 0, (int)certStream.Length);
			}

			X509Certificate2 certificate = new X509Certificate2(embeddedCert, "", X509KeyStorageFlags.MachineKeySet);


			bool requireClientCertificate = true;
			SslProtocols enabledSslProtocols = SslProtocols.Tls;
			bool checkCertificateRevocation = true;
			sslStream.AuthenticateAsServer
			  (certificate, requireClientCertificate, enabledSslProtocols, checkCertificateRevocation);
		}
	}
}
