using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.GeoCorrection
{
    [Export(typeof(ICommand))]
    public class CommandGeoCorrection : GeoDo.RSS.Core.UI.Command
    {
        public CommandGeoCorrection()
        {
            _id = 40001;
            _name = "GeoCorrection";
            _text = "几何精校正";
            _toolTip = "几何精校正";
        }


        public override void Execute()
        {
            Execute("");
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="argument">
        /// 格式"*#*"冒号前面部分表示文件名称，冒号后面部分表示所选投影坐标的Wkt格式字符串
        /// </param>
        public override void Execute(string argument)
        {
            ISmartWindow smartWindow = _smartSession.SmartWindowManager.GetSmartWindow((w) => { return w.GetType() == typeof(GeoCorrectionPage); });
            if (smartWindow == null)
            {
                smartWindow = new GeoCorrectionPage(_smartSession);
                _smartSession.SmartWindowManager.DisplayWindow(smartWindow);
            }
            else
                _smartSession.SmartWindowManager.DisplayWindow(smartWindow);
        }
    }
}
