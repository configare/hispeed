using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    /// <summary>
    /// 约定：为NULL表示该参数不起作用,string.Empty作为无效参数
    /// </summary>
    public class ExtractAlgorithmIdentify
    {
        public string Satellite;
        public string Sensor;
        public string Resolution;
        public string CustomIdentify;
    }
}
