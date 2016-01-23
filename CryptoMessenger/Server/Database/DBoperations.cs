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
		/// <param name="_login">user's login.</param>
		/// <param name="_password">user's password.</param>
		/// <param name="_id">client's id in db.</param>
		/// <returns>true, if operation had success.</returns>
		public static bool Login(string _login, string _password, out int _id)
		{
			using (LinqToSqlDataContext DBcontext = new LinqToSqlDataContext())
			{
				// get user
				var data =
					from user in DBcontext.Users
					where user.login == _login
					select user;

				if (data.Any())
				{
					foreach (User user in data)
					{
						if (PasswordHash.PasswordHash.ValidatePassword(_password, user.password))
						{
							_id = user.user_id;
							return true;
						}
						else
						{
							_id = 0;
							return false;
						}
					}

					_id = 0;
					return false; // something wrong
				}
				else
				{
					// user not registered
					_id = 0;
					return false;
				}
			}
		}

		/// <summary>
		/// Do registration.
		/// </summary>
		/// <param name="_login">user's login.</param>
		/// <param name="_password">user's password.</param>
		/// <returns>true, if operation had success.</returns>
		public static bool Register(string _login, string _password)
		{
			using (LinqToSqlDataContext DBcontext = new LinqToSqlDataContext())
			{
				// if login already exist
				var data =
					from user in DBcontext.Users
					where user.login == _login
					select user;

				if (data.Any())
				{
					// login already exist
					return false;
				}
				else
				{
					// new user
					User newUser = new User
					{
						login = _login,
						password = PasswordHash.PasswordHash.CreateHash(_password)
					};
					DBcontext.Users.InsertOnSubmit(newUser);

					try
					{
						DBcontext.SubmitChanges();
						Console.WriteLine(" - new user: {0}", _login);
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
				var data =
					from user in DBcontext.Users
					select user;

				return data.Select(x => x.login).ToArray();
			}
		}

		/// <summary>
		/// Get friends from database.
		/// </summary>
		/// <returns>array with all users.</returns>
		public static string[] GetFriends(string login)
		{
			using (LinqToSqlDataContext DBcontext = new LinqToSqlDataContext())
			{
				var uid =
					from user in DBcontext.Users
					where user.login == login
					select user.user_id;

				if (!uid.Any()) return null;

				var data1 =
					from friendship in DBcontext.Friends
					where friendship.friend_one == uid.First() &
					friendship.status == true
					select friendship.friend_two;

				var data2 =
					from friendship in DBcontext.Friends
					where friendship.friend_two == uid.First() &
					friendship.status == true
					select friendship.friend_one;

				List<string> friends = new List<string>();
				if (data1.Any())
				{
					foreach (int id in data1)
					{
						var ulogin =
							from user in DBcontext.Users
							where user.user_id == id
							select user.login;

						friends.Add(ulogin.First());
					}
				}
				if (data2.Any())
				{
					foreach (int id in data2)
					{
						var ulogin =
							from user in DBcontext.Users
							where user.user_id == id
							select user.login;

						friends.Add(ulogin.First());
					}
				}

				if (friends.Count == 0)
					return null;
				else
					return friends.ToArray();
			}
		}

		/// <summary>
		/// Add or update friendship in database.
		/// </summary>
		/// <param name="_status">friendship status.</param>
		/// <param name="user_one">user one login.</param>
		/// <param name="user_two">user two login.</param>
		public static void SetFriendship(bool _status, string user_one, string user_two)
		{
			using (LinqToSqlDataContext DBcontext = new LinqToSqlDataContext())
			{
				var user_one_id =
					from user in DBcontext.Users
					where user.login == user_one
					select user.user_id;

				if (!user_one_id.Any()) return;

				var user_two_id =
					from user in DBcontext.Users
					where user.login == user_two
					select user.user_id;

				if (!user_two_id.Any()) return;

				var data =
					from friendship in DBcontext.Friends
					where friendship.friend_one == user_one_id.First() &
					friendship.friend_two == user_two_id.First()
					select friendship;

				if (data.Any() && data.First().status == _status)
				{
					return;
				}
				else 
				{

					if (data.Any() && data.First().status != _status)
					{
						data.First().status = _status;
					}
					else
					{
						Friendship f = new Friendship
						{
							friend_one = user_one_id.First(),
							friend_two = user_two_id.First(),
							status = _status
						};
						DBcontext.Friends.InsertOnSubmit(f);
					}

					try
					{
						DBcontext.SubmitChanges();
					}
					catch
					{
						// TODO logger
					}
				}
			}
		}

		public static string[] GetIncomeRequests(string login)
		{
			using (LinqToSqlDataContext DBcontext = new LinqToSqlDataContext())
			{
				var id =
					from user in DBcontext.Users
					where user.login == login
					select user.user_id;

				if (!id.Any()) return null;

				var data =
					from friendship in DBcontext.Friends
					where friendship.friend_two == id.First() &
					friendship.status == false
					select friendship.friend_one;

				if (data.Any())
				{
					List<string> logins = new List<string>();
					foreach (int uid in data)
					{
						var ulogin =
							from user in DBcontext.Users
							where user.user_id == uid
							select user.login;

						logins.Add(ulogin.First());
					}

					return logins.ToArray();
				}
				else
				{
					return null;
				}
			}
		}

		public static string[] GetOutcomeRequests(string login)
		{
			using (LinqToSqlDataContext DBcontext = new LinqToSqlDataContext())
			{
				var id =
					from user in DBcontext.Users
					where user.login == login
					select user.user_id;

				if (!id.Any()) return null;

				var data =
					from friendship in DBcontext.Friends
					where friendship.friend_one == id.First() &
					friendship.status == false
					select friendship.friend_two;

				if (data.Any())
				{
					List<string> logins = new List<string>();
					foreach (int uid in data)
					{
						var ulogin =
							from user in DBcontext.Users
							where user.user_id == uid
							select user.login;

						logins.Add(ulogin.First());
					}

					return logins.ToArray();
				}
				else
				{
					return null;
				}
			}
		}

		public static void RemoveFriendshipRequest(string user_one, string user_two)
		{
			using (LinqToSqlDataContext DBcontext = new LinqToSqlDataContext())
			{
				var user_one_id =
					from user in DBcontext.Users
					where user.login == user_one
					select user.user_id;

				if (!user_one_id.Any()) return;

				var user_two_id =
					from user in DBcontext.Users
					where user.login == user_two
					select user.user_id;

				if (!user_two_id.Any()) return;

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
				}
				catch
				{
					
				}
			}
		}

		public static void RemoveFriend(string user_one, string user_two)
		{
			using (LinqToSqlDataContext DBcontext = new LinqToSqlDataContext())
			{
				var user_one_id =
					from user in DBcontext.Users
					where user.login == user_one
					select user.user_id;

				if (!user_one_id.Any()) return;

				var user_two_id =
					from user in DBcontext.Users
					where user.login == user_two
					select user.user_id;

				if (!user_two_id.Any()) return;

				var data =
					from friendship in DBcontext.Friends
					where (friendship.friend_one == user_one_id.First() &
					friendship.friend_two == user_two_id.First()) |
					(friendship.friend_one == user_two_id.First() &
					friendship.friend_two == user_one_id.First())
					select friendship;

				foreach (var friendship in data)
				{
					DBcontext.Friends.DeleteOnSubmit(friendship);
				}

				try
				{
					DBcontext.SubmitChanges();
				}
				catch
				{

				}
			}
		}
	}
}
