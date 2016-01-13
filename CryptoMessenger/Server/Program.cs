using System;

namespace Server
{
	/// <summary>
	/// Write to console.
	/// </summary>
	class ConsoleWriter
	{
		private static object Locker = new object();

		/// <summary>
		/// Write text on console; cursor always would be on next line.
		/// </summary>
		/// <param name="text">text to write.</param>
		public static void WriteLine(string text)
		{
			lock (Locker)
			{
				int currentTopCursor = Console.CursorTop;
				int currentLeftCursor = Console.CursorLeft;

				Console.MoveBufferArea(0, currentTopCursor, Console.WindowWidth, 1, 0, currentTopCursor + 1);
				Console.CursorTop = currentTopCursor;
				Console.CursorLeft = 0;

				Console.WriteLine(text);

				Console.CursorTop = currentTopCursor + 1;
				Console.CursorLeft = currentLeftCursor;
			}
		}

		/// <summary>
		/// Write text on console; cursor always would be on next line.
		/// </summary>
		/// <param name="text">text to write.</param>
		/// <param name="arg0">object to format text.</param>
		public static void WriteLine(string text, object arg0)
		{
			WriteLine(string.Format(text, arg0));
		}

		/// <summary>
		/// Write text on console; cursor always would be on next line.
		/// </summary>
		/// <param name="text">text to write.</param>
		/// <param name="arg">array of objects to format text.</param>
		public static void WriteLine(string text, params object[] arg)
		{
			WriteLine(string.Format(text, arg));
		}
	}

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
			LoginHandler loginHandler = new LoginHandler(44301);
			RegisterHandler registrationHandler = new RegisterHandler(44302);

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
					//System.Threading.Thread.Sleep(500);
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
