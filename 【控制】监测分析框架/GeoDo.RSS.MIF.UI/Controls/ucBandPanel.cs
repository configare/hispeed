using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.UI
{
    public partial class ucBandPanel : UserControl
    {
        private ExtractPanelHelper _entractHelper = new ExtractPanelHelper();
        private IArgumentProvider _argumentProvider;

        public event Action<string, int> OnBandSet;
        
        public ucBandPanel(IArgumentProvider _argumentProvider,Action<string, int> onBandSet)
        {
            // TODO: Complete member initialization
            this._argumentProvider = _argumentProvider;
            InitializeComponent();
            OnBandSet = onBandSet;
        }

        public void CreateBandPanel(AlgorithmDef algorithmDef)
        {
            this.Controls.Clear();

            BandDef[] bands = algorithmDef.Bands;
            if (bands == null || bands.Length == 0)
                return;
            List<Control> bandEdits = new List<Control>();
            int y = 0;
            for (int i = 0; i < bands.Length; i++)
            {
                BandDef band = bands[i];
                Label label = new Label();
                label.Location = new Point(0, (label.Height + 2) * i + 1);
                label.AutoSize = true;
                string bandName = band.Identify;
                if (_entractHelper.BandDes.ContainsKey(bandName))
                    bandName = _entractHelper.BandDes[bandName];
                if (band.Wavelength != null && band.Wavelength.Length != 0)
                {
                    string bandWave = "";
                    for (int w = 0; w < band.Wavelength.Length; w++)
                    {
                        bandWave += "," + band.Wavelength[w];
                    }
                    label.Text = bandName + "(" + bandWave.TrimStart(',') + ")";
                }
                else
                    label.Text = bandName;
                int defaultBand = GetDefaultBand(band);
                DoubleTextBox bandEdit = new DoubleTextBox();
                bandEdit.DefaultValue = 1;
                bandEdit.MinValue = 1;
                bandEdit.Width = 80;
                bandEdit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                bandEdit.Location = new Point(this.Right - 80 - 2, label.Top);
                bandEdit.Text = defaultBand.ToString();
                bandEdit.Tag = band;
                //bandEdit.KeyPressEnter += new KeyPressEventHandler(bandEdit_OnKeyPressEnter);
                //bandEdit.LostFocus += new EventHandler(bandEdit_LostFocus);
                //bandEdit.TextChanged+=new EventHandler(bandEdit_TextChanged);
                bandEdit.LostFocusValueChanged += new EventHandler(bandEdit_LostFocusValueChanged);

                this.Controls.Add(bandEdit);
                this.Controls.Add(label);
                bandEdits.Add(bandEdit);
                y = bandEdit.Bottom;
            }

            Button btnDefault = new Button();
            btnDefault.Text = "撤销更改";
            btnDefault.Location = new Point(0, y + 10);
            btnDefault.Click += new EventHandler(btnDefault_Click);
            btnDefault.Tag = bandEdits.ToArray();

            Button btnOk = new Button();
            btnOk.Text = "应用";
            btnOk.Location = new Point(btnDefault.Right + 4, y + 10);
            btnOk.Click += new EventHandler(btnOk_Click);
            btnOk.Tag = bandEdits.ToArray();

            this.Controls.Add(btnDefault);
            this.Controls.Add(btnOk);
        }

        void bandEdit_LostFocusValueChanged(object sender, EventArgs e)
        {
            UpdateArg(sender as DoubleTextBox);
        }

        //bool _valueChanged = false;

        //void  bandEdit_TextChanged(object sender, EventArgs e)
        //{   
        //    _valueChanged = true;
        //}

        //void bandEdit_LostFocus(object sender, EventArgs e)
        //{
        //    if (_valueChanged)
        //    {
        //        try
        //        {
        //            UpdateArg(sender as DoubleTextBox);
        //        }
        //        finally
        //        {
        //            _valueChanged = false;
        //        }
        //    }
        //}

        //void bandEdit_OnKeyPressEnter(object sender, KeyPressEventArgs e)
        //{
        //    if (_valueChanged)
        //    {
        //        try
        //        {
        //            UpdateArg(sender as DoubleTextBox);
        //        }
        //        finally
        //        {
        //            _valueChanged = false;
        //        }
        //    }
        //}

        private void UpdateArg(DoubleTextBox bandEdit)
        {
            BandDef band = bandEdit.Tag as BandDef;
            string bandName = band.Identify as string;
            int argValue = (int)bandEdit.Value;
            SetCurArg(bandName, argValue);
        }

        private void SetCurArg(string bandName, int argValue)
        {
            if (OnBandSet != null)
                OnBandSet(bandName, argValue);
        }

        private int GetDefaultBand(BandDef band)
        {
            object obj = _argumentProvider.GetArg(band.Identify);
            int bandValue = -1;
            if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()) || obj.ToString() == "-1")
                bandValue = band.BandNo;
            else if (!int.TryParse(obj.ToString(), out bandValue))
                bandValue = -1;
            SetCurArg(band.Identify, bandValue);
            return bandValue;
        }

        void btnDefault_Click(object sender, EventArgs e)
        {
            Control[] edits = (sender as Button).Tag as Control[];
            for (int i = 0; i < edits.Length; i++)
            {
                BandDef band = edits[i].Tag as BandDef;
                int bandValue = GetDefaultBand(band);
                edits[i].Text = bandValue.ToString();
                SetCurArg(band.Identify, bandValue);
            }
        }

        void btnOk_Click(object sender, EventArgs e)
        {
            Control[] edits = (sender as Button).Tag as Control[];
            for (int i = 0; i < edits.Length; i++)
            {
                BandDef band = edits[i].Tag as BandDef;
                int bandValue;
                if (int.TryParse(edits[i].Text, out bandValue))
                    SetCurArg(band.Identify, bandValue);
                else
                    edits[i].Text = "-1";
            }
        }
    }
}
