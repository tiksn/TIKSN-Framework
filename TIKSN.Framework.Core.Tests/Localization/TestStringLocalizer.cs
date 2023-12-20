using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using TIKSN.Localization;

namespace TIKSN.Tests.Localization;

public class TestStringLocalizer : CompositeAssemblyStringLocalizer
{
    public TestStringLocalizer(
        IResourceNamesCache resourceNamesCache,
        ILogger<TestStringLocalizer> logger) : base(
            resourceNamesCache,
            logger)
    {
    }
}
