using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Core
{
    public interface IThemeGraphGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataFileName"></param>
        /// <param name="templateName"></param>
        /// <param name="options">例如：ColorTableName=L6Graph</param>
        /// <returns></returns>
        void Generate(string dataFileName, string templateName, int[] aoi, string extInfos,string outputIdentify, params object[] options);
        string Save();
    }
}
