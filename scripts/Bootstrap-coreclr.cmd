SETLOCAL

for /f "delims=" %%a in ('"%USERPROFILE%\.dnx\runtimes\dnx-coreclr-win-x64.1.0.0-rc2-16388\bin\dnx" -p %~dp0..\src\OmniSharp.Bootstrap run %*') do @set LOCATION=%%a
if "%LOCATION%"=="" (set LOCATION="%~dp0..\src\OmniSharp")
if not exist %LOCATION%\project.lock.json (
  call "%USERPROFILE%\.dnx\runtimes\dnx-coreclr-win-x64.1.0.0-rc2-16388\bin\dnu.cmd" restore %LOCATION%
)
"%USERPROFILE%\.dnx\runtimes\dnx-coreclr-win-x64.1.0.0-rc2-16388\bin\dnx" -p %LOCATION% run %*
