using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace TIKSN.Localization.Tests
{
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
}
