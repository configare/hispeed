#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2014-2-18 17:25:19
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.IO;
using System.Drawing;
using GeoDo.RSS.BlockOper;
using CodeCell.AgileMap.Core;
using System.Windows.Forms;
using GeoDo.MicroWaveSnow.FYJoin;
using GeoDo.RSS.UI.AddIn.Layout;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Core.DrawEngine;
namespace GeoDo.RSS.MIF.Prds.MWS
{
    /// <summary>
    /// 类名：SubProductAutoSD
    /// 属性描述：
    /// 创建者：lxj   创建日期：2014-2-18 17:25:19
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class SubProductAutoSD : CmaMonitoringSubProduct
    {
        const int THICK_DRY_SNOW = 1;
        const int THICK_WET_SNOW = 2;
        const int THIN_DRY_SNOW = 3;
        const int THIN_WET_SNOW_OR_FOREST_SNOW = 4;
        const int VERY_THICK_WET_SNOW = 5;
        const int NO_SNOW = 6;
        const int WET_FACTOR = -35;
        const int WET_FACTOR1 = -5;
        const int NO_SCATTER = 5;
        private IArgumentProvider _curArguments = null;
        private IContextMessage _contextMessage;
        private string layoutTime = null;
        public SubProductAutoSD(SubProductDef subProductDef)
            : base(subProductDef)
        {
        }
        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            _curArguments = _argumentProvider;
            if (_curArguments == null)
                return null;
            if (_curArguments.GetArg("AlgorithmName") == null)
                return null;
            if (_curArguments.GetArg("AlgorithmName").ToString() == "AutoMWSParaAlgorithm")
            {
                return AutoMWSParaAlgorithm(progressTracker);
            }
            return null;
        }
        private IExtractResult AutoMWSParaAlgorithm(Action<int, string> progressTracker)
        {
            List<string> autoParas = _argumentProvider.GetArg("UCAutoSDSWE") as List<string>;
            //判识参数
            string extrString = autoParas[autoParas.Count - 2];
            float[] extrParas = GetExtractPara(extrString);
            //四种地类计算参数
            string sdPara = autoParas[autoParas.Count - 1];
            float[] sdParas = GetSDPara(sdPara);
            //其它参数
            string chosefileInfo = autoParas[autoParas.Count - 3];
            string[] infos = chosefileInfo.Split(new char[] { ':' });
            //infos[0]: orbit
            //infos[1]:areaNams
            //infos[2]:radio 拼或者不拼
            //infos[3] + infos[4]: 起止时间
            infos[1] = infos[1].Substring( 0,infos[1].Length - 1);
            string[] areas = infos[1].Split(new char[] { ',' });
            string outname = "";
            foreach(string ars in areas)
            {
                outname = outname + ars.Substring(0,1);
            }
            if (infos[0] == "MWRIA")
            {
                layoutTime = infos[3].Substring(0, 4) + "年" + infos[3].Substring(4, 2) + "月" + infos[3].Substring(6, 2) + "日" + "—" +
                             infos[4].Substring(0, 4) + "年" + infos[4].Substring(4, 2) + "月" + infos[4].Substring(6, 2) + "日"+" 20点合成";

            }
            else
            {
                if (infos[0] == "MWRID")
                    layoutTime = infos[3].Substring(0, 4) + "年" + infos[3].Substring(4, 2) + "月" + infos[3].Substring(6, 2) + "日" + "—" +
                            infos[4].Substring(0, 4) + "年" + infos[4].Substring(4, 2) + "月" + infos[4].Substring(6, 2) + "日" + " 12点合成";
                else
                    layoutTime = infos[3].Substring(0, 4) + "年" + infos[3].Substring(4, 2) + "月" + infos[3].Substring(6, 2) + "日" + "—" +
                             infos[4].Substring(0, 4) + "年" + infos[4].Substring(4, 2) + "月" + infos[4].Substring(6, 2) + "日" + "合成";
            }
            autoParas.Remove(chosefileInfo);
            autoParas.Remove(extrString);
            autoParas.Remove(sdPara);
            string[] getfiles = autoParas.ToArray();
            if (getfiles.Length == 0 || getfiles == null)
                return null;
            CompTBDataJoin DataJoin = new CompTBDataJoin();
            progressTracker(1, "轨道数据投影拼接开始");
            ClipSNWParaData clipname = new ClipSNWParaData();
            string[] files = DataJoin.CompJoinData(getfiles, chosefileInfo);//拼接后的文件
            if (files.Length == 0 || files == null)
                return null;
            progressTracker(20, "轨道数据投影拼接完成");
            //查找参数数据
            string argFileDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\MWS\\SnowArgFile");
            string fileNameBare = Path.Combine(argFileDir, "china_bares.dat");
            string filenameGrass = Path.Combine(argFileDir, "china_grass.dat");
            string filenameForest = Path.Combine(argFileDir, "china_forest.dat");
            string filenameFarmhand = Path.Combine(argFileDir, "china_farmhand.dat");
            string filenameDensity = Path.Combine(argFileDir, "china_snow_density.dat");
            if (!File.Exists(filenameDensity))
                return null;
            //1、查找是否存在各土地类型百分比文件（4个），
            string bareFracFile = Path.ChangeExtension(fileNameBare, "_frac.dat");
            string grassFracFile = Path.ChangeExtension(filenameGrass, "_frac.dat");
            string forestFracFile = Path.ChangeExtension(filenameForest, "_frac.dat");
            string farmhandFracFile = Path.ChangeExtension(filenameFarmhand, "_frac.dat");
            string[] argFiles = new string[] { bareFracFile, grassFracFile, forestFracFile, farmhandFracFile };
            //2、计算雪深、雪水当量
            //雪深
            float outResolution = 0.1f;
            IExtractResultArray array = new ExtractResultArray("积雪参数");
            //（1）雪深原文件是拼（2）雪深平滑文件拼接（3）雪水当量原文件拼接（4）雪水当量平滑文件拼接
            List<string> fileSD = new List<string>();
            List<string> fileSDsmooth = new List<string>();
            List<string> fileSWE = new List<string>();
            List<string> fileSWEsmooth = new List<string>();
            progressTracker(21, "积雪参数提取开始");
            int count = files.Length;
            int j = 0;
            List<string> delefiles = new List<string>(); //20140521
            foreach (string file in files) //分别循环拼接后的文件
            {
                IFileExtractResult depthResult = ComputeSnowDepth(file, argFiles, outResolution, sdParas, extrParas);
                //雪水当量
                if (!File.Exists(filenameDensity))
                    return null;
                string depthFileName = (depthResult as FileExtractResult).FileName;
                if (!File.Exists(depthFileName))
                    return null;
                IFileExtractResult sweResult = ComputeSnowSWE(filenameDensity, depthFileName);
                fileSD.Add(depthResult.FileName);
                fileSWE.Add(sweResult.FileName);
                //中值滤波
                Int16 smoothwindow = 5;
                string sdfilename = depthResult.FileName;
                IFileExtractResult midSDFilterResult = ComputerMid(sdfilename, smoothwindow);//滤波
                string swefilename = sweResult.FileName;
                IFileExtractResult midSWEFilterResult = ComputerMid(swefilename, smoothwindow);
                IRasterDataProvider inRaster1 = GeoDataDriver.Open(midSDFilterResult.FileName) as IRasterDataProvider;
                string areanm = Path.GetFileName(file).Substring(Path.GetFileName(file).IndexOf("区域") + 2, Path.GetFileName(file).LastIndexOf("_") - Path.GetFileName(file).IndexOf("区域") - 2);
                if (inRaster1.Width >= 600)
                {
                    if (inRaster1 != null)
                        inRaster1.Dispose();
                    IFileExtractResult clipSD = clipname.ClipSNWResult(midSDFilterResult.FileName, areanm);
                    fileSDsmooth.Add(clipSD.FileName);
                    IFileExtractResult clipSWE = clipname.ClipSNWResult(midSWEFilterResult.FileName, areanm);
                    fileSWEsmooth.Add(clipSWE.FileName);
                    progressTracker(20 + (j + 1) * 30, "区域" + areanm + "计算完成");
                }
                else
                {
                    //临时条件，判断不是全国范围，就进行滤波后双线性插值
                    if (inRaster1 != null)
                        inRaster1.Dispose();
                    Int16 zoom = 10;
                    string identify = "MBSD";
                    IFileExtractResult sdBilinearfile =Bilinear(midSDFilterResult.FileName, identify, zoom);
                    File.SetAttributes(midSDFilterResult.FileName, FileAttributes.Normal);
                    File.Delete(midSDFilterResult.FileName);//删除原中值滤波的“MFSD”文件
                    string sdhdrfile = midSDFilterResult.FileName.Substring(0, midSDFilterResult.FileName.Length - 4) + ".hdr";
                    File.Delete(sdhdrfile);
                    IFileExtractResult sdReName = ReName(sdBilinearfile.FileName, "MBSD", "MFSD");//将插值的“MBSD”变成“MFSD”；
                    IFileExtractResult clipSD = clipname.ClipSNWResult(sdReName.FileName, areanm);
                    //File.SetAttributes(sdReName.FileName, FileAttributes.Normal);
                    //File.Delete(sdReName.FileName);  //裁完后删除被裁的那个
                    delefiles.Add(sdReName.FileName);
                    fileSDsmooth.Add(clipSD.FileName);
                    identify = "MBWE";
                    IFileExtractResult sweBilinearfile = Bilinear(midSWEFilterResult.FileName, identify, zoom);
                    File.SetAttributes(midSWEFilterResult.FileName, FileAttributes.Normal);
                    File.Delete(midSWEFilterResult.FileName);//删除原中值滤波的“MFWE”文件
                    string swehdrfile = midSWEFilterResult.FileName.Substring(0, midSWEFilterResult.FileName.Length - 4) + ".hdr";
                    File.Delete(swehdrfile);
                    IFileExtractResult sweReName = ReName(sweBilinearfile.FileName, "MBWE", "MFWE");//将插值的“MBWE”变成“MFWE”；
                    IFileExtractResult clipSWE = clipname.ClipSNWResult(sweReName.FileName, areanm);
                    fileSWEsmooth.Add(clipSWE.FileName);
                    //File.SetAttributes(sweReName.FileName, FileAttributes.Normal);
                    //File.Delete(sweReName.FileName);
                    delefiles.Add(sweReName.FileName);
                }
                progressTracker(20 + (j + 1) * 15, "区域" + areanm + "计算完成");
                j++;
            }
            FY3MWRIJoin Join = new FY3MWRIJoin();
            string[] sdfile = fileSD.ToArray();
            string[] sdfilesmooth = fileSDsmooth.ToArray();
            string[] swefile = fileSWE.ToArray();
            string[] swefilesmooth = fileSWEsmooth.ToArray();
           //2014.4.09 补充上出专题图  把平滑后的文件放到一个数组里
            List<string> layFile = new List<string>();
           //分别进行四项拼接，四个拼接结果给array.
            if (infos[2] == "yes")
            {
                progressTracker(20 + (j+1) * 15 + 1, "积雪参数数据拼接开始");
                string access = "create";
                //原雪深
                //拼文件名
                string[] sNames = Path.GetFileName(sdfile[0]).Split(new char[] { '_' });
                sNames[2] = outname;
                if(infos[0] == "MWRIA") 
                   sNames[sNames.Length - 1] = infos[3] + "120000"+ "_" + infos[4] + "120000" + ".dat";
                else
                   sNames[sNames.Length - 1] = infos[3] + "040000" + "_" + infos[4] + "040000" + ".dat";
                string outSDfile = Path.GetDirectoryName(sdfile[0]) + "\\";
                foreach (string s in sNames)
                {
                    outSDfile = outSDfile + s + "_";
                }
                outSDfile = outSDfile.Remove(outSDfile.Length - 1);
                Join.SNWParaFileJoin(sdfile, 0.1f, outSDfile, access);
                IFileExtractResult outSD = new FileExtractResult(_subProductDef.Identify, outSDfile, true);
                array.Add(outSD);
                //平滑雪深
                //拼文件名
                string[] sNames1 = Path.GetFileName(sdfilesmooth[0]).Split(new char[] { '_' });
                sNames1[2] = outname;
                if (infos[0] == "MWRIA")
                    sNames1[sNames1.Length - 1] = infos[3] + "120000" + "_" + infos[4] + "120000" + ".dat";
                else
                    sNames1[sNames1.Length - 1] = infos[3] + "040000" + "_" + infos[4] + "040000" + ".dat";
                string outsmoothSDfile = Path.GetDirectoryName(sdfile[0]) + "\\";//+ Path.GetFileNameWithoutExtension(sdfile[0]) + outname + ".dat";
                foreach (string s in sNames1)
                {
                    outsmoothSDfile = outsmoothSDfile + s + "_";
                }
                outsmoothSDfile = outsmoothSDfile.Remove(outsmoothSDfile.Length - 1);
                
                Join.SNWParaFileJoin(sdfilesmooth, 0.1f, outsmoothSDfile, access);
                IFileExtractResult outSDsmooth = new FileExtractResult(_subProductDef.Identify, outsmoothSDfile, true);
                array.Add(outSDsmooth);
                string infofile1 = Path.GetDirectoryName(outsmoothSDfile) + "\\" + Path.GetFileNameWithoutExtension(outsmoothSDfile) + ".INFO";
                File.Delete(infofile1);
                //原雪水当量
                //拼文件名
                string[] sNames2 = Path.GetFileName(swefile[0]).Split(new char[] { '_' });
                sNames2[2] = outname;
                if (infos[0] == "MWRIA")
                    sNames2[sNames2.Length - 1] = infos[3] + "120000" + "_" + infos[4] + "120000" + ".dat";
                else
                    sNames2[sNames2.Length - 1] = infos[3] + "040000" + "_" + infos[4] + "040000" + ".dat";
                string outSWEfile = Path.GetDirectoryName(sdfile[0]) + "\\";
                foreach (string s in sNames2)
                {
                    outSWEfile = outSWEfile + s + "_";
                }
                outSWEfile = outSWEfile.Remove(outSWEfile.Length - 1);
                Join.SNWParaFileJoin(swefile, 0.1f, outSWEfile, access);
                IFileExtractResult outSWE = new FileExtractResult(_subProductDef.Identify, outSWEfile, true);
                array.Add(outSWE);
                //平滑雪水当量
                string[] sNames3 = Path.GetFileName(swefilesmooth[0]).Split(new char[] { '_' });
                sNames3[2] = outname;
                if (infos[0] == "MWRIA")
                    sNames3[sNames3.Length - 1] = infos[3] + "120000" + "_" + infos[4] + "120000" + ".dat";
                else
                    sNames3[sNames3.Length - 1] = infos[3] + "040000" + "_" + infos[4] + "040000" + ".dat";
                string outsmoothSWEfile = Path.GetDirectoryName(sdfile[0]) + "\\";
                foreach (string s in sNames3)
                {
                    outsmoothSWEfile = outsmoothSWEfile + s + "_";
                }
                outsmoothSWEfile = outsmoothSWEfile.Remove(outsmoothSWEfile.Length - 1);
                Join.SNWParaFileJoin(swefilesmooth, 0.1f, outsmoothSWEfile, access);
                IFileExtractResult outSWEsmooth = new FileExtractResult(_subProductDef.Identify, outsmoothSWEfile, true);
                array.Add(outSWEsmooth);
                string info2 = Path.GetDirectoryName(outsmoothSWEfile) + "\\" + Path.GetFileNameWithoutExtension(outsmoothSWEfile) + ".INFO";
                File.Delete(info2);
                for (int i = 0; i < sdfile.Length; i++)
                {
                    string sdfilehdr = Path.GetDirectoryName(sdfile[i]) + Path.GetFileNameWithoutExtension(sdfile[i]) + ".hdr";
                    File.Delete(sdfile[i]);
                    File.Delete(sdfilehdr);
                    PrintInfo("删除原雪深" + sdfilehdr);
                    string sdsmoothhdr = Path.GetDirectoryName(sdfilesmooth[i]) + Path.GetFileNameWithoutExtension(sdfilesmooth[i]) + ".hdr";
                    File.Delete(sdfilesmooth[i]);
                    File.Delete(sdsmoothhdr);
                    PrintInfo("删除平滑雪深" + sdsmoothhdr);
                    string swefilehdr = Path.GetDirectoryName(swefile[i]) + Path.GetFileNameWithoutExtension(swefile[i]) + ".hdr";
                    File.Delete(swefile[i]);
                    File.Delete(swefilehdr);
                    string swesmoothhdr = Path.GetDirectoryName(swefilesmooth[i]) + Path.GetFileNameWithoutExtension(swefilesmooth[i]) + ".hdr";
                    File.Delete(swefilesmooth[i]); 
                    File.Delete(swesmoothhdr);
                }
                for (int i = 0; i < delefiles.Count; i++)
                {
                    string filehdr = Path.GetDirectoryName(delefiles[i]) + Path.GetFileNameWithoutExtension(delefiles[i]) + ".hdr";
                    File.Delete(delefiles[i]);
                    File.Delete(filehdr);
                }
                layFile.Add(outsmoothSDfile);
                layFile.Add(outsmoothSWEfile);
            }
            else
            {
                if (infos[2] == "no")
                {
                    progressTracker(20 + (j + 1) * 15 + 1, "积雪参数数据输出");
                    for (int i = 0; i < sdfile.Length; i++)
                    {
                        IFileExtractResult resSD = new FileExtractResult(_subProductDef.Identify, sdfile[i], true);
                        array.Add(resSD);
                        IFileExtractResult resSDsmooth = new FileExtractResult(_subProductDef.Identify, sdfilesmooth[i], true);
                        array.Add(resSDsmooth);
                        IFileExtractResult resSWE = new FileExtractResult(_subProductDef.Identify, swefile[i], true);
                        array.Add(resSWE);
                        IFileExtractResult resSWEsmooth = new FileExtractResult(_subProductDef.Identify, swefilesmooth[i], true);
                        array.Add(resSWEsmooth);
                        layFile.Add(sdfilesmooth[i]);
                        layFile.Add(swefilesmooth[i]);
                    }
                }
            }
            
            if (progressTracker != null)
                progressTracker(100, "计算完成");
           
            #region 专题图
            foreach (string arguments in layFile.ToArray())
            {
                if (arguments.Contains("MFSD"))
                {
                    _argumentProvider.SetArg("OutFileIdentify", "MADI");
                }
                if (arguments.Contains("MFWE"))
                {
                    _argumentProvider.SetArg("OutFileIdentify", "MAWI");
                }
                _argumentProvider.SetArg("SelectedPrimaryFiles", arguments);
                _argumentProvider.SetArg("fileOpenArgs", arguments);
                FileExtractResult result = ThemeGraphyResult(null) as FileExtractResult;
                array.Add(result);
            }
            #endregion
            return array;
        }
        private float[] GetSDPara(string sdPara)
        {
            string[] infos = sdPara.Split(new char[] { ',' });
            float value;
            float firPara = 0f; float secPara = 0f; float thrPara = 0f; float fivPara = 0f; float fouPara = 0f;
            float sixPara = 0f; float sevPara = 0f; float eigPara = 0f; float ninPara = 0f; float tenPara = 0f;
            float elePara = 0f; float twePara = 0f; float thirteenPara = 0f; float fourteenPara = 0f; float fifteenPara = 0f;
            if (float.TryParse(infos[0].Split('=')[1], out value))
                firPara = value;
            if (float.TryParse(infos[1].Split('=')[1], out value))
                secPara = value;
            if (float.TryParse(infos[2].Split('=')[1], out value))
                thrPara = value;
            if (float.TryParse(infos[3].Split('=')[1], out value))
                fouPara = value;
            if (float.TryParse(infos[4].Split('=')[1], out value))
                fivPara = value;
            if (float.TryParse(infos[5].Split('=')[1], out value))
                sixPara = value;
            if (float.TryParse(infos[6].Split('=')[1], out value))
                sevPara = value;
            if (float.TryParse(infos[7].Split('=')[1], out value))
                eigPara = value;
            if (float.TryParse(infos[8].Split('=')[1], out value))
                ninPara = value;
            if (float.TryParse(infos[9].Split('=')[1], out value))
                tenPara = value;
            if (float.TryParse(infos[10].Split('=')[1], out value))
                elePara = value;
            if (float.TryParse(infos[11].Split('=')[1], out value))
                twePara = value;
            if (float.TryParse(infos[12].Split('=')[1], out value))
                thirteenPara = value;
            if (float.TryParse(infos[13].Split('=')[1], out value))
                fourteenPara = value;
            if (float.TryParse(infos[14].Split('=')[1], out value))
                fifteenPara = value;
            return new float[] { firPara, secPara, thrPara, fouPara, fivPara, sixPara, sevPara, eigPara, ninPara, tenPara, elePara, twePara, thirteenPara, fourteenPara, fifteenPara };
        }
        private float[] GetExtractPara(string extractPara)
        {
            string[] infos = extractPara.Split(new char[] { ',' });
            float value;
            float firPara = 0f; float secPara = 0f; float thrPara = 0f; float fivPara = 0f; float fouPara = 0f;
            float sixPara = 0f; 
            if (float.TryParse(infos[0].Split('=')[1], out value))
                firPara = value;
            if (float.TryParse(infos[1].Split('=')[1], out value))
                secPara = value;
            if (float.TryParse(infos[2].Split('=')[1], out value))
                thrPara = value;
            if (float.TryParse(infos[3].Split('=')[1], out value))
                fouPara = value;
            if (float.TryParse(infos[4].Split('=')[1], out value))
                fivPara = value;
            if (float.TryParse(infos[5].Split('=')[1], out value))
                sixPara = value;
            return new float[] { firPara, secPara, thrPara, fouPara, fivPara, sixPara};
        }
        //专题图相关设置
        #region 
        GxdEnvelope _envelope = null;

        protected override void ApplyAttributesOfLayoutTemplate(ILayoutTemplate template)
        {
            string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
            string[] files = GetStringArray("SelectedPrimaryFiles");
            ILayout layout = template.Layout;
            double minx;
            double miny;
            double maxx;
            double maxy;
            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(files[0], "") as IRasterDataProvider)
            {
                minx = dataPrd.CoordEnvelope.MinX - 1;   //扩大数据框范围
                maxx = dataPrd.CoordEnvelope.MaxX + 1;
                miny = dataPrd.CoordEnvelope.MinY - 1;
                maxy = dataPrd.CoordEnvelope.MaxY + 1;
            }
            _envelope = new Layout.GxdEnvelope(minx, maxx, miny, maxy);
            Dictionary<string, string> vars = new Dictionary<string, string>();
            vars.Add("{JoinTime}", layoutTime);
            template.ApplyVars(vars); 
        }
        protected override void ApplyAttributesOfDataFrame(IGxdDataFrame gxdDataFrame, IDataFrame dataFrame, ILayout layout)
        {
            string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;

            Layout.GxdEnvelope evp = ToPrjEnvelope(_envelope, gxdDataFrame, dataFrame);
            if (evp != null)
            {
                FitSizeToTemplateWidth(layout, (float)(evp.MaxX - evp.MinX), (float)(evp.MaxY - evp.MinY));
                gxdDataFrame.Envelope = evp;
                _envelope = null;
            }

        }
        private GxdEnvelope ToPrjEnvelope(GxdEnvelope env, IGxdDataFrame gxdDataFrame, IDataFrame dataFrame)
        {
            if (env == null)
                return null;
            GeoDo.Project.IProjectionTransform tran = GetProjectionTransform(gxdDataFrame.SpatialRef);
            if (tran == null)
                return null;
            double[] xs = new double[] { env.MinX, env.MaxX };
            double[] ys = new double[] { env.MaxY, env.MinY };
            try
            {
                tran.Transform(xs, ys);
                return new Layout.GxdEnvelope(xs[0], xs[1], ys[1], ys[0]);
            }
            catch
            {
                return null;
            }
            
        }

        private void ChangeTemplateSize(ILayout layout, int width, int height)
        {
            IElement[] dfs = layout.QueryElements((e) => { return e is IDataFrame; });
            if (dfs == null || dfs.Length == 0)
                return;
            IDataFrame df = (dfs[0] as IDataFrame);
            if (df.Size.Width == width && df.Size.Height == height)
                return;
            float yOffset = height - df.Size.Height;
            float xOffset = width - df.Size.Width;
            df.IsLocked = false;
            df.ApplySize(xOffset, yOffset);
            df.IsLocked = true;
            layout.Size = new System.Drawing.SizeF(width, height);
            List<IElement> eles = layout.Elements;
            for (int i = 0; i < eles.Count; i++)
            {
                if (eles[i].Name == "标题" ||
                    eles[i].Name.Contains("Time") ||
                    eles[i].Name.Contains("Date"))
                    continue;
                if (eles[i] is IBorder ||
                    eles[i] is IDataFrame)
                    continue;
                if (eles[i] is ISizableElement)
                {
                    (eles[i] as ISizableElement).ApplyLocation(xOffset, yOffset);
                }
            }
        }

        private Project.IProjectionTransform GetProjectionTransform(string spatialRef)
        {
            if (spatialRef == null)
                return Project.ProjectionTransformFactory.GetDefault();
            else
                return Project.ProjectionTransformFactory.GetProjectionTransform(GeoDo.Project.SpatialReference.GetDefault(), GeoDo.Project.SpatialReferenceFactory.GetSpatialReferenceByWKT(spatialRef, Project.enumWKTSource.EsriPrjFile));
        }
        #endregion
        /// <summary>
        /// 修改文件名
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="oldIdentify"></param>
        /// <param name="newIdentify"></param>
        /// <returns></returns>
        private IFileExtractResult ReName(string filename, string oldIdentify, string newIdentify)
        {
            string fileReName = "";
            string hdrfile = "";
            if (File.Exists(filename))
            {
                FileInfo fi = new FileInfo(filename);
                fi.MoveTo(filename.Replace(oldIdentify, newIdentify));
                fileReName = fi.FullName;
                hdrfile = filename.Substring(0, filename.Length - 4) + ".hdr";
                FileInfo fihdr = new FileInfo(hdrfile);
                fihdr.MoveTo(hdrfile.Replace(oldIdentify, newIdentify));
            }
            IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, fileReName, true);
            return res;
        }
        /// <summary>
        /// 中值滤波
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private IFileExtractResult ComputerMid(string filename, Int16 smoothwindow)
        {
            List<RasterMaper> rms = null;
            IRasterDataProvider outRaster = null;
            RasterProcessModel<float, float> rfr = null;
            //float outResloution = 0.01f;
            string identyString = "";
            if (filename.Contains("MWSD"))
            {
                identyString = "MFSD";// 雪深平滑文件标识
            }
            if (filename.Contains("MSWE"))
            {
                identyString = "MFWE"; //雪水当量平滑文件标识
            }
            try
            {
                rms = new List<RasterMaper>();
                IRasterDataProvider inRaster1 = GeoDataDriver.Open(filename) as IRasterDataProvider;
                RasterMaper fileIn1 = new RasterMaper(inRaster1, new int[] { 1 });
                rms.Add(fileIn1);

                string middelFilterFileName = GetFileName(new string[] { filename }, _subProductDef.ProductDef.Identify, identyString, ".dat", null);
                outRaster = CreateOutRaster(middelFilterFileName, enumDataType.Float, rms.ToArray(), inRaster1.ResolutionX);
                //outRaster = CreateOutRaster(middelFilterFileName, enumDataType.Float, rms.ToArray(), outResloution);
                RasterMaper fileOut = new RasterMaper(outRaster, new int[] { 1 });
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { fileOut };
                rfr = new RasterProcessModel<float, float>();
                rfr.SetRaster(fileIns, fileOuts);
                rfr.RegisterCalcModel(new RasterCalcHandler<float, float>((rvInVistor, rvOutVistor, aoi) =>
                {
                    if (rvInVistor[0].RasterBandsData[0] != null)
                    {

                        int dataLength = rvInVistor[0].SizeY * rvInVistor[0].SizeX;
                        //int dataLength = rvInVistor[0].RasterBandsData[0].Length;
                        float[] outpixel = new float[dataLength];
                        float[] temp = new float[smoothwindow * smoothwindow];
                        int col = rvInVistor[0].SizeX;
                        if (smoothwindow == 5)
                        {
                            for (int i = 0; i < dataLength; i++)
                            {
                                if (i < 2 * col || i % col == 0 || (i - 1) % col == 0 || (i + 1) % col == 0 || (i + 2) % col == 0 || i > dataLength - 2 * col)
                                {
                                    if (rvInVistor[0].RasterBandsData[0][i] == -999.0f)
                                    {
                                        rvOutVistor[0].RasterBandsData[0][i] = -999.0f;
                                    }
                                    else
                                        rvOutVistor[0].RasterBandsData[0][i] = rvInVistor[0].RasterBandsData[0][i];
                                }
                                else
                                {
                                    if (rvInVistor[0].RasterBandsData[0][i] == -999.0f)
                                    {
                                        rvOutVistor[0].RasterBandsData[0][i] = -999.0f;
                                    }
                                    else
                                    {
                                        temp[0] = rvInVistor[0].RasterBandsData[0][i - 2 * col - 2];
                                        temp[1] = rvInVistor[0].RasterBandsData[0][i - 2 * col - 1];
                                        temp[2] = rvInVistor[0].RasterBandsData[0][i - 2 * col];
                                        temp[3] = rvInVistor[0].RasterBandsData[0][i - 2 * col + 1];
                                        temp[4] = rvInVistor[0].RasterBandsData[0][i - 2 * col + 2];
                                        temp[5] = rvInVistor[0].RasterBandsData[0][i - col - 2];
                                        temp[6] = rvInVistor[0].RasterBandsData[0][i - col - 1];
                                        temp[7] = rvInVistor[0].RasterBandsData[0][i - col];
                                        temp[8] = rvInVistor[0].RasterBandsData[0][i - col + 1];
                                        temp[9] = rvInVistor[0].RasterBandsData[0][i - col + 2];
                                        temp[10] = rvInVistor[0].RasterBandsData[0][i - 2];
                                        temp[11] = rvInVistor[0].RasterBandsData[0][i - 1];
                                        temp[12] = rvInVistor[0].RasterBandsData[0][i];
                                        temp[13] = rvInVistor[0].RasterBandsData[0][i + 1];
                                        temp[14] = rvInVistor[0].RasterBandsData[0][i + 2];
                                        temp[15] = rvInVistor[0].RasterBandsData[0][i + col - 2];
                                        temp[16] = rvInVistor[0].RasterBandsData[0][i + col - 1];
                                        temp[17] = rvInVistor[0].RasterBandsData[0][i + col];
                                        temp[18] = rvInVistor[0].RasterBandsData[0][i + col + 1];
                                        temp[19] = rvInVistor[0].RasterBandsData[0][i + col + 2];
                                        temp[20] = rvInVistor[0].RasterBandsData[0][i + 2 * col - 2];
                                        temp[21] = rvInVistor[0].RasterBandsData[0][i + 2 * col - 1];
                                        temp[22] = rvInVistor[0].RasterBandsData[0][i + 2 * col];
                                        temp[23] = rvInVistor[0].RasterBandsData[0][i + 2 * col + 1];
                                        temp[24] = rvInVistor[0].RasterBandsData[0][i + 2 * col + 2];
                                        int count = 0;
                                        for (int n = 0; n < 25; n++)
                                        {
                                            if (temp[n] == -999.0f)
                                                count++;
                                        }
                                        Array.Sort(temp);
                                        if (count >= 12)
                                            rvOutVistor[0].RasterBandsData[0][i] = rvInVistor[0].RasterBandsData[0][i];
                                        else
                                            rvOutVistor[0].RasterBandsData[0][i] = temp[temp.Length / 2];
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < dataLength; i++)
                            {

                                if (i < col || i % col == 0 || (i + 1) % col == 0 || i > dataLength - col)
                                {
                                    rvOutVistor[0].RasterBandsData[0][i] = rvInVistor[0].RasterBandsData[0][i];
                                }
                                else
                                {
                                    if (rvInVistor[0].RasterBandsData[0][i] == -999.0f)
                                    {
                                        rvOutVistor[0].RasterBandsData[0][i] = -999.0f;
                                    }
                                    else
                                    {
                                        temp[0] = rvInVistor[0].RasterBandsData[0][i - col - 1];
                                        temp[1] = rvInVistor[0].RasterBandsData[0][i - col];
                                        temp[2] = rvInVistor[0].RasterBandsData[0][i - col + 1];
                                        temp[3] = rvInVistor[0].RasterBandsData[0][i - 1];
                                        temp[4] = rvInVistor[0].RasterBandsData[0][i];
                                        temp[5] = rvInVistor[0].RasterBandsData[0][i + 1];
                                        temp[6] = rvInVistor[0].RasterBandsData[0][i + col - 1];
                                        temp[7] = rvInVistor[0].RasterBandsData[0][i + col];
                                        temp[8] = rvInVistor[0].RasterBandsData[0][i + col + 1];
                                        int count = 0;
                                        for (int n = 0; n < 9; n++)
                                        {
                                            if (temp[n] == -999.0f)
                                                count++;
                                        }
                                        Array.Sort(temp);
                                        if (count >= 4)
                                            rvOutVistor[0].RasterBandsData[0][i] = rvInVistor[0].RasterBandsData[0][i];
                                        else
                                            rvOutVistor[0].RasterBandsData[0][i] = temp[temp.Length / 2];
                                    }
                                }
                            }
                        }
                    }
                }));
                rfr.Excute();
                IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, middelFilterFileName, true);
                res.SetDispaly(false);
                return res;
            }
            finally
            {
                if (outRaster != null)
                    outRaster.Dispose();
                if (rms != null && rms.Count > 0)
                {
                    foreach (RasterMaper rm in rms)
                    {
                        if (rm.Raster != null)
                            rm.Raster.Dispose();
                    }
                }
            }

        }
        private IFileExtractResult ComputeSnowSWE(string filenameDensity, string depthFileName)
        {
            List<RasterMaper> rms = null;
            IRasterDataProvider outRaster = null;
            RasterProcessModel<float, float> rfr = null;
            try
            {
                rms = new List<RasterMaper>();
                IRasterDataProvider inRaster1 = GeoDataDriver.Open(depthFileName) as IRasterDataProvider;
                RasterMaper fileIn1 = new RasterMaper(inRaster1, new int[] { 1 });
                rms.Add(fileIn1);
                IRasterDataProvider inRaster2 = GeoDataDriver.Open(filenameDensity) as IRasterDataProvider;
                RasterMaper fileIn2 = new RasterMaper(inRaster2, new int[] { 1 });
                rms.Add(fileIn2);

                string sweFileName = GetFileName(new string[] { depthFileName }, _subProductDef.ProductDef.Identify, "MSWE", ".dat", null);
                outRaster = CreateOutRaster(sweFileName, enumDataType.Float, rms.ToArray(), inRaster1.ResolutionX);
                RasterMaper fileOut = new RasterMaper(outRaster, new int[] { 1 });
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { fileOut };
                rfr = new RasterProcessModel<float, float>();
                rfr.SetRaster(fileIns, fileOuts);

                rfr.RegisterCalcModel(new RasterCalcHandler<float, float>((rvInVistor, rvOutVistor, aoi) =>
                {
                    if (rvInVistor[0].RasterBandsData[0] != null && rvInVistor[1].RasterBandsData[0] != null)
                    {
                        //int dataLength = rvInVistor[0].SizeY * rvInVistor[0].SizeX;
                        int dataLength = rvInVistor[0].RasterBandsData[0].Length;
                        float[] swetmp = new float[dataLength];
                        for (int i = 0; i < dataLength; i++)
                        {
                            if (rvInVistor[1].RasterBandsData[0][i] == 0)
                            {
                                rvInVistor[1].RasterBandsData[0][i] = 0.242666f;
                            }
                            if (rvInVistor[0].RasterBandsData[0][i] == -999.0f)
                            {
                                swetmp[i] = -999.0f;
                            }
                            else
                            {
                                if (rvInVistor[0].RasterBandsData[0][i] == 0.003f)
                                {
                                    swetmp[i] = 0.003f;
                                }
                                else
                                {
                                    swetmp[i] = rvInVistor[0].RasterBandsData[0][i] * rvInVistor[1].RasterBandsData[0][i] * 10;
                                }
                                if (swetmp[i] < 0.0f)
                                    swetmp[i] = 0.003f; //为与后拼接时默认的0区分开
                            }
                            if (swetmp[i] < 5.0f && swetmp[i] > 0.0031f) //20140523 将小于5值改为5
                                swetmp[i] = 5.0f;
                            rvOutVistor[0].RasterBandsData[0][i] = swetmp[i];
                        }
                    }
                }));
                rfr.Excute();
                IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, sweFileName, true);
                res.SetDispaly(false);
                return res;
            }
            finally
            {
                if (outRaster != null)
                    outRaster.Dispose();
                if (rms != null && rms.Count > 0)
                {
                    foreach (RasterMaper rm in rms)
                    {
                        if (rm.Raster != null)
                            rm.Raster.Dispose();
                    }
                }
            }
        }
        private IFileExtractResult ComputeSnowDepth(string inputFileName, string[] argFiles, float outResolution, float[] sdParas, float[] extrParas)
        { 
            
            List<RasterMaper> rms = null;
            IRasterDataProvider outRaster = null;
            RasterProcessModel<Int16, float> rfr = null;
            try
            {
                rms = new List<RasterMaper>();
                IRasterDataProvider inRaster = GeoDataDriver.Open(inputFileName) as IRasterDataProvider;
                RasterMaper fileIn = new RasterMaper(inRaster, new int[]{1,2,3,4,5,6,7,8,9,10});
                rms.Add(fileIn);
                foreach (string file in argFiles)
                {
                    IRasterDataProvider argRaster = GeoDataDriver.Open(file) as IRasterDataProvider;
                    RasterMaper argRm = new RasterMaper(argRaster, new int[] { 1 });
                    rms.Add(argRm);
                }
                string depthFileName = GetFileName(new string[] { inputFileName }, _subProductDef.ProductDef.Identify, "MWSD", ".dat", null);
                outRaster = CreateOutRaster(depthFileName, enumDataType.Float, rms.ToArray(), outResolution);
                RasterMaper fileOut = new RasterMaper(outRaster, new int[] { 1 });
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { fileOut };
                rfr = new RasterProcessModel<Int16, float>();
                rfr.SetRaster(fileIns, fileOuts);
               rfr.RegisterCalcModel(new RasterCalcHandler<Int16, float>((rvInVistor, rvOutVistor, aoi) =>
                {
                    if (rvInVistor[0].RasterBandsData[0] != null && rvInVistor[1].RasterBandsData[0] != null &&
                        rvInVistor[2].RasterBandsData[0] != null && rvInVistor[3].RasterBandsData[0] != null &&
                        rvInVistor[4].RasterBandsData[0] != null)
                    {
                        int dataLength = rvInVistor[0].RasterBandsData[0].Length;
                        int[] type = new int[dataLength];
                        float[] sdtmp = new float[dataLength];
                        for (int i = 0; i < dataLength; i++)
                        {
                            //type
                            //type[i] = NO_SCATTER;
                            float ch10v = btValue(rvInVistor[0].RasterBandsData[0][i]);
                            float ch10h = btValue(rvInVistor[0].RasterBandsData[1][i]);
                            float ch18v = btValue(rvInVistor[0].RasterBandsData[2][i]);
                            float ch18h = btValue(rvInVistor[0].RasterBandsData[3][i]);
                            float ch23v = btValue(rvInVistor[0].RasterBandsData[4][i]);
                            float ch23h = btValue(rvInVistor[0].RasterBandsData[5][i]);
                            float ch36v = btValue(rvInVistor[0].RasterBandsData[6][i]);
                            float ch36h = btValue(rvInVistor[0].RasterBandsData[7][i]);
                            float ch89v = btValue(rvInVistor[0].RasterBandsData[8][i]);
                            float ch89h = btValue(rvInVistor[0].RasterBandsData[9][i]);
                            float si1 = ch23v - ch89v;
                            float si2 = ch18v - ch36v;
                            if (ch10v == 0.0f)   //数据未覆盖区或者填充值区
                            {
                                sdtmp[i] = -999.0f;
                            }
                            else
                            {
                                if (si1 >= extrParas[0] || si2 >= extrParas[1])
                                {
                                    if (ch23v <= extrParas[2])
                                    {
                                        if (ch18v - ch36v >= extrParas[3])
                                        {
                                            if (si1 - si2 >= extrParas[4])
                                                type[i] = THICK_DRY_SNOW;
                                            else
                                                type[i] = THICK_WET_SNOW;
                                        }
                                        else
                                        {
                                            if (si1 - si2 >= extrParas[5])
                                                type[i] = THIN_DRY_SNOW;
                                            else
                                            {
                                                if (si1 - si2 <= WET_FACTOR1)
                                                    type[i] = VERY_THICK_WET_SNOW;
                                                else
                                                {
                                                    if (ch18v - ch18h <= 6 && ch18v - ch36v >= 10)
                                                        type[i] = THIN_WET_SNOW_OR_FOREST_SNOW;
                                                    else
                                                        type[i] = NO_SNOW;
                                                }
                                            }
                                        }
                                    }
                                    else
                                        type[i] = NO_SNOW;
                                }
                                else
                                    type[i] = NO_SNOW;
                                //sdtmp
                                //float sdFarmland = -5.690f + 0.345f * (ch18v - ch36h) + 0.817f * (ch89v - ch89h);
                                //float sdGrass = 4.320f + 0.506f * (ch18h - ch36h) - 0.131f * (ch18v - ch18h) + 0.183f * (ch10v - ch89h) - 0.123f * (ch18v - ch89h);
                                //float sdBaren = 3.418f + 0.411f * (ch36h - ch89h) - 0.212f * (ch10v - ch89v);
                                //float sdForest = 10.766f + 0.421f * (ch18h - ch36v) - 1.121f * (ch18v - ch18h) + 0.673f * (ch89v - ch89h);
                                float sdFarmland = sdParas[0] + sdParas[1] * (ch18v - ch36h) + sdParas[2] * (ch89v - ch89h);
                                float sdGrass = sdParas[3] + sdParas[4] * (ch18h - ch36h) - sdParas[5] * (ch18v - ch18h) + sdParas[6] * (ch10v - ch89h) - sdParas[7] * (ch18v - ch89h);
                                float sdBaren = sdParas[8] + sdParas[9] * (ch36h - ch89h) - sdParas[10] * (ch10v - ch89v);
                                float sdForest = sdParas[11] + sdParas[12] * (ch18h - ch36v) - sdParas[13] * (ch18v - ch18h) + sdParas[14] * (ch89v - ch89h);
                                sdtmp[i] = (rvInVistor[1].RasterBandsData[0][i] * sdBaren +
                                    rvInVistor[2].RasterBandsData[0][i] * sdGrass +
                                    rvInVistor[3].RasterBandsData[0][i] * sdForest +
                                    rvInVistor[4].RasterBandsData[0][i] * sdFarmland) / 10000;  //原地类百分比文件扩大了10000倍 
                                //设置输出数据值
                                if (type[i] == NO_SNOW || type[i] == THICK_WET_SNOW || type[i] == THIN_WET_SNOW_OR_FOREST_SNOW || type[i] == VERY_THICK_WET_SNOW)
                                {
                                    //sdtmp[i] = 0.0f;
                                    sdtmp[i] = 0.003f;//为后面拼接时与默认的0区分开。
                                }
                                if (sdtmp[i] < 5.0f && sdtmp[i] > 0.0031f) //20140523 将小于5值改为5
                                    sdtmp[i] = 5.0f;

                                if (sdtmp[i] < 0.0f)
                                    
                                    sdtmp[i] = 0.003f;
                            }
                            rvOutVistor[0].RasterBandsData[0][i] = sdtmp[i];
                        }
                    }
                    //输出
                }));
                rfr.Excute();
                IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, depthFileName, true);
                res.SetDispaly(false);
                return res;
            }
            finally
            {
                if (outRaster != null)
                    outRaster.Dispose();
                if (rms != null && rms.Count > 0)
                {
                    foreach (RasterMaper rm in rms)
                    {
                        if (rm.Raster != null)
                            rm.Raster.Dispose();
                    }
                }
            }
        }
        private float btValue(Int16 chbt)
        {
            float ch;
//          FY3B MWRI 轨道数据无效值是-999,
//          FY3C MWRI 轨道数据无效值是29999.
            if (chbt == 0||chbt == -999|| chbt == 29999 )
                ch = 0.0f;
            else
                ch = chbt * 0.01f + 327.68f;
            return ch;
        }
        /// <summary>
        /// 双线性插值方法提高原数据文件分辨率
        /// f(i+u,j+v)=(1-u)(1-v)*f(i,j) + (1-v)u*f(i,j+1) + v(1-u)*f(i+1,j) + uv*f(i+1,j+1)
        /// </summary>
        /// <param name="srcFilename">原文件</param>
        /// <param name="factor">分辨率提高比例如0.1度到0.01度，zoom = 10</param>
        /// <returns></returns>                                   
        public IFileExtractResult Bilinear(string srcFilename, string identify, Int16 zoom)
        {
            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider outRaster = null;
            IRasterDataProvider inRaster1 = GeoDataDriver.Open(srcFilename) as IRasterDataProvider;
            RasterMaper fileIn1 = new RasterMaper(inRaster1, new int[] { 1 });
            rms.Add(fileIn1);
            string sdBilinearfilename = GetFileName(new string[] { srcFilename }, _subProductDef.ProductDef.Identify, identify, ".dat", null);
            outRaster = CreateOutRaster(sdBilinearfilename, enumDataType.Float, rms.ToArray(), inRaster1.ResolutionX / zoom);
            float[] inRasterBuffer = new float[inRaster1.Width * inRaster1.Height];
            IRasterBand bandinRaster1 = inRaster1.GetRasterBand(1);
            float[] sd = new float[inRaster1.Width * inRaster1.Height];
            float[] sdSnow = new float[outRaster.Width * outRaster.Height];//输出数组
            unsafe
            {
                fixed (float* pointer = inRasterBuffer)
                {
                    IntPtr ptr = new IntPtr(pointer);
                    bandinRaster1.Read(0, 0, inRaster1.Width, inRaster1.Height, ptr, inRaster1.DataType, inRaster1.Width, inRaster1.Height);
                    for (int j = 0; j < inRaster1.Width * inRaster1.Height; j++)
                    {
                        sd[j] = inRasterBuffer[j];
                    }
                }
            }
            Int32[] index = new Int32[zoom * zoom];
            try
            {
                for (int i = 0; i < inRaster1.Width * inRaster1.Height; i++)
                {
                    if ((i + 1) % inRaster1.Width == 0 || i % inRaster1.Width == 0 || i >= inRaster1.Width * inRaster1.Height - inRaster1.Width || i < inRaster1.Width) //插值后的高分格子落在原低分格子的最后一列或第一列，或最后一行，最后一列
                    {
                        for (int row = 0; row < zoom; row++)
                        {
                            for (int col = 0; col < zoom; col++)
                            {
                                index[col + row * zoom] = (i / inRaster1.Width * zoom * outRaster.Width + i % inRaster1.Width * zoom) + (row * outRaster.Width) + col;
                                //sdSnow[index[col + row * zoom]] = 0.00000001f;
                                sdSnow[index[col + row * zoom]] = sd[i];
                            }
                        }
                    }
                    else
                    {
                        for (int row = 0; row < zoom; row++)
                        {
                            for (int col = 0; col < zoom; col++)
                            {
                                index[col + row * zoom] = (i / inRaster1.Width * zoom * outRaster.Width + i % inRaster1.Width * zoom) + (row * outRaster.Width) + col;
                                //(2)通过u,v所处的象限
                                int xCenter = zoom / 2;
                                int yCenter = zoom / 2;
                                if (col < xCenter && row < yCenter) //第二象限 高分格子所在象元f(i,j) 其余三个象元分别是f(i,j-1),f(i-1,j),f(i-1,j-1)
                                {
                                    float u = 0.5f + (float)(col * 1.0f / zoom);//列
                                    float v = 0.5f + (float)(row * 1.0f / zoom);//行
                                    
                                    sdSnow[index[col + row * zoom]] = (1 - u) * (1 - v) * sd[i - inRaster1.Width - 1] + (1 - v) * u * sd[i - inRaster1.Width] + v * (1 - u) * sd[i - 1] + u * v * sd[i];
                                }
                                if (col >= xCenter && row < yCenter) //第一象限 高分格子所在象元f(i,j) 其余三个象元分别是f(i-1,j),f(i-1,j+1),f(i,j+1)
                                {
                                    float u = (float)(col * 1.0f / zoom) - 0.5f; //列  横向 
                                    float v = (float)(row * 1.0f / zoom) + 0.5f;   //行 纵
                                    sdSnow[index[col + row * zoom]] = (1 - u) * (1 - v) * sd[i - inRaster1.Width] + (1 - v) * u * sd[i - inRaster1.Width + 1] + v * (1 - u) * sd[i] + u * v * sd[i + 1];
                                }
                                if (col < xCenter && row >= yCenter) //第三象限 高分格子所在象元f(i,j) 其余三个象元分别是f(i,j-1),f(i+1,j-1),f(i+1,j)
                                {
                                    float u = 0.5f + (float)(col * 1.0f / zoom); //列
                                    float v = (float)(row * 1.0f / zoom) - 0.5f; //行
                                    sdSnow[index[col + row * zoom]] = (1 - u) * (1 - v) * sd[i - 1] + (1 - v) * u * sd[i] + v * (1 - u) * sd[i + inRaster1.Width - 1] + v * u * sd[i + inRaster1.Width];
                                }
                                if (col >= xCenter && row >= yCenter) //第四象限 高分格子所在象元f(i,j) 其余三个象元分别是f(i,j+1),f(i+1,j),f(i+1,j+1)
                                {
                                    float u = (float)(col * 1.0f / zoom) - 0.5f; //列
                                    float v = (float)(row * 1.0f / zoom) - 0.5f;//行
                                    sdSnow[index[col + row * zoom]] = (1 - u) * (1 - v) * sd[i] + (1 - v) * u * sd[i + 1] + v * (1 - u) * sd[i + inRaster1.Width] + u * v * sd[i + inRaster1.Width + 1];
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                PrintInfo(ex.Message);
            }
            try
            {
                unsafe
                {
                    fixed (float* ptr = sdSnow)
                    {
                        IntPtr sdSnowPtr = new IntPtr(ptr);
                        IRasterBand bandoutRaster = outRaster.GetRasterBand(1);
                        bandoutRaster.Write(0, 0, outRaster.Width, outRaster.Height, sdSnowPtr, outRaster.DataType, outRaster.Width, outRaster.Height);
                    }
                }
                IFileExtractResult res = new FileExtractResult(_subProductDef.Identify, sdBilinearfilename, true);
                return res;
            }
            finally
            {
                if (outRaster != null)
                    outRaster.Dispose();
                if (rms != null && rms.Count > 0)
                {
                    foreach (RasterMaper rm in rms)
                    {
                        if (rm.Raster != null)
                            rm.Raster.Dispose();
                    }
                }
            }
        }
        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
            //创建输出删格文件
        protected IRasterDataProvider CreateOutRaster(string outFileName, enumDataType dataType, RasterMaper[] inrasterMaper, float resolution)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            GeoDo.RSS.Core.DF.CoordEnvelope outEnv = null;
            foreach (RasterMaper inRaster in inrasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv = outEnv.Intersect(inRaster.Raster.CoordEnvelope);
            }
            float resX, resY;
            if (resolution != 0f)
            {
                resX = resolution;
                resY = resolution;
            }
            else
            {
                resX = inrasterMaper[0].Raster.ResolutionX;
                resY = inrasterMaper[0].Raster.ResolutionY;
            }
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            if (File.Exists(outFileName))
                File.Delete(outFileName);
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, dataType, mapInfo) as RasterDataProvider;
            return outRaster;
        }
    }
}
