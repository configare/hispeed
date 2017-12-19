using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.Data
{
    //public class LoadItemEventArgs : EventArgs
    //{
    //    public LoadItemEventArgs(IDataItem dataItem)
    //    {
                
    //    }
    //}

    public delegate void LoadItemHandler(object sender, EventArgs e);
    public interface IDataItemSource
    {
        IDataItem NewItem();

        void Initialize();
        void BindingComplete();
        //void Reload(LoadItemHandler loadItemHandler);
        
        BindingContext BindingContext { get; }
        
        event EventHandler BindingContextChanged;        
    }
}
