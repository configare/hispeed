using System;
using System.Collections.Generic;
using System.Drawing;


using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Collections;
using Telerik.WinControls.Layouts;
using System.Runtime.InteropServices;
using Telerik.WinControls.Design;
using System.IO;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.UI
{
    public class RadMDIControlsItem : RadItem
    {
        private RadImageButtonElement closeButton;
        private RadImageButtonElement minimizeButton;
        private RadImageButtonElement maximizeButton;
        private StackLayoutPanel systemButtons;

		/// <summary>
		/// Gets the Minimize button
		/// </summary>
        public RadButtonElement MinimizeButton
        {
            get
            {
                return minimizeButton;
            }
        }

		/// <summary>
		/// Gets the Maximize button
		/// </summary>
        public RadButtonElement MaximizeButton
        {
            get
            {
                return maximizeButton;
            }
        }

		/// <summary>
		/// Gets the Close button
		/// </summary>
        public RadButtonElement CloseButton
        {
            get
            {
                return closeButton;
            }
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;

            this.systemButtons = new StackLayoutPanel();
            this.systemButtons.Class = "RibbonMDIButtonsStackLayout";
            this.systemButtons.Alignment = ContentAlignment.MiddleRight;
            base.Children.Add(this.systemButtons);

            this.minimizeButton = new RadImageButtonElement();
            this.minimizeButton.Class = "RibbonMDIMinimizeButton";
            this.minimizeButton.Click += new EventHandler(this.OnMinimize);
            this.minimizeButton.Image = Properties.Resources.MDIminimize;
            
            this.systemButtons.Children.Add(minimizeButton);

            this.maximizeButton = new RadImageButtonElement();
            this.maximizeButton.Class = "RibbonMDIMaximizeButton";
            this.maximizeButton.Click += new EventHandler(this.OnMaximizeRestore);
            this.maximizeButton.Image = Properties.Resources.MDImaximize;
            this.systemButtons.Children.Add(maximizeButton);

            this.closeButton = new RadImageButtonElement();
            this.closeButton.Class = "RibbonMDICloseButton";
            this.closeButton.Click += new EventHandler(this.OnClose);
            this.closeButton.Image = Properties.Resources.MDIclose;
            this.systemButtons.Children.Add(closeButton);

            this.Visibility = ElementVisibility.Collapsed;
        }

        private Form hostForm = null;//point to host form in application

        #region Events and helper function

        private void PerfomMdiCommand(FormWindowState newState)
        {            
            if (hostForm != null && hostForm.IsMdiContainer)
            {
                Form activeMdiChild = hostForm.ActiveMdiChild;
                if (activeMdiChild != null)
                {
                    activeMdiChild.WindowState = newState;
                    activeMdiChild.ControlBox = (newState != FormWindowState.Maximized);
                    
                }
            }

        }
        private void OnMaximizeRestore(object sender, EventArgs args)
        {
            PerfomMdiCommand(FormWindowState.Normal);
            hostForm_Layout(sender, null);
        }

        private void OnMinimize(object sender, EventArgs args)
        {
            PerfomMdiCommand(FormWindowState.Minimized);
            hostForm_Layout(sender, null);
        }

        private void OnClose(object sender, EventArgs args)
        {
            this.Visibility = ElementVisibility.Collapsed;

            if (hostForm != null && hostForm.IsMdiContainer)
            {
                Form activeMdiChild = hostForm.ActiveMdiChild;

                if (activeMdiChild != null)
                {
                    activeMdiChild.Close();

                    if (hostForm.ActiveMdiChild == null)
                    {
                        this.Visibility = ElementVisibility.Collapsed;
                    }
                }
            }
        }
        /// <summary>
        /// Find main form and save it in member variable
        /// </summary>
        public void LayoutPropertyChanged()
        {   
            if (hostForm == null && 
                this.ElementTree != null &&
                this.ElementTree != null)
            {
                hostForm = this.ElementTree.Control.FindForm();
                if (hostForm != null)//found host form
                {
                    hostForm.Layout += new LayoutEventHandler(hostForm_Layout);                             
                }                
            }
        }        
        /// <summary>
        /// Main method for internal logic
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hostForm_Layout(object sender, LayoutEventArgs e)
        {
            if (hostForm != null && hostForm.IsMdiContainer)
            {
                bool IsMaximizedForm = false;
                foreach (Form form in hostForm.MdiChildren)
                {

                    if (form is ShapedForm)
                    {
                        foreach (Control mdiFormControls in form.Controls)
                        {
                            if (mdiFormControls is RadTitleBar)
                            {
                                mdiFormControls.Visible = form.WindowState != FormWindowState.Maximized;
                            }
                        }
                    }

                    if (form.WindowState == FormWindowState.Maximized)
                    {
                        IsMaximizedForm = true;
                        break;
                    }
                }
                if (IsMaximizedForm)
                {
                    this.Visibility = ElementVisibility.Visible;

                    this.InvalidateMeasure();
                    this.InvalidateArrange();
                    this.UpdateLayout();
                }
                else
                {
                    this.Visibility = ElementVisibility.Collapsed;
                    this.InvalidateMeasure();
                    this.InvalidateArrange();
                    this.UpdateLayout();
                }

                

            }
        }
        #endregion        
    }
}
