
set _wd=%~dp0
set _bld=%_wd%bld
set _codegen=%_wd%CodeGen\bin\Debug\codegen.exe

%_codegen% --project "%_bld%\NetOffice 1.7.4.1.xml" --ref "%_bld%\ReferenceIndex2.xml" --keyfiles "%_bld%\KeyFiles" --output %_wd%out
