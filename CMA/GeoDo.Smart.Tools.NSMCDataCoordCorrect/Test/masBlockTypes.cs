using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Smart.Tools.NSMCDataCoordCorrect
{
    public enum masBlockTypes
    {
        D05,            //5度分幅
        D10,            //10度分幅
        DXX,            //自由分幅
        DXX_WATER,      //水体分幅
        DXX_ICE,        //海冰分幅
        DXX_Fire,       //火情中国区域5度分幅
        DXX_Drought,    //干旱中国区域固定区域分幅
        DXX_HHBL,       //黄河冰凌固定区域分幅
        DXX_Snow,       //积雪中国区域固定区域分幅
        D00             //无分幅
    }
}
