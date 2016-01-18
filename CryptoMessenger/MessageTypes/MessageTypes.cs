using System;

namespace MessageTypes
{
	/// <summary>
	/// Type of message.
	/// </summary>
	public enum MessageType { LOGIN, REGISTER };

	/// <summary>
	/// Server's response on login/registration request.
	/// </summary>
	public enum LoginRegisterResponse { SUCCESS, FAIL, ERROR };

	/// <summary>
	/// Message, that client send to server 
	/// when try to login or register.
	/// </summary>
	[Serializable]
	public class LoginRegisterMessage
    {
		public MessageType type { get; set; }
		public string login { get; set; }
		public string password { get; set; }
	}

	/// <summary>
	/// Message, that server send to client
	/// after login/registration attempt.
	/// </summary>
	[Serializable]
	public class LoginRegisterResponseMessage
	{
		public LoginRegisterResponse response { get; set; }
	}
}
