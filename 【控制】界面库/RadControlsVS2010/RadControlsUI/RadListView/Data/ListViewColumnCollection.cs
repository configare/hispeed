using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Data;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Design;

namespace Telerik.WinControls.UI
{
    [Serializable()]
    [Editor(DesignerConsts.ListViewColumnCollectionDesignerString, typeof(UITypeEditor))]
    public class ListViewColumnCollection : ObservableCollection<ListViewDetailColumn>
    {
        #region Fields
        
        private RadListViewElement owner;
        private HybridDictionary columnNames;

        #endregion

        #region Constructors

        public ListViewColumnCollection(RadListViewElement owner)
        {
            this.owner = owner;
            this.columnNames = new HybridDictionary(true);
        }

        #endregion

        #region Properties

        public RadListViewElement Owner
        {
            get { return owner; }
        }

        public ListViewDetailColumn this[string columnName]
        {
            get
            {
                return columnNames[columnName] as ListViewDetailColumn;
            }
        }

        #endregion

        #region Methods

        public void Add(string name)
        {
            this.Add(name, name);
        }
        
        public void Add(string name, string headerText)
        {
            ListViewDetailColumn column = new ListViewDetailColumn(name, headerText); 
            
            this.Add(column);
        }
        
        public void Remove(string columnName)
        {
            int index = this.IndexOf(columnName);
            if (index >= 0)
            {
                this.RemoveAt(index);
            }
        }

        public bool Contains(string columnName)
        {
            if (string.IsNullOrEmpty(columnName))
            {
                return false;
            }

            return this.columnNames.Contains(columnName);
        }

        public int IndexOf(string columnName)
        {
            if (this.Contains(columnName))
            {
                for (int i = 0; i < this.Count; i++)
                {
                    if (this[i].Name.Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        public virtual void AddRange(params ListViewDetailColumn[] listViewColumns)
        {
            this.BeginUpdate();

            for (int i = 0; i < listViewColumns.Length; i++)
            {
                this.Add(listViewColumns[i]);
            }

            this.EndUpdate();
        }

        public void Rename(string name, string newName)
        {
            if (columnNames.Contains(name))
            {
                ListViewDetailColumn col = this[name];
                columnNames.Remove(name);
                columnNames.Add(newName, col);

                int index = this.owner.FilterDescriptors.IndexOf(name);
                if (index >= 0)
                {
                    this.owner.FilterDescriptors[index].PropertyName = newName;
                }
            }
        }

        #endregion

        #region Overrides

        protected override void InsertItem(int index, ListViewDetailColumn column, Action<ListViewDetailColumn> approvedAction)
        {
            this.columnNames.Add(column.Name, column);
            column.Owner = this.owner;
            base.InsertItem(index, column, approvedAction);
        }

        protected override void RemoveItem(int index)
        {
            ListViewDetailColumn column = base[index];
            base.RemoveItem(index);
            columnNames.Remove(column.Name);
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            columnNames.Clear();
        }

        #endregion
    }
}
