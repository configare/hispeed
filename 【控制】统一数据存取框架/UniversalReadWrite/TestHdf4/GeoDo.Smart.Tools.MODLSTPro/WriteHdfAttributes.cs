using System;
using System.Text;
using HDF5DotNet;

namespace GeoDo.Smart.Tools.MODLSTPro
{
    public static class WriteHdfAttributes
    {
        public static void WriteHdfAttribute(H5ObjectWithAttributes fileOrdatasetId, HDFAttributeDef hDFAttributeDef)
        {

            H5DataSpaceId dataSpaceId = H5S.create_simple(1, new long[] { (long)hDFAttributeDef.Size });
            H5DataTypeId dataTypeId = null;
            H5AttributeId attributeId = null;
            try
            {
                switch (Type.GetTypeCode(hDFAttributeDef.Type))
                {
                    case TypeCode.Byte:
                        dataTypeId = H5T.copy(H5T.H5Type.STD_U8BE);
                        attributeId = H5A.create(fileOrdatasetId, hDFAttributeDef.Name, dataTypeId, dataSpaceId);
                        WriteTAttribute<byte>.WriteAttribute(hDFAttributeDef, dataTypeId, attributeId);
                        break;
                    case TypeCode.Char:
                        dataTypeId = H5T.copy(H5T.H5Type.STD_U8BE);
                        attributeId = H5A.create(fileOrdatasetId, hDFAttributeDef.Name, dataTypeId, dataSpaceId);
                        WriteTAttribute<char>.WriteAttribute(hDFAttributeDef, dataTypeId, attributeId);
                        break;
                    case TypeCode.Double:
                        //dataTypeId = H5T.copy(H5T.H5Type.IEEE_F64BE);
                        dataTypeId = H5T.copy(H5T.H5Type.IEEE_F64LE);
                        attributeId = H5A.create(fileOrdatasetId, hDFAttributeDef.Name, dataTypeId, dataSpaceId);
                        WriteTAttribute<double>.WriteAttribute(hDFAttributeDef, dataTypeId, attributeId);
                        break;
                    case TypeCode.Int16:
                        dataTypeId = H5T.copy(H5T.H5Type.STD_I16BE);
                        attributeId = H5A.create(fileOrdatasetId, hDFAttributeDef.Name, dataTypeId, dataSpaceId);
                        WriteTAttribute<Int16>.WriteAttribute(hDFAttributeDef, dataTypeId, attributeId);
                        break;
                    case TypeCode.Int32:
                        dataTypeId = H5T.copy(H5T.H5Type.STD_I32BE);
                        attributeId = H5A.create(fileOrdatasetId, hDFAttributeDef.Name, dataTypeId, dataSpaceId);
                        WriteTAttribute<Int32>.WriteAttribute(hDFAttributeDef, dataTypeId, attributeId);
                        break;
                    case TypeCode.Int64:
                        dataTypeId = H5T.copy(H5T.H5Type.STD_I64BE);
                        attributeId = H5A.create(fileOrdatasetId, hDFAttributeDef.Name, dataTypeId, dataSpaceId);
                        WriteTAttribute<Int64>.WriteAttribute(hDFAttributeDef, dataTypeId, attributeId);
                        break;
                    case TypeCode.Object:
                        dataTypeId = H5T.copy(H5T.H5Type.STD_REF_OBJ);
                        attributeId = H5A.create(fileOrdatasetId, hDFAttributeDef.Name, dataTypeId, dataSpaceId);
                        WriteTAttribute<object>.WriteAttribute(hDFAttributeDef, dataTypeId, attributeId);
                        break;
                    case TypeCode.Single:
                        dataTypeId = H5T.copy(H5T.H5Type.IEEE_F32BE);
                        attributeId = H5A.create(fileOrdatasetId, hDFAttributeDef.Name, dataTypeId, dataSpaceId);
                        WriteTAttribute<Single>.WriteAttribute(hDFAttributeDef, dataTypeId, attributeId);
                        break;
                    case TypeCode.String:
                        dataTypeId = H5T.copy(H5T.H5Type.C_S1);
                        dataSpaceId = H5S.create(H5S.H5SClass.SCALAR);
                        attributeId = WriteStringAttribute(fileOrdatasetId, hDFAttributeDef, dataSpaceId, dataTypeId);
                        break;
                    case TypeCode.UInt16:
                        dataTypeId = H5T.copy(H5T.H5Type.STD_U16BE);
                        attributeId = H5A.create(fileOrdatasetId, hDFAttributeDef.Name, dataTypeId, dataSpaceId);
                        WriteTAttribute<UInt16>.WriteAttribute(hDFAttributeDef, dataTypeId, attributeId);
                        break;
                    case TypeCode.UInt32:
                        dataTypeId = H5T.copy(H5T.H5Type.STD_U32BE);
                        attributeId = H5A.create(fileOrdatasetId, hDFAttributeDef.Name, dataTypeId, dataSpaceId);
                        WriteTAttribute<UInt32>.WriteAttribute(hDFAttributeDef, dataTypeId, attributeId);
                        break;
                    case TypeCode.UInt64:
                        dataTypeId = H5T.copy(H5T.H5Type.STD_U64BE);
                        attributeId = H5A.create(fileOrdatasetId, hDFAttributeDef.Name, dataTypeId, dataSpaceId);
                        WriteTAttribute<UInt64>.WriteAttribute(hDFAttributeDef, dataTypeId, attributeId);
                        break;
                }
            }
            catch (Exception ex)
            {
                int i = 9;
                int j = i;
            }
            finally
            {
                if (attributeId != null)
                    H5A.close(attributeId);
            }
        }

        private static H5AttributeId WriteStringAttribute(H5ObjectWithAttributes fileOrdatasetId, HDFAttributeDef hDFAttributeDef, H5DataSpaceId dataSpaceId, H5DataTypeId dataTypeId)
        {
            string attValue = Convert.ToString(hDFAttributeDef.Value);
            int stringLength = attValue.Length;
            H5T.setSize(dataTypeId, stringLength);
            H5AttributeId attributeId = H5A.create(fileOrdatasetId, hDFAttributeDef.Name, dataTypeId, dataSpaceId);
            byte[] bs = Encoding.Default.GetBytes(attValue);
            H5A.write(attributeId, dataTypeId, new H5Array<byte>(bs));
            return attributeId;
        }
    }

    public class WriteTAttribute<T>
    {
        public static void WriteAttribute(HDFAttributeDef hDFAttributeDef, H5DataTypeId dataTypeId, H5AttributeId attributeId)
        {
            if (hDFAttributeDef.Value == null || hDFAttributeDef.Value.ToString() == "")
                return;
            H5A.write<T>(attributeId, dataTypeId, new H5Array<T>(hDFAttributeDef.Value as T[]));
        }
    }
}
