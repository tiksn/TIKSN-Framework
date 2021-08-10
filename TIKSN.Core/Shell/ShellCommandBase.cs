using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TIKSN.Shell
{
    public abstract class ShellCommandBase : IShellCommand
    {
        private readonly IConsoleService _consoleService;

        protected ShellCommandBase(IConsoleService consoleService) => this._consoleService = consoleService;

        public abstract Task ExecuteAsync(CancellationToken cancellationToken);

        protected void WriteObject<T>(T tableValue) => this._consoleService.WriteObject(tableValue);

        protected void WriteObjects<T>(IEnumerable<T> tableValues) => this._consoleService.WriteObjects(tableValues);
    }
}
