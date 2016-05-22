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
		Success,
		Fail,
		Error,
		AlreadyLogin
	};

	/// <summary>
	/// Actions with frends and friendship requests.
	/// </summary>
	[Serializable]
	public enum UserActions
	{
		SendFriendshipRequest,
		CancelFriendshipRequest,
		AcceptFriendship,
		RejectFriendship,
		RemoveFromFriends,
		GoOnline,
		GoOffline
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
	[XmlInclude(typeof(UserActionMessage))]
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
		public string Login { get; set; }
		public string Password { get; set; }
	}

	/// <summary>
	/// Message, that client send to server 
	/// when try to register.
	/// </summary>
	[Serializable]
	public class RegisterRequestMessage : Message
	{
		public string Login { get; set; }
		public string Password { get; set; }
	}

	/// <summary>
	/// Message, that server send to client
	/// after login/register attempt.
	/// </summary>
	[Serializable]
	public class LoginRegisterResponseMessage : Message
	{
		public LoginRegisterResponse Response { get; set; }
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
		public string[] OnlineUsers { get; set; }
		public string[] OfflineUsers { get; set; }
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
		public string[] OnlineFriends { get; set; }
		public string[] OfflineFriends { get; set; }
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
		public string[] Logins { get; set; }
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
		public string[] Logins { get; set; }
	}

	/// <summary>
	/// Message with friendship request.
	/// </summary>
	[Serializable]
	public class FriendshipRequestMessage : Message
	{
		public string LoginOfNeededUser { get; set; }
	}

	/// <summary>
	/// Message with action.
	/// </summary>
	[Serializable]
	public class UserActionMessage : Message
	{
		public string UserLogin { get; set; }
		public UserActions Action { get; set; }
	}

	/// <summary>
	/// Message, that client send to server 
	/// to get conversation with interlocutor.
	/// </summary>
	[Serializable]
	public class GetConversationMessage : Message
	{
		public string Interlocutor { get; set; }
	}

	/// <summary>
	/// Message with new conversation reply.
	/// </summary>
	[Serializable]
	public class NewReplyMessage : Message
	{
		public string Interlocutor { get; set; }

		public string Author { get; set; }
		public DateTime Time { get; set; }
		public string Text { get; set; }
	}

	/// <summary>
	/// Message with old conversation reply.
	/// </summary>
	[Serializable]
	public class OldReplyMessage : Message
	{
		public string Interlocutor { get; set; }

		public string Author { get; set; }
		public DateTime Time { get; set; }
		public string Text { get; set; }
	}
}
