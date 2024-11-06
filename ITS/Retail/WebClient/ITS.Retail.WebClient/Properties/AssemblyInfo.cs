using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("ITS.Retail.WebClient")]
[assembly: AssemblyDescription("Retail")]
#if _RETAIL_DUAL
[assembly: AssemblyConfiguration("DUAL")]
#elif _RETAIL_STORECONTROLLER
[assembly: AssemblyConfiguration("STORECONTROLLER")]
#else
[assembly: AssemblyConfiguration("HQ")]
#endif
[assembly: AssemblyProduct("ITS.Retail.WebClient")]
[assembly: AssemblyTrademark("ITS SA Expert Retail Solutions")]
[assembly: AssemblyCulture("")]
// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]
// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("15b6843e-24b7-476f-a844-f4b8e94698b9")]

