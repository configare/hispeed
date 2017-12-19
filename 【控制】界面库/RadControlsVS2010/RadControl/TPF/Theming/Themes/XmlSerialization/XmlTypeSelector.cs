using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Drawing.Design;

namespace Telerik.WinControls
{
    public class XmlTypeSelector : XmlSelectorBase
    {
        private string elementType;

        public XmlTypeSelector()
        {
        }
        public XmlTypeSelector(string elementType)
        {
            this.elementType = elementType;
        }

        protected override IElementSelector CreateInstance()
        {
            Type type = XmlTheme.DeserializeType(this.ElementType);
            return new TypeSelector(type);
        }

        [XmlAttribute]
        [Editor(typeof(ElementTypeEditor), typeof(UITypeEditor))]
        public string ElementType
        {
            get { return elementType; }
            set { elementType = value; }
        }

        public override string ToString()
        {
            return "TypeSelector";
        }

        public override bool Equals(object obj)
        {
            if (obj is XmlTypeSelector)
            {
                XmlTypeSelector selector = obj as XmlTypeSelector;
                if (selector.elementType == elementType)
                    return true;
            }
            
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ElementTypeEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            TypeTreeForm form = new TypeTreeForm(false, value);
            if (form.ShowDialog() == DialogResult.OK)
                if (form.SelectedTag != null)
                    return (form.SelectedTag as Type).FullName;

            return value;
        }
    }
}
