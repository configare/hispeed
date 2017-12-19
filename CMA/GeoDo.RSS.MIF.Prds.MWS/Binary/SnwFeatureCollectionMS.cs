#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2013-11-1 9:08:13
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
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.MWS
{
    /// <summary>
    /// 类名：SnwFeatureCollectionMS
    /// 属性描述：微波积雪判识辅助信息
    /// 创建者：李喜佳  创建日期：2013-11-1 9:08:13
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class SnwFeatureCollectionMS : ICursorDisplayedFeatures
    {
        private Dictionary<int, SnwFeatureMS> _features;
        private string _name;

        public SnwFeatureCollectionMS(string name, Dictionary<int, SnwFeatureMS> features)
        {
            _name = name;
            _features = features;
        }

        public string Name
        {
            get { return _name; }
        }

        public string GetCursorInfo(int pixelIndex)
        {
            if (_features == null)
                return string.Empty;
            if (_features.ContainsKey(pixelIndex))
                return _features[pixelIndex].ToString();
            return string.Empty;
        }

    }
}
