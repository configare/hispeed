using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    public class MapToolBase:IMapTool
    {
        public MapToolBase()
        { 
        }

        #region IContainerEventHandler 成员

        public void Handle(object sender, enumContainerEventType eventType, object eventArg, ref bool isHandled)
        {
            isHandled = true;
            switch (eventType)
            { 
                case enumContainerEventType.Click:
                    Click(sender as IMapControl, eventArg as MouseEventArgs);
                    break;
                case enumContainerEventType.MouseDown:
                    MouseDown(sender as IMapControl, eventArg as MouseEventArgs);
                    break;
                case enumContainerEventType.MouseMove:
                    MouseMove(sender as IMapControl, eventArg as MouseEventArgs);
                    break;
                case enumContainerEventType.MouseUp:
                    MouseUp(sender as IMapControl, eventArg as MouseEventArgs);
                    break;
                case enumContainerEventType.MouseWheel:
                    MouseWheel(sender as IMapControl, eventArg as MouseEventArgs);
                    break;
                case enumContainerEventType.MouseDoubleClick:
                    MouseDoubleClick(sender as IMapControl, eventArg as MouseEventArgs);
                    break;
                default:
                    {
                        isHandled = false;
                        throw new NotSupportedException("暂不支持类型为\"" + eventType.ToString() + "\"的事件处理。");
                    }
            }
        }

        protected virtual void MouseDoubleClick(IMapControl mapcontrol, MouseEventArgs e)
        {

        }

        protected virtual void Click(IMapControl mapcontrol, MouseEventArgs e)
        {
        }

        protected virtual void MouseDown(IMapControl mapcontrol, MouseEventArgs e)
        {
        }

        protected virtual void MouseMove(IMapControl mapcontrol, MouseEventArgs e)
        {
        }

        protected virtual void MouseUp(IMapControl mapcontrol, MouseEventArgs e)
        {
        }

        protected virtual void MouseWheel(IMapControl mapcontrol, MouseEventArgs e)
        {
        }

        public virtual void Render(RenderArgs arg)
        { 
        }

        #endregion
    }
}
