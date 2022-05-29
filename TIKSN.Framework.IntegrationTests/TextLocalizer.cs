using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using TIKSN.Localization;

namespace TIKSN.IntegrationTests
{
    public class TextLocalizer : CompositeAssemblyStringLocalizer
    {
        public TextLocalizer(
            IResourceNamesCache resourceNamesCache,
            ILogger<CompositeAssemblyStringLocalizer> logger) : base(resourceNamesCache, logger)
        {
        }
    }
}
