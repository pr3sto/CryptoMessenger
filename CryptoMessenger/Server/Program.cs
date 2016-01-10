using System;

namespace Server
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.Title = "Server";

			Console.WriteLine("Server started.");
			Console.WriteLine("===============");
			Console.WriteLine("Admin commands:");
			Console.WriteLine("start - Start listening to clients.");
			Console.WriteLine("stop - Stop listening to clients and wait for processing clients.");
			Console.WriteLine("exit - Close server.");
			Console.WriteLine();

			// client handlers
			LoginHandler loginHandler = new LoginHandler(4431);
			RegistrationHandler registrationHandler = new RegistrationHandler(4432);			

			// admin commands
			while (true)
			{
				string command = Console.ReadLine();

				if ("start".Equals(command))
				{
					loginHandler.StartAcceptClients();
					registrationHandler.StartAcceptClients();
				}
				else if ("stop".Equals(command))
				{
					loginHandler.StopAcceptClients();
					registrationHandler.StopAcceptClients();
				}
				else if ("exit".Equals(command))
				{
					return;
				}
			}
		}
	}
}
