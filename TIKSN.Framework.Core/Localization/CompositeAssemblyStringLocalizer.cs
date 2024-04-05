using System.Reflection;
using System.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace TIKSN.Localization;

public abstract class CompositeAssemblyStringLocalizer : CompositeStringLocalizer
{
    private readonly Lazy<IEnumerable<IStringLocalizer>> localizers;
    private readonly ILogger<CompositeAssemblyStringLocalizer> logger;
    private readonly IResourceNamesCache resourceNamesCache;

    protected CompositeAssemblyStringLocalizer(IResourceNamesCache resourceNamesCache,
        ILogger<CompositeAssemblyStringLocalizer> logger)
    {
        this.resourceNamesCache = resourceNamesCache;
        this.localizers = new Lazy<IEnumerable<IStringLocalizer>>(this.CreateLocalizers, isThreadSafe: false);
        this.logger = logger;
    }

    public override IEnumerable<IStringLocalizer> Localizers => this.localizers.Value;

    protected virtual IEnumerable<Assembly> GetAssemblies()
    {
        yield return typeof(CompositeAssemblyStringLocalizer).GetTypeInfo().Assembly;

        yield return typeof(LanguageLocalizationParameters).GetTypeInfo().Assembly;

        yield return typeof(RegionLocalizationParameters).GetTypeInfo().Assembly;
    }

    private List<IStringLocalizer> CreateLocalizers()
    {
        var result = new List<IStringLocalizer>();
        foreach (var (assembly, manifestResourceName) in this.GetAssemblies()
            .Where(assembly => assembly is not null)
            .SelectMany(assembly => assembly.GetManifestResourceNames()
                .Where(manifestResourceName => manifestResourceName.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
                .Select(manifestResourceName => (assembly, manifestResourceName))))
        {
            var resourceName = manifestResourceName[..^10];
            var resourceManager = new ResourceManager(resourceName, assembly);
            result.Add(new ResourceManagerStringLocalizer(resourceManager, assembly, resourceName,
                                                    this.resourceNamesCache, this.logger));
        }

        return result;
    }
}
