using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Defines the available buttons for a DockWindow, residing in a DocumentTabStrip instance.
    /// </summary>
    [Flags]
    public enum DocumentStripButtons
    {
        /// <summary>
        /// No buttons are displayed.
        /// </summary>
        None = 0,
        /// <summary>
        /// Close button is displayed.
        /// </summary>
        Close = 1,
        /// <summary>
        /// The drop-down button which displayes all opened windows within the strip.
        /// </summary>
        ActiveWindowList = Close << 1,
        /// <summary>
        /// The system menu, which is available for each active window.
        /// </summary>
        SystemMenu = ActiveWindowList << 1,
        /// <summary>
        /// All flags are set.
        /// </summary>
        All = Close | ActiveWindowList | SystemMenu
    }

    /// <summary>
    /// Defines the possible order of items in the ActiveDocumentList menu, displayed for each <see cref="DocumentTabStrip">DocumentTabStrip</see>.
    /// </summary>
    public enum ActiveDocumentListSortOrder
    {
        /// <summary>
        /// No srot order is applied. The items in the menu are in the order they ppear in the owned documents.
        /// </summary>
        None,
        /// <summary>
        /// The items are sorted by the Text value of each document.
        /// </summary>
        ByText,
        /// <summary>
        /// The items are sorted by their z-order, supported by the <see cref="DocumentManager">DocumentManager</see>.
        /// </summary>
        ZOrdered,
    }

    /// <summary>
    /// Defines the possible actions to be taken when a <see cref="RadDock.CloseWindow">CloseWindow</see> request is made.
    /// </summary>
    public enum DockWindowCloseAction
    {
        /// <summary>
        /// The associated DockWindow is unregistered and removed from its current parent.
        /// </summary>
        Close,
        /// <summary>
        /// The associated DockWindow is unregistered, removed from its current parent and explicitly disposed.
        /// </summary>
        CloseAndDispose,
        /// <summary>
        /// The associated DockWindow is removed from its current parent but kept as registered with its owning RadDock.
        /// A hidden window may be later on displayed again at its previous state.
        /// </summary>
        Hide
    }

    /// <summary>
    /// Defines the visible buttons for a DockWindow, residing in a ToolTabStrip instance.
    /// </summary>
    [Flags]
    public enum ToolStripCaptionButtons
    {
        /// <summary>
        /// No buttons are displayed.
        /// </summary>
        None = 0,
        /// <summary>
        /// Close button is displayed.
        /// </summary>
        Close = 1,
        /// <summary>
        /// Auto-hide (pin) button is displayed.
        /// </summary>
        AutoHide = Close << 1,
        /// <summary>
        /// The built-in system menu is displayed.
        /// </summary>
        SystemMenu = AutoHide << 1,
        /// <summary>
        /// All bits are set.
        /// </summary>
        All = Close | AutoHide | SystemMenu
    }

    /// <summary>
    /// Defines the possible modes for animating an auto-hidden window.
    /// </summary>
    public enum AutoHideAnimateMode
    {
        /// <summary>
        /// No Animation is applied.
        /// </summary>
        None,
        /// <summary>
        /// The window is animated when shown.
        /// </summary>
        AnimateShow,
        /// <summary>
        /// The window is animated when hidden.
        /// </summary>
        AnimateHide,
        /// <summary>
        /// The window is animated when shown and hidden.
        /// </summary>
        Both,
    }

    /// <summary>
    /// Defines the possible insertion order when adding new documents.
    /// </summary>
    public enum DockWindowInsertOrder
    {
        /// <summary>
        /// Default order is chosen
        /// </summary>
        Default,
        /// <summary>
        /// The document is put in front of the opened documents. The default mode.
        /// </summary>
        InFront,
        /// <summary>
        /// The document is put to the end of the opened documents.
        /// </summary>
        ToBack,
    }

    /// <summary>
    /// Defines which document should be activated upon closing active document.
    /// </summary>
    public enum DocumentCloseActivation
    {
        /// <summary>
        /// The document manager chooses the default action. Typically this will activate the first tab in the strip.
        /// </summary>
        Default,
        /// <summary>
        /// Activates the last active document in the z-order. Typically this is the document, activated before the closed active one.
        /// </summary>
        FirstInZOrder,
    }
}
