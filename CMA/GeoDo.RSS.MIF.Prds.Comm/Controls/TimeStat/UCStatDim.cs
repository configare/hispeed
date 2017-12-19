using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.Comm
{
    public partial class UCStatDim : UserControl, IArgumentEditorUI2
    {
        private Action<object> _handler;

        public UCStatDim()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            string[] names = Enum.GetNames(typeof(enumStatDimType));
            ckStatDim.Items.AddRange(names);
            ckStatDim.SelectedIndex = 0;
            names = Enum.GetNames(typeof(enumStatCompoundType));
            ckStatCompound.Items.AddRange(names);
            ckStatCompound.SelectedIndex = 0;
            names = Enum.GetNames(typeof(enumStatDayMosaicType));
            cbDayMosaicType.Items.AddRange(names);
            cbDayMosaicType.SelectedIndex = 0;
        }

        public object GetArgumentValue()
        {
            return new StatDimClass((enumStatDimType)Enum.Parse(typeof(enumStatDimType), ckStatDim.Text),
                                    (enumStatCompoundType)Enum.Parse(typeof(enumStatCompoundType), ckStatCompound.Text),
                                    (enumStatDayMosaicType)Enum.Parse(typeof(enumStatDayMosaicType), cbDayMosaicType.Text));
        }

        public void InitControl(IExtractPanel panel, ArgumentBase arg)
        {
            if (_handler != null)
                _handler(GetArgumentValue());

        }

        public bool IsExcuteArgumentValueChangedEvent
        {
            get
            {
                return false;
            }
            set
            {
                
            }
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            return new StatDimClass((enumStatDimType)Enum.Parse(typeof(enumStatDimType), ckStatDim.Text),
                                    (enumStatCompoundType)Enum.Parse(typeof(enumStatCompoundType), ckStatCompound.Text),
                                    (enumStatDayMosaicType)Enum.Parse(typeof(enumStatDayMosaicType), cbDayMosaicType.Text));
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        private void ckStatDim_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_handler != null)
                _handler(GetArgumentValue());
        }

        private void ckStatCompound_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_handler != null)
                _handler(GetArgumentValue());
        }

        private void cbDayMosaicType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_handler != null)
                _handler(GetArgumentValue());
        }
    }
}
