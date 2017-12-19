using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using System.Xml.Linq;

namespace GeoDo.Smart.TrueColorImageWorldFile
{
    public partial class Form1 : Form
    {
        private Regex[] regexs;
        SortedDictionary<string, Rectangle> tCode;

        public Form1()
        {
            InitializeComponent();
            tCode = TCode.GetTCode();
            InitRegexs();
        }

        private void InitRegexs()
        {
            string config = System.AppDomain.CurrentDomain.BaseDirectory + "GeoDo.Smart.TrueColorImageWorldFile.exe.config";
            if (File.Exists(config))
            {
                XElement xml = XElement.Load(config);
                XElement xmlregexs = xml.Element("Regexs");
                List<Regex> regexList = new List<Regex>();
                if (xmlregexs != null)
                {
                    IEnumerable<XElement> xmlrexs = xmlregexs.Elements("Regex");
                    foreach (XElement rex in xmlrexs)
                    {
                        if (!string.IsNullOrWhiteSpace(rex.Value))
                        {
                            regexList.Add(new Regex(rex.Value));
                        }
                    }
                }
                regexs = regexList.ToArray();
            }
            if (regexs == null || regexs.Length == 0)
            {
                regexs = new Regex[]
                {
                    new Regex(@"^FY3[A|B]_MERSI_(?<Code>T\d{3})_L2_PAD_MLT_(?<Projection>GLL)_\d{8}_\d{4}_(?<Resolution>0250)M_MS_03_02_01.JPG$", RegexOptions.Compiled),
                    new Regex(@"^FY3[A|B]_MERSI_(?<Code>T\d{3})_L2_PAD_MLT_(?<Projection>GLL)_\d{8}_POAD_(?<Resolution>0250)M_MS_03_02_01.JPG$", RegexOptions.Compiled)
                };
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string path = txtPath.Text;
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
                return;
            CreateWorldFile(path);
        }

        //FY3A_MERSI_T214_L2_PAD_MLT_GLL_20130408_2225_0250M_MS_03_02_01.JPG
        private void CreateWorldFile(string path)
        {
            int count = 0;
            string[] files = Directory.GetFiles(path);
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                string filenameonly = Path.GetFileName(file);
                foreach (Regex regex in regexs)
                {
                    if (regex.IsMatch(filenameonly))
                    {
                        Match match = regex.Match(filenameonly);
                        if (!match.Success)
                            continue;
                        Group gpCode = match.Groups["Code"];
                        string code = gpCode.Value;
                        Group gpProjection = match.Groups["Projection"];
                        string projection = gpProjection.Value;
                        Group gpResolution = match.Groups["Resolution"];
                        string resolution = gpResolution.Value;
                        if (tCode.ContainsKey(code))
                        {
                            Rectangle rect = tCode[code];
                            double minx = rect.Left;
                            double maxy = rect.Bottom;
                            double resolutionx = GetResolution(resolution);
                            ISpatialReference spatial = GetSpatial(projection);
                            WriteWorldFile(minx, maxy, resolutionx, resolutionx, file, spatial);
                        }
                        count++;
                    }
                }
            }
            MessageBox.Show("配准结束,共识别" + count + "个数据", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private ISpatialReference GetSpatial(string projection)
        {
            if (projection == "GLL")
                return SpatialReference.GetDefault();
            if (projection == "PGS")
                return null;
            return SpatialReference.GetDefault();
        }

        private double GetResolution(string resolution)
        {
            if (resolution == "0250")
                return 0.0025d;
            if (resolution == "1000")
                return 0.01d;
            return 0d;
        }

        private void WriteWorldFile(double minx, double maxy, double resolutionx, double resolutiony, string filename, ISpatialReference spatial)
        {
            WorldFile worldFile = new WorldFile();
            worldFile.CreatWorldFile(resolutionx, -resolutiony, minx, maxy, filename);
            worldFile.CreatXmlFile(spatial == null ? SpatialReference.GetDefault() : spatial, filename);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPath_Click(object sender, EventArgs e)
        {
            FillPath();
        }

        private void FillPath()
        {
            using (FolderBrowserDialog diag = new FolderBrowserDialog())
            {
                diag.Description = "选择待配准的路径";
                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtPath.Text = diag.SelectedPath;
                }
            }
        }
    }
}
