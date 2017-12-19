using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;

namespace Telerik.WinControls
{
    public class DataSetObjectRelation : ObjectRelation
    {
        private List<string> childRelationNames, parentRelationNames;

        internal DataSetObjectRelation(object list)
            :base(list)
        {
            
        }

        internal DataSetObjectRelation(object dataSource, string dataMember)
            :this(ListBindingHelper.GetList(dataSource, dataMember))
        {
           
        }

        public override string[] ParentRelationNames
        {
            get
            {
                return this.parentRelationNames.ToArray();
            }
        }

        public override string[] ChildRelationNames
        {
            get
            {
                return this.childRelationNames.ToArray();
            }
        }

        protected override void Initialize()
        {
            this.parentRelationNames = new List<string>();
            this.childRelationNames = new List<string>();

            DataTable table = this.List as DataTable;
            if (table == null && this.List is DataView)
            {
                table = ((DataView)this.List).Table;
            }

            if (table == null)
            {
                return;
            }

            this.Properties = ListBindingHelper.GetListItemProperties(table);
            this.Name = table.TableName;

            for (int i = 0; i < table.ChildRelations.Count; i++)
            {
                DataTable childTable = table.ChildRelations[i].ChildTable;
                
                if (childTable == table)
                {
                    this.ChildRelations.Add(new SelfReferenceRelation(childTable, 
                        table.ChildRelations[i].ParentColumns[0].ColumnName, 
                        table.ChildRelations[i].ChildColumns[0].ColumnName));
                }
                else
                {
                    DataSetObjectRelation child = new DataSetObjectRelation(childTable);
                    child.Parent = this;

                    for (int j = 0; j < table.ChildRelations[i].ParentColumns.Length; j++)
                    {
                        child.parentRelationNames.Add(table.ChildRelations[i].ParentColumns[j].ColumnName);
                    }

                    for (int j = 0; j < table.ChildRelations[i].ChildColumns.Length; j++)
                    {
                        child.childRelationNames.Add(table.ChildRelations[i].ChildColumns[j].ColumnName);
                    }
                    this.ChildRelations.Add(child);
                }
            }
        }
    }
}
