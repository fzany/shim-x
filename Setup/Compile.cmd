@echo off

echo.
echo Portable AForge-X Framework all projects Release builder
echo ==========================================================
echo. 
echo This Windows batch file will use Visual Studio 2013 to
echo compile the Release versions of the Portable X framework.
echo. 

timeout /T 5

@if "%VS120COMNTOOLS%"=="" goto error_no_VS120COMNTOOLSDIR
@call "%VS120COMNTOOLS%VsDevCmd.bat"

@cd ..
@msbuild "Xamarin Build All.sln" /t:Rebuild /p:Configuration=Release;Platform="Any CPU"
@msbuild "Xamarin Build All.sln" /t:Rebuild /p:Configuration="Release (Full)";Platform="Any CPU"

@echo .mdb >> exclude.txt

@set PUBLISH=.\Setup\Publish\
@set COMMERCIAL=.\Setup\Publish Commercial\
@set DROIDDIR=%PUBLISH%lib\MonoAndroid1\
@set TOUCHDIR=%PUBLISH%lib\MonoTouch1\
@set UNIFIEDDIR=%PUBLISH%lib\Xamarin.iOS10\
@set UNIVERSALDIR=%PUBLISH%lib\portable-win81+wpa81\
@set CDROIDDIR=%COMMERCIAL%lib\MonoAndroid1\
@set CTOUCHDIR=%COMMERCIAL%lib\MonoTouch1\
@set CUNIFIEDDIR=%COMMERCIAL%lib\Xamarin.iOS10\
@set CUNIVERSALDIR=%COMMERCIAL%lib\portable-win81+wpa81\

@if EXIST "%PUBLISH%" (rd /s /q "%PUBLISH%")

@md "%TOUCHDIR%"
@xcopy /k /r /v /y /exclude:exclude.txt System.Drawing\_Touch\bin\Release\Shim.Drawing.* "%TOUCHDIR%"

@md "%DROIDDIR%"
@xcopy /k /r /v /y /exclude:exclude.txt System.Drawing\_Droid\bin\Release\Shim.Drawing.* "%DROIDDIR%"

@md "%UNIFIEDDIR%"
@xcopy /k /r /v /y /exclude:exclude.txt System.Drawing\_Unified\bin\Release\Shim.Drawing.* "%UNIFIEDDIR%"

@md "%UNIVERSALDIR%"
@xcopy /k /r /v /y /exclude:exclude.txt System.Drawing\_Universal\bin\Release\Shim.Drawing.* "%UNIVERSALDIR%"

@if EXIST "%COMMERCIAL%" (rd /s /q "%COMMERCIAL%")

@md "%CTOUCHDIR%"
@xcopy /k /r /v /y /exclude:exclude.txt System.Drawing\_Touch\bin\Commercial\Shim.Drawing.* "%CTOUCHDIR%"

@md "%CDROIDDIR%"
@xcopy /k /r /v /y /exclude:exclude.txt System.Drawing\_Droid\bin\Commercial\Shim.Drawing.* "%CDROIDDIR%"

@md "%CUNIFIEDDIR%"
@xcopy /k /r /v /y /exclude:exclude.txt System.Drawing\_Unified\bin\Commercial\Shim.Drawing.* "%CUNIFIEDDIR%"

@md "%CUNIVERSALDIR%"
@xcopy /k /r /v /y /exclude:exclude.txt System.Drawing\_Universal\bin\Commercial\Shim.Drawing.* "%CUNIVERSALDIR%"

@del /f exclude.txt

@goto end

@REM -----------------------------------------------------------------------
:error_no_VS120COMNTOOLSDIR
@echo ERROR: Cannot determine the location of the VS Common Tools folder.
@goto end

:end