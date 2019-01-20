using System;

namespace TIKSN.PowerShell
{
    public class CurrentCommandContext : ICurrentCommandStore, ICurrentCommandProvider
    {
        private CommandBase _command;

        public CommandBase GetCurrentCommand()
        {
            if (_command == null)
            {
                throw new NullReferenceException("Command is not set yet.");
            }

            return _command;
        }

        public void SetCurrentCommand(CommandBase command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            _command = command;
        }
    }
}