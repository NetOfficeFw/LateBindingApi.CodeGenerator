using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NetOffice;

/*
    Contains the following Type Libraries:
%List%
*/

[assembly: AssemblyTitle("%Title%")]
[assembly: AssemblyDescription("%Description%")]
[assembly: AssemblyConfiguration("%Configuration%")]
[assembly: AssemblyCompany("%Company%")]
[assembly: AssemblyProduct("%Product%")]
[assembly: AssemblyCopyright("%Copyright%")]
[assembly: AssemblyTrademark("%Trademark%")]
[assembly: AssemblyCulture("%Culture%")]
[assembly: AssemblyVersion("%Version%")]
[assembly: AssemblyFileVersion("%FileVersion%")]
[assembly: PrimaryInteropAssembly(1, 0)]
[assembly: ImportedFromTypeLib("%ImportedTypeLibName%")]
[assembly: Guid("%ImportedTypeLibGuid%")]
[assembly: LateBindingAttribute("1.0")]

/*
Alias Table
 
%AliasInclude%*/