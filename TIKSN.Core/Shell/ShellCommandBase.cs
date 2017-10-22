using System.Collections.Generic;
using System.Threading.Tasks;

namespace TIKSN.Shell
{
    public abstract class ShellCommandBase : IShellCommand
    {
        private readonly IConsoleService _consoleService;

        public abstract Task ExecuteAsync();

        protected ShellCommandBase(IConsoleService consoleService)
        {
            _consoleService = consoleService;
        }

        protected void WriteObject<T>(T tableValue)
        {
            _consoleService.WriteObject<T>(tableValue);
        }

        protected void WriteObjects<T>(IEnumerable<T> tableValues)
        {
            _consoleService.WriteObjects<T>(tableValues);
        }
    }
}