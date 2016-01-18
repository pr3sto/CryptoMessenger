﻿using System;

namespace Server
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.Title = "Server";

			// server
			Server server = new Server(44300);

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
					server.Start();
				}
				else if ("stop".Equals(command))
				{
					server.Stop();
				}
				else if ("exit".Equals(command))
				{
					return;
				}
			}
		}
	}
}
