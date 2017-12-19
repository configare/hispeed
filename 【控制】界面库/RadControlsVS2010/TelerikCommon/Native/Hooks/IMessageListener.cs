using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls
{
    /// <summary>
    /// Defines a message listener that may be registered with a RadMessageListener.
    /// </summary>
    public interface IMessageListener
    {
        /// <summary>
        /// Gets the desired hook to be installed for this listener.
        /// </summary>
        InstalledHook DesiredHook
        {
            get;
        }

        /// <summary>
        /// Preview the specified message.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns>True to indicate that the message is processed and no further processing is required, false otherwise.</returns>
        MessagePreviewResult PreviewMessage(ref Message msg);
        /// <summary>
        /// Preview the message before it is dispatched to the target window. Cannot be modified.
        /// </summary>
        /// <param name="msg"></param>
        void PreviewWndProc(Message msg);
        /// <summary>
        /// Previews the specified system message. System messages are sent to internal windows like menus, dialogs, scrollbars, etc.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="msg"></param>
        void PreviewSystemMessage(SystemMessage message, Message msg);
    }

    /// <summary>
    /// Defines the possible results of a PreviewMessage method of an IMessageListener instance.
    /// </summary>
    [Flags]
    public enum MessagePreviewResult
    {
        /// <summary>
        /// The message is not processed.
        /// </summary>
        NotProcessed = 0,
        /// <summary>
        /// The message is processed.
        /// </summary>
        Processed = 1,
        /// <summary>
        /// No dispatch of the message is allowed.
        /// </summary>
        NoDispatch = Processed << 1,
        /// <summary>
        /// No further delegation to other listeners is desired.
        /// </summary>
        NoContinue = NoDispatch << 1,
        /// <summary>
        /// Processed and Dispatched flags
        /// </summary>
        ProcessedNoDispatch = Processed | NoDispatch,
        /// <summary>
        /// All flags are set
        /// </summary>
        All = Processed | NoDispatch | NoContinue,
    }

    /// <summary>
    /// Defines the possible hooks available for installation.
    /// </summary>
    [Flags]
    public enum InstalledHook
    {
        /// <summary>
        /// No hook is installed.
        /// </summary>
        None = 0,
        /// <summary>
        /// A WH_GETMESSAGE hook is installed.
        /// </summary>
        GetMessage = 1,
        /// <summary>
        /// A WH_CALLWNDPROC hook is installed.
        /// </summary>
        CallWndProc = GetMessage << 1,
        /// <summary>
        /// A WH_MSGFILTER hook is installed.
        /// </summary>
        SystemMessage = CallWndProc << 1,
        All = GetMessage | CallWndProc | SystemMessage,
    }

    /// <summary>
    /// Defines the possible targets of a system message.
    /// </summary>
    public enum SystemMessage
    {
        DialogBox = 0,
        MessageBox = 1,
        Menu = 2,
        ScrollBar = 5,
    }
}
