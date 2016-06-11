using System.Reflection;
using TIKSN.Web.Tests;
using Xunit.Runners.UI;

namespace TIKSN.Framework.UnitTestRunner.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : RunnerApplication
    {
        protected override void OnInitializeRunner()
        {
            AddTestAssembly(GetType().GetTypeInfo().Assembly);

            AddTestAssembly(typeof(SitemapTests).GetTypeInfo().Assembly);
        }
    }
}
