using System;

namespace Encircled
{
	public class GameShouldEndException : Exception
	{
		public GameShouldEndException()
		{
		}

		public GameShouldEndException(string message)
			: base(message)
		{
		}

		public GameShouldEndException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}

