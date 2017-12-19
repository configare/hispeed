using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Security;
using System.Security.Permissions;
using System.ComponentModel;
using System.Windows.Forms.Design;

namespace Telerik.WinControls.Keyboard
{
    [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.UnmanagedCode), PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
    public class ChordsEditor : UITypeEditor
    {
        // Methods
        public ChordsEditor()
        {
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                IWindowsFormsEditorService service1 = (IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));
                if (service1 == null)
                {
                    return value;
                }
                if (this.chordKeysUI == null)
                {
                    this.chordKeysUI = new ChordKeysUI(this);
                }
                this.chordKeysUI.Start(service1, value);
                service1.DropDownControl(this.chordKeysUI);
				//service1.ShowDialog(this.chordKeysUI);
                if (this.chordKeysUI.Value != null)
                {
					string temp = (this.chordKeysUI.Value as Chord).ChordKeys;
                    value = this.chordKeysUI.Value;
                }
                else if (value is Chord)
                {
                    (value as Chord).Keys = "";
                }
                this.chordKeysUI.End();
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        // Fields
        private ChordKeysUI chordKeysUI;
    }
}
