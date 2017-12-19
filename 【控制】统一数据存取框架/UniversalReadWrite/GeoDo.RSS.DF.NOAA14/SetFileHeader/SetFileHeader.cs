#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/8/21 15:44:34
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

namespace GeoDo.RSS.DF.NOAA14.SetFileHeader
{
    /// <summary>
    /// 类名：SetFileHeader
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/8/21 15:44:34
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public static class SetFileHeader
    {
        public static NA141BHeader Set1BHeader(string filename)
        {
            FileStream fs = null;
            BinaryReader br = null;
            NA141BHeader nA141BHeader = null;
            try
            {
                fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                br = new BinaryReader(fs, Encoding.Default);
                nA141BHeader = new SetNA141BFileHeader().Create(fs, br, 0, 14800) as NA141BHeader;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
                if (br != null)
                    br.Close();
            }
            return nA141BHeader;
        }
    }
}
