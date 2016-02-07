using System.Linq;
using System.Collections.Generic;

using ConversationTypes;

namespace CryptoMessenger.Stuff
{
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
			conversations.Add(c);
		}

		/// <summary>
		/// Get conversation with interlocutor.
		/// </summary>
		/// <param name="interlocutor">interlocutor.</param>
		/// <returns>conversation.</returns>
		public Conversation GetConversation(string interlocutor)
		{
			if (Contains(interlocutor))
				return conversations.Find(x => x.interlocutor == interlocutor);
			else
				return null;
		}

		/// <summary>
		/// Add reply to conversation with interlocutor.
		/// </summary>
		/// <param name="interlocutor">interlocutor.</param>
		/// <param name="reply">reply.</param>
		public void AddReply(string interlocutor, ConversationReply reply)
		{
			Conversation c = conversations.Find(x => x.interlocutor == interlocutor);
			if (c != null) c.replies.Add(reply);
		}
	}
}
