using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CryptoMessenger.ClientServerCommunication
{
	class CertificateCallback
	{
		/// <summary>     
		/// Callback for the verification of the server certificate     
		/// </summary>     
		public static bool ServerValidationCallback
			   (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			//if (SslPolicyErrors.None.Equals(sslPolicyErrors))
			//	return true;
			//else
			//	return false;
			return true;
		}

		///   <summary>     
		///  Certificate selection callback.     
		///   </summary>    
		public static X509Certificate ClientCertificateSelectionCallback(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
		{
			return localCertificates[0];
		}
	}
}
