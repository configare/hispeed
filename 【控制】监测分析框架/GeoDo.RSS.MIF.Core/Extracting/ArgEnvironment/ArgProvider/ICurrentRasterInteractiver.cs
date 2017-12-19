using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    /// <summary>
    /// 判识面板与当前影像窗口交互接口
    /// </summary>
    public interface ICurrentRasterInteractiver
    {
        //do bandValuesNotifier while mouse down
        void StartInteractive(int[] bandNos,Action<double[]> bandValuesNotifier);
        double[] GetBandValuesInAOI(int bandNo);
        double GetAvgBandValueInAOI(int bandNo);
        double GetMinBandValueInAOI(int bandNo);
        double GetMaxBandValueInAOI(int bandNo);
        void StartAOIDrawing(Action drawFinishedNotifiter);
    }
}
