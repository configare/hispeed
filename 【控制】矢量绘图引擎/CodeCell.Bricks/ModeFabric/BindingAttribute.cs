using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.Bricks.ModelFabric
{
    public enum enumBindingDirection
    {
        None,             //非绑定参数
        Input ,            //输入绑定参数
        Output,          //输出绑定参数
        InputOutput   //既是输入绑定参数又是输出绑定参数
    }

    public class BindingAttribute:Attribute
    {
        protected string _name = string.Empty;
        protected string _description = string.Empty;
        protected enumBindingDirection _direction = enumBindingDirection.None;
        protected Type _semanticType = null;
        protected Type _dataType = null;
        protected bool _isStrongBinding = false;

        public BindingAttribute(string name, enumBindingDirection direction, Type semanticType,bool isStrongBinding)
        {
            _name = name;
            _direction = direction;
            _semanticType = semanticType;
            _isStrongBinding = isStrongBinding;
        }

        public BindingAttribute(string name, enumBindingDirection direction, Type semanticType, Type dataType,bool isStrongBinding)
        {
            _name = name;
            _direction = direction;
            _semanticType = semanticType;
            _isStrongBinding = isStrongBinding;
            _dataType = dataType;
        }

        public BindingAttribute(string name, string description, enumBindingDirection direction, Type semanticType, bool isStrongBinding)
            : this(name, direction, semanticType, isStrongBinding)
        {
            _description = description;
        }

        public BindingAttribute(string name, string description, enumBindingDirection direction, Type semanticType, Type dataType,bool isStrongBinding)
            : this(name, direction, semanticType, isStrongBinding)
        {
            _description = description;
            _dataType = dataType;
        }

        /// <summary>
        /// 参数的名称
        /// 例如：待投影文件名
        /// </summary>
        public string Name 
        {
            get { return string.IsNullOrEmpty(_name) ? ToString() : _name; }
        }

        /// <summary>
        /// 参数的描述
        /// 例如：HDF格式的待投影文件名
        /// </summary>
        public string Description
        {
            get { return string.IsNullOrEmpty(_description) ? ToString() : _description; }
        }

        /// <summary>
        /// 绑定方向
        /// </summary>
        public enumBindingDirection Direction
        {
            get { return _direction; }
        }

        /// <summary>
        /// 语义上的类型
        /// 例如：文件名(注意与字符串的区别)
        /// </summary>
        public Type SemanticType
        {
            get { return _semanticType; }
        }

        public Type DataType
        {
            get { return _dataType; }
        }

        /// <summary>
        /// 允许绑定语义类型的父类型
        /// 例如：某个Action的Filename属性的绑定语义类型为[File]
        ///          [OriginalFile]与[ProjectedFile]为[File]的派生类(即子类)
        ///          那么，如果AllowBindParentType = True
        ///          则可以将语义类型为[OriginalFile]的属性绑定到该Action的Filename属性,否则不行
        /// </summary>
        public bool IsStrongBinding
        {
            get { return _isStrongBinding; }
        }

        public ISemanticTypeEditor GetEditor()
        {
            return Activator.CreateInstance(_semanticType) as ISemanticTypeEditor;
        }
    }
}
