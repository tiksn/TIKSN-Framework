using System;

namespace TIKSN.Shell
{
	public class ShellCommandSuspendException : Exception
	{
		public ShellCommandSuspendException() { }
		public ShellCommandSuspendException(string message) : base(message) { }
		public ShellCommandSuspendException(string message, Exception inner) : base(message, inner) { }
	}
}
