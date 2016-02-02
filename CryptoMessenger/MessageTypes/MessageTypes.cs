using System;
using System.Xml.Serialization;

namespace MessageTypes
{
	/// <summary>
	/// Server's response on login/registration request.
	/// </summary>
	public enum LoginRegisterResponse
	{
		SUCCESS,
		FAIL,
		ERROR,
		ALREADY_LOGIN
	};

	/// <summary>
	/// Actions with frends and friendship requests.
	/// </summary>
	public enum ActionsWithFriend
	{
		CANCEL_FRIENDSHIP_REQUEST,
		ACCEPT_FRIENDSHIP,
		REJECT_FRIENDSHIP,
		REMOVE_FROM_FRIENDS
	};

	/// <summary>
	/// Basic class for messages.
	/// </summary>
	[Serializable]
	[XmlInclude(typeof(LoginRequestMessage))]
	[XmlInclude(typeof(LogoutRequestMessage))]
	[XmlInclude(typeof(RegisterRequestMessage))]
	[XmlInclude(typeof(GetAllUsersMessage))]
	[XmlInclude(typeof(GetFriendsMessage))]
	[XmlInclude(typeof(GetIncomeFriendshipRequestsMessage))]
	[XmlInclude(typeof(GetOutcomeFriendshipRequestsMessage))]
	[XmlInclude(typeof(FriendshipRequestMessage))]
	[XmlInclude(typeof(FriendActionMessage))]
	[XmlInclude(typeof(LoginResponseMessage))]
	[XmlInclude(typeof(RegisterResponseMessage))]
	[XmlInclude(typeof(AllUsersMessage))]
	[XmlInclude(typeof(FriendsMessage))]
	[XmlInclude(typeof(IncomeFriendshipRequestsMessage))]
	[XmlInclude(typeof(OutcomeFriendshipRequestsMessage))]
	public abstract class Message
	{
	}

	/// <summary>
	/// Message, that client send to server 
	/// when try to login.
	/// </summary>
	[Serializable]
	public class LoginRequestMessage : Message
	{
		public string login { get; set; }
		public string password { get; set; }
	}

	/// <summary>
	/// Message, that client send to server 
	/// when log out.
	/// </summary>
	[Serializable]
	public class LogoutRequestMessage : Message
	{
	}

	/// <summary>
	/// Message, that client send to server 
	/// when try to register.
	/// </summary>
	[Serializable]
	public class RegisterRequestMessage : Message
	{
		public string login { get; set; }
		public string password { get; set; }
	}

	/// <summary>
	/// Message, that client send to server 
	/// to get array of all users.
	/// </summary>
	[Serializable]
	public class GetAllUsersMessage : Message
	{
	}

	/// <summary>
	/// Message, that client send to server 
	/// to get array of friends.
	/// </summary>
	[Serializable]
	public class GetFriendsMessage : Message
	{
	}

	/// <summary>
	/// Message, that client send to server 
	/// to get income friendship requests.
	/// </summary>
	[Serializable]
	public class GetIncomeFriendshipRequestsMessage : Message
	{
	}

	/// <summary>
	/// Message, that client send to server 
	/// to get outcome friendship requests.
	/// </summary>
	[Serializable]
	public class GetOutcomeFriendshipRequestsMessage : Message
	{
	}

	/// <summary>
	/// Message with friendship request.
	/// </summary>
	[Serializable]
	public class FriendshipRequestMessage : Message
	{
		public string login_of_needed_user;
	}

	/// <summary>
	/// Message with action.
	/// </summary>
	[Serializable]
	public class FriendActionMessage : Message
	{
		public string friends_login;
		public ActionsWithFriend action;
	}

	/// <summary>
	/// Message, that server send to client
	/// after login attempt.
	/// </summary>
	[Serializable]
	public class LoginResponseMessage : Message
	{
		public LoginRegisterResponse response { get; set; }
	}

	/// <summary>
	/// Message, that server send to client
	/// after register attempt.
	/// </summary>
	[Serializable]
	public class RegisterResponseMessage : Message
	{
		public LoginRegisterResponse response { get; set; }
	}

	/// <summary>
	/// Message with all user's logins.
	/// </summary>
	[Serializable]
	public class AllUsersMessage : Message
	{
		public string[] users;
	}

	/// <summary>
	/// Message with friend's logins.
	/// </summary>
	[Serializable]
	public class FriendsMessage : Message
	{
		public string[] friends;
	}

	/// <summary>
	/// Message with income friendship requests.
	/// </summary>
	[Serializable]
	public class IncomeFriendshipRequestsMessage : Message
	{
		public string[] logins;
	}

	/// <summary>
	/// Message with outcome friendship requests.
	/// </summary>
	[Serializable]
	public class OutcomeFriendshipRequestsMessage : Message
	{
		public string[] logins;
	}
}
