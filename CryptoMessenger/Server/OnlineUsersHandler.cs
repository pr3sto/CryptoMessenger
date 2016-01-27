using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;

using MessageTypes;
using Server.Database;

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

			Console.WriteLine(" - client disconnected. ip {0}",
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
	/// Class for handle online user.
	/// </summary>
	class OnlineUsersHandler : IDisposable
	{ 
		// list of online users
		public List<OnlineUser> onlineUsers;

		/// <summary>
		/// Constructor.
		/// </summary>
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
		/// Add user to list of online users and start to listen him.
		/// </summary>
		/// <param name="user"></param>
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
			try
			{
				while (true)
				{
					// user's incoming message
					RequestMessage message = ClientConnection.ReceiveMessage(user.client, user.sslStream);

					// process message
					if (message is LogoutRequestMessage)
					{
						Logout(user);
						break;
					}
					else if (message is GetAllUsersRequestMessage)
					{
						SendAllUsers(user);
					}
					else if (message is GetFriendsRequestMessage)
					{
						SendFriends(user);
					}
					else if (message is GetFriendshipReqsRequestMessage)
					{
						SendFriendshipRequests(user);
					}
					else if (message is FriendshipReqRequestMessage)
					{
						SetFriendshipRequest(user, ((FriendshipReqRequestMessage)message).login_of_needed_user);
					}
					else if (message is FriendActionRequestMessage)
					{
						FriendActionRequestMessage msg = (FriendActionRequestMessage)message;

						if (ActionsWithFriend.CANCEL_FRIENDSHIP_REQUEST.Equals(msg.action))
						{
							CancelFriendshipRequest(user, msg.friends_login);
						}
						else if (ActionsWithFriend.ACCEPT_FRIENDSHIP.Equals(msg.action))
						{
							AcceptFriendshipRequest(user, msg.friends_login);
						}
						else if (ActionsWithFriend.REJECT_FRIENDSHIP.Equals(msg.action))
						{
							RejectFriendshipRequest(user, msg.friends_login);
						}
						else if (ActionsWithFriend.REMOVE_FROM_FRIENDS.Equals(msg.action))
						{
							RemoveFriend(user, msg.friends_login);
						}
					}
				}
			}
			catch
			{
				// client disconnected in a bad way
				if (onlineUsers.Contains(user))
				{
					onlineUsers.Remove(user);
					user.Dispose();
				}
				// or server has been stopped
			}
		}

		#region Actions on recieved message

		/// <summary>
		/// Logout user.
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
			string[] income_requests = DBoperations.GetIncomeFriendshipRequests(user.login);
			string[] outcome_requests = DBoperations.GetOutcomeFriendshipRequests(user.login);
			string[] friends = DBoperations.GetFriends(user.login);

			string[] users = all_users.Where(x =>
				!income_requests.Contains(x) & 
				!outcome_requests.Contains(x) &
				!friends.Contains(x) &
				x != user.login).ToArray();

			GetAllUsersResponseMessage response = new GetAllUsersResponseMessage
			{
				users = users
			};
			ClientConnection.SendMessage(user.sslStream, response);
		}

		/// <summary>
		/// Send array of friends to user.
		/// </summary>
		/// <param name="user">user.</param>
		private void SendFriends(OnlineUser user)
		{
			GetFriendsResponseMessage response = new GetFriendsResponseMessage
			{
				friends = DBoperations.GetFriends(user.login)
			};
			ClientConnection.SendMessage(user.sslStream, response);
		}

		/// <summary>
		/// Send arrays of friendship requests to user.
		/// </summary>
		/// <param name="user">user.</param>
		private void SendFriendshipRequests(OnlineUser user)
		{
			GetFriendsReqsResponseMessage response = new GetFriendsReqsResponseMessage
			{
				income_requests = DBoperations.GetIncomeFriendshipRequests(user.login),
				outcome_requests = DBoperations.GetOutcomeFriendshipRequests(user.login)
			};
			ClientConnection.SendMessage(user.sslStream, response);
		}

		/// <summary>
		/// Set friendship request in db.
		/// </summary>
		/// <param name="user">user.</param>
		/// <param name="friends_login">friend's login.</param>
		private void SetFriendshipRequest(OnlineUser user, string friends_login)
		{
			// set friendship request
			if (DBoperations.SetFriendship(false, user.login, friends_login))
			{
				// send new lists to user one
				SendAllUsers(user);
				SendFriendshipRequests(user);
				// send new lists to user two if online
				OnlineUser friend = GetOnlineUser(friends_login);
				if (friend != null) SendFriendshipRequests(friend);
			}
		}

		/// <summary>
		/// Cancel friendship request.
		/// </summary>
		/// <param name="user">user.</param>
		/// <param name="friends_login">friend's login.</param>
		private void CancelFriendshipRequest(OnlineUser user, string friends_login)
		{
			if (DBoperations.RemoveFriendshipRequest(user.login, friends_login))
			{
				// send new lists to user one
				SendFriendshipRequests(user);
				// send new lists to user two if online
				OnlineUser friend = GetOnlineUser(friends_login);
				if (friend != null) SendFriendshipRequests(friend);
			}
		}

		/// <summary>
		/// Accept friendship request.
		/// </summary>
		/// <param name="user">user.</param>
		/// <param name="friends_login">friend's login.</param>
		private void AcceptFriendshipRequest(OnlineUser user, string friends_login)
		{
			if (DBoperations.SetFriendship(true, friends_login, user.login))
			{
				// send new lists to user one
				SendFriendshipRequests(user);
				SendFriends(user);
				// send new lists to user two if online
				OnlineUser friend = GetOnlineUser(friends_login);
				if (friend != null)
				{
					SendFriendshipRequests(friend);
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
			if (DBoperations.RemoveFriendshipRequest(friends_login, user.login))
			{
				// send new lists to user one
				SendFriendshipRequests(user);
				// send new lists to user two if online
				OnlineUser friend = GetOnlineUser(friends_login);
				if (friend != null) SendFriendshipRequests(friend);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="user">user.</param>
		/// <param name="friends_login">friend's login.</param>
		private void RemoveFriend(OnlineUser user, string friends_login)
		{
			if (DBoperations.RemoveFriend(user.login, friends_login))
			{
				// send new friends list to user one
				SendFriends(user);
				// send new friends list to user two if online
				OnlineUser friend = GetOnlineUser(friends_login);
				if (friend != null) SendFriends(friend);
			}
		}

		#endregion
	}
}
