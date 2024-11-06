''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''  Increment the version number of an MSI setup project
''  and update relevant GUIDs
''  
''  Hans-Jürgen Schmidt / 19.12.2007  
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
set a = wscript.arguments
if a.count = 0 then wscript.quit 1

'read and backup project file
Set fso = CreateObject("Scripting.FileSystemObject")
Set f = fso.OpenTextFile(a(0))
s = f.ReadAll
f.Close
fbak = a(0) & ".bak"
if fso.fileexists(fbak) then fso.deletefile fbak
fso.movefile a(0), fbak

'find setup version number
set re = new regexp
re.global = true
re.pattern = "(""ProductVersion"" = ""8:)(\d+(\.\d+)+)"""
set m = re.execute(s)
v = m(0).submatches(1)
v1 = split(v, ".")
'v1(ubound(v1)) = v1(ubound(v1)) + 1
'vnew = join(v1, ".")
'msgbox v & " --> " & vnew

'find retail.platform version number
Set fsoAssemblyVersion =  CreateObject("Scripting.FileSystemObject")
Set file = fsoAssemblyVersion.OpenTextFile(a(1))
fileContent = file.ReadAll
set re2 = new regexp
re2.global = true
re2.pattern = "\[assembly: AssemblyVersion\(\""(\d+)\.(\d+)\.(\d+)\.(\d+)""\)\]"
set matches = re2.execute(fileContent)
vn1 = matches(0).submatches(0)
vn2 = matches(0).submatches(1)
vn3 = matches(0).submatches(2)
vn4 = matches(0).submatches(3)

'convert Retail version to Setup Version as such: (Retail Major Number). (Retail Minor Number x 100 + Retail Build Number x 10 ).(Retail Build Number x 10 + Retail Revision Number)
'vnew = vn1 & "." & ((CInt(vn2) * 100) + (CInt(vn3) * 10)) & "." & ((CInt(vn3) * 10)+ CInt(vn4))   'so a version of 2.1.1.30 converts to 2.110.40. This is because setup projects only have 3 numbers

'convert Retail version to Setup Version as such: (Retail Major Number). (Retail Minor Number).(Retail Build Number & Retail Revision Number)
vnew = vn1 & "." & vn2 & "." & vn3 & vn4   'so a version of 2.1.1.30 converts to 2.1.130. This is because setup projects only have 3 numbers
'msgbox v & " -- > " & vnew

if v <> vnew then

	'replace Version
	s = re.replace(s, "$1" & vnew & """")

	'replace PackageCode
	re.pattern = "(""PackageCode"" = ""8:)(\{.+\})"""
	guid = CreateObject("Scriptlet.TypeLib").Guid
	guid = left(guid, len(guid) - 2)
	s = re.replace(s, "$1" & guid & """")

end if

'replace ProductCode
re.pattern = "(""ProductCode"" = ""8:)(\{.+\})"""
guid = CreateObject("Scriptlet.TypeLib").Guid
guid = left(guid, len(guid) - 2)
s = re.replace(s, "$1" & guid & """")
'msgbox guid & ""

'configuration specific alterations
configurationName = a(2)
if Not IsNull(configurationName) and configurationName <> "" then
	'constants
	'==========
	masterUpgradeCode = "{2EE9D9AD-A9B0-49BB-847B-F27E829D03CE}"
	storeControllerUpgradeCode = "{609CAAC4-6D62-4451-BA0E-8098C8508527}"
	dualUpgradeCode = "{7CFD475E-54B5-472E-B027-56C5D0758070}"
	virtualDirectory = "Retail" 'default directory
	productName = "ITS Retail Platform" 'default productName
	'==========
	
	upgradeCode = masterUpgradeCode
	if configurationName = "Release - Store Controller" then
		virtualDirectory = "RetailStoreController"
		productName = "ITS Retail Store Controller"
		upgradeCode = storeControllerUpgradeCode
	elseif configurationName = "Release - Retail Master" then
		virtualDirectory = "RetailHQ"
		productName = "ITS Retail HQ"
	elseif configurationName = "Release - Dual" then
		virtualDirectory = "RetailDual"
		productName = "ITS Retail Dual"
		upgradeCode = dualUpgradeCode
	end if
	'msgbox "ConfigName:" & configurationName
	'msgbox "Productname:" & productName
	'msgbox "virtualDirectory:" & virtualDirectory

	'replace virtual directory
	re.pattern = "(""VirtualDirectory"" = ""8:)(.+)"""
	s = re.replace(s, "$1" & virtualDirectory & """")

	'replace product name  and arpcomments
	re.pattern = "(""ProductName"" = ""8:)(.+)"""
	s = re.replace(s, "$1" & productName & """")
	re.pattern = "(""ARPCOMMENTS"" = ""8:)(.+)"""
	s = re.replace(s, "$1" & productName & """")

	'replace UpgradeCode
	re.pattern = "(""UpgradeCode"" = ""8:)(\{.+\})"""
	s = re.replace(s, "$1" & upgradeCode & """")
	'msgbox upgradeCode & ""
end if

'write project file
fnew = a(0)
set f = fso.CreateTextfile(fnew, true)
f.write(s)
f.close
