using System.Reflection;
using System.Runtime.InteropServices;

// Shared information about assemblies in the SystemWrapper solution.
[assembly: AssemblyCompany("NetOffice Framework")]
[assembly: AssemblyProduct("LateBinding Code Generator")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Disable COM visibility
[assembly: ComVisible(false)]

// NOTE: Assembly name is set in respective project files.
[assembly: AssemblyVersion("0.9.2.0")]
[assembly: AssemblyFileVersion("0.9.2.0")]
[assembly: AssemblyInformationalVersion("0.9.2.0-dev")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#elif RELEASE
[assembly: AssemblyConfiguration("Release")]
#endif
