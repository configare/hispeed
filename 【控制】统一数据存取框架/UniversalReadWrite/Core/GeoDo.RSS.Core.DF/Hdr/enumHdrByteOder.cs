using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public enum enumHdrByteOder
    {
        Host_intel = 0,//小端序，内存中低位字节在高位字节之前
        Network_IEEE = 1//大端序，内存中高位字节在低位字节之前
    }
}
