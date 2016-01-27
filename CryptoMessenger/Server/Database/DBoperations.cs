using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Database
{
	class DBoperations
	{
		/// <summary>
		/// Do login.
		/// </summary>
		/// <param name="login">user's login.</param>
		/// <param name="password">user's password.</param>
		/// <param name="id">client's id in db.</param>
		/// <returns>true, if operation had success.</returns>
		public static bool Login(string login, string password, out int id)
		{
			using (LinqToSqlDataContext DBcontext = new LinqToSqlDataContext())
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
		/// <returns>true, if operation had success.</returns>
		public static bool Register(string login, string password)
		{
			using (LinqToSqlDataContext DBcontext = new LinqToSqlDataContext())
			{
				var user =
					from users in DBcontext.Users
					where users.login == login
					select users;

				if (user.Any())
				{
					// login already exist
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
						Console.WriteLine(" - new user: {0}", login);
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
			using (LinqToSqlDataContext DBcontext = new LinqToSqlDataContext())
			{
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
		/// <returns>array with friends.</returns>
		public static string[] GetFriends(string login)
		{
			using (LinqToSqlDataContext DBcontext = new LinqToSqlDataContext())
			{
				var user_id =
					from user in DBcontext.Users
					where user.login == login
					select user.user_id;

				// user not found
				if (!user_id.Any()) return null;

				// user id
				int id = user_id.First();

				// select friends
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

						var friend_login =
							from user in DBcontext.Users
							where user.user_id == friend_id
							select user.login;

						if (friend_login.Any())
							friends.Add(friend_login.First());
					}
				}

				return friends.ToArray();
			}
		}

		/// <summary>
		/// Add or update friendship in database.
		/// </summary>
		/// <param name="accepted">is friendship request accepted.</param>
		/// <param name="user_one">user one login.</param>
		/// <param name="user_two">user two login.</param>
		/// <returns>true, if operations success.</returns>
		public static bool SetFriendship(bool accepted, string user_one, string user_two)
		{
			using (LinqToSqlDataContext DBcontext = new LinqToSqlDataContext())
			{
				var user_one_id =
					from user in DBcontext.Users
					where user.login == user_one
					select user.user_id;

				// user not found
				if (!user_one_id.Any()) return false;

				var user_two_id =
					from user in DBcontext.Users
					where user.login == user_two
					select user.user_id;

				// user not found
				if (!user_two_id.Any()) return false;

				var data =
					from friendship in DBcontext.Friends
					where (friendship.friend_one == user_one_id.First() &
					friendship.friend_two == user_two_id.First()) |
					(friendship.friend_two == user_one_id.First() &
					friendship.friend_one == user_two_id.First())
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
							friend_one = user_one_id.First(),
							friend_two = user_two_id.First(),
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
		/// <param name="login">user's login.</param>
		/// <returns>array of income friendship requests.</returns>
		public static string[] GetIncomeFriendshipRequests(string login)
		{
			using (LinqToSqlDataContext DBcontext = new LinqToSqlDataContext())
			{
				var user_id =
					from user in DBcontext.Users
					where user.login == login
					select user.user_id;

				// user not found
				if (!user_id.Any()) return null;

				// get friendsip requests
				var data =
					from friendship in DBcontext.Friends
					where friendship.friend_two == user_id.First() &
					friendship.accepted == false
					select friendship.friend_one;

				List<string> logins = new List<string>();

				if (data.Any())
				{
					foreach (int id in data)
					{
						var user_login =
							from user in DBcontext.Users
							where user.user_id == id
							select user.login;

						if (user_login.Any())
							logins.Add(user_login.First());
					}					
				}

				return logins.ToArray();
			}
		}

		/// <summary>
		/// Get array of outcome friendship requests.
		/// </summary>
		/// <param name="login">user's login.</param>
		/// <returns>array of outcome friendship requests.</returns>
		public static string[] GetOutcomeFriendshipRequests(string login)
		{
			using (LinqToSqlDataContext DBcontext = new LinqToSqlDataContext())
			{
				var user_id =
					from user in DBcontext.Users
					where user.login == login
					select user.user_id;

				// user not found
				if (!user_id.Any()) return null;

				// get friendsip requests
				var data =
					from friendship in DBcontext.Friends
					where friendship.friend_one == user_id.First() &
					friendship.accepted == false
					select friendship.friend_two;

				List<string> logins = new List<string>();

				if (data.Any())
				{	
					foreach (int id in data)
					{
						var user_login =
							from user in DBcontext.Users
							where user.user_id == id
							select user.login;

						if (user_login.Any())
							logins.Add(user_login.First());
					}						
				}

				return logins.ToArray();
			}
		}

		/// <summary>
		/// Remove friendship request.
		/// </summary>
		/// <param name="user_one">user one login.</param>
		/// <param name="user_two">user two login.</param>
		/// <returns>true, if operations success.</returns>
		public static bool RemoveFriendshipRequest(string user_one, string user_two)
		{
			using (LinqToSqlDataContext DBcontext = new LinqToSqlDataContext())
			{
				var user_one_id =
					from user in DBcontext.Users
					where user.login == user_one
					select user.user_id;

				// user not found
				if (!user_one_id.Any()) return false;

				var user_two_id =
					from user in DBcontext.Users
					where user.login == user_two
					select user.user_id;

				// user not found
				if (!user_two_id.Any()) return false;

				var data =
					from friendship in DBcontext.Friends
					where friendship.friend_one == user_one_id.First() &
					friendship.friend_two == user_two_id.First()
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
		/// <param name="user_one">user one login.</param>
		/// <param name="user_two">user two login.</param>
		/// <returns>true, if operations success.</returns>
		public static bool RemoveFriend(string user_one, string user_two)
		{
			using (LinqToSqlDataContext DBcontext = new LinqToSqlDataContext())
			{
				var user_one_id =
					from user in DBcontext.Users
					where user.login == user_one
					select user.user_id;

				// user not found
				if (!user_one_id.Any()) return false;

				var user_two_id =
					from user in DBcontext.Users
					where user.login == user_two
					select user.user_id;

				// user not found
				if (!user_two_id.Any()) return false;

				var data =
					from friendship in DBcontext.Friends
					where (friendship.friend_one == user_one_id.First() &
					friendship.friend_two == user_two_id.First() &
					friendship.accepted == true) |
					(friendship.friend_one == user_two_id.First() &
					friendship.friend_two == user_one_id.First() &
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
	}
}
