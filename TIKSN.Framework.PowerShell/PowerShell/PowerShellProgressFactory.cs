using System.Management.Automation;
using TIKSN.Progress;

namespace TIKSN.PowerShell
{
    public class PowerShellProgressFactory : IOperationProgressFactory
    {
        private readonly Cmdlet cmdlet;

        public PowerShellProgressFactory(Cmdlet cmdlet)
        {
            this.cmdlet = cmdlet;
        }

        public DisposableProgress<OperationProgressReport> Create(string activity, string statusDescription)
        {
            return new PowerShellProgress(cmdlet, activity, statusDescription);
        }
    }
}