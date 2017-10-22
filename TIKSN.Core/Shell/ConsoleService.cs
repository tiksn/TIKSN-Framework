using ConsoleTables;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using TIKSN.Localization;

namespace TIKSN.Shell
{
    public class ConsoleService : IConsoleService
    {
        private readonly IStringLocalizer _stringLocalizer;

        public ConsoleService(IStringLocalizer stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
        }

        public string ReadLine(string promptMessage, ConsoleColor promptForegroundColor)
        {
            WritePromptMessage(promptMessage, promptForegroundColor);
            return Console.ReadLine();
        }

        private void WritePromptMessage(string promptMessage, ConsoleColor promptForegroundColor)
        {
            ConsoleWrite(promptMessage, promptForegroundColor);
            ConsoleWrite(_stringLocalizer.GetRequiredString(LocalizationKeys.Key444677337), promptForegroundColor);
        }

        public SecureString ReadPasswordLine(string promptMessage, ConsoleColor promptForegroundColor)
        {
            WritePromptMessage(promptMessage, promptForegroundColor);

            var pwd = new SecureString();
            while (true)
            {
                var i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 0)
                    {
                        pwd.RemoveAt(pwd.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    pwd.AppendChar(i.KeyChar);
                    Console.Write("*");
                }
            }
            return pwd;
        }

        public int UserPrompt(string message, params string[] options)
        {
            if (!options.Any())
                throw new ArgumentException("User prompt must contain at least one options", nameof(options)); //TODO: localize

            while (true)
            {
                ConsoleWrite($"{message} [{string.Join("/", options)}]{_stringLocalizer.GetRequiredString(LocalizationKeys.Key444677337)}");

                var answer = Console.ReadLine();

                for (int i = 0; i < options.Length; i++)
                {
                    if (string.Equals(options[i], answer, StringComparison.OrdinalIgnoreCase))
                        return i;
                }
            }
        }

        public void WriteError(string errorMessage)
        {
            ConsoleWriteLine(errorMessage, ConsoleColor.Red);
        }

        public void WriteObject<T>(T tableValue)
        {
            var tableValues = new List<T> { tableValue };
            WriteObjects(tableValues, false);
        }

        public void WriteObjects<T>(IEnumerable<T> tableValues)
        {
            WriteObjects(tableValues, true);
        }

        private static void ConsoleWrite(string message)
        {
            Console.Write(message);
        }

        private static void ConsoleWrite(string message, ConsoleColor foreground)
        {
            var backupForeground = Console.ForegroundColor;
            Console.ForegroundColor = foreground;
            Console.Write(message);
            Console.ForegroundColor = backupForeground;
        }

        private static void ConsoleWriteLine(string message, ConsoleColor foreground)
        {
            ConsoleWrite(message, foreground);
            Console.WriteLine();
        }

        private static void WriteObjects<T>(IEnumerable<T> tableValues, bool enableCount)
        {
            var consoleTable = ConsoleTable.From(tableValues);
            consoleTable.Options.EnableCount = enableCount;
            consoleTable.Write();
            Console.WriteLine();
        }
    }
}