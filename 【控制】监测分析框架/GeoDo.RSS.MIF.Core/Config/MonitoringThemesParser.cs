using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Drawing;
using System.IO;

namespace GeoDo.RSS.MIF.Core
{
    public class MonitoringThemesParser : IDisposable
    {
        private XDocument _doc;

        public MonitoringThemesParser()
        {
        }

        public MonitoringThemesParser(string fname)
        {
            _doc = XDocument.Load(fname);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ThemeDef[] Parse()
        {
            if (_doc == null)
                return null;
            XElement root = _doc.Root;
            IEnumerable<XElement> elements = root.Elements();
            if (elements == null || elements.Count() == 0)
                return null;
            List<ThemeDef> themes = new List<ThemeDef>();
            ThemeDef theme;
            foreach (XElement ele in elements)
            {
                if (ele.Name == "Theme")
                {
                    theme = CreatTheme(ele);
                    if (theme != null)
                        themes.Add(theme);
                }
            }
            return (themes == null || themes.Count == 0) ? null : themes.ToArray();
        }

        private ThemeDef CreatTheme(XElement ele)
        {
            ThemeDef theme = new ThemeDef();
            string value = ele.Attribute("name").Value;
            if (!String.IsNullOrEmpty(value))
                theme.Name = value;
            value = ele.Attribute("identify").Value;
            if (!String.IsNullOrEmpty(value))
                theme.Identify = value;
            value = TryGetString(ele.Attribute("productdefdir"));
            if (!string.IsNullOrWhiteSpace(value))
                theme.ProductDir = value;
            List<ProductDef> products = new List<ProductDef>();
            //解析产品目录下独立xml的产品定义
            ProductDef[] pds = ParseProductDefFromDir(theme.ProductDir, theme);
            if (pds != null && pds.Length != 0)
            {
                foreach (ProductDef product in pds)
                {
                    if (!ExistsProduct(product.Identify, products))
                        products.Add(product);
                }
            }
            //解析Theme文档中的产品定义
            ProductDef[] productDefsInXml = ParseProductDefFromXml(ele, theme);
            if (productDefsInXml != null && productDefsInXml.Length != 0)
            {
                foreach (ProductDef product in productDefsInXml)
                {
                    if (!ExistsProduct(product.Identify, products))
                        products.Add(product);
                }
            }
            if (products != null && products.Count != 0)
                theme.Products = products.ToArray();
            //AOIDefs
            AOIDef defaultAOI;
            theme.AOIDefs = ParseAOIDefs(ele.Element("SystemAOIs"), out defaultAOI);
            if (defaultAOI != null)
                theme.DefaultAOIDef = defaultAOI;
            return theme;
        }

        private string TryGetString(XAttribute attribute)
        {
            if (attribute != null)
                return attribute.Value;
            else
                return null;
        }

        private ProductDef[] ParseProductDefFromXml(XElement ele, ThemeDef theme)
        {
            List<ProductDef> products = new List<ProductDef>();
            IEnumerable<XElement> elements = ele.Element(XName.Get("Products")).Elements();
            if (elements == null || elements.Count() == 0)
                return null;
            //productDefs
            ProductDef productdef;
            foreach (XElement element in elements)
            {
                productdef = CreatProduct(element, theme);
                products.Add(productdef);
            }
            return products.ToArray();
        }

        private ProductDef[] ParseProductDefFromDir(string productDefDir, ThemeDef theme)
        {
            if (string.IsNullOrWhiteSpace(productDefDir))
                return null;
            string productdir = Path.GetFullPath(System.AppDomain.CurrentDomain.BaseDirectory + productDefDir);
            if (!Directory.Exists(productdir))
                return null;
            string[] pds = Directory.GetFiles(productdir, "*.xml");
            if (pds == null || pds.Length == 0)
                return null;
            ProductDef product;
            List<ProductDef> products = new List<ProductDef>();
            for (int i = 0; i < pds.Length; i++)
            {
                try
                {
                    XElement element = XElement.Load(pds[i]);
                    product = CreatProduct(element, theme);
                    if (product != null)
                        products.Add(product);
                }
                catch
                {
                    continue;
                }
            }
            return products.ToArray();
        }

        private bool ExistsProduct(string identify, List<ProductDef> pds)
        {
            for (int i = 0; i < pds.Count; i++)
            {
                if (pds[i].Identify == identify)
                    return true;
            }
            return false;
        }

        private AOIDef[] ParseAOIDefs(XElement xElement, out AOIDef defaultAOI)
        {
            defaultAOI = null;
            if (xElement == null)
                return null;
            var elements = xElement.Elements();
            if (elements == null || elements.Count() == 0)
                return null;
            List<AOIDef> aois = new List<AOIDef>();
            foreach (XElement ele in elements)
            {
                if (ele.Name == "DefaultAOI")
                    defaultAOI = GetAOIDef(ele);
                else if (ele.Name == "AOI")
                {
                    AOIDef aoi = GetAOIDef(ele);
                    if (aoi != null)
                        aois.Add(aoi);
                }
            }
            return aois.Count > 0 ? aois.ToArray() : null;
        }

        private AOIDef GetAOIDef(XElement ele)
        {
            AOIDef aoi = new AOIDef();
            aoi.Name = AttributeValue(ele, "name");
            aoi.Provider = AttributeValue(ele, "provider");
            if (ele.Elements("AOI") != null)
            {
                AOIDef nulldef;
                aoi.SubAOIs = ParseAOIDefs(ele, out nulldef);
            }
            return aoi;
        }

        private ProductDef CreatProduct(XElement element, ThemeDef theme)
        {
            ProductDef product = new ProductDef();
            product.Theme = theme;
            string value = element.Attribute("name").Value;
            if (!String.IsNullOrEmpty(value))
                product.Name = value;
            value = element.Attribute("image").Value;
            if (!String.IsNullOrEmpty(value))
                product.Image = value;
            value = element.Attribute("group").Value;
            if (!String.IsNullOrEmpty(value))
                product.Group = value;
            value = element.Attribute("uiprovider").Value;
            if (!String.IsNullOrEmpty(value))
                product.UIProvider = value;
            value = element.Attribute("identify").Value;
            if (!String.IsNullOrEmpty(value))
                product.Identify = value;
            IEnumerable<XElement> subProductsElements = element.Element(XName.Get("SubProducts")).Elements();
            if (subProductsElements != null && subProductsElements.Count() != 0)
            {
                SubProductDef subPro;
                List<SubProductDef> subs = new List<SubProductDef>();
                foreach (XElement ele in subProductsElements)
                {
                    subPro = CreatSubProduct(ele);
                    if (subPro != null)
                        subs.Add(subPro);
                    subPro.ProductDef = product;
                    //Console.WriteLine("SubProduct:"+ele.Attribute("name").Value);
                }
                if (subs != null && subs.Count != 0)
                    product.SubProducts = subs.ToArray();
            }
            IEnumerable<XElement> aoisElements = Elements(element.Element("AOITemplates"));
            if (aoisElements != null && aoisElements.Count() != 0)
            {
                List<AOITemplate> aoiList = new List<AOITemplate>();
                foreach (XElement ele in aoisElements)
                {
                    AOITemplate aoi = CreateAOITemplate(ele);
                    if (aoi != null)
                        aoiList.Add(aoi);
                }
                product.AOITemplates = aoiList.ToArray();
            }
            return product;
        }

        private IEnumerable<XElement> Elements(XElement element)
        {
            if (element == null || !element.HasElements)
                return null;
            return element.Elements();
        }

        private AOITemplate CreateAOITemplate(XElement element)
        {
            AOITemplate aoi = new AOITemplate();
            string value = AttributeValue(element, "name");
            if (!String.IsNullOrEmpty(value))
                aoi.Name = value;
            value = AttributeValue(element, "template");
            if (!String.IsNullOrEmpty(value))
                aoi.Template = value;
            value = AttributeValue(element, "ischecked");
            aoi.IsChecked = value == "true";
            value = AttributeValue(element, "isreverse");
            aoi.IsReverse = value == "true";
            return aoi;
        }

        private SubProductDef CreatSubProduct(XElement element)
        {
            SubProductDef sub = new SubProductDef();
            string value = element.Attribute("name").Value;
            if (!String.IsNullOrEmpty(value))
                sub.Name = value;
            value = element.Attribute("identify").Value;
            if (!String.IsNullOrEmpty(value))
                sub.Identify = value;
            value = AttributeValue(element, "isuseaoitemplate");
            if (!String.IsNullOrEmpty(value))
                sub.IsUseAoiTemplate = value == "true";
            value = AttributeValue(element, "iskeepusercontrol");
            if (!String.IsNullOrEmpty(value))
                sub.IsKeepUserControl = value == "true";
            value = AttributeValue(element, "aoitemplates");
            if (!String.IsNullOrEmpty(value))
                sub.AoiTemplates = value;
            value = AttributeValue(element, "color");
            if (!String.IsNullOrEmpty(value))
                sub.Color = GetColor(value);
            value = AttributeValue(element, "isdisplaypanel");
            if (!String.IsNullOrEmpty(value))
                sub.IsDisplayPanel = value == "true";
            value = AttributeValue(element, "visiablesavebtn");
            if (!String.IsNullOrEmpty(value))
                sub.VisiableSaveBtn = value == "true";
            value = AttributeValue(element, "isneedcurrentraster");
            if (!String.IsNullOrEmpty(value))
                sub.IsNeedCurrentRaster = value.ToLower() == "true";
            value = AttributeValue(element, "aoiseconaryinfofromarg");
            if (!String.IsNullOrEmpty(value))
                sub.AOISecondaryInfoFromArg = value;
            value = AttributeValue(element, "isautogenerate");
            if (!String.IsNullOrEmpty(value))
                sub.IsAutoGenerate = bool.Parse(value);
            value = AttributeValue(element, "autogenerategroup");
            if (!String.IsNullOrEmpty(value))
            {
                string[] groupIds = value.Split(',');
                if (groupIds != null)
                    sub.AutoGenerateGroup = groupIds;
            }
            //algorithms
            IEnumerable<XElement> elements = element.Element("Algorithms").Elements();
            if (elements != null && elements.Count() != 0)
            {
                AlgorithmDef alg;
                List<AlgorithmDef> algs = new List<AlgorithmDef>();
                foreach (XElement ele in elements)
                {
                    alg = CreatAlgorithm(ele);
                    if (alg != null)
                        algs.Add(alg);
                }
                if (algs != null && algs.Count != 0)
                    sub.Algorithms = algs.ToArray();
            }
            //instances
            sub.SubProductInstanceDefs = ParseSubProductInstanceDefs(element.Element("Instances"));
            return sub;
        }

        private SubProductInstanceDef[] ParseSubProductInstanceDefs(XElement xElement)
        {
            if (xElement == null)
                return null;
            var eles = xElement.Elements("Instance");
            if (eles == null || eles.Count() == 0)
                return null;
            List<SubProductInstanceDef> instances = new List<SubProductInstanceDef>();
            foreach (XElement ele in eles)
            {
                SubProductInstanceDef inst = GetSubProductInstanceDef(ele);
                if (inst == null)
                    continue;
                instances.Add(inst);
            }
            return instances.Count > 0 ? instances.ToArray() : null;
        }

        private SubProductInstanceDef GetSubProductInstanceDef(XElement ele)
        {
            string name = AttributeValue(ele, "name");
            string fileprovider = AttributeValue(ele, "fileprovider");
            string argument = AttributeValue(ele, "argument");
            string outfileidentify = AttributeValue(ele, "outfileidentify");
            string layoutname = AttributeValue(ele, "layoutname");
            string aoiprovider = AttributeValue(ele, "aoiprovider");
            string colortablename = AttributeValue(ele, "colortablename");
            string isautogenerate = AttributeValue(ele, "isautogenerate");
            string autogenerategroup = AttributeValue(ele, "autogenerategroup");
            string extinfo = AttributeValue(ele, "extinfo");
            string isOriginal = AttributeValue(ele, "isoriginal");
            string isFixTempleSize = AttributeValue(ele, "isfixtemplesize");
            string outdir = AttributeValue(ele, "outdir");
            string isCurrentView = AttributeValue(ele, "iscurrentview");
            string isextmethod = AttributeValue(ele, "isextmethod");
            string defaultProj = AttributeValue(ele, "defaultproj");
            SubProductInstanceDef sub = new SubProductInstanceDef();
            sub.AOIProvider = aoiprovider;
            sub.Argument = argument;
            sub.FileProvider = fileprovider;
            sub.LayoutName = layoutname;
            sub.Name = name;
            sub.OutFileIdentify = outfileidentify;
            sub.ColorTableName = colortablename;
            sub.OutDir = outdir;
            sub.DefaultProj = defaultProj;
            if (!string.IsNullOrEmpty(isautogenerate))
                sub.isautogenerate = bool.Parse(isautogenerate);
            if (!string.IsNullOrEmpty(autogenerategroup))
            {
                string[] groupIds = autogenerategroup.Split(',');
                if (groupIds != null && groupIds.Length != 0)
                    sub.AutoGenerateGroup = groupIds;
            }
            if (!string.IsNullOrEmpty(extinfo))
                sub.extInfo = extinfo;
            if (!string.IsNullOrEmpty(isOriginal))
                sub.isOriginal = bool.Parse(isOriginal);
            if (!string.IsNullOrEmpty(isFixTempleSize))
                sub.isFixTempleSize = bool.Parse(isFixTempleSize);
            if (!string.IsNullOrEmpty(isCurrentView))
                sub.isCurrentView = bool.Parse(isCurrentView);
            if (!string.IsNullOrEmpty(isextmethod))
                sub.isExtMethod = bool.Parse(isextmethod);
            else
                sub.isExtMethod = true;
            return sub;
        }

        private Color GetColor(string value)
        {
            string[] colorStrings = value.Split(',');
            if (colorStrings == null)
                return Color.Red;
            int a, r, g, b;
            if (colorStrings.Length == 4)
            {
                a = int.Parse(colorStrings[0]);
                r = int.Parse(colorStrings[1]);
                g = int.Parse(colorStrings[2]);
                b = int.Parse(colorStrings[3]);
                return Color.FromArgb(a, r, g, b);
            }
            else if (colorStrings.Length == 3)
            {
                r = int.Parse(colorStrings[0]);
                g = int.Parse(colorStrings[1]);
                b = int.Parse(colorStrings[2]);
                return Color.FromArgb(r, g, b);
            }
            else
                return Color.Red;
        }

        private AlgorithmDef CreatAlgorithm(XElement ele)
        {
            AlgorithmDef alg = new AlgorithmDef();
            IEnumerable<XAttribute> attrs = ele.Attributes();
            foreach (XAttribute att in attrs)
            {
                switch (att.Name.ToString().ToLower())
                {
                    case "name":
                        if (!String.IsNullOrEmpty(att.Value))
                            alg.Name = att.Value;
                        break; 
                    case "identify":
                        if (!String.IsNullOrEmpty(att.Value))
                            alg.Identify = att.Value;
                        break;
                    case "satellite":
                        alg.Satellites = AttributsToStrings(att.Value);
                        break;
                    case "sensor":
                        alg.Sensors = AttributsToStrings(att.Value);
                        break;
                    case "customidentify":
                        if (!String.IsNullOrEmpty(att.Value))
                            alg.CustomIdentify = att.Value;
                        break;
                    case "grouptype":
                        if (!string.IsNullOrEmpty(att.Value))
                            alg.GroupTypeName = att.Value;
                        else
                        {
                            alg.GroupTypeName = "默认组";
                        }
                        break;
                }
            }
            
            ParseBands(ele, ref alg);
            ParseArguments(ele, ref alg);
            ConvertBandZoomToArg(ref alg);
            ConvertBandCenterWaveNumToArg(ref alg);
            return alg;
        }

        private void ConvertBandCenterWaveNumToArg(ref AlgorithmDef alg)
        {
            if (alg.Bands == null || alg.Bands.Length == 0)
                return;
            ArgumentBase[] args = new ArgumentBase[alg.Arguments.Length + alg.Bands.Length];
            for (int i = 0; i < alg.Arguments.Length; i++)
                args[i] = alg.Arguments[i];
            int idx = alg.Arguments.Length;
            foreach (BandDef band in alg.Bands)
            {
                ArgumentDef arg = new ArgumentDef();
                arg.Name = (band.Identify + "_CenterWaveNum");
                arg.Datatype = "double";
                arg.Description = arg.Name;
                arg.Visible = false;
                arg.Defaultvalue = (band.CenterWaveNum).ToString();
                args[idx++] = arg;
            }
            alg.Arguments = args;
        }

        private void ConvertBandZoomToArg(ref AlgorithmDef alg)
        {
            if (alg.Bands == null || alg.Bands.Length == 0)
                return;
            ArgumentBase[] args = new ArgumentBase[alg.Arguments.Length + alg.Bands.Length];
            for (int i = 0; i < alg.Arguments.Length; i++)
                args[i] = alg.Arguments[i];
            int idx = alg.Arguments.Length;
            foreach (BandDef band in alg.Bands)
            {
                ArgumentDef arg = new ArgumentDef();
                arg.Name = (band.Identify + "_Zoom");
                arg.Datatype = "double";
                arg.Defaultvalue = (band.Zoom).ToString();
                args[idx++] = arg;
            }
            alg.Arguments = args;
        }

        private void ParseBands(XElement ele, ref AlgorithmDef alg)
        {
            XElement subEle = ele.Element(XName.Get("Bands"));
            if (subEle == null)
                return;
            IEnumerable<XElement> eleBands = subEle.Elements();
            if (eleBands == null || eleBands.Count() == 0)
                return;
            BandDef band;
            List<BandDef> bands = new List<BandDef>();
            foreach (XElement element in eleBands)
            {
                band = CreatBand(element);
                if (band != null)
                    bands.Add(band);
            }
            if (bands != null && bands.Count != 0)
                alg.Bands = bands.ToArray();
        }

        private void ParseArguments(XElement ele, ref AlgorithmDef alg)
        {
            XElement subXElement = ele.Element(XName.Get("Arguments"));
            if (subXElement == null)
                return;
            ArgumentBase[] args = ParseArguments(subXElement);
            if (args != null && args.Length != 0)
                alg.Arguments = args;
        }

        private ArgumentBase[] ParseArguments(XElement subEle)
        {
            IEnumerable<XElement> eleArgs = subEle.Elements();
            if (eleArgs == null || eleArgs.Count() == 0)
                return null;
            ArgumentBase arg = null;
            List<ArgumentBase> args = new List<ArgumentBase>();
            foreach (XElement element in eleArgs)
            {
                if (element.Name == "ArgumentPair")
                    arg = CreatArgumentPair(element);
                else if (element.Name == "Argument")
                    arg = CreatArgument(element);
                else if (element.Name == "ArgumentGroup")
                    arg = CreateArgumentGroup(element);
                if (arg != null)
                    args.Add(arg);
            }
            return args.ToArray();
        }

        private ArgumentBase CreateArgumentGroup(XElement element)
        {
            IEnumerable<XAttribute> attrs = element.Attributes();
            if (attrs == null || attrs.Count() == 0)
                return null;
            ArgumentGroup arg = new ArgumentGroup();
            foreach (XAttribute att in attrs)
            {
                switch (att.Name.ToString().ToLower())
                {
                    case "name":
                        if (!String.IsNullOrWhiteSpace(att.Value))
                            arg.Name = att.Value;
                        break;
                    case "description":
                        if (!String.IsNullOrWhiteSpace(att.Value))
                            arg.Description = att.Value;
                        break;
                    case "visible":
                        if (att.Value == "false")
                            arg.Visible = false;
                        else
                            arg.Visible = true;
                        break;
                }
            }
            ArgumentBase[] args = ParseArguments(element);

            if (args != null && args.Length != 0)
                arg.Arguments = args;
            return arg;
        }

        private ArgumentBase CreatArgumentPair(XElement element)
        {
            IEnumerable<XAttribute> attrs = element.Attributes();
            if (attrs == null || attrs.Count() == 0)
                return null;
            ArgumentPair argPair = new ArgumentPair();
            foreach (XAttribute att in attrs)
            {
                switch (att.Name.ToString().ToLower())
                {
                    case "description":
                        if (!String.IsNullOrWhiteSpace(att.Value))
                            argPair.Description = att.Value;
                        break;
                    case "datatype":
                        if (!String.IsNullOrWhiteSpace(att.Value))
                            argPair.Datatype = att.Value;
                        break;
                    case "minvalue":
                        if (!String.IsNullOrWhiteSpace(att.Value))
                            argPair.MinValue = att.Value;
                        break;
                    case "maxvalue":
                        if (!String.IsNullOrWhiteSpace(att.Value))
                            argPair.MaxValue = att.Value;
                        break;
                    case "visible":
                        if (att.Value == "false")
                            argPair.Visible = false;
                        else
                            argPair.Visible = true;
                        break;
                    case "finetuning":
                        if (!String.IsNullOrWhiteSpace(att.Value))
                            argPair.FineTuning = att.Value;
                        break;
                }
            }
            IEnumerable<XElement> eleArgs = element.Elements();
            if (eleArgs == null || eleArgs.Count() == 0)
                return null;
            ArgumentDef argMin = null;
            ArgumentDef argMax = null;
            foreach (XElement ele in eleArgs)
            {
                string endpointtype = ele.Attribute("endpointtype").Value;
                if (endpointtype == "min")
                    argMin = CreatArgument(ele);
                else if (endpointtype == "max")
                    argMax = CreatArgument(ele);
            }
            if (argMax != null)
            {
                argMax.Datatype = argPair.Datatype;
                argPair.ArgumentMax = argMax;
            }
            if (argMin != null)
            {
                argMin.Datatype = argPair.Datatype;
                argPair.ArgumentMin = argMin;
            }
            return argPair;
        }

        private ArgumentDef CreatArgument(XElement element)
        {
            IEnumerable<XAttribute> attrs = element.Attributes();
            if (attrs == null || attrs.Count() == 0)
                return null;
            ArgumentDef argument = new ArgumentDef();
            foreach (XAttribute att in attrs)
            {
                switch (att.Name.ToString().ToLower())
                {
                    case "visible":
                        if (att.Value == "false")
                            argument.Visible = false;
                        else
                            argument.Visible = true;
                        break;
                    case "name":
                        if (!String.IsNullOrEmpty(att.Value))
                            argument.Name = att.Value;
                        break;
                    case "description":
                        if (!String.IsNullOrEmpty(att.Value))
                            argument.Description = att.Value;
                        break;
                    case "datatype":
                        if (!String.IsNullOrEmpty(att.Value))
                            argument.Datatype = att.Value;
                        break;
                    case "defaultvalue":
                        if (!String.IsNullOrEmpty(att.Value))
                            argument.Defaultvalue = att.Value;
                        break;
                    case "minvalue":
                        if (!String.IsNullOrEmpty(att.Value))
                            argument.MinValue = att.Value;
                        break;
                    case "maxvalue":
                        if (!String.IsNullOrEmpty(att.Value))
                            argument.MaxValue = att.Value;
                        break;
                    case "refdatatype":
                        if (!String.IsNullOrEmpty(att.Value))
                            argument.RefType = att.Value;
                        break;
                    case "refidentify":
                        if (!String.IsNullOrEmpty(att.Value))
                            argument.RefIdentify = att.Value;
                        break;
                    case "reffilter":
                        if (!String.IsNullOrEmpty(att.Value))
                            argument.RefFilter = att.Value;
                        break;
                    case "ismultiselect":
                        if (!String.IsNullOrEmpty(att.Value))
                            argument.IsMultiSelect = att.Value == "true";
                        break;
                    case "isoptional":
                        if (!String.IsNullOrEmpty(att.Value))
                            argument.IsOptional = att.Value == "true";
                        break;
                    case "editoruiprovider":
                        if (!String.IsNullOrEmpty(att.Value))
                            argument.EditorUiProvider = att.Value;
                        break;
                    case "fileprovider":
                        if (!String.IsNullOrEmpty(att.Value))
                            argument.FileProvider = att.Value;
                        break;
                    case "isalgorithmshare":
                        if (!String.IsNullOrEmpty(att.Value))
                            argument.IsAlgorithmShare = att.Value == "true";
                        break;
                    case "iseventnotification":
                        bool value;
                        if (!String.IsNullOrEmpty(att.Value) && bool.TryParse(att.Value, out value))
                            argument.IsEventNotification = value;
                        break;
                }
            }
            //自定义参数的缺省参数
            if (element.Element("DefaultValue") != null)
            {
                argument.DefaultValueElement = element.Element("DefaultValue");
            }
            //
            return argument;
        }

        private BandDef CreatBand(XElement element)
        {
            IEnumerable<XAttribute> attrs = element.Attributes();
            if (attrs == null || attrs.Count() == 0)
                return null;
            BandDef band = new BandDef();
            foreach (XAttribute att in attrs)
            {
                switch (att.Name.ToString().ToLower())
                {
                    case "identify":
                        if (!String.IsNullOrEmpty(att.Value))
                            band.Identify = att.Value;
                        break;
                    case "bandno":
                        int bandno;
                        if (int.TryParse(att.Value, out bandno))
                            band.BandNo = bandno;
                        else
                            band.BandNo = -1;
                        break;
                    case "wavelength":
                        if (!String.IsNullOrEmpty(att.Value))
                            band.Wavelength = AttributeToFloats(att.Value);
                        else
                            band.Wavelength = null;
                        break;
                    case "bandtype":
                        if (!String.IsNullOrEmpty(att.Value))
                            band.BandType = att.Value;
                        break;
                    case "zoom":
                        if (!String.IsNullOrEmpty(att.Value))
                            band.Zoom = double.Parse(att.Value);
                        break;
                    case "fromarg":
                        if (!String.IsNullOrEmpty(att.Value))
                            band.FromArgument = att.Value;
                        break;
                    case "centerwavenum":
                        double waveNum;
                        if (double.TryParse(att.Value, out waveNum))
                            band.CenterWaveNum = waveNum;
                        else
                            band.CenterWaveNum = -1;
                        break;
                }
            }
            return band;
        }

        private string[] AttributsToStrings(string p)
        {
            if (String.IsNullOrEmpty(p))
                return null;
            return p.Split(',');
        }

        private float[] AttributeToFloats(string att)
        {
            string[] atts = att.Split(',');
            List<float> values = new List<float>();
            float value;
            foreach (string s in atts)
            {
                bool ok = float.TryParse(s, out value);
                if (ok)
                    values.Add(value);
            }
            return (values == null || values.Count == 0) ? null : values.ToArray();
        }

        private string AttributeValue(XElement element, XName attribute)
        {
            XAttribute attr = element.Attribute(attribute);
            return attr != null ? attr.Value : "";
        }

        public void Dispose()
        {
            _doc = null;
        }
    }
}
