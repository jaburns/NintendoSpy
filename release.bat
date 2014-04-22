@echo off

C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe NintendoSpy.sln /p:Configuration=Release

if errorlevel 0 goto buildOK
echo Aborting release. Error during build.
goto end

:buildOK
del release\NintendoSpy.zip

cd bin\Release
"C:\Program Files\7-Zip\7z.exe" a ..\..\release\NintendoSpy.zip NintendoSpy.exe
cd ..\..

cd slimdx\x64
"C:\Program Files\7-Zip\7z.exe" a ..\..\release\NintendoSpy.zip SlimDX.dll
cd ..\..

"C:\Program Files\7-Zip\7z.exe" a release\NintendoSpy.zip skins

:end
pause
