using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.ModelFabric;
using System.Windows.Forms;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    public class ArgFeatures : ArgRefType
    {
        public override bool IsNeedInput
        {
            get
            {
                return false;
            }
        }

        public override string ToString(object value)
        {
            if (value == null || (value as Feature[] == null))
                return string.Empty;
            string str = null;
            int n = 0;
            Feature[] fets = value as Feature[];
            foreach (Feature fet in fets)
            {
                if (n > 100)
                {
                    str += "...";
                    break;
                }
                str += (fet.OID.ToString()+",");
                n++;
            }
            return "{" + str.Substring(0, str.Length - 1) + "}";
        }

        public override object GetValue(object sender)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "ESRI Shape Files(*.shp)|*.shp";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    return ReadFeatures(dlg.FileName); 
                }
            }
            return null ;
        }

        private object ReadFeatures(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return null;
            IVectorFeatureDataReader dr = VectorDataReaderFactory.GetUniversalDataReader(filename) as IVectorFeatureDataReader;
            if (dr == null)
                return null;
            try
            {
                return dr.Features;
            }
            finally 
            {
                dr.Dispose();
            }
        }
    }
}
