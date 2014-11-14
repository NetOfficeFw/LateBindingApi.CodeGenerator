# Late Binding API Code Generator

Creates .NET proxy components from COM+ Type Libraries. 
The components will be created as C# or VB.NET source code in a generated Visual Studio solution. 
The classes in the generated solution are accessing the COM Server with late binding reflection technique.

This project is a helper for NetOffice library that provides version independent access to Microsoft Office.


## Required Software

Requires the free Microsoft TypeLib API ActiveX library (TLBINF32.DLL) to compile.

Please, see the `LateBindingApi.CodeGenerator.ComponentAnalyzer\TLBINF32.DLL` file.
Copy it to any directory and register it using `regsvr32.exe`.
You can now refresh TLI reference in ComponentAnalyzer project or re-open the project.


## License

Late Binding API Code Generator source code is license under [Microsoft Public License (Ms-PL)](LICENSE.txt)

This repository is based on [LateBindingApi.CodeGenerator](https://latebindingapi.codeplex.com/) source code.