using Microsoft.Extensions.Localization;
using System;
using TIKSN.Localization;

namespace TIKSN.Shell
{
    public class ShellCommandContext : IShellCommandContext, IShellCommandContextStore
    {
        private readonly IConsoleService _consoleService;
        private readonly IStringLocalizer _stringLocalizer;

        private string commandName;
        private bool noToAll;
        private bool yesToAll;

        public ShellCommandContext(IStringLocalizer stringLocalizer, IConsoleService consoleService)
        {
            _stringLocalizer = stringLocalizer;
            _consoleService = consoleService;
        }

        public void SetCommandName(string commandName)
        {
            this.commandName = commandName;
        }

        public bool ShouldContinue(string query, string caption)
        {
            var message = $"{caption}{Environment.NewLine}{query}";

            var answer = _consoleService.UserPrompt(message,
                _stringLocalizer.GetRequiredString(LocalizationKeys.Key592470584),
                _stringLocalizer.GetRequiredString(LocalizationKeys.Key132999259),
                _stringLocalizer.GetRequiredString(LocalizationKeys.Key777755530));

            switch (answer)
            {
                case 0:
                    return true;

                case 1:
                    return false;

                case 2:
                    throw new ShellCommandSuspendedException();

                default:
                    throw new NotSupportedException();
            }
        }

        public bool ShouldContinue(string query, string caption, ref bool yesToAll, ref bool noToAll)
        {
            if (yesToAll)
                return true;

            if (noToAll)
                return false;
            var message = $"{caption}{Environment.NewLine}{query}";

            var answer = _consoleService.UserPrompt(message,
                _stringLocalizer.GetRequiredString(LocalizationKeys.Key592470584),
                _stringLocalizer.GetRequiredString(LocalizationKeys.Key268507527),
                _stringLocalizer.GetRequiredString(LocalizationKeys.Key132999259),
                _stringLocalizer.GetRequiredString(LocalizationKeys.Key191067042),
                _stringLocalizer.GetRequiredString(LocalizationKeys.Key777755530));

            switch (answer)
            {
                case 0:
                    return true;

                case 1:
                    yesToAll = true;
                    return true;

                case 2:
                    return false;

                case 3:
                    noToAll = true;
                    return false;

                case 4:
                    throw new ShellCommandSuspendedException();

                default:
                    throw new NotSupportedException();
            }
        }

        public bool ShouldProcess(string target, string action)
        {
            return ShouldContinue(_stringLocalizer.GetRequiredString(LocalizationKeys.Key439104548),
                string.Format(_stringLocalizer.GetRequiredString(LocalizationKeys.Key284914810), action, target),
                ref yesToAll, ref noToAll);
        }

        public bool ShouldProcess(string target)
        {
            return ShouldProcess(target, commandName);
        }

        public bool ShouldProcess(string verboseDescription, string verboseWarning, string caption)
        {
            return ShouldContinue(verboseWarning, caption, ref yesToAll, ref noToAll);
        }
    }
}