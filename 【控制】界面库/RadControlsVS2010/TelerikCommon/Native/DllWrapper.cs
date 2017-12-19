using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Telerik.WinControls
{
    /// <summary>
    /// Wrapper of native dlls. This class automatically disposes the loaded dll.
    /// </summary>
    public class DllWrapper : IDisposable
    {
        private IntPtr nativeDll = IntPtr.Zero;

        /// <summary>
        /// The default wrapper has no dll and IsDllLoaded returns false.
        /// </summary>
        public DllWrapper()
        {
        }

        /// <summary>
        /// Tries to load the given dll. If ti fails - IsDllLoaded will return false.
        /// </summary>
        /// <param name="dllName"></param>
        public DllWrapper(string dllName)
        {
            this.nativeDll = NativeMethods.LoadLibrary(dllName);
        }

        /// <summary>
        /// Gets a boolean value indicating whether a native dll is loaded in this wrapper.
        /// </summary>
        public bool IsDllLoaded
        {
            get { return this.nativeDll != IntPtr.Zero; }
        }

        /// <summary>
        /// Loads a native dll by given name (could be name only or with full path).
        /// If only a name is given the Operatin System's search order will be used.
        /// </summary>
        /// <param name="dllName"></param>
        /// <returns></returns>
        public bool LoadDll(string dllName)
        {
            FreeDll();
            lock (this)
            {
                this.nativeDll = NativeMethods.LoadLibrary(dllName);
            }
            return this.IsDllLoaded;
        }

        /// <summary>
        /// Releases the unmanaged dll. This method is called both by Dispose and by the destructor so it is not
        /// obligatory to call it explicitly.
        /// </summary>
        /// <returns>true if the wrapped handle was truly released</returns>
        public bool FreeDll()
        {
            bool res = false;
            lock (this)
            {
                if (this.nativeDll != IntPtr.Zero)
                {
                    res = NativeMethods.FreeLibrary(this.nativeDll);
                    this.nativeDll = IntPtr.Zero;
                }
            }
            return res;
        }

        /// <summary>
        /// Gets a delegate that wraps a function in a native dll.
        /// This method works only if there is a successfully loaded dll.
        /// </summary>
        /// <param name="functionName">Name of the function for which a delegate will be returned.</param>
        /// <param name="delegateType">The type of the delegate that should be returned.</param>
        /// <returns>Managed delegate of the given type or null on error.</returns>
        public object GetFunctionAsDelegate(string functionName, Type delegateType)
        {
            object res = null;

            lock (this)
            {
                if (this.nativeDll != IntPtr.Zero && delegateType != null)
                {
                    IntPtr procAddress = NativeMethods.GetProcAddress(this.nativeDll, functionName);
                    if (procAddress != IntPtr.Zero)
                    {
                        res = Marshal.GetDelegateForFunctionPointer(procAddress, delegateType);
                    }
                }
            }

            return res;
        }

        #region IDisposable Members
        public virtual void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        protected virtual void Dispose(bool disposing)
        {
            // When disposing is false the managed resources could be already finalized.
            // Free managed resources only when disposing is true!
            // Native resources can be disposed anytime...
            FreeDll();
        }

        ~DllWrapper()
        {
            this.Dispose(false);
        }
    }
}
