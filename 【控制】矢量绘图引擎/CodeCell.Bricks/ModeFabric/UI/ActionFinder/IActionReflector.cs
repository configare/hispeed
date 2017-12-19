using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CodeCell.Bricks.ModelFabric
{
    public interface IActionReflector : IDisposable
    {
        void AddScanDir(string dir,SearchOption searchOption);
        void AddScanAssembly(string assemblyUrl);
        ActionInfo[] Reflector();
    }
}
