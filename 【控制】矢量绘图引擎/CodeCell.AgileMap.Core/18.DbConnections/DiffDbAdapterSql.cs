using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    internal class DiffDbAdapterSql : DiffDbAdapter
    {
        protected override string GetFieldTypeFromINT64()
        {
            throw new NotImplementedException();
        }

        protected override string GetFieldTypeFromINT16()
        {
            throw new NotImplementedException();
        }

        protected override string GetFieldTypeFromINT32()
        {
            throw new NotImplementedException();
        }

        public override string GetGeometryFieldDef()
        {
            throw new NotImplementedException();
        }

        public override string DateTimeToSql(DateTime dateTime)
        {
            throw new NotImplementedException();
        }
    }
}
