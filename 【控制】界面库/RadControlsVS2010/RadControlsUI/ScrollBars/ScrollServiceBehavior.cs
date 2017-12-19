using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class ScrollServiceBehavior
    {
        List<ScrollService> list;

        public List<ScrollService> ScrollServices
        {
            get
            {
                return list;
            } 
        }

        public bool IsRunning
        {
            get
            {
                foreach (ScrollService service in list)
                {
                    if (service.IsScrolling)
                    {
                        return true;
                    }
                }
                return false;
            }
        } 

        public ScrollServiceBehavior()
        {
            this.list = new List<ScrollService>();
        }

        public void Add(ScrollService scrollService)
        {
            this.list.Add(scrollService);
        }

        public void Stop()
        {
            foreach (ScrollService service in list)
            {
                service.Stop();
            }
        }

        public void MouseDown(Point location)
        {
            foreach (ScrollService scrollService in list)
            {
                if (scrollService.Owner.ElementState == Telerik.WinControls.ElementState.Loaded)
                {
                    scrollService.MouseDown(location);
                }
            }
        }

        public void MouseUp(Point location)
        {
            foreach (ScrollService scrollService in list)
            {
                if (scrollService.Owner.ElementState == Telerik.WinControls.ElementState.Loaded)
                    scrollService.MouseUp(location);
            }
        }

        public void MouseMove(Point location)
        {
            foreach (ScrollService scrollService in list)
            {
                if (scrollService.Owner.ElementState == Telerik.WinControls.ElementState.Loaded)
                    scrollService.MouseMove(location);
            }
        }
    }
}
