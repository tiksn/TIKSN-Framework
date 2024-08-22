using System.Globalization;
using Microsoft.Extensions.Hosting;
using TIKSN.Deployment;

namespace TIKSN.Deployment;

/// <summary>
/// Extension methods for <see cref="IHostEnvironment"/>.
/// </summary>
public static class HostEnvironmentExtensions
{
    /// <summary>
    /// Checks if the current host environment name is <see cref="Environments.Development"/>.
    /// </summary>
    /// <param name="hostEnvironment">An instance of <see cref="IHostEnvironment"/></param>
    /// <returns>
    /// True if the environment name matches to <see cref="Environments.Development"/>,
    /// otherwise false.
    /// </returns>
    public static bool MatchesDevelopment(this IHostEnvironment hostEnvironment)
        => hostEnvironment.MatchesEnvironment(Environments.Development);

    /// <summary>
    /// Compares the current host environment name against the specified value.
    /// </summary>
    /// <param name="hostEnvironment">An instance of <see cref="IHostEnvironment"/></param>
    /// <param name="environmentName">Environment name to validate against.</param>
    /// <returns>True if the current environment matches to the specified name, otherwise false.</returns>
    public static bool MatchesEnvironment(this IHostEnvironment hostEnvironment, string environmentName)
    {
        if (hostEnvironment is null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(environmentName))
        {
            return false;
        }

        var specifiedEnvironmentName = EnvironmentName.Parse(environmentName, asciiOnly: true, CultureInfo.InvariantCulture);
        return specifiedEnvironmentName.Match(hostEnvironment.MatchesEnvironment, None: false);
    }

    /// <summary>
    /// Compares the current host environment name against the specified value.
    /// </summary>
    /// <param name="hostEnvironment">An instance of <see cref="IHostEnvironment"/></param>
    /// <param name="environmentName">Environment name to validate against.</param>
    /// <returns>True if the current environment matches to the specified name, otherwise false.</returns>
    public static bool MatchesEnvironment(this IHostEnvironment hostEnvironment, EnvironmentName environmentName)
    {
        if (hostEnvironment is null)
        {
            return false;
        }

        var hostEnvironmentName = EnvironmentName.Parse(hostEnvironment.EnvironmentName, asciiOnly: true, CultureInfo.InvariantCulture);
        return hostEnvironmentName.Match(e => e.Matches(environmentName), None: false);
    }

    /// <summary>
    /// Checks if the current host environment name is <see cref="Environments.Production"/>.
    /// </summary>
    /// <param name="hostEnvironment">An instance of <see cref="IHostEnvironment"/></param>
    /// <returns>
    /// True if the environment name matches to <see cref="Environments.Production"/>,
    /// otherwise false.
    /// </returns>
    public static bool MatchesProduction(this IHostEnvironment hostEnvironment)
        => hostEnvironment.MatchesEnvironment(Environments.Production);

    /// <summary>
    /// Checks if the current host environment name is <see cref="Environments.Staging"/>.
    /// </summary>
    /// <param name="hostEnvironment">An instance of <see cref="IHostEnvironment"/></param>
    /// <returns>
    /// True if the environment name matches to <see cref="Environments.Staging"/>,
    /// otherwise false.
    /// </returns>
    public static bool MatchesStaging(this IHostEnvironment hostEnvironment)
        => hostEnvironment.MatchesEnvironment(Environments.Staging);
}
