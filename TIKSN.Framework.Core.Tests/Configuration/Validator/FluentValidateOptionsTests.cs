using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Shouldly;
using TIKSN.Settings;
using Xunit;

namespace TIKSN.Tests.Configuration.Validator;

public class FluentValidateOptionsTests
{
    [Fact]
    public void GivenInvalidOptions_WhenHostStarted_ThenItShouldThrowOptionsValidationException()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();

        using var host = new HostBuilder()
            .ConfigureServices(services =>
            {
                _ = services
                    .AddOptions<FileSettingsServiceOptions>()
                    .Bind(configuration)
                    .ValidateOnStart();
                _ = services.AddSingleton<IValidateOptions<FileSettingsServiceOptions>,
                    FileSettingsServiceOptionsValidator>();
            })
            .Build();

        _ = Should.Throw<OptionsValidationException>(host.Start);
    }
}
