using System;

namespace TIKSN.Shell
{
	public class ShellSuspendCommandException : Exception
	{
		public ShellSuspendCommandException() { }
		public ShellSuspendCommandException(string message) : base(message) { }
		public ShellSuspendCommandException(string message, Exception inner) : base(message, inner) { }
	}
}
