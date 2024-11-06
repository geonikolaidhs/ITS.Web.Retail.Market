cd /d %~dp0
xcopy "Documentation\DoxygenDocumentation\html" "\\loki\wwwroot\ITS.Retail.Platform.Documentation\" /y /s
xcopy "documentation-js" "\\loki\wwwroot\ITS.Retail.Platform.Documentation\javascript\" /y /s
xcopy "ITS\Retail\WebClient\ITS.Retail.WebClient\Scripts\react\doc" "\\loki\wwwroot\ITS.Retail.Platform.Documentation\react\" /y /s