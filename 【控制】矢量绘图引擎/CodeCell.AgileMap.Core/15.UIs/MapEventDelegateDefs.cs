using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public delegate void OnMapScaleChangedHandler(object sender,int scale);
    public delegate void OnViewExtentChangedHandler(object sender,Envelope envelope);
}
