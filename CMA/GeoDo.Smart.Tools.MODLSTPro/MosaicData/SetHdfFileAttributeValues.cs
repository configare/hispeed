using HDF5DotNet;
using GeoDo.HDF5;

namespace GeoDo.Smart.Tools.MODLSTPro
{
    public static class SetHdfFileAttributeValues
    {
        public static void SetHdfFileAttributeValue(H5FileId fileId, HDFDef hDFDef)
        {
            foreach (HDFAttributeDef hdfAttributeDef in hDFDef.AttCollection.Attributes)
            {
                WriteHdfAttributes.WriteHdfAttribute(fileId, hdfAttributeDef);
            }
        }
    }
}
