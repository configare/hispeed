using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DrawEngine
{
    public interface IDummyRenderModeSupport
    {
        bool IsDummyModel { get; }
        bool IsEnabledDummyCache { get; set; }
        void SetToDummyRenderMode();
        void SetToNomralRenderMode();
    }
}
