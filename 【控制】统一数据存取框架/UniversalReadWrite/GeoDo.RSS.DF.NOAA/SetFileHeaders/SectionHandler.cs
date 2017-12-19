using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.DF.NOAA
{
    public class SectionHandler
    {
        public virtual object Create(Stream fileStream, BinaryReader binaryReader, int offset, int endOffset)
        {
            //将流指针抬升
            fileStream.Seek(endOffset, SeekOrigin.Begin);
            return null;
        }
    }
}
