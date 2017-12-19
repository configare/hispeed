using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Telerik.WinControls.Themes.Serialization;
using System.Diagnostics;

namespace Telerik.WinControls.XmlSerialization
{
    //TODO: 
    // - Instance descriptor 
    // - component reference
    /// <summary>
    /// Serializes components to XML, using the same rules that apply in Code Dom serialization, in VS designer.
    /// </summary>
    public class ComponentXmlSerializer
    {
        private IDictionary typeMap;
        private IDictionary reversedTypeMap;
        private ComponentXmlSerializationInfo componentSerializationInfo;

        private object rootSerializationObject = null;
        private IPropertiesProvider propertiesProvider;

        private bool resolveTypesOnlyInTelerikAssemblies = false;
        private InstanceFactory instanceFactory;

        XmlSerializerDisposalBin disposalBin = new XmlSerializerDisposalBin();

        public ComponentXmlSerializer()
        {
            this.componentSerializationInfo = new ComponentXmlSerializationInfo(new PropertySerializationMetadataCollection());
        }

        /// <summary>
        /// Constructs new instance of the class, providing extended properties serialization information
        /// </summary>
        /// <param name="componentSerializationInfo">Extends the properties serialization information.</param>
        public ComponentXmlSerializer(ComponentXmlSerializationInfo componentSerializationInfo)
        {
            this.componentSerializationInfo = componentSerializationInfo;
            if (this.componentSerializationInfo == null)
            {
                this.componentSerializationInfo =
                    new ComponentXmlSerializationInfo(new PropertySerializationMetadataCollection());
            }
        }

        /// <summary>
        /// Constructs new instance of the class, providing extended serialization information.
        /// </summary>
        /// <param name="typeMap">Dictinary to use that maps type names to XML element names. Keys of the dictionary entries should be type full names. Values should correspond to the type instances.</param>
        /// <param name="componentSerializationInfo">Extends the properties serialization information.</param>
        public ComponentXmlSerializer(IDictionary typeMap, ComponentXmlSerializationInfo componentSerializationInfo): this(componentSerializationInfo)
        {
            this.typeMap = typeMap;
            if (this.typeMap != null)
            {
                reversedTypeMap = new Hashtable(this.typeMap.Count);
                foreach (DictionaryEntry entry in typeMap)
                {
                    reversedTypeMap[entry.Value] = entry.Key;
                }
            }
        }

        public object RootSerializationObject
        {
            get { return rootSerializationObject; }
        }

        public IPropertiesProvider PropertiesProvider
        {
            get { return propertiesProvider; }
            set { propertiesProvider = value; }
        }

        /// <summary>
        /// Gets or sets value indicating whether the serializer will search all domain assemblies for a specified type 
        /// (by FullName) or will search only assemblies related to telerik
        /// </summary>
        public bool ResolveTypesOnlyInTelerikAssemblies
        {
            get { return resolveTypesOnlyInTelerikAssemblies; }
            set { resolveTypesOnlyInTelerikAssemblies = value; }
        }

        protected void ReadDictionaryElement(XmlReader reader, object dictionaryOwner, IDictionary toRead)
        {
            this.ReadCollectionElement(reader, dictionaryOwner, new DictionarySerializationListWrapper(toRead));
        }

        /// <summary>
        /// if Reader is positioned at an element that is a collection, reads the collection items.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="toRead"></param>        
        public void ReadCollectionElement(XmlReader reader, IList toRead)
        {
            this.ReadCollectionElement(reader, null, toRead);
        }

        /// <summary>
        /// if Reader is positioned at an element that is a collection, reads the collection items.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="collectionOwner">object that owns the property (collection) currently deserialized</param>
        /// <param name="toRead"></param>        
        public void ReadCollectionElement(XmlReader reader, object collectionOwner, IList toRead)
        {
            this.ReadCollectionElement(reader, collectionOwner, toRead, false);
        }

        /// <summary>
        /// if Reader is positioned at an element that is a collection, reads the collection items.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="collectionOwner">object that owns the property (collection) currently deserialized</param>
        /// <param name="toRead"></param>      
        /// <param name="disposeObjects"></param>
        public void ReadCollectionElement(XmlReader reader, object collectionOwner, IList toRead, bool disposeObjects)
        {
            if (this.RootSerializationObject == null)
            {
                this.rootSerializationObject = toRead;
                this.InitializeRead();
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            int startDepth = reader.Depth;

            if (disposeObjects)
            {
                this.disposalBin.AddObjectsToDispose(toRead);
            }
                        
            toRead.Clear();            

            while (reader.ReadState != ReadState.Error &&
                   !reader.EOF &&
                   !(reader.NodeType == XmlNodeType.EndElement
                     && reader.Depth == startDepth))
            {
                reader.Read();

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Depth == startDepth + 1)
                        {
                            ObjectSerializationInfo serializationInfo;
                            try
                            {
                                serializationInfo = this.ResolveSerializationInfo(reader, null, toRead);
                            }
                            catch (Exception)
                            {
                                this.SkipUnknownXml(reader);
                                continue;
                            }

                            Type type = serializationInfo.ObjectType;
                            if (type == null)
                            {
                                break;
                            }

                            object itemInstance;

                            if (serializationInfo.IsSerializedAsString)
                            {
                                itemInstance = this.ReadElementValueAsObject(reader, serializationInfo);
                            }
                            else
                            {
                                try
                                {
                                    itemInstance = this.InstanceFactory.CreateInstance(type);
                                }
                                catch (TargetInvocationException)
                                {
                                    //TODO: log ex
                                    continue;
                                }
                                catch (MissingMethodException)
                                {
                                    //TODO: log ex
                                    continue;
                                }

                                this.ReadObjectElement(reader, collectionOwner, itemInstance);
                            }

                            if (toRead is ISerializationValidatable)
                            {
                                ((ISerializationValidatable) toRead).Validate();
                            }

                            toRead.Add(itemInstance);
                        }

                        break;
                    case XmlNodeType.EndElement:
                        break;
                }
            }
        }

        public virtual InstanceFactory InstanceFactory
        {
            get
            {
                if (this.instanceFactory == null)
                {
                    this.instanceFactory = new RuntimeInstanceFactory();
                }
                return this.instanceFactory;
            }
            set
            {
                this.instanceFactory = value;
            }
        }

        private object CreateInstance(Type type)
        {
            return Activator.CreateInstance(type);            
        }

        /// <summary>
        /// Reads the collection items if reader is positioned on an element that is a collection.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="parent"></param>
        /// <param name="parentProperty"></param>
        /// <param name="toRead"></param>
        /// <param name="uniquePropertyName">property used to match objects in collecion</param>
        protected void ReadMergeCollection(XmlReader reader, Object parent, PropertyDescriptor parentProperty, IList toRead, string uniquePropertyName)
        {
            ReadMergeCollection(reader, parent, parentProperty, toRead, uniquePropertyName, false, false);
        }

        /// <summary>
        /// Reads the collection items if reader is positioned on an element that is a collection.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="parent"></param>
        /// <param name="parentProperty"></param>
        /// <param name="toRead"></param>
        /// <param name="uniquePropertyName">property used to match objects in collecion</param>
        /// <param name="disposeObjects"></param>
        protected void ReadMergeCollection(XmlReader reader, Object parent, PropertyDescriptor parentProperty, IList toRead, string uniquePropertyName, bool disposeObjects)
        {
            ReadMergeCollection(reader, parent, parentProperty, toRead, uniquePropertyName, false, disposeObjects);
        }

        /// <summary>
        /// if Reader is positioned at an element that is a collection, reads the collection items.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="uniquePropertyName">property used to match objects in collecion</param>
        /// <param name="toRead"></param>
        /// <param name="disposeObjects"></param>
        /// <param name="parent"></param>
        /// <param name="parentProperty"></param>
        /// <param name="preserveNotSerializedExistingElements">States whether the list specified by toRead should not be cleared before reading</param>
        protected void ReadMergeCollection(XmlReader reader, object parent, PropertyDescriptor parentProperty, IList toRead, string uniquePropertyName, bool preserveNotSerializedExistingElements, bool disposeObjects)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            int startDepth = reader.Depth;
            int index = 0;

            if (toRead == null)
                return;            

            //Array to copy existing items in an array and clear the original list
            ArrayList existingElements = new ArrayList(toRead.Count);            
            for (int i = 0; i < toRead.Count; i++ )
            {
                existingElements.Add(toRead[i]);
            }

            //clear original list, re-add existing items later on; add new instances if needed
            toRead.Clear();

            while (!(reader.NodeType == XmlNodeType.EndElement
                && reader.Depth == startDepth))
            {
                reader.Read();

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Depth == startDepth + 1)
                        {
                            ObjectSerializationInfo serializationInfo;
                            try
                            {
                                serializationInfo = this.ResolveSerializationInfo(reader, null, toRead);
                            }
                            catch (Exception)
                            {
                                this.SkipUnknownXml(reader);
                                continue;
                            }

                            Type collectionItemType = serializationInfo.ObjectType;
                            if (collectionItemType == null)
                            {
                                break;
                            }

                            //object itemInstance;
                            //TODO : bug if toRead is first cleared?
                            if (serializationInfo.IsSerializedAsString) // TODO: merge items serializaed as string
                            {
                                object itemInstance = this.ReadElementValueAsObject(reader, serializationInfo);
                            }
                            else
                            {
                                if (uniquePropertyName != null) //egg. in the case when reading grid columns
                                {
                                    int foundAtIndex;
                                    object foundElement = this.MatchObjectElement(reader, parent, parentProperty, toRead, uniquePropertyName, existingElements, out foundAtIndex);

                                    if (foundElement == null)
                                    {
                                        foundElement = this.InstanceFactory.CreateInstance(collectionItemType);
                                    }
                                    else
                                    {
                                        disposalBin.SetObjectShouldNotBeDisposed(foundElement);

                                        if (foundAtIndex != -1)
                                        {
                                            existingElements.RemoveAt(foundAtIndex);
                                        }
                                    }

                                    this.ReadObjectElement(reader, parent, foundElement);
                                    toRead.Add(foundElement);
                                }
                                else // this is the case when deserializing GridViewTemplates - maching by index
                                {
                                    if (existingElements.Count > index)
                                    {
                                        object instanceFound = existingElements[index];
                                        this.ReadObjectElement(reader, instanceFound);
                                        if (!preserveNotSerializedExistingElements)
                                        {
                                            toRead.Add(instanceFound);
                                        }
                                        //else it has been never removed from the original list
                                    }
                                    else
                                    {
                                        object newInstance = this.InstanceFactory.CreateInstance(collectionItemType);
                                        toRead.Add(newInstance);
                                        this.ReadObjectElement(reader, newInstance);
                                    }
                                    // TODO: else - instance collection contains less objects than serilized what?   
                                }

                                index++;
                            }

                            if (toRead is ISerializationValidatable)
                            {
                                ((ISerializationValidatable)toRead).Validate();
                            }
                        }

                        break;
                    case XmlNodeType.EndElement:
                        break;
                }
            }

            //re-add elements previously cleared from toRead, which have not been found in the xml
            if (preserveNotSerializedExistingElements)
            {
                for (int i = 0; i < existingElements.Count; i++)
                {
                    toRead.Add(existingElements[i]);
                }
            }
            else
            {
                if (disposeObjects)
                {
                    for (int i = 0; i < existingElements.Count; i++)
                    {
                        disposalBin.AddObjectToDispose(existingElements[i]);
                    }
                }
            }
        }

        private ObjectSerializationInfo ResolveSerializationInfo(PropertyDescriptor property, object propertyOwner)
        {
            if (property != null)
            {
                SerializationConverterAttribute attribute = (SerializationConverterAttribute)property.Attributes[typeof(SerializationConverterAttribute)];
                if (attribute != null)
                {
                    SerializationConverter converter = attribute.GetConverterInstance();
                    return new ObjectSerializationInfo(this, converter, propertyOwner, property);
                }
                else
                    if (property.PropertyType == typeof(string))
                    {
                        return new ObjectSerializationInfo(this, property.PropertyType, null, propertyOwner, property);
                    }
                    else
                    {
                        TypeConverter converter = property.Converter;
                        if (converter.CanConvertTo(typeof(string)) && converter.CanConvertFrom(typeof(string)))
                            return new ObjectSerializationInfo(this, property.PropertyType, converter, propertyOwner, property);
                    }
            }

            return new ObjectSerializationInfo(this, property.PropertyType, null, propertyOwner, property);
        }

        private ObjectSerializationInfo ResolveSerializationInfo(object currentObject)
        {
            Type type = currentObject.GetType();
            if (type == typeof(string))
            {
                return new ObjectSerializationInfo(this, type, currentObject);
            }
            else
            {
                TypeConverter converter = TypeDescriptor.GetConverter(type);
                if (converter.CanConvertTo(typeof(string)) && converter.CanConvertFrom(typeof(string)))
                    return new ObjectSerializationInfo(this, type, converter, currentObject);
            }

            return new ObjectSerializationInfo(this, type, currentObject);
        }

        private ObjectSerializationInfo ResolveSerializationInfo(XmlReader reader, PropertyDescriptor property, object propertyOwner)
        {
            string typeName = null;

            string schemaInstanceNs = reader.LookupNamespace(@"http://www.w3.org/2001/XMLSchema-instance");
            if (string.IsNullOrEmpty(schemaInstanceNs))
            {
                schemaInstanceNs = "xsi";
            }

            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == schemaInstanceNs + ":type")
                {
                    typeName = reader.Value;
                    break;
                }
            }

            reader.MoveToElement();

            Type res = null;

            if (typeName == null && property == null)
            {
                typeName = reader.Name;
            }

            if (typeName != null)
            {
                res = this.FindTypeSafe(typeName);
                //Short type names support /type names without RadObject/
                if (res == null)
                {
                    if (typeName != null && this.typeMap != null)
                    {
                        res = (Type)typeMap[typeName];
                    }

                    if (res == null && !typeName.Contains("."))
                    {
                        typeName = typeof(RadObject).Namespace + "." + typeName;
                        res = RadTypeResolver.Instance.GetTypeByName(typeName, false, true);
                    }
                }
            }

            if (res == null && property != null)
            {
                SerializationConverterAttribute attribute = (SerializationConverterAttribute)property.Attributes[typeof(SerializationConverterAttribute)];
                if (attribute != null)
                {
                    SerializationConverter converter = attribute.GetConverterInstance();
                    return new ObjectSerializationInfo(this, converter, propertyOwner, property);
                }

                res = property.PropertyType;
            }
            else
                if (property != null)
                {
                    if (property.PropertyType == typeof(string))
                    {
                        return new ObjectSerializationInfo(this, typeof(string), null, propertyOwner, property);
                    }
                    else
                    {
                        TypeConverter converter = property.Converter;
                        if (converter.CanConvertTo(typeof(string)) && converter.CanConvertFrom(typeof(string)))
                            return new ObjectSerializationInfo(this, res, converter, propertyOwner, property);
                    }
                }
                else
                    if (res != null)
                    {
                        TypeConverter converter = TypeDescriptor.GetConverter(res);
                        if (converter.CanConvertTo(typeof(string)) && converter.CanConvertFrom(typeof(string)))
                            return new ObjectSerializationInfo(this, res, converter, propertyOwner, property);
                    }

            return new ObjectSerializationInfo(this, res, null, propertyOwner, property);
        }        

        internal Type FindTypeSafe(string className)
        {
            return RadTypeResolver.Instance.GetTypeByName(className, false, this.ResolveTypesOnlyInTelerikAssemblies);
        }

        /// <summary>
        /// Matches the instance of the element by an attribute value and then deserializes its properties.
        /// If the instance is not found in existingInstancesToMatch,
        /// new instance of type instanceType will be created and added to existingInstancesToMatch list.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="parent"></param>
        /// <param name="parentProperty"></param>
        /// <param name="toRead"></param>
        /// <param name="propertyToMatch"></param>       
        /// <param name="existingInstancesToMatch">the list with existing instances</param>        
        /// <param name="foundAtIndex">index of the element if found in existingInstanceToMatch</param>
        protected virtual object MatchObjectElement(XmlReader reader, object parent, PropertyDescriptor parentProperty, IList toRead, string propertyToMatch, IList existingInstancesToMatch, out int foundAtIndex)
        {
            string uniquePropertyValue = null;

            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == propertyToMatch)
                {
                    uniquePropertyValue = reader.Value;
                    break;
                }
            }

            reader.MoveToElement();

            object instanceFound = null;
            foundAtIndex = -1;

            if (uniquePropertyValue != null)
            {
                instanceFound = this.MatchExistingItem(reader, toRead, parent, parentProperty, propertyToMatch, uniquePropertyValue, existingInstancesToMatch, ref foundAtIndex);
            }

            return instanceFound;
        }

        protected virtual object MatchExistingItem(XmlReader reader, IList toRead, object parent, PropertyDescriptor parentProperty, string propertyToMatch, string uniquePropertyValue, IList existingInstancesToMatch, ref int foundAtIndex)
        {
            object instanceFound = null;
            PropertyDescriptor propToMatchDesc = null;
            for (int i = 0; i < existingInstancesToMatch.Count; i++)
            {
                object instance = existingInstancesToMatch[i];
                if (instance == null)
                {
                    continue;
                }

                //cache property descriptor
                if (propToMatchDesc == null)
                {
                    propToMatchDesc = TypeDescriptor.GetProperties(instance).Find(propertyToMatch, false);
                }

                if (propToMatchDesc == null)
                {
                    continue;
                }

                object value = this.GetPropertyValue(propToMatchDesc, instance);

                //TODO: convert proerty value - currently will work only for properties of type string
                if (value != null && value.Equals(uniquePropertyValue))
                {
                    instanceFound = instance;
                    foundAtIndex = i;
                    break;
                }
            }

            return instanceFound;
        }


        /// <summary>
        /// Reads properties of an object and subobject the reader is currently 
        /// positioned on.
        /// </summary>
        /// <param name="reader">Xml reader instance, positioned on the element to read.</param>
        /// <param name="toRead">object instance to be processed</param>
        public void ReadObjectElement(XmlReader reader, object toRead)
        {
            this.ReadObjectElement(reader, null, toRead);
        }

        /// <summary>
        /// Reads properties of an object and subobject the reader is currently 
        /// positioned on.
        /// </summary>
        /// <param name="reader">Xml reader instance, positioned on the element to read.</param>
        /// <param name="parentObject">parent object instance, null if there is no parent object information</param>
        /// <param name="toRead">object instance to be processed</param>
        public void ReadObjectElement(XmlReader reader, object parentObject, object toRead)
        {
            if (reader.EOF)
            {
                return;
            }

            if (this.RootSerializationObject == null)
            {
                this.rootSerializationObject = toRead;
                this.InitializeRead();
            }

            if (this.ReadObjectElementOverride(reader, toRead))
            {
                return;
            }

            PropertyDescriptorCollection instanceProperties = TypeDescriptor.GetProperties(toRead);                        

            int startDepth = reader.Depth;

            bool isEmpty = false;

            if (reader.IsEmptyElement)
            {
                isEmpty = true;
            }

            //for (int propertyIndex = 0; propertyIndex < instanceProperties.Count; propertyIndex++)
            {
                //TODO filter only readable properties
                //if ()
            }

            Hashtable processedProperties = new Hashtable();

            List<ObjectSerializationInfo> propertiesWithDelayedDeserialization = new List<ObjectSerializationInfo>(1);

            while (reader.MoveToNextAttribute())
            {
                PropertyDescriptor property = instanceProperties.Find(reader.Name, true);
                if (property == null)
                    continue;

                processedProperties[reader.Name] = true;

                //Fix for custom properties shadowed by component designers
                if (!property.ComponentType.IsAssignableFrom(toRead.GetType()))
                {
                    continue;
                }

                ObjectSerializationInfo serializationInfo = null;
                try
                {
                    serializationInfo = this.ResolveSerializationInfo(property, toRead);
                }
                catch (Exception)
                {
                    SkipUnknownXml(reader);
                    return;
                }


                if (!string.IsNullOrEmpty(reader.Value))
                {
                    if (serializationInfo.ShouldWaitForAttribute)
                    {
                        //handles the case when RadProperty' value attribute is deserializaed first 
                        //bfore the actual RadPrperty is known
                        serializationInfo.SetTempStringValueFromDeserialization(reader.Value);
                        propertiesWithDelayedDeserialization.Add(serializationInfo);
                    }
                    else
                    {
                        if (serializationInfo.IsSerializedAsString)
                        {
                            object propertyValue = serializationInfo.ConvertFromString(reader.Value);
                            SetPropertyValue(property, toRead, propertyValue);
                        }
                    }
                }
            }

            //propecess any cases of changed order of serialization of XmlPropertySettings' property/name
            foreach (ObjectSerializationInfo serInfo in propertiesWithDelayedDeserialization)
            {
                object propertyValue = serInfo.ConvertFromDelayedDeserialization();
                if (serInfo.IsSerializedAsString)
                {
                    this.SetPropertyValue(serInfo.Property, toRead, propertyValue);
                }
            }

            if (isEmpty)
            {
                return;
            }

            //support for nested lists
            if (toRead is IList)
            {
                reader.MoveToElement();
                this.ReadCollectionElement(reader, parentObject, (IList)toRead);
                return;
            }

            while (
                reader.ReadState != ReadState.Error &&
                   !reader.EOF &&
                !(reader.NodeType == XmlNodeType.EndElement
                && reader.Depth == startDepth))
            {
                reader.Read();

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Depth == startDepth || reader.Depth == startDepth + 1)
                        {
                            processedProperties[reader.Name] = true;

                            PropertyDescriptor property = instanceProperties.Find(reader.Name, true);
                            if (property != null)
                            {
                                this.ReadElementInObject(reader, property, toRead);
                            }
                            else
                            {
                                Trace.WriteLine("Error loading xml: unknown property " + reader.Name + " of a " + toRead);
                                Debug.Fail("Error loading xml: unknown property " + reader.Name + " of a " + toRead);
                            }
                        }

                        break;
                    case XmlNodeType.EndElement:
                        break;
                }
            }

            //BugFix - Clear moved to ReadObjectEllement 
            //determine IList/IDictionary properties and Clear instances
            foreach (PropertyDescriptor property in instanceProperties)
            {
                if (processedProperties[property.Name] != null)
                {
                    continue;
                }

                if (ProcessProperty(property))
                {
                    continue;
                }

                bool shouldSerialize; // = false;
                bool shouldSerializeAsAttribute; // = false;
                bool decideByType = false;

                //TODO: fix for DataType property
                if (property.PropertyType == typeof(System.Type))
                    continue;

                //Fix for custom properties shadowed by component designers
                if (!property.ComponentType.IsAssignableFrom(toRead.GetType()))
                {
                    continue;
                }

                Attribute attribute = property.Attributes[typeof(StyleSerializationAttributeAttribute)];
                shouldSerializeAsAttribute = attribute != null;
                shouldSerialize = attribute != null;

                if (attribute == null)
                {
                    attribute = property.Attributes[typeof(XmlAttributeAttribute)];
                    shouldSerializeAsAttribute = attribute != null;
                    shouldSerialize = attribute != null;
                }

                PropertySerializationMetadata overrideMetadata = null;

                if (attribute == null)
                {
                    overrideMetadata =
                        componentSerializationInfo.SerializationMetadata.FindPropertyMetadata(toRead, property);

                    if (overrideMetadata != null)
                    {
                        attribute = overrideMetadata.Attributes[typeof(DesignerSerializationVisibilityAttribute)];
                        decideByType = attribute == null || attribute.IsDefaultAttribute();
                        if (attribute != null)
                        {
                            DesignerSerializationVisibility visibility =
                                ((DesignerSerializationVisibilityAttribute)attribute).Visibility;

                            shouldSerialize = visibility != DesignerSerializationVisibility.Hidden;
                            shouldSerializeAsAttribute = visibility == DesignerSerializationVisibility.Visible;
                        }
                    }
                }

                if (attribute == null &&
                    !this.componentSerializationInfo.DisregardOriginalSerializationVisibility)
                {
                    attribute = property.Attributes[typeof(DesignerSerializationVisibilityAttribute)];
                    decideByType = attribute == null || attribute.IsDefaultAttribute();
                    shouldSerialize = property.SerializationVisibility != DesignerSerializationVisibility.Hidden;
                    shouldSerializeAsAttribute = property.SerializationVisibility ==
                                                 DesignerSerializationVisibility.Visible;
                }

                if (attribute == null || shouldSerializeAsAttribute || shouldSerialize)
                {
                    attribute = property.Attributes[typeof(XmlObsoleteOnlyRead)];
                    if (attribute != null)
                    {
                        shouldSerializeAsAttribute = false;
                        shouldSerialize = false;
                    }
                }

                if (shouldSerialize)
                {
                    try
                    {
                        if (!shouldSerializeAsAttribute)
                        {
                            ObjectSerializationInfo serializationInfo = this.ResolveSerializationInfo(property, toRead);
                            if (!serializationInfo.IsSerializedAsString)
                            {
                                if (typeof(IList).IsAssignableFrom(serializationInfo.ObjectType))
                                {
                                    IList list = (IList)serializationInfo.GetCurrPropertyValue();
                                    if (list != null)
                                        list.Clear();
                                }
                                else if (typeof(IDictionary).IsAssignableFrom(serializationInfo.ObjectType))
                                {
                                    IDictionary dictionary = (IDictionary)serializationInfo.GetCurrPropertyValue();
                                    if (dictionary != null)
                                        dictionary.Clear();
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //Skip properties that throw error when value serialized
                        continue;
                    }
                }
            }

            if (reader.EOF ||
                (reader.NodeType == XmlNodeType.EndElement &&
                reader.Depth == 0))
            {
                this.disposalBin.DisposeDisposalBin(this.DisposeObject);
            }
        }

        protected virtual bool ProcessProperty(PropertyDescriptor property)
        {
            return false;
        }

        protected virtual void DisposeObject(IDisposable toBeDisposed)
        {
            toBeDisposed.Dispose();
        }

        /// <summary>
        /// Override to provide alternative deserialization of objects.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="toRead"></param>
        /// <returns>value indicating whether the object should be processed any further by serializer</returns>
        protected virtual bool ReadObjectElementOverride(XmlReader reader, object toRead)
        {
            return false;
        }

        private void SkipUnknownXml(XmlReader reader)
        {
            //Experimental
            //Try to skip the tags that deserializer cannot recognize
            int startDepth = reader.Depth;
            while (reader.ReadState != ReadState.Error &&
                   !reader.EOF &&
                   (reader.Depth >= startDepth ||
                    reader.NodeType != XmlNodeType.EndElement))
            {
                reader.Read();
            }
        }

        private void ReadElementProperty(XmlReader reader, object toRead, object propertyOwner, PropertyDescriptor currentProperty, ObjectSerializationInfo info)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            int startDepth = reader.Depth;

            while (reader.ReadState != ReadState.Error &&
                   !reader.EOF &&
                   !(reader.NodeType == XmlNodeType.EndElement
                     && reader.Depth == startDepth))
            {
                reader.Read();

                switch (reader.NodeType)
                {
                    case XmlNodeType.Text:

                        object propertyValue;
                        propertyValue = info.ConvertFromString(reader.Value);
                        /*if (info.RadProperty != null)
                        {
                            propertyValue = XmlPropertySetting.DeserializeValue(info.RadProperty, reader.Value);                            
                        }
                        else
                        {
                            propertyValue = XmlPropertySetting.DeserializeValue(info.ObjectType, reader.Value, currentProperty.DisplayName);
                        }*/
                        if (propertyValue is ISerializationValidatable)
                        {
                            ((ISerializationValidatable) propertyValue).Validate();
                        }

                        this.SetPropertyValue(currentProperty, toRead, propertyValue);
                        break;

                    case XmlNodeType.Element:
                        this.ReadElementInObject(reader, currentProperty, toRead);

                        break;
                    case XmlNodeType.EndElement:
                        break;
                }
            }

            //Should not read the end element so ReadObjectElement to know if it's done too
            //reader.ReadEndElement();
        }

        private object GetNullValue(Type type)
        {
            //cannot use null for value-types
            if (type.IsValueType)
                return Activator.CreateInstance(type);

            return null;
        }

        private object ReadElementValueAsObject(XmlReader reader, ObjectSerializationInfo info)
        {
            if (reader.IsEmptyElement)
            {
                return this.GetNullValue(info.ObjectType);
            }

            int startDepth = reader.Depth;

            while (
                reader.ReadState != ReadState.Error &&
                !reader.EOF &&
                !(reader.NodeType == XmlNodeType.EndElement
                  && reader.Depth == startDepth))
            {
                reader.Read();

                switch (reader.NodeType)
                {
                    case XmlNodeType.Text:

                        object propertyValue;
                        propertyValue = info.ConvertFromString(reader.Value);
                        return propertyValue;

                    case XmlNodeType.Element:
                        //Error: Incorrect structure
                        //this.ReadElementInObject(reader, currentProperty, toRead);

                        break;
                    case XmlNodeType.EndElement:
                        break;
                }
            }

            return null;
        }

        /// <summary>
        /// Deserializes a specified property of an object
        /// </summary>
        /// <param name="reader">Xml reader, positioned on the element coresponding to the property to deserialize</param>
        /// <param name="property">Property descriptor of the property to deserialize</param>
        /// <param name="toRead">Object that owns the property to deserialize</param>
        protected void ReadElementInObject(XmlReader reader, PropertyDescriptor property, object toRead)
        {
            ObjectSerializationInfo serializationInfo;
            try
            {
                serializationInfo = this.ResolveSerializationInfo(reader, property, toRead);
            }
            catch (Exception)
            {
                this.SkipUnknownXml(reader);
                return;
            }

            Type type = serializationInfo.ObjectType;

           
            if (type.IsArray)
            {
                ArrayList list = new ArrayList();
                this.ReadCollectionElement(reader, toRead, list);

                Array resArray = Array.CreateInstance(type.GetElementType(), list.Count);
                list.CopyTo(resArray);

                this.SetPropertyValue(property, toRead, resArray);
            }
            else if (typeof(IList).IsAssignableFrom(type))
            {
                if (property.IsReadOnly ||
                    property.SerializationVisibility == DesignerSerializationVisibility.Content)
                {
                    IList list = (IList)this.GetPropertyValue(property, toRead);

                    if (list == null) //TODO: check if this is the component serializer behavior in this case
                        return;


                    if (!ProcessListOverride(reader, toRead, property, list))
                    {
                        this.ReadCollectionElement(reader, toRead, list);
                    }
                }
                else
                {
                    IList list = (IList)this.InstanceFactory.CreateInstance(type);
                    this.ReadCollectionElement(reader, toRead, list);
                    this.SetPropertyValue(property, toRead, list);
                }
            } 
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                if (property.IsReadOnly ||
                    property.SerializationVisibility == DesignerSerializationVisibility.Content)
                {
                    IDictionary list = (IDictionary)this.GetPropertyValue(property, toRead);

                    if (list == null) //TODO: check if this is the component serializer behavior in this case
                        return;

                    //if (!ProcessListOverride(reader, list))
                    {
                        this.ReadDictionaryElement(reader, toRead, list);
                    }
                }
                else
                {
                    IDictionary list = (IDictionary)this.InstanceFactory.CreateInstance(type);
                    this.ReadDictionaryElement(reader, toRead, list);
                    this.SetPropertyValue(property, toRead, list);
                }
            }
            else if (serializationInfo.IsSerializedAsString)
            {
                this.ReadElementProperty(reader, toRead, toRead, property, serializationInfo);
            }
            else
            {
                //treat properties of type object differently
                if (property.PropertyType == typeof(object) && type != typeof(object))
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(type);
                    if (converter != null &&
                        converter.CanConvertTo(typeof(string)) &&
                        converter.CanConvertFrom(typeof(string)))
                    {
                        string stringValue = reader.ReadElementContentAsString();
                        object toReadValue = converter.ConvertFrom(null, CultureInfo.InvariantCulture, stringValue);
                        this.SetPropertyValue(property, toRead, toReadValue);
                        return;
                    }
                }

                if (property.IsReadOnly)
                {
                    object newInstance = GetPropertyValue(property, toRead);

                    if (newInstance == null)
                    {
                        //throw new InvalidOperationException("Property is readonloy but value was null..");
                        return;
                    }

                    this.ReadObjectElement(reader, toRead, newInstance);
                    if (newInstance is ISerializationValidatable)
                    {
                        ((ISerializationValidatable)newInstance).Validate();
                    }
                }
                else
                {
                    object newInstance = null;
                    try
                    {
                        newInstance = this.InstanceFactory.CreateInstance(type);
                    }
                    catch (TargetInvocationException)
                    {
                        return;
                    }
                    catch (MissingMethodException)
                    {
                        return;
                    }

                    this.ReadObjectElement(reader, toRead, newInstance);
                    if (newInstance is ISerializationValidatable)
                    {
                        ((ISerializationValidatable)newInstance).Validate();
                    }

                    this.SetPropertyValue(property, toRead, newInstance);
                }
            }
        }

        internal void SetPropertyValue(PropertyDescriptor property, object propertyOwner, object value)
        {
            object target = TypeDescriptor.GetAssociation(property.ComponentType, propertyOwner);

            property.SetValue(target, value);
        }

        internal object GetPropertyValue(PropertyDescriptor property, object propertyOwner)
        {
            object target = TypeDescriptor.GetAssociation(property.ComponentType, propertyOwner);

            if (!property.ComponentType.IsInstanceOfType(target))
            {
                ICustomTypeDescriptor customTypeDesriptor = propertyOwner as ICustomTypeDescriptor;
                if (customTypeDesriptor != null)
                {
                    target = customTypeDesriptor.GetPropertyOwner(property);
                    if (!property.ComponentType.IsInstanceOfType(target))
                    {
                        return null;
                    }
                }
                else
                {
                    try
                    {
                        property.GetValue(target);
                    }
                    catch(Exception)
                    {
                        //abandon all hope
                        return null;
                    }
                }
            }

            return property.GetValue(target);
        }

        /// <summary>
        /// Override to provide custom processing of collection being deserialized
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="listOwner"></param>
        /// <param name="ownerProperty"></param>
        /// <param name="list"></param>
        /// <returns>True if the list does not require further processign by the deserializer, False to use the default deserailization</returns>
        protected virtual bool ProcessListOverride(XmlReader reader, Object listOwner, PropertyDescriptor ownerProperty, IList list)
        {
            return false;
        }
        
        private void WriteTypeAttribute(XmlWriter writer, Type expectedType, object toWrite, DesignerSerializationVisibility serializationVisibility)
        {
            if (toWrite == null)
            {
                return;
            }

            if (serializationVisibility != DesignerSerializationVisibility.Visible)
            {
                return;
            }

            Type objectType = toWrite.GetType();
            if (objectType != expectedType)
            {
                writer.WriteAttributeString("xsi", "type", @"http://www.w3.org/2001/XMLSchema-instance", this.GetElementNameByType(objectType));
            }
        }

        /// <summary>
        /// Serializes the given object using the specified XmlWriter.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="toWrite"></param>
        public virtual void WriteObjectElement(XmlWriter writer, object toWrite)
        {
            if (toWrite == null)
                return;

            if (this.RootSerializationObject == null)
            {
                this.rootSerializationObject = toWrite;
                this.InitializeWrite();
            }

            //if (!xsiAttributeWritten)
            //{

                //string prefix = writer.LookupPrefix(@"http://www.w3.org/2001/XMLSchema-instance");
                //if (string.IsNullOrEmpty(prefix))
                //{
                //    writer.WriteAttributeString("xmlns", "xsi", null, @"http://www.w3.org/2001/XMLSchema-instance");
                //}

                //calls private void PushNamespace(string prefix, string ns, bool declared) -for test purposes
                //if (writer is XmlTextWriter)
                //{
                //    MethodInfo pushNamespaceMethod =
                //        typeof(XmlTextWriter).GetMethod("PushNamespace",
                //                                         BindingFlags.Instance | BindingFlags.InvokeMethod |
                //                                         BindingFlags.NonPublic);

                //    if (pushNamespaceMethod != null)
                //    {
                //        pushNamespaceMethod.Invoke(writer, new object[] { "xsi", "http://www.w3.org/2001/XMLSchema-instance", true });
                //    }
                //}

            //    xsiAttributeWritten = true;
            //}

            PropertyDescriptorCollection instanceProperties = TypeDescriptor.GetProperties(toWrite);
            ArrayList propertiesToSerializeAsElements = new ArrayList();

            //replaced with propertyserialization metadata 
            //if (componentSerializationInfo.NonSerializableProperties.Contains(path))
            //{
            //    return;
            //}

            //Persist attributes
            foreach (PropertyDescriptor property in instanceProperties)
            {
                bool shouldSerialize; // = false;
                bool serializationVisibilityVisible; // = false;
                bool decideByType = false;

                //TODO: fix for DataType property
                if (property.PropertyType == typeof (System.Type))
                    continue;

                ////Fix for custom properties shadowed by component designers
                //if (!property.ComponentType.IsAssignableFrom(toWrite.GetType()))
                //{
                //    continue;
                //}

                Attribute attribute = property.Attributes[typeof (StyleSerializationAttributeAttribute)];
                serializationVisibilityVisible = attribute != null;
                shouldSerialize = attribute != null;

                if (attribute == null)
                {
                    attribute = property.Attributes[typeof (XmlAttributeAttribute)];
                    serializationVisibilityVisible = attribute != null;
                    shouldSerialize = attribute != null;
                }

                PropertySerializationMetadata overrideMetadata = null;

                if (attribute == null)
                {
                    overrideMetadata =
                        componentSerializationInfo.SerializationMetadata.FindPropertyMetadata(
                            toWrite,
                            property);

                    if (overrideMetadata != null)
                    {
                        attribute = overrideMetadata.Attributes[typeof (DesignerSerializationVisibilityAttribute)];
                        decideByType = attribute == null || attribute.IsDefaultAttribute();
                        if (attribute != null)
                        {
                            DesignerSerializationVisibility visibility =
                                ((DesignerSerializationVisibilityAttribute) attribute).Visibility;

                            shouldSerialize = visibility != DesignerSerializationVisibility.Hidden;
                            serializationVisibilityVisible = visibility == DesignerSerializationVisibility.Visible;
                        }
                    }
                }

                if (attribute == null &&
                    !this.componentSerializationInfo.DisregardOriginalSerializationVisibility)
                {
                    attribute = property.Attributes[typeof (DesignerSerializationVisibilityAttribute)];
                    decideByType = attribute == null || attribute.IsDefaultAttribute();
                    shouldSerialize = property.SerializationVisibility != DesignerSerializationVisibility.Hidden;
                    serializationVisibilityVisible = property.SerializationVisibility ==
                                                 DesignerSerializationVisibility.Visible;
                }

                if (attribute == null || serializationVisibilityVisible || shouldSerialize)
                {
                    attribute = property.Attributes[typeof (XmlObsoleteOnlyRead)];
                    if (attribute != null)
                    {
                        serializationVisibilityVisible = false;
                        shouldSerialize = false;
                    }
                }

                if (shouldSerialize && this.ShouldSerializeValue(toWrite, property, overrideMetadata))
                {
                    try
                    {
                        if (serializationVisibilityVisible && !decideByType)
                        {
                            writer.WriteAttributeString(property.Name,
                                                        XmlPropertySetting.SerializeValue(property,
                                                                                          this.GetPropertyValue(property, toWrite),
                                                                                          property.DisplayName));
                        }
                        else
                        {
                            ObjectSerializationInfo serializationInfo = this.ResolveSerializationInfo(property, toWrite);
                            if (serializationInfo.IsSerializedAsString)
                            {
                                writer.WriteAttributeString(property.Name, serializationInfo.ConvertToString());
                            }
                            else
                            {
                                propertiesToSerializeAsElements.Add(property);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //Skip properties that throw error when value serialized
                        continue;
                    }
                }
            }            
             
            foreach (PropertyDescriptor property in propertiesToSerializeAsElements)
            {
                //TODO: large scale code duplication                
                DesignerSerializationVisibility serializationVisibility = property.SerializationVisibility;

                PropertySerializationMetadata overrideMetadata =
                        componentSerializationInfo.SerializationMetadata.FindPropertyMetadata(
                            toWrite,
                            property);

                if (overrideMetadata != null)
                {
                    Attribute attribute = overrideMetadata.Attributes[typeof (DesignerSerializationVisibilityAttribute)];                    
                    if (attribute != null)
                    {
                        serializationVisibility =
                            ((DesignerSerializationVisibilityAttribute) attribute).Visibility;
                    }
                }

                if (serializationVisibility == DesignerSerializationVisibility.Content && !property.IsReadOnly ||
                    serializationVisibility == DesignerSerializationVisibility.Visible && property.IsReadOnly)
                {
                    continue;
                }

                //TODO: duplicate serialization info resolution
                ObjectSerializationInfo serializationInfo = this.ResolveSerializationInfo(property, toWrite);
                Type type = serializationInfo.ObjectType;
                object propertyValue;
                propertyValue = serializationInfo.GetCurrPropertyValue();

                //treat properties of type object differently
                if (property.PropertyType == typeof(object) && propertyValue != null)
                {
                    Type propertyValueType = propertyValue.GetType();
                    TypeConverter converter = TypeDescriptor.GetConverter(propertyValueType);
                    if (converter != null &&
                        converter.CanConvertTo(typeof(string)) && 
                        converter.CanConvertFrom(typeof(string)))
                    {
                        writer.WriteStartElement(property.Name);
                        this.WriteTypeAttribute(writer, property.PropertyType, propertyValue, serializationVisibility);
                        writer.WriteValue(converter.ConvertToString(null, CultureInfo.InvariantCulture, propertyValue));
                        writer.WriteEndElement();
                        continue;
                    }
                }

                if (propertyValue != null)
                {
                    using (StringWriter stringWriter = new StringWriter())
                    {
                        using (XmlWriter innerWriter = new XmlTextWriter(stringWriter))
                        {
                            //replaced by PropertySerializationMetadata
                            //string newPath = String.IsNullOrEmpty(path) ? property.Name : path + "." + property.Name;
                            //if (componentSerializationInfo.NonSerializableProperties.Contains(newPath))
                            //    continue;

                            innerWriter.WriteStartElement(property.Name);
                            try
                            {
                                this.WriteTypeAttribute(innerWriter, property.PropertyType, propertyValue, serializationVisibility);

                                if (type.IsArray)
                                {
                                    this.WriteCollectionElement(innerWriter, (IList) propertyValue, toWrite, property);
                                }
                                else if (typeof (IList).IsAssignableFrom(type))
                                {
                                    this.WriteCollectionElement(innerWriter, (IList) propertyValue, toWrite, property);
                                }
                                else if (typeof (IDictionary).IsAssignableFrom(type))
                                {
                                    this.WriteDictionaryElement(innerWriter, (IDictionary)propertyValue, toWrite, property);
                                }
                                else
                                {
                                    this.WriteObjectElement(innerWriter, propertyValue);
                                }
                            }
                            catch (Exception ex)
                            {
                                //Skip properties that throw error when serialized
                                Trace.WriteLine("Exception caught while loading xml: " + ex.ToString());
                                continue;
                            }

                            if (innerWriter.WriteState != WriteState.Error)
                            {
                                innerWriter.WriteEndElement();

                                string output = stringWriter.ToString();

                                if (output.Length > property.Name.Length + 4)
                                {
                                    using (StringReader reader = new StringReader(output))
                                    {
                                        using (XmlTextReader textReader = new XmlTextReader(reader))
                                        {
                                            writer.WriteNode(textReader, true);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //support for nested lists
            if (toWrite is IList)
            {
                this.WriteCollectionElement(writer, (IList)toWrite, null, null);
            }
            else if (toWrite is IDictionary) //support for nested dictionaries
            {
                this.WriteDictionaryElement(writer, (IDictionary)toWrite, null, null);
            }
        }                

        /// <summary>
        /// Provides logic to determine whether property value should be serlilized.
        /// </summary>
        /// <remarks>
        /// ShouldSerialize value resolution is as follows: 
        /// <list>
        /// <item>1. ComponentSerializationInfo.SerializeDefaultValues</item>
        /// <item>2. overwriteMetadata contains attribute DesignerSerializationVisibilityAttribute.Content</item>
        /// <item>3. property.ShouldSerialize</item>
        /// </list>
        /// </remarks>
        /// <param name="component"></param>
        /// <param name="property">property to serialize</param>
        /// <param name="overwriteMetadata">collection of extra serialization attributes for the proeprty, corresponding to ComponentSerializationInfo</param>
        /// <returns>value indicating whether property value should be serialized</returns>
        protected virtual bool ShouldSerializeValue(object component, PropertyDescriptor property, PropertySerializationMetadata overwriteMetadata)
        {
            //First, check if the propery descriptor belongs to the compnent ir its designer /GetAssociation/
            object target = TypeDescriptor.GetAssociation(property.ComponentType, component);

            if (!property.ComponentType.IsInstanceOfType(target))
            {
                ICustomTypeDescriptor customDesriptor = component as ICustomTypeDescriptor;
                if (customDesriptor != null)
                {
                    target = customDesriptor.GetPropertyOwner(property);
                    if (!property.ComponentType.IsInstanceOfType(target))
                    {
                        return false;
                    }
                }

                return false;
            }

            if (property.Attributes.Contains(DesignOnlyAttribute.Yes))
            {
                return false;
            }

            if (overwriteMetadata != null)
            {
                if (property.IsReadOnly)
                {
                    return overwriteMetadata.Attributes.Contains(DesignerSerializationVisibilityAttribute.Content);
                }

                if (overwriteMetadata.ShouldSerializeProperty != null)
                {
                    return (bool)overwriteMetadata.ShouldSerializeProperty;
                }

                if (overwriteMetadata.Attributes.Contains(DesignerSerializationVisibilityAttribute.Visible))
                {
                    return true;
                }

                if (overwriteMetadata.Attributes.Contains(DesignerSerializationVisibilityAttribute.Hidden))
                {
                    return false;
                }
            }

            if (this.componentSerializationInfo.SerializeDefaultValues)
            {
                return !property.IsReadOnly ||
                       property.Attributes.Contains(DesignerSerializationVisibilityAttribute.Content);
            }            
            
            //ToDo
            //if (overwriteMetadata.Attributes.Contains(DefaultValueAttribute))
            //{
            //    ... return defaultValueAttribute.Value != property.GetValue();
            //}
            
            //TODO: Implement logic to override DefaultValue attribute as in ReflectedPropertyDescriptor (bellow)
            //if (this.DefaultValue != noValue)
            //{
            //    return !object.Equals(this.DefaultValue, this.GetValue(component));
            //}

            //if (this.ShouldSerializeMethodValue != null)
            //{
            //    try
            //    {
            //        flag = (bool)this.ShouldSerializeMethodValue.Invoke(component, null);
            //    }
            //    catch
            //    {
            //    }
            //    return flag;
            //}
            //
            //return true;

            return property.ShouldSerializeValue(TypeDescriptor.GetAssociation(property.ComponentType, component));
        }

        protected void WriteDictionaryElement(XmlWriter writer, IDictionary toWrite, object listOwner, PropertyDescriptor property)
        {
            this.WriteCollectionElement(writer, new DictionarySerializationListWrapper(toWrite), listOwner, property);
        }

        protected virtual void InitializeWrite()
        {
        }

        protected virtual void InitializeRead()
        {
        }

        protected virtual IEnumerable GetCollectionElementOverride(IEnumerable list, object owner, PropertyDescriptor property)
        {
            return list;
        }

        public void WriteCollectionElement(XmlWriter writer, IEnumerable list, string collectionName)
        {
            writer.WriteStartElement(collectionName);
            this.WriteCollectionElement(writer, list, null, null);
            writer.WriteEndElement();
        }

        protected void WriteCollectionElement(XmlWriter writer, IEnumerable list, object listOwner, PropertyDescriptor property)
        {
            if (this.RootSerializationObject == null)
            {
                this.rootSerializationObject = list;
                this.InitializeWrite();
            }

            IEnumerable collectionElements = GetCollectionElementOverride(list, listOwner, property);
            foreach (object element in collectionElements)
            {
                Type elementType = element.GetType();

                string elementName = this.GetElementNameByType(elementType);

                ObjectSerializationInfo info = ResolveSerializationInfo(element);
                if (info.IsSerializedAsString)
                {
                    writer.WriteElementString(elementName, info.ConvertToString());
                }
                else
                {
                    writer.WriteStartElement(elementName);
                    this.WriteObjectElement(writer, element);
                    writer.WriteEndElement();
                }
            }
        }        

        protected virtual string GetElementNameByType(Type elementType)
        {
            if (this.reversedTypeMap != null)
            {
                string mappedName = (string)this.reversedTypeMap[elementType];
                if (!string.IsNullOrEmpty(mappedName))
                {
                    return mappedName;
                }
            }

            string elementName = elementType.FullName;
            string elementNamespace = elementType.Namespace;
            if (elementNamespace == "Telerik.WinControls")
            {
                elementName = elementType.Name;
            }

            return elementName;
        }       
    }
}
