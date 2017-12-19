using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.Themes
{
    public class MetadataActionEventArgs : EventArgs
    {
        private UIProperties uiProperties;
        private VsbElementMetadata metadata;

        public MetadataActionEventArgs(UIProperties props, VsbElementMetadata metadata)
        {
            this.uiProperties = props;
            this.metadata = metadata;
        }

        public UIProperties UIProperties
        {
            get
            {
                return this.uiProperties;
            }
        }

        public VsbElementMetadata ElementMetadata
        {
            get
            {
                return this.metadata;
            }
        }
    }

    public class VsbMetadataAction
    {
        #region Fields

        public const string SeparatorText = "-";

        private object tag;
        private string actionDisplayName;
        private string actionDescription;
        private EventHandler<MetadataActionEventArgs> actionCallback;
        private EventHandler<MetadataActionEventArgs> uiStateInitializer;
        /// <summary>
        /// Defines the execution properties for the current <see cref="VsbMetadataAction"/>
        /// </summary>
        public ActionExecutionProperties ExecutionProperties;

        #endregion

        #region Ctor

        public VsbMetadataAction(string name, 
            EventHandler<MetadataActionEventArgs> actionCallback, 
            EventHandler<MetadataActionEventArgs> uiInit)
        {
            this.actionDisplayName = name;
            this.actionCallback = actionCallback;
            this.uiStateInitializer = uiInit;
        }

        public VsbMetadataAction(string name,
                                 string description,
                                 EventHandler<MetadataActionEventArgs> actionCallback,
                                 EventHandler<MetadataActionEventArgs> uiInit)
        {
            this.actionDisplayName = name;
            this.actionDescription = description;
            this.actionCallback = actionCallback;
            this.uiStateInitializer = uiInit;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets additional information associated with the action.
        /// </summary>
        public object Tag
        {
            get
            {
                return this.tag;
            }
            set
            {
                this.tag = value;
            }
        }

        /// <summary>
        /// Gets or sets the description for the current action. This
        /// description may be used for contextual help or tool tips.
        /// </summary>
        public string ActionDescription
        {
            get
            {
                return this.actionDescription;
            }
            set
            {
                this.actionDescription = value;
            }
        }

        /// <summary>
        /// A method that is called when the user executes the action.
        /// </summary>
        public EventHandler<MetadataActionEventArgs> ActionCallback
        {
            get
            {
                return this.actionCallback;
            }
        }

        /// <summary>
        /// A method that is called to initialize the ui state for the action.
        /// </summary>
        public EventHandler<MetadataActionEventArgs> UIInitializer
        {
            get
            {
                return this.uiStateInitializer;
            }
        }

        /// <summary>
        /// Gets a string representing the name which
        /// will describe the action.
        /// </summary>
        public string DisplayName
        {
            get
            {
                return this.actionDisplayName;
            }
        }
        #endregion
    }

    /// <summary>
    /// A struct containing information for the UI state
    /// of a <see cref="VsbMetadataAction"/>
    /// </summary>
    public class UIProperties
    {
        public static readonly UIProperties Empty = new UIProperties();

        /// <summary>
        /// Defines whether the UI item for the current action will
        /// be marked as checked.
        /// </summary>
        public bool IsChecked = false;

        /// <summary>
        /// Defines whether the UI item for the current action
        /// will be marked as enabled.
        /// </summary>
        public bool IsEnabled = true;
    }

    /// <summary>
    /// This enumerator defines the tasks that the
    /// VSB will perform after executing a <see cref="VsbMetadataAction"/>.
    /// </summary>
    [Flags]
    public enum ActionExecutionProperties
    {
        /// <summary>
        /// The current control metadata tree will be rebuilt after
        /// executing the action.
        /// </summary>
        RebuildMetadataTree = 1,
        /// <summary>
        /// The theme will be reapplied to the preview control.
        /// </summary>
        ReapplyThemeToPreviewControl = 2,
        /// <summary>
        /// The content of the Actions menu item will be rebuilt.
        /// </summary>
        RebuildActionsMenu = 4
    }
}
