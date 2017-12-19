using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.UI
{
    /// <summary>
    /// 算法切换
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="algDef"></param>
    public delegate void OnAlgorithmChangedHandler(object sender, AlgorithmDef algDef);
    /// <summary>
    /// 参数值改变中
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="arg"></param>
    public delegate void OnArgumentValueChangingHandler(object sender, IArgumentProvider arg);
    /// <summary>
    /// 参数值改变后
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="arg"></param>
    public delegate void OnArgumentValueChangedHandler(object sender, IArgumentProvider arg);
    /// <summary>
    /// 感兴趣模版改变
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="aois"></param>
    public delegate void OnAOITempleteChangedHandler(object sender, string[] aois);

}
