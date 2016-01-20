using System;
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
		/// Get all user from database
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
	}
}
