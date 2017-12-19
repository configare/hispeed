using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Design;

namespace Telerik.WinControls
{
    //[XmlInclude(typeof(XmlSelectorBase))]
    public abstract class XmlSelectorBase : XmlElementSelector
    {
        private bool autoUnapply = true;
        private XmlCondition condition = null;
        private XmlCondition unapplyCondition = null;
        private XmlElementSelector childSelector;

        [DefaultValue(null)]
        [Editor(typeof(ConditionEditor), typeof(UITypeEditor))]
        public XmlCondition Condition
        {
            get
            {
                return condition;
            }
            set
            {
                this.condition = value;
            }
        }

        [DefaultValue(null)]
        [Editor(typeof(ConditionEditor), typeof(UITypeEditor))]
        public XmlCondition UnapplyCondition
        {
            get
            {
                return unapplyCondition;
            }
            set
            {
                this.unapplyCondition = value;
            }
        }

        public bool AutoUnapply
        {
            get { return autoUnapply; }
            set { autoUnapply = value; }
        }

        public bool ShouldSerializeAutoUnapply()
        {
            return false;
        }

        public XmlSelectorBase()
        {
        }

        [DefaultValue(null)]
        public XmlElementSelector ChildSelector
        {
            get
            {
                return this.childSelector;
            }
            set
            {
                this.childSelector = value;
            }
        }

        protected override void DeserializeProperties(IElementSelector selector)
        {
            SelectorBase instance = (SelectorBase)selector;
            if (this.Condition != null)
            {
                instance.Condition = this.Condition.Deserialize();
            }

            if (this.UnapplyCondition != null)
            {
                instance.UnapplyCondition = this.UnapplyCondition.Deserialize();
            }

            if (this.ChildSelector != null)
            {
                instance.ChildSelector = this.ChildSelector.Deserialize();
            }

            instance.AutoUnapply = this.AutoUnapply;
        }
    }

    public class ConditionEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            ConditionBuilderForm form = new ConditionBuilderForm((XmlCondition)value);

            if (form.ShowDialog() == DialogResult.OK)
                return form.GetConditionTree();

            return value;
        }
    }
}
