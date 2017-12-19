using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;
using System.Windows.Forms;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    public class cmdScale:BaseControlItem
    {
        private ToolStripComboBox _cb = null;
        private IMapControl _mapControl = null;

        public cmdScale()
            : base()
        {
            Init();
        }

        public cmdScale(bool beginGroup)
            : base(beginGroup)
        {
            Init();
        }

        private void Init()
        {
            _control = new ToolStripComboBox();
            _cb = _control as ToolStripComboBox;
            int[] scales = new int[] 
                                {
                                    60000000,
                                    10000000,
                                    5000000,
                                    1000000,
                                    500000,
                                    100000,
                                    10000,
                                    5000,
                                    2000
                                };
            for (int i = 0; i < scales.Length; i++)
                _cb.Items.Add(scales[i].ToString("###,###"));
            _cb.SelectedIndex = 1;
            _cb.SelectedIndexChanged += new EventHandler(_cb_SelectedIndexChanged);
        }

        void _cb_SelectedIndexChanged(object sender, EventArgs e)
        {
            string scaleText = _cb.Text.Replace(",",string.Empty);
            int scale = 0;
            if (int.TryParse(scaleText, out scale))
            {
                _mapControl.ScaleDenominator = scale;
            }
            else
            {
                _cb.Focus();
                _cb.SelectAll();
            }
        }

        public override void Init(IHook hook)
        {
            base.Init(hook);
            //
            _mapControl = (hook as IHookOfAgileMap).MapControl;
            IMapControlEvents events = _mapControl as IMapControlEvents;
            events.OnMapScaleChanged += new OnMapScaleChangedHandler(ScaleChanged);
        }

        private void ScaleChanged(object sender, int scale)
        {
            _cb.Text = scale.ToString("###,###");
        }
    }
}
