using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Core
{
    public static class MonitoringThemeFactory
    {
        private static ThemeDef[] _themes;

        static MonitoringThemeFactory()
        {
            _themes = (new MonitoringThemesParser()).Parse();
        }

        public static void MergerTheme(string fname)
        {
            ThemeDef[] ths  = (new MonitoringThemesParser(fname)).Parse();
            if (ths != null)
            {
                if (_themes == null)
                    _themes = ths;
                else
                {
                    ThemeDef[] newThemes = new ThemeDef[_themes.Length + ths.Length];
                    Array.Copy(_themes, newThemes, _themes.Length);
                    Array.Copy(ths, 0, newThemes, _themes.Length, ths.Length);
                    _themes = newThemes;
                }
            }
        }

        public static ThemeDef[] GetAllThemes()
        {
            return _themes;
        }

        public static ThemeDef GetThemeDefByIdentify(string identify)
        {
            if (_themes == null || _themes.Length == 0 || identify == null)
                return null;
            foreach (ThemeDef t in _themes)
                if (t.Identify != null && t.Identify.ToUpper() == identify.ToUpper())
                    return t;
            return null;
        }

        public static ProductDef GetProductDef(string themeIdentify, string productIdentify)
        {
            ThemeDef theme = GetThemeDefByIdentify(themeIdentify);
            if (theme == null)
                return null;
            return theme.GetProductDefByIdentify(productIdentify);
        }

        public static IArgumentProvider GetArgumentProvider(ExtractProductIdentify productIdentify, string algorithmIdentify)
        {
            if (productIdentify == null || algorithmIdentify == null)
                return null;
            ThemeDef theme = GetThemeDefByIdentify(productIdentify.ThemeIdentify);
            if (theme == null)
                return null;
            ProductDef prd = theme.GetProductDefByIdentify(productIdentify.ProductIdentify);
            if (prd == null)
                return null;
            SubProductDef subPrd = prd.GetSubProductDefByIdentify(productIdentify.SubProductIdentify);
            if (subPrd == null)
                return null;
            AlgorithmDef alg = subPrd.GetAlgorithmDefByIdentify(algorithmIdentify);
            IArgumentProvider argPrd = CreateArgumentProvider(alg, null);
            if (alg.Bands != null)
                foreach (BandDef b in alg.Bands)
                    argPrd.SetArg(b.Identify, b.BandNo);
            return argPrd;
        }

        public static IArgumentProvider GetArgumentProvider(ExtractProductIdentify productIdentify, string algorithmIdentify, string satellite, string sensor)
        {
            if (productIdentify == null || algorithmIdentify == null)
                return null;
            ThemeDef theme = GetThemeDefByIdentify(productIdentify.ThemeIdentify);
            if (theme == null)
                return null;
            ProductDef prd = theme.GetProductDefByIdentify(productIdentify.ProductIdentify);
            if (prd == null)
                return null;
            SubProductDef subPrd = prd.GetSubProductDefByIdentify(productIdentify.SubProductIdentify);
            if (subPrd == null)
                return null;
            AlgorithmDef alg = subPrd.GetAlgorithmDefByIdentify(algorithmIdentify);
            return CreateArgumentProvider(alg, BandRefTableHelper.GetBandRefTable(satellite, sensor));
        }

        public static IArgumentProvider GetArgumentProvider(ExtractProductIdentify productIdentify, ExtractAlgorithmIdentify algorithmIdentify)
        {
            if (productIdentify == null || algorithmIdentify == null)
                return null;
            ThemeDef theme = GetThemeDefByIdentify(productIdentify.ThemeIdentify);
            if (theme == null)
                return null;
            ProductDef prd = theme.GetProductDefByIdentify(productIdentify.ProductIdentify);
            if (prd == null)
                return null;
            SubProductDef subPrd = prd.GetSubProductDefByIdentify(productIdentify.SubProductIdentify);
            if (subPrd == null)
                return null;
            AlgorithmDef alg = subPrd.GetAlgorithmDefByAlgorithmIdentify(algorithmIdentify);
            IArgumentProvider argprd = CreateArgumentProvider(alg, BandRefTableHelper.GetBandRefTable(algorithmIdentify.Satellite, algorithmIdentify.Sensor));
            if (argprd != null)
                argprd.SetArg("AlgorithmName", alg.Identify);
            return argprd;
        }

        private static IArgumentProvider CreateArgumentProvider(AlgorithmDef alg, BandnameRefTable bandRefTable)
        {
            if (alg == null)
                return null;
            IArgumentProvider prd = new ArgumentProvider(alg);
            if (bandRefTable != null)
                SetBands(prd, alg.Bands, bandRefTable);
            SetArguments(prd, alg.Arguments);
            return prd;
        }

        public static void SetBandArgs(IMonitoringSubProduct subProduct, string satellite, string sensor, params object[] args)
        {
            if (subProduct.ArgumentProvider == null || subProduct.ArgumentProvider.CurrentAlgorithmDef == null)
                return;
            BandnameRefTable bandRefTable = BandRefTableHelper.GetBandRefTable(satellite, sensor);
            bool isOK = false;
            if (bandRefTable != null)
            {
                isOK = SetBands(subProduct.ArgumentProvider, subProduct.ArgumentProvider.CurrentAlgorithmDef.Bands, bandRefTable);
                (subProduct.ArgumentProvider as ArgumentProvider).BandArgsIsSetted = true;
            }
            if (!isOK)
            {
                isOK = SetBands(subProduct.ArgumentProvider, subProduct.ArgumentProvider.CurrentAlgorithmDef.Bands);
                (subProduct.ArgumentProvider as ArgumentProvider).BandArgsIsSetted = isOK;
            }
        }

        public static void FillDefaultArguments(IArgumentProvider argumentProvider, AlgorithmDef alg, string satellite, string sensor)
        {
            argumentProvider.Reset();
            SetBands(argumentProvider, alg.Bands, BandRefTableHelper.GetBandRefTable(satellite, sensor));
            SetArguments(argumentProvider, alg.Arguments);
        }

        private static void SetArguments(IArgumentProvider prd, ArgumentBase[] argumentDefs)
        {
            if (argumentDefs == null || argumentDefs.Length == 0)
                return;
            ArgumentDef argmin;
            ArgumentDef argmax;
            foreach (ArgumentBase arg in argumentDefs)
            {
                if (arg is ArgumentGroup)
                {
                    ArgumentBase[] args = (arg as ArgumentGroup).Arguments;
                    if (args == null || args.Length == 0)
                        continue;
                    SetArguments(prd, args);
                    //foreach (ArgumentDef def in defs)
                    //    SetArgToArgumentProvider(prd, def);
                }
                else if (arg is ArgumentPair)
                {
                    argmin = (arg as ArgumentPair).ArgumentMin;
                    argmax = (arg as ArgumentPair).ArgumentMax;
                    SetArgToArgumentProvider(prd, argmin);
                    SetArgToArgumentProvider(prd, argmax);
                }
                else if (arg is ArgumentDef)
                {
                    if ((arg as ArgumentDef).RefType == "file")
                        SetArgRefFileToArgumentProvider(prd, arg as ArgumentDef);
                    else
                        SetArgToArgumentProvider(prd, arg as ArgumentDef);
                }
            }
        }

        private static void SetArgRefFileToArgumentProvider(IArgumentProvider prd, ArgumentDef arg)
        {
            arg.Datatype = "string";
            object v = GetDefaultValue(arg);
            prd.SetArg(arg.Name, v);
        }

        private static void SetArgToArgumentProvider(IArgumentProvider prd, ArgumentDef arg)
        {
            if (arg == null)
                return;
            object v = GetDefaultValue(arg);
            prd.SetArg(arg.Name, v);
        }

        private static object GetDefaultValue(ArgumentDef arg)
        {
            return GetActualValue(arg.Datatype, arg.Defaultvalue);
        }

        private static object GetActualValue(string type, string value)
        {
            if (type == null)
                return null;
            type = type.ToUpper();
            switch (type)
            {
                case "BOOL":
                    return string.IsNullOrWhiteSpace(value) ? false : bool.Parse(value);
                case "BYTE":
                    return string.IsNullOrWhiteSpace(value) ? (byte)0 : byte.Parse(value);
                case "UINT16":
                    return string.IsNullOrWhiteSpace(value) ? (ushort)0 : UInt16.Parse(value);
                case "INT16":
                    return string.IsNullOrWhiteSpace(value) ? (short)0 : Int16.Parse(value);
                case "INT32":
                    return string.IsNullOrWhiteSpace(value) ? (int)0 : Int32.Parse(value);
                case "UINT32":
                    return string.IsNullOrWhiteSpace(value) ? (uint)0 : UInt32.Parse(value);
                case "DOUBLE":
                    return string.IsNullOrWhiteSpace(value) ? 0d : Double.Parse(value);
                case "SINGLE":
                case "FLOAT":
                    return string.IsNullOrWhiteSpace(value) ? 0f : float.Parse(value);
                case "STRING":
                    return value;
                case "CHAR":
                    if (value.Length > 0)
                        return value.ToCharArray()[0];
                    break;
                case "OBJECT":
                default:
                    return value;
            }
            return null;
        }

        private static bool SetBands(IArgumentProvider prd, BandDef[] bandDefs, BandnameRefTable bandRefTable)
        {
            if (prd == null || bandDefs == null || bandDefs.Length == 0 || bandRefTable == null)
                return false;
            bool isOK = false;
            int bandNo;
            foreach (BandDef def in bandDefs)
            {
                bandNo = 0;
                //按波长查找
                if (def.Wavelength != null && def.Wavelength.Length > 0)
                {
                    foreach (float wl in def.Wavelength)
                    {
                        bandNo = bandRefTable.GetBandIndex(wl);
                        if (bandNo > 0)
                            goto setLine;
                    }
                }
                if (def.CenterWaveNum == 0)
                {
                    bandNo = def.BandNo;
                    if (bandNo > 0)
                        goto setLine;
                }
                //按波长类型查找 //GetBandIndexByType
                bandNo = bandRefTable.GetBandIndexByType(def.BandType);
                if (bandNo < 1)
                    continue;
            setLine:
                isOK = true;
                prd.SetArg(def.Identify, bandNo);
            }
            return isOK;
        }

        private static bool SetBands(IArgumentProvider prd, BandDef[] bandDefs)
        {
            if (prd == null || bandDefs == null || bandDefs.Length == 0)
                return true;
            foreach (BandDef def in bandDefs)
            {
                if (def.BandNo > 0)
                {
                    prd.SetArg(def.Identify, def.BandNo);
                    continue;
                }
            }
            return true;
        }

    }
}
