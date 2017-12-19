using System.Collections.Generic;
using System.Linq;

namespace GeoDo.Smart.Tools.MODLSTPro
{
    public class DataFields : List<DataField>
    {
        private DataFields()
        {

        }

        public DataFields(Pair pair)
        {
            foreach (Pair dPair in pair.Pairs)
            {
                Add(new DataField(dPair.KeyValues));
            }
        }

        public bool IsSameDataFields(DataFields dataFields)
        {
            if (dataFields == null)
                return false;
            if (Count != dataFields.Count)
                return false;

            for (int i = 0; i < Count; i++)
            {
                if (!this[i].IsSameDataField(dataFields[i]))
                    return false;
            }
            return true;
        }


        public DataFields Clone()
        {
            var dataFields = new DataFields();
            dataFields.AddRange(this.Select(dataField => dataField.Clone()));
            return dataFields;
        }
    }
}