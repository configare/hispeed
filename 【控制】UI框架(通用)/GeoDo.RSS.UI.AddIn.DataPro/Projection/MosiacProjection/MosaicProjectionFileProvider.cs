using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GeoDo.FileProject;
using GeoDo.RasterProject;
using GeoDo.RSS.BlockOper;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using System.Drawing;
using System.Drawing.Imaging;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    public class MosaicProjectionFileProvider : IDisposable
    {
        private DataIdentify _dataIdentify;
        private List<MosaicProjectItem> _fileItems = new List<MosaicProjectItem>();
        private int[] _overviewBands = null;

        public event Action<DataIdentify, IRasterDataProvider> DataIdentifyChanged;

        public MosaicProjectionFileProvider()
        {
        }

        public int[] OverviewBands
        {
            get { return _overviewBands; }
            set { _overviewBands = value; }
        }

        public PrjEnvelope ExtendEnvelope
        {
            get
            {
                if (_fileItems == null || _fileItems.Count == 0)
                    return null;
                PrjEnvelope env = null;
                for (int i = 0; i < _fileItems.Count; i++)
                {
                    env = PrjEnvelope.Union(env, _fileItems[i].Envelope);
                }
                return env;
            }
        }

        public MosaicProjectItem[] FileItems
        {
            get { return _fileItems.ToArray(); }
        }

        public IRasterDataProvider[] DoProject(PrjOutArg prjArgs, MosaicType mosaicType, Action<int, string> progressCallback, out string msg)
        {
            ProjectionFactory proj = new ProjectionFactory();
            msg = "";
            TryCreateOutDir(prjArgs.OutDirOrFile);
            StringBuilder str = new StringBuilder();
            List<string[]> projectedFiles = new List<string[]>();
            for (int fileIndex = 0; fileIndex < _fileItems.Count; fileIndex++)
            {
                string errMessage;
                MosaicProjectItem item = _fileItems[fileIndex];
                if (item == null || item.MainFile == null)
                    continue;
                string file = item.MainFile.fileName;
                string outfilename = "";
                if (mosaicType != MosaicType.NoMosaic &&
                    !IsDir(prjArgs.OutDirOrFile) && _fileItems.Count > 1)
                {
                    outfilename = prjArgs.OutDirOrFile;
                    prjArgs.OutDirOrFile = Path.GetDirectoryName(outfilename) +"\\"+ Guid.NewGuid().ToString() + Path.GetExtension(outfilename);
                }
                string[] files = proj.Project(file, prjArgs, progressCallback, out errMessage);
                if(!string.IsNullOrWhiteSpace(outfilename))
                    prjArgs.OutDirOrFile = outfilename;
                if (!string.IsNullOrWhiteSpace(errMessage))
                    str.AppendLine(Path.GetFileName(file) + errMessage);
                if (files == null || files.Length == 0)
                    continue;
                if (files.Length == 1 && files[0] == null)
                    continue;
                projectedFiles.Add(files);
            }
            msg = str.ToString();
            if (projectedFiles.Count == 0)
                return null;
            if (projectedFiles.Count == 1 && projectedFiles[0] == null)
                return null;
            int envCount = prjArgs.Envelopes == null ? 1 : prjArgs.Envelopes.Length;
            IRasterDataProvider[] retEnvFiles = new IRasterDataProvider[envCount];
            List<IRasterDataProvider> retEnvFilesNoMosaic = new List<IRasterDataProvider>();
            for (int i = 0; i < envCount; i++)
            {
                List<string> envFiles = new List<string>();
                for (int j = 0; j < _fileItems.Count; j++)
                {
                    if (j >= projectedFiles.Count)
                        break;
                    if (projectedFiles[j] != null && projectedFiles[j].Length > i)
                    {
                        if (projectedFiles[j] == null || projectedFiles[j].Length == 0 || string.IsNullOrWhiteSpace(projectedFiles[j][i]))
                            continue;
                        envFiles.Add(projectedFiles[j][i]);
                    }
                }
                //by chennan 批量分幅投影
                if (mosaicType != MosaicType.NoMosaic)
                {
                    if (envFiles.Count > 1)
                    {
                        List<IRasterDataProvider> mosaicFiles = new List<IRasterDataProvider>();
                        try
                        {
                            foreach (string envfile in envFiles)
                            {
                                IRasterDataProvider file = FileHelper.Open(envfile);
                                mosaicFiles.Add(file);
                            }
                            string mosaicFile = null;
                            if(prjArgs == null || string.IsNullOrWhiteSpace(prjArgs.OutDirOrFile))
                            {
                                string outDir =  Path.GetDirectoryName(mosaicFiles[0].fileName);
                                mosaicFile = Path.Combine(outDir, Path.GetFileNameWithoutExtension(mosaicFiles[0].fileName) + "_MOSAIC.ldf");
                            }
                            else
                            {
                                if (IsDir(prjArgs.OutDirOrFile))
                                    mosaicFile = Path.Combine(prjArgs.OutDirOrFile, Path.GetFileNameWithoutExtension(mosaicFiles[0].fileName) + "_MOSAIC.ldf");
                                else
                                    mosaicFile = prjArgs.OutDirOrFile;
                            }
                            RasterMoasicProcesser processer = new RasterMoasicProcesser();
                            IRasterDataProvider drcDataProvider = processer.Moasic(mosaicFiles.ToArray(), "LDF", mosaicFile, true, new string[] { "0" }, progressCallback);
                            //拼接角度文件
                            if (prjArgs.Args != null && prjArgs.Args.Length > 0)
                            {
                                foreach (object arg in prjArgs.Args)
                                {
                                    if (arg is string)
                                    {
                                        bool isMoasic = true;
                                        string[] extentStr = null;
                                        List<IRasterDataProvider> mosaicExtFile = new List<IRasterDataProvider>();
                                        List<string> extFileList = new List<string>();
                                        try
                                        {
                                            string extFileName;
                                            string extension;
                                            string argStr = arg as string;
                                            if (argStr.Contains("="))
                                            {
                                                argStr = argStr.Split('=')[1];
                                                if (argStr.Contains(";"))
                                                    extentStr = argStr.Split(';');
                                            }
                                            if (extentStr != null)
                                            {
                                                for (int extentIndex = 0; extentIndex < extentStr.Length; extentIndex++)
                                                {
                                                    argStr = extentStr[extentIndex];
                                                    extension = "." + argStr + ".ldf";
                                                    for (int j = 0; j < envFiles.Count; j++)
                                                    {
                                                        extFileName = Path.ChangeExtension(envFiles[j], extension);
                                                        if (!File.Exists(extFileName))
                                                        {
                                                            isMoasic = false;
                                                            break;
                                                        }
                                                        else
                                                            isMoasic = true;
                                                        IRasterDataProvider file = FileHelper.Open(extFileName);
                                                        mosaicExtFile.Add(file);
                                                        extFileList.Add(extFileName);
                                                    }
                                                    if (isMoasic)
                                                    {
                                                        string outFileName = Path.ChangeExtension(mosaicFile, extension);
                                                        IRasterDataProvider drcExtDataProvider = processer.Moasic(mosaicExtFile.ToArray(), "LDF", outFileName, true, new string[] { "0" }, progressCallback);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                extension = "." + argStr + ".ldf";
                                                for (int j = 0; j < envFiles.Count; j++)
                                                {
                                                    extFileName = Path.ChangeExtension(envFiles[j], extension);
                                                    if (!File.Exists(extFileName))
                                                    {
                                                        isMoasic = false;
                                                        break;
                                                    }
                                                    IRasterDataProvider file = FileHelper.Open(extFileName);
                                                    mosaicExtFile.Add(file);
                                                    extFileList.Add(extFileName);
                                                }
                                                if (isMoasic)
                                                {
                                                    string outFileName = Path.ChangeExtension(mosaicFile, extension);
                                                    IRasterDataProvider drcExtDataProvider = processer.Moasic(mosaicExtFile.ToArray(), "LDF", outFileName, true, new string[] { "0" }, progressCallback);
                                                }
                                            }
                                        }
                                        finally
                                        {
                                            if (mosaicExtFile.Count > 0)
                                            {
                                                foreach (IRasterDataProvider file in mosaicExtFile)
                                                {
                                                    if (file != null)
                                                        file.Dispose();
                                                }
                                                mosaicExtFile.Clear();
                                            }
                                            if (extFileList.Count > 0)
                                            {
                                                foreach (string envfile in extFileList)
                                                {
                                                    TryDeleteRasterFile(envfile);
                                                }
                                                extFileList.Clear();
                                            }
                                        }
                                    }  
                                }
                            }
                            retEnvFiles[i] = drcDataProvider;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        finally
                        {
                            foreach (IRasterDataProvider envfile in mosaicFiles)
                            {
                                if (envfile != null)
                                    envfile.Dispose();
                            }
                            mosaicFiles.Clear();
                            foreach (string envfile in envFiles)
                            {
                                TryDeleteRasterFile(envfile);
                            }
                        }
                    }
                    else if (envFiles.Count == 1)
                    {
                        if (!IsDir(prjArgs.OutDirOrFile)&&!string.IsNullOrWhiteSpace(prjArgs.OutDirOrFile) && envFiles[0] != prjArgs.OutDirOrFile)
                        {
                            if (!Directory.Exists(Path.GetDirectoryName(prjArgs.OutDirOrFile)))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(prjArgs.OutDirOrFile));
                            }
                            File.Copy(envFiles[0], prjArgs.OutDirOrFile);
                            TryDeleteRasterFile(envFiles[0]);
                            retEnvFiles[i] = FileHelper.Open(prjArgs.OutDirOrFile);
                        }
                        else
                            retEnvFiles[i] = FileHelper.Open(envFiles[0]);
                    }
                    else
                        retEnvFiles[i] = null;
                    //
                }
                else if (envFiles.Count != 0)
                {
                    List<IRasterDataProvider> mosaicFiles = new List<IRasterDataProvider>();
                    foreach (string envfile in envFiles)
                    {
                        IRasterDataProvider file = FileHelper.Open(envfile);
                        mosaicFiles.Add(file);
                    }
                    retEnvFilesNoMosaic.AddRange(mosaicFiles);
                }
            }
            //
            return retEnvFilesNoMosaic.Count == 0 ? retEnvFiles : retEnvFilesNoMosaic.ToArray();
        }

        private void TryCreateOutDir(string outDirOrFile)
        {
            if (IsDir(outDirOrFile))
            {
                if (!Directory.Exists(outDirOrFile))
                {
                    Directory.CreateDirectory(outDirOrFile);
                }
            }
            else
            {
                if (!Directory.Exists(Path.GetDirectoryName(outDirOrFile)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(outDirOrFile));
                }
            }
        }

        private bool IsDir(string path)
        {
            if (Directory.Exists(path))
                return true;
            else if (File.Exists(path))
                return false;
            else if (Path.HasExtension(path))
                return false;
            else
                return true;
        }

        private void TryDeleteRasterFile(string filename)
        {
            string hdrFile = Path.ChangeExtension(filename, ".hdr");
            TryDeleteFile(filename);
            TryDeleteFile(hdrFile);
        }

        private bool TryDeleteFile(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                return false;
            try
            {
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }
                return true;
            }
            catch
            {
                return false; 
            }
        }

        public IRasterDataProvider Add(string file,out string errorMsg)
        {
            errorMsg = "";
            IRasterDataProvider prd = null;
            try
            {
                prd = GeoDataDriver.Open(file) as IRasterDataProvider;
                if (prd != null)
                {
                    IRasterDataProvider newprd = ProjectionFactory.CheckPrjArg(prd);
                    if (newprd != prd)
                        prd.Dispose();
                    if (Add(newprd, out errorMsg))
                        return newprd;
                }
                if (prd != null)
                    prd.Dispose();
                return null;
            }
            catch
            {
                if (prd != null)
                    prd.Dispose();
                throw;
            }
        }

        public bool Add(IRasterDataProvider file, out string errorMsg)
        {
            errorMsg = "";
            if (!Contains(file))
            {
                DataIdentify identify = file.DataIdentify;
                if (_dataIdentify == null)
                {
                    _dataIdentify = identify;
                    MosaicProjectItem item = new MosaicProjectItem(file);
                    if (!string.IsNullOrWhiteSpace(item.ErrorMsg))
                    {
                        errorMsg = item.ErrorMsg;
                        return false;
                    }
                    _fileItems.Add(item);
                    if (DataIdentifyChanged != null)
                    {
                        DataIdentifyChanged(_dataIdentify, file);
                    }
                    return true;
                }
                else if (_dataIdentify.IsOrbit)
                {
                    if (_dataIdentify.Sensor != identify.Sensor || _dataIdentify.Satellite != identify.Satellite
                        || (_dataIdentify.OrbitDateTime == identify.OrbitDateTime && identify.OrbitDateTime != DateTime.MinValue))
                    {
                        return false;
                    }
                    MosaicProjectItem item = new MosaicProjectItem(file);
                    _fileItems.Add(item);
                    return true;
                }
            }
            return false;
        }

        private bool Contains(IRasterDataProvider file)
        {
            if (_fileItems == null || _fileItems.Count == 0)
                return false;
            foreach (MosaicProjectItem item in _fileItems)
            {
                IRasterDataProvider prd = item.MainFile;
                if (prd.fileName == file.fileName)
                    return true;
            }
            return false;
        }

        public bool Remove(IRasterDataProvider file)
        {
            if (_fileItems != null)
            {
                int i = _fileItems.RemoveAll(new Predicate<MosaicProjectItem>(item => { return (item.MainFile == file); }));
                if (_fileItems.Count == 0)
                {
                    _fileItems.Clear();
                    _dataIdentify = null;
                }
                return i != 0;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            if (_fileItems != null)
            {
                _fileItems.RemoveAt(index);
                if (_fileItems.Count == 0)
                {
                    _fileItems.Clear();
                    _dataIdentify = null;
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < _fileItems.Count; i++)
            {
                _fileItems[i].Dispose();
                _fileItems[i] = null;
            }
            _fileItems.Clear();
            _dataIdentify = null;
        }

        internal void MoveDown(int index)
        {
            if (index < 0 || index >= _fileItems.Count)
                return;
            if (index == _fileItems.Count - 1)
                return;
            MosaicProjectItem item = _fileItems[index];
            _fileItems.RemoveAt(index);
            _fileItems.Insert(index + 1, item);
        }

        internal void MoveUp(int index)
        {
            if (index < 0 || index >= _fileItems.Count)
                return;
            if (index == 0)
                return;
            MosaicProjectItem item = _fileItems[index];
            _fileItems.RemoveAt(index);
            _fileItems.Insert(index - 1, item);
        }

        public void Dispose()
        {
            Clear();
        }
    }
}
