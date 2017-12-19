using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IMapRefresh
    {
        void Render();
        void ReRender();
        void Render(OnRenderIsFinishedHandler finishNotify);
        void ReRender(OnRenderIsFinishedHandler finishNotify);
    }
}
