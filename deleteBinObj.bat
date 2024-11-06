cd /d %~dp0
for /d /r . %%d in (bin,obj) do @if exist "%%d" rd /s/q "%%d"
del *.bak /s /a
