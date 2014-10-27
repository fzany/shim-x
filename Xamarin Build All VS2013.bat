@if "%VS120COMNTOOLS%"=="" goto error_no_VS120COMNTOOLSDIR
@call "%VS120COMNTOOLS%VsDevCmd.bat"

@cd "%HOMEDRIVE%%HOMEPATH%\Documents\Visual Studio 2012\Projects\aforge-x"
@msbuild "Xamarin Build All.sln" /t:Rebuild /p:Configuration=Release;Platform="Any CPU"

@echo .pri > exclude.txt
@echo .mdb >> exclude.txt

@if EXIST Publish (rd /s /q Publish)

@md "Publish\iOS\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt System.Drawing\_Touch\bin\iPhone\Release\Shim.Drawing.* "Publish\iOS\Any CPU"

@md "Publish\Android\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt System.Drawing\_Droid\bin\Release\Shim.Drawing.* "Publish\Android\Any CPU"

@md "Publish\Universal\Any CPU"
@xcopy /k /r /v /y /exclude:exclude.txt System.Drawing\_Universal\bin\Release\Shim.Drawing.* "Publish\Universal\Any CPU"

@del /f exclude.txt

@goto end

@REM -----------------------------------------------------------------------
:error_no_VS120COMNTOOLSDIR
@echo ERROR: Cannot determine the location of the VS Common Tools folder.
@goto end

:end