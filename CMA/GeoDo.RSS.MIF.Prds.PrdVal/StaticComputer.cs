#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2014-6-20 08:42:25
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
using System.Windows.Forms;
namespace GeoDo.RSS.MIF.Prds.PrdVal
{
    /// <summary>
    /// 类名：StaticComputer
    /// 属性描述：临时替换用
    /// 创建者：lxj    创建日期：2014-6-20 08:42:25
    /// 修改者：DongW      修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    /// 

    public interface IStaticComputer<T>
    {
        double ComputeRMSE(T[] toVal, T[] forVal, T[] toInvalid, T[] forInvalid);
        List<string[]> ComputeDeviation(T[] toVal, T[] forVal, T[] toInvalid, T[] forInvalid, int maxIntervalCount);
    }

    public abstract class StaticComputer<T>:IStaticComputer<T> where T:IComparable
    {

       public List<string[]> CompBiasHist(Int16[] toVal, Int16[] forVal, string toinvalid, string forinvalid, int height, int width, int columMax)
       {
           return CompBiasHist1(toVal, forVal, toinvalid, forinvalid, height, width, columMax);
       }
       /// <summary>
       /// 
       /// </summary>
       /// <param name="toVal"></param>
       /// <param name="forVal"></param>
       /// <param name="invalid">无效值</param>
       /// <param name="height">高</param>
       /// <param name="width">宽</param>
       /// <param name="columMax">最大分组</param>
       /// <returns></returns>
       public List<string[]> CompBiasHist1(Int16[] toVal, Int16[] forVal, string toinvalid, string forinvalid,int height, int width, int columMax)
       {
           Int16[] hist = new Int16[height * width];
           List<Int16> histList = new List<short>();
           Dictionary<Int16, int> satCount = new Dictionary<Int16, int>();//差值，个数
           string[] toinvalids = new string[] { };
           if (!String.IsNullOrWhiteSpace(toinvalid) && toinvalid.Contains(","))
               toinvalids = toinvalid.Split(new char[] { ',' });
           else
               toinvalids = new string[] { toinvalid };
           string[] forinvalids = new string[] { };
           if (!String.IsNullOrWhiteSpace(forinvalid) && forinvalid.Contains(","))
               forinvalids = forinvalid.Split(new char[] { ',' });
           else
               forinvalids = new string[] { forinvalid };

           for (int i = 0; i < height * width; i++)
           {
               int id = 0; //标志
               if (toinvalids.Length >= 1)
               {
                   foreach (string toinval in toinvalids)
                   {
                       if (toinval == Convert.ToString(toVal[i]))
                       {
                           id = 1;
                       }
                   }
               }
               if(forinvalids.Length >=1)
               {
                   foreach (string forinval in forinvalids)
                   {
                       if (forinval == Convert.ToString(forVal[i]))
                       {
                           id = 1;
                       }
                   }
               }
               //剔除无效值
               if (id == 1)
                {
                    hist[i] = -1; //无效值
                }
                else
                {
                    hist[i] = Convert.ToInt16(toVal[i] - forVal[i]);
                    histList.Add(hist[i]);
                    if (!satCount.ContainsKey(hist[i]))
                        satCount.Add(hist[i], 1);
                    else
                        satCount[hist[i]] = satCount[hist[i]] + 1;
                }
           }
           Int16[] hist1 = histList.ToArray();
           Int16 max = hist1.Max();
           Int16 min = hist1.Min();
           Int16 step = 0;
           List<string[]> listRow = new List<string[]>();
           if (satCount.Count > columMax) //依最大组数,分段间隔
           {
               List<int> sum = new List<int>();
               for (int m = 0; m < columMax; m++)  //每分段组初始化。
               {
                   sum.Add(0);
               }
               step = Convert.ToInt16(Math.Ceiling(Convert.ToDouble((max - min + 1) / columMax))); //步长 step 取最大整数,最后一组的最大值是大于max
               int exsum = 0;
               try
               {
                   foreach (short i in satCount.Keys)
                   {
                       Int16 nstep = Convert.ToInt16((i - min) / step);//这个差值落在第几组，相应的组个数累加
                       if (nstep >= sum.Count)
                       {
                           exsum = exsum + satCount[i];
                       }
                       else
                       {
                           sum[nstep] = sum[nstep] + satCount[i];
                       }
                   }
               }
               catch (Exception ex)
               {
                   MessageBox.Show("异常信息：" + ex.Message);
               }
               for (int j = 0; j < columMax; j++)   //[start,end)
               {
                   string[] row = new string[] { Convert.ToString(min + step * j) + "~" + Convert.ToString(min + step * (j + 1)), Convert.ToString(sum[j]) };
                   listRow.Add(row);
               }
               int exStart = max - (min + step * columMax);
               listRow.Add(new string[] { Convert.ToString(exStart) + "~" + Convert.ToString(max), Convert.ToString(exsum) });
           }
           else   
           {
               foreach(short i in satCount.Keys)
               {
                   string[] row = new string[] { Convert.ToString(i),Convert.ToString(satCount[i]) };
                   listRow.Add(row);
               }
           }
           return listRow;
       }

        /// <summary>
        /// 计算两组数据的均方根误差
        /// </summary>
        /// <param name="toVal"></param>
        /// <param name="forVal"></param>
        /// <param name="toInvalid">toVal数组无效值</param>
        /// <param name="forInvalid">forVal数组无效值</param>
        /// <returns>均方根误差</returns>
       public abstract double ComputeRMSE(T[] toVal, T[] forVal, T[] toInvalid, T[] forInvalid);
       
       /// <summary>
       /// 计算偏差
       /// </summary>
       /// <param name="toVal">待验证数据数组</param>
       /// <param name="forVal">验证数据数组</param>
       /// <param name="toInvalid">toVal中无效值</param>
       /// <param name="forInvalid">forVal中无效值</param>
       /// <param name="maxIntervalCount"></param>
       /// <returns>由区间及该区间累计次数组成的字符串数组序列</returns>
       public abstract List<string[]> ComputeDeviation(T[] toVal, T[] forVal, T[] toInvalid, T[] forInvalid, int maxIntervalCount);

       protected bool IsInvalidValue(T value, T[] invalidValues)
       {
           if (invalidValues == null || invalidValues.Length < 1)
               return false;
           for (int i = 0; i < invalidValues.Length; i++)
           {
               if (value.CompareTo(invalidValues[i]) == 0)
                   return true;
           }
           return false;
       }

    }

    public class StaticComputerUInt16 : StaticComputer<UInt16>
    {
        public override double ComputeRMSE(UInt16[] toVal, UInt16[] forVal, UInt16[] toInvalid, UInt16[] forInvalid)
        {
            if (toVal == null || forVal == null || toVal.Length < 1 || forVal.Length < 1 || toVal.Length != forVal.Length)
                return 0;
            int length = forVal.Length;
            double sum = 0;
            int count = 0;
            for (int i = 0; i < length; i++)
            {
                if (IsInvalidValue(toVal[i], toInvalid))
                    continue;
                if (IsInvalidValue(forVal[i], forInvalid))
                    continue;
                sum += Math.Pow((toVal[i] - forVal[i]), 2);
                count++;
            }
            return Math.Sqrt(sum / count);
        }

        public override List<string[]> ComputeDeviation(UInt16[] toVal, UInt16[] forVal, UInt16[] toInvalid, UInt16[] forInvalid, int maxIntervalCount)
        {
            if (toVal == null || forVal == null || toVal.Length < 1 || forVal.Length < 1 || toVal.Length != forVal.Length)
                return null;
            Dictionary<int, int> devSet = new Dictionary<int, int>();
            int difference;
            int length = toVal.Length;
            List<string[]> listRow = new List<string[]>();
            for (int i = 0; i < length; i++)
            {
                if (IsInvalidValue(toVal[i], toInvalid))
                    continue;
                if (IsInvalidValue(forVal[i], forInvalid))
                    continue;
                difference = toVal[i] - forVal[i];
                if (devSet.ContainsKey(difference))
                    devSet[difference] += 1;
                else
                    devSet.Add(difference, 1);
            }
            if (devSet.Keys.Count < maxIntervalCount)
            {
                foreach (short key in devSet.Keys)
                {
                    string[] row = new string[] { key.ToString(), devSet[key].ToString() };
                    listRow.Add(row);
                }
            }
            else
            {
                int minValue = devSet.Keys.ToArray().Min();
                int maxValue = devSet.Keys.ToArray().Max();
                int step = (int)(Math.Ceiling((double)(maxValue - minValue + 1) / maxIntervalCount));
                int[] stepValueCount = new int[maxIntervalCount];   //存放每个区间的个数
                foreach (int key in devSet.Keys)
                {
                    int index = (key - minValue) / step;
                    stepValueCount[index] += devSet[key];
                }
                for (int j = 1; j < maxIntervalCount; j++)   //[start,end)
                {
                    listRow.Add(new string[] { (minValue + step * (j - 1)) + "~" + (minValue + step * j), stepValueCount[j - 1].ToString() });
                }
                //listRow.Add(new string[] { minValue+step*(maxIntervalCount-1) + "~" + maxValue, stepValueCount[maxIntervalCount-1].ToString() });
            }
            return listRow;
        }
    }

    public class StaticComputerInt16 : StaticComputer<Int16>
    {
        public override double ComputeRMSE(Int16[] toVal, Int16[] forVal, Int16[] toInvalid, Int16[] forInvalid)
        {
            if (toVal == null || forVal == null || toVal.Length < 1 || forVal.Length < 1 || toVal.Length != forVal.Length)
                return 0;
            int length = forVal.Length;
            double sum = 0;
            int count = 0;
            for (int i = 0; i < length; i++)
            {
                if (IsInvalidValue(toVal[i], toInvalid))
                    continue;
                if (IsInvalidValue(forVal[i], forInvalid))
                    continue;
                sum+=Math.Pow((toVal[i]-forVal[i]),2);
                count++;
            }
            return Math.Sqrt(sum / count);
        }

        public override List<string[]> ComputeDeviation(Int16[] toVal, Int16[] forVal, Int16[] toInvalid, Int16[] forInvalid, int maxIntervalCount)
        {
            if (toVal == null || forVal == null || toVal.Length < 1 || forVal.Length < 1 || toVal.Length != forVal.Length)
                return null;
            Dictionary<int, int> devSet = new Dictionary<int, int>();
            int difference ;
            int length = toVal.Length;
            List<string[]> listRow = new List<string[]>();
            for (int i = 0; i < length; i++)
            {
                if (IsInvalidValue(toVal[i], toInvalid))
                    continue;
                if (IsInvalidValue(forVal[i], forInvalid))
                    continue;
                difference = toVal[i] - forVal[i];
                if (devSet.ContainsKey(difference))
                    devSet[difference] += 1;
                else
                    devSet.Add(difference, 1);
            }
            if (devSet.Keys.Count < maxIntervalCount)
            {
                foreach (short key in devSet.Keys)
                {
                    string[] row = new string[] { key.ToString(), devSet[key].ToString() };
                    listRow.Add(row);
                }
            }
            else
            {
                int minValue = devSet.Keys.ToArray().Min();
                int maxValue = devSet.Keys.ToArray().Max();
                int step = (int)(Math.Ceiling((double)(maxValue - minValue + 1) / maxIntervalCount));
                int[] stepValueCount = new int[maxIntervalCount];   //存放每个区间的个数
                foreach (int key in devSet.Keys)
                {
                    int index = (key - minValue) / step;
                    stepValueCount[index] += devSet[key];
                }
                for (int j = 1; j < maxIntervalCount; j++)   //[start,end)
                {
                    listRow.Add(new string[]{(minValue+step*(j-1))+"~"+(minValue+step*j),stepValueCount[j-1].ToString()});
                }
                //listRow.Add(new string[] { minValue+step*(maxIntervalCount-1) + "~" + maxValue, stepValueCount[maxIntervalCount-1].ToString() });
            }
            return listRow;
        }
    }

    public class StaticComputerUInt32 : StaticComputer<UInt32>
    {
        public override double ComputeRMSE(UInt32[] toVal, UInt32[] forVal, UInt32[] toInvalid, UInt32[] forInvalid)
        {
            if (toVal == null || forVal == null || toVal.Length < 1 || forVal.Length < 1 || toVal.Length != forVal.Length)
                return 0;
            int length = forVal.Length;
            double sum = 0;
            int count = 0;
            for (int i = 0; i < length; i++)
            {
                if (IsInvalidValue(toVal[i], toInvalid))
                    continue;
                if (IsInvalidValue(forVal[i], forInvalid))
                    continue;
                sum += Math.Pow((toVal[i] - forVal[i]), 2);
                count++;
            }
            return Math.Sqrt(sum / count);
        }

        public override List<string[]> ComputeDeviation(UInt32[] toVal, UInt32[] forVal, UInt32[] toInvalid, UInt32[] forInvalid, int maxIntervalCount)
        {
            if (toVal == null || forVal == null || toVal.Length < 1 || forVal.Length < 1 || toVal.Length != forVal.Length)
                return null;
            Dictionary<Int64, int> devSet = new Dictionary<Int64, int>();
            Int64 difference;
            int length = toVal.Length;
            List<string[]> listRow = new List<string[]>();
            for (int i = 0; i < length; i++)
            {
                if (IsInvalidValue(toVal[i], toInvalid))
                    continue;
                if (IsInvalidValue(forVal[i], forInvalid))
                    continue;
                difference = toVal[i] - forVal[i];
                if (devSet.ContainsKey(difference))
                    devSet[difference] += 1;
                else
                    devSet.Add(difference, 1);
            }
            if (devSet.Keys.Count < maxIntervalCount)
            {
                foreach (short key in devSet.Keys)
                {
                    string[] row = new string[] { key.ToString(), devSet[key].ToString() };
                    listRow.Add(row);
                }
            }
            else
            {
                Int64 minValue = devSet.Keys.ToArray().Min();
                Int64 maxValue = devSet.Keys.ToArray().Max();
                int step = (int)(Math.Ceiling((double)(maxValue - minValue + 1) / maxIntervalCount));
                Int64[] stepValueCount = new Int64[maxIntervalCount];   //存放每个区间的个数
                foreach (int key in devSet.Keys)
                {
                    int index = (int)(key - minValue) / step;
                    stepValueCount[index] += devSet[key];
                }
                for (int j = 1; j < maxIntervalCount; j++)   //[start,end)
                {
                    listRow.Add(new string[] { (minValue + step * (j - 1)) + "~" + (minValue + step * j), stepValueCount[j - 1].ToString() });
                }
            }
            return listRow;
        }
    }

    public class StaticComputerInt32 : StaticComputer<Int32>
    {
        public override double ComputeRMSE(Int32[] toVal, Int32[] forVal, Int32[] toInvalid, Int32[] forInvalid)
        {
            if (toVal == null || forVal == null || toVal.Length < 1 || forVal.Length < 1 || toVal.Length != forVal.Length)
                return 0;
            int length = forVal.Length;
            double sum = 0;
            int count = 0;
            for (int i = 0; i < length; i++)
            {
                if (IsInvalidValue(toVal[i], toInvalid))
                    continue;
                if (IsInvalidValue(forVal[i], forInvalid))
                    continue;
                sum += Math.Pow((toVal[i] - forVal[i]), 2);
                count++;
            }
            return Math.Sqrt(sum / count);
        }

        public override List<string[]> ComputeDeviation(Int32[] toVal, Int32[] forVal, Int32[] toInvalid, Int32[] forInvalid, int maxIntervalCount)
        {
            if (toVal == null || forVal == null || toVal.Length < 1 || forVal.Length < 1 || toVal.Length != forVal.Length)
                return null;
            Dictionary<int, int> devSet = new Dictionary<int, int>();
            int difference;
            int length = toVal.Length;
            List<string[]> listRow = new List<string[]>();
            for (int i = 0; i < length; i++)
            {
                if (IsInvalidValue(toVal[i], toInvalid))
                    continue;
                if (IsInvalidValue(forVal[i], forInvalid))
                    continue;
                difference = toVal[i] - forVal[i];
                if (devSet.ContainsKey(difference))
                    devSet[difference] += 1;
                else
                    devSet.Add(difference, 1);
            }
            if (devSet.Keys.Count < maxIntervalCount)
            {
                foreach (short key in devSet.Keys)
                {
                    string[] row = new string[] { key.ToString(), devSet[key].ToString() };
                    listRow.Add(row);
                }
            }
            else
            {
                int minValue = devSet.Keys.ToArray().Min();
                int maxValue = devSet.Keys.ToArray().Max();
                int step = (int)(Math.Ceiling((double)(maxValue - minValue + 1) / maxIntervalCount));
                int[] stepValueCount = new int[maxIntervalCount];   //存放每个区间的个数
                foreach (int key in devSet.Keys)
                {
                    int index = (key - minValue) / step;
                    stepValueCount[index] += devSet[key];
                }
                for (int j = 1; j < maxIntervalCount; j++)   //[start,end)
                {
                    listRow.Add(new string[] { (minValue + step * (j - 1)) + "~" + (minValue + step * j), stepValueCount[j - 1].ToString() });
                }
            }
            return listRow;
        }
    }

    public class StaticComputerFloat : StaticComputer<float>
    {
        public override double ComputeRMSE(float[] toVal, float[] forVal, float[] toInvalid, float[] forInvalid)
        {
            if (toVal == null || forVal == null || toVal.Length < 1 || forVal.Length < 1 || toVal.Length != forVal.Length)
                return 0;
            int length = forVal.Length;
            double sum = 0;
            int count = 0;
            for (int i = 0; i < length; i++)
            {
                if (IsInvalidValue(toVal[i], toInvalid))
                    continue;
                if (IsInvalidValue(forVal[i], forInvalid))
                    continue;
                sum += Math.Pow((toVal[i] - forVal[i]), 2);
                count++;
            }
            return Math.Sqrt(sum / count);
        }

        public override List<string[]> ComputeDeviation(float[] toVal, float[] forVal, float[] toInvalid, float[] forInvalid, int maxIntervalCount)
        {
            if (toVal == null || forVal == null || toVal.Length < 1 || forVal.Length < 1 || toVal.Length != forVal.Length)
                return null;
            Dictionary<float, int> devSet = new Dictionary<float, int>();
            float difference;
            int length = toVal.Length;
            List<string[]> listRow = new List<string[]>();
            for (int i = 0; i < length; i++)
            {
                if (IsInvalidValue(toVal[i], toInvalid))
                    continue;
                if (IsInvalidValue(forVal[i], forInvalid))
                    continue;
                difference = toVal[i] - forVal[i];
                if (devSet.ContainsKey(difference))
                    devSet[difference] += 1;
                else
                    devSet.Add(difference, 1);
            }
            if (devSet.Keys.Count < maxIntervalCount)
            {
                foreach (short key in devSet.Keys)
                {
                    string[] row = new string[] { key.ToString(), devSet[key].ToString() };
                    listRow.Add(row);
                }
            }
            else
            {
                float minValue = devSet.Keys.ToArray().Min();
                float maxValue = devSet.Keys.ToArray().Max();
                int step = (int)(Math.Ceiling((double)(maxValue - minValue + 1) / maxIntervalCount));
                int[] stepValueCount = new int[maxIntervalCount];   //存放每个区间的个数
                foreach (float key in devSet.Keys)
                {
                    int index = (int)(key - minValue) / step;
                    stepValueCount[index] += devSet[key];
                }
                for (int j = 1; j < maxIntervalCount; j++)   //[start,end)
                {
                    listRow.Add(new string[] { (minValue + step * (j - 1)) + "~" + (minValue + step * j), stepValueCount[j - 1].ToString() });
                }
            }
            return listRow;
        }
    }

    public class StaticComputerDouble : StaticComputer<double>
    {
        public override double ComputeRMSE(double[] toVal, double[] forVal, double[] toInvalid, double[] forInvalid)
        {
            if (toVal == null || forVal == null || toVal.Length < 1 || forVal.Length < 1 || toVal.Length != forVal.Length)
                return 0;
            int length = forVal.Length;
            double sum = 0;
            int count = 0;
            for (int i = 0; i < length; i++)
            {
                if (IsInvalidValue(toVal[i], toInvalid))
                    continue;
                if (IsInvalidValue(forVal[i], forInvalid))
                    continue;
                sum += Math.Pow((toVal[i] - forVal[i]), 2);
                count++;
            }
            return Math.Sqrt(sum / count);
        }

        public override List<string[]> ComputeDeviation(double[] toVal, double[] forVal, double[] toInvalid, double[] forInvalid, int maxIntervalCount)
        {
            if (toVal == null || forVal == null || toVal.Length < 1 || forVal.Length < 1 || toVal.Length != forVal.Length)
                return null;
            Dictionary<double, int> devSet = new Dictionary<double, int>();
            double difference;
            int length = toVal.Length;
            List<string[]> listRow = new List<string[]>();
            for (int i = 0; i < length; i++)
            {
                if (IsInvalidValue(toVal[i], toInvalid))
                    continue;
                if (IsInvalidValue(forVal[i], forInvalid))
                    continue;
                difference = Math.Round(toVal[i] - forVal[i],2);
                if (devSet.ContainsKey(difference))
                    devSet[difference] += 1;
                else
                    devSet.Add(difference, 1);
            }
            if (devSet.Keys.Count < maxIntervalCount)
            {
                foreach (short key in devSet.Keys)
                {
                    string[] row = new string[] { key.ToString(), devSet[key].ToString() };
                    listRow.Add(row);
                }
            }
            else
            {
                double minValue = Math.Round(devSet.Keys.ToArray().Min(), 2);
                double maxValue = Math.Round(devSet.Keys.ToArray().Max(), 2);
                int step = (int)(Math.Ceiling((double)(maxValue - minValue + 1) / maxIntervalCount));
                int[] stepValueCount = new int[maxIntervalCount];   //存放每个区间的个数
                foreach (float key in devSet.Keys)
                {
                    int index = (int)(key - minValue) / step;
                    stepValueCount[index] += devSet[key];
                }
                for (int j = 1; j < maxIntervalCount; j++)   //[start,end)
                {
                    listRow.Add(new string[] { (minValue + step * (j - 1)) + "~" + (minValue + step * j), stepValueCount[j - 1].ToString() });
                }
            }
            return listRow;
        }
    }
}
