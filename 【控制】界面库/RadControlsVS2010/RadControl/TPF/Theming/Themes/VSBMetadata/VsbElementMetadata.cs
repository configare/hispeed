using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;
using Telerik.WinControls;
using System.ComponentModel;

namespace Telerik.WinControls.Themes
{
    public class VsbElementMetadata
    {
        #region Fields

        private const char TreePathDelimiter = '.';

        private string displayText;
        private bool showInItemTree;
        private bool canHaveChildSelector;
        private Filter elementDefinedPropertiesFilter;
        private Filter stylablePropertiesFilter;
        private VsbItemMetadata parent;
        private string elementClass;
        private Type themeType;
        private ElementRepositoryOptions repositoryOptions = ElementRepositoryOptions.None;
        private string styleRelatedTreeHandlerType;
        private Type styleRelatedElementType;
        private Type parentType;
        private IElementSelector customSelector;
        private List<VsbMetadataAction> actions;

        #endregion

        #region Ctors

        public VsbElementMetadata(string displayText)
        {
            this.displayText = displayText;
            this.actions = new List<VsbMetadataAction>();
        }

        public VsbElementMetadata(RadElement element)
        {
            this.Initialize(element);
            this.repositoryOptions = ElementRepositoryOptions.GetDefaultRepositoryOpions(element);
            this.actions = new List<VsbMetadataAction>();
        }

		public VsbElementMetadata(RadElement element, ElementRepositoryOptions repositoryOptions) : this(element)
        {
            this.repositoryOptions = repositoryOptions;
            this.actions = new List<VsbMetadataAction>(); 
        }

        #endregion

        #region Properties


        /// <summary>
        /// Gets an instance of the <see cref="System.Collections.Generic.List&lt;T&gt;"/>class
        /// containing instances of the <see cref="VsbMetadataAction"/>
        /// class that can be executed over a metadata from the VSB UI.
        /// </summary>
        public List<VsbMetadataAction> Actions
        {
            get
            {
                return this.actions;
            }
        }

        /// <summary>
        /// Gets the type of the parent the element is hosted by.
        /// This helps an element to be uniquely identified on an element tree.
        /// </summary>
        public Type ParentType
        {
            get
            {
                return this.parentType;
            }
        }

        /// <summary>
        /// Gets or sets the type of the IComponentTreeHandler
        /// implementation that holds the element described in this metadata.
        /// </summary>
        public virtual string StyleRelatedTreeHandlerType
        {
            get
            {
                return this.styleRelatedTreeHandlerType;
            }
            set
            {
                this.styleRelatedTreeHandlerType = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the <see cref="RadElement"/>
        /// that is part of the <see cref="RadStylesheetRelation"/>
        /// which is responsible for defining the stylesheet
        /// which styles the element described in this metadata.
        /// </summary>
        public virtual Type StyleRelatedElementType
        {
            get
            {
                if (this.styleRelatedElementType == null)
                {
                    return this.ParentMetadata.StyleRelatedElementType;
                }

                return this.styleRelatedElementType;
            }
            set
            {
                this.styleRelatedElementType = value;
            }
        }

        /// <summary>
        /// Gets the control metadata where this instance resides.
        /// </summary>
        public VsbControlMetadata ControlMetadata
        {
            get
            {
                if (this is VsbControlMetadata)
                {
                    return this as VsbControlMetadata;
                }

                VsbItemMetadata parent = this.parent;
                while (parent != null)
                {
                    VsbControlMetadata result = parent as VsbControlMetadata;
                    if (result != null)
                    {
                        return result;
                    }

                    parent = parent.parent;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the filter, defined by the element itself, used to display all the stylable properties.
        /// </summary>
        public Filter ElementDefinedPropertiesFilter
        {
            get
            {
                return this.elementDefinedPropertiesFilter;
            }
        }

        /// <summary>
        /// Gets or sets the current stylable properties filter for the element.
        /// </summary>
        public Filter StylablePropertiesFilter
        {
            get
            {
                return this.stylablePropertiesFilter;
            }
            set
            {
                this.stylablePropertiesFilter = value;
            }
        }

        public string ElementClass
        {
            get
            {
                return this.elementClass;
            }
        }

        public Type ElementThemeType
        {
            get
            {
                return this.themeType;
            }
        }

        public string DisplayText
        {
            get
            {
                return this.displayText;
            }

            set
            {
                this.displayText = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public VsbItemMetadata ParentMetadata
        {
            get
            {
                return this.parent;
            }
            protected internal set
            {
                this.parent = value;
            }
        }

        public virtual VsbMetadataType MetadataType
        {
            get
            {
                return VsbMetadataType.Element;
            }
        }

        /// <summary>
        /// Gets or sets the selector, defined explicitly by the user.
        /// </summary>
        public IElementSelector UserSelector
        {
            get
            {
                return this.customSelector;
            }
            set
            {
                this.customSelector = value;
            }
        }

        public bool ShowInItemTree
        {
            get
            {
                return this.showInItemTree;
            }
            set
            {
                this.showInItemTree = value;
            }
        }

        /// <summary>
        /// Determines whether the metadata needs a child selector. This is false for RadItem instances.
        /// </summary>
        public bool CanHaveChildSelector
        {
            get
            {
                return this.canHaveChildSelector;
            }
            set
            {
                this.canHaveChildSelector = value;
            }
        }

#endregion

        #region Methods

        public ElementRepositoryOptions GetSupportedRepositoryOptions()
        {
            return this.repositoryOptions;
        }

        private void Initialize(RadElement element)
        {
            this.elementDefinedPropertiesFilter = element.GetStylablePropertiesFilter();
            this.stylablePropertiesFilter = this.elementDefinedPropertiesFilter;
            this.canHaveChildSelector = !(element is RadItem);
            this.elementClass = element.Class;
            this.themeType = element.GetThemeEffectiveType();
            this.displayText = this.EvaluateDisplayTextFor(element);
            this.styleRelatedTreeHandlerType = element.ElementTree.ComponentTreeHandler.ThemeClassName;
            //the parent type will help us to uniquely identify an element on the element tree
            this.parentType = element.Parent == null ? null : element.Parent.GetType();
        }

        protected virtual string EvaluateDisplayTextFor(RadElement element)
        {
            if (string.IsNullOrEmpty(element.Class))
            {
                return element.GetThemeEffectiveType().Name;
            }
            else
            {
                return element.Class + " (" + element.GetThemeEffectiveType().Name + ")";
            }
        }

        #endregion
    }
}
