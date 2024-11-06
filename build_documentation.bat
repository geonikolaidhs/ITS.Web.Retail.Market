cd /d %~dp0
del /Q /S "Documentation\DoxygenDocumentation\html\*.*"
doxygen Doxyfile