requires the free Microsoft TypeLib Api ActiveX-Dll(TLBINF32.DLL) to compile. 
this binary is very common, but it's not installed on your system maybe.
in this case DONT PANIC!
checkout: LateBindingApi.CodeGenerator.ComponentAnalyzer\TLBINF32.DLL
copy to any directory you want and register(regsvr32.exe)
now refresh TLI reference in ComponentAnalyzer project or re-open