using System.Collections.Generic;

namespace TIKSN.Shell
{
    public class ShellCommandHelpItem
    {
        public ShellCommandHelpItem(string commandName, IEnumerable<string> parameters)
        {
            this.CommandName = commandName;
            this.Parameters = string.Join(", ", parameters); //TODO: localize
        }

        public string CommandName { get; }

        public string Parameters { get; }
    }
}
