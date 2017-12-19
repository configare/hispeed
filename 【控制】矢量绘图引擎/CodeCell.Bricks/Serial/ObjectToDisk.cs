using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.IO;

namespace CodeCell.Bricks.Serial
{
    public static class ObjectToDisk
    {

        public static void SerializeClassToBinary(object obj, string sFileName)
        {
            using (Stream f = File.Open(sFileName, FileMode.Create))
            {
                IFormatter formatter = (IFormatter)new BinaryFormatter();
                formatter.Serialize(f, obj);
                f.Close();
            }
        }

        public static object DerializeClassFromBinary(string sFileName)
        {
            using(Stream f = File.Open(sFileName, FileMode.Open))
            {
                IFormatter formatter = (IFormatter)new BinaryFormatter();
                return formatter.Deserialize(f);
            }
        }
    }
}
