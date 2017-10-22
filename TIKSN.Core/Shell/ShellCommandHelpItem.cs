using System.Collections.Generic;

namespace TIKSN.Shell
{
    public class ShellCommandHelpItem
    {
        public ShellCommandHelpItem(string commandName, IEnumerable<string> parameters)
        {
            CommandName = commandName;
            Parameters = string.Join(", ", parameters); //TODO: localize
        }

        public string CommandName { get; }

        public string Parameters { get; }
    }
}