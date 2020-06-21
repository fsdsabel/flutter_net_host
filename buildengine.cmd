@echo off
SETLOCAL

rem follows 
rem https://github.com/flutter/flutter/wiki/Setting-up-the-Engine-development-environment 
rem and
rem https://github.com/flutter/flutter/wiki/Compiling-the-engine#compiling-for-windows
rem needs depot_tools

SET PATH=%~dp0tools\depot_tools;%PATH%;C:\Program Files (x86)\Microsoft Visual Studio\Installer\

pushd

cd %~dp0lib\engine

SET DEPOT_TOOLS_WIN_TOOLCHAIN=0
FOR /F "tokens=*" %%F IN ('vswhere.exe -latest -format value -all -property installationPath') do (SET GYP_MSVS_OVERRIDE_PATH=%%F)

SET WINDOWSSDKDIR=C:\Program Files (x86)\Windows Kits\10


cmd /c gclient sync

cd %~dp0lib\engine\src

cmd /c python .\flutter\tools\gn --unoptimized

echo "Building..."

ninja -C .\out\host_debug_unopt

popd

ENDLOCAL