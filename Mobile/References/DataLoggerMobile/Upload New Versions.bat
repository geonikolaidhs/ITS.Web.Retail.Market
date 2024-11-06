ECHO ON
ECHO Recreating cab files.
ECHO

SET VERSION=1.4.0.0
SET PROJECT_PATH=..\Setup
SET STORE_PATH=\\thor\Software Products\Releases\DataLogger\Version %VERSION%

"C:\Program Files\Microsoft Visual Studio 8\smartdevices\sdk\sdktools\cabwiz.exe" "%PROJECT_PATH%\Release\DataLogger.inf" /dest "%PROJECT_PATH%\Release\" /err CabWiz.log

cls
ECHO
ECHO Transfering cab files to server.
ECHO

md "%STORE_PATH%"
copy "%PROJECT_PATH%\Release\DataLogger.cab" "%STORE_PATH%\DataLogger.cab"

pause