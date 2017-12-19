using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace GeoDo.RSS.Core.DrawEngine
{
    public class Layer : ILayer
    {
        protected string _name = string.Empty;
        protected string _alias = string.Empty;
        protected bool _enabled = true;

        public Layer()
        {
        }

        #region ILayer 成员

        [DisplayName("名字"),Category("基本信息")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [DisplayName("别名"), Category("基本信息")]
        public string Alias
        {
            get { return _alias; }
            set { _alias = value; }
        }

        [DisplayName("是否可用"),Category("状态")]
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        #endregion

        #region IDisposable 成员

        public virtual void Dispose()
        {
        }

        #endregion
    }
}
