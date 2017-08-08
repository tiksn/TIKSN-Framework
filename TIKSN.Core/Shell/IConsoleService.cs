using System;
using System.Collections.Generic;
using System.Security;

namespace TIKSN.Shell
{
	public interface IConsoleService
	{
		string ReadLine(string promptMessage, ConsoleColor promptForegroundColor);

		SecureString ReadPasswordLine(string promptMessage, ConsoleColor promptForegroundColor);

		int UserPrompt(string message, params string[] options);

		void WriteError(string errorMessage);

		void WriteObject<T>(T value);

		void WriteObjects<T>(IEnumerable<T> values);
	}
}