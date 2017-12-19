using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Permissions;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace Telerik.WinControls
{
    [/*SecurityCritical(SecurityCriticalScope.Everything), */HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
    internal sealed class SafeLibraryHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        // Methods
        internal SafeLibraryHandle()
            : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            return TimeZoneUnsafeNativeMethods.FreeLibrary(base.handle);
        }
    }
}
