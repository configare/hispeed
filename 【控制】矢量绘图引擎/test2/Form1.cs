using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeCell.AgileMap.Core;
using CodeCell.Bricks.Runtime;

namespace test2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        object v;
        private void button1_Click(object sender, EventArgs e)
        {
            Color c = Color.Transparent;
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "ESRI Shape Files(*.shp)|*.shp";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    IVectorFeatureDataReader dr = VectorDataReaderFactory.GetUniversalDataReader(dlg.FileName) as IVectorFeatureDataReader;
                    //Feature fet = dr.FetchFirstFeature();
                    //fet = dr.FetchFeature((f) => { return f.GetFieldValue("CNTRY_NAME") == "China"; });
                     v = dr.FetchFeatures();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string fname = @"F:\\产品与项目\\MAS-II\\SMART0718\\SMART\\【控制】UI框架(新整理)\\SMART\\bin\\Release\\数据引用\\基础矢量\\行政区划\\面\\中国行政区.shp";
            string refdir = @"F:\\产品与项目\\MAS-II\\SMART0718\\SMART\\【控制】UI框架(新整理)\\SMART\\bin\\Release\\LayoutTemplate\\自定义\\TempMcd.xml";
            refdir = "f:\\";
            refdir = @"F:\\产品与项目\\MAS-II\\SMART0718\\SMART\\【控制】UI框架(新整理)\\SMART\\TempMcd.xml";
            refdir = "e:\\";
            refdir = "F:\\产品与项目\\MAS-II\\SMART0718\\SMART\\【控制】UI框架(新整理)\\SMART\\bin\\Release\\数据引用\\基础矢量\\行政区划\\面\\中国面\\西北面\\TempMcd.xml";

            string path = RelativePathHelper.GetRelativePath(refdir, fname);
            path =@"..\..\数据引用\基础矢量\行政区划\面\中国边界.SHP";
            refdir = @"F:\产品与项目\MAS-II\SMART0718\SMART\【控制】UI框架(新整理)\SMART\bin\Release\LayoutTemplate\自定义\测试相对路径3.gxt";
            string absPath = RelativePathHelper.GetFullPath(refdir, path);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "ESRI Shape Files(*.shp)|*.shp";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    MemoryEstimateOfShapeFile m = new MemoryEstimateOfShapeFile();
                    int featureCount ,fieldCount ;
                    float memorySize = m.Estimate(dlg.FileName, out featureCount, out fieldCount);

                    MessageBox.Show("要素个数:" + featureCount.ToString()+",字段个数:" + fieldCount.ToString() + ",预计占内存:" + memorySize.ToString()+" MB");
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Feature[] fets = new Feature[10000];
            for (int i = 0; i < 10000; i++)
            {
                fets[i] = new Feature(0, null, null, null, null);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FileDataSource ds = new FileDataSource("");
            FeatureClass fetclass = new FeatureClass(new GridDefinition(), new MemoryGridLimiter(), ds);
            FeatureLayer lyr = new FeatureLayer(null, fetclass);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            object v = null;
            IDisposable dis = v as IDisposable;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "全球火点数据(*.hdf)|*.hdf";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    using (IVectorFeatureDataReader dr = VectorDataReaderFactory.GetUniversalDataReader(dlg.FileName) as IVectorFeatureDataReader)
                    {
                        //dr.FeatureCount;
                        Feature[] features = dr.Features;
                        Console.WriteLine(features.Length);
                    }
                }
            }
        }
    }
}
