#region Version Info
/*========================================================================
* 功能概述：标记
* 
* 创建者：luozhanke     时间：2013-08-16 13:53:50
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
using GeoDo.RasterProject;
using System.IO;

namespace GeoDo.Smart.Tools.NSMCDataCoordCorrect
{
    /// <summary>
    /// 类名：CodeToCoordDef
    /// 属性描述：
    ///   提供读取地理编码文件获得地理编码信息的功能。
    ///   如LST、NVI分块数据编码信息表LSTCodeToCoordDic.txt、NVICodeToCoordDic.txt
    ///   编码文件格式如下：
    ///   编码,minx,maxx,miny,maxy
    /// 创建者：luozhanke   创建日期：2013-08-16 13:53:50
    /// 
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class CodeToCoordDef
    {
        private string _identify;
        private Dictionary<string, PrjEnvelope> _dic;

        public CodeToCoordDef()
        {
            string codeFile = "CodeToCoordDic.txt";
            _identify = codeFile;
            string dicfile = System.AppDomain.CurrentDomain.BaseDirectory + @"SystemData\" + codeFile;
            if (File.Exists(dicfile))//坐标编码文件不存在。
                _dic = CodeToCoordDef.GetDic(dicfile);
        }

        /// <summary>
        /// 通过文件获取地理编码信息表
        /// </summary>
        /// <param name="dicFilename">编码文件</param>
        /// <returns></returns>
        internal static Dictionary<string, PrjEnvelope> GetDic(string dicFilename)
        {
            using (FileStream fs = new FileStream(dicFilename, FileMode.Open))
            {
                return GetDic(fs);
            }
        }

        /// <summary>
        /// 通过资源文件获取地理编码信息表
        /// </summary>
        /// <param name="dicfs">编码文件流</param>
        /// <returns></returns>
        internal static Dictionary<string, PrjEnvelope> GetDic(Stream dicfs)
        {
            Dictionary<string, PrjEnvelope> dic = new Dictionary<string, PrjEnvelope>();
            using (StreamReader br = new StreamReader(dicfs))
            {
                string name;
                double minx, maxx, miny, maxy;
                string linetext = null;
                while ((linetext = br.ReadLine()) != null)
                {
                    if (linetext.StartsWith("#"))
                        continue;
                    string[] splits = linetext.Split(',');
                    if (splits.Length != 5)
                        continue;
                    name = splits[0];
                    if (!double.TryParse(splits[1], out minx))
                        continue;
                    if (dic.ContainsKey(name))//编码重复
                        continue;
                    if (!double.TryParse(splits[2], out maxx))
                        continue;
                    if (!double.TryParse(splits[3], out miny))
                        continue;
                    if (!double.TryParse(splits[4], out maxy))
                        continue;
                    dic.Add(name, new PrjEnvelope(minx, maxx, miny, maxy));
                }
            }
            return dic;
        }

        public string Identify
        {
          get { return _identify; }
        }

        public PrjEnvelope Find(string code)
        {
            if(_dic.ContainsKey(code))
                return _dic[code];
            return null;
        }
    }
}
