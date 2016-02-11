using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Server.Database;

using MessageProtocol;
using MessageProtocol.MessageTypes;

namespace Server
{
	/// <summary>
	/// Online user data.
	/// </summary>
	class OnlineUser : IDisposable
	{
		private bool disposed = false;

		// client's id in db
		public int id { get; private set; }
		// client's login
		public string login { get; private set; }
		// client
		public MpClient client { get; private set; }

		/// <summary>
		/// Create user.
		/// </summary>
		/// <param name="id">client's id in db.</param>
		/// <param name="login">client's login.</param>
		/// <param name="client">client.</param>
		public OnlineUser(int id, string login, MpClient client)
		{
			this.id = id;
			this.login = login;
			this.client = client;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			if (disposed) return;

			Console.WriteLine("{0}: Client disconnected. ip {1}", DateTime.Now,
				((IPEndPoint)client.tcpClient.Client.RemoteEndPoint).Address.ToString());

			try
			{
				client.Close();
				disposed = true;
			}
			catch
			{
				// TODO logger
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
			return onlineUsers.Find(x => x.login.Equals(login));
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
					message = user.client.ReceiveMessage();
				}
				// user disconnected
				catch (ConnectionInterruptedException)
				{
					if (onlineUsers.Contains(user))
					{
						onlineUsers.Remove(user);
						user.Dispose();
					}
					break;
				}

				// process message
				try
				{
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
				catch (ConnectionInterruptedException)
				{
					// TODO logger
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
		/// <exception cref="ConnectionInterruptedException"></exception>
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

			user.client.SendMessage(new AllUsersMessage
			{
				users = users
			});
		}

		/// <summary>
		/// Send array of friends to user.
		/// </summary>
		/// <param name="user">user.</param>
		/// <exception cref="ConnectionInterruptedException"></exception>
		private void SendFriends(OnlineUser user)
		{
			user.client.SendMessage(new FriendsMessage
			{
				friends = DBoperations.GetFriends(user.id)
			});
		}

		/// <summary>
		/// Send array of income friendship requests to user.
		/// </summary>
		/// <param name="user">user.</param>
		/// <exception cref="ConnectionInterruptedException"></exception>
		private void SendIncomeFriendshipRequests(OnlineUser user)
		{
			user.client.SendMessage(new IncomeFriendshipRequestsMessage
			{
				logins = DBoperations.GetIncomeFriendshipRequests(user.id)
			});
		}

		/// <summary>
		/// Send array of outcome friendship requests to user.
		/// </summary>
		/// <param name="user">user.</param>
		/// <exception cref="ConnectionInterruptedException"></exception>
		private void SendOutcomeFriendshipRequests(OnlineUser user)
		{
			user.client.SendMessage(new OutcomeFriendshipRequestsMessage
			{
				logins = DBoperations.GetOutcomeFriendshipRequests(user.id)
			});
		}

		/// <summary>
		/// Send conversation to user.
		/// </summary>
		/// <param name="user">user.</param>
		/// <param name="interlocutor">interlocutor of user in conversation.</param>
		/// <exception cref="ConnectionInterruptedException"></exception>
		private void SendConversation(OnlineUser user, string interlocutor)
		{
			int interlocutors_id = DBoperations.GetUserId(interlocutor);
			if (interlocutors_id == 0) return;

			// get replies from db
			ConversationReply[] replies = DBoperations.GetConversation(user.id, interlocutors_id);

			
			if (replies != null)
			{
				foreach (var r in replies)
					user.client.SendMessage(new ReplyMessage
					{
						interlocutor = interlocutor,
						reply_author = DBoperations.GetUserLogin(r.user_id),
						reply_time = r.time,
						reply_text = r.reply
					});
			}
		}

		/// <summary>
		/// Set friendship request in db.
		/// </summary>
		/// <param name="user">user.</param>
		/// <param name="friends_login">friend's login.</param>
		/// <exception cref="ConnectionInterruptedException"></exception>
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
		/// <exception cref="ConnectionInterruptedException"></exception>
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
		/// <exception cref="ConnectionInterruptedException"></exception>
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
		/// <exception cref="ConnectionInterruptedException"></exception>
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
		/// <exception cref="ConnectionInterruptedException"></exception>
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
		/// <exception cref="ConnectionInterruptedException"></exception>
		private void HandleReply(OnlineUser user, string interlocutor, string text)
		{
			int interlocutors_id = DBoperations.GetUserId(interlocutor);
			if (interlocutors_id == 0) return;

			// time of reply
			DateTime time = DateTime.Now;

			if (DBoperations.AddNewReply(user.id, interlocutors_id, text, time))
			{
				// send reply to user one
				user.client.SendMessage(new ReplyMessage
				{
					interlocutor = interlocutor,
					reply_author = user.login,
					reply_time = time,
					reply_text = text
				});

				// send reply to user two if online
				OnlineUser friend = GetOnlineUser(interlocutor);
				if (friend != null)
				{
					friend.client.SendMessage(new ReplyMessage
					{
						interlocutor = user.login,
						reply_author = user.login,
						reply_time = time,
						reply_text = text
					});
				}
			}
		}

		#endregion
	}
}
