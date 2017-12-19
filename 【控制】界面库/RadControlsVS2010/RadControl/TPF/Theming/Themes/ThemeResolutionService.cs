using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Primitives;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using Telerik.WinControls.Styles;
using Telerik.WinControls.Serialization;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Telerik.WinControls
{
	/// <summary>
    /// Represents event data for the 
    /// </summary>
	public class ThemeChangedEventArgs : EventArgs
	{
		private string themeName;

		public ThemeChangedEventArgs(string themeName)
		{
			this.themeName = themeName;
		}

		public string ThemeName
		{
			get { return themeName; }
		}
	}
    /// <summary>
    /// Represents the method that will handle a ThemeChanged event.
    /// </summary>
    /// <param name="sender">
    /// Initializes the event sender.  
    /// </param>
    /// <param name="args">
    /// Initializes the %event argument:ThemeChangedEventArgs%.
    /// </param>
	public delegate void ThemeChangedHandler(object sender, ThemeChangedEventArgs args);

    /// <summary>
    /// A Class that represents storage for themes in an application that contains
    /// RadControls.
    /// </summary>
    /// <remarks>
    /// 	<para>
    ///         A <see cref="Telerik.WinControls.Theme"/> consists of one or more
    ///         <strong>StyleSheet</strong> objects, and one or more
    ///         <strong>StyleSheetRelation</strong> objects.
    ///     </para>
    /// 	<para>
    /// 		<see cref="Telerik.WinControls.StyleSheet"/> object defines the appearance
    ///         and/or certain aspects of behavior of one <strong>RadControl</strong> or
    ///         <strong>RadItem</strong>.
    ///     </para>
    /// 	<para><strong>StyleSheetRelation</strong> objects contain data that maps a control
    ///     and a certain <strong>StyleSheet</strong>.</para>
    /// 	<para><strong>Theme.ThemeName</strong> is used by <strong>RadControl</strong> to
    ///     automatically query <strong>ThemeResolutionService</strong>, upon
    ///     <strong>Initialize</strong> and retrieve its <strong>StyleSheet</strong>. Use the
    ///     API of this class to register, un-register, query themes storage for specific
    ///     themes by attributes like Name, certain relations, etc.</para>
    /// </remarks>
    public class ThemeResolutionService
    {
        #region Nested Types

        private class TripleNameKey
        {
            // Fields
            private int hashCode;
            private string name1;
            private string name2;
            private string name3;

            // Methods
            public TripleNameKey(string name1, string name2, string name3)
            {
                this.name1 = name1;
                this.name2 = name2;
                this.name3 = name3;
                this.hashCode = this.Name1.ToUpper().GetHashCode() ^ this.Name2.ToUpper().GetHashCode() ^ this.Name3.GetHashCode();
            }

            public string Name1
            {
                get { return name1; }
            }

            public string Name2
            {
                get { return name2; }
            }

            public string Name3
            {
                get { return name3; }
            }

            public override bool Equals(object o)
            {
                if ((o != null) && (o is TripleNameKey))
                {
                    return this.Equals((TripleNameKey)o);
                }
                return false;
            }

            public bool Equals(TripleNameKey key)
            {
                if (string.Compare(this.Name1, key.Name1, true) == 0)
                {
                    if (string.Compare(this.Name2, key.Name2, true) == 0)
                    {
                        return string.Compare(this.Name3, key.Name3, true) == 0;
                    }
                }

                return false;
            }

            public override int GetHashCode()
            {
                return this.hashCode;
            }
        }

        private class ThemeChangeInfo
        {
            private string themeName;
            private string targetThemeClassName;

            public ThemeChangeInfo(string themeName, string targetThemeClassName)
            {
                this.themeName = themeName;
                this.targetThemeClassName = targetThemeClassName;
            }

            public string ThemeName
            {
                get { return themeName; }
            }

            public string TargetThemeClassName
            {
                get { return targetThemeClassName; }
            }

            public override bool Equals(object obj)
            {
                ThemeChangeInfo target = obj as ThemeChangeInfo;
                if (target == null)
                {
                    return false;
                }

                return target.themeName == this.themeName &&
                    target.targetThemeClassName == this.targetThemeClassName;
            }

            public override int GetHashCode()
            {
                return this.themeName.GetHashCode() ^ this.targetThemeClassName.GetHashCode();
            }
        }

        public class ResourceParams
        {
            public string ResourcePath;
            public Assembly CallingAssembly;
            public Assembly ExecutingAssembly;
            public Assembly UserAssembly;
        }

        #endregion

        #region Fields

        private static bool allowAnimations = true;
		private static string applicationThemeName = null;
        private const string asteriskThemeName = "*";
        //list with all registered listeners for theme change
        private static WeakReferenceList<IThemeChangeListener> themeChangeListeners;
        private static object syncRoot = new object();

        private static Hashtable registeredBuildersName = new Hashtable();
        private static Hashtable registeredBuildersByElementNameControlID = new Hashtable();
        private static Hashtable registeredBuildersByElementNameControlType = new Hashtable();
        private static Hashtable registeredBuildersByElementTypeControlID = new Hashtable();
        private static Hashtable registeredBuildersByElementTypeControlType = new Hashtable();
        private static Hashtable registeredBuildersDefaultByElementType = new Hashtable();
        private static Hashtable registeredStyleRepositoriesByThemeName = new Hashtable();
        private static Dictionary<string, object> loadedResourcePackages = new Dictionary<string, object>();
        private static Hashtable registeredThemes = new Hashtable();

        private static int themeChangeSuspendCounter = 0;
        private static LinkedList<ThemeChangeInfo> themesChangeDuringSuspend = new LinkedList<ThemeChangeInfo>();

        #endregion

        #region Constructor

        static ThemeResolutionService()
        {
            themeChangeListeners = new WeakReferenceList<IThemeChangeListener>(true);
        }

        #endregion

        #region Events

        public static event ResolveStyleBuilderEventHandler ResolveStyleBuilder;

        public static event ThemeChangedHandler ApplicationThemeChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets value indicating the theme name that will be used by all controls in the application.
        /// </summary>
        /// <remarks>
        /// If the value of this property is null or empty each control will be assigned a theme, corresponding on the 
        /// <see cref="RadControl.ThemeName"/> property of the control. Otherwise the ThemeName property will be disregarded.
        /// If a sopecific control in the application has no theme registered with the name specified by ApplicationThemeName, it will be
        /// assigned its ControlDefault theme name.
        /// </remarks>
        public static string ApplicationThemeName
        {
            get
            {
                return applicationThemeName;
            }
            set
            {
                applicationThemeName = value;

                //Why loop?
                //foreach (Theme theme in GetAvailableThemes())
                //{
                //    RaiseThemeChanged(theme.ThemeName);
                //}

                RaiseThemeChanged(applicationThemeName);

                if (ApplicationThemeChanged != null)
                    ApplicationThemeChanged(null, new ThemeChangedEventArgs(applicationThemeName));
            }
        }

        /// <summary>
        /// Determines whether animations are allowed across entire application.
        /// </summary>
        public static bool AllowAnimations
        {
            get
            {
                return allowAnimations;
            }
            set
            {
                allowAnimations = value;
            }
        }

        /// <summary>
        /// "ControlDefault" theme name
        /// </summary>
        public static string ControlDefaultThemeName
        {
            get
            {
                return "ControlDefault";
            }
        }

        public static string SystemThemeName
        {
            get
            {
                return "System";
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads a theme package, stored in the provided file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool LoadPackageFile(string filePath)
        {
            return LoadPackageFile(filePath, true);
        }

        /// <summary>
        /// Loads a theme package stored in the provided file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="throwOnError">True to throw an exception if it occurs, false otherwise.</param>
        /// <returns></returns>
        public static bool LoadPackageFile(string filePath, bool throwOnError)
        {
            if (!File.Exists(filePath))
            {
                if (throwOnError)
                {
                    throw new FileNotFoundException("File '" + filePath + "' does not exist.");
                }

                return false;
            }

            FileInfo fi = new FileInfo(filePath);
            if (fi.Extension != "."+RadThemePackage.FileExtension)
            {
                if (throwOnError)
                {
                    throw new ArgumentException("Provided file is not a valid RadThemePackage");
                }

                return false;
            }

            RadThemePackage package = RadThemePackage.Decompress(filePath, typeof(RadThemePackage)) as RadThemePackage;
            if (package == null)
            {
                return false;
            }

            return LoadPackage(package, throwOnError);
        }

        /// <summary>
        /// Loads a theme package, stored in the provided embedded resource.
        /// The calling assembly is used to read the manifest resource stream.
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <returns></returns>
        public static bool LoadPackageResource(string resourcePath)
        {
            ResourceParams resourceParams = new ResourceParams();
            resourceParams.CallingAssembly = Assembly.GetCallingAssembly();
            resourceParams.ExecutingAssembly = Assembly.GetExecutingAssembly();
            resourceParams.ResourcePath = resourcePath;
            return LoadPackageResource(resourceParams, true);
        }

        /// <summary>
        /// Loads a theme package from an embedded resource in the specified assembly.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool LoadPackageResource(Assembly sourceAssembly, string resourcePath)
        {
            ResourceParams resourceParams = new ResourceParams();
            resourceParams.UserAssembly = sourceAssembly;
            resourceParams.ResourcePath = resourcePath;
            return LoadPackageResource(resourceParams, true);
        }

        /// <summary>
        /// Loads a theme package stored in the provided embedded resource.
        /// </summary>
        /// <param name="resourceParams"></param>
        /// <param name="throwOnError">True to throw an exception if it occurs, false otherwise.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool LoadPackageResource(ResourceParams resourceParams, bool throwOnError)
        {
            if (loadedResourcePackages.ContainsKey(resourceParams.ResourcePath))
            {
                return true;
            }

            Assembly assembly = null;
            assembly = resourceParams.UserAssembly;
            string resourcePath = resourceParams.ResourcePath;

            if (assembly == null)
            {
                assembly = resourceParams.CallingAssembly;

                if (assembly == null)
                {
                    assembly = Assembly.GetCallingAssembly();
                }
            }

            Stream stream = assembly.GetManifestResourceStream(resourcePath);

            if (stream == null)
            {
                assembly = resourceParams.ExecutingAssembly;

                if (assembly == null)
                {
                    assembly = Assembly.GetExecutingAssembly();
                }

                stream = assembly.GetManifestResourceStream(resourcePath);
            }

            if (stream == null)
            {
                if (throwOnError)
                {
                    throw new ArgumentException("Specified resource does not exist in the provided assembly.");
                }

                return false;
            }

            RadThemePackage package = RadThemePackage.Decompress(stream, typeof(RadThemePackage)) as RadThemePackage;
            if (package == null)
            {
                return false;
            }

            //remember this location and do not load it again
            loadedResourcePackages[resourceParams.ResourcePath] = null;

            return LoadPackage(package, throwOnError);
        }

        /// <summary>
        /// Call to subscribe for theme change event, for the specified control theme class name. Note the event may be fired from another thread
        /// </summary>
        /// <param name="listener"></param>
        public static void SubscribeForThemeChanged(IThemeChangeListener listener)
        {
            lock (syncRoot)
            {
                themeChangeListeners.Add(listener);
            }
        }

        public static void UnsubscribeFromThemeChanged(IThemeChangeListener listener)
        {
            lock (syncRoot)
            {
                themeChangeListeners.Remove(listener);
            }
        }

        /// <summary>
        /// Gets any themes registered for a specific control by control type name or control name.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static ThemeList GetAvailableThemes(IComponentTreeHandler control)
        {
            ThemeList res = new ThemeList();
            if (control == null)
                return res;

            foreach (DictionaryEntry entry in ThemeResolutionService.registeredThemes)
            {
                Theme theme = (Theme)entry.Value;

                if (theme.ThemeName == asteriskThemeName)
                    continue;

                bool found = false;
                foreach (StyleBuilderRegistration builder in theme.GetRegisteredStyleBuilders())
                {
                    foreach (RadStylesheetRelation relation in builder.StylesheetRelations)
                    {
                        if ((relation.ControlType == control.ThemeClassName /*&& relation.ElementType == typeof(RootRadElement).FullName*/)
                            ||
                            (!string.IsNullOrEmpty(relation.ControlName) && relation.ControlName == control.Name))
                        {
                            found = true;
                            break;
                        }
                    }
                }

                if (found)
                    res.Add(theme);
            }

            return res;
        }

        /// <summary>
        /// Registers a theme from a given assembly. The method enumerates all resources names
        /// in the assembly and checks whether the resource name contains the given theme name.
        /// If the resource name is a name of a XML file and contains the theme name as a substring,
        /// this file is considered part of the theme which should be loaded and is consequently registered
        /// in the <see cref="ThemeResolutionService"/>.
        /// </summary>
        /// <param name="assembly">The source assembly from which to load the theme.</param>
        /// <param name="themeName">The theme name which is used to recognize the corresponding resources.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void RegisterThemeFromAssembly(Assembly assembly, string themeName)
        {
            string[] manifestResourceNames = assembly.GetManifestResourceNames();

            foreach (string currentName in manifestResourceNames)
            {
                if (!currentName.Contains(themeName) || !currentName.EndsWith(".xml"))
                {
                    continue;
                }

                ThemeSource source = new ThemeSource();
                source.SetCallingAssembly(Assembly.GetCallingAssembly());
                source.StorageType = ThemeStorageType.Resource;
                source.ThemeLocation = currentName;
                source.ReloadThemeFromStorage();
            }
        }

        /// <summary>
        /// Regesters theme from a file or resource that contains a xml-serilized Theme object.
        /// The Visual Style Builder application for example is capable of designing and serializing
        /// themes. Theme files generally contain Theme with one or several style sheets each assigned a 
        /// registration that defines which RadControl and/or RadElment the style sheet applies.
        /// </summary>
        /// <param name="storageType"></param>
        /// <param name="themeLocation"></param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void RegisterThemeFromStorage(ThemeStorageType storageType, string themeLocation)
        {
            ResourceParams resParams = new ResourceParams();
            resParams.CallingAssembly = Assembly.GetCallingAssembly();
            resParams.ExecutingAssembly = Assembly.GetExecutingAssembly();
            resParams.ResourcePath = themeLocation;

            RegisterThemeFromStorage(storageType, resParams);
        }

        /// <summary>
        /// Regesters theme from a file or resource that contains a xml-serilized Theme object.
        /// The Visual Style Builder application for example is capable of designing and serializing
        /// themes. Theme files generally contain Theme with one or several style sheets each assigned a 
        /// registration that defines which RadControl and/or RadElment the style sheet applies.
        /// </summary>
        /// <param name="storageType"></param>
        /// <param name="assembly"></param>
        /// <param name="themeLocation"></param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void RegisterThemeFromStorage(ThemeStorageType storageType, Assembly assembly, string themeLocation)
        {
            ResourceParams resParams = new ResourceParams();
            resParams.CallingAssembly = Assembly.GetCallingAssembly();
            resParams.ExecutingAssembly = Assembly.GetExecutingAssembly();
            resParams.ResourcePath = themeLocation;
            resParams.UserAssembly = assembly;

            RegisterThemeFromStorage(storageType, resParams);
        }

        /// <summary>
        /// Regesters theme from a file or resource that contains a xml-serilized Theme object.
        /// The Visual Style Builder application for example is capable of designing and serializing
        /// themes. Theme files generally contain Theme with one or several style sheets each assigned a 
        /// registration that defines which RadControl and/or RadElment the style sheet applies.
        /// </summary>
        /// <param name="storageType"></param>
        /// <param name="resourceParams"></param>
        public static void RegisterThemeFromStorage(ThemeStorageType storageType, ResourceParams resourceParams)
        {
            ThemeSource source = new ThemeSource();
            Assembly lookUpAssembly = resourceParams.UserAssembly;
            if (lookUpAssembly == null)
            {
                lookUpAssembly = resourceParams.CallingAssembly;
            }

            source.SetCallingAssembly(lookUpAssembly);
            source.StorageType = storageType;
            source.ThemeLocation = resourceParams.ResourcePath;
            source.ReloadThemeFromStorage();
        }

        /// <summary>
        /// Removes an instance of the <see cref="StyleBuilder"/> class
        /// from the dictionaries with registered style builders.
        /// </summary>
        /// <param name="builder">The instance to remove.</param>
        public static void UnregisterStyleSheetBuilder(StyleBuilder builder)
        {
            foreach (DictionaryEntry entry in registeredBuildersDefaultByElementType)
            {
                if (object.ReferenceEquals(builder, entry.Value))
                {
                    registeredBuildersDefaultByElementType.Remove(entry.Key);
                    return;
                }
            }

            foreach (DictionaryEntry entry in registeredBuildersByElementTypeControlType)
            {
                if (object.ReferenceEquals(builder, entry.Value))
                {
                    registeredBuildersByElementTypeControlType.Remove(entry.Key);
                    return;
                }
            }

            foreach (DictionaryEntry entry in registeredBuildersName)
            {
                if (object.ReferenceEquals(builder, entry.Value))
                {
                    registeredBuildersName.Remove(entry.Key);
                    return;
                }
            }
        }

        /// <summary>
        /// Gets all StyleSheets registered under a theme name.
        /// </summary>
        /// <param name="themeName"></param>
        /// <returns></returns>
        public static StyleBuilderRegistration[] GetStyleSheetBuilders(string themeName)
        {
            if (string.IsNullOrEmpty(themeName))
            {
                themeName = ControlDefaultThemeName;
            }

            ArrayList res = new ArrayList();

            foreach (DictionaryEntry entry in registeredBuildersDefaultByElementType)
            {
                TripleNameKey key = (TripleNameKey)entry.Key;
                if (string.Compare(key.Name2, themeName, true) == 0)
                {
                    StyleBuilder builder = (StyleBuilder)entry.Value;


                    BuilderRegistrationType regType = BuilderRegistrationType.ElementTypeDefault;
                    string elementType = key.Name3;
                    string controlType = null;
                    string elementName = null;
                    string controlName = null;

                    AddBuilderToList(res, builder, regType, elementType, controlType, elementName, controlName);
                }
            }

            foreach (DictionaryEntry entry in registeredBuildersByElementTypeControlType)
            {
                TripleNameKey key = (TripleNameKey)entry.Key;
                if (string.Compare(key.Name3, themeName, true) == 0)
                {
                    StyleBuilder builder = (StyleBuilder)entry.Value;
                    BuilderRegistrationType regType = BuilderRegistrationType.ElementTypeControlType;
                    string elementType = key.Name2;
                    string controlType = key.Name1;
                    string elementName = null;
                    string controlName = null;

                    AddBuilderToList(res, builder, regType, elementType, controlType, elementName, controlName);
                }
            }

            foreach (DictionaryEntry entry in registeredBuildersName)
            {
                string key = (string)entry.Key;
                if (string.Compare(key, themeName, true) == 0)
                {
                    StyleBuilder builder = (StyleBuilder)entry.Value;
                    BuilderRegistrationType regType = BuilderRegistrationType.ElementTypeGlobal;
                    string elementType = null;
                    string controlType = null;
                    string elementName = null;
                    string controlName = null;

                    AddBuilderToList(res, builder, regType, elementType, controlType, elementName, controlName);
                }
            }

            StyleBuilderRegistration[] resArray = new StyleBuilderRegistration[res.Count];
            res.CopyTo(resArray, 0);

            return resArray;
        }

        public static StyleBuilder GetStyleSheetBuilder(RadElement element)
        {
            return GetStyleSheetBuilder(element, null);
        }

        public static StyleBuilder GetStyleSheetBuilder(RadElement element, string proposedThemeName)
        {
            //Optimization: only RadItems can obtain styles from TRS...
            if (!element.CanHaveOwnStyle)
            {
                return null;
            }
            //-------------

            IComponentTreeHandler control = null;

            if (element.ElementTree != null)
            {
                control = element.ElementTree.ComponentTreeHandler;
                if (control == null || control.ElementTree.CallControlDefinesThemeForElement(element))
                {
                    return null;
                }
            }

            string elementTypeFullName = element.GetThemeEffectiveType().FullName;
            string elementName = element.Name;

            bool rootElementHasStyle = !(element is RootRadElement) && element.ElementTree != null;
            StyleSheet rootElementStyle = null;
            if (rootElementHasStyle)
            {
                rootElementStyle = element.ElementTree.RootElement.Style;
            }

            return GetStyleSheetBuilder(control, elementTypeFullName, elementName, proposedThemeName, rootElementHasStyle, rootElementStyle);
        }

        public static StyleBuilder GetStyleSheetBuilder(IComponentTreeHandler control, string elementTypeFullName, string elementName, string proposedThemeName)
        {
            return GetStyleSheetBuilder(control, elementTypeFullName, elementName, proposedThemeName, false, null);
        }

        public static void RegisterGlobalStyleBuilder(string themeName, StyleBuilder builder)
        {
            EnsureThemeRegistered(themeName);

            registeredBuildersName[themeName] = builder;

            RaiseThemeChanged(themeName);
        }

        public static void RegisterElementTypeDefaultStyleBuilder(string themeName, string elementTypeName, StyleBuilder builder)
        {
            EnsureThemeRegistered(ControlDefaultThemeName);

            TripleNameKey key = new TripleNameKey("", themeName, elementTypeName);
            registeredBuildersDefaultByElementType[key] = builder;
        }

        /// <summary>
        /// Registers a StyleBuilder for specific type of controls and specific type of elements under the name given.
        /// </summary>
        /// <param name="controlTypeName"></param>
        /// <param name="elementTypeName"></param>
        /// <param name="builder"></param>
        /// <param name="themeName"></param>
        public static void RegisterControlStyleBuilder(string controlTypeName, string elementTypeName, StyleBuilder builder, string themeName)
        {
            EnsureThemeRegistered(themeName);

            TripleNameKey key = new TripleNameKey(controlTypeName, elementTypeName, themeName);
            registeredBuildersByElementTypeControlType[key] = builder;

            RaiseThemeChanged(themeName, controlTypeName);
        }

        public static StyleBuilder GetRegisteredControlStyleBuilder(string controlTypeName, string elementTypeName, string themeName)
        {
            TripleNameKey key = new TripleNameKey(controlTypeName, elementTypeName, themeName);
            return registeredBuildersByElementTypeControlType[key] as StyleBuilder;
        }

        public static void RegisterStyleBuilderByControlName(string controlName, string elementTypeName, StyleBuilder builder, string themeName)
        {
            EnsureThemeRegistered(themeName);

            TripleNameKey key = new TripleNameKey("__ID" + controlName, elementTypeName, themeName);
            registeredBuildersByElementTypeControlID[key] = builder;
        }

        /// <summary>
        /// Creates and registeres an emty Theme if one is not alreay registered.
        /// </summary>
        /// <param name="themeName"></param>
        /// <returns></returns>
        public static Theme EnsureThemeRegistered(string themeName)
        {
            Theme theme = registeredThemes[themeName] as Theme;
            if (theme == null)
            {
                theme = new Theme(themeName);
                registeredThemes[themeName] = theme;
            }

            return theme;
        }

        /// <summary>
        /// Unregisters a <see cref="Theme"/> instance from the ThemeResolutionService.
        /// </summary>
        /// <param name="themeName">The name of the theme to unregister</param>
        /// <returns>True if theme unregistered, otherwise false.</returns>
        public static bool RemoveThemeRegistration(string themeName)
        {
            Theme theme = registeredThemes[themeName] as Theme;
            if (theme != null)
            {
                registeredThemes.Remove(themeName);
                XmlStyleRepository repository = registeredStyleRepositoriesByThemeName[themeName] as XmlStyleRepository;
                if (repository != null)
                {
                    registeredStyleRepositoriesByThemeName.Remove(themeName);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets a list of all registered themes.
        /// </summary>
        /// <returns></returns>
        public static ThemeList GetAvailableThemes()
        {
            ThemeList res = new ThemeList();
            foreach (DictionaryEntry entry in ThemeResolutionService.registeredThemes)
            {
                Theme theme = (Theme)entry.Value;
                if (theme.ThemeName != asteriskThemeName)
                {
                    res.Add((Theme)entry.Value);
                }
            }

            return res;
        }

        /// <summary>
        /// Get previously registered theme by theme name.
        /// </summary>
        /// <param name="themeName"></param>
        /// <returns></returns>
        public static Theme GetTheme(string themeName)
        {
            Theme res = null;
            if (themeName != null)
            {
                res = registeredThemes[themeName] as Theme;
            }

            return res;
        }

        /// <summary>
        /// Registers a StyleSheet found in styleBuilderRegistration using also the registration details specified under the theme name specified.
        /// </summary>
        /// <param name="styleBuilderRegistration"></param>
        /// <param name="themeName"></param>
        public static void RegisterStyleBuilder(StyleBuilderRegistration styleBuilderRegistration, string themeName)
        {
            foreach (RadStylesheetRelation relation in styleBuilderRegistration.StylesheetRelations)
            {
                switch (relation.RegistrationType)
                {
                    case BuilderRegistrationType.ElementTypeDefault:
                        ThemeResolutionService.RegisterElementTypeDefaultStyleBuilder(
                            themeName, relation.ElementType, styleBuilderRegistration.Builder);
                        break;
                    case BuilderRegistrationType.ElementTypeControlType:
                        ThemeResolutionService.RegisterControlStyleBuilder(
                            relation.ControlType,
                            relation.ElementType,
                            styleBuilderRegistration.Builder,
                            themeName);
                        break;
                    /*case BuilderRegistrationType.ElementNameControlType:
                ThemeResolutionService.RegisterControlStyleBuilder(
                    styleBuilderRegistration.ControlType,
                    styleBuilderRegistration.ElementName,
                    styleBuilderRegistration.Builder,
                    themeName);
                break;*/
                    case BuilderRegistrationType.ElementTypeControlName:
                        ThemeResolutionService.RegisterStyleBuilderByControlName(
                            relation.ControlName,
                            relation.ElementType,
                            styleBuilderRegistration.Builder,
                            themeName);
                        break;/*
				case BuilderRegistrationType.ElementNameControlName:
					ThemeResolutionService.RegisterControlStyleBuilder(
						styleBuilderRegistration.ControlName,
						styleBuilderRegistration.ElementName,
						styleBuilderRegistration.Builder,
						themeName);
					break;*/
                }
            }

            EnsureThemeRegistered(themeName);
            GetTheme(themeName).StyleBuilderRegistered(styleBuilderRegistration);
        }

        /// <summary>
        ///  Clears all stylesheets registered previously with the themeName specified.
        /// </summary>
        /// <param name="themeName"></param>
        public static void ClearTheme(string themeName)
        {
            lock (syncRoot)
            {
                ArrayList toDelete = new ArrayList();
                foreach (DictionaryEntry entry in registeredBuildersByElementTypeControlType)
                {
                    TripleNameKey key = (TripleNameKey)entry.Key;
                    if (string.Compare(key.Name3, themeName, true) == 0)
                    {
                        toDelete.Add(key);
                    }
                }

                foreach (object key in toDelete)
                {
                    registeredBuildersByElementTypeControlType.Remove(key);
                }

                toDelete.Clear();

                foreach (DictionaryEntry entry in registeredBuildersByElementNameControlType)
                {
                    TripleNameKey key = (TripleNameKey)entry.Key;
                    if (string.Compare(key.Name3, themeName, true) == 0)
                    {
                        toDelete.Add(key);
                    }
                }

                foreach (object key in toDelete)
                {
                    registeredBuildersByElementNameControlType.Remove(key);
                }

                toDelete.Clear();

                foreach (DictionaryEntry entry in registeredBuildersByElementTypeControlID)
                {
                    TripleNameKey key = (TripleNameKey)entry.Key;
                    if (string.Compare(key.Name3, themeName, true) == 0)
                    {
                        toDelete.Add(key);
                    }
                }

                foreach (object key in toDelete)
                {
                    registeredBuildersByElementTypeControlID.Remove(key);
                }

                toDelete.Clear();

                foreach (DictionaryEntry entry in registeredBuildersDefaultByElementType)
                {
                    TripleNameKey key = (TripleNameKey)entry.Key;
                    if (string.Compare(key.Name3, themeName, true) == 0)
                    {
                        toDelete.Add(key);
                    }
                }

                foreach (object key in toDelete)
                {
                    registeredBuildersDefaultByElementType.Remove(key);
                }

                toDelete.Clear();

                foreach (DictionaryEntry entry in registeredBuildersName)
                {
                    string key = (string)entry.Key;
                    if (string.Compare(key, themeName, true) == 0)
                    {
                        toDelete.Add(key);
                    }
                }

                foreach (object key in toDelete)
                {
                    registeredBuildersName.Remove(key);
                }
            }

            RaiseThemeChanged(themeName);
        }

        /// <summary>
        /// Applies the specified ThemeName to all RadControls that are children of the specified Control and its child Controls
        /// </summary>
        /// <param name="control"></param>
        /// <param name="themeName"></param>
        public static void ApplyThemeToControlTree(Control control, string themeName)
        {
            IComponentTreeHandler radControl = control as IComponentTreeHandler;
            if (radControl != null)
            {
                radControl.ThemeName = themeName;
            }

            foreach (Control child in control.Controls)
            {
                ApplyThemeToControlTree(child, themeName);
            }
        }

        public static void RegisterThemeRepository(Telerik.WinControls.Styles.XmlStyleRepository styleRepository, string themeName)
        {
            XmlStyleRepository existingRepository = registeredStyleRepositoriesByThemeName[themeName] as XmlStyleRepository;
            if (existingRepository != null)
            {
                existingRepository.MergeWith(styleRepository);
            }
            else
            {
                registeredStyleRepositoriesByThemeName[themeName] = styleRepository;
            }

            RaiseThemeChanged(themeName);
        }

        public static XmlStyleRepository GetThemeRepository(string themeName)
        {
            return GetThemeRepository(themeName, true);
        }

        public static XmlStyleRepository GetThemeRepository(string themeName, bool useAsterikTheme)
        {
            return GetThemeRepository(themeName, useAsterikTheme, true);
        }

        public static XmlStyleRepository GetThemeRepository(string themeName, bool useAsterikTheme, bool useAppThemeName)
        {
            if (useAppThemeName && !string.IsNullOrEmpty(ApplicationThemeName))
            {
                themeName = ApplicationThemeName;
            }
            else if (string.IsNullOrEmpty(themeName))
            {
                themeName = ThemeResolutionService.ControlDefaultThemeName;
            }

            XmlStyleRepository registered = registeredStyleRepositoriesByThemeName[themeName] as XmlStyleRepository;
            if (registered == null && useAsterikTheme)
            {
                registered = registeredStyleRepositoriesByThemeName[asteriskThemeName] as XmlStyleRepository;
            }

            return registered;
        }

        #endregion

        #region Internals

        internal static void RaiseThemeChanged(string themeName)
        {
            RaiseThemeChanged(themeName, null);
        }

        /// <summary>
        /// Method not available
        /// </summary>
        /// <param name="control"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        //TODO: Implement and change to public
        internal static bool HasBuildersFor(RadControl control, string p)
        {
            return false;
        }

        internal static void SuspendThemeChange()
        {
            themeChangeSuspendCounter++;
        }

        internal static void ResumeThemeChange()
        {
            ResumeThemeChange(true);
        }

        internal static void ResumeThemeChange(bool raiseChanged)
        {
            lock (syncRoot)
            {
                if (themeChangeSuspendCounter > 0)
                {
                    themeChangeSuspendCounter--;
                }

                if (themeChangeSuspendCounter == 0)
                {
                    if (raiseChanged)
                    {
                        foreach (ThemeChangeInfo changedTheme in themesChangeDuringSuspend)
                        {
                            RaiseThemeChanged(changedTheme.ThemeName, changedTheme.TargetThemeClassName);
                        }
                    }

                    themesChangeDuringSuspend.Clear();
                }
            }
        }

        #endregion

        #region Private Implementation

        private static bool LoadPackage(RadThemePackage package, bool throwOnError)
        {
            try
            {
                foreach (XmlTheme xmlTheme in package.DecompressThemes())
                {
                    Theme.Deserialize(xmlTheme);
                }

                return true;
            }
            catch (Exception ex)
            {
                if (throwOnError)
                {
                    throw;
                }

                Debug.Fail("Failed to decompress themes from package. Exception was:\r\n" + ex);
                return false;
            }
        }

        private static void RaiseThemeChanged(string themeName, string controlThemeClassName)
        {
            lock (syncRoot)
            {
                if (themeChangeSuspendCounter > 0)
                {
                    ThemeChangeInfo info = new ThemeChangeInfo(themeName, controlThemeClassName);
                    if (!themesChangeDuringSuspend.Contains(info))
                    {
                        themesChangeDuringSuspend.AddLast(info);
                    }

                    return;
                }

                ThemeChangedEventArgs args = new ThemeChangedEventArgs(themeName);

                foreach (IThemeChangeListener listener in themeChangeListeners)
                {
                    if (!string.IsNullOrEmpty(controlThemeClassName) &&
                        String.CompareOrdinal(controlThemeClassName, listener.ControlThemeClassName) != 0)
                    {
                        continue;
                    }
                    listener.OnThemeChanged(args);
                }
            }
        }

        private static void AddBuilderToList(ArrayList res, StyleBuilder builder, BuilderRegistrationType regType, string elementType, string controlType, string elementName, string controlName)
        {
            bool found = false;
            foreach (StyleBuilderRegistration reg in res)
            {
                if (reg.Builder == builder)
                {
                    RadStylesheetRelation relation = new RadStylesheetRelation(
                        regType, elementType, controlType, elementName, controlName);
                    reg.StylesheetRelations.Add(relation);
                    found = true;
                }
            }

            if (!found)
            {
                StyleBuilderRegistration registration = new StyleBuilderRegistration(
                    regType, elementType, controlType, elementName, controlName, builder);
                res.Add(registration);
            }
        }

        private static StyleBuilder GetStyleSheetBuilder(IComponentTreeHandler /*RadControl*/ control, string elementTypeFullName, string elementName, string proposedThemeName, bool rootElementHasStyle, StyleSheet rootElementStyle)
        {
            string controlType = typeof(RadControl).FullName;
            string controlThemeName = null;

            string controlID = string.Empty;
            if (control != null)
            {
                controlThemeName = control.ThemeName;

                Control controlHandler = control as Control;
                if (controlHandler != null)
                {
                    controlID = controlHandler.Name;
                }
                controlType = control.ThemeClassName;
            }

            StyleBuilder res = null;

            //ThemeName Look up priority:
            //1. applicationThemeName
            //2. proposedThemeName
            //3. controlThemeName
            //4. ControlDefault
            //5. * theme

            if (applicationThemeName != null)
            {
                res = LookUpStyleBuilder(controlID, controlType, elementName, elementTypeFullName, rootElementHasStyle,
                                       rootElementStyle, applicationThemeName);
            }

            bool lookUpOnlyAsteriskTheme = false; //true means - theme name specified correctly, but no stylesheet found

            if (res == null && !string.IsNullOrEmpty(proposedThemeName))
            {
                lookUpOnlyAsteriskTheme = true;
                res = LookUpStyleBuilder(controlID, controlType, elementName, elementTypeFullName, rootElementHasStyle,
                                       rootElementStyle, proposedThemeName);
            }

            if (!lookUpOnlyAsteriskTheme && res == null && !string.IsNullOrEmpty(controlThemeName))
            {
                lookUpOnlyAsteriskTheme = true;
                res = LookUpStyleBuilder(controlID, controlType, elementName, elementTypeFullName, rootElementHasStyle,
                                       rootElementStyle, controlThemeName);
            }

            if (!lookUpOnlyAsteriskTheme && res == null)
            {
                res = LookUpStyleBuilder(controlID, controlType, elementName, elementTypeFullName, rootElementHasStyle,
                                       rootElementStyle, ControlDefaultThemeName);
            }

            //Now if no stylesheet found, look if "*" registered
            if (res == null)
            {
                res = LookUpStyleBuilder(controlID, controlType, elementName, elementTypeFullName, rootElementHasStyle,
                                       rootElementStyle, asteriskThemeName);
            }

            return res;
        }

        private static StyleBuilder LookUpStyleBuilder(string controlID, string controlType, string elementName, string elementTypeFullName, bool rootElementHasStyle, StyleSheet rootElementStyle, string themeName)
        {
            //priority of resolution

            //Find builder by ElementName + ControlID
            //Find builder by ElementName + ControlType.Name
            //Find builder by ElementType + ControlID
            //Find builder by ElementType + ControlType.Name
            //Find Element default builder

            TripleNameKey key = new TripleNameKey(elementName + "", themeName + "", elementTypeFullName);

            StyleBuilder res = registeredBuildersByElementNameControlID[key] as StyleBuilder;

            int resolution = 0;

            if (res == null)
            {
                key = new TripleNameKey(controlID + "", themeName + "", elementTypeFullName);
                res = registeredBuildersByElementTypeControlID[key] as StyleBuilder;
                resolution = 1;
            }

            if (res == null)
            {
                TripleNameKey key1 = new TripleNameKey(controlType, elementTypeFullName, themeName + "");
                res = registeredBuildersByElementTypeControlID[key1] as StyleBuilder;
                resolution = 2;
            }

            if (res == null)
            {
                TripleNameKey key1 = new TripleNameKey(controlType, elementTypeFullName, themeName + "");
                res = registeredBuildersByElementTypeControlType[key1] as StyleBuilder;
                resolution = 3;
            }

            if (res == null)
            {
                TripleNameKey key1 = new TripleNameKey("", themeName + "", elementTypeFullName);
                res = registeredBuildersDefaultByElementType[key1] as StyleBuilder;
                resolution = 4;
            }

            if (res == null && themeName != null)
            {
                res = registeredBuildersName[themeName] as StyleBuilder;
                resolution = 5;
            }

            if (resolution == 4)
            {
                //Fix for "same stylresheet on element only and its control" propblem
                if (rootElementHasStyle &&
                    res is DefaultStyleBuilder &&
                    rootElementStyle == ((DefaultStyleBuilder)res).Style)
                {
                    return null;
                }
            }

            ResolveStyleBuilderEventHandler eh = ResolveStyleBuilder;
            if (eh != null)
            {
                ResolveStyleBuilderEventArgs args = new ResolveStyleBuilderEventArgs(themeName, res);
                eh(null, args);
                res = args.Builder;
            }

            return res;
        }

        #endregion
    }
}
