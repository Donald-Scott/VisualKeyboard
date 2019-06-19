REM Create a 'GeneratedReports' folder if it does not exist
if not exist "%~dp0GeneratedReports" mkdir "%~dp0GeneratedReports"
 
REM Remove any previous test execution files to prevent issues overwriting
IF EXIST "%~dp0VisualKeyboard.trx" del "%~dp0dp0VisualKeyboard.trx%"
 
REM Remove any previously created test output directories
CD %~dp0
FOR /D /R %%X IN (%USERNAME%*) DO RD /S /Q "%%X"
 
REM Run the tests against the targeted output
call :RunOpenCoverUnitTestMetrics
 
REM Generate the report output based on the test results
if %errorlevel% equ 0 ( 
 call :RunReportGeneratorOutput 
)
 
REM Launch the report
if %errorlevel% equ 0 ( 
 call :RunLaunchReport 
)
exit /b %errorlevel%
 
:RunOpenCoverUnitTestMetrics
"%~dp0\packages\OpenCover.4.7.922\tools\OpenCover.Console.exe" ^
-register:user ^
-target:"%VSAPPIDDIR%CommonExtensions\Microsoft\TestWindow\vstest.console.exe" ^
-targetargs:"\"%~dp0\VisualKeyboard.Tests\bin\Debug\VisualKeyboard.Tests.dll\" /Logger:trx;logfilename=VisualKeyboard.trx /ResultsDirectory:\"%~dp0TestResults\"" ^
-filter:"+[VisualKeyboard*]* -[VisualKeyboard.Tests]*" ^
-skipautoprops ^
-output:"%~dp0\GeneratedReports\VisualKeyboard.xml"
exit /b %errorlevel%
 
:RunReportGeneratorOutput
"%~dp0\packages\ReportGenerator.4.2.1\tools\net47\ReportGenerator.exe" ^
-reports:"%~dp0\GeneratedReports\VisualKeyboard.xml" ^
-targetdir:"%~dp0\GeneratedReports\ReportGenerator Output"
exit /b %errorlevel%
 
:RunLaunchReport
start "report" "%~dp0\GeneratedReports\ReportGenerator Output\index.htm"
exit /b %errorlevel%