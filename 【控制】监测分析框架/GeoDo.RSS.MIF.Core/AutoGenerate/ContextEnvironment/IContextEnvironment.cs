using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public interface IContextEnvironment
    {
        /// <summary>
        /// GeoDo.RSS.Core.UI.ISmartSession
        /// </summary>
        object Session { get; set; }
        void PutContextVar(string varIdentify, string varValue);
        void Reset();
        string GetContextVar(string varIdentify);
    }
}
