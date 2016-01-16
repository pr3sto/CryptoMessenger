using System;

namespace Server
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.Title = "Server";

			// client handlers
			LoginHandler loginHandler = new LoginHandler(44301);
			RegisterHandler registrationHandler = new RegisterHandler(44302);

			// commands
			Console.WriteLine("Server is running.");
			Console.WriteLine("===============");
			Console.WriteLine("Commands:");
			Console.WriteLine("start - Start listening to clients.");
			Console.WriteLine("stop - Stop listening to clients and wait for processing clients.");
			Console.WriteLine("exit - Close server.");
			Console.WriteLine();

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
