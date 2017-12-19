using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.Comm
{
    public class MonitorShowSettings
    {
        public static MonitorShowSettings CurrentSettings;
        public bool NeedSaveSettings = false;
        public bool IsOrigResolution = true;
        public bool IsOutputBinImage = true;
        public bool IsOutputGrid = false;
        public bool IsOutputVector = true;
        public enumOutputRegion OutputRegion = enumOutputRegion.AllView;
        public string regionName = string.Empty;

        public enum enumOutputRegion
        {
            AllView,
            CurrentView,
            SomeRegion
        }


    }
}
