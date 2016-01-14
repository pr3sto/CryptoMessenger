using System.Linq;

namespace Server
{
	class LoginHandler : BasicLoginRegisterHandler
	{
		/// <summary>
		/// Initialize Login Handler, that listen to clients.
		/// </summary>
		public LoginHandler(int port) : base(port)
		{
		}

		/// <summary>
		/// Do required operation (login).
		/// </summary>
		/// <param name="_login">user's login.</param>
		/// <param name="_password">user's password.</param>
		/// <returns>true, if operation had success.</returns>
		protected override bool DoRequiredOperation(string _login, string _password)
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
					if (user.password == _password)
						return true;
					else
						return false;
				}

				return false; // something wrong...
			}
			else
			{
				// user not registered
				return false;
			}
		}
	}
}
