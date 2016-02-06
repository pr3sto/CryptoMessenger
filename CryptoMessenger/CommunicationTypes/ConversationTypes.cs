using System;
using System.Collections.Generic;

namespace ConversationTypes
{
	/// <summary>
	/// Reply in conversation.
	/// </summary>
	[Serializable]
	public class ConversationReply
	{
		public string author { get; set; }
		public DateTime time { get; set; }
		public string text { get; set; }
	}

	/// <summary>
	/// Represent conversation between two users.
	/// </summary>
	[Serializable]
	public class Conversation
	{
		public string interlocutor { get; set; }
		public List<ConversationReply> replies { get; set; }
	}
}
