using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.MIF.Core
{
    public class ArgumentProvider:IArgumentProvider,IDisposable
    {
        public const string ARGUMENT_NAME_INTERACTIVER = "CurrentRasterInteractiver";
        public const string ENV_VAR_PROVIDER = "EnvironmentVarProvider";
        protected IRasterDataProvider _dataProvider;
        protected Dictionary<string, object> _args = new Dictionary<string, object>();
        protected int[] _aoi;
        protected Feature[] _aois;
        protected bool _bandArgsIsSetted = false;
        protected AlgorithmDef _currentAlgorithmDef;

        public ArgumentProvider(AlgorithmDef currentAlgorithm)
        {
            _currentAlgorithmDef = currentAlgorithm;
        }

        public ArgumentProvider(Dictionary<string, object> args)
        {
            if (args != null && args.Count > 0)
            {
                foreach (string argName in args.Keys)
                    _args.Add(argName, args[argName]);
            }
        }

        public ArgumentProvider(IRasterDataProvider dataProvider,Dictionary<string ,object> args)
            :this(args)
        {
            _dataProvider = dataProvider;
        }

        public AlgorithmDef CurrentAlgorithmDef
        {
            get { return _currentAlgorithmDef; }
            set { _currentAlgorithmDef = value; }
        }

        public bool BandArgsIsSetted
        {
            get { return _bandArgsIsSetted; }
            internal set { _bandArgsIsSetted = value; }
        }

        public string[] ArgNames
        {
            get
            {
                return _args.Count > 0 ? _args.Keys.ToArray() : null;
            }
        }

        public ICurrentRasterInteractiver CurrentRasterInteractiver
        {
            get
            {
                if (_args.ContainsKey(ARGUMENT_NAME_INTERACTIVER))
                    return _args[ARGUMENT_NAME_INTERACTIVER] as ICurrentRasterInteractiver;
                return null ;
            }
        }

        public IEnvironmentVarProvider EnvironmentVarProvider
        {
            get 
            {
                if(_args.ContainsKey(ENV_VAR_PROVIDER))
                    return _args[ENV_VAR_PROVIDER] as IEnvironmentVarProvider;
                return null;
            }
        }

        public IRasterDataProvider DataProvider
        {
            get { return _dataProvider; }
            set { _dataProvider = value; }
        }

        public int[] AOI
        {
            get { return _aoi; }
            set { _aoi = value; }
        }

        public Feature[] AOIs
        {
            get { return _aois; }
            set { _aois = value; }
        }

        public ArgumentDef GetArgDef(string argName)
        {
            if (_currentAlgorithmDef == null || string.IsNullOrEmpty(argName))
                return null;
            return GetArgumentDef(argName, _currentAlgorithmDef.Arguments);
        }

        private ArgumentDef GetArgumentDef(string argName, ArgumentBase[] argumentBase)
        {
            ArgumentDef def = null;
            foreach (ArgumentBase df in argumentBase)
            {
                if (df is ArgumentDef && df.Name == argName)
                {
                    return df as ArgumentDef;
                }
                else if (df is ArgumentGroup)
                {
                    def = GetArgumentDef(argName, (df as ArgumentGroup).Arguments);
                    if (def != null)
                        return def;
                }
                else if (df is ArgumentPair)
                {
                    def = GetArgumentDef(argName, new ArgumentDef[] { (df as ArgumentPair).ArgumentMax, (df as ArgumentPair).ArgumentMin });
                    if (def != null)
                        return def;
                }
            }
            return def;
        }

        public object GetArg(string argName)
        {
            if(_args != null && !string.IsNullOrEmpty(argName) && _args.ContainsKey(argName))
                return _args[argName];
            return string.Empty;
        }

        public void SetArg(string argName,object argValue)
        {
            if (string.IsNullOrEmpty(argName))
                return;
            if (_args.ContainsKey(argName))
                _args[argName] = argValue;
            else
            {
                _args.Add(argName, argValue);
            }
        }

        public void Reset()
        {
            object v = null;
            if(_args.ContainsKey("FileNameGenerator"))
                v = _args["FileNameGenerator"];
            _args.Clear();
            if (v != null)
                _args.Add("FileNameGenerator", v);
            _bandArgsIsSetted = false;
        }

        public void Dispose()
        {
            _dataProvider = null;
            if (_args != null)
                _args.Clear();
        }
    }
}
