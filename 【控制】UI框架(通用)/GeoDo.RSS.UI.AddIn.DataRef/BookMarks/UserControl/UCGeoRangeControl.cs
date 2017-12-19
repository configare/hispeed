using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.DataRef
{
    public partial class UCGeoRangeControl : UserControl
    {
        public delegate void ValueChangedHandler(object sender);
        public event ValueChangedHandler OnValueChanged = null;

        public UCGeoRangeControl()
        {
            InitializeComponent();
            txtMinLon.KeyDown += new KeyEventHandler(txtMinLon_KeyDown);
            txtMinLat.KeyDown += new KeyEventHandler(txtMinLon_KeyDown);
            txtMaxLon.KeyDown += new KeyEventHandler(txtMinLon_KeyDown);
            txtMaxLat.KeyDown += new KeyEventHandler(txtMinLon_KeyDown);
            txtMinLon.LostFocus += new EventHandler(txtMinLon_LostFocus);
            txtMinLat.LostFocus += new EventHandler(txtMinLon_LostFocus);
            txtMaxLon.LostFocus += new EventHandler(txtMinLon_LostFocus);
            txtMaxLat.LostFocus += new EventHandler(txtMinLon_LostFocus);
            this.LostFocus += new EventHandler(UCGeoRangeControl_LostFocus);
        }

        void UCGeoRangeControl_LostFocus(object sender, EventArgs e)
        {
            if (OnValueChanged != null)
                OnValueChanged(this);
        }

        void txtMinLon_LostFocus(object sender, EventArgs e)
        {
            if (OnValueChanged != null)
                OnValueChanged(this);
        }

        void txtMinLon_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (OnValueChanged != null)
                    OnValueChanged(this);
            }
        }

        public bool IsStrongOK()
        {
            if (txtMaxLat.Value > 90 || txtMaxLat.Value < -90)
                return false;
            if (txtMaxLon.Value > 180 || txtMaxLon.Value < -180)
                return false;
            if (txtMinLat.Value > 90 || txtMinLat.Value < -90)
                return false;
            if (txtMinLon.Value > 180 || txtMinLon.Value < -180)
                return false;
            if (txtMinLon.Value >= txtMaxLon.Value)
                return false;
            if (txtMinLat.Value >= txtMaxLat.Value)
                return false;
            return true;
        }

        public bool IsOK()
        {
            if (txtMaxLat.Value > 90 || txtMaxLat.Value < -90)
                return false;
            //if (txtMaxLon.Value > 180 || txtMaxLon.Value < -180)
            //    return false;
            //if (txtMinLat.Value > 90 || txtMinLat.Value < -90)
            //    return false;
            //if (txtMinLon.Value > 180 || txtMinLon.Value < -180)
            //    return false;
            //if (txtMinLon.Value >= txtMaxLon.Value)
            //    return false;
            if (txtMinLat.Value >= txtMaxLat.Value)
                return false;
            return true;
        }

        public bool IsOver180()
        {
            return txtMinLon.Value >= txtMaxLon.Value;
        }

        public void Adjust()
        {
            double x1 = MinX;
            double x2 = MaxX;
            double y1 = MinY;
            double y2 = MaxY;
            MinX = Math.Min(x1, x2);
            MaxX = Math.Max(x1, x2);
            MinY = Math.Min(y1, y2);
            MaxY = Math.Max(y1, y2);
        }

        public double MinX
        {
            get { return txtMinLon.Value; }
            set { txtMinLon.Value = value; }
        }

        public double MaxX
        {
            get { return txtMaxLon.Value; }
            set { txtMaxLon.Value = value; }
        }

        public double MinY
        {
            get { return txtMinLat.Value; }
            set { txtMinLat.Value = value; }
        }

        public double MaxY
        {
            get { return txtMaxLat.Value; }
            set { txtMaxLat.Value = value; }
        }

        private void doubleTextBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
