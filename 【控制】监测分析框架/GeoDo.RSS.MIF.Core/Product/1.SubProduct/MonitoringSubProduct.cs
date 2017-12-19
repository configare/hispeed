using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.IO;
using System.Drawing;
using GeoDo.RasterProject;

namespace GeoDo.RSS.MIF.Core
{
    public abstract class MonitoringSubProduct : IMonitoringSubProduct, IDisposable
    {
        protected string _name;
        protected string _identify;
        protected bool _isBinary;
        protected SubProductDef _subProductDef;
        protected List<AlgorithmDef> _algorithmDefs;
        protected IArgumentProvider _argumentProvider;
        protected string _error = null;
        protected object _tag = null;

        public MonitoringSubProduct()
        {
        }

        public MonitoringSubProduct(SubProductDef subProductDef)
        {
            _subProductDef = subProductDef;
            if (subProductDef == null)
                return;
            _name = subProductDef.Name;
            _identify = subProductDef.Identify;
            if (subProductDef.Algorithms == null || subProductDef.Algorithms.Length == 0)
                return;
            //通过默认算法设置参数提供者
            _algorithmDefs = new List<AlgorithmDef>(subProductDef.Algorithms);
            if (_algorithmDefs.Count > 0)
            {
                ExtractProductIdentify pid = new ExtractProductIdentify();
                pid.ThemeIdentify = subProductDef.ProductDef.Theme.Identify;
                pid.ProductIdentify = subProductDef.ProductDef.Identify;
                pid.SubProductIdentify = subProductDef.Identify;
                _argumentProvider = MonitoringThemeFactory.GetArgumentProvider(pid, _algorithmDefs[0].Identify);
                _argumentProvider.SetArg("AlgorithmName", _algorithmDefs[0].Identify);
            }

        }

        public string Name
        {
            get { return _name; }
        }

        public string Identify
        {
            get { return _identify; }
        }

        public bool IsBinary
        {
            get { return _isBinary; }
        }

        public SubProductDef Definition
        {
            get { return _subProductDef; }
        }

        public List<AlgorithmDef> AlgorithmDefs
        {
            get { return _algorithmDefs; }
        }

        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        /// <summary>
        /// 参数提供者中可以指定算法标识,指定本次产品生成用的算法
        /// </summary>
        /// <param name="argProvider"></param>
        /// <param name="outputer"></param>
        /// <returns></returns>
        public abstract IExtractResult Make(Action<int, string> progressTracker);

        public virtual IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            return Make(progressTracker);
        }

        /// <summary>
        /// Make后调用
        /// 二值图等产品后续产品加工
        /// </summary>
        /// <param name="piexd">Make(...)方法输出的多值图</param>
        /// <param name="progressTracker"></param>
        /// <returns></returns>
        public abstract IExtractResult MakeExtProduct(IPixelIndexMapper piexd, Action<int, string> progressTracker);

        public virtual bool ArgumentsIsOK()
        {
            return false;
        }

        public virtual void Reset()
        {
        }

        public bool CanDo
        {
            get
            {
                return ArgsIsFull();
            }
        }

        private bool ArgsIsFull()
        {
            if (_argumentProvider == null)
            {
                _error = "未提供[" + _subProductDef.Name + "]子产品参数提供者.";
                return false;
            }
            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                _error = "未提供[" + _subProductDef.Name + "]子产品所用算法名.";
                return false;
            }
            string algorithmName = _argumentProvider.GetArg("AlgorithmName").ToString();
            AlgorithmDef algorithmDef = _subProductDef.GetAlgorithmDefByIdentify(algorithmName);
            if (algorithmDef == null)
            {
                _error = "当前[" + _subProductDef.Name + "]子产品不包含\"" + algorithmName + "\"算法.";
                return false;
            }
            if (algorithmDef != null)
            {
                object bandArg = null;
                foreach (BandDef arg in algorithmDef.Bands)
                {
                    bandArg = _argumentProvider.GetArg(arg.Identify);
                    if (bandArg == null)
                    {
                        _error = "未提供波段[" + arg.Identify + "]信息.";
                        return false;
                    }
                    if (bandArg.ToString() == "-1")
                    {
                        _error = "波段[" + arg.Identify + "]未找到对应的波段映射.";
                        return false;
                    }
                }
            }
            ArgumentPair argPair = null;
            ArgumentDef argDef = null;
            foreach (ArgumentBase arg in algorithmDef.Arguments)
            {
                if (arg is ArgumentPair)
                {
                    argPair = arg as ArgumentPair;
                    if (argPair.ArgumentMax != null && _argumentProvider.GetArg(argPair.ArgumentMax.Name) == null)
                    {
                        _error = "参数[" + argPair.ArgumentMax.Name + "]未提供.";
                        return false;
                    }
                    if (argPair.ArgumentMin != null && _argumentProvider.GetArg(argPair.ArgumentMin.Name) == null)
                    {
                        _error = "参数[" + argPair.ArgumentMin.Name + "]未提供.";
                        return false;
                    }
                }
                else if (arg is ArgumentDef)
                {
                    argDef = arg as ArgumentDef;
                    if (_argumentProvider.GetArg(argDef.Name) == null)
                    {
                        _error = "参数[" + argDef.Name + "]未提供.";
                        return false;
                    }

                    if (argDef.RefType == "file")
                    {
                        string filename = _argumentProvider.GetArg(argDef.Name).ToString();
                        if (!File.Exists(filename))
                        {
                            _error = "参数文件[" + Path.GetFileName(filename) + "]不存在.";
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public IArgumentProvider ArgumentProvider
        {
            get { return _argumentProvider; }
        }

        public void ResetArgumentProvider(string algIdentify)
        {
            ExtractProductIdentify pid = new ExtractProductIdentify();
            pid.ThemeIdentify = _subProductDef.ProductDef.Theme.Identify;
            pid.ProductIdentify = _subProductDef.ProductDef.Identify;
            pid.SubProductIdentify = _subProductDef.Identify;
            Dictionary<string, object> algShare = ReadAlgShareArg();
            IArgumentProvider prd = MonitoringThemeFactory.GetArgumentProvider(pid, algIdentify);
            if (prd != null)
            {
                string[] argNames = prd.ArgNames;
                if (argNames != null)
                    foreach (string name in argNames)
                        _argumentProvider.SetArg(name, prd.GetArg(name));
            }
            CopyAlgShareArg(algShare);
            UpdateCurrentAlgorithmDef(algIdentify);
        }

        public void ResetArgumentProvider(string satellite, string sensor, params string[] args)
        {
            if (_argumentProvider == null)
                _argumentProvider = GetDefaultArgProvider(satellite, sensor, args);
            else
            {
                Dictionary<string, object> algShare = ReadAlgShareArg();
                _argumentProvider.Reset();
                IArgumentProvider prd = GetDefaultArgProvider(satellite, sensor, args);
                if (prd != null)
                {
                    string[] argNames = prd.ArgNames;
                    if (argNames != null)
                        foreach (string name in argNames)
                            _argumentProvider.SetArg(name, prd.GetArg(name));
                }
                CopyAlgShareArg(algShare);
            }
        }

        public void ResetArgumentProvider(string algIdentify, string satellite, string sensor, params string[] args)
        {
            if (_argumentProvider == null)
                _argumentProvider = GetDefaultArgProvider(algIdentify, satellite, sensor, args);
            else
            {
                Dictionary<string, object> algShare = ReadAlgShareArg();
                _argumentProvider.Reset();
                IArgumentProvider prd = GetDefaultArgProvider(algIdentify, satellite, sensor, args);
                if (prd != null)
                {
                    string[] argNames = prd.ArgNames;
                    if (argNames != null)
                    {
                        foreach (string name in argNames)
                            _argumentProvider.SetArg(name, prd.GetArg(name));
                    }
                }
                CopyAlgShareArg(algShare);
                UpdateCurrentAlgorithmDef(algIdentify);
            }
        }

        private void UpdateCurrentAlgorithmDef(string algIdentify)
        {
            AlgorithmDef alg = _subProductDef.GetAlgorithmDefByIdentify(algIdentify);
            _argumentProvider.CurrentAlgorithmDef = alg;
        }

        private Dictionary<string, object> ReadAlgShareArg()
        {
            //by chennan 无共享参数情况错误
            if (_argumentProvider.ArgNames == null || _argumentProvider.ArgNames.Length == 0)
                return null;
            Dictionary<string, object> algShare = new Dictionary<string, object>();
            foreach (string argName in _argumentProvider.ArgNames)
            {
                ArgumentBase argDef = _argumentProvider.GetArgDef(argName);
                if (argDef != null && argDef.IsAlgorithmShare)
                    algShare.Add(argName, _argumentProvider.GetArg(argName));
            }
            return algShare;
        }

        private void CopyAlgShareArg(Dictionary<string, object> algShare)
        {
            //by chennan 无共享参数情况错误
            if (algShare == null || algShare.Count == 0)
                return;
            foreach (string key in algShare.Keys)
            {
                _argumentProvider.SetArg(key, algShare[key]);
            }
        }

        private IArgumentProvider GetDefaultArgProvider(string algIdentify, string satellite, string sensor, string[] args)
        {
            ExtractProductIdentify id1 = new ExtractProductIdentify();
            id1.ProductIdentify = _subProductDef.ProductDef.Identify;
            id1.SubProductIdentify = _subProductDef.Identify;
            id1.ThemeIdentify = _subProductDef.ProductDef.Theme.Identify;
            return MonitoringThemeFactory.GetArgumentProvider(id1, algIdentify, satellite, sensor);
        }

        private IArgumentProvider GetDefaultArgProvider(string satellite, string sensor, string[] args)
        {
            ExtractProductIdentify id1 = new ExtractProductIdentify();
            id1.ProductIdentify = _subProductDef.ProductDef.Identify;
            id1.SubProductIdentify = _subProductDef.Identify;
            id1.ThemeIdentify = _subProductDef.ProductDef.Theme.Identify;
            //
            ExtractAlgorithmIdentify id2 = new ExtractAlgorithmIdentify();
            id2.Satellite = satellite;
            id2.Sensor = sensor;
            id2.CustomIdentify = (args != null && args.Length > 0) ? args[0] : null;
            //
            return MonitoringThemeFactory.GetArgumentProvider(id1, id2);
        }

        public virtual void SetExtHeader(IExtHeaderSetter setter, object header)
        {
        }

        public AlgorithmDef UseDefaultAlgorithm(string productIdentify)
        {
            if (_algorithmDefs == null || _algorithmDefs.Count == 0)
                return null;
            ExtractProductIdentify pid = new ExtractProductIdentify();
            pid.ProductIdentify = productIdentify;
            pid.SubProductIdentify = _identify;
            pid.ThemeIdentify = _subProductDef.ProductDef.Theme.Identify;
            _argumentProvider = MonitoringThemeFactory.GetArgumentProvider(pid, _algorithmDefs[0].Identify);
            return _algorithmDefs[0];
        }

        public string[] GetStringArray(string arugmentName)
        {
            object obj = _argumentProvider.GetArg(arugmentName);
            if (obj == null)
                return null;
            string[] resultArray = obj as string[];
            if (arugmentName == "SelectedPrimaryFiles" && resultArray != null)
                return resultArray;
            //
            if (arugmentName == "mainfiles" && resultArray != null)
                return resultArray;
            else if (resultArray != null && obj.ToString() == "System.String[]")
                return resultArray;
            else
            {
                //如果obj为数组，则obj.tostring() = System.String[];
                string[] tempSplit = obj.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (tempSplit == null || tempSplit.Length == 0)
                    return null;
                List<string> result = new List<string>();
                for (int i = 0; i < tempSplit.Length; i++)
                {
                    if (File.Exists(tempSplit[i]))
                        result.Add(tempSplit[i]);
                }
                return result.Count == 0 ? null : result.ToArray();
            }
        }

        public void Dispose()
        {
            if (_algorithmDefs != null)
            {
                _algorithmDefs.Clear();
                _algorithmDefs = null;
            }
            _subProductDef = null;
            _tag = null;
        }
    }
}