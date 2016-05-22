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
		// logger
		private static readonly log4net.ILog log = LogHelper.GetLogger();

		/// <summary>
		/// Get id of user by his login.
		/// </summary>
		/// <param name="login">user's login.</param>
		/// <returns>user's id.</returns>
		public static int GetUserId(string login)
		{
			using (var DBcontext = new LinqToSqlDataContext())
			{
				var userId =
					from user in DBcontext.Users
					where user.login.Equals(login)
					select user.user_id;

				if (userId.Any())
					return userId.First();
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
				var userLogin =
					from user in DBcontext.Users
					where user.user_id.Equals(id)
					select user.login;

				return userLogin.FirstOrDefault();
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
					where users.login.Equals(login)
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
					where users.login.Equals(login)
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
					catch (Exception e)
					{
						log.Error(e);
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
					return users.Select(x => x.login)?.ToArray();
				else
					return null;
			}
		}

		/// <summary>
		/// Get friends of user with this id from database.
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
					where (friendship.friend_one.Equals(id) |
					friendship.friend_two.Equals(id)) &
					friendship.accepted
					select friendship;

				// list of friend's logins
				List<string> friends = new List<string>();

				if (data.Any())
				{
					foreach (Friendship f in data)
					{
						int friendId = f.friend_one.Equals(id) ? f.friend_two : f.friend_one;
						string friendLogin = GetUserLogin(friendId);
							
						if (!string.IsNullOrEmpty(friendLogin))
							friends.Add(friendLogin);
					}
				}

				return friends.ToArray();
			}
		}

		/// <summary>
		/// Add or update friendship in database.
		/// </summary>
		/// <param name="userOneId">user one id.</param>
		/// <param name="userTwoId">user two id.</param>
		/// <param name="accepted">is friendship request accepted.</param>
		/// <returns>true if operations success; otherwise, false.</returns>
		public static bool SetFriendship(int userOneId, int userTwoId, bool accepted)
		{
			using (var DBcontext = new LinqToSqlDataContext())
			{
				// get friendship records
				var data =
					from friendship in DBcontext.Friends
					where (friendship.friend_one.Equals(userOneId) &
					friendship.friend_two.Equals(userTwoId)) |
					(friendship.friend_two.Equals(userOneId) &
					friendship.friend_one.Equals(userTwoId))
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
							friend_one = userOneId,
							friend_two = userTwoId,
							accepted = accepted
						};
						DBcontext.Friends.InsertOnSubmit(f);
					}

					try
					{
						DBcontext.SubmitChanges();
						return true;
					}
					catch (Exception e)
					{
						log.Error(e);
						return false;
					}
				}
			}
		}

		/// <summary>
		/// Get array of income friendship requests for user with this id.
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
					where friendship.friend_two.Equals(id) &
					!friendship.accepted
					select friendship.friend_one;

				// list of logins
				List<string> logins = new List<string>();

				if (data.Any())
				{
					foreach (int uid in data)
					{
						string userLogin = GetUserLogin(uid);

						if (!string.IsNullOrEmpty(userLogin))
							logins.Add(userLogin);
					}
				}

				return logins.ToArray();
			}
		}

		/// <summary>
		/// Get array of outcome friendship requests for user with this id.
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
					where friendship.friend_one.Equals(id) &
					!friendship.accepted
					select friendship.friend_two;

				List<string> logins = new List<string>();

				// list of logins
				if (data.Any())
				{
					foreach (int uid in data)
					{
						string userLogin = GetUserLogin(uid);

						if (!string.IsNullOrEmpty(userLogin))
							logins.Add(userLogin);
					}
				}

				return logins.ToArray();
			}
		}

		/// <summary>
		/// Remove friendship request.
		/// </summary>
		/// <param name="userOneId">user one id.</param>
		/// <param name="userTwoId">user two id.</param>
		/// <returns>true if operations success; otherwise, false.</returns>
		public static bool RemoveFriendshipRequest(int userOneId, int userTwoId)
		{
			using (var DBcontext = new LinqToSqlDataContext())
			{
				// get friendsip requests
				var data =
					from friendship in DBcontext.Friends
					where friendship.friend_one.Equals(userOneId) &
					friendship.friend_two.Equals(userTwoId) &
					!friendship.accepted
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
				catch (Exception e)
				{
					log.Error(e);
					return false;
				}
			}
		}

		/// <summary>
		/// Remove friend.
		/// </summary>
		/// <param name="userOneId">user one id.</param>
		/// <param name="userTwoId">user two id.</param>
		/// <returns>true if operations success; otherwise, false.</returns>
		public static bool RemoveFriend(int userOneId, int userTwoId)
		{
			using (var DBcontext = new LinqToSqlDataContext())
			{
				// get friendship records
				var data =
					from friendship in DBcontext.Friends
					where (friendship.friend_one.Equals(userOneId) &
					friendship.friend_two.Equals(userTwoId) &
					friendship.accepted) |
					(friendship.friend_one.Equals(userTwoId) &
					friendship.friend_two.Equals(userOneId) &
					friendship.accepted)
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
				catch (Exception e)
				{
					log.Error(e);
					return false;
				}
			}
		}

		/// <summary>
		/// Add new reply (conversation) to database.
		/// </summary>
		/// <param name="senderId">sender's id.</param>
		/// <param name="receiverId">receiver's id.</param>
		/// <param name="text">text of reply.</param>
		/// <param name="time">time.</param>
		/// <returns>true if operations success; otherwise, false.</returns>
		public static bool AddNewReply(int senderId, int receiverId, string text, DateTime time)
		{
			using (var DBcontext = new LinqToSqlDataContext())
			{
				// get conversation id
				var data = from conversation in DBcontext.Conversations
					where (conversation.user_one.Equals(senderId) &
					conversation.user_two.Equals(receiverId)) |
					(conversation.user_one.Equals(receiverId) &
					conversation.user_two.Equals(senderId))
					select conversation.conversation_id;
				
				int conversationId;

				// conversation exist
				if (data.Any())
				{
					conversationId = data.First();
				}
				// conversation dont exist
				else
				{
					// new conversation
					Conversation conv = new Conversation
					{
						user_one = senderId,
						user_two = receiverId
					};
					DBcontext.Conversations.InsertOnSubmit(conv);

					try
					{
						DBcontext.SubmitChanges();
						conversationId = conv.conversation_id;
					}
					catch (Exception e)
					{
						log.Error(e);
						return false;
					}
				}

				// new reply
				ConversationReply reply = new ConversationReply
				{
					reply = text,
					conversation_id = conversationId,
					user_id = senderId,
					time = time
				};
				DBcontext.Conversation_replies.InsertOnSubmit(reply);

				try
				{
					DBcontext.SubmitChanges();
					return true;
				}
				catch (Exception e)
				{
					log.Error(e);
					return false;
				}
			}
		}

		/// <summary>
		/// Get array of conversation replies.
		/// </summary>
		/// <param name="userOneId">user one id.</param>
		/// <param name="userTwoId">user two id.</param>
		/// <returns>array of conversation replies.</returns>
		public static ConversationReply[] GetConversation(int userOneId, int userTwoId)
		{
			using (var DBcontext = new LinqToSqlDataContext())
			{
				// get conversation id
				var conversationId = from conversation in DBcontext.Conversations
					where (conversation.user_one.Equals(userOneId) &
					conversation.user_two.Equals(userTwoId)) |
					(conversation.user_one.Equals(userTwoId) &
					conversation.user_two.Equals(userOneId))
					select conversation.conversation_id;

				// conversation exist
				if (conversationId.Any())
				{
					var replies = from reply in DBcontext.Conversation_replies
						where reply.conversation_id.Equals(conversationId.First())
						select reply;

					if (replies.Any())
						return replies.AsEnumerable().Reverse().ToArray();
				}
				
				// no conversation or no replies
				return null;
			}
		}
	}
}
