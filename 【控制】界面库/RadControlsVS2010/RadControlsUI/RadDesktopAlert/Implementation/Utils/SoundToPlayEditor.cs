using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Media;
using System.Windows.Forms.Design;

namespace Telerik.WinControls.UI
{
    public class SoundToPlayEditor : UITypeEditor
    {
        #region Methods

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            ListBox availableSounds = new ListBox();
            availableSounds.Items.Add(new SystemSoundItem(SystemSounds.Exclamation, "Asterisk"));
            availableSounds.Items.Add(new SystemSoundItem(SystemSounds.Exclamation, "Exclamation"));
            availableSounds.Items.Add(new SystemSoundItem(SystemSounds.Exclamation, "Hand"));
            availableSounds.Items.Add(new SystemSoundItem(SystemSounds.Exclamation, "Beep"));
            availableSounds.Items.Add(new SystemSoundItem(SystemSounds.Exclamation, "Question"));

            IWindowsFormsEditorService service = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

            service.DropDownControl(availableSounds);

            if (availableSounds.SelectedIndex == -1)
            {
                return null;
            }

            return ((SystemSoundItem)availableSounds.SelectedItem).Sound;
        }

        #endregion
    }

    public class SystemSoundItem
    {
        #region Fields

        public SystemSound Sound;
        public string SoundName;

        #endregion

        #region Ctor

        public SystemSoundItem(SystemSound sound, string soundName)
        {
            this.Sound = sound;
            this.SoundName = soundName;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return SoundName;
        }

        #endregion
    }
}
