using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI
{
   /// <summary>
   /// present RadGripElement
   /// </summary>
    public class RadGripElement : RadElement
    {
        //members
        private ImagePrimitive image;

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.MaxSize = new Size(4, 4);
            this.Margin = new Padding(7);
            this.Alignment = ContentAlignment.BottomRight;
            this.ZIndex = 1000;
        }

        /// <summary>
        /// creacte child elements
        /// </summary>
        protected override void CreateChildElements()
        {
            this.image = new ImagePrimitive();
            this.image.Class = "GripImage";
            this.Children.Add(this.image);
        }

        /// <summary>
        /// Grip image
        /// </summary>
        public ImagePrimitive Image
        {
            get
            {
                return image;
            }
        }        

        //logic for resize

        private Point downPoint = Point.Empty;

        /// <summary>
        /// OnMouseDown
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.downPoint = e.Location;
        }

        /// <summary>
        /// OnMouseUp
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Cursor.Current = Cursors.Default;
        }

        private const int mouseResizeOffset = 15;

        /// <summary>
        /// OnMouseMove
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            Form form = this.ElementTree.Control.FindForm();
            if (form == null)
            {
                return;
            }

            int wParam = 0;

            FormBorderStyle borderStyle = FormBorderStyle.None;
            RadForm radForm = form as RadForm;
            //fixed #300984 - gripUnable to resize RadForm
            if( radForm != null )
            {
                borderStyle = radForm.FormBorderStyle;
            }
            else
            {
                borderStyle = form.FormBorderStyle;
            }

            if (this.Parent != null &&
                form.WindowState!= FormWindowState.Maximized &&
                (borderStyle != FormBorderStyle.None || (form is ShapedForm && (form as ShapedForm).AllowResize) || form is RadRibbonForm) &&
                borderStyle != FormBorderStyle.Fixed3D &&
                borderStyle != FormBorderStyle.FixedDialog &&
                borderStyle != FormBorderStyle.FixedSingle &&
                borderStyle != FormBorderStyle.FixedToolWindow)
            {
                if (this.RightToLeft)
                {
                    if (e.X < mouseResizeOffset && e.Y > this.Parent.Size.Height - mouseResizeOffset)
                    {
                        Cursor.Current = Cursors.SizeNESW;
                        wParam = NativeMethods.HTBOTTOMLEFT;
                    }
                }
                else
                {
                    if (e.X > this.Parent.Size.Width - mouseResizeOffset && e.Y > this.Parent.Size.Height - mouseResizeOffset)
                    {
                        Cursor.Current = Cursors.SizeNWSE;
                        wParam = NativeMethods.HTBOTTOMRIGHT;
                    }
                }

                if (e.Button == MouseButtons.Left && downPoint != e.Location)
                {
                    NativeMethods.ReleaseCapture();
                    NativeMethods.SendMessage(new HandleRef(this, form.Handle), NativeMethods.WM_NCLBUTTONDOWN, wParam, IntPtr.Zero);
                }
            }
        }
        //end resize
    }
}
