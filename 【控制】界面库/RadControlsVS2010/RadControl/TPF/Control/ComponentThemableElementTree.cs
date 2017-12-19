using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Drawing;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Themes.Design;
using System.Collections;
using System.Reflection;
using Telerik.WinControls.Keyboard;
using Telerik.WinControls.Assistance;
using Telerik.WinControls.Design;
using System.Drawing.Design;
using Telerik.WinControls.Layouts;
using System.Globalization;
using System.Runtime.InteropServices;
using Telerik.WinControls.Styles;
using System.Diagnostics;


namespace Telerik.WinControls
{
    delegate void ElementRadPropertyChangedDelegate(RadElement sender, RadPropertyChangedEventArgs e);

    public interface IThemeChangeListener
    {
        void OnThemeChanged(ThemeChangedEventArgs e);
        string ControlThemeClassName
        {
            get;
        }
    }

    interface IPropertyChangeListener
    {
        void OnRadPropertyChanged(RadElement element, RadPropertyChangedEventArgs changeArgs);
    }

    public interface IElementTreeChangeListener
    {
        void OnElementAdded(RadElement addedElement);
        void OnElementRemoved(RadElement formerParent, RadElement removedElement);
        void OnElementDisposed(RadElement formerParent, RadElement disposed);
    }

    public class ComponentThemableElementTree : ComponentLayoutElementTree, IDisposable, IThemeChangeListener
    {
        //Fields
        private bool enableTheming = true;
        private string themeName = "";
        private string themeClassName = null;
        private bool disposing;
        private bool isDisposed;
        private Dictionary<RadProperty.FromNameKey, List<IPropertyChangeListener>> elementPropertyChangeListeners = new Dictionary<RadProperty.FromNameKey, List<IPropertyChangeListener>>();
        private List<IElementTreeChangeListener> treeChangeListeners = new List<IElementTreeChangeListener>(1);

        private int animationSuspendCounter = 0;
        private StyleManager styleManager;

        RadEventHandlerList<ElementPropertyKey, ElementRadPropertyChangedDelegate> elementPropertyChangeEventList = new RadEventHandlerList<ElementPropertyKey, ElementRadPropertyChangedDelegate>();
    
        #region Constructors

        public ComponentThemableElementTree(IComponentTreeHandler owner)
            : base(owner)
        {
            ThemeResolutionService.SubscribeForThemeChanged(this);
        }

		public bool EnableTheming
		{
			get
			{
				return this.enableTheming;
			}
			set
			{
                if (this.enableTheming != value)
                {
                    this.enableTheming = value;

                    if (!this.enableTheming)
                    {
                        this.StyleManager.DetachStylesFromElementTree();
                        this.StyleManager.SuspendStyling();
                    }
                    else
                    {
                        this.StyleManager.ResumeStyling();
                        if (!this.ComponentTreeHandler.Initializing)
                        {
                            this.StyleManager.AttachStylesToElementTree();
                        }
                    }
                }
			}
		}

        #endregion

        public bool Disposing
        {
            get
            {
                return this.disposing;
            }
        }

        internal protected virtual void Dispose(bool disposing)
        {
            //we are already disposed of
            if (this.isDisposed)
            {
                return;
            }

            this.disposing = true;

            if (disposing)
            {
                elementPropertyChangeEventList.Dispose();
                this.RootElement.Dispose();
            }

            ThemeResolutionService.UnsubscribeFromThemeChanged(this);

            this.disposing = false;
            this.isDisposed = true;
        }

        protected internal override void CreateChildItems(RadElement parent)
        {
            if (this.isDisposed || this.disposing)
            {
                throw new ObjectDisposedException("Element tree already disposed.");
            }

            if (!this.UseNewLayoutSystem)
            {
                this.SuspendRadLayout();
            }

            base.CreateChildItems(parent);

            this.creatingChildItems = false;
            this.SetChildItemsCreated(true);

            if (!this.UseNewLayoutSystem)
            {
                this.ResumeRadLayout(true);
            }
        }

        protected override void InitializeRootElement(RootRadElement rootElement)
        {
        }

        #region Theme support
        /// <summary>
        ///     Gets or sets control's preffered theme name. Themes are stored and retrieved using
        ///     APIs of <see cref="Telerik.WinControls.ThemeResolutionService"/>.
        /// </summary>
        /// <remarks>
        /// If <strong>ThemeResolutionService.ApplicatonThemeName</strong> refers to a
        /// non-empty string, the theme of a RadControl can differ from the one set using
        /// <strong>RadControls.ThemeName</strong> property. If the themes differ, the
        /// <strong>RadControls.ThemeName</strong> property will be overridden by
        /// <strong>ThemeResolutionService.ApplicatonThemeName</strong>. If no theme is registered
        /// with a name as <strong>ThemeResolutionService.ApplicatonThemeName</strong>, then
        /// control will revert to the theme specified by its <strong>ThemeName</strong> property.
        /// If <strong>ThemeName</strong> is assigned to a non-existing theme name, the control may
        /// have no visual properties assigned, which will cause it look and behave in unexpected
        /// manner. If <strong>ThemeName</strong> equals empty string, control's theme is set to a
        /// theme that is registered within <strong>ThemeResolutionService</strong> with the name
        /// "ControlDefault".
        /// </remarks>
        [Browsable(true), Category(RadDesignCategory.StyleSheetCategory)]
        [Description("Gets or sets theme name.")]
        [DefaultValue((string)"")]
        [Editor(DesignerConsts.ThemeNameEditorString, typeof(UITypeEditor))]
        public string ThemeName
        {
            get
            {
                return themeName;
            }

            set
            {
                if (value == themeName)
                {
                    return;
                }

                string oldThemeName = this.themeName;
                string newThemeName = value ?? "";

                themeName = newThemeName;                

                //Code moved to ApplyThemeToElementTree
                ////when theme changes we should always call both methods
                //this.RootElement.ControlThemeChanged();
                //this.ControlThemeChanged();
                this.ApplyThemeToElementTree(true);

                ThemeNameChangedEventArgs arg = new ThemeNameChangedEventArgs(oldThemeName, newThemeName);
                this.OnThemeNameChanged(arg);
            }
        }

        protected internal virtual void OnThemeNameChanged(ThemeNameChangedEventArgs e)
        {
            if (this.ComponentTreeHandler.Initializing)
            {
                return;
            }

            this.ComponentTreeHandler.CallOnThemeNameChanged(e);
        }

        /// <summary>
        /// Gets or sets the class name string that ThemeResolutionService will use to find the themes registered for the control.
        /// </summary>
        /// <remarks>
        /// By default the return value is RadControl's type FullName; Some controls like drop down menu has different ThemeClassName
        /// depending on the runtime usaage of the control.
        /// </remarks>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category(RadDesignCategory.StyleSheetCategory)]
        public virtual string ThemeClassName
        {
            get
            {
                if (themeClassName != null)
                {
                    return themeClassName;
                }
                return this.ComponentTreeHandler.GetType().FullName;
            }
            set
            {
                this.themeClassName = value;
            }
        }

        /// <summary>
        /// Used internally to support RadControl infrastructure. This method is not intended for use directly from your code.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ForceReapplyTheme()
        {
            CallControlThemeChanged();
        }

        /// <summary>
        /// Used internally to support RadControl infrastructure. This method is not intended for use directly from your code.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void EnsureThemeAppliedInitially(bool checkInitializing)
        {
            if (!this.RootElement.IsThemeApplied)
            {
                this.ApplyThemeToElementTree(checkInitializing);
                this.OnThemeNameChanged(new ThemeNameChangedEventArgs(this.themeName, this.themeName));
            }
        }

        /// <summary>
        /// Used internally to support RadControl infrastructure. This method is not intended for use directly from your code.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ApplyThemeToElementTree(bool checkInitializing)
        {
            if (this.ComponentTreeHandler == null)
            {
                return;
            }

            //invalidate "theme applied" status
            this.RootElement.SetThemeApplied(false);

            //if control is initializing (either not loaded yet or within a Begin/End Init block) do nothing
            if (checkInitializing)
            {
                if (this.ComponentTreeHandler.Initializing)
                {
                    return;
                }
            }

            CallControlThemeChanged();
        }
      
        private void CallControlThemeChanged()        
        {
            Debug.Assert(this.Control != null, "No Control instance???");
            if (this.Control.InvokeRequired)
            {
                this.Control.BeginInvoke(new MethodInvoker(this.RootElement.ControlThemeChanged));
                this.Control.BeginInvoke(new MethodInvoker(this.ComponentTreeHandler.ControlThemeChangedCallback));
            }
            else
            {
                this.RootElement.ControlThemeChanged();
                this.ComponentTreeHandler.ControlThemeChangedCallback();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the ThemeClassName property was set to value different from null (Nothing in VB.NET).
        /// </summary>
        public bool IsThemeClassNameSet
        {
            get { return this.themeClassName != null; }
        }

        internal bool CallControlDefinesThemeForElement(RadElement element)
        {
            return this.ControlDefinesThemeForElement(element);
        }

        /// <summary>
        /// Gets a value indicating if control themes by default define PropertySettings for the specified element. 
        /// If true is returned the ThemeResolutionService would not not set any theme to the element to avoid duplicatingthe style
        /// settings of the element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool ControlDefinesThemeForElement(RadElement element)
        {
            return false;
        }
        #endregion

        void IDisposable.Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region IThemeChangeListener

        void IThemeChangeListener.OnThemeChanged(ThemeChangedEventArgs e)
        {
            //Note the event may be fired from another thread
            if (e.ThemeName == this.ThemeName ||
                (e.ThemeName == ThemeResolutionService.ControlDefaultThemeName && this.ThemeName == string.Empty) ||
                e.ThemeName == ThemeResolutionService.ApplicationThemeName)
            {
                this.ApplyThemeToElementTree(true);
            }
        }
        string IThemeChangeListener.ControlThemeClassName
        {
            get
            {
                if (this.ComponentTreeHandler != null)
                {
                    return this.ComponentTreeHandler.ThemeClassName;
                }

                return null;
            }
        }

        #endregion

        #region AnimationsEnabled

        /// <summary>
        /// Suspends the animated property changes for the control. When animation are suspended property changes still occur but without aniumations.
        /// </summary>
        public void SuspendAnimations()
        {
            this.animationSuspendCounter++;
        }

        /// <summary>
        /// Resumes the animated property changes for the conrol. For more info see <see cref="SuspendAnimations"/>
        /// </summary>
        public void ResumeAnimations()
        {
            this.animationSuspendCounter--;
        }

        /// <summary>
        /// Gets value indicating whether the animated property changes are suspended for the control. Also see <see cref="SuspendAnimations"/>.
        /// </summary>
        public bool AnimationsEnabled
        {
            get
            {
                return this.animationSuspendCounter == 0;
            }
        }

        #endregion

        #region PropertyChangeListeners support

        /// <summary>
        /// Subscribe for property-change event for given RadProperty of a specific RadElement
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="radElement"></param>
        /// <param name="radProperty"></param>
        internal void SubscribeForElementPropertyChange(ElementRadPropertyChangedDelegate handler, RadElement radElement, RadProperty radProperty)
        {
            ElementPropertyKey key = new ElementPropertyKey(radElement, radProperty);
            this.elementPropertyChangeEventList.AddHandler(key, handler);
        }

        internal void UnsubscribeFromElementPropertyChange(ElementRadPropertyChangedDelegate handler, RadElement radElement, RadProperty radProperty)
        {
            ElementPropertyKey key = new ElementPropertyKey(radElement, radProperty);
            this.elementPropertyChangeEventList.RemoveHandler(key, handler);
        }

        /// <summary>
        /// Subscribe for property-change event for given RadProperty for all elements in the tree.
        /// </summary>
        /// <param name="listener"></param>
        /// <param name="radProperty"></param>
        internal void AddElementPropertyChangeListener(IPropertyChangeListener listener, RadProperty radProperty)
        {
            List<IPropertyChangeListener> changeListeners = GetPropertyChangeListeners(radProperty);

            if (changeListeners == null)
            {
                changeListeners = new List<IPropertyChangeListener>(1);
                elementPropertyChangeListeners[radProperty.PropertyKey] = changeListeners;
            }

            if (!changeListeners.Contains(listener))
            {
                changeListeners.Add(listener);
            }
        }

        internal void RemoveElementPropertyChangeListener(IPropertyChangeListener listener, RadProperty radProperty)
        {
            List<IPropertyChangeListener> changeListeners = GetPropertyChangeListeners(radProperty);

            if (changeListeners == null)
            {
                return;
            }

            changeListeners.Remove(listener);
        }

        private List<IPropertyChangeListener> GetPropertyChangeListeners(RadProperty radProperty)
        {
            List<IPropertyChangeListener> changeListeners;
            elementPropertyChangeListeners.TryGetValue(radProperty.PropertyKey, out changeListeners);
            return changeListeners;
        }

        internal void NotifyElementPropertyChanged(RadElement element, RadPropertyChangedEventArgs args)
        {
            ElementPropertyKey key = new ElementPropertyKey(element, args.Property);

            ElementRadPropertyChangedDelegate handler = this.elementPropertyChangeEventList[key];
            if (handler != null)
            {
                handler(element, args);
            }

            List<IPropertyChangeListener> changeListeners = GetPropertyChangeListeners(args.Property);
            if (changeListeners == null)            
            {
                return;
            }

            for (int i = 0; i < changeListeners.Count; i++)
            {
                changeListeners[i].OnRadPropertyChanged(element, args);
            }
        }

        #endregion

        #region ElementTreeChangeListeners

        internal void AddElementTreeChangeListener(IElementTreeChangeListener listener)
        {
            treeChangeListeners.Add(listener);
        }

        internal void RemoveElementTreeChangeListener(IElementTreeChangeListener listener)
        {
            this.treeChangeListeners.Remove(listener);
        }

        internal void NotifyElementAdded(RadElement addedElement)
        {
            for (int i = 0; i < this.treeChangeListeners.Count; i++)
            {
                this.treeChangeListeners[i].OnElementAdded(addedElement);
            }
        }

        internal void NotifyElementDisposed(RadElement formerParent, RadElement disposedElement)
        {
            for (int i = 0; i < this.treeChangeListeners.Count; i++)
            {
                this.treeChangeListeners[i].OnElementDisposed(formerParent, disposedElement);
            }

            this.DetachElement(formerParent, disposedElement);
        }

        internal void NotifyElementRemoved(RadElement formerParent, RadElement removedElement)
        {
            for (int i = 0; i < this.treeChangeListeners.Count; i++)
            {
                this.treeChangeListeners[i].OnElementRemoved(formerParent, removedElement);
            }

            this.DetachElement(formerParent, removedElement);
        }

        private void DetachElement(RadElement formerParent, RadElement removedElement)
        {
            //TODO: Examine carefully. May cause some memory leaks.
            if (removedElement.IsThemeRefreshSuspended && !removedElement.IsDisposing)
            {
                return;
            }

            //remove all Delegates subscribed for property changes of this element.
            bool found;
            do
            {
                found = false;
                foreach (KeyValuePair<ElementPropertyKey, Delegate> item in this.elementPropertyChangeEventList.IemsEnumerable)
                {
                    if (item.Key.Element == removedElement)
                    {
                        this.elementPropertyChangeEventList.RemoveAllEventHandlersByKey(item.Key);
                        found = true;

                        //Debug.Fail("Event handlers not removed after Element is removed from the tree: " + removedElement);
                        break;
                    }
                }
            }
            while (found);
        }

        #endregion

        #region StyleManager

        public StyleManager StyleManager
        {
            get
            {
                if (this.styleManager == null)
                {
                    this.styleManager = new StyleManager(this.ComponentTreeHandler);
                }
                return this.styleManager;
            }
        }

        #endregion

        #region ElementPropertyKey class declaration
        internal class ElementPropertyKey
        {
            // Fields
            private int hashCode;
            private RadElement element;
            private RadProperty property;

			public RadProperty Property
			{
				get
				{
					return property;
				}
			}

            public RadElement Element
            {
                get
                {
                    return element;
                }
            }

            // Methods
            public ElementPropertyKey(RadElement element, RadProperty property)
            {
                this.element = element;
                this.property = property;
                this.hashCode = this.element.GetHashCode() ^ this.property.PropertyKey.GetHashCode();
            }

            public static bool operator == (ElementPropertyKey key1, ElementPropertyKey key2)
            {
                return key1.Equals(key2);
            }

            public static bool operator !=(ElementPropertyKey key1, ElementPropertyKey key2)
            {
                return !key1.Equals(key2);
            }

            public override bool Equals(object o)
            {
                if ((o != null) && (o is ElementPropertyKey))
                {
                    return this.Equals((ElementPropertyKey)o);
                }

                return false;
            }

            public bool Equals(ElementPropertyKey key)
            {
                if (this.element == (key.element))
                {
                    return (this.property == key.property);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return this.hashCode;
            }

            public override string ToString()
            {
                return this.element.Class + "/" + element.GetThemeEffectiveType() + "/" + "." + this.property.Name;
            }
        }

        /// <devdoc>
        ///    <para>Provides a simple list of delegates. This class cannot be inherited.</para>
        /// </devdoc> 
        public sealed class RadEventHandlerList<KeyType, DelegateType> : IDisposable where DelegateType: class
        {
            Dictionary<KeyType, Delegate> items = new Dictionary<KeyType, Delegate>();
            Component parent;

            /// <devdoc>
            ///    Creates a new event handler list.
            /// </devdoc> 
            public RadEventHandlerList()
            {
            }

            /// <devdoc>
            ///     Creates a new event handler list.  The parent component is used to check the component's
            ///     CanRaiseEvents property.
            /// </devdoc> 
            internal RadEventHandlerList(Component parent)
            {
                this.parent = parent;
            }

            /// <devdoc>
            ///    <para>Gets or sets the delegate for the specified key.</para>
            /// </devdoc> 
            public DelegateType this[KeyType key]
            {
                get
                {
                    Delegate result;
                    this.items.TryGetValue(key, out result);

                    return result as DelegateType;
                }
                set
                {
                    this.items[key] = value as Delegate;
                }
            }

            /// <devdoc> 
            ///    <para>[To be supplied.]</para>
            /// </devdoc> 
            public void AddHandler(KeyType key, DelegateType value)
            {
                Delegate target;
                if (this.items.TryGetValue(key, out target))
                {
                    this.items[key] = Delegate.Combine(target, value as Delegate);
                }

                this.items[key] = value as Delegate;
            }

            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public void Dispose()
            {
                this.items.Clear();
                this.items = null;
            }

            /// <summary>
            /// Removes all delages registeres with the speified key from the event hadler list
            /// </summary>
            /// <param name="key"></param>
			public void RemoveAllEventHandlersByKey(KeyType key)
			{
                this.items.Remove(key);
			}            

            /// <devdoc>
            ///    <para>[To be supplied.]</para>
            /// </devdoc>
            public void RemoveHandler(KeyType key, DelegateType value)
            {
                Delegate target;
                if (this.items.TryGetValue(key, out target))
                {
                    Delegate newValue = Delegate.Remove(target, value as Delegate);
                    if (newValue == null)
                    {
                        this.items.Remove(key);
                    }
                    else
                    {
                        this.items[key] = newValue;
                    }
                }
                // else... no error for removal of non-existant delegate
                //
            }

            internal IEnumerable<KeyValuePair<KeyType, Delegate>> IemsEnumerable
            {
                get
                {
                    return this.items;
                }
            }
        }

        #endregion
    }    
}
