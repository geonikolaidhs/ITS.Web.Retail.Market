@echo off
echo Current Branch:
echo ==================
svn info --show-item relative-url
echo ==================
echo.
echo Branches list:
SET index=1
SET branch%index%=trunk
ECHO %index% - trunk
SET /A index=%index%+1

SETLOCAL ENABLEDELAYEDEXPANSION
for /f "delims=" %%i in ('svn ls "^/branches"') do (
	rem echo %%i	
	SET branch!index!=%%i
	ECHO !index! - %%i
	SET /A index=!index!+1
) 

SETLOCAL DISABLEDELAYEDEXPANSION

echo.

SET /P selection="Select branch to switch: "

SET branch%selection% >nul 2>&1

IF ERRORLEVEL 1 (
   ECHO invalid number selected   
   EXIT /B 1
)

CALL :RESOLVE %%branch%selection%%%

ECHO Switching to: %branch_name%
svn switch %switch_url% --accept p
choice /C:YN /M "Revert to remove any switch conflicts? Yes/No"

if %errorlevel%==2 GOTO :EOF
echo Reverting...
svn revert . --depth infinity


GOTO :EOF

:RESOLVE
SET branch_name=%1
if %branch_name%==trunk set switch_url="^/trunk"
if NOT %branch_name%==trunk set switch_url="^/branches/%branch_name%"
GOTO :EOF

rem echo Select a branch:

rem set branch_name=%1

rem if %branch_name%==trunk set switch_url="^/trunk"
rem if NOT %branch_name%==trunk set switch_url="^/branches/%branch_name%"

rem svn switch %switch_url%
rem if %revert%==1 svn revert . --depth=infinity