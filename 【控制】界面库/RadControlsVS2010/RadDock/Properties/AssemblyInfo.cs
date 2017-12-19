using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;
using System.Resources;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

#if EVALUATION

[assembly: AssemblyTitle("Telerik.WinControls.RadDock Trial Version")]
[assembly: AssemblyDescription("Trial Version")]
#else

[assembly: AssemblyTitle("Telerik.WinControls.RadDock")]
[assembly: AssemblyDescription("")]
#endif


[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Telerik Corporation")]
[assembly: AssemblyProduct("Telerik.WinControls.RadDock")]
[assembly: AssemblyCopyright(Telerik.WinControls.VersionNumber.CopyrightText)]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("665f8280-0035-446d-8546-cfdb681ae252")]

[assembly: NeutralResourcesLanguage("en-US")]

[assembly: InternalsVisibleTo("RadDockTests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100a1d418f82d72dc0e2f407fa60bf0b04797d423db31b4e5a4a99c53908c7f9428eb9dc9a3d20533fe88893a5541de45059e3267320fb19c95bb256855a6a7019eae0538e8af2682d78fc33217dbc465cc495301cf792ab97482ebc9d32bd5be4b83de352160d05ed1be61b21ee3c602d7c507fdda4bd2d25d830660ba1300c9de")]
[assembly: InternalsVisibleTo("Telerik.WinControls.UI.Design, PublicKey=0024000004800000940000000602000000240000525341310004000001000100a1d418f82d72dc0e2f407fa60bf0b04797d423db31b4e5a4a99c53908c7f9428eb9dc9a3d20533fe88893a5541de45059e3267320fb19c95bb256855a6a7019eae0538e8af2682d78fc33217dbc465cc495301cf792ab97482ebc9d32bd5be4b83de352160d05ed1be61b21ee3c602d7c507fdda4bd2d25d830660ba1300c9de")]
[assembly: InternalsVisibleTo("VisualStyleBuilder.Design, PublicKey=0024000004800000940000000602000000240000525341310004000001000100a1d418f82d72dc0e2f407fa60bf0b04797d423db31b4e5a4a99c53908c7f9428eb9dc9a3d20533fe88893a5541de45059e3267320fb19c95bb256855a6a7019eae0538e8af2682d78fc33217dbc465cc495301cf792ab97482ebc9d32bd5be4b83de352160d05ed1be61b21ee3c602d7c507fdda4bd2d25d830660ba1300c9de")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion(Telerik.WinControls.VersionNumber.Number)]
[assembly: AssemblyFileVersion(Telerik.WinControls.VersionNumber.Number)]

#if NANT
[assembly: AssemblyKeyFile(@"..\RadControl.snk")]
#endif
