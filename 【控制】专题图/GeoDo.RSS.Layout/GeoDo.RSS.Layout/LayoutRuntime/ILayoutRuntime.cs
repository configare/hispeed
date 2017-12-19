using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Layout
{
    public interface ILayoutRuntime:ILayoutCoordinateTransform,IDisposable
    {
        float Scale { get; set; }
        void ApplyOffset(int offsetX, int offsetY);
        void ResetOffets();
        void Render(IDrawArgs drawArgs);
        ILayout Layout { get; }
        ILayoutHost GetHost();
        void ChangeLayout(ILayout layout);
        void UpdateMatrix();
        IElement[] Selection { get; }
        void ClearSelection();
        IElement[] LockedElements { get; }
        IElement[] Hit(float layoutX, float layoutY);
        IElement[] Hit(RectangleF layoutRect);
        ISelectedEditBoxManager SelectedEditBoxManager { get; }
        IElement[] QueryElements(Func<IElement, bool> filter,bool returnOnlyOne);
    }
}
