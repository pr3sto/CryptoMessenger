﻿using System;
using System.Xml.Serialization;

namespace MessageTypes
{
	/// <summary>
	/// Server's response on login/registration request.
	/// </summary>
	public enum LoginRegisterResponse { SUCCESS, FAIL, ERROR };

	#region Request messages

	/// <summary>
	/// Basic class for request messages.
	/// </summary>
	[Serializable]
	[XmlInclude(typeof(LoginRequestMessage))]
	[XmlInclude(typeof(RegisterRequestMessage))]
	[XmlInclude(typeof(LogoutRequestMessage))]
	public abstract class RequestMessage
	{
	}

	/// <summary>
	/// Message, that client send to server 
	/// when try to login.
	/// </summary>
	[Serializable]
	public class LoginRequestMessage : RequestMessage
	{
		public string login { get; set; }
		public string password { get; set; }
	}

	/// <summary>
	/// Message, that client send to server 
	/// when try to register.
	/// </summary>
	[Serializable]
	public class RegisterRequestMessage : RequestMessage
	{
		public string login { get; set; }
		public string password { get; set; }
	}

	/// <summary>
	/// Message, that client send to server 
	/// when he log out.
	/// </summary>
	[Serializable]
	public class LogoutRequestMessage : RequestMessage
	{
	}

	#endregion

	#region Response messages

	/// <summary>
	/// Basic class for response messages.
	/// </summary>
	[Serializable]
	[XmlInclude(typeof(LoginResponseMessage))]
	[XmlInclude(typeof(RegisterResponseMessage))]
	[XmlInclude(typeof(LogoutResponseMessage))]
	public abstract class ResponseMessage
	{
	}

	/// <summary>
	/// Message, that server send to client
	/// after login attempt.
	/// </summary>
	[Serializable]
	public class LoginResponseMessage : ResponseMessage
	{
		public LoginRegisterResponse response { get; set; }
	}

	/// <summary>
	/// Message, that server send to client
	/// after register attempt.
	/// </summary>
	[Serializable]
	public class RegisterResponseMessage : ResponseMessage
	{
		public LoginRegisterResponse response { get; set; }
	}

	/// <summary>
	/// Message, that server send to client
	/// after client's log out.
	/// </summary>
	/// [Serializable]
	public class LogoutResponseMessage : ResponseMessage
	{
	}

	#endregion
}
