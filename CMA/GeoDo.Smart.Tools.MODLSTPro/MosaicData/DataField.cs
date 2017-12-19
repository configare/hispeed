namespace GeoDo.Smart.Tools.MODLSTPro
{
    public class DataField
    {
        private DataField()
        {

        }

        public DataField(KeyValues keyValues)
        {
            DataFieldName = keyValues["DataFieldName"];
            DataType = keyValues["DataType"];
            DimList = keyValues["DimList"];
        }
        public string DataFieldName { get; private set; }
        public string DataType { get; private set; }
        public string DimList { get; private set; }

        public bool IsSameDataField(DataField dataFields)
        {
            if (DataFieldName != dataFields.DataFieldName)
                return false;
            if (DataType != dataFields.DataType)
                return false;
            if (DimList != dataFields.DimList)
                return false;

            return true;
        }

        public DataField Clone()
        {
            var dataField = new DataField { DataFieldName = DataFieldName, DataType = DataType, DimList = DimList };
            return dataField;
        }
    }
}