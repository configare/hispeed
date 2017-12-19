using System;
using System.Collections.Generic;
using System.Text;

namespace GeoDo.RSS.AppEnv
{
    public interface IStartChecker
    {
        bool IsCanContinue { get; }
        bool IsOK { get; }
        object Data { get; }
        Exception Exception { get; }
        string Message { get; }
    }
}
