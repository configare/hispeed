using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CodeCell.AgileMap.Components
{
    public abstract class CatalogEntityBase:ICatalogEntity
    {
        private string _id = null;
        private string _name = null;
        private string _description = null;
        private string _newId = null ;
        internal string _connString = null;

        public CatalogEntityBase()
        {
            _id = Guid.NewGuid().ToString();
            _newId = _id ;
        }

        public string ConnString
        {
            get { return _connString; }
        }

        #region ICatalogEntity Members

        [AttToFieldMap("Id", typeof(string))]
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [AttToFieldMap("Name", typeof(string))]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [AttToFieldMap("Description", typeof(string))]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public void Store()
        {
            using (ICatalogEntityClass cec = GetCatalogEntityClass())
            {
                if (cec.IsExist(this))
                    cec.Update(this);
                else
                    cec.Insert(this);
            }
        }

        protected abstract ICatalogEntityClass GetCatalogEntityClass();

        public void Refresh()
        { 
        }

        public virtual bool IsEmpty()
        {
            return (_newId == _id || string.IsNullOrEmpty(_id)) && _name == null && _description == null;
        }

        #endregion
    }
}
