using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.ModelFabric;
using System.Windows.Forms;

namespace CodeCell.AgileMap.Components
{
    public abstract class ArgCatalogItem : ArgRefType
    {
        public override bool IsNeedInput
        {
            get
            {
                return false;
            }
        }

        public override object GetValue(object sender)
        {
            using (frmDataSource frmds = new frmDataSource())
            {
                frmds.ucBudGISDataSource1.SelectableType = GetCatalogItemType();
                if (frmds.ShowDialog() == DialogResult.OK)
                {
                    return frmds.ucBudGISDataSource1.SelectedCatalogItem;
                }
            }
            return base.GetValue(sender);
        }

        protected abstract enumCatalogItemType GetCatalogItemType();

        public override string ToString(object value)
        {
            ICatalogItem ci = value as ICatalogItem;
            if (ci is CatalogLocal)
                return (ci as CatalogLocal).Tag.ToString();
            else if (ci is CatalogFile)
                return (ci as CatalogFile).Tag.ToString();
            else if (ci is CatalogDatabaseConn)
                return ((ci as CatalogDatabaseConn).Tag as SpatialDatabaseConn).ConnectionString;
            else if (ci is CatalogFeatureDataset)
            {
                SpatialFeatureDataset sfd = (ci as CatalogFeatureDataset).Tag as SpatialFeatureDataset;
                return (sfd as CatalogEntityBase)._connString + "@" + sfd.Name;
            }
            else if (ci is CatalogFeatureClass)
            {
                SpatialFeatureClass sfc = (ci as CatalogFeatureClass).Tag as SpatialFeatureClass;
                return (sfc as CatalogEntityBase)._connString + "@" + sfc.Name;
            }
            else
                return base.ToString();
        }
    }
}
