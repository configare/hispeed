using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.ModelFabric;
using MySql.Data.MySqlClient;
using System.Data;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    public class ArgDBConnection : ArgRefType
    {
        public override object GetValue(object sender)
        {
            using (frmNewSpatialDbConn frm = new frmNewSpatialDbConn())
            {
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string connString = frm.GetConnString();
                    return DbConnectionFactory.CreateDbConnection(connString);
                }
            }
            return null;
        }

        public override string ToString(object value)
        {
            return (value as IDbConnection).ConnectionString;
        }

        public override bool TryParse(string text, out object value)
        {
            return base.TryParse(text, out value);
        }

        public override bool IsNeedInput
        {
            get
            {
                return false;
            }
        }
    }
}
