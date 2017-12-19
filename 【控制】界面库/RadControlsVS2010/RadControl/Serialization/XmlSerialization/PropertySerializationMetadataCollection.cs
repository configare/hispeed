using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Telerik.WinControls.Data;

namespace Telerik.WinControls.XmlSerialization
{
    public class PropertySerializationMetadataCollection : ObservableCollection<PropertySerializationMetadata>
    {
        public void Add(string objectTypeFullName, string propertyName, params Attribute[] attributes)
        {
            this.Add(new PropertySerializationMetadata(objectTypeFullName, propertyName, attributes));
        }

        public void Add(Type objectType, string propertyName, params Attribute[] attributes)
        {
            this.Add(new PropertySerializationMetadata(objectType, propertyName, attributes));
        }

        protected override bool OnCollectionChanging(NotifyCollectionChangingEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add ||
                e.Action == NotifyCollectionChangedAction.ItemChanged ||
                e.Action == NotifyCollectionChangedAction.Replace)
            {
                if (e.NewItems.Count == 1)
                {
                    int i = 0;
                    PropertySerializationMetadata metadata = (PropertySerializationMetadata)e.NewItems[i];
                    PropertySerializationMetadata existing =
                        this.Find(delegate(PropertySerializationMetadata toLookUp)
                                      {
                                          return toLookUp.TypeFullName == metadata.TypeFullName &&
                                                 toLookUp.PropertyName == metadata.PropertyName;
                                      });

                    if (existing != null)
                        existing.Attributes = metadata.Attributes;
                }
            }

            return base.OnCollectionChanging(e);
        }

        public PropertySerializationMetadata FindClassMetadata(object targetObject)
        {
            return FindPropertyMetadata(targetObject, null);
        }

        public PropertySerializationMetadata FindPropertyMetadata(object targetObject, PropertyDescriptor property)
        {
            string name = property.Name;
            PropertySerializationMetadata res =
                this.Find(delegate(PropertySerializationMetadata metadata)
                              {
                                  return metadata.Type != null &&
                                         metadata.Type.IsInstanceOfType(targetObject) &&
                                         metadata.PropertyName == name;
                              });

            if (res == null)
            {
                res = this.Find(delegate(PropertySerializationMetadata metadata)
                                    {
                                        return (metadata.TypeFullName == targetObject.GetType().FullName ||
                                                metadata.TypeFullName == property.ComponentType.FullName) &&
                                               metadata.PropertyName == name;
                                    });
            }

            return res;
        }

        public PropertySerializationMetadata Find(Predicate<PropertySerializationMetadata> match)
        {
            if (match == null)
            {
                new ArgumentException("Parameter 'match' cannot be null");
            }

            for (int i = 0; i < this.Count; i++)
            {
                if (match(this[i]))
                {
                    return this[i];
                }
            }

            return default(PropertySerializationMetadata);
        }
    }
}
