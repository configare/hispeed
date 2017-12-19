using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls;

namespace Telerik.WinControls.Commands
{
    
    public interface ICommandPresentation : IImageListProvider
    {
        string Text { get; set; }
    }
}
