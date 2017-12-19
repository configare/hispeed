#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/11/4 15:59:59
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
using System.IO;

namespace GeoDo.RSS.MIF.Prds.DST
{
    /// <summary>
    /// 类名：DstFY3ExtractArgSet
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/11/4 15:59:59
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class DstFY3ExtractArgSet
    {
        public DstFY3ExtractArg BareLandArg;
        public DstFY3ExtractArg VegetationArg;
        public DstFY3ExtractArg WaterArg;
        public bool isSmooth;

        public DstFY3ExtractArgSet()
        {
            InitArgSet();
        }

        private void InitArgSet()
        {
            string argFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ProductArgs\\DST\\FY3ExtractThreshold.txt");
            if (!File.Exists(argFileName))
                return;
            string[] valueLines = File.ReadAllLines(argFileName);
            if (valueLines != null && valueLines.Length != 12)
                return;
            BareLandArg = new DstFY3ExtractArg();
            VegetationArg = new DstFY3ExtractArg();
            WaterArg = new DstFY3ExtractArg();
            for (int j = 0; j < valueLines.Length; j++)
            {
                string[] values = valueLines[j].Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length != 18)
                    return;
                SetArgsWithSingleLine(j, values);
            }
            isSmooth = true;
        }

        private void SetArgsWithSingleLine(int j,string[] valueStrs)
        {
            int index = j % 4;
            switch (j / 4)
            {
                case 0:
                    SetArg(index, valueStrs, WaterArg);
                    break;
                case 1:
                    SetArg(index, valueStrs, BareLandArg);
                    break;
                case 2:
                    SetArg(index, valueStrs, VegetationArg);
                    break;
            }
        }

        private void SetArg(int index, string[] valueStrs, DstFY3ExtractArg arg)
        {
            float value;
            if (float.TryParse(valueStrs[0], out value))
                arg.Ref470[index] = value;
            if (float.TryParse(valueStrs[1], out value))
                arg.Ref650[index] = value;
            if (float.TryParse(valueStrs[5], out value))
                arg.Ref1380[index] = value;
            if (float.TryParse(valueStrs[6], out value))
                arg.TBB11[index] = value;
            if (float.TryParse(valueStrs[7], out value))
                arg.TBB37[index] = value;
            if (float.TryParse(valueStrs[8], out value))
                arg.BTD11_12[index] = value;
            if (float.TryParse(valueStrs[9], out value))
                arg.BTD11_37[index] = value;
            if (float.TryParse(valueStrs[10], out value))
                arg.R47_64[index] = value;
            if (float.TryParse(valueStrs[11], out value))
                arg.NDVI[index] = value;
            if (float.TryParse(valueStrs[12], out value))
                arg.NDSI[index] = value;
            if (float.TryParse(valueStrs[14], out value))
                arg.NDDI[index] = value;
            if (float.TryParse(valueStrs[17], out value))
                arg.IDSI[index] = value;
        }
    }
}
