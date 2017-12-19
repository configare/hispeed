#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/8/21 16:16:36
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

namespace GeoDo.RSS.DF.NOAA14
{
    /// <summary>
    /// 类名：SectionHandler
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/8/21 16:16:36
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class SectionHandler
    {
        public virtual object Create(Stream fileStream, BinaryReader binaryReader, int offset, int endOffset)
        {
            //将流指针抬升
            fileStream.Seek(endOffset, SeekOrigin.Begin);
            return null;
        }
    }
}
