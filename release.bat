@echo off

C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe NintendoSpy.sln /p:Configuration=Release

if errorlevel 0 goto buildOK
echo Aborting release. Error during build.
goto end

:buildOK
del NintendoSpy-release.zip

cd bin\Release
"C:\Program Files\7-Zip\7z.exe" a ..\..\NintendoSpy-release.zip NintendoSpy.exe
cd ..\..

cd slimdx\x64
"C:\Program Files\7-Zip\7z.exe" a ..\..\NintendoSpy-release.zip SlimDX.dll
cd ..\..

"C:\Program Files\7-Zip\7z.exe" a NintendoSpy-release.zip skins
"C:\Program Files\7-Zip\7z.exe" a NintendoSpy-release.zip firmware
"C:\Program Files\7-Zip\7z.exe" a NintendoSpy-release.zip keybindings.xml

:end
pause
