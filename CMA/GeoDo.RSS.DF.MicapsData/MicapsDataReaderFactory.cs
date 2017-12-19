#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/4 17:11:10
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
using CodeCell.AgileMap.Core;
using System.IO;
using System.Xml.Linq;

namespace GeoDo.RSS.DF.MicapsData
{
    /// <summary>
    /// 类名：MicapsDataReaderFactory
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/9/4 17:11:10
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public static class MicapsDataReaderFactory
    {
        public static IVectorFeatureDataReader GetVectorFeatureDataReader(string filename, params object[] args)
        {
            MicapsDataReader reader = new MicapsDataReader();
            try
            {
                if (reader.TryOpen(filename, null, args))
                    return reader;
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
