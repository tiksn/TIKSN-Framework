using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using TIKSN.Shell;

namespace Shell_Commander.Commands
{
    [ShellCommand("6cee4dbc-3701-434f-97fa-1a32f53159e3")]
    public class SampleCommand : ShellCommandBase
    {
        private readonly ILogger<SampleCommand> _logger;

        public SampleCommand(ILogger<SampleCommand> logger, IConsoleService consoleService) : base(consoleService)
        {
            _logger = logger;
        }

        [ShellCommandParameter("25b6d4e3-0375-4213-aff0-623b807e9f13", Mandatory = true)]
        public string SampleParameter { get; set; }

        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Sample parameter value is '{SampleParameter}'.");

            for (int i = 1; i <= 10; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                _logger.LogInformation("Attempt {0}", i);

                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}