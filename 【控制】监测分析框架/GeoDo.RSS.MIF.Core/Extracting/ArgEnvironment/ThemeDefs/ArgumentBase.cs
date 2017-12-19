using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public abstract class ArgumentBase
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 参数描述
        /// </summary>
        public string Description;
        /// <summary>
        /// 参数类型
        /// </summary>
        public string Datatype;
        /// <summary>
        /// 参数最小值
        /// </summary>
        public string MinValue;
        /// <summary>
        /// 参数最大值
        /// </summary>
        public string MaxValue;

        public bool Visible = true;

        /// <summary>
        /// 是否可在算法间共享 
        /// 如果为true，则在切换算法时候，会保留该参数及其值。
        /// 默认值false。
        /// </summary>
        public bool IsAlgorithmShare = false;

        /// <summary>
        /// 参数值发生改变时候是否通知事件
        /// </summary>
        public bool IsEventNotification = true;
    }
}
