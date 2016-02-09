using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;

using Server.Database;

using MessageProtocol.MessageTypes;
using MessageProtocol.Client;

namespace Server
{
	/// <summary>
	/// Online user data.
	/// </summary>
	class OnlineUser : IDisposable
	{
		private bool disposed;

		// client's id in db
		public int id { get; private set; }
		// client's login
		public string login { get; private set; }

		// client
		public TcpClient client { get; private set; }
		// ssl stream with connected client
		public SslStream sslStream { get; private set; }

		/// <summary>
		/// Create instance of user.
		/// </summary>
		/// <param name="id">client's id in db.</param>
		/// <param name="login">client's login.</param>
		/// <param name="client">tcp client.</param>
		/// <param name="sslStream">ssl stream with connected client.</param>
		public OnlineUser(int id, string login, TcpClient client, SslStream sslStream)
		{
			this.id = id;
			this.login = login;
			this.client = client;
			this.sslStream = sslStream;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			if (disposed) return;

			Console.WriteLine("{0}: Client disconnected. ip {1}", DateTime.Now,
				((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());

			try
			{
				client.Client.Shutdown(SocketShutdown.Both);
			}
			catch
			{
				// TODO logger
			}
			finally
			{
				sslStream.Dispose();
				client.Close();
				disposed = true;
			}
		}
	}

	/// <summary>
	/// Class for handle online users.
	/// </summary>
	class OnlineUsersHandler : IDisposable
	{ 
		private List<OnlineUser> onlineUsers;

		public OnlineUsersHandler()
		{
			onlineUsers = new List<OnlineUser>();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			onlineUsers.ForEach((x) => { x.Dispose(); });
			onlineUsers.Clear();
		}

		/// <summary>
		/// Add user to list of online users and start to listen to him.
		/// </summary>
		/// <param name="user">user.</param>
		public void AddUser(OnlineUser user)
		{
			if (user != null)
				onlineUsers.Add(user);

			// listen this user
			Task.Run(() => UserListener(user));
		}	

		/// <summary>
		/// Get online user.
		/// </summary>
		/// <param name="login">user's login.</param>
		/// <returns>user, if he online; otherwise - null.</returns>
		public OnlineUser GetOnlineUser(string login)
		{
			return onlineUsers.Find(x => x.login == login);
		}

		/// <summary>
		/// Listen to user's messages.
		/// </summary>
		/// <param name="user">user.</param>
		private void UserListener(OnlineUser user)
		{
			while (true)
			{
				Message message;

				try
				{ 
					// user's incoming message
					message = MpClient.ReceiveMessage(user.sslStream);
				}
				catch (ConnectionInterruptedException)
				{
					if (onlineUsers.Contains(user))
					{
						onlineUsers.Remove(user);
						user.Dispose();
					}

					// TODO logger
					break;
				}
				catch
				{
					// TODO logger
					break;
				}

				// process message
				if (message is GetAllUsersMessage)
				{
					SendAllUsers(user);
				}
				else if (message is GetFriendsMessage)
				{
					SendFriends(user);
				}
				else if (message is GetIncomeFriendshipRequestsMessage)
				{
					SendIncomeFriendshipRequests(user);
				}
				else if (message is GetOutcomeFriendshipRequestsMessage)
				{
					SendOutcomeFriendshipRequests(user);
				}
				else if (message is GetConversationMessage)
				{
					SendConversation(user, ((GetConversationMessage)message).interlocutor);
				}
				else if (message is FriendshipRequestMessage)
				{
					SetFriendshipRequest(user, ((FriendshipRequestMessage)message).login_of_needed_user);
				}
				else if (message is FriendActionMessage)
				{
					FriendActionMessage msg = (FriendActionMessage)message;

					switch (msg.action)
					{
						case ActionsWithFriend.CANCEL_FRIENDSHIP_REQUEST:
							CancelFriendshipRequest(user, msg.friends_login);
							break;
						case ActionsWithFriend.ACCEPT_FRIENDSHIP:
							AcceptFriendshipRequest(user, msg.friends_login);
							break;
						case ActionsWithFriend.REJECT_FRIENDSHIP:
							RejectFriendshipRequest(user, msg.friends_login);
							break;
						case ActionsWithFriend.REMOVE_FROM_FRIENDS:
							RemoveFriend(user, msg.friends_login);
							break;
					}
				}
				else if (message is ReplyMessage)
				{
					HandleReply(user, ((ReplyMessage)message).interlocutor, ((ReplyMessage)message).reply_text);
				}
				else if (message is LogoutRequestMessage)
				{
					Logout(user);
					break;
				}
			}
		}

		#region Actions on received message

		/// <summary>
		/// Log out user.
		/// </summary>
		/// <param name="user">user.</param>
		private void Logout(OnlineUser user)
		{
			onlineUsers.Remove(user);
			user.Dispose();
		}

		/// <summary>
		/// Send array of all users to user.
		/// </summary>
		/// <param name="user">user.</param>
		private void SendAllUsers(OnlineUser user)
		{
			string[] all_users = DBoperations.GetAllUsers();
			string[] income_requests = DBoperations.GetIncomeFriendshipRequests(user.id);
			string[] outcome_requests = DBoperations.GetOutcomeFriendshipRequests(user.id);
			string[] friends = DBoperations.GetFriends(user.id);

			string[] users = all_users.Where(x =>
				!income_requests.Contains(x) & 
				!outcome_requests.Contains(x) &
				!friends.Contains(x) &
				x != user.login).ToArray();

			try
			{
				MpClient.SendMessage(user.sslStream, new AllUsersMessage
				{
					users = users
				});
			}
			catch
			{
				// TODO logger
			}
		}

		/// <summary>
		/// Send array of friends to user.
		/// </summary>
		/// <param name="user">user.</param>
		private void SendFriends(OnlineUser user)
		{
			try
			{
				MpClient.SendMessage(user.sslStream, new FriendsMessage
				{
					friends = DBoperations.GetFriends(user.id)
				});
			}
			catch
			{
				// TODO logger
			}
		}

		/// <summary>
		/// Send array of income friendship requests to user.
		/// </summary>
		/// <param name="user">user.</param>
		private void SendIncomeFriendshipRequests(OnlineUser user)
		{
			try
			{
				MpClient.SendMessage(user.sslStream, new IncomeFriendshipRequestsMessage
				{
					logins = DBoperations.GetIncomeFriendshipRequests(user.id)
				});
			}
			catch
			{
				// TODO logger
			}
		}

		/// <summary>
		/// Send array of outcome friendship requests to user.
		/// </summary>
		/// <param name="user">user.</param>
		private void SendOutcomeFriendshipRequests(OnlineUser user)
		{
			try
			{
				MpClient.SendMessage(user.sslStream, new OutcomeFriendshipRequestsMessage
				{
					logins = DBoperations.GetOutcomeFriendshipRequests(user.id)
				});
			}
			catch
			{
				// TODO logger
			}
		}

		/// <summary>
		/// Send conversation to user.
		/// </summary>
		/// <param name="user">user.</param>
		/// <param name="interlocutor">interlocutor of user in conversation.</param>
		private void SendConversation(OnlineUser user, string interlocutor)
		{
			int interlocutors_id = DBoperations.GetUserId(interlocutor);
			if (interlocutors_id == 0) return;

			// get replies from db
			ConversationReply[] replies = DBoperations.GetConversation(user.id, interlocutors_id);

			
			if (replies != null)
			{
				try
				{
					foreach (var r in replies)
						MpClient.SendMessage(user.sslStream, new ReplyMessage
						{
							interlocutor = interlocutor,
							reply_author = DBoperations.GetUserLogin(r.user_id),
							reply_time = r.time,
							reply_text = r.reply
						});
				}
				catch
				{
					// TODO logger
				}
			}
		}

		/// <summary>
		/// Set friendship request in db.
		/// </summary>
		/// <param name="user">user.</param>
		/// <param name="friends_login">friend's login.</param>
		private void SetFriendshipRequest(OnlineUser user, string friends_login)
		{
			int friends_id = DBoperations.GetUserId(friends_login);
			if (friends_id == 0) return;

			// set friendship request
			if (DBoperations.SetFriendship(false, user.id, friends_id))
			{
				// send new data to user one
				SendAllUsers(user);
				SendOutcomeFriendshipRequests(user);
				// send new data to user two if online
				OnlineUser friend = GetOnlineUser(friends_login);
				if (friend != null) SendIncomeFriendshipRequests(friend);
			}
		}

		/// <summary>
		/// Cancel friendship request.
		/// </summary>
		/// <param name="user">user.</param>
		/// <param name="friends_login">friend's login.</param>
		private void CancelFriendshipRequest(OnlineUser user, string friends_login)
		{
			int friends_id = DBoperations.GetUserId(friends_login);
			if (friends_id == 0) return;

			if (DBoperations.RemoveFriendshipRequest(user.id, friends_id))
			{
				// send new data to user one
				SendOutcomeFriendshipRequests(user);
				// send new data to user two if online
				OnlineUser friend = GetOnlineUser(friends_login);
				if (friend != null) SendIncomeFriendshipRequests(friend);
			}
		}

		/// <summary>
		/// Accept friendship request.
		/// </summary>
		/// <param name="user">user.</param>
		/// <param name="friends_login">friend's login.</param>
		private void AcceptFriendshipRequest(OnlineUser user, string friends_login)
		{
			int friends_id = DBoperations.GetUserId(friends_login);
			if (friends_id == 0) return;

			if (DBoperations.SetFriendship(true, friends_id, user.id))
			{
				// send new data to user one
				SendIncomeFriendshipRequests(user);
				SendFriends(user);
				// send new data to user two if online
				OnlineUser friend = GetOnlineUser(friends_login);
				if (friend != null)
				{
					SendOutcomeFriendshipRequests(friend);
					SendFriends(friend);
				}
			}
		}

		/// <summary>
		/// Reject friendship request.
		/// </summary>
		/// <param name="user">user.</param>
		/// <param name="friends_login">friend's login.</param>
		private void RejectFriendshipRequest(OnlineUser user, string friends_login)
		{
			int friends_id = DBoperations.GetUserId(friends_login);
			if (friends_id == 0) return;

			if (DBoperations.RemoveFriendshipRequest(friends_id, user.id))
			{
				// send new data to user one
				SendIncomeFriendshipRequests(user);
				// send new data to user two if online
				OnlineUser friend = GetOnlineUser(friends_login);
				if (friend != null) SendOutcomeFriendshipRequests(friend);
			}
		}

		/// <summary>
		/// Remove user from friends.
		/// </summary>
		/// <param name="user">user.</param>
		/// <param name="friends_login">friend's login.</param>
		private void RemoveFriend(OnlineUser user, string friends_login)
		{
			int friends_id = DBoperations.GetUserId(friends_login);
			if (friends_id == 0) return;

			if (DBoperations.RemoveFriend(user.id, friends_id))
			{
				// send new data to user one
				SendFriends(user);
				// send new data to user two if online
				OnlineUser friend = GetOnlineUser(friends_login);
				if (friend != null) SendFriends(friend);
			}
		}

		/// <summary>
		/// Handle new reply in conversation.
		/// </summary>
		/// <param name="user">user.</param>
		/// <param name="interlocutor">interlocutor.</param>
		/// <param name="text">text of reply.</param>
		private void HandleReply(OnlineUser user, string interlocutor, string text)
		{
			int interlocutors_id = DBoperations.GetUserId(interlocutor);
			if (interlocutors_id == 0) return;

			// time of reply
			DateTime time = DateTime.Now;

			if (DBoperations.AddNewReply(user.id, interlocutors_id, text, time))
			{
				try
				{
					// send reply to user one
					MpClient.SendMessage(user.sslStream, new ReplyMessage
					{
						interlocutor = interlocutor,
						reply_author = user.login,
						reply_time = time,
						reply_text = text
					});
				}
				catch
				{
					// TODO logger
				}

				// send reply to user two if online
				OnlineUser friend = GetOnlineUser(interlocutor);
				if (friend != null)
				{
					try
					{
						MpClient.SendMessage(friend.sslStream, new ReplyMessage
						{
							interlocutor = user.login,
							reply_author = user.login,
							reply_time = time,
							reply_text = text
						});
					}
					catch
					{
						// TODO logger
					}
				}
			}
		}

		#endregion
	}
}
