using Android.App;
using Android.OS;
using System.Reflection;
using TIKSN.Web.Tests;
using Xunit.Runners.UI;
using Xunit.Sdk;

namespace TIKSN.UnitTestRunner.Android
{
    [Activity(Label = "UnitTestRunner.Android", MainLauncher = true, Icon = "@drawable/icon")]
    //[Activity(Label = "xUnit Android Runner", MainLauncher = true, Theme = "@android:style/Theme.Material.Light")]
    public class MainActivity : RunnerActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            // tests can be inside the main assembly
            AddTestAssembly(Assembly.GetExecutingAssembly());

            AddExecutionAssembly(typeof(ExtensibilityPointFactory).Assembly);
            // or in any reference assemblies

            AddTestAssembly(typeof(SitemapTests).Assembly);
            // or in any assembly that you load (since JIT is available)

            // you cannot add more assemblies once calling base
            base.OnCreate(bundle);
        }
    }
}