using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;

namespace CodeCell.AgileMap.Core
{
    [Serializable]
    public class RotateFieldDef : IFieldNamesProvider,IPersistable
    {
        private string _rotateField = null;
        private float _offset = 0;
        private string[] _fields = null;

        public RotateFieldDef(string rotateField, float offset,string[] fields)
        {
            _rotateField = rotateField;
            _offset = offset;
            _fields = fields;
        }

        [EditorAttribute(typeof(UIFieldTypeEditor), typeof(UITypeEditor)),DisplayName("角度字段")]        
        public string RotateField
        {
            get { return _rotateField; }
            set { _rotateField = value; }
        }

        [DisplayName("角度偏移"), Description("本系统采用X轴正方向顺时针方向为0度,offset用于修正0度不同的情况。")/*,Browsable(false)*/]
        public float Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        #region IFieldNamesProvider 成员

        [Browsable(false)]
        public string[] Fields
        {
            get { return _fields; }
        }

        #endregion

        public override string ToString()
        {
            return _rotateField == null ? string.Empty : _rotateField.ToString();
        }

        #region IPersistable Members

        public PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("RotateField");
            obj.AddAttribute("rotatefield", _rotateField != null ?_rotateField:string.Empty);
            obj.AddAttribute("angleoffset", _offset.ToString());
            //
            string fields = string.Empty;
            if (_fields != null)
                fields = string.Join(",", _fields);
            obj.AddAttribute("fields", fields);
            return obj;
        }

        #endregion

        public static RotateFieldDef FromXElement(XElement ele)
        {
            if (ele == null)
                return null;
            string field = ele.Attribute("rotatefield").Value;
            string[] fields = null;
            XAttribute att = ele.Attribute("fields");
            if (att != null)
                fields = att.Value.Split(',');
            int angleOffset = int.Parse(ele.Attribute("angleoffset").Value);
            return new RotateFieldDef(field, angleOffset, fields);
        }
    }
}
