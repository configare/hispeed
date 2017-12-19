using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.SNW
{
    public class SubProductSTATSNW : CmaMonitoringSubProduct
    {

        public SubProductSTATSNW(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "STATAlgorithm")
            {
                //按照Instance执行统计操作
                string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
                if (instanceIdentify != null)
                {
                    SubProductInstanceDef instance = FindSubProductInstanceDefs(instanceIdentify);
                    if (instance == null || instance.OutFileIdentify == "CCCA" || instance.OutFileIdentify == "CCAR")     //省市县面积统计
                        return STATAlgorithm();
                    else if(instance.OutFileIdentify == "ADLU")
                        return ADLUSTAT();
                    else
                        return StatRaster<short>(instance, (v) => { return v == 1; }, progressTracker);
                }   
            }
            return null;
        }

        private IExtractResult ADLUSTAT() 
        {
            object aioObj = _argumentProvider.GetArg("AOI");
            string outFileIdentify = GetStringArgument("OutFileIdentify");
            SubProductInstanceDef instance = FindSubProductInstanceDefs(outFileIdentify);
            string[] files = GetStringArray("SelectedPrimaryFiles");
            string extInfos = GetStringArgument("extinfo");
            if (files == null || files.Length == 0)
                return null;
            _argumentProvider.SetArg("statname", "变化积雪");
                Dictionary<string,Func<short,bool>> dic=new Dictionary<string,Func<short,bool>>();
                dic.Add("持续雪面积（平方公里）", (v) => { return v == 1; });  
            string title = string.Empty;
            StatResultItem[] sameResult = CommProductStat.AreaStat<Int16>("变化积雪", files[0], ref title, aioObj, (v) => { return v == 1; });
            Dictionary<string, string[]> result = new Dictionary<string, string[]>();
            StatResultItem[][] resultArray = new StatResultItem[][] { sameResult };
                if (resultArray[0] != null && resultArray[0].Length > 0)
                {
                    foreach (StatResultItem item in resultArray[0])
                    {
                        if (result.ContainsKey(item.Name))
                        {
                            result[item.Name][0] = item.Value.ToString();
                        }
                        else
                        {
                            result.Add(item.Name, new string[1] { "0" });
                            result[item.Name][0] = item.Value.ToString();
                        }
                    }
                }
            
            if (result.Count == 0)
                return null;
            List<string[]> resultList = new List<string[]>();
            foreach (string key in result.Keys)
            {
                resultList.Add(new string[] { key, result[key][0] });
            }
            string sentitle = "统计日期：" + DateTime.Now.ToShortDateString();
            RasterIdentify id = new RasterIdentify(files[0]);
            if (id.OrbitDateTime != null)
                sentitle += "    轨道日期：" + id.OrbitDateTime.ToShortDateString();
            string[] columns = new string[] { "矢量分区",  "积雪面积（平方公里）" };
            IStatResult fresult = new StatResult(sentitle, columns, resultList.ToArray());
            string outputIdentify = _argumentProvider.GetArg("OutFileIdentify").ToString();
            string filename = StatResultToFile(files, fresult, "SNW", outputIdentify, title, null, 1, true, 1);
            return new FileExtractResult(outputIdentify, filename);
        }
   
        private IExtractResult STATAlgorithm()
        {
            
            return AreaStatResult<Int16>("积雪", "SNW", (v) => { return v == 1; });
        }
    }
}
