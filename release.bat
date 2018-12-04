@echo off

C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe RetroSpy.sln /p:Configuration=Release

if errorlevel 0 goto buildOK
echo Aborting release. Error during build.
goto end

:buildOK
del RetroSpy-release.zip

cd bin\x64\Release
"C:\Program Files\7-Zip\7z.exe" a ..\..\..\RetroSpy-release.zip RetroSpy.exe
cd ..\..\..

;cd slimdx\x64
;"C:\Program Files\7-Zip\7z.exe" a ..\..\RetroSpy-release.zip SlimDX.dll
;cd ..\..

;"C:\Program Files\7-Zip\7z.exe" a RetroSpy-release.zip skins
;"C:\Program Files\7-Zip\7z.exe" a RetroSpy-release.zip firmware
;"C:\Program Files\7-Zip\7z.exe" a RetroSpy-release.zip experimental
;"C:\Program Files\7-Zip\7z.exe" a RetroSpy-release.zip keybindings.xml

:end
pause
