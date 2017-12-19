using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Collections.Generic;
using System;

namespace Telerik.WinControls
{
    public class ObjectRelation
    {
        #region Fields

        private string name;
        private object list;
        private ObjectRelation parent;
        private ObjectRelationCollecion childRelations;
        private PropertyDescriptorCollection properties;
        private object tag;
        private List<string> childNames = new List<string>();
        
        #endregion

        #region Factory

        public static ObjectRelation GetObjectRelation(object list)
        {
            if (list is BindingSource)
            {
                list = ((BindingSource)list).List;
            }

            if (list is DataTable || list is DataView)
            {
                return new DataSetObjectRelation(list);
            }

            return new ObjectRelation(list);
        }

        public static ObjectRelation GetObjectRelation(object dataSource, string dataMember)
        {
            return ObjectRelation.GetObjectRelation(ListBindingHelper.GetList(dataSource, dataMember));
        }

        #endregion

        #region Constructors

        internal ObjectRelation(object list)
        {
            this.childRelations = new ObjectRelationCollecion();
            this.list = list;

            Initialize();
        }

        internal ObjectRelation(object dataSource, string dataMember)
            : this(ListBindingHelper.GetList(dataSource, dataMember))
        {

        }

        protected virtual void Initialize()
        {
            if (this.list is ITypedList)
            {
                this.properties = ((ITypedList)this.list).GetItemProperties(null);
                this.name = ((ITypedList)this.list).GetListName(null);
            }
            else if(this.list is IEnumerable)
            {
                this.properties = ListBindingHelper.GetListItemProperties(this.list);
                this.name = ListBindingHelper.GetListName(this.list, null);
            }

            BuildChildren(this);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag
        {
            get
            {
                return this.tag;
            }
            set
            {
                this.tag = value;
            }
        }

        public ObjectRelation Parent
        {
            get
            {
                return this.parent;
            }
            protected set
            {
                this.parent = value;
            }
        }

        public PropertyDescriptorCollection Properties
        {
            get
            {
                return this.properties;
            }
            protected set
            {
                this.properties = value;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            protected set
            {
                this.name = value;
            }
        }

        public object List
        {
            get
            {
                return this.list;
            }
        }

        public ObjectRelationCollecion ChildRelations
        {
            get
            {
                return this.childRelations;
            }
        }

        public virtual string[] ParentRelationNames
        {
            get
            {
                return new string[0];
            }
        }

        public virtual string[] ChildRelationNames
        {
            get
            {
                if (this.parent == null)
                {
                    return new string[0];
                }

                if (childNames.Count == 0)
                {
                    for (int i = 0; i < this.parent.Properties.Count; i++)
                    {
                        for (int j = 0; j < this.Properties.Count; j++)
                        {
                            if (this.Properties[j].Name.Equals(this.parent.Properties[i].Name, StringComparison.InvariantCultureIgnoreCase) &&
                               this.Properties[j].PropertyType == this.parent.Properties[i].PropertyType)
                            {
                                childNames.Add(this.Properties[j].Name);
                            }
                        }
                    }
                }

                return childNames.ToArray();
            }
        }

        #endregion

        #region Internal 

        //private IList LoadFromEnumerable()
        //{
        //    IEnumerable e = this.list as IEnumerable;
        //    if (e == null)
        //    {
        //        return null;
        //    }

        //    IEnumerator enumerator = e.GetEnumerator();
        //    List<object> newlist = new List<object>(255);
        //    while (enumerator.MoveNext())
        //    {
        //        newlist.Add(enumerator.Current);
        //    }

        //    return newlist;
        //}

        private void BuildChildren(ObjectRelation parent)
        {
            if (this.properties == null)
            {
                return;
            }

            for (int i = 0; i < properties.Count; i++)
            {
                bool isIList = typeof(IBindingList).IsAssignableFrom(this.properties[i].PropertyType)
                    || typeof(IList).IsAssignableFrom(this.properties[i].PropertyType)
                    || typeof(ITypedList).IsAssignableFrom(this.properties[i].PropertyType)
                    || typeof(IListSource).IsAssignableFrom(this.properties[i].PropertyType);

                if (!isIList && typeof(IEnumerable).IsAssignableFrom(this.properties[i].PropertyType) && this.properties[i].PropertyType.IsGenericType)
                {
                    //this.list = this.LoadFromEnumerable();

                    isIList = !(list is string);
                }
                
                if (isIList)//(IEnumerable).IsAssignableFrom(this.properties[i].PropertyType)
                {
                    IEnumerable values = this.list as IEnumerable;
                    if (values == null) continue;

                    ObjectRelation child = null;
                    IEnumerator e = values.GetEnumerator();
                    if (e.MoveNext())
                    {
                        child = new ObjectRelation(properties[i].GetValue(e.Current));
                    }
                    else
                    {
                        child = new ObjectRelation(properties[i].PropertyType);
                    }

                    //ObjectRelation child = new ObjectRelation(properties[i].PropertyType);
                    child.childNames.Add(this.properties[i].Name);
                    child.parent = this;
                    this.childRelations.Add(child);
                }
            }
        }

        #endregion
    }
}
