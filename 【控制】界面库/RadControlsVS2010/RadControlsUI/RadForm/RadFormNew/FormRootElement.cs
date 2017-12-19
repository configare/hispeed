using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Telerik.WinControls.UI;

namespace Telerik.WinControls
{
    /// <summary>
    /// This class represents the root element of a RadFormControlBase Element Tree.
    /// This class is needed because some extended region calculations are needed when the control
    /// is a Form and shape is applied to it.
    /// </summary>
    public class FormRootElement : RootRadElement
    {
        #region Fields

        private RadFormControlBase formControl;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates an instance of the FormRootElement class.
        /// </summary>
        /// <param name="control">The RadFormControlBase which is owner of this root element</param>
        public FormRootElement(RadFormControlBase control)
        {
            this.formControl = control;
        }

        #endregion

        #region Methods

        protected override void OnLoaded()
        {
            base.OnLoaded();

            this.SetDefaultValueOverride(RootRadElement.ApplyShapeToControlProperty, true);
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {

            if (e.Property == RadElement.BoundsProperty)
            {
                if (!this.UseNewLayoutSystem)
                {
                    if (this.AutoSize && this.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize)
                    {
                        //this.Size = this.ElementTree.Control.Size;
                    }
                    else
                    {
                        this.ElementTree.ComponentTreeHandler.ElementTree.LockControlLayout();
                        this.ElementTree.Control.Size = ((Rectangle)e.NewValue).Size;
                        this.ElementTree.ComponentTreeHandler.ElementTree.UnlockControlLayout();
                    }
                }
                if ((this.Shape != null) && (this.formControl != null) &&
                    this.ApplyShapeToControl)
                {
                    Rectangle oldBounds = (Rectangle)e.OldValue;
                    Rectangle newBounds = (Rectangle)e.NewValue;
                    //change region only of the Size has changed
                    if (oldBounds.Size != newBounds.Size)
                    {
                        CreateRegionFromShape(newBounds.Size);
                    }
                }
            }
            else if ((e.Property == ShapeProperty) && this.ApplyShapeToControl)
            {
                ElementShape shape = e.NewValue as ElementShape;
                if ((shape != null) && (this.ElementTree != null))
                {
                    CreateRegionFromShape(this.Size);
                }
            }

            else if (e.Property == ApplyShapeToControlProperty)
            {
                if ((bool)e.NewValue && this.Shape != null)
                {
                    CreateRegionFromShape(this.Size);
                }
                else
                {
                    this.ElementTree.Control.Region = null;
                }
            }
            else
            {
                base.OnPropertyChanged(e);

            }
        }

        private void CreateRegionFromShape(Size regionSize)
        {
            Rectangle boundsRect;
            if (this.formControl.WindowState != System.Windows.Forms.FormWindowState.Maximized
                || this.formControl.MaximumSize != Size.Empty)
            {
                boundsRect = new Rectangle(Point.Empty, regionSize);

                using (GraphicsPath path = this.Shape.CreatePath(boundsRect))
                {
                    this.formControl.Region = new Region(path);
                }
            }
            else
            {
                int result;

                if (!DWMAPI.IsCompositionEnabled)
                {
                    result = SystemInformation.FixedFrameBorderSize.Height;
                }
                else
                {
                    result = SystemInformation.FrameBorderSize.Height;
                }

                boundsRect = new Rectangle(new Point(result, result), new Size(regionSize.Width - (result * 2), regionSize.Height - (result * 2)));

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddRectangle(boundsRect);
                    this.formControl.Region = new Region(path);
                }
            }
        }


        #endregion

        #region Properties

        protected override Type ThemeEffectiveType
        {
            get
            {
                return typeof(RootRadElement);
            }
        }

        #endregion
    }
}
