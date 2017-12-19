using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.BlockOper;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.UI;
using System.IO;
using GeoDo.RSS.DF.MEM;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    public class RasterClip
    {
        public string[] RasterClipT(string infName, BlockDef[] blocks, IProgressMonitor progress, string type)
        {
            string outDir = Path.GetDirectoryName(infName);
            return RasterClipT(infName, blocks, outDir, progress, type);
        }

        public string[] RasterClipT(string infName, BlockDef[] blocks, string outDir, IProgressMonitor progress, string type)
        {
            IRasterDataProvider prd = GeoDataDriver.Open(infName) as IRasterDataProvider;

            try
            {
                if (progress != null)
                {
                    progress.Reset("", 100);
                    progress.Start(false);
                }
                if (type.Equals("Clip"))
                {
                    RasterClipProcesser clip = new RasterClipProcesser();
                    IRasterDataProvider[] outs = null;
                    if (prd is IMemoryRasterDataProvider)
                        outs = clip.Clip(prd, blocks, 100, "MEM", outDir,
                            new Action<int, string>((int progerss, string text) =>
                            {
                                if (progress != null)
                                    progress.Boost(progerss, text);
                            }));
                    else
                        outs = clip.Clip(prd, blocks, 100, "LDF", outDir,
                       new Action<int, string>((int progerss, string text) =>
                       {
                           if (progress != null)
                               progress.Boost(progerss, text);
                       }));
                    List<string> files = new List<string>();
                    for (int i = 0; i < outs.Length; i++)
                    {
                        if (outs[i] != null)
                        {
                            files.Add(outs[i].fileName);
                            outs[i].Dispose();
                        }
                    }
                    return files.ToArray();
                }
                else if (type.Equals("Cut"))
                {
                    RasterCutProcesser cut = new RasterCutProcesser();
                    IRasterDataProvider result = null;
                    if (prd is IMemoryRasterDataProvider)
                        result = cut.Cut(prd, blocks[0] as BlockDefWithAOI, 100, "MEM", outDir,
                        new Action<int, string>((int progerss, string text) =>
                        {
                            if (progress != null)
                                progress.Boost(progerss, text);
                        }));
                    else
                        result = cut.Cut(prd, blocks[0] as BlockDefWithAOI, 100, "LDF", outDir,
                        new Action<int, string>((int progerss, string text) =>
                        {
                            if (progress != null)
                                progress.Boost(progerss, text);
                        }));
                    List<string> files = new List<string>();
                    if (result != null)
                    {
                        files.Add(result.fileName);
                    }
                    return files.ToArray();
                }
                else
                    return null;

            }
            finally
            {
                if (progress != null)
                    progress.Finish();
            }
        }

        public string[] RasterClipT(string infName, BlockDefWithAOI[] blocks, string outDir, IProgressMonitor progress, string type)
        {
            IRasterDataProvider prd = GeoDataDriver.Open(infName) as IRasterDataProvider;

            try
            {
                if (progress != null)
                {
                    progress.Reset("", 100);
                    progress.Start(false);
                }
                if (blocks[0].AOIIndexes == null || blocks[0].AOIIndexes.Count() == 0)
                    type = "Clip";
                if (type.Equals("Clip"))
                {
                    RasterClipProcesser clip = new RasterClipProcesser();
                    IRasterDataProvider[] outs = null;
                    if (prd is MemoryRasterDataProvider)
                        outs = clip.Clip(prd, blocks, 100, "MEM", outDir,
                              new Action<int, string>((int progerss, string text) =>
                              {
                                  if (progress != null)
                                      progress.Boost(progerss, text);
                              }));
                    else
                        outs = clip.Clip(prd, blocks, 100, "LDF", outDir,
                            new Action<int, string>((int progerss, string text) =>
                            {
                                if (progress != null)
                                    progress.Boost(progerss, text);
                            }));
                    List<string> files = new List<string>();
                    for (int i = 0; i < outs.Length; i++)
                    {
                        if (outs[i] != null)
                        {
                            files.Add(outs[i].fileName);
                            outs[i].Dispose();
                        }
                    }
                    return files.ToArray();
                }
                else if (type.Equals("Cut"))
                {
                    RasterCutProcesser cut = new RasterCutProcesser();
                    IRasterDataProvider result = null;
                    if (prd is MemoryRasterDataProvider)
                        result = cut.Cut(prd, blocks[0], 100, "MEM", outDir,
                        new Action<int, string>((int progerss, string text) =>
                        {
                            if (progress != null)
                                progress.Boost(progerss, text);
                        }));
                    else
                        result = cut.Cut(prd, blocks[0], 100, "LDF", outDir,
                       new Action<int, string>((int progerss, string text) =>
                       {
                           if (progress != null)
                               progress.Boost(progerss, text);
                       }));
                    List<string> files = new List<string>();
                    if (result != null)
                    {
                        files.Add(result.fileName);
                        result.Dispose();
                    }
                    return files.ToArray();
                }
                else
                    return null;

            }
            finally
            {
                if (progress != null)
                    progress.Finish();
            }
        }
    }
}
