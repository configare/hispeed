using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Provides customization dialogs for the customization of a <c ref="RadCommandBar"/>.
    /// </summary>
    public class CommandBarCustomizeDialogProvider
    {
        /// <summary>
        /// Fires when the current dialog provider has changed.
        /// </summary>
        public static event EventHandler CurrentProviderChanged;

        /// <summary>
        /// Fires when a customize dialog is shown
        /// </summary>
        public static event EventHandler CustomizeDialogOpened;

        /// <summary>
        /// Fires before a customize dialog is shown
        /// </summary>
        public static event CancelEventHandler CustomizeDialogOpening;

        private static void OnCurrentProviderChanged()
        {
            EventHandler eh = CurrentProviderChanged;
            if (eh != null)
            {
                eh(currentProvider, EventArgs.Empty);
            }
        }

        protected static bool OnDialogOpening(object sender)
        {
            if (CustomizeDialogOpening != null)
            {
                CancelEventArgs args = new CancelEventArgs();
                CustomizeDialogOpening(sender, args);
                return args.Cancel;
            }

            return false;
        }

        protected static void OnDialogOpened(object sender)
        {
            if (CustomizeDialogOpened != null)
            { 
                CustomizeDialogOpened(sender, new EventArgs());
            } 
        }

        private static CommandBarCustomizeDialogProvider currentProvider =
        CommandBarCustomizeDialogProvider.CreateDefaultCustomizeDialogProvider();

        /// <summary>
        /// Creates an instance of a dialog form.
        /// </summary>
        /// <param name="infoHolder"> <c ref="CommandBarStripInfoHolder"/> object that contains information about strips.</param>
        /// <returns>A refference to the created form.</returns>
        public virtual Form ShowCustomizeDialog(object sender, CommandBarStripInfoHolder infoHolder)
        {
            CommandBarCustomizeDialog dialog = new CommandBarCustomizeDialog(infoHolder);
            RadElement senderElement = sender as RadElement;
            RadControl senderControl = sender as RadControl;
            
            if (senderControl == null && senderElement != null && senderElement.ElementTree != null)
            {
                senderControl = senderElement.ElementTree.Control as RadControl;
            }

            if (sender is CommandBarStripElement)
            {
                dialog.stripsListControl.SelectedValue = sender;
                dialog.radPageView.SelectedPage = dialog.toolstripItemsPage;
            }
            else if (sender is RadCommandBar)
            {
                dialog.radPageView.SelectedPage = dialog.toolstripsPage;
            }

            if (senderControl != null)
            {
                dialog.ThemeName = senderControl.ThemeName;
                dialog.RightToLeft = senderControl.RightToLeft;
            }
            else if (senderElement != null)
            {
                dialog.RightToLeft = senderElement.RightToLeft ? RightToLeft.Yes : RightToLeft.No;
            }

            if (OnDialogOpening(dialog))
            {
                return null;
            }

            OnDialogOpened(dialog);
            dialog.ShowDialog();

            return dialog;
        }
        
        /// <summary>
        /// Gets or sets the current localization provider.
        /// </summary>
        [Browsable(false)]
        public static CommandBarCustomizeDialogProvider CurrentProvider
        {
            get
            {
                return CommandBarCustomizeDialogProvider.currentProvider;
            }
            set
            {
                if (value == null)
                {
                    CommandBarCustomizeDialogProvider.currentProvider =
                    CommandBarCustomizeDialogProvider.CreateDefaultCustomizeDialogProvider();
                }
                else
                {
                    CommandBarCustomizeDialogProvider.currentProvider = value;
                }
                CommandBarCustomizeDialogProvider.OnCurrentProviderChanged();
            }
        }
         
        /// <summary>
        /// Creates a default localization provider.
        /// </summary>
        /// <returns>A new instance of the default localization provider.</returns>
        private static CommandBarCustomizeDialogProvider CreateDefaultCustomizeDialogProvider()
        {
            return new CommandBarCustomizeDialogProvider();
        }
    }
}
