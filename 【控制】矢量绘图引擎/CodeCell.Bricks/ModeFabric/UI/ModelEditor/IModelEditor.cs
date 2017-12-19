using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace CodeCell.Bricks.ModelFabric
{
    public delegate void OnScaleChangedHandler(object sender,float scale);
    public delegate void OnExtentChangedHandler(object sender,RectangleF extent);

    public interface IModelEditor:IDisposable
    {
        Control Container { get; }
        float Scale { get; set; }
        RectangleF FullExtent { get; }
        RectangleF Extent { get; set; }
        Color BackColor { get; set; }
        void Render();
        Point ToViewCoord(Point containerPixelCoord);
        IBindingEnvironment BindingEnvironment { get; }
        OnScaleChangedHandler OnScaleChanged { get; set; }
        OnExtentChangedHandler OnExtentChanged { get; set; }
        void ToScriptFile(string filename);
        void LoadScriptFile(string filename);
    }
}
