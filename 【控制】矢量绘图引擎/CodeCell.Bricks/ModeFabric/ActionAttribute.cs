using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.Bricks.ModelFabric
{
    /// <summary>
    /// 每一个Action的静态元数据
    /// </summary>
    public class ActionAttribute:Attribute
    {
        private string _name = null;
        private string _category = "未分类";
        private string _description = null;

        public ActionAttribute(string name, string description)
        {
            _name = name;
            _description = description;
        }

        public ActionAttribute(string name, string category, string description)
            : this(name, description)
        {
            _category = category;
        }

        public string Name
        {
            get { return _name == null ? "未命名" : _name; }
        }

        public string Category
        {
            get { return string.IsNullOrEmpty(_category) ? "未分类" : _category; }
        }

        public string Description
        {
            get { return _description == null ? string.Empty : _description; }
        }
    }
}
