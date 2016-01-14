using System.Linq;

namespace Server
{
	class RegisterHandler : BasicLoginRegisterHandler
	{
		/// <summary>
		/// Initialize Registration Handler, that listen to clients.
		/// </summary>
		public RegisterHandler(int port) : base(port)
		{
		}

		/// <summary>
		/// Do required operation (registration).
		/// </summary>
		/// <param name="_login">user's login.</param>
		/// <param name="_password">user's password.</param>
		/// <returns>true, if operation had success.</returns>
		protected override bool DoRequiredOperation(string _login, string _password)
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
					password = _password
				};
				DBcontext.Users.InsertOnSubmit(newUser);

				try
				{
					DBcontext.SubmitChanges();
				}
				catch
				{
					// TODO logger
					return false; 
				}

				return true;
			} 
		}
	}
}
