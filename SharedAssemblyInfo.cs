using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyCompany("TIKSN Development")]
[assembly: AssemblyProduct("TIKSN Framework")]
[assembly: AssemblyCopyright("Copyright © 2016, TIKSN Lab")]

#if DEBUG

[assembly: AssemblyConfiguration("Debug")]
[assembly: AssemblyDescription("Flavor=Debug")] // a.k.a. “Comments”

#else

[assembly: AssemblyConfiguration("Retail")]

[assembly: AssemblyDescription("Flavor=Retail")] // a.k.a. “Comments”

#endif

[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("1.6.0.0")]
[assembly: AssemblyFileVersion("1.6.0.0")]
[assembly: AssemblyInformationalVersion("1.6.0.0")] // a.k.a. “Product version”

