using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.GRIB
{
    public class Parameter
    {
        private int _parameterNo;//参数编号
        private string _name;//参数名称
        private string _description;//参数的描述信息
        private string _unit;//参数单位
        private string _abbrev; //缩写

        public Parameter()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="number">参数编号</param>
        /// <param name="name">参数名称</param>
        /// <param name="description">参数描述信息</param>
        /// <param name="unit">参数单位</param>
        public Parameter(int number, string name, string description, string unit)
        {
            _parameterNo = number;
            _name = name;
            _description = description;
            _unit = unit;
        }

        /// <summary>
        /// 获取设置参数编号
        /// </summary>
        public int ParameterNo
        {
            get { return _parameterNo; }
            set { this._parameterNo = value; }
        }
        /// <summary>
        /// 获取设置参数名称
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// 参数描述信息
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        /// <summary>
        /// 获取设置参数单位
        /// </summary>
        public string Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }

        /// <summary>
        /// 缩写
        /// </summary>
        public string Abbrev
        {
            get { return _abbrev; }
            set { _abbrev = value; }
        }

    }
}
