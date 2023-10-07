using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace TIKSN.Localization
{
    public abstract class CompositeAssemblyStringLocalizer : CompositeStringLocalizer
    {
        private readonly ILogger<CompositeAssemblyStringLocalizer> _logger;
        private readonly Lazy<IEnumerable<IStringLocalizer>> localizers;
        private readonly IResourceNamesCache resourceNamesCache;

        protected CompositeAssemblyStringLocalizer(IResourceNamesCache resourceNamesCache,
            ILogger<CompositeAssemblyStringLocalizer> logger)
        {
            this.resourceNamesCache = resourceNamesCache;
            this.localizers = new Lazy<IEnumerable<IStringLocalizer>>(this.CreateLocalizers, false);
            this._logger = logger;
        }

        public override IEnumerable<IStringLocalizer> Localizers => this.localizers.Value;

        protected virtual IEnumerable<Assembly> GetAssemblies()
        {
            yield return typeof(CompositeAssemblyStringLocalizer).GetTypeInfo().Assembly;

            yield return typeof(LanguageLocalizationParameters).GetTypeInfo().Assembly;

            yield return typeof(RegionLocalizationParameters).GetTypeInfo().Assembly;
        }

        private IEnumerable<IStringLocalizer> CreateLocalizers()
        {
            var result = new List<IStringLocalizer>();

            var assemblies = this.GetAssemblies().ToArray();

            foreach (var assembly in assemblies)
            {
                foreach (var manifestResourceName in assembly.GetManifestResourceNames())
                {
                    if (manifestResourceName.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
                    {
                        var resourceName = manifestResourceName.Substring(0, manifestResourceName.Length - 10);
                        var resourceManager = new ResourceManager(resourceName, assembly);
                        result.Add(new ResourceManagerStringLocalizer(resourceManager, assembly, resourceName,
                            this.resourceNamesCache, this._logger));
                    }
                }
            }

            return result;
        }
    }
}
