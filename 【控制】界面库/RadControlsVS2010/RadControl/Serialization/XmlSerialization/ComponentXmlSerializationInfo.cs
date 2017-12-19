using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.XmlSerialization
{
    /// <summary>
    /// Instances of this type are used by <see cref="ComponentXmlSerializer"/> to provide information used to control
    /// XML of properties and sub-objets.
    /// </summary>
    public class ComponentXmlSerializationInfo
    {
        private PropertySerializationMetadataCollection serializationMetadata;
        private bool disregardOriginalSerializationVisibility;
        private bool serializeDefaultValues;

        public ComponentXmlSerializationInfo(PropertySerializationMetadataCollection serializationMetadata)
        {
            this.serializationMetadata = serializationMetadata;
        }

        //Depricated
        //public List<string> SerializableObjects
        //{
        //    get
        //    {
        //        return this.serializableObjects;
        //    }
        //    set
        //    {
        //        this.serializableObjects = value;
        //    }
        //}

        //Depricated
        //public List<string> NonSerializableProperties
        //{
        //    get
        //    {
        //        return this.nonSerializableProperties;
        //    }
        //    set
        //    {
        //        this.nonSerializableProperties = value;
        //    }
        //}

        //Depricated
        //public List<object> NonSerializableObjects
        //{
        //    get
        //    {
        //        return this.nonSerializableObjects;
        //    }
        //    set
        //    {
        //        this.nonSerializableObjects = value;
        //    }
        //}

        /// <summary>
        /// Gets a collection of attributes for properties that would override the original designer serialization 
        /// metadata for these properties
        /// </summary>
        public PropertySerializationMetadataCollection SerializationMetadata
        {
            get
            {
                if (this.serializationMetadata == null)
                {
                    serializationMetadata = new PropertySerializationMetadataCollection();
                }

                return serializationMetadata;
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether the serializer will use the serialization visibility attributes 
        /// of the properties of the serialized objects or only those found in <see cref="SerializationMetadata"/>
        /// </summary>
        public bool DisregardOriginalSerializationVisibility
        {
            get { return disregardOriginalSerializationVisibility; }
            set { disregardOriginalSerializationVisibility = value; }
        }

        /// <summary>
        /// Gets or sets value indincating whether the serializer will force serialization of properties, disregarding 
        /// the values of the DefaultValue attribute or ShouldSerialize method
        /// </summary>
        public bool SerializeDefaultValues
        {
            get { return serializeDefaultValues; }
            set { serializeDefaultValues = value; }
        }
    }
}
