using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.HDF5
{
    public static class HDF5Helper
    {
        public static bool IsHdf5(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open,FileAccess.Read,FileShare.Read))
            {
                byte[] buffer = new byte[8];
                try
                {
                    fs.Read(buffer, 0, 8);
                }
                catch (ArgumentOutOfRangeException)
                {
                    return false;
                }
                //HDF5的文件头，共8个字节
                byte[] hdf5 = new byte[8] {137,72,68,70,13,10,26,10 };
                for (int i = 0; i < buffer.Length; i++)
                    if (hdf5[i] != buffer[i])
                        return false;
            }
            return true;
        }

        public static bool IsHdf5(byte[] bytes1024)
        {
            if (bytes1024 == null || bytes1024.Length < 8)
                return false;
            //HDF5的文件头，共8个字节
            byte[] hdf5 = new byte[8] { 137, 72, 68, 70, 13, 10, 26, 10 };
            for (int i = 0; i < hdf5.Length; i++)
                if (hdf5[i] != bytes1024[i])
                    return false;
            return true;
        }
    }
}
