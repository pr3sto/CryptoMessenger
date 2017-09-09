using System;

// logger config
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Server
{
	class Program
	{
		/// <summary>
		/// The main entry point for the server application.
		/// </summary>
		static void Main()
		{
			Console.Title = "Server is stopped";

			Server server = new Server(443);

			// commands
			Console.WriteLine("Server is stopped.");
			Console.WriteLine("==================");
			Console.WriteLine("Commands:");
			Console.WriteLine("start - Start listening to clients.");
			Console.WriteLine("stop - Stop listening to clients.");
			Console.WriteLine("exit - Close server.");
			Console.WriteLine();

			while (true)
			{
				string command = Console.ReadLine();

				if ("start".Equals(command))
				{
					server.Start();
					Console.Title = "Server is running";
				}
				else if ("stop".Equals(command))
				{
					server.Stop();
					Console.Title = "Server is stopped";
				}
				else if ("exit".Equals(command))
					return;
			}
		}
	}
}
