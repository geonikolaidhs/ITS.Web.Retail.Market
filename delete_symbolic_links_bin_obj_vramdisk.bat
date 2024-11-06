FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj?') DO ( 
 IF EXIST "%%G" (
 RMDIR /S /Q "%%G"
 )
)
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S bin?') DO ( 
 IF EXIST "%%G" (
 RMDIR /S /Q "%%G"
 )
)


rem mkdir "ITS\Retail\WebClient\ITS.Retail.WebClient\POS.old"
rem xcopy /s /y "ITS\Retail\WebClient\ITS.Retail.WebClient\POS" "ITS\Retail\WebClient\ITS.Retail.WebClient\POS.old" 
rem RMDIR /S /Q "ITS\Retail\WebClient\ITS.Retail.WebClient\POS"
rem REN "ITS\Retail\WebClient\ITS.Retail.WebClient\POS.old" "ITS\Retail\WebClient\ITS.Retail.WebClient\POS"

rem mkdir "ITS\Retail\WebClient\ITS.Retail.WebClient\Tools.old"
rem xcopy /s /y "ITS\Retail\WebClient\ITS.Retail.WebClient\Tools" "ITS\Retail\WebClient\ITS.Retail.WebClient\Tools.old" 
rem RMDIR /S /Q "ITS\Retail\WebClient\ITS.Retail.WebClient\Tools"
rem REN "ITS\Retail\WebClient\ITS.Retail.WebClient\Tools.old" "ITS\Retail\WebClient\ITS.Retail.WebClient\Tools"