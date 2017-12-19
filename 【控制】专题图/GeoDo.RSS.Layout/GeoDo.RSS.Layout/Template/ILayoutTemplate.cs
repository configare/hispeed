using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Layout
{
    public interface ILayoutTemplate:IDisposable
    {
        /// <summary>
        /// 模版的完整路径
        /// </summary>
        string FullPath { get; set; }
        string Name { get; set; }
        string Text { get; set; }
        ILayout Layout { get; }
        Bitmap GetOverview(Size size);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fname">完整路径</param>
        void SaveTo(string fname);
        void ApplyVars(Dictionary<string, string> vars);
    }
}
