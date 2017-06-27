# Late Binding API Code Generator

![version: 0.9.0-alpha](https://img.shields.io/badge/version-0.9.0--alpha-68217a.svg)
[![AppVeyor](https://img.shields.io/appveyor/ci/jozefizso/latebindingapi-codegenerator.svg)](https://ci.appveyor.com/project/jozefizso/latebindingapi-codegenerator)

Creates .NET proxy components from COM+ Type Libraries. 
The components will be created as C# or VB.NET source code in a generated Visual Studio solution. 
The classes in the generated solution are accessing the COM Server with late binding reflection technique.

This project is a helper for NetOffice library that provides version independent access to Microsoft Office.


## Generating NetOffice Source Code

[NetOffice][netoffice] source code is generated using LateBinding Code Generator from a project file.
NetOffice project files (named like `NetOffice 1.7.3.xml`) are XML documents with information about
all type libraries in Microsoft Office applications.

Use can use command line tool `codegen.exe` to regenerate NetOffice code:

```
codegen.exe --project "NetOffice 1.7.3.xml" --ref "RerefenceIndex2.xml" --keyfiles "KeyFiles\4.5" --output "build"
```

You will need these files:

* **NetOffice 1.7.3.xml** - NetOffice project file
* **RerefenceIndex2.xml** - index file with links to MSDN documentation pages
* **KeyFiles\4.5** - a KeyFiles folder which contains all `.snk` files which are used to sign NetOffice assemblies


## Required Software

To analyze COM+ type libraries, you need the free Microsoft TypeLib API ActiveX library (TLBINF32.DLL).
If the library is not registered on your system, use these commands in elevated command prompt:

```
> nuget restore
> cd packages\TypeLibInformation.1.1.0\tools
> regsvr32.exe /s /i TlbInf32.dll
```


## Development

Please, read the [contribution guideline](CONTRIBUTING.md) for information about development process.

The `master` branch contains stable release codebase. Use the `develop` branch to implement new features.


## License

Late Binding API Code Generator source code is license under [Microsoft Public License (Ms-PL)](LICENSE.txt)

This repository is based on [LateBindingApi.CodeGenerator](https://latebindingapi.codeplex.com/) source code.

[netoffice]: https://github.com/NetOfficeFw/NetOffice
