using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Server.SslStuff
{
	class CertificateCallback
	{
		/// <summary>
		/// Callback for the verification of the client's certificate
		/// </summary>
		public static bool ClientValidationCallback(object sender, 
			X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			//if (SslPolicyErrors.None.Equals(sslPolicyErrors))
			//	return true;
			//else
			//	return false;
			return true;
		}

		/// <summary>     
		/// Certificate selection callback.     
		/// </summary>     
		public static X509Certificate ServerCertificateSelectionCallback(object sender, 
			string targetHost, X509CertificateCollection localCertificates, 
			X509Certificate remoteCertificate, string[] acceptableIssuers)
		{
			return localCertificates[0];
		}
	}
}
