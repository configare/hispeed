using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GeoDo.RasterProject;
using System.Text.RegularExpressions;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;
using GeoDo.Project;

namespace GeoDo.Smart.Tools.NSMCDataCoordCorrect
{
    public partial class Main : Form
    {
        private Regex _regex = null;
        private CoordCorrectArgs[] _args = null;
        private CoordCorrectArgParse _parse = null;
        private string _curIdentify = null;

        public Main()
        {
            InitializeComponent();
            _regex = new Regex(@"^(?<satellite>\S+)_(?<sensor>VIRR|MERSI|AVHRR|MODIS|VIRRX)(?<DayOrNight>D|N?)_(?<bound>\S+)_(?<level>L2|L3)_(?<product>\S+)_(?<instrument>\S+)_(?<project>\S+)_(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})_(?<utc>\S+)_(?<resolution>\d+)(?<units>\S+)_MS\S*.HDF$", RegexOptions.Compiled);
            _parse = new CoordCorrectArgParse();
            _args = _parse.LoadArgs();
            tvDataType.BeforeSelect += new TreeViewCancelEventHandler(tvDataType_BeforeSelect);
            LoadDataType();
        }

        void tvDataType_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            _curIdentify = e.Node.Tag as string;
            UpdateCurDataType();
        }

        private IRasterDataProvider CreateOutRaster(string outdir, IRasterDataProvider raster, ISpatialReference dstSpatial, PrjEnvelope envelope, double resolutionX, double resolutionY, string bandNames)
        {
            IRasterDataProvider outRaster = null;
            if (raster.Driver.Name == "MEM")    //smart 产品数据，输出也用MEM驱动，目前数据纠正这里不存在。
            {
                string[] options = new string[]
                {
                    "INTERLEAVE=BSQ",
                    "VERSION=MEM",
                    "SPATIALREF=" + dstSpatial.ToProj4String(),
                    "MAPINFO={" + 1 + "," + 1 + "}:{" + envelope.MinX + "," + envelope.MaxY + "}:{" + resolutionX + "," + resolutionY + "}",
                    string.IsNullOrWhiteSpace(bandNames)?"": "BANDNAMES=" + bandNames
                };
                string outfilename = Path.Combine(outdir, Path.GetFileNameWithoutExtension(raster.fileName) + ".DAT");
                outRaster = (raster.Driver as IRasterDataDriver).Create(outfilename, raster.Width, raster.Height, 1, enumDataType.Int16, options);
            }
            else//ldf
            {
                string[] options = new string[]
                {
                    "INTERLEAVE=BSQ",
                    "VERSION=LDF",
                    "SPATIALREF=" + dstSpatial.ToProj4String(),
                    "MAPINFO={" + 1 + "," + 1 + "}:{" + envelope.MinX + "," + envelope.MaxY + "}:{" + resolutionX + "," + resolutionY + "}",
                    "DATETIME=" + raster.DataIdentify.OrbitDateTime,
                    string.IsNullOrWhiteSpace(bandNames)?"": "BANDNAMES=" + bandNames
                };
                string outfilename = Path.Combine(outdir, Path.GetFileNameWithoutExtension(raster.fileName) + ".LDF");
                IRasterDataDriver driver = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
                outRaster = driver.Create(outfilename, raster.Width, raster.Height, 1, enumDataType.Int16, options);
            }
            return outRaster;
        }

        private PrjEnvelope GetEnvelopeFromFilename(string filename, CodeToCoordDef codeDic)
        {
            string filenameOnly = Path.GetFileName(filename);
            Match match = _regex.Match(filenameOnly);
            if (!match.Success)
                return null;
            Group boundGroup = match.Groups["bound"];
            if (boundGroup == null || !boundGroup.Success || string.IsNullOrWhiteSpace(boundGroup.Value))
                return null;
            string bound = boundGroup.Value;
            PrjEnvelope envelope = codeDic.Find(bound);//地理范围
            return envelope;
        }

        private string[] GetFiles(string path, string filter)
        {
            if (!Directory.Exists(path))
                return null;
            string[] files = Directory.GetFiles(path, filter);
            return files;
        }

        private void rbVIRRPOAD_CheckedChanged(object sender, EventArgs e)
        {
            _curIdentify = (sender as Control).Tag as string;
            UpdateCurDataType();
        }

        private void UpdateCurDataType()
        {
            foreach (CoordCorrectArgs arg in _args)
            {
                if (arg.Identify == _curIdentify)
                {
                    txtInputFilter.Text = arg.Filter;
                    txtDataNames.Text = arg.Datasets;
                    txtInputDir.Text = arg.Indir;
                    txtOutputDir.Text = arg.Outdir;
                    txtResolution.Text = arg.Resolution;
                }
            }
        }

        private void LoadDataType()
        {
            tvDataType.Nodes.Clear();
            foreach (CoordCorrectArgs arg in _args)
            { 
                TreeNode node = new TreeNode(arg.Name);
                node.Tag = arg.Identify;
                tvDataType.Nodes.Add(node);
            }
            if(tvDataType.Nodes.Count!=0)
                tvDataType.SelectedNode = tvDataType.Nodes[0];
        }

        #region 各项参数修改
        private void UpdateArgs()
        {
            string inDir = txtInputDir.Text;
            string filter = txtInputFilter.Text;
            string datasets = txtDataNames.Text;
            string outdir = txtOutputDir.Text;
            string resolution = txtResolution.Text;
            foreach (CoordCorrectArgs arg in _args)
            {
                if (arg.Identify == _curIdentify)
                {
                    arg.Indir = inDir;
                    arg.Outdir = outdir;
                    arg.Filter = filter;
                    arg.Datasets = datasets;
                    arg.Resolution = resolution;
                    break;
                }
            }
            _parse.WriteToXml(_args);
        }

        #endregion

        private void btnOk_Click(object sender, EventArgs e)
        {
            string path = txtInputDir.Text;
            string filter = txtInputFilter.Text;
            string projectionInfo = txtProjectionInfo.Text;
            string outdir = txtOutputDir.Text;
            string bandNames = txtDataNames.Text;
            string resolutionStr = txtResolution.Text;
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            {
                ShowMsg("参数不正确，输入路径为空或不存在");
                return;
            }
            if (string.IsNullOrWhiteSpace(projectionInfo))
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(bandNames))//输入参数不正确，没有定义波段
            {
                ShowMsg("参数不正确，没有定义波段");
                return;
            }
            if (string.IsNullOrWhiteSpace(outdir))
            {
                ShowMsg("参数不正确，没有定义输出路径");
                return;
            }
            string[] files = GetFiles(path, filter);
            if (files == null || files.Length == 0)
            {
                ShowMsg("输入路径下获取文件为空");
                return;
            }
            double resolution;
            if (!double.TryParse(resolutionStr, out resolution))
            {
                ShowMsg("分辨率输入错误");
                return;
            }
            CodeToCoordDef coordCode = new CodeToCoordDef();
            ISpatialReference spatialRef = SpatialReferenceFactory.GetSpatialReferenceByWKT(projectionInfo, enumWKTSource.EsriPrjFile);
            try
            {
                if (!Directory.Exists(outdir))
                    Directory.CreateDirectory(outdir);
                StartProgress();
                for (int fi = 0; fi < files.Length; fi++)
                {
                    string file = files[fi];
                    Progress((int)((fi + 1) * 100f / files.Length));
                    PrjEnvelope envelope = GetEnvelopeFromFilename(file, coordCode);
                    if (envelope == null || envelope.IsEmpty)
                        continue;
                    string[] args = new string[] { "datasets=" + bandNames };
                    using (IRasterDataProvider raster = RasterDataDriver.Open(file, args) as IRasterDataProvider)
                    {
                        if (raster == null)
                            continue;
                        if (raster.BandCount == 0)
                            continue;
                        using (IRasterDataProvider outRaster = CreateOutRaster(outdir, raster, spatialRef, envelope, resolution, resolution, bandNames))
                        {
                            short[] buffer = new short[raster.Width * raster.Height];
                            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                            try
                            {
                                IntPtr ptr = handle.AddrOfPinnedObject();
                                for (int i = 0; i < raster.BandCount; i++)
                                {
                                    raster.GetRasterBand(i + 1).Read(0, 0, raster.Width, raster.Height, ptr, raster.DataType, raster.Width, raster.Height);
                                    outRaster.GetRasterBand(i + 1).Write(0, 0, raster.Width, raster.Height, ptr, raster.DataType, raster.Width, raster.Height);
                                }
                            }
                            finally
                            {
                                if (handle.IsAllocated)
                                    handle.Free();
                            }
                        }
                    }
                }
                UpdateArgs();
                ShowMsg("执行结束");
            }
            catch (Exception ex)
            {
                ShowMsg(ex.Message);
            }
            finally
            {
                FinishProgress();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void StartProgress()
        {
            progressBar1.Visible = true;
            progressBar1.Value = 0;
        }

        private void Progress(int i)
        {
            if (progressBar1.Maximum >= i)
                progressBar1.Value = i;
        }

        private void FinishProgress()
        {
            progressBar1.Visible = false;
        }

        private void ShowMsg(string msg)
        {
            MessageBox.Show(msg, "系统消息");
        }

        private void btnIn_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog diag = new FolderBrowserDialog())
            {
                diag.RootFolder = Environment.SpecialFolder.MyComputer;
                diag.ShowNewFolderButton = false;
                diag.SelectedPath = txtInputDir.Text;
                diag.Description = "选择植被指数、陆表温度数据目录";
                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtInputDir.Text = diag.SelectedPath;
                }
            }
        }

        private void btnOut_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog diag = new FolderBrowserDialog())
            {
                diag.RootFolder = Environment.SpecialFolder.MyComputer;
                diag.SelectedPath = txtOutputDir.Text;
                diag.Description = "选择修正后的数据存放目录";
                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtOutputDir.Text = diag.SelectedPath;
                }
            }
        }
    }
}
