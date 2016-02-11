using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Database
{
	/// <summary>
	/// Operations with database.
	/// </summary>
	static class DBoperations
	{
		/// <summary>
		/// Get id of user by his login.
		/// </summary>
		/// <param name="login">user's login.</param>
		/// <returns>user's id.</returns>
		public static int GetUserId(string login)
		{
			using (var DBcontext = new LinqToSqlDataContext())
			{
				var user_id =
					from user in DBcontext.Users
					where user.login == login
					select user.user_id;

				if (user_id.Any())
					return user_id.First();
				else
					return 0;
			}
		}

		/// <summary>
		/// Get login of user by his id.
		/// </summary>
		/// <param name="id">user's id.</param>
		/// <returns>user's login.</returns>
		public static string GetUserLogin(int id)
		{
			using (var DBcontext = new LinqToSqlDataContext())
			{
				var user_login =
					from user in DBcontext.Users
					where user.user_id == id
					select user.login;

				if (user_login.Any())
					return user_login.First();
				else
					return null;
			}
		}

		/// <summary>
		/// Do login.
		/// </summary>
		/// <param name="login">user's login.</param>
		/// <param name="password">user's password.</param>
		/// <param name="id">client's id in db.</param>
		/// <returns>true if operation had success; otherwise, false.</returns>
		public static bool Login(string login, string password, out int id)
		{
			using (var DBcontext = new LinqToSqlDataContext())
			{
				// get user
				var user =
					from users in DBcontext.Users
					where users.login == login
					select users;

				if (user.Any())
				{
					if (PasswordHash.PasswordHash.ValidatePassword(password, user.First().password))
					{
						// login success
						id = user.First().user_id;
						return true;
					}
					else
					{
						// login fail
						id = 0;
						return false;
					}
				}
				else
				{
					// user not registered
					id = 0;
					return false;
				}
			}
		}

		/// <summary>
		/// Do registration.
		/// </summary>
		/// <param name="login">user's login.</param>
		/// <param name="password">user's password.</param>
		/// <returns>true if operation had success; otherwise, false.</returns>
		public static bool Register(string login, string password)
		{
			using (var DBcontext = new LinqToSqlDataContext())
			{
				// get user
				var user =
					from users in DBcontext.Users
					where users.login == login
					select users;

				if (user.Any())
				{
					// account already exist
					return false;
				}
				else
				{
					// new user
					User newUser = new User
					{
						login = login,
						password = PasswordHash.PasswordHash.CreateHash(password)
					};
					DBcontext.Users.InsertOnSubmit(newUser);

					try
					{
						DBcontext.SubmitChanges();
						return true;
					}
					catch
					{
						// TODO logger
						return false;
					}
				}
			}
		}

		/// <summary>
		/// Get all user from database.
		/// </summary>
		/// <returns>array with all users.</returns>
		public static string[] GetAllUsers()
		{
			using (var DBcontext = new LinqToSqlDataContext())
			{
				// get all users
				var users =
					from user in DBcontext.Users
					select user;

				if (users.Any())
					return users.Select(x => x.login).ToArray();
				else
					return null;
			}
		}

		/// <summary>
		/// Get friends from database.
		/// </summary>
		/// <param name="id">user's id</param>
		/// <returns>array with friends.</returns>
		public static string[] GetFriends(int id)
		{
			using (var DBcontext = new LinqToSqlDataContext())
			{
				// get friends
				var data =
					from friendship in DBcontext.Friends
					where (friendship.friend_one == id |
					friendship.friend_two == id) &
					friendship.accepted == true
					select friendship;

				// list of friend's logins
				List<string> friends = new List<string>();

				if (data.Any())
				{
					foreach (Friendship f in data)
					{
						int friend_id = f.friend_one == id ? f.friend_two : f.friend_one;
						string friend_login = GetUserLogin(friend_id);
							
						if (!string.IsNullOrEmpty(friend_login))
							friends.Add(friend_login);
					}
				}

				return friends.ToArray();
			}
		}

		/// <summary>
		/// Add or update friendship in database.
		/// </summary>
		/// <param name="accepted">is friendship request accepted.</param>
		/// <param name="user_one_id">user one id.</param>
		/// <param name="user_two_id">user two id.</param>
		/// <returns>true if operations success; otherwise, false.</returns>
		public static bool SetFriendship(bool accepted, int user_one_id, int user_two_id)
		{
			using (var DBcontext = new LinqToSqlDataContext())
			{
				// get friendship records
				var data =
					from friendship in DBcontext.Friends
					where (friendship.friend_one == user_one_id &
					friendship.friend_two == user_two_id) |
					(friendship.friend_two == user_one_id &
					friendship.friend_one == user_two_id)
					select friendship;

				// already friends
				if (data.Any() && data.First().accepted)
				{
					return false;
				}
				else
				{
					// accept friendship request
					if (data.Any() && !data.First().accepted)
					{
						data.First().accepted = accepted;
					}
					// add friendship request
					else
					{
						Friendship f = new Friendship
						{
							friend_one = user_one_id,
							friend_two = user_two_id,
							accepted = accepted
						};
						DBcontext.Friends.InsertOnSubmit(f);
					}

					try
					{
						DBcontext.SubmitChanges();
						return true;
					}
					catch
					{
						// TODO logger
						return false;
					}
				}
			}
		}

		/// <summary>
		/// Get array of income friendship requests.
		/// </summary>
		/// <param name="id">user's id.</param>
		/// <returns>array of income friendship requests.</returns>
		public static string[] GetIncomeFriendshipRequests(int id)
		{
			using (var DBcontext = new LinqToSqlDataContext())
			{
				// get friendsip requests
				var data =
					from friendship in DBcontext.Friends
					where friendship.friend_two == id &
					friendship.accepted == false
					select friendship.friend_one;

				// list of logins
				List<string> logins = new List<string>();

				if (data.Any())
				{
					foreach (int uid in data)
					{
						string user_login = GetUserLogin(uid);

						if (!string.IsNullOrEmpty(user_login))
							logins.Add(user_login);
					}
				}

				return logins.ToArray();
			}
		}

		/// <summary>
		/// Get array of outcome friendship requests.
		/// </summary>
		/// <param name="id">user's id.</param>
		/// <returns>array of outcome friendship requests.</returns>
		public static string[] GetOutcomeFriendshipRequests(int id)
		{
			using (var DBcontext = new LinqToSqlDataContext())
			{
				// get friendsip requests
				var data =
					from friendship in DBcontext.Friends
					where friendship.friend_one == id &
					friendship.accepted == false
					select friendship.friend_two;

				List<string> logins = new List<string>();

				// list of logins
				if (data.Any())
				{
					foreach (int uid in data)
					{
						string user_login = GetUserLogin(uid);

						if (!string.IsNullOrEmpty(user_login))
							logins.Add(user_login);
					}
				}

				return logins.ToArray();
			}
		}

		/// <summary>
		/// Remove friendship request.
		/// </summary>
		/// <param name="user_one_id">user one id.</param>
		/// <param name="user_two_id">user two id.</param>
		/// <returns>true if operations success; otherwise, false.</returns>
		public static bool RemoveFriendshipRequest(int user_one_id, int user_two_id)
		{
			using (var DBcontext = new LinqToSqlDataContext())
			{
				// get friendsip requests
				var data =
					from friendship in DBcontext.Friends
					where friendship.friend_one == user_one_id &
					friendship.friend_two == user_two_id &
					friendship.accepted == false
					select friendship;

				foreach (var friendship in data)
				{
					DBcontext.Friends.DeleteOnSubmit(friendship);
				}

				try
				{
					DBcontext.SubmitChanges();
					return true;
				}
				catch
				{
					// TODO logger
					return false;
				}
			}
		}

		/// <summary>
		/// Remove friend.
		/// </summary>
		/// <param name="user_one_id">user one id.</param>
		/// <param name="user_two_id">user two id.</param>
		/// <returns>true if operations success; otherwise, false.</returns>
		public static bool RemoveFriend(int user_one_id, int user_two_id)
		{
			using (var DBcontext = new LinqToSqlDataContext())
			{
				// get friendship records
				var data =
					from friendship in DBcontext.Friends
					where (friendship.friend_one == user_one_id &
					friendship.friend_two == user_two_id &
					friendship.accepted == true) |
					(friendship.friend_one == user_two_id &
					friendship.friend_two == user_one_id &
					friendship.accepted == true)
					select friendship;

				foreach (var friendship in data)
				{
					DBcontext.Friends.DeleteOnSubmit(friendship);
				}

				try
				{
					DBcontext.SubmitChanges();
					return true;
				}
				catch
				{
					// TODO logger
					return false;
				}
			}
		}

		/// <summary>
		/// Add new reply (conversation) to database.
		/// </summary>
		/// <param name="sender_id">sender's id.</param>
		/// <param name="receiver_id">receiver's id.</param>
		/// <param name="text">text of reply.</param>
		/// <param name="time">time.</param>
		/// <returns>true if operations success; otherwise, false.</returns>
		public static bool AddNewReply(int sender_id, int receiver_id, string text, DateTime time)
		{
			using (var DBcontext = new LinqToSqlDataContext())
			{
				// get conversation id
				var data = from conversation in DBcontext.Conversations
					where (conversation.user_one == sender_id &
					conversation.user_two == receiver_id) |
					(conversation.user_one == receiver_id &
					conversation.user_two == sender_id)
					select conversation.conversation_id;

				// conversation_id
				int c_id;

				// conversation exist
				if (data.Any())
				{
					c_id = data.First();
				}
				// conversation dont exist
				else
				{
					// new conversation
					Conversation conv = new Conversation
					{
						user_one = sender_id,
						user_two = receiver_id
					};
					DBcontext.Conversations.InsertOnSubmit(conv);

					try
					{
						DBcontext.SubmitChanges();
						c_id = conv.conversation_id;
					}
					catch
					{
						// TODO logger
						return false;
					}
				}

				// new reply
				ConversationReply reply = new ConversationReply
				{
					reply = text,
					conversation_id = c_id,
					user_id = sender_id,
					time = time
				};
				DBcontext.Conversation_replies.InsertOnSubmit(reply);

				try
				{
					DBcontext.SubmitChanges();
					return true;
				}
				catch
				{
					// TODO logger
					return false;
				}
			}
		}

		/// <summary>
		/// Get array of conversation replies.
		/// </summary>
		/// <param name="user_one_id">user one id.</param>
		/// <param name="user_two_id">user two id.</param>
		/// <returns>array of conversation replies.</returns>
		public static ConversationReply[] GetConversation(int user_one_id, int user_two_id)
		{
			using (var DBcontext = new LinqToSqlDataContext())
			{
				// get conversation id
				var c_id = from conversation in DBcontext.Conversations
					where (conversation.user_one == user_one_id &
					conversation.user_two == user_two_id) |
					(conversation.user_one == user_two_id &
					conversation.user_two == user_one_id)
					select conversation.conversation_id;

				// conversation exist
				if (c_id.Any())
				{
					var replies = from reply in DBcontext.Conversation_replies
						where reply.conversation_id == c_id.First()
						select reply;

					if (replies.Any())
						return replies.ToArray();
				}
				
				// no conversation or no replies
				return null;
			}
		}
	}
}
