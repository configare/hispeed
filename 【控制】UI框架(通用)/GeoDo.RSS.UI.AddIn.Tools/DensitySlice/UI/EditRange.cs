using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    public partial class EditRange : Form
    {

        public EditRange(DensityRange dr)
        {
            InitializeComponent();

            this.TopMost = true;
            this.redText.DataBindings.Add("Text", this, "Red", true, DataSourceUpdateMode.OnPropertyChanged);
            this.redTrack.DataBindings.Add("Value", this, "Red", true, DataSourceUpdateMode.OnPropertyChanged);
            this.greenText.DataBindings.Add("Text", this, "Green", true, DataSourceUpdateMode.OnPropertyChanged);
            this.greenTrack.DataBindings.Add("Value", this, "Green", true, DataSourceUpdateMode.OnPropertyChanged);
            this.blueText.DataBindings.Add("Text", this, "Blue", true, DataSourceUpdateMode.OnPropertyChanged);
            this.blueTrack.DataBindings.Add("Value", this, "Blue", true, DataSourceUpdateMode.OnPropertyChanged);
            this.MinText.Text = dr.minValue.ToString();
            this.MaxText.Text = dr.maxValue.ToString();
            this.Red = dr.RGB_r;
            this.Green = dr.RGB_g;
            this.Blue = dr.RGB_b;
            UpdateColor();
        }

        public EditRange()
        {
            InitializeComponent();

            this.TopMost = true;
            this.redText.DataBindings.Add("Text", this, "Red", true, DataSourceUpdateMode.OnPropertyChanged);
            this.redTrack.DataBindings.Add("Value", this, "Red", true, DataSourceUpdateMode.OnPropertyChanged);
            this.greenText.DataBindings.Add("Text", this, "Green", true, DataSourceUpdateMode.OnPropertyChanged);
            this.greenTrack.DataBindings.Add("Value", this, "Green", true, DataSourceUpdateMode.OnPropertyChanged);
            this.blueText.DataBindings.Add("Text", this, "Blue", true, DataSourceUpdateMode.OnPropertyChanged);
            this.blueTrack.DataBindings.Add("Value", this, "Blue", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        public byte Red
        {
            get { return _red; }
            set
            {
                if (value > 255 || value < 0)
                    return;
                _red = value;
                Invalidate(new Rectangle(pictureBox1.Location.X, pictureBox1.Location.Y, pictureBox1.Width, pictureBox1.Height));

            }
        }

        public byte Green
        {
            get { return _green; }
            set
            {
                if (value > 255 || value < 0)
                    return;
                _green = value;
                Invalidate(new Rectangle(pictureBox1.Location.X, pictureBox1.Location.Y, pictureBox1.Width, pictureBox1.Height));

            }
        }

        public byte Blue
        {
            get { return _blue; }
            set
            {
                if (value > 255 || value < 0)
                    return;
                _blue = value;
                Invalidate(new Rectangle(pictureBox1.Location.X, pictureBox1.Location.Y, pictureBox1.Width, pictureBox1.Height));

            }
        }

        public float Edit_min
        {
            get
            {
                return edit_min;
            }
            set
            {
                edit_min = value;
            }
        }

        public float Edit_max
        {
            get
            {
                return edit_max;
            }
            set
            {
                edit_max = value;
            }
        }

        public DensityRange Edit_range
        {
            get
            {
                return edit_range;
            }
            set
            {
                edit_range = value;
            }
        }


        private void EditRange_Load(object sender, EventArgs e)
        {

        }

        private float edit_min;
        private float edit_max;
        private byte _red;
        private byte _green;
        private byte _blue;
        private DensityRange edit_range = new DensityRange();

        private void button1_Click(object sender, EventArgs e)//make sure
        {
            Edit_min = 0; //Int32.Parse(MinText.Text);
            Edit_max = 0; //Int32.Parse(MaxText.Text);
            if (float.TryParse(MinText.Text, out edit_min))
            {
                if (float.TryParse(MaxText.Text, out edit_max))
                {
                    DensityRange temp_range = new DensityRange(Edit_min, Edit_max, Red, Green, Blue);
                    Edit_range = temp_range;
                    g.Dispose();
                    brush_picbox.Dispose();

                }
                else
                {
                    MaxText.Focus();
                    MaxText.SelectAll();
                }
            }
            else
            {
                MinText.Focus();
                MinText.SelectAll();
            }
        }

        private void button2_Click(object sender, EventArgs e)//cancel
        {
            g.Dispose();
            brush_picbox.Dispose();
            this.Close();
            return;

        }

        private void redTrack_Scroll(object sender, EventArgs e)
        {
            Invalidate(new Rectangle(pictureBox1.Location.X, pictureBox1.Location.Y, pictureBox1.Width, pictureBox1.Height));
        }

        private System.Drawing.Graphics g;
        private SolidBrush brush_picbox;

        private void EditRange_Paint(object sender, PaintEventArgs e)
        {
            brush_picbox = new SolidBrush(Color.FromArgb(Red, Green, Blue));
            g = pictureBox1.CreateGraphics();
            g.FillRectangle(brush_picbox, 0, 0, pictureBox1.Width, pictureBox1.Height);
        }

        private void greenTrack_Scroll(object sender, EventArgs e)
        {
            Invalidate(new Rectangle(pictureBox1.Location.X, pictureBox1.Location.Y, pictureBox1.Width, pictureBox1.Height));
        }

        private void blueTrack_Scroll(object sender, EventArgs e)
        {
            Invalidate(new Rectangle(pictureBox1.Location.X, pictureBox1.Location.Y, pictureBox1.Width, pictureBox1.Height));
        }

        private void redText_ValueChanged(object sender, EventArgs e)
        {
            Invalidate(new Rectangle(pictureBox1.Location.X, pictureBox1.Location.Y, pictureBox1.Width, pictureBox1.Height));
        }

        private void greenText_ValueChanged(object sender, EventArgs e)
        {
            Invalidate(new Rectangle(pictureBox1.Location.X, pictureBox1.Location.Y, pictureBox1.Width, pictureBox1.Height));
        }

        private void blueText_ValueChanged(object sender, EventArgs e)
        {
            Invalidate(new Rectangle(pictureBox1.Location.X, pictureBox1.Location.Y, pictureBox1.Width, pictureBox1.Height));
        }

        public void UpdateColor()
        {
            this.pictureBox1.BackColor = Color.FromArgb(255, Red, Green, Blue);
        }
    }
}