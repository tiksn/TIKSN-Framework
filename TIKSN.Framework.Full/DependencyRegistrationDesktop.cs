﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TIKSN.Shell;

namespace TIKSN
{
    public static class DependencyRegistrationDesktop
    {
        public static void Register(IServiceCollection services)
        {
            services.TryAddSingleton<IConsoleService, ConsoleService>();
            services.TryAddSingleton<IShellCommandEngine, ShellCommandEngine>();
        }
    }
}
