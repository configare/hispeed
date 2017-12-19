using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;

namespace Telerik.WinControls.Themes.Design
{
    ///<exclude/>
    public class RadSpinControlElement : RadControlDesignTimeData
    {
        public RadSpinControlElement()
        { }

        public RadSpinControlElement(string name)
            : base(name)
        { }

        public override ControlStyleBuilderInfoList GetThemeDesignedControls(Control previewSurface)
        {
            ControlStyleBuilderInfoList res = new ControlStyleBuilderInfoList();
            
            // ComboBox
            RadSpinEditor spinControlElement = new RadSpinEditor();

            RadSpinEditor spinControlElementStructure = new RadSpinEditor();
            
            ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(spinControlElement, spinControlElementStructure.RootElement);
            designed.Placemenet = PreviewControlPlacemenet.MiddleCenter;
            designed.MainElementClassName = typeof(RadComboBoxElement).FullName;
            res.Add(designed);                     
            

            return res;
        }
    }
}
