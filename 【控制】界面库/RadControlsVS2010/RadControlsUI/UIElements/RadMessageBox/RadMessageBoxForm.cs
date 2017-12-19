using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using System.Drawing;
using System.Reflection;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Paint;
using System.Runtime.InteropServices;

namespace Telerik.WinControls
{
    internal class RadMessageBoxForm : RadForm
    {
        private int buttonCount;
        private MessageBoxButtons buttonsConfiguration;
        private const int BORDER_OFFSET = 5;
        private const int BUTTON_MARGIN = 3;
        private Size buttonSize = new Size(75, 23);

        /// <summary>
        /// Determines whether to use compatible text rendering engine (GDI+) or not (GDI).
        /// </summary>
        public bool UseCompatibleTextRendering
        {
            set
            {
                this.UseCompatibleTextRendering = value;
                this.radLabel1.UseCompatibleTextRendering = value;
                this.radButton1.UseCompatibleTextRendering = value;
                this.radButton2.UseCompatibleTextRendering = value;
                this.radButton3.UseCompatibleTextRendering = value;
            }
        }

        public MessageBoxButtons ButtonsConfiguration
        {
            set
            {
                this.buttonsConfiguration = value;
                this.ConfigureButtons(value);
            }
        }
        
        public MessageBoxDefaultButton DefaultButton
        {
            set 
            {
                this.SetButtonFocus(value); 
            }
        }

        public override RightToLeft RightToLeft
        {
            set
            {
                base.RightToLeft = value;
                this.radLabel1.RightToLeft = value;
                this.radButton1.RightToLeft = value;
                this.radButton2.RightToLeft = value;
                this.radButton3.RightToLeft = value;
            }
        }

        private RadButton radButton1;
        private RadButton radButton2;
        private RadButton radButton3;
        private PictureBox pictureBox1;
        private RadLabel radLabel1;
        
        /// <summary>
        /// RadMessageBox Text
        /// </summary>
        public string MessageText
        {
            get { return this.radLabel1.Text; }
            set 
            {
                this.SetLabelTextAndSize(value);
            }
        }

        /// <summary>
        /// RadMessageBox caption text
        /// </summary>
        public override string Text
        {
            set
            {
                base.Text = value;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.IsLoaded)
            {
                this.LoadElementTree();
            }
            this.SetSizeAndLocations();
            base.OnLoad(e);
        }

        /// <summary>
        /// RadMessageBox Icon
        /// </summary>
        public Bitmap MessageIcon
        {
            set 
            {
                if (value != null)
                {
                    Bitmap icon = value;
                    if (icon.Size.Height > 32 || icon.Size.Width > 32)
                    {
                        icon = new Bitmap(icon, new Size(32, 32));
                    }
                    this.pictureBox1.Image = icon;
                    this.pictureBox1.Visible = true;
                }
                else
                {
                    this.pictureBox1.Image = null;
                    this.pictureBox1.Visible = false;
                }
            }
        }

        public RadMessageBoxForm()
        {
            this.InitializeComponents();

            this.buttonCount = 0;
            this.ThemeNameChanged += new ThemeNameChangedEventHandler(RadMessageBoxForm_ThemeNameChanged);
            this.KeyDown += new KeyEventHandler(RadMessageBoxForm_KeyDown);
            this.KeyPreview = true;
        }

        private void InitializeComponents()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.radButton1 = new Telerik.WinControls.UI.RadButton();
            this.radButton2 = new Telerik.WinControls.UI.RadButton();
            this.radButton3 = new Telerik.WinControls.UI.RadButton();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();

            this.pictureBox1.BackColor = Color.Transparent;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Size(48, 48);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;

            this.radButton1.Name = "radButton1";
            this.radButton1.Size = this.buttonSize;
            this.radButton1.TabIndex = 0;
            this.radButton1.Click += new EventHandler(radButton1_Click);
            this.radButton1.BackColor = Color.Transparent;
            
            this.radButton2.Name = "radButton2";
            this.radButton2.Size = this.buttonSize;
            this.radButton2.TabIndex = 1;
            this.radButton2.Click += new EventHandler(radButton2_Click);
            this.radButton2.BackColor = Color.Transparent;

            this.radButton3.Name = "radButton3";
            this.radButton3.Size = this.buttonSize;
            this.radButton3.TabIndex = 2;
            this.radButton3.Click += new EventHandler(radButton3_Click);
            this.radButton3.BackColor = Color.Transparent;

            this.radLabel1.AutoSize = true;
            this.radLabel1.LabelElement.StretchHorizontally = true;
            this.radLabel1.LabelElement.StretchVertically = true;
            this.radLabel1.BackColor = Color.Transparent;
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.MaximumSize = new Size(650, 0);
            this.radLabel1.TextWrap = true;
            this.radLabel1.TabIndex = 4;
            this.radLabel1.TextAlignment = System.Drawing.ContentAlignment.TopLeft;

            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.radLabel1);
            this.Controls.Add(this.radButton1);
            this.Controls.Add(this.radButton2);
            this.Controls.Add(this.radButton3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RadMessageBox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "MessageBox";

            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }


        private void RadMessageBoxForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                Clipboard.Clear();
                string clipboardText = this.BuildTextForClipboard();
                Clipboard.SetText(clipboardText);

            }
            else if (e.KeyCode == Keys.Escape && (
                this.buttonsConfiguration == MessageBoxButtons.OK ||
                this.buttonsConfiguration == MessageBoxButtons.OKCancel ||
                this.buttonsConfiguration == MessageBoxButtons.YesNoCancel))
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void RadMessageBoxForm_ThemeNameChanged(object source, ThemeNameChangedEventArgs args)
        {
            for (int i = 0; i < this.Controls.Count; i++)
            {
                if (this.Controls[i] is RadControl)
                {
                    (this.Controls[i] as RadControl).ThemeName = this.ThemeName;
                }
            }
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            this.DialogResult = (DialogResult)this.radButton1.Tag;
            this.Close();
        }

        private void radButton2_Click(object sender, EventArgs e)
        {
            this.DialogResult = (DialogResult)this.radButton2.Tag;
            this.Close();
        }

        private void radButton3_Click(object sender, EventArgs e)
        {
            this.DialogResult = (DialogResult)this.radButton3.Tag;
            this.Close();
        }

        private void SetButtonFocus(MessageBoxDefaultButton defaultButton)
        {
            this.ActiveControl = this.radButton1;

            switch (defaultButton)
            {
                case MessageBoxDefaultButton.Button2:
                    if (this.buttonCount > 1)
                    {
                        this.ActiveControl = this.radButton2;
                    }
                    break;
                case MessageBoxDefaultButton.Button3:
                    if (this.buttonCount > 2)
                    {
                        this.ActiveControl = this.radButton3;
                    }
                    break;
            }
        }

        private void ConfigureButtons(MessageBoxButtons messageBoxButtons)
        {
            this.SetButtonsText(messageBoxButtons);
            switch (messageBoxButtons)
            {
                case MessageBoxButtons.YesNoCancel:
                    this.radButton1.Tag = DialogResult.Yes;
                    this.radButton2.Tag = DialogResult.No;
                    this.radButton3.Tag = DialogResult.Cancel;
                    
                    this.radButton1.Visible = true;
                    this.radButton2.Visible = true;
                    this.radButton3.Visible = true;
                    this.FormElement.TitleBar.CloseButton.Enabled = true;
                    this.buttonCount = 3;
                    break;
                case MessageBoxButtons.AbortRetryIgnore:
                    this.radButton1.Tag = DialogResult.Abort;
                    this.radButton2.Tag = DialogResult.Retry;
                    this.radButton3.Tag = DialogResult.Ignore;

                    this.radButton1.Visible = true;
                    this.radButton2.Visible = true;
                    this.radButton3.Visible = true;
                    this.FormElement.TitleBar.CloseButton.Enabled = false;
                    this.buttonCount = 3;
                    break;
                case MessageBoxButtons.OK:
                    this.radButton1.Tag = DialogResult.OK;

                    this.radButton1.Visible = true;
                    this.radButton2.Visible = false;
                    this.radButton3.Visible = false;
                    this.FormElement.TitleBar.CloseButton.Enabled = true;
                    this.buttonCount = 1;
                    break;
                case MessageBoxButtons.RetryCancel:
                    this.radButton1.Tag = DialogResult.Retry;
                    this.radButton2.Tag = DialogResult.Cancel;

                    this.radButton1.Visible = true;
                    this.radButton2.Visible = true;
                    this.radButton3.Visible = false;
                    this.FormElement.TitleBar.CloseButton.Enabled = true;
                    this.buttonCount = 2;
                    break;
                case MessageBoxButtons.YesNo:
                    this.radButton1.Tag = DialogResult.Yes;
                    this.radButton2.Tag = DialogResult.No;

                    this.radButton1.Visible = true;
                    this.radButton2.Visible = true;
                    this.radButton3.Visible = false;
                    this.FormElement.TitleBar.CloseButton.Enabled = false;
                    this.buttonCount = 2;
                    break;
                case MessageBoxButtons.OKCancel:
                    this.radButton1.Tag = DialogResult.OK;
                    this.radButton2.Tag = DialogResult.Cancel;

                    this.radButton1.Visible = true;
                    this.radButton2.Visible = true;
                    this.radButton3.Visible = false;
                    this.FormElement.TitleBar.CloseButton.Enabled = true;
                    this.buttonCount = 2;
                    break;
            }
        }

        private void SetButtonsLocation()
        {
            Size clientSize = this.ClientSize;
            int locationY = clientSize.Height - this.buttonSize.Height - BORDER_OFFSET;

            Point pt1;
            Point pt2;
            Point pt3;

            if (this.RightToLeft == RightToLeft.Yes)
            {
                pt1 = new Point((clientSize.Width + (this.buttonCount - 2) * this.buttonSize.Width) / 2, locationY);
                pt2 = new Point(pt1.X - this.buttonSize.Width - BUTTON_MARGIN, locationY);
                pt3 = new Point(pt2.X - this.buttonSize.Width - BUTTON_MARGIN, locationY);
            }
            else
            {
                pt1 = new Point((clientSize.Width - this.buttonCount * this.buttonSize.Width) / 2, locationY);
                pt2 = new Point(pt1.X + this.buttonSize.Width + BUTTON_MARGIN, locationY);
                pt3 = new Point(pt2.X + this.buttonSize.Width + BUTTON_MARGIN, locationY);
            }

            this.radButton1.Location = pt1;
            this.radButton2.Location = pt2;
            this.radButton3.Location = pt3;
        }

        private void SetSizeAndLocations()
        {
            //set form min size according to button numbers
            switch (this.buttonCount)
            {
                case 1:
                    this.MinimumSize = (new Size(140, this.MinimumSize.Height));
                    break;
                case 2:
                    this.MinimumSize = (new Size(220, this.MinimumSize.Height));
                    break;
                case 3:
                    this.MinimumSize = (new Size(300, this.MinimumSize.Height));
                    break;
            }

            Padding ncPadding = this.FormBehavior.ClientMargin;
            //set size according the label size
            Size formSize = this.radLabel1.Size;
            //add non-client margins as well as default border padding
            formSize.Width += ncPadding.Horizontal + 2 * BORDER_OFFSET;
            formSize.Height += this.buttonSize.Height + ncPadding.Vertical + 4 * BORDER_OFFSET;

            //set size according the title text
            int titleWidth = this.CalculateWidthFromTitleSize();
            if (titleWidth > formSize.Width)
            {
                formSize.Width = titleWidth;
            }

            int measureWidth = this.MinimumSize.Width;

            //change size and locations if there is an icon
            if (this.pictureBox1.Image != null)
            {
                formSize.Width += this.pictureBox1.Width;
                if (this.radLabel1.Height < this.pictureBox1.Height)
                {
                    formSize.Height += (this.pictureBox1.Height - this.radLabel1.Height);
                }

                if (this.RightToLeft == RightToLeft.Yes)
                {
                     if (measureWidth < formSize.Width)
                    {
                        measureWidth = formSize.Width;
                    }

                    this.pictureBox1.Location =
                        new Point(measureWidth - BORDER_OFFSET - this.pictureBox1.Width, BORDER_OFFSET);
                    this.radLabel1.Location =
                        new Point(measureWidth - this.pictureBox1.Width - BORDER_OFFSET - this.radLabel1.Width, BORDER_OFFSET);                
                }
                else
                {
                    this.pictureBox1.Location = new Point(BORDER_OFFSET, BORDER_OFFSET);
                    this.radLabel1.Location = new Point(BORDER_OFFSET + this.pictureBox1.Width, BORDER_OFFSET);
                }
            }
            else
            {
                if (this.RightToLeft == RightToLeft.Yes)
                {
                     if (measureWidth < formSize.Width)
                    {
                        measureWidth = formSize.Width;
                    }
                    this.radLabel1.Location = new Point(measureWidth - BORDER_OFFSET - this.radLabel1.Width, BORDER_OFFSET);                    
                }
                else
                {
                    this.radLabel1.Location = new Point(BORDER_OFFSET, BORDER_OFFSET);
                }
                
            }

            this.Size = formSize;

            //set button location
            this.SetButtonsLocation();
        }

        /// <summary>
        /// Set label text and size according to text string measure
        /// </summary>
        /// <param name="text"></param>
        private void SetLabelTextAndSize(string text)
        {
            //this.radLabel1.Size = new Size(0, 0);
            //this.radLabel1.Text = String.Empty;

            //Graphics graphics1 = TelerikPaintHelper.CreateMeasurementGraphics();

            //SizeF sizeF =
            //    graphics1.MeasureString(text, this.radLabel1.Font,
            //    this.radLabel1.MaximumSize.Width - 20, StringFormat.GenericDefault);

            //this.radLabel1.Size = new Size((int)(sizeF.Width + 20), (int)(sizeF.Height + 6));

            //graphics1.ReleaseHdc(graphics1.GetHdc());
            //graphics1.Dispose();

            this.radLabel1.Text = text;
        }
      
        /// <summary>
        /// Calculate form size according to title text size
        /// </summary>
        /// <returns>width</returns>
        private int CalculateWidthFromTitleSize()
        {
            int width = 0;
            MeasurementGraphics graphics1 =  MeasurementGraphics.CreateMeasurementGraphics();
            SizeF sizeF = graphics1.Graphics.MeasureString(this.Text, this.Font);
            width = (int)(sizeF.Width + 170);//Engineer constant???
            graphics1.Dispose();
            return width;
        }


        private void SetButtonsText(MessageBoxButtons buttons)
        {
            switch (buttons)
            {
                case MessageBoxButtons.AbortRetryIgnore:
                    this.radButton1.Text =
                        RadMessageLocalizationProvider.CurrentProvider.GetLocalizedString(RadMessageStringID.AbortButton);
                    this.radButton2.Text =
                        RadMessageLocalizationProvider.CurrentProvider.GetLocalizedString(RadMessageStringID.RetryButton);
                    this.radButton3.Text =
                        RadMessageLocalizationProvider.CurrentProvider.GetLocalizedString(RadMessageStringID.IgnoreButton);
                    break;
                case MessageBoxButtons.OK:
                    this.radButton1.Text = 
                        RadMessageLocalizationProvider.CurrentProvider.GetLocalizedString(RadMessageStringID.OKButton);
                    this.radButton2.Text = String.Empty;
                    this.radButton3.Text = String.Empty;
                    break;
                case MessageBoxButtons.OKCancel:
                    this.radButton1.Text =
                        RadMessageLocalizationProvider.CurrentProvider.GetLocalizedString(RadMessageStringID.OKButton);
                    this.radButton2.Text = 
                        RadMessageLocalizationProvider.CurrentProvider.GetLocalizedString(RadMessageStringID.CancelButton);
                    this.radButton3.Text = String.Empty;
                    break;
                case MessageBoxButtons.RetryCancel:
                    this.radButton1.Text =
                        RadMessageLocalizationProvider.CurrentProvider.GetLocalizedString(RadMessageStringID.RetryButton);
                    this.radButton2.Text = 
                        RadMessageLocalizationProvider.CurrentProvider.GetLocalizedString(RadMessageStringID.CancelButton);
                    this.radButton3.Text = String.Empty;
                    break;
                case MessageBoxButtons.YesNo:
                    this.radButton1.Text = 
                        RadMessageLocalizationProvider.CurrentProvider.GetLocalizedString(RadMessageStringID.YesButton);
                    this.radButton2.Text = 
                        RadMessageLocalizationProvider.CurrentProvider.GetLocalizedString(RadMessageStringID.NoButton);
                    this.radButton3.Text = String.Empty;
                    break;
                case MessageBoxButtons.YesNoCancel:
                    this.radButton1.Text = 
                        RadMessageLocalizationProvider.CurrentProvider.GetLocalizedString(RadMessageStringID.YesButton);
                    this.radButton2.Text = 
                        RadMessageLocalizationProvider.CurrentProvider.GetLocalizedString(RadMessageStringID.NoButton);
                    this.radButton3.Text =
                        RadMessageLocalizationProvider.CurrentProvider.GetLocalizedString(RadMessageStringID.CancelButton);
                    break;
            }
        }

        private string BuildTextForClipboard()
        {
            StringBuilder strB = new StringBuilder();

            strB.AppendLine("---------------------------");
            strB.AppendLine(this.Text);
            strB.AppendLine("---------------------------");
            strB.AppendLine(this.radLabel1.Text);
            strB.AppendLine("---------------------------");

            strB.Append(this.radButton1.Text);
            if (this.radButton2.Visible)
            {
                strB.Append("  ");
                strB.Append(this.radButton2.Text);
            }
            if (this.radButton3.Visible)
            {
                strB.Append("  ");
                strB.Append(this.radButton3.Text);
            }

            strB.AppendLine();
            strB.AppendLine("---------------------------");

            return strB.ToString();
        }
    }
}
