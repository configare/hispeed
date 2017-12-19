using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;

namespace Telerik.WinControls.Themes.Design
{
    ///<exclude/>
    public class RadSpinControlDesignTimeData : RadControlDesignTimeData
    {
        public RadSpinControlDesignTimeData()
        { }

        public RadSpinControlDesignTimeData(string name)
            : base(name)
        { }

        public override ControlStyleBuilderInfoList GetThemeDesignedControls(Control previewSurface)
        {
            RadSpinEditor control = new RadSpinEditor();
            int i;

            i = control.SpinElement.Children.Count;
            // PATCH - See the fix of Boyko in RadElement.Propagate[Suspend/Resume]Layout()
            /*control.BeginInit();
            control.Bounds = new System.Drawing.Rectangle(0, 0, 100, 15);
            control.EndInit();*/

            RadSpinEditor controlStructure = new RadSpinEditor();


            i = controlStructure.SpinElement.Children.Count;
            // PATCH - See the fix of Boyko in RadElement.Propagate[Suspend/Resume]Layout()
            /*controlStructure.BeginInit();
            controlStructure.Bounds = new System.Drawing.Rectangle(0, 0, 100, 15);
            controlStructure.EndInit();*/

            ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(control, controlStructure.RootElement);
            designed.Placemenet = PreviewControlPlacemenet.MiddleCenter;
            designed.MainElementClassName = typeof(RadSpinControlElement).FullName;

            ControlStyleBuilderInfoList res = new ControlStyleBuilderInfoList();
            res.Add(designed);
            
            return res;
        }
    }
}
