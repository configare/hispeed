using Telerik.WinControls;
using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;
using System.ComponentModel;
using System.Drawing;
using Telerik.WinControls.UI.Properties;
using System.Reflection;
using Telerik.WinControls.Primitives;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a split button in <see cref="CommandBarStripElement"/>.
    /// </summary>
    public class CommandBarSplitButton : CommandBarDropDownButton
    {
        #region Static members

        static CommandBarSplitButton()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new DropDownButtonStateManagerFatory(), typeof(CommandBarSplitButton));
        }
         
        #endregion

        #region Events

        /// <summary>
        /// Occurs when the default item is changed.
        /// </summary>
        public event EventHandler DefaultItemChanged;

        /// <summary>
        /// Occurs before the default item is changed.
        /// </summary>
        public event CancelEventHandler DefaultItemChanging;

        #endregion

        #region Fields

        protected RadItem defaultItem;
        protected BorderPrimitive buttonBorder;
        protected RadCommandBarVisualElement buttonSeparator;

        #endregion

        #region Properties


        public BorderPrimitive ActionPartBorder
        {
            get
            {
                return buttonBorder;
            }
        }

        public LightVisualElement Separator
        {
            get
            {
                return buttonSeparator;
            }
        }

        /// <summary>
        /// Gets or sets the default item of the split button.
        /// </summary>
        public RadItem DefaultItem
        {
            get
            {
                return defaultItem;
            }
            set
            {
                if (this.defaultItem != value && !this.OnDefaultItemChanging())
                {
                    this.defaultItem = value;
                    this.SetDefaultItemCore();
                    this.OnDefaultItemChanged();
                }
            }
        }
            
        #endregion

        #region Event handlers

        /// <summary>
        /// Raises the <see cref="E:CommandBarSplitButton.DefaultItemChanging"/> event.
        /// </summary>
        /// <returns>true if the event should be canceled, false otherwise.</returns>
        protected virtual bool OnDefaultItemChanging()
        {
            if (this.DefaultItemChanging != null)
            {
                CancelEventArgs args = new CancelEventArgs();
                this.DefaultItemChanging(this, args);
                return args.Cancel;
            }

            return false;
        }

        /// <summary>
        /// Raises the <see cref="E:CommandBarSplitButton.DefaultItemChanged"/> event.
        /// </summary>
        protected virtual void OnDefaultItemChanged()
        {
            if (this.DefaultItemChanged != null)
            {
                this.DefaultItemChanged(this,EventArgs.Empty);
            }
        } 

        #endregion

        #region Overrides

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            RectangleF arrowRect = this.arrowButton.ControlBoundingRectangle;
            
            if (e.X>=arrowRect.X && e.X<=arrowRect.Right &&
                e.Y>=arrowRect.Y && e.Y<=arrowRect.Bottom)
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                this.SetValue(RadButtonItem.IsPressedProperty, true);
                if (defaultItem != null)
                {
                    this.defaultItem.CallDoClick(EventArgs.Empty);
                }
            }
        }
         
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(e);
            this.SetValue(RadButtonItem.IsPressedProperty, false);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.SetValue(RadButtonItem.IsPressedProperty, false);
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.arrowButton.Class = "CommandBarSplitButtonArrow";

            this.buttonSeparator = new RadCommandBarVisualElement();
            this.buttonSeparator.NotifyParentOnMouseInput = true;
            this.buttonSeparator.Class = "CommandBarSplitButtonSeparator";
            this.buttonSeparator.StretchVertically = true;
            this.buttonSeparator.StretchHorizontally = false;
            this.buttonSeparator.SetDefaultValueOverride(RadElement.MinSizeProperty, new System.Drawing.Size(2, 2));
            this.buttonSeparator.DrawText = false;
            this.buttonSeparator.SetDefaultValueOverride(LightVisualElement.DrawFillProperty, false);
            this.buttonSeparator.SetDefaultValueOverride(LightVisualElement.DrawBorderProperty, true);
            this.buttonSeparator.SetDefaultValueOverride(LightVisualElement.BorderBoxStyleProperty, BorderBoxStyle.FourBorders);
            this.buttonSeparator.SetDefaultValueOverride(LightVisualElement.BorderBottomWidthProperty, 0f);
            this.buttonSeparator.SetDefaultValueOverride(LightVisualElement.BorderTopWidthProperty, 0f);
            this.buttonSeparator.SetDefaultValueOverride(LightVisualElement.BorderLeftWidthProperty, 1f);
            this.buttonSeparator.SetDefaultValueOverride(LightVisualElement.BorderRightWidthProperty, 1f);
            this.Children.Add(this.buttonSeparator);

            this.buttonBorder = new BorderPrimitive();
            this.Children.Add(buttonBorder);
            this.arrowButton.Click += new EventHandler(arrowButton_Click);
            
        }

        private void arrowButton_Click(object sender, EventArgs e)
        {
            this.ShowDropdown();
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            if (!this.VisibleInStrip)
            {
                return SizeF.Empty;
            }

            SizeF totalSize = base.MeasureOverride(availableSize);
            
            this.buttonSeparator.Measure(availableSize);
            totalSize.Width += this.buttonSeparator.DesiredSize.Width;

            Padding p = this.buttonBorder.GetBorderThickness();
            totalSize.Width += p.Left + p.Right;
            totalSize.Height += p.Top + p.Bottom;
            return totalSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            RectangleF clientArea = GetClientRectangle(finalSize);
            Padding borderThickness = this.buttonBorder.GetBorderThickness();

            if (this.RightToLeft)
            {
                float leftPos = clientArea.Left;
                this.arrowButton.Arrange(new RectangleF(clientArea.Location,new SizeF(arrowButton.DesiredSize.Width,clientArea.Height)));

                leftPos += arrowButton.DesiredSize.Width;
                this.buttonSeparator.Arrange(new RectangleF(leftPos, clientArea.Top, this.buttonSeparator.DesiredSize.Width, clientArea.Height));

                leftPos += this.buttonSeparator.DesiredSize.Width;
                this.buttonBorder.Arrange(new RectangleF(leftPos, clientArea.Top, clientArea.Right - leftPos, clientArea.Height));

                leftPos += borderThickness.Left;
                this.layoutManagerPart.Arrange(new RectangleF(leftPos, clientArea.Top + borderThickness.Top, 
                    clientArea.Right - leftPos - borderThickness.Right, clientArea.Height - borderThickness.Top - borderThickness.Bottom));
            }
            else
            { 
                float rightPos = clientArea.Right;

                rightPos -= arrowButton.DesiredSize.Width;
                this.arrowButton.Arrange(new RectangleF(new PointF(rightPos,clientArea.Top), new SizeF(arrowButton.DesiredSize.Width, clientArea.Height)));
                
                rightPos -= this.buttonSeparator.DesiredSize.Width;
                this.buttonSeparator.Arrange(new RectangleF(rightPos, clientArea.Top, this.buttonSeparator.DesiredSize.Width, clientArea.Height));

                this.buttonBorder.Arrange(new RectangleF(clientArea.Location,new SizeF( rightPos - clientArea.Left, clientArea.Height)));

                this.layoutManagerPart.Arrange(new RectangleF(clientArea.Left + borderThickness.Left, clientArea.Top + borderThickness.Top,
                    rightPos - borderThickness.Right -borderThickness.Left - clientArea.Left, clientArea.Height - borderThickness.Top - borderThickness.Bottom));
            }

            return finalSize;
        }

        #endregion
         
        private void SetDefaultItemCore()
        {
            if (this.defaultItem != null)
            {
                this.Text = this.defaultItem.Text;
                Type t = this.defaultItem.GetType();
                PropertyInfo p = t.GetProperty("Image");

                if (p != null)
                {

                    this.Image = (Image)(p.GetValue(this.defaultItem, null));
                }
            }
        }
    }
}
