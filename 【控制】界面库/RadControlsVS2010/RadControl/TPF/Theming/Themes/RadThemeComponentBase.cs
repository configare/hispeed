using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Xml;
using Telerik.WinControls.Themes;
using Telerik.WinControls.Themes.Design;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Telerik.WinControls.Design;

namespace Telerik.WinControls
{
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public class XmlThemeList : List<XmlTheme>
    {
        public XmlThemeList()
        {
        }

        public XmlThemeList(int capacity)
            : base(capacity)
        {
        }
    }

    /// <summary>
    /// Class used by RadThemeManager to recognize classes that load themes from resources in a class library
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [System.ComponentModel.ToolboxItem(false)]
    [Designer(DesignerConsts.RadThemeComponentBaseDesignerString, typeof(IRootDesigner))]
    public class RadThemeComponentBase: Component
    {
        internal XmlThemeList serializableThemes = new XmlThemeList(3);

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual void DeserializeTheme()
        {
            if (this.serializableThemes != null)
            {
                foreach(XmlTheme theme in this.serializableThemes)
                {
                    Theme.Deserialize(theme);
                }
            }
        }


        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#if !DEBUG
        [Browsable(false)]
#endif
        public virtual XmlThemeList SerializableThemes
        {
            get { return serializableThemes; }
        }        
    }
}
