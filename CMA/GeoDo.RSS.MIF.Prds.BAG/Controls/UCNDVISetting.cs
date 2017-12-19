using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using System.Xml.Linq;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Prds.BAG
{
    public partial class UCNDVISetting : UserControl, IArgumentEditorUI
    {
        private Action<object> _handler;
        private NDVISettingItem[] _settingItems = null;

        public UCNDVISetting()
        {
            InitializeComponent();
        }

        public object GetArgumentValue()
        {
            if (_settingItems==null||_settingItems.Length < 1)
                return null;
            return _settingItems;
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
             //<DefaultValue>
             //   <ValueItem name="太湖" envelope="" minvalue="" maxvalue="" isuse="false"/>
             //   <ValueItem name="巢湖" envelope="" minvalue="" maxvalue="" isuse="false"/>
             //   <ValueItem name="滇池" envelope="" minvalue="" maxvalue="" isuse="false"/>
             //</DefaultValue>
            IEnumerable<XElement> nodes = ele.Elements("ValueItem");
            List<NDVISettingItem> settings = new List<NDVISettingItem>();
            if (nodes != null && nodes.Count() != 0)
            {
                foreach (XElement item in nodes)
                {
                    bool isUse;
                    float minvalue,maxvalue;
                    if(bool.TryParse(item.Attribute("isuse").Value,out isUse))
                    {
                        if (!isUse)
                            return null;
                        else
                        {
                            NDVISettingItem settingItem = new NDVISettingItem();
                            settingItem.IsUse = true;
                            settingItem.Name = item.Attribute("name").Value;
                            if (float.TryParse(item.Attribute("minvalue").Value, out minvalue) &&
                                float.TryParse(item.Attribute("maxvalue").Value, out maxvalue))
                            {
                                settingItem.MaxValue = minvalue;
                                settingItem.MinValue = maxvalue;
                                //解析envelope
                                string envelopeStr = item.Attribute("envelope").Value;
                                string[] strArray = envelopeStr.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
                                if (strArray == null || strArray.Length != 4)
                                    continue;
                                CoordEnvelope envelope = new CoordEnvelope(double.Parse(strArray[0]), double.Parse(strArray[1]),
                                    double.Parse(strArray[2]),double.Parse(strArray[3]));
                                settingItem.Envelope = envelope;
                                settings.Add(settingItem);
                            }
                            else
                                continue;
                        }
                    }
                }
                if (settings != null && settings.Count > 0)
                    _settingItems = settings.ToArray();
                if (_handler != null)
                    _handler(GetArgumentValue());
            }
            return null;
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }
    }
}
