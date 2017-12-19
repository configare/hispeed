using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Telerik.WinControls
{
    /// <exclude/>
	class ColorBox: Label
	{
        private bool mouseEnter;
		public ColorBox()
		{
			this.BackColor = Color.White;
			this.AutoSize = false;
			this.Size = new Size(41, 23);
			this.BorderStyle = BorderStyle.Fixed3D;
            this.MouseEnter += new EventHandler(ColorBox_MouseEnter);
            this.MouseLeave += new EventHandler(ColorBox_MouseLeave);
            this.Click += new EventHandler(ColorBox_Click);
            this.MouseMove += new MouseEventHandler(ColorBox_MouseMove);
        }

        void ColorBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (Cursor.Current != Cursors.Hand)
                Cursor.Current = Cursors.Hand;
        }

     
        void ColorBox_MouseLeave(object sender, EventArgs e)
        {
            mouseEnter = false;
            Cursor.Current = Cursors.Default;
            this.Invalidate();
        }

        void ColorBox_MouseEnter(object sender, EventArgs e)
        {
            mouseEnter = true;
            Cursor.Current = Cursors.Hand;
            this.Invalidate();
        }

		/// <summary>
		/// Fires when the selected color changes
		/// </summary>
		public event ColorChangedEventHandler ColorChanged;
        public event ColorDialogCreatedEventHandler ColorDialogCreated;
		void ColorBox_Click(object sender, EventArgs e)
		{
            Cursor.Current = Cursors.Hand;
            IRadColorDialog dialogForm = RadColorEditor.CreateColorDialogInstance();

            if (this.ColorDialogCreated != null)
            {
                ColorDialogEventArgs args = new ColorDialogEventArgs(dialogForm);

                this.ColorDialogCreated(this, args);
            }
			
			UserControl colorSelector = RadColorEditor.CreateColorSelectorInstance() as UserControl;
			((IColorSelector)dialogForm.RadColorSelector).SelectedColor = this.BackColor;
			((IColorSelector)dialogForm.RadColorSelector).OldColor = this.BackColor;

			((IColorSelector)colorSelector).OkButtonClicked += delegate(object sender1, ColorChangedEventArgs args) 
			{ 
				((Form)dialogForm).DialogResult = DialogResult.OK; 
				((Form)dialogForm).Close(); 
			};
			((IColorSelector)colorSelector).CancelButtonClicked += delegate(object sender1, ColorChangedEventArgs args) 
			{ 
				((Form)dialogForm).DialogResult = DialogResult.Cancel; 
				((Form)dialogForm).Close(); 
			};
			colorSelector.Dock = DockStyle.Fill;
			((Form)dialogForm).Controls.Add(colorSelector);
			if (((Form)dialogForm).ShowDialog() == DialogResult.OK)
			{
				this.BackColor = ((IColorSelector)dialogForm.RadColorSelector).SelectedColor;
				if (ColorChanged != null)
					ColorChanged(this, new ColorChangedEventArgs(this.BackColor));
			}

            this.Invalidate();
		}
        protected override void OnPaint(PaintEventArgs e)
        {
            Cursor.Current = Cursors.Hand;
        
            if( mouseEnter )
            using (Pen pen = new Pen(this.ForeColor))
            {
                e.Graphics.DrawRectangle(pen, new Rectangle(ClientRectangle.X, ClientRectangle.Y,
                    ClientRectangle.Width - 1, ClientRectangle.Height - 1));
            }
            base.OnPaint(e);
        }
        
	}

    public class ColorDialogEventArgs : EventArgs
    {
        private IRadColorDialog dialog;

        public ColorDialogEventArgs(IRadColorDialog dialog)
        {
            this.dialog = dialog;
        }

        public IRadColorDialog Dialog
        {
            get
            {
                return dialog;
            }
        }
    }

    public delegate void ColorDialogCreatedEventHandler(object sender, ColorDialogEventArgs args);


}
