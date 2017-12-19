using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoDo.RSS.MIF.Core
{
    public class ArgumentDef : ArgumentBase
    {
        /// <summary>
        /// 参数默认值
        /// </summary>
        public string Defaultvalue;

        //从文件中获取
        /// <summary>
        /// 表示参数为复杂类型(例如：文件）,取值：[var|file]
        /// 如果reftype不存在,则参数类型为var
        /// 生成时：
        ///     如果reftype为file,则生成ComboBox + FileOpenButton
        ///          ComboBox中默认为本次会话中符合条件的文件
        /// </summary>
        public string RefType;
        /// <summary>
        /// 表示复杂参数的标识(例如：子产品标识)
        /// </summary>
        public string RefIdentify;
        /// <summary>
        ///选择文件的过滤条件 例如： *.dat,*.ldf
        /// </summary>
        public string RefFilter;
        /// <summary>
        /// 允许多选文件
        /// </summary>
        public bool IsMultiSelect;

        /// <summary>
        /// 参数是可选的
        /// </summary>
        public bool IsOptional = false ;

        /// <summary>
        /// editoruiprovider="assembly:class"
        /// </summary>
        public string EditorUiProvider;
        /// <summary>
        /// 自定义参数编辑器的缺省参数,由EditorUiProvier解析
        /// </summary>
        public XElement DefaultValueElement;
        /// <summary>
        /// 文件提供者(例如：ContextEnvironment:CurrentRasterFile)
        /// </summary>
        public string FileProvider;


        public override string ToString()
        {
            return Defaultvalue;
        }
    }
}
