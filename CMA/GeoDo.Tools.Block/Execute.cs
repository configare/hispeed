using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GeoDo.FileProject;
using GeoDo.RasterProject;
using GeoDo.RSS.BlockOper;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.UI.AddIn.DataPro;
using System.Drawing;
using System.Drawing.Imaging;

namespace GeoDo.Tools.Block
{
    public class Execute
    {
        private RasterClipProcesser _rasterClipProcess;
        private int _blockPngSize;

        public Execute()
        {
            _rasterClipProcess = new RasterClipProcesser();

            string size = System.Configuration.ConfigurationManager.AppSettings["BlockThumbnailSize"];
            if (!string.IsNullOrWhiteSpace(size))
                int.TryParse(size, out _blockPngSize);
            
        }

        public void Do(string xmlArg)
        {
            BlockArg inArg = BlockArg.ParseXml(xmlArg);

            string outDir = inArg.OutputDir;
            IRasterDataProvider inRaster = null;
            OutputArg outArg = new OutputArg();
            try
            {
                CheckArg(inArg);
                BlockDef[] blocks = GenericBlock(inArg);
                inRaster = GeoDataDriver.Open(inArg.InputFilename) as IRasterDataProvider;
                DataIdentify dataIdentify = inRaster.DataIdentify;
                outArg.OrbitFilename = Path.GetFileName(inArg.InputFilename);
                outArg.Satellite = dataIdentify.Satellite;
                outArg.Sensor = dataIdentify.Sensor;
                outArg.Level = "L1";
                outArg.ProjectionIdentify = ParseProjectionIdentify(Path.GetFileName(inArg.InputFilename));
                outArg.ObservationDate = dataIdentify.OrbitDateTime.ToString("yyyyMMdd");
                outArg.ObservationTime = dataIdentify.OrbitDateTime.ToString("HHmm");
                outArg.Station = ParseStation(Path.GetFileName(inArg.InputFilename));
                List<FileArg> fileArgs = new List<FileArg>();
                string retMessage = "";
                for (int blockIndex = 0; blockIndex < blocks.Length; blockIndex++)
                {
                    BlockDef blockDef = blocks[blockIndex];
                    if (blocks == null)
                        continue;
                    CoordEnvelope oEnvelope = inRaster.CoordEnvelope;
                    CoordEnvelope tEnvelope = blockDef.ToEnvelope();
                    IRasterDataProvider outBlockRasters = null;
                    try
                    {
                        if (!IsInteractived(oEnvelope, tEnvelope))
                            continue;
                        double validPercent = 0;
                        outBlockRasters = _rasterClipProcess.Clip(inRaster, blockDef, 100, "LDF", outDir, new Action<int, string>(OnProgress), out validPercent);
                        if (outBlockRasters == null)
                            continue;
                        if (validPercent == 0)
                        {
                            string filename = outBlockRasters.fileName;
                            outBlockRasters.Dispose();
                            outBlockRasters = null;
                            TryDeleteLdfFiles(filename);
                            continue;
                        }
                        IRasterDataProvider outfileRaster = outBlockRasters;
                        string overViewFilename = OverViewHelper.OverView(outfileRaster, _blockPngSize);
                        FileArg fileArg = new FileArg();
                        CoordEnvelope env = null;
                        float resolutionX;
                        float resolutionY;
                        env = outfileRaster.CoordEnvelope;
                        resolutionX = outfileRaster.ResolutionX;
                        resolutionY = outfileRaster.ResolutionY;
                        fileArg.OutputFilename = Path.GetFileName(outfileRaster.fileName);
                        fileArg.Thumbnail = (string.IsNullOrWhiteSpace(overViewFilename) ? "" : Path.GetFileName(overViewFilename));
                        fileArg.ExtendFiles = Path.ChangeExtension(Path.GetFileName(outfileRaster.fileName), "hdr");
                        fileArg.Envelope = new PrjEnvelopeItem(blockDef.Name, env == null ? null : new RasterProject.PrjEnvelope(env.MinX, env.MaxX, env.MinY, env.MaxY));
                        fileArg.ResolutionX = resolutionX.ToString();
                        fileArg.ResolutionY = resolutionY.ToString();
                        fileArg.Length = new FileInfo(outfileRaster.fileName).Length;
                        fileArgs.Add(fileArg);
                    }
                    catch (Exception ex)
                    {
                        retMessage += (ex.Message + " Block:" + blockDef.Name + tEnvelope == null ? "" : tEnvelope.ToString());
                    }
                    finally
                    {
                        if (outBlockRasters != null)
                        {
                            if (outBlockRasters != null)
                            {
                                outBlockRasters.Dispose();
                            }
                        }
                    }
                }
                outArg.OutputFiles = fileArgs.ToArray();
                outArg.LogLevel = "info";
                if (string.IsNullOrWhiteSpace(retMessage))
                    outArg.LogInfo = "分幅成功";
                else
                    outArg.LogInfo = retMessage;
            }
            catch (Exception ex)
            {
                LogFactory.WriteLine(ex);
                outArg.LogLevel = "error";
                outArg.LogInfo = ex.Message;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                string outXmlFilename = Path.Combine(inArg.OutputDir, Path.GetFileName(inArg.InputFilename) + ".xml");
                OutputArg.WriteXml(outArg, outXmlFilename);
                if (inRaster != null)
                    inRaster.Dispose();
            }
        }

        private static void TryDeleteLdfFiles(string filename)
        {
            try
            {
                File.Delete(filename);
                File.Delete(Path.ChangeExtension(filename, "hdr"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("删除空白数据失败 " + ex.Message);
            }
        }

        private bool IsInteractived(CoordEnvelope envelopeA, CoordEnvelope envelopeB)
        {
            PrjEnvelope prja = new PrjEnvelope(envelopeA.MinX, envelopeA.MaxX, envelopeA.MinY, envelopeA.MaxY);
            PrjEnvelope prjb = new PrjEnvelope(envelopeB.MinX, envelopeB.MaxX, envelopeB.MinY, envelopeB.MaxY);
            return prja.IntersectsWith(prjb);
        }

        private void CheckArg(BlockArg inArg)
        {
            if (string.IsNullOrWhiteSpace(inArg.BlockType))
                throw new Exception("节点BlockType必须为以下值中的一个：identify、custom");
            if (inArg.BlockType == "identify")
            {
                if (string.IsNullOrWhiteSpace(inArg.BlockIdentify))
                    throw new Exception("BlockType为identify时候，BlockIdentify节点必须有值");
                //if (inArg.BlockIdentify != "005" && inArg.BlockIdentify != "010")
                //    throw new Exception("BlockType为identify时候，BlockIdentify节点必须为以下值中的一个：005、010");
            }
            else if (inArg.BlockType == "custom")
            {
                if (inArg.BlockEnvelopes == null || inArg.BlockEnvelopes.Length == 0)
                    throw new Exception("BlockType为custom时候，Blocks节点必须有值");
            }
            if (string.IsNullOrWhiteSpace(inArg.InputFilename))
                throw new Exception("参数InputFilename为空值");
            if (!File.Exists(inArg.InputFilename))
                throw new Exception("参数InputFilename提供的文件不存在或者不可访问[" + inArg.InputFilename + "]");
        }

        private string ParseProjectionIdentify(string filename)
        {
            string[] projectionIdentifys = new string[] { "GLL", "LBT", "MCT", "OTG", "AEA", "PSG", "NOM", "NUL" };
            foreach (string projectionIdentify in projectionIdentifys)
            {
                if (filename.Contains(projectionIdentify))
                    return projectionIdentify;
            }
            return "NUL";
        }

        private BlockDef[] GenericBlock(BlockArg arg)
        {
            if (arg.BlockType == "identify")
            {
                return GenericBlockIdentify(arg.BlockIdentify);
            }
            else if (arg.BlockType == "custom")
            {
                List<BlockDef> blockList = new List<BlockDef>();
                foreach (PrjEnvelopeItem prjEnv in arg.BlockEnvelopes)
                {
                    PrjEnvelope env = prjEnv.PrjEnvelope;
                    blockList.Add(new BlockDef(prjEnv.Name, env.MinX, env.MinY, env.MaxX, env.MaxY));
                }
                return blockList.ToArray();
            }
            else
                return null;
        }

        /// <summary>
        /// 005 5度分幅
        /// 010 10度分幅
        /// </summary>
        /// <param name="blockIdentify"></param>
        /// <returns></returns>
        private BlockDef[] GenericBlockIdentify(string blockIdentify)
        {
            DefinedRegionParse p = new DefinedRegionParse();
            PrjEnvelopeItem[] ps = p.GetEnvelopeItems(blockIdentify);
            if (ps == null || ps.Length == 0)
                return null;
            List<BlockDef> l = new List<BlockDef>();
            foreach (PrjEnvelopeItem i in ps)
            { 
                l.Add(new BlockDef(i.Name,i.PrjEnvelope.MinX,i.PrjEnvelope.MinY,i.PrjEnvelope.MaxX,i.PrjEnvelope.MaxY));
            }
            return l.ToArray();
        }

        private bool ValidEnvelope(IRasterDataProvider inputRaster, PrjEnvelopeItem[] validEnvelopes, out string msg)
        {
            bool haValid = false;
            StringBuilder str = new StringBuilder();
            PrjEnvelope fileEnv = ProjectionFactory.GetEnvelope(inputRaster);
            foreach (PrjEnvelopeItem validEnvelope in validEnvelopes)
            {
                if (!validEnvelope.PrjEnvelope.IntersectsWith(fileEnv))
                    str.AppendLine("数据不在范围内：" + validEnvelope.Name + validEnvelope.PrjEnvelope.ToString());
                else
                    haValid = true;
            }
            msg = str.ToString();
            return haValid;
        }

        private string ParseStation(string filename)
        {
            string[] stations = new string[] { "MS" };
            foreach (string station in stations)
            {
                if (filename.Contains(station))
                    return station;
            }
            return null;
        }

        public void OnProgress(int progress, string text)
        {
            Console.WriteLine(progress + "," + text);
        }
    }
}
