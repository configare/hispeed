using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls
{
    /// <summary>
    /// Provides basic implementation of the IMessageListener 
    /// </summary>
    public class RadMessageListener : IMessageListener
    {
        #region IMessageListener Members

        public virtual InstalledHook DesiredHook
        {
            get
            {
                return InstalledHook.GetMessage;
            }
        }

        public virtual MessagePreviewResult PreviewMessage(ref Message msg)
        {
            return MessagePreviewResult.NotProcessed;
        }

        public virtual void PreviewWndProc(Message msg)
        {
        }

        public virtual void PreviewSystemMessage(SystemMessage message, Message msg)
        {
        }

        #endregion
    }
}
