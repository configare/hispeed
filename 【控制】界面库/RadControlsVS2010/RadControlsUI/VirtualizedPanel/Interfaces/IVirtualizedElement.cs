using System;

namespace Telerik.WinControls.UI
{
    public interface IVirtualizedElement<T>
    {
        T Data { get; }

        void Attach(T data, object context);
        void Detach();
        void Synchronize();
        bool IsCompatible(T data, object context);
    }
}