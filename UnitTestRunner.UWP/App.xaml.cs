using System.Reflection;
using Xunit.Runners.UI;

namespace TIKSN.Framework.UnitTestRunner.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    internal sealed partial class App : RunnerApplication
    {
        protected override void OnInitializeRunner()
        {
            AddTestAssembly(GetType().GetTypeInfo().Assembly);
        }
    }
}
