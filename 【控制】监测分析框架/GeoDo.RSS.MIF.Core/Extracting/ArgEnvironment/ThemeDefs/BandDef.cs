using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
   public class BandDef
    {
       /// <summary>
       /// 波段名称
       /// </summary>
       public string Identify;
       /// <summary>
       /// 波段号
       /// </summary>
       public int BandNo = -1;
       /// <summary>
       /// 波长
       /// </summary>
       public float[] Wavelength;
       /// <summary>
       /// 波段类型
       /// </summary>
       public string BandType;

       public double Zoom { get; set; }

       public string FromArgument { get; set; }
       /// <summary>
       /// 中心波数
       /// </summary>
       public double CenterWaveNum;
    }
}
