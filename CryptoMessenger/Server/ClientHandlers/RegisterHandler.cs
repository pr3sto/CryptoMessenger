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
		/// <param name="login_password">Array of users login and password.</param>
		/// <returns>true, if operation had success.</returns>
		protected override bool DoRequiredOperation(string[] login_password)
		{
			System.Threading.Thread.Sleep(2000);
			return true;
		}
	}
}
