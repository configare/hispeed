using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Diagnostics;
using Telerik.WinControls;
using System.ComponentModel;


namespace Telerik.WinControls.Styles
{
    /// <summary>
    /// Used by StyleManager to hold information about a Stylesheet object mapped to an Element in an ElementTree.
    /// </summary>
    public class StyleMap
    {
        private StyleBuilder styleBuilder;
        private StyleSheet styleSheet;
        private RadElement styleRootElement;
        private StylesheetTree stylesheetTree;
        private StyleManager ownerStyleManager;

		public StyleMap(StyleManager ownerStyleManager, StyleBuilder styleBuilder, RadElement styleRoot)
        {
            this.ownerStyleManager = ownerStyleManager;
            Debug.Assert(styleRoot != null, "The root element can not be null.");

            this.styleRootElement = styleRoot;
            this.styleBuilder = styleBuilder;
        }

        public StyleMap(StyleManager ownerStyleManager, StyleSheet styleSheet, RadElement styleRoot)
        {
            this.ownerStyleManager = ownerStyleManager;
            this.styleRootElement = styleRoot;
            this.SetStyleSheet(styleSheet);
        }

        public StyleManager OwnerStyleManager
        {
            get
            {
                return ownerStyleManager;
            }
        }

        public StyleSheet Style
        {
            get
            {
                return this.styleSheet;
            }
        }

        public StyleBuilder StyleBuilder
        {
            get
            {
                return this.styleBuilder;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadElement StyleRootElement
        {
            get
            {
                return this.styleRootElement;
            }
            internal set
            {
                this.styleRootElement = value;
            }
        }

        internal void SetStyleSheet(StyleSheet styleSheet)
        {
            if (this.styleSheet == styleSheet)
            {
                return;
            }

            if (this.styleSheet != null)
            {
                UnapplyStyle();
            }

            this.styleSheet = styleSheet;

            ApplyStyle();
        }

        public void UnapplyStyle()
        {
            if (this.stylesheetTree != null)
            {
                this.stylesheetTree.UnmapNodes();
            }

            this.styleRootElement.Style = null;
            this.stylesheetTree = null;
        }

        private void ApplyStyle()
        {
            if (this.styleSheet == null)
            {
                return;
            }

            this.ProcessStyle();            
        }

        internal void RemapElement(RadElement element)
        {
            if (element == null)
            {
                element = this.styleRootElement;
            }
            if (this.stylesheetTree != null)
            {
                this.ownerStyleManager.SuspendStyleMapCleanUp();
                this.ownerStyleManager.Owner.ElementTree.SuspendAnimations();

                this.stylesheetTree.DetachElement(element);
                this.stylesheetTree.AttachElement(element);

                this.ownerStyleManager.Owner.ElementTree.ResumeAnimations();
                this.ownerStyleManager.ResumeStyleMapCleanUp();
            }
        }

        internal void OnElementAdded(RadElement addedElement)
        {
            if (this.stylesheetTree != null)
            {
                this.stylesheetTree.AttachElement(addedElement);
            }
        }        

        internal void OnElementRemoved(RadElement removedElement)
        {
            if (this.stylesheetTree != null)
            {
                this.stylesheetTree.DetachElement(removedElement);
            }
        }

        /// <summary>
        /// Uses the already specified StyleBuilder to build the style of the styleRootElement
        /// Generally this call results in assignment of a StyleSheet to the styleRootElement
        /// </summary>
        public void BuildStyle()
        {
            Debug.Assert(this.styleBuilder != null, "Can not build style with a null style builder.");
            Debug.Assert(this.styleRootElement != null, "Can not build style with a null root element.");
            try
            {
                this.styleBuilder.BuildStyle(this.styleRootElement);
                this.styleRootElement.OnStyleBuilt();
            }
            catch (Exception ex)
            {
                string errMessge = "Error applying theme to an element of type " + this.styleRootElement.GetType().FullName;
                if (this.styleRootElement.ElementTree.Control != null)
                {
                    if (string.IsNullOrEmpty(this.styleRootElement.ElementTree.Control.Name))
                    {
                        errMessge += " that is part of control " + this.styleRootElement.ElementTree.Control.Name;
                        errMessge += " of type " + this.styleRootElement.ElementTree.Control.GetType().FullName;
                    }
                    else
                    {
                        errMessge += " that is part of control: " + this.styleRootElement.ElementTree.Control + "";
                    }
                }

                errMessge += ". Theme builder: " + this.styleBuilder.ToString();
                XmlStyleSheet xmlStyleSheet = this.styleBuilder.BuilderData as XmlStyleSheet;
                if (xmlStyleSheet != null)
                {
                    errMessge += ". Theme file location: " + xmlStyleSheet.ThemeLocation;
                }

                Debug.Fail(errMessge + ". Exception details:" + ex.ToString());
#if !DEBUG
                Trace.WriteLine(errMessge + ". Exception details:" + ex.ToString());
#endif
            }
        }

        public StylesheetTree StylesheetTree
        {
            get
            {
                return stylesheetTree;
            }
        }        

        private void ProcessStyle()
        {
            if (this.OwnerStyleManager != null && this.OwnerStyleManager.Owner != null)
            {
                XmlStyleRepository repository = ThemeResolutionService.GetThemeRepository(this.OwnerStyleManager.Owner.ThemeName);
                if (repository != null)
                {
                    this.styleSheet.ProcessGroupsInheritance(repository);
                }
            }

            this.stylesheetTree = new StylesheetTree(this);
            this.stylesheetTree.CreateNodesFromStyle(this.styleSheet);

            //Part of brute force algorithm test implemetation
            this.stylesheetTree.AttachElement(this.styleRootElement);
        }

        public void OnElementStyleSelectorKeyPropertyChanged(RadElement element, RadPropertyChangedEventArgs changeArgs)
        {
            if (this.stylesheetTree == null)
            {
                return;
            }

            this.stylesheetTree.OnElementSelectorKeyChanged(element, changeArgs);
        }
    }    
}
