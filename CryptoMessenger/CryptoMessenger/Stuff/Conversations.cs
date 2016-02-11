﻿using System;
using System.Collections.Generic;

namespace CryptoMessenger.Stuff
{
	/// <summary>
	/// Reply in conversation.
	/// </summary>
	public class ConversationReply
	{
		public string author { get; private set; }
		public DateTime time { get; private set; }
		public string text { get; private set; }

		public ConversationReply(string author , DateTime time, string text)
		{
			this.author = author;
			this.time = time;
			this.text = text;
		}		
	}

	/// <summary>
	/// Represent conversation between two users.
	/// </summary>
	public class Conversation
	{
		private List<ConversationReply> replies;

		public string interlocutor { get; private set; }

		public Conversation(string interlocutor)
		{
			this.interlocutor = interlocutor;
			replies = new List<ConversationReply>();
		}

		/// <summary>
		/// Add reply in conversation.
		/// </summary>
		/// <param name="reply">reply.</param>
		public void AddReply(ConversationReply reply)
		{
			if (reply != null)
				replies.Add(reply);
		}

		/// <summary>
		/// Copies elements of the replies list to new array.
		/// </summary>
		/// <returns>array.</returns>
		public ConversationReply[] GetArrayOfReplies()
		{
			return replies.ToArray();
		}
	}

	/// <summary>
	/// All conversations of user.
	/// </summary>
	public class Conversations
	{
		private List<Conversation> conversations;

		public Conversations()
		{
			conversations = new List<Conversation>();
		}

		/// <summary>
		/// Determines whether conversation with interlocutor is in list.
		/// </summary>
		/// <param name="interlocutor">interlocutor.</param>
		/// <returns>true if contains; otherwise, false.</returns>
		public bool Contains(string interlocutor)
		{
			var data = conversations.Find(x => x.interlocutor == interlocutor);

			if (data != null)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Add new conversation.
		/// </summary>
		/// <param name="c">conversation.</param>
		public void AddConversation(Conversation c)
		{
			if (c != null)
				conversations.Add(c);
		}

		/// <summary>
		/// Get conversation with interlocutor.
		/// </summary>
		/// <param name="interlocutor">interlocutor.</param>
		/// <returns>conversation.</returns>
		public Conversation GetConversation(string interlocutor)
		{
			return conversations.Find(x => x.interlocutor == interlocutor);
		}

		/// <summary>
		/// Add reply to conversation with interlocutor.
		/// </summary>
		/// <param name="interlocutor">interlocutor.</param>
		/// <param name="reply">reply.</param>
		public void AddReply(string interlocutor, ConversationReply reply)
		{
			Conversation c = conversations.Find(x => x.interlocutor == interlocutor);
			if (c != null) c.AddReply(reply);
		}
	}
}
