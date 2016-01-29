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

	#region Request messages

	/// <summary>
	/// Basic class for request messages.
	/// </summary>
	[Serializable]
	[XmlInclude(typeof(LoginRequestMessage))]
	[XmlInclude(typeof(LogoutRequestMessage))]
	[XmlInclude(typeof(RegisterRequestMessage))]
	[XmlInclude(typeof(GetAllUsersRequestMessage))]
	[XmlInclude(typeof(GetFriendsRequestMessage))]
	[XmlInclude(typeof(GetIncomeFriendshipReqsRequestMessage))]
	[XmlInclude(typeof(GetOutcomeFriendshipReqsRequestMessage))]
	[XmlInclude(typeof(FriendshipReqRequestMessage))]
	[XmlInclude(typeof(FriendActionRequestMessage))]
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
	/// when log out.
	/// </summary>
	[Serializable]
	public class LogoutRequestMessage : RequestMessage
	{
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
	/// to get array of all users.
	/// </summary>
	[Serializable]
	public class GetAllUsersRequestMessage : RequestMessage
	{
	}

	/// <summary>
	/// Message, that client send to server 
	/// to get array of friends.
	/// </summary>
	[Serializable]
	public class GetFriendsRequestMessage : RequestMessage
	{
	}

	/// <summary>
	/// Message, that client send to server 
	/// to get income friendship requests.
	/// </summary>
	[Serializable]
	public class GetIncomeFriendshipReqsRequestMessage : RequestMessage
	{
	}

	/// <summary>
	/// Message, that client send to server 
	/// to get outcome friendship requests.
	/// </summary>
	[Serializable]
	public class GetOutcomeFriendshipReqsRequestMessage : RequestMessage
	{
	}

	/// <summary>
	/// Message with friendship request.
	/// </summary>
	[Serializable]
	public class FriendshipReqRequestMessage : RequestMessage
	{
		public string login_of_needed_user;
	}

	/// <summary>
	/// Message with action.
	/// </summary>
	[Serializable]
	public class FriendActionRequestMessage : RequestMessage
	{
		public string friends_login;
		public ActionsWithFriend action;
	}

	#endregion

	#region Response messages

	/// <summary>
	/// Basic class for response messages.
	/// </summary>
	[Serializable]
	[XmlInclude(typeof(LoginResponseMessage))]
	[XmlInclude(typeof(RegisterResponseMessage))]
	[XmlInclude(typeof(GetAllUsersResponseMessage))]
	[XmlInclude(typeof(GetFriendsResponseMessage))]
	[XmlInclude(typeof(GetIncomeFriendshipReqsResponseMessage))]
	[XmlInclude(typeof(GetOutcomeFriendshipReqsResponseMessage))]
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
	/// Message with all user's logins.
	/// </summary>
	[Serializable]
	public class GetAllUsersResponseMessage : ResponseMessage
	{
		public string[] users;
	}

	/// <summary>
	/// Message with friend's logins.
	/// </summary>
	[Serializable]
	public class GetFriendsResponseMessage : ResponseMessage
	{
		public string[] friends;
	}

	/// <summary>
	/// Message with income friendship requests.
	/// </summary>
	[Serializable]
	public class GetIncomeFriendshipReqsResponseMessage : ResponseMessage
	{
		public string[] logins;
	}

	/// <summary>
	/// Message with outcome friendship requests.
	/// </summary>
	[Serializable]
	public class GetOutcomeFriendshipReqsResponseMessage : ResponseMessage
	{
		public string[] logins;
	}

	#endregion
}
