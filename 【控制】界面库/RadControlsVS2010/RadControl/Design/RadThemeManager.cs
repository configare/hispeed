using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Reflection;
using System.Text;
using System.Collections;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel.Design;
using Telerik.WinControls.Design;

namespace Telerik.WinControls
{
    /// <summary>
    /// Theme manager Component is used to load user-defined themes for RadControls in an application. 
    /// Use the LoadedThemes property to add new team source files. Themes load immediately when correct 
    /// property values specified and last for the life time of the application. After a theme is loaded 
    /// it can be used by the corresponding types of controls placed on any Form of the application. 
    /// </summary>
    [TelerikToolboxCategory(ToolboxGroupStrings.ThemesGroup)]
	[Designer(DesignerConsts.RadThemeManagerDesignerString)]
    [ToolboxItemFilter("System.Windows.Forms")]
    public class RadThemeManager: Component
    {
        private ThemeSourceCollection loadedThemes;
        

        public RadThemeManager()
        {
            this.loadedThemes = new ThemeSourceCollection(this);            
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ThemeSourceCollection LoadedThemes
        {
            get { return loadedThemes; }
        }

        internal bool IsDesignMode
        {
            get{ return base.DesignMode; }
        }

        /// <summary>
        /// Gets a list theme names currently loaded by this instance
        /// </summary>
        [ReadOnly(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Indicated currently loaded theme names")]
        [TypeConverter(typeof(LoadedThemeNamesConverter))]
        public string[] LoadedThemeNames
        {
            get
            {
                if (loadedThemes == null)
                    return null;
                string[] themeNames = new string[loadedThemes.Count];
                for (int i = 0; i < loadedThemes.Count; i++)
                {
                    XmlTheme theme = loadedThemes[i].GetXmlThemeObject();
                    if (theme != null)
                        themeNames[i] = theme.ThemeName;
                }
                return themeNames;
            }
        }

        private class LoadedThemeNamesConverter: TypeConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext context, Type sourceType)
            {
                return sourceType == typeof(string);
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (value == null || value.GetType() != typeof(string[]) || destinationType != typeof(string))
                    return string.Empty;

                string[] res = (string[])value;

                System.Globalization.CultureInfo validCulture = Thread.CurrentThread.CurrentUICulture;
                if (culture != null)
                {
                    validCulture = culture;
                }

                return string.Join(validCulture.TextInfo.ListSeparator + " ", res);
            }
        }
	}

    /// <summary>
    /// Defines the theme storage type.
    /// </summary>
    public enum 
        ThemeStorageType
    {
        /// <summary>
        /// Indicates that the theme is contained in a external file.
        /// </summary>
        File,

        /// <summary>
        /// Indicates that the theme is contained as a resource.
        /// </summary>
        Resource
    }

    /// <summary>
    /// ThemeSource is used to load user-defined themes for RadControls in an application. 
    /// Themes load immediately when correct property values specified and last for the life 
    /// time of the application. After a theme is loaded it can be used by the corresponding 
    /// types of controls placed on any Form of the application. ThemeSource object are generally 
    /// used by ThemeManager component placed on a Form
    /// </summary>
    public class ThemeSource: RadObject
    {
		Assembly callingAssembly;
		private RadThemeManager ownerThemeManager;

        private static readonly Hashtable locationsAlreadyLoaded = new Hashtable();
        private static readonly object locationLoadedMarker = new object();
        
        public static RadProperty ThemeLocationProperty = RadProperty.Register("ThemeLocation", typeof(string),
            typeof(ThemeSource), new RadPropertyMetadata("", new PropertyChangedCallback(ThemeSource.OnThemeLocationChanged)));

        internal static void OnThemeLocationChanged(RadObject d, RadPropertyChangedEventArgs e)
        {
            ((ThemeSource)d).OnThemeLocationChanged(e);
        }

        protected virtual void OnThemeLocationChanged(RadPropertyChangedEventArgs e)
        {
            if (this.SettingsAreValid)
            {
                ReloadThemeFromStorage();
            }
        }

        internal Assembly GetInitialCallingAssembly()
        {
            return this.callingAssembly;
        }
        
		internal bool loadSucceeded = false;
		internal string loadError = string.Empty;
        
        /// <summary>
        /// Loads the theme from the file resource specified and registers it into ThemeResolutionService. Thais method is called 
        /// immediately when correct ThemeLocation and StorageType are specified.
        /// </summary>
        public virtual void ReloadThemeFromStorage()
        {
            //check for file load redundancy, only if it's not in Designmode
            bool checkFileLoadRedundancy = this.OwnerThemeManager == null || !this.OwnerThemeManager.IsDesignMode;

            if (callingAssembly == null)
            {
                callingAssembly = Assembly.GetCallingAssembly();
            }

            if (checkFileLoadRedundancy)
                if (locationsAlreadyLoaded[this.callingAssembly.FullName + this.ThemeLocation] == locationLoadedMarker)
                    return;

            XmlTheme theme = new XmlTheme();

            theme.LoadPartiallyFromStorage(this);            
            Theme.Deserialize(theme);

            if (checkFileLoadRedundancy && this.loadSucceeded)
                locationsAlreadyLoaded[this.callingAssembly.FullName + this.ThemeLocation] = locationLoadedMarker;			
        }

        /// <summary>
        /// Used internally by RadControls infrastructure to pre-load and obtain a reference to the corresponding XmlTheme object.
        /// </summary>
        /// <returns></returns>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public XmlTheme GetXmlThemeObject()
		{
            if (callingAssembly == null)
            {
                callingAssembly = Assembly.GetCallingAssembly();
            }
            XmlTheme theme = new XmlTheme();

            theme.LoadPartiallyFromStorage(this);
            Theme.Deserialize(theme);

            return theme;
		}

        /// <summary>
        /// Indicates whether the specified theme was loaded successfully.
        /// </summary>
        [Description("Indicates whether the specified theme was loaded successfully.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ThemeStorageValid
		{
			get 
			{
				return loadSucceeded;
			}
		}

        /// <summary>
        /// Gets value indicating the error message if Theme was not loaded successfully.
        /// </summary>
        [Description("Indicates the error message if theme was not loaded successfully.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ThemeLoadingError
        {
            get
            {
                return loadError;
            }
        }

		internal void SetCallingAssembly(Assembly asm)
		{
			callingAssembly = asm;
		}

        /// <summary>
        /// Gets or sets the full resource name if StorageType is Resource. Example: "MyNamespace.MyThemeFileName.xml". 
        /// If the StorageType specified is File, then the value of this property should represent the full or relative file path,
        /// accessible by the application. The "~" sign can be used to substitute the application executable path.
        /// Eg. "C:\MyApp\MyThemeFileName.xml" or "..\..\MyThemeFileName.xml" or "~\MyThemeFileName.xml"
        /// </summary>
        [Description("Name of the theme file or resource.")]
		[Editor(DesignerConsts.FileNameEditorString, typeof(UITypeEditor))]
        [DefaultValue("")]
        public string ThemeLocation
        {
            get { return (string)this.GetValue(ThemeLocationProperty); }
            set 
			{
				if (callingAssembly == null)
				{
					callingAssembly = Assembly.GetCallingAssembly();
				}

				this.SetValue(ThemeLocationProperty, value); 
			}
        }

        public static RadProperty StorageTypeProperty = RadProperty.Register("StorageType", typeof(ThemeStorageType),
            typeof(ThemeSource), new RadPropertyMetadata(ThemeStorageType.File, new PropertyChangedCallback(ThemeSource.OnStorageTypeChanged)));

        internal static void OnStorageTypeChanged(RadObject d, RadPropertyChangedEventArgs e)
        {
            ((ThemeSource)d).OnStorageTypeChanged(e);
        }

        protected virtual void OnStorageTypeChanged(RadPropertyChangedEventArgs e)
        {
            if (this.SettingsAreValid)
            {
                ReloadThemeFromStorage();
            }
        }

        /// <summary>
        /// Gets or sets File or Resource type of storage for the theme file
        /// </summary>
        [RadPropertyDefaultValue("StorageType", typeof(ThemeSource))]
        public ThemeStorageType StorageType
        {
            get { return (ThemeStorageType)this.GetValue(StorageTypeProperty); }
            set
			{
				if (callingAssembly == null)
				{
					callingAssembly = Assembly.GetCallingAssembly();
				}

				this.SetValue(StorageTypeProperty, value);
			}
        }

        /// <summary>
        /// Gets a value indicating whether property values are valid
        /// </summary>
        [Description("Indicates whether property values are valid")]
        public bool SettingsAreValid
        {
            get 
            {
                return !string.IsNullOrEmpty(this.ThemeLocation) && this.OwnerThemeManager != null;
            }
        }

        /// <summary>
        /// Gets or sets the owner theme manager component. Generally used by Form's designer.
        /// </summary>
        //[TypeConverter(typeof(ReferenceConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadThemeManager OwnerThemeManager
        {
            get { return ownerThemeManager; }
            set
            {
                ownerThemeManager = value;
                if (this.SettingsAreValid)
                {
                    ReloadThemeFromStorage();
                }
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public enum ThemeSourceChangeOperation
    {
        /// <summary>
        /// 
        /// </summary>
        Inserted = 0,
        /// <summary>
        /// 
        /// </summary>
        Removed,
        /// <summary>
        /// 
        /// </summary>
        Replaced,
        /// <summary>
        /// 
        /// </summary>
        Cleared
    }
    /// <summary>
    /// Represents
    /// </summary>
    /// <param name="changed">
    /// 
    /// </param>
    /// <param name="tartet">
    /// 
    /// </param>
    /// <param name="atIndex">
    /// 
    /// </param>
    /// <param name="operation">
    /// 
    ///</param>
    public delegate void ThemeSourcesChangedDelegate(ThemeSourceCollection changed, ThemeSource tartet, int atIndex, ThemeSourceChangeOperation operation);

    /// <summary>
    ///     <para>
    ///       A collection that stores <see cref='Telerik.WinControls.ThemeSource'/> objects.
    ///    </para>
    /// </summary>
    [Serializable()]
    public class ThemeSourceCollection : CollectionBase
    {
        private RadThemeManager ownerManager;
        public ThemeSourcesChangedDelegate ThemeSourcesChanged;

        /// <summary>
        ///     <para>
        ///       Initializes a new instance of the<see cref='Telerik.WinControls.ThemeSourceCollection'/>.
        ///    </para>
        /// </summary>
        /// <param name="ownerManager">
        ///     Owner component
        /// </param>
        public ThemeSourceCollection(RadThemeManager ownerManager)
        {
            this.ownerManager = ownerManager;
        }

        /// <summary>
        ///     <para>
        ///       Initializes a new instance of the <see cref='Telerik.WinControls.ThemeSourceCollection'/> based on another <see cref='Telerik.WinControls.ThemeSourceCollection'/>.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///       A <see cref='Telerik.WinControls.ThemeSourceCollection'/> from which the contents are copied
        /// </param>
        /// <param name="ownerManager">
        ///     Owner component
        /// </param>
        public ThemeSourceCollection(ThemeSourceCollection value, RadThemeManager ownerManager)
        {
            this.ownerManager = ownerManager;
            this.AddRange(value);
        }

        /// <summary>
        ///     <para>
        ///       Initializes a new instance of the <see cref='Telerik.WinControls.ThemeSourceCollection'/> containing any array of <see cref='Telerik.WinControls.ThemeSource'/> objects.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///       A array of <see cref='Telerik.WinControls.ThemeSource'/> objects with which to intialize the collection
        /// </param>
        /// <param name="ownerManager">
        ///     Owner component
        /// </param>
        public ThemeSourceCollection(ThemeSource[] value, RadThemeManager ownerManager)
        {
            this.ownerManager = ownerManager;
            this.AddRange(value);
        }

        /// <summary>
        /// <para>Represents the entry at the specified index of the <see cref='Telerik.WinControls.ThemeSource'/>.</para>
        /// </summary>
        /// <param name='index'><para>The zero-based index of the entry to locate in the collection.</para></param>
        /// <value>
        ///    <para> The entry at the specified index of the collection.</para>
        /// </value>
        /// <exception cref='System.ArgumentOutOfRangeException'><paramref name='index'/> is outside the valid range of indexes for the collection.</exception>
        public ThemeSource this[int index]
        {
            get
            {
                return ((ThemeSource)(List[index]));
            }
            set
            {
                List[index] = value;
            }
        }

        public RadThemeManager OwnerManager
        {
            get { return ownerManager; }
        }

        /// <summary>
        ///    <para>Adds a <see cref='Telerik.WinControls.ThemeSource'/> with the specified value to the 
        ///    <see cref='Telerik.WinControls.ThemeSourceCollection'/> .</para>
        /// </summary>
        /// <param name='value'>The <see cref='Telerik.WinControls.ThemeSource'/> to add.</param>
        /// <returns>
        ///    <para>The index at which the new element was inserted.</para>
        /// </returns>
        public int Add(ThemeSource value)
        {
            return List.Add(value);
        }

        /// <summary>
        /// <para>Copies the elements of an array to the end of the <see cref='Telerik.WinControls.ThemeSourceCollection'/>.</para>
        /// </summary>
        /// <param name='value'>
        ///    An array of type <see cref='Telerik.WinControls.ThemeSource'/> containing the objects to add to the collection.
        /// </param>
        /// <returns>
        ///   <para>None.</para>
        /// </returns>
        public void AddRange(ThemeSource[] value)
        {
            for (int i = 0; (i < value.Length); i = (i + 1))
            {
                this.Add(value[i]);
            }
        }

        /// <summary>
        ///     <para>
        ///       Adds the contents of another <see cref='Telerik.WinControls.ThemeSourceCollection'/> to the end of the collection.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///    A <see cref='Telerik.WinControls.ThemeSourceCollection'/> containing the objects to add to the collection.
        /// </param>
        /// <returns>
        ///   <para>None.</para>
        /// </returns>
        public void AddRange(ThemeSourceCollection value)
        {
            for (int i = 0; (i < value.Count); i = (i + 1))
            {
                this.Add(value[i]);
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether the 
        ///    <see cref='Telerik.WinControls.ThemeSourceCollection'/> contains the specified <see cref='Telerik.WinControls.ThemeSource'/>.</para>
        /// </summary>
        /// <param name='value'>The <see cref='Telerik.WinControls.ThemeSource'/> to locate.</param>
        /// <returns>
        /// <para><see langword='true'/> if the <see cref='Telerik.WinControls.ThemeSource'/> is contained in the collection; 
        ///   otherwise, <see langword='false'/>.</para>
        /// </returns>
        public bool Contains(ThemeSource value)
        {
            return List.Contains(value);
        }

        /// <summary>
        /// <para>Copies the <see cref='Telerik.WinControls.ThemeSourceCollection'/> values to a one-dimensional <see cref='System.Array'/> instance at the 
        ///    specified index.</para>
        /// </summary>
        /// <param name='array'><para>The one-dimensional <see cref='System.Array'/> that is the destination of the values copied from <see cref='Telerik.WinControls.ThemeSourceCollection'/> .</para></param>
        /// <param name='index'>The index in <paramref name='array'/> where copying begins.</param>
        /// <returns>
        ///   <para>None.</para>
        /// </returns>
        /// <exception cref='System.ArgumentException'><para><paramref name='array'/> is multidimensional.</para> <para>-or-</para> <para>The number of elements in the <see cref='Telerik.WinControls.ThemeSourceCollection'/> is greater than the available space between <paramref name='index'/> and the end of <paramref name='array'/>.</para></exception>
        /// <exception cref='System.ArgumentNullException'><paramref name='array'/> is <see langword='null'/>. </exception>
        /// <exception cref='System.ArgumentOutOfRangeException'><paramref name='index'/> is less than <paramref name='array'/>'s lowbound. </exception>
        public void CopyTo(ThemeSource[] array, int index)
        {
            List.CopyTo(array, index);
        }

        /// <summary>
        ///    <para>Returns the index of a <see cref='Telerik.WinControls.ThemeSource'/> in 
        ///       the <see cref='Telerik.WinControls.ThemeSourceCollection'/> .</para>
        /// </summary>
        /// <param name='value'>The <see cref='Telerik.WinControls.ThemeSource'/> to locate.</param>
        /// <returns>
        /// <para>The index of the <see cref='Telerik.WinControls.ThemeSource'/> of <paramref name='value'/> in the 
        /// <see cref='Telerik.WinControls.ThemeSourceCollection'/>, if found; otherwise, -1.</para>
        /// </returns>
        public int IndexOf(ThemeSource value)
        {
            return List.IndexOf(value);
        }

        /// <summary>
        /// <para>Inserts a <see cref='Telerik.WinControls.ThemeSource'/> into the <see cref='Telerik.WinControls.ThemeSourceCollection'/> at the specified index.</para>
        /// </summary>
        /// <param name='index'>The zero-based index where <paramref name='value'/> should be inserted.</param>
        /// <param name=' value'>The <see cref='Telerik.WinControls.ThemeSource'/> to insert.</param>
        /// <returns><para>None.</para></returns>
        public void Insert(int index, ThemeSource value)
        {
            List.Insert(index, value);
        }

        /// <summary>
        ///    <para>Returns an enumerator that can iterate through 
        ///       the <see cref='Telerik.WinControls.ThemeSourceCollection'/> .</para>
        /// </summary>
        /// <returns><para>None.</para></returns>
        public new ThemeSourceEnumerator GetEnumerator()
        {
            return new ThemeSourceEnumerator(this);
        }

        /// <summary>
        ///    <para> Removes a specific <see cref='Telerik.WinControls.ThemeSource'/> from the 
        ///    <see cref='Telerik.WinControls.ThemeSourceCollection'/> .</para>
        /// </summary>
        /// <param name='value'>The <see cref='Telerik.WinControls.ThemeSource'/> to remove from the <see cref='Telerik.WinControls.ThemeSourceCollection'/> .</param>
        /// <returns><para>None.</para></returns>
        /// <exception cref='System.ArgumentException'><paramref name='value'/> is not found in the Collection. </exception>
        public void Remove(ThemeSource value)
        {
            List.Remove(value);
        }

        protected override void OnInsertComplete(int index, object value)
        {
            ((ThemeSource)value).OwnerThemeManager = this.ownerManager;

            if (this.ThemeSourcesChanged != null)
            {
                this.ThemeSourcesChanged(this, (ThemeSource)value, index, ThemeSourceChangeOperation.Inserted);
            }
            base.OnInsertComplete(index, value);
        }

        protected override void OnSetComplete(int index, object oldValue, object newValue)
        {
            ((ThemeSource)newValue).OwnerThemeManager = this.ownerManager;
            if (this.ThemeSourcesChanged != null)
            {
                this.ThemeSourcesChanged(this, (ThemeSource)oldValue, index, ThemeSourceChangeOperation.Replaced);
            }
            base.OnSetComplete(index, oldValue, newValue);
        }

        protected override void OnRemoveComplete(int index, object value)
        {
            if (this.ThemeSourcesChanged != null)
            {
                this.ThemeSourcesChanged(this, (ThemeSource)value, index, ThemeSourceChangeOperation.Removed);
            }
            base.OnRemoveComplete(index, value);
        }

        protected override void OnClearComplete()
        {
            if (this.ThemeSourcesChanged != null)
            {
                this.ThemeSourcesChanged(this, null, -1, ThemeSourceChangeOperation.Cleared);
            }
            base.OnClearComplete();
        }

        public class ThemeSourceEnumerator : object, IEnumerator
        {

            private IEnumerator baseEnumerator;

            private IEnumerable temp;

            public ThemeSourceEnumerator(ThemeSourceCollection mappings)
            {
                this.temp = ((IEnumerable)(mappings));
                this.baseEnumerator = temp.GetEnumerator();
            }

            public ThemeSource Current
            {
                get
                {
                    return ((ThemeSource)(baseEnumerator.Current));
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return baseEnumerator.Current;
                }
            }

            public bool MoveNext()
            {
                return baseEnumerator.MoveNext();
            }

            bool IEnumerator.MoveNext()
            {
                return baseEnumerator.MoveNext();
            }

            public void Reset()
            {
                baseEnumerator.Reset();
            }

            void IEnumerator.Reset()
            {
                baseEnumerator.Reset();
            }
        }
    }

}
