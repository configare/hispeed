using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using Telerik.WinControls.Primitives;
using System.Reflection;

namespace Telerik.WinControls
{
    /// <summary>
    /// Used to resolve Telerik types
    /// </summary>
    public sealed class RadTypeResolver
    {
        #region Constructor

        private RadTypeResolver()
        {
            this.loadedAssemblies = new List<LoadedAssembly>(20);
            this.knownTypes = new Dictionary<int, Type>(30);
            this.loadedTypes = new Dictionary<int, Type>(20);

            this.InitializeKnownTypes();
        }

        static RadTypeResolver()
        {
            syncRoot = new object();
        }

        #endregion

        #region Internal Methods

        internal void RegisterKnownType(string className, Type type)
        {
            lock (syncRoot)
            {
                int key = className.GetHashCode();
                if (!this.knownTypes.ContainsKey(key))
                {
                    this.knownTypes.Add(key, type);
                }
            }
        }

        public Type GetTypeByName(string className)
        {
            return GetTypeByName(className, true);
        }

        internal Type GetTypeByName(string className, bool throwOnError)
        {
            return this.GetTypeByName(className, throwOnError, true);
        }

        public Type GetTypeByName(string className, bool throwOnError, bool onlyInTelerikAssemblies)
        {
            Type t = null;
            lock (syncRoot)
            {
                t = this.FindTypeByName(className, onlyInTelerikAssemblies);
            }

            //we should have a valid type at this point
            if (t == null && throwOnError)
            {
                throw new ArgumentException("Type not found: " + className + ". Please make sure you have reference to an assembly which contains type: " + className);
            }

            return t;
        }        

        internal RadProperty GetRegisteredRadProperty(Type radOjectType, string propertyName)
        {
            RadProperty result = null;
            Type currType = radOjectType;
            Type objectType = typeof(object);

            while (currType != objectType)
            {
                result = RadProperty.FindSafe(currType, propertyName);
                if (result != null)
                {
                    break;
                }

                currType = currType.BaseType;
            }

            return result;
        }

        public RadProperty GetRegisteredRadPropertyFromFullName(string propertyFullName)
        {
            RadProperty res = null;

            if (propertyFullName != string.Empty)
            {
                string[] propertyParts = propertyFullName.Split('.');
                string propertyName;
                string className;
                if (propertyParts.Length > 1)
                {
                    propertyName = propertyParts[propertyParts.Length - 1];
                    className = string.Join(".", propertyParts, 0, propertyParts.Length - 1);
                }
                else
                {
                    throw new Exception("Invalid property parts");
                }

                res = GetRegisteredRadProperty(GetTypeByName(className), propertyName);
            }

            return res;
        }

        public bool IsTelerikAssembly(Assembly asm)
        {
            if (asm == null)
                return false;

            if (resolveTypesInCurrentAssembly)
            {
                return asm == Assembly.GetCallingAssembly();
            }

            Version myVersion = this.typeResolverAssemblyVersion;

            if (myVersion == null)
            {
                myVersion = Assembly.GetExecutingAssembly().GetName().Version;
            }

            if (asm.FullName.Contains(this.typeResolverAssemblyName) &&
                asm.GetName().Version == myVersion)
                return true;

            AssemblyName[] names = asm.GetReferencedAssemblies();
            for (int i = 0; i < names.Length; i++)
            {
                if (names[i].FullName.Contains(this.typeResolverAssemblyName) &&
                    names[i].Version == myVersion)
                    return true;
            }

            return false;
        }

        #endregion

        #region Private Methods

        private void InitializeKnownTypes()
        {
            lock (syncRoot)
            {
                this.knownTypes.Add("Telerik.WinControls.Primitives.FillPrimitive".GetHashCode(), typeof(FillPrimitive));
                this.knownTypes.Add("Telerik.WinControls.VisualElement".GetHashCode(), typeof(VisualElement));
                this.knownTypes.Add("Telerik.WinControls.XmlPropertySetting".GetHashCode(), typeof(XmlPropertySetting));
                this.knownTypes.Add("Telerik.WinControls.RadElement".GetHashCode(), typeof(RadElement));
                this.knownTypes.Add("Telerik.WinControls.Primitives.BorderPrimitive".GetHashCode(), typeof(BorderPrimitive));
                this.knownTypes.Add("Telerik.WinControls.XmlAnimatedPropertySetting".GetHashCode(), typeof(XmlAnimatedPropertySetting));
                this.knownTypes.Add("Telerik.WinControls.XmlPropertySettingGroup".GetHashCode(), typeof(XmlPropertySettingGroup));
                this.knownTypes.Add("Telerik.WinControls.XmlPropertySettingCollection".GetHashCode(), typeof(XmlPropertySettingCollection));
                this.knownTypes.Add("Telerik.WinControls.RadItem".GetHashCode(), typeof(RadItem));
                this.knownTypes.Add("Telerik.WinControls.Primitives.TextPrimitive".GetHashCode(), typeof(TextPrimitive));
                this.knownTypes.Add("Telerik.WinControls.RadStylesheetRelation".GetHashCode(), typeof(RadStylesheetRelation));
                this.knownTypes.Add("Telerik.WinControls.XmlStyleBuilderRegistration".GetHashCode(), typeof(XmlStyleBuilderRegistration));
                this.knownTypes.Add("Telerik.WinControls.XmlStyleSheet".GetHashCode(), typeof(XmlStyleSheet));
                this.knownTypes.Add("Telerik.WinControls.XmlVisualStateSelector".GetHashCode(), typeof(XmlVisualStateSelector));
                this.knownTypes.Add("Telerik.WinControls.XmlClassSelector".GetHashCode(), typeof(XmlClassSelector));
                this.knownTypes.Add("Telerik.WinControls.XmlSimpleCondition".GetHashCode(), typeof(XmlSimpleCondition));
                this.knownTypes.Add("Telerik.WinControls.XmlComplexCondition".GetHashCode(), typeof(XmlComplexCondition));
                this.knownTypes.Add("Telerik.WinControls.RoundRectShape".GetHashCode(), typeof(RoundRectShape));
                this.knownTypes.Add("Telerik.WinControls.XmlTypeSelector".GetHashCode(), typeof(XmlTypeSelector));
            }
        }

        private Type GetKnownType(string className)
        {
            Type t;
            this.knownTypes.TryGetValue(className.GetHashCode(), out t);
            return t;
        }

        private void BuildLoadedAssemblies(Assembly[] asmArray)
        {
            this.loadedAssemblies.Clear();

            int length = asmArray.Length;
            for (int i = 0; i < length; i++)
            {
                Assembly asm = asmArray[i];
                LoadedAssembly loadedAsm = new LoadedAssembly(asm, IsTelerikAssembly(asm));
                this.loadedAssemblies.Add(loadedAsm);
            }

            this.lastParsedAssemblyCount = length;
        }

        private Type FindTypeByNameInAllAssemblies(string className)
        {
            return FindTypeByName(className, false);
        }

        private Type FindTypeByName(string className, bool onlyInTelerikAssemblies)
        {
            //check for know type
            Type knownType = this.GetKnownType(className);
            if (knownType != null)
            {
                RuntimeHelpers.RunClassConstructor(knownType.TypeHandle);
                return knownType;
            }

            //check for already loaded type
            Type loadedType;
            this.loadedTypes.TryGetValue(className.GetHashCode(), out loadedType);
            if (loadedType != null)
            {
                return loadedType;
            }

            Assembly[] asmArray = AppDomain.CurrentDomain.GetAssemblies();
            //re-initialize assemblies if needed
            if (asmArray.Length != this.lastParsedAssemblyCount)
            {
                this.BuildLoadedAssemblies(asmArray);
            }

            for (int i = 0; i < this.loadedAssemblies.Count; i++)
            {
                LoadedAssembly asm = this.loadedAssemblies[i];
                if (onlyInTelerikAssemblies && !asm.isTelerik)
                {
                    continue;
                }

                loadedType = asm.assembly.GetType(className, false, false);
                if (loadedType != null)
                {
                    break;
                }
            }

            if (loadedType != null)
            {
                this.loadedTypes.Add(className.GetHashCode(), loadedType);
                RuntimeHelpers.RunClassConstructor(loadedType.TypeHandle);
            }

            return loadedType;
        }

        #endregion

        #region Properties

        internal List<LoadedAssembly> LoadedAssemblies
        {
            get
            {
                if (this.loadedAssemblies.Count == 0)
                {
                    this.FindTypeByNameInAllAssemblies("RadControl");
                }

                return this.loadedAssemblies;
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether the TypeResolver should look up types in the calling assembly only.
        /// This option (if set to true) is very usefull particularly in the case when all the assemblies of the application, including the 
        /// Telerik assemblies are merged into a single assembly.
        /// </summary>
        public bool ResolveTypesInCurrentAssembly
        {
            get
            {
                return this.resolveTypesInCurrentAssembly;
            }
            set
            {
                this.resolveTypesInCurrentAssembly = value;
            }
        }

        /// <summary>
        /// Gets or sets value indicating the search pattern for assembly in the domain that contains the types referenced in RadControls theme files.
        /// <remarks>
        /// By default the types referencd in theme files are contained in assemblies with the name "Telerik"
        /// </remarks>
        /// </summary>
        public string TypeResolverAssemblyName
        {
            get
            {
                return typeResolverAssemblyName;
            }
            set
            {
                typeResolverAssemblyName = value;
            }
        }

        /// <summary>
        /// Gets or sets value indicating the version of the assembly specified in TypeResolverAssemblyName
        /// </summary>
        public Version TypeResolverAssemblyVersion
        {
            get
            {
                return typeResolverAssemblyVersion;
            }
            set
            {
                typeResolverAssemblyVersion = value;
            }
        }

        /// <summary>
        /// Gets the only instance of the resolver.
        /// </summary>
        public static RadTypeResolver Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new RadTypeResolver();
                        }
                    }
                }

                return instance;
            }
        }

        #endregion

        #region Fields

        private Dictionary<int, Type> loadedTypes;
        private Dictionary<int, Type> knownTypes;
        private List<LoadedAssembly> loadedAssemblies;
        private int lastParsedAssemblyCount;

        private const string TelerikName = "Telerik";
        private static RadTypeResolver instance;
        private static object syncRoot;

        private string typeResolverAssemblyName = TelerikName;
        private Version typeResolverAssemblyVersion;

        private bool resolveTypesInCurrentAssembly = false;

        #endregion

        #region Nested Types

        internal struct LoadedAssembly
        {
            public LoadedAssembly(Assembly assembly, bool isTelerik)
            {
                this.assembly = assembly;
                this.isTelerik = isTelerik;
            }

            public override int GetHashCode()
            {
 	             return this.assembly.FullName.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                LoadedAssembly asm = (LoadedAssembly)obj;
                return asm.assembly == this.assembly && asm.isTelerik == this.isTelerik;
            }

            public Assembly assembly;
            public bool isTelerik;
        }

        #endregion
    }
}
