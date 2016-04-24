using System;
using System.Xml.Serialization;

namespace MessageProtocol.MessageTypes
{
	/// <summary>
	/// Server's response on login/registration request.
	/// </summary>
	[Serializable]
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
	[Serializable]
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
	[XmlInclude(typeof(LoginRegisterResponseMessage))]
	[XmlInclude(typeof(AllUsersMessage))]
	[XmlInclude(typeof(FriendsMessage))]
	[XmlInclude(typeof(IncomeFriendshipRequestsMessage))]
	[XmlInclude(typeof(OutcomeFriendshipRequestsMessage))]
	[XmlInclude(typeof(GetConversationMessage))]
	[XmlInclude(typeof(NewReplyMessage))]
	[XmlInclude(typeof(OldReplyMessage))]
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
	/// when try to register.
	/// </summary>
	[Serializable]
	public class RegisterRequestMessage : Message
	{
		public string login { get; set; }
		public string password { get; set; }
	}

	/// <summary>
	/// Message, that server send to client
	/// after login/register attempt.
	/// </summary>
	[Serializable]
	public class LoginRegisterResponseMessage : Message
	{
		public LoginRegisterResponse response { get; set; }
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
	/// to get array of all users.
	/// </summary>
	[Serializable]
	public class GetAllUsersMessage : Message
	{
	}

	/// <summary>
	/// Message with all user's logins.
	/// </summary>
	[Serializable]
	public class AllUsersMessage : Message
	{
		public string[] users { get; set; }
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
	/// Message with friend's logins.
	/// </summary>
	[Serializable]
	public class FriendsMessage : Message
	{
		public string[] friends { get; set; }
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
	/// Message with income friendship requests.
	/// </summary>
	[Serializable]
	public class IncomeFriendshipRequestsMessage : Message
	{
		public string[] logins { get; set; }
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
	/// Message with outcome friendship requests.
	/// </summary>
	[Serializable]
	public class OutcomeFriendshipRequestsMessage : Message
	{
		public string[] logins { get; set; }
	}

	/// <summary>
	/// Message with friendship request.
	/// </summary>
	[Serializable]
	public class FriendshipRequestMessage : Message
	{
		public string login_of_needed_user { get; set; }
	}

	/// <summary>
	/// Message with action.
	/// </summary>
	[Serializable]
	public class FriendActionMessage : Message
	{
		public string friends_login { get; set; }
		public ActionsWithFriend action { get; set; }
	}

	/// <summary>
	/// Message, that client send to server 
	/// to get conversation with interlocutor.
	/// </summary>
	[Serializable]
	public class GetConversationMessage : Message
	{
		public string interlocutor { get; set; }
	}

	/// <summary>
	/// Message with new conversation reply.
	/// </summary>
	[Serializable]
	public class NewReplyMessage : Message
	{
		public string interlocutor { get; set; }

		public string reply_author { get; set; }
		public DateTime reply_time { get; set; }
		public string reply_text { get; set; }
	}

	/// <summary>
	/// Message with old conversation reply.
	/// </summary>
	[Serializable]
	public class OldReplyMessage : Message
	{
		public string interlocutor { get; set; }

		public string reply_author { get; set; }
		public DateTime reply_time { get; set; }
		public string reply_text { get; set; }
	}
}
