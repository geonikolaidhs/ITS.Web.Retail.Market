FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj?') DO ( 
 IF EXIST "%%G" (
 RMDIR /S /Q "%%G" 
 mkdir "R:%%~npG"
 mklink /D "%%G" "R:%%~npG"
 )
)
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S bin?') DO ( 
 IF EXIST "%%G" (
 RMDIR /S /Q "%%G" 
 mkdir "R:%%~npG"
 mklink /D "%%G" "R:%%~npG"
 )
)
rem move "ITS\Retail\WebClient\ITS.Retail.WebClient\POS" "ITS\Retail\WebClient\ITS.Retail.WebClient\POS.old"
rem mkdir "R:\ITS\Retail\WebClient\ITS.Retail.WebClient\POS"
rem mklink /D "ITS\Retail\WebClient\ITS.Retail.WebClient\POS" "R:\ITS\Retail\WebClient\ITS.Retail.WebClient\POS"
rem xcopy /s /y "ITS\Retail\WebClient\ITS.Retail.WebClient\POS.old" "ITS\Retail\WebClient\ITS.Retail.WebClient\POS" 
rem RMDIR /S /Q "ITS\Retail\WebClient\ITS.Retail.WebClient\POS.old"
rem move "ITS\Retail\WebClient\ITS.Retail.WebClient\Tools" "ITS\Retail\WebClient\ITS.Retail.WebClient\Tools.old"
rem mkdir "R:\ITS\Retail\WebClient\ITS.Retail.WebClient\Tools"
rem mklink /D "ITS\Retail\WebClient\ITS.Retail.WebClient\Tools" "R:\ITS\Retail\WebClient\ITS.Retail.WebClient\Tools"
rem xcopy /s /y "ITS\Retail\WebClient\ITS.Retail.WebClient\Tools.old" "ITS\Retail\WebClient\ITS.Retail.WebClient\Tools" 
rem RMDIR /S /Q "ITS\Retail\WebClient\ITS.Retail.WebClient\Tools.old"