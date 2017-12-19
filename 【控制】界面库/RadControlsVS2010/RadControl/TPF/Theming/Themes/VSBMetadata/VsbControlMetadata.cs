using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace Telerik.WinControls.Themes
{
    /// <summary>
    /// This class is associated with a control and provides information
    /// needed by the VSB to style this control. Since this class inherits from
    /// <see cref="VsbItemMetadata"/> it also contains information about the states this control
    /// can have and also about the elements which build the visual hierarchy. These elements
    /// are the ones that are actually styled in the VSB.
    /// </summary>
    public abstract class VsbControlMetadata : VsbItemMetadata
    {
        #region Fields
        private List<StyleBuilderRegistration> styleBuilderRegistrations;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates an instance of the <see cref="VsbControlMetadata"/> class
        /// based on a control name.
        /// </summary>
        /// <param name="name"></param>
        public VsbControlMetadata(string name)
        : base(name)
        {
            this.StyleRelatedElementType = typeof(RootRadElement);
        }

        #endregion

        #region Properties

        public IEnumerable<StyleBuilderRegistration> Registrations
        {
            get
            {
                foreach (StyleBuilderRegistration registration in this.StyleBuilderRegistrations)
                {
                    yield return registration;
                }
            }
        }

        protected List<StyleBuilderRegistration> StyleBuilderRegistrations
        {
            get
            {
                if (this.styleBuilderRegistrations == null)
                {
                    this.styleBuilderRegistrations = new List<StyleBuilderRegistration>();
                    this.CreateDefaultRegistrations();
                }

                return this.styleBuilderRegistrations;
            }
        }

        /// <summary>
        /// Gets the image shown for the corresponding control
        /// in the Tree View of the VSB.
        /// </summary>
        public abstract Image MetadataImage { get; }

        /// <summary>
        /// Gets an instance of the <see cref="Control"/> class
        /// which represents the preview control used
        /// in the Preview pane in the VSB.
        /// </summary>
        public abstract Control PreviewControl { get; }

        /// <summary>
        /// Gets a <see cref="IComponentTreeHandler"/> implementation
        /// that represents the preview control.
        /// </summary>
        public IComponentTreeHandler PreviewTreeHandler
        {
            get
            {
                return this.PreviewControl as IComponentTreeHandler;
            }
        }

        public override VsbMetadataType MetadataType
        {
            get
            {
                return VsbMetadataType.Control;
            }
        }

        public RadElement RootElement
        {
            get
            {
                IComponentTreeHandler treeHandler = this.PreviewControl as IComponentTreeHandler;
                return treeHandler == null ? null : treeHandler.RootElement;
            }
        }

        #endregion

        #region Methods

        public virtual string GetUniqueThemeNameIdentifier(StyleBuilderRegistration registration)
        {
            return registration.StylesheetRelations[0].ControlType;
        }

        /// <summary>
        /// Resets the values of all properties of the control's element tree that come from a style.
        /// </summary>
        public virtual void ResetPreviewControlStyleSettings()
        {
            IComponentTreeHandler previewControl = this.PreviewControl as IComponentTreeHandler;

            if (previewControl == null)
            {
                return;
            }

            previewControl.RootElement.ResetStyleSettings(true);
            previewControl.ElementTree.StyleManager.DetachStylesFromElementTree();
        }

        /// <summary>
        /// Allows metadata to provide specific element instances.
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public virtual RadElement GetElementInstance(VsbElementMetadata metadata)
        {
            return null;
        }

        /// <summary>
        /// Gets the Control instance displayed in the Preview and Design views of the Visual Style Builder.
        /// </summary>
        /// <returns></returns>
        public virtual Control GetDisplayControl()
        {
            return this.PreviewControl;
        }

        #region Child controls support

        /// <summary>
        /// This method finds and returns a reference to a control of the specified type
        /// which is child of the PreviewControl associated with this metadata.
        /// </summary>
        /// <param name="typeName">The type of control to look for.</param>
        /// <returns>A reference to a child control with the specified type if one found,
        /// otherwise null.</returns>
        public virtual Control GetTreeHandlerByTypeRecursively(string typeName)
        {
            Control previewControl = this.PreviewControl;
            IComponentTreeHandler treeHandler = this.PreviewTreeHandler;
            Debug.Assert(previewControl != null && treeHandler != null, "Preview Control should not be null.");

            if (treeHandler.ThemeClassName == typeName)
            {
                return previewControl;
            }

            Type controlType = RadTypeResolver.Instance.GetTypeByName(typeName, false);
            if (controlType != null && controlType.IsInstanceOfType(previewControl))
            {
                return previewControl;
            }

            foreach (Control child in ControlHelper.EnumChildControls(previewControl, true))
            {
                if (controlType != null && child.GetType() == controlType)
                {
                    return child;
                }

                IComponentTreeHandler childTreeHandler = child as IComponentTreeHandler;
                if (childTreeHandler != null && childTreeHandler.ThemeClassName == typeName)
                {
                    return child;
                }
            }

            Debug.Assert(false, "The current preview control does not have a descendant of the specified type.");
            return null;
        }

        #endregion

        public RadStylesheetRelation FindRelation(VsbItemMetadata metadata, BuilderRegistrationType registrationType)
        {
            StyleBuilderRegistration registration = this.GetStyleBuilderRegistration(metadata);
            if (registration == null)
            {
                return null;
            }

            foreach (RadStylesheetRelation relation in registration.StylesheetRelations)
            {
                if (relation.RegistrationType == registrationType &&
                    metadata.ElementThemeType.FullName == relation.ElementType)
                {
                    return relation;
                }
            }

            return null;
        }

        public void AddRelationForItem(VsbItemMetadata metadata, BuilderRegistrationType registrationType)
        {
            StyleBuilderRegistration registration = this.GetStyleBuilderRegistration(metadata);
            if (registration == null)
            {
                throw new InvalidOperationException("Could not add relation for Item " + metadata.ItemThemeRole + " because no style registration is available.");
            }

            string controlType = string.Empty;
            if (registrationType == BuilderRegistrationType.ElementTypeControlType ||
                registrationType == BuilderRegistrationType.ElementNameControlType)
            {
                controlType = this.PreviewTreeHandler.ThemeClassName;
            }

            registration.AddStylesheetRelation(registrationType, metadata.ElementThemeType.FullName, controlType, string.Empty, string.Empty);
        }

        public void RemoveRelationForItem(VsbItemMetadata metadata, BuilderRegistrationType registrationType)
        {
            StyleBuilderRegistration registration = this.GetStyleBuilderRegistration(metadata);
            if (registration == null)
            {
                throw new InvalidOperationException("Could not add relation for Item " + metadata.ItemThemeRole + " because no style registration is available.");
            }

            foreach (RadStylesheetRelation relation in registration.StylesheetRelations)
            {
                if (relation.RegistrationType == registrationType &&
                    metadata.ElementThemeType.FullName == relation.ElementType)
                {
                    registration.StylesheetRelations.Remove(relation);
                    break;
                }
            }
        }

        private bool CheckContainsRegistration(StyleBuilderRegistration registration)
        {
            foreach (StyleBuilderRegistration currentRegistration in this.StyleBuilderRegistrations)
            {
                foreach (RadStylesheetRelation relationToCheck in currentRegistration.StylesheetRelations)
                {
                    foreach (RadStylesheetRelation sourceRelation in registration.StylesheetRelations)
                    {
                        if (relationToCheck.Equals(sourceRelation))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Adds a new <see cref="StyleBuilderRegistration"/>to the collection of registrations
        /// of the control metadata.
        /// </summary>
        /// <param name="registration">The <see cref="StyleBuilderRegistration"/>to add.</param>
        public virtual void AddStyleBuilderRegistration(StyleBuilderRegistration registration)
        {
            if (!this.CheckContainsRegistration(registration))
            {
                this.StyleBuilderRegistrations.Add(registration);
            }
        }

        /// <summary>
        /// Removes the <see cref="StyleBuilderRegistration"/>from the collection of registrations
        /// of the control metadata.
        /// </summary>
        /// <param name="registration">The <see cref="StyleBuilderRegistration"/>to remove.</param>
        public virtual void RemoveStyleBuilderRegistration(StyleBuilderRegistration registration)
        {
            if (this.CheckContainsRegistration(registration))
            {
                this.StyleBuilderRegistrations.Remove(registration);
            }
        }

        /// <summary>
        /// This method creates and returns an instance of the StyleBuilderRegistration
        /// class. This method should be called before a control is edited in the Visual Style Builder
        /// and later accessed when styles for the control are modified. This can happen when a repository item
        /// is associated with element for particular state or when an element is edited directly from an
        /// editor.
        /// </summary>
        protected virtual void CreateDefaultRegistrations()
        {
            Debug.Assert(this.PreviewTreeHandler != null, "No preview control for metadata " + this.DisplayText);
            StyleBuilderRegistration defaultRegistration = new StyleBuilderRegistration(
                BuilderRegistrationType.ElementTypeControlType,
                typeof(RootRadElement).FullName,
                this.PreviewTreeHandler.ThemeClassName,
                string.Empty,
                string.Empty,
                new DefaultStyleBuilder());
            this.AddStyleBuilderRegistration(defaultRegistration);
        }

        /// <summary>
        /// Gets the default <see cref="StyleBuilderRegistration"/>for the control
        /// described by this metadata.
        /// </summary>
        /// <returns>An instance of the <see cref="StyleBuilderRegistration"/>class
        /// that represents the default registration.</returns>
        public virtual StyleBuilderRegistration GetStyleBuilderRegistration()
        {
            return this.GetStyleBuilderRegistration(this.PreviewTreeHandler.ThemeClassName);
        }

        /// <summary>
        /// Gets the <see cref="StyleBuilderRegistration"/>for the control
        /// of the specified type.
        /// </summary>
        /// <returns>An instance of the <see cref="StyleBuilderRegistration"/>class
        /// that represents the registration.</returns>
        public StyleBuilderRegistration GetStyleBuilderRegistration(string themeClassName)
        {
            foreach (StyleBuilderRegistration reg in this.StyleBuilderRegistrations)
            {
                foreach (RadStylesheetRelation relation in reg.StylesheetRelations)
                {
                    if (relation.ControlType.Equals(themeClassName))
                    {
                        return reg;
                    }
                }
            }

            return null;
        }

        public virtual Type GetStyleRelatedElementType(VsbElementMetadata metadata)
        {
            return metadata.StyleRelatedElementType;
        }

        public StyleBuilderRegistration GetStyleBuilderRegistration(VsbElementMetadata elementMetadata)
        {
            string controlType = elementMetadata.StyleRelatedTreeHandlerType;
            Type elementType = this.GetStyleRelatedElementType(elementMetadata);

            foreach (StyleBuilderRegistration reg in this.StyleBuilderRegistrations)
            {
                foreach (RadStylesheetRelation relation in reg.StylesheetRelations)
                {
                    if (elementType == null)
                    {
                        if (relation.ControlType.Equals(controlType))
                        {
                            return reg;
                        }
                    }
                    else if (elementType != null)
                    {
                        if (relation.ControlType.Equals(controlType) && relation.ElementType.Equals(elementType.FullName))
                        {
                            return reg;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Attempts to merge the specified style builder registration with our current registrations.
        /// </summary>
        /// <param name="mergeRegistration">The source registration which is to be merged with the default registration
        /// of the metadata.</param>
        /// <param name="themeName"></param>
        /// <returns>The default registration if successfully merged, otherwise null</returns>
        protected internal virtual StyleBuilderRegistration TryMergeRegistration(XmlStyleBuilderRegistration mergeRegistration, string themeName)
        {
            List<StyleBuilderRegistration> allRegistrations = this.StyleBuilderRegistrations;
            StyleSheet mergeStyle = null;

            foreach (StyleBuilderRegistration thisRegistration in allRegistrations)
            {
                bool match = false;

                foreach (RadStylesheetRelation mergeRelation in mergeRegistration.StylesheetRelations)
                {
                    foreach (RadStylesheetRelation thisRelation in thisRegistration.StylesheetRelations)
                    {
                        //our registration matches the provided one
                        if (!mergeRelation.Equals(thisRelation))
                        {
                            continue;
                        }

                        if (mergeStyle == null)
                        {
                            mergeStyle = (mergeRegistration.GetRegistration().Builder as DefaultStyleBuilder).Style;
                        }
                        //merge style and relations
                        (thisRegistration.Builder as DefaultStyleBuilder).Style = mergeStyle;
                        this.AddUniqueRelations(thisRegistration, mergeRegistration.StylesheetRelations, themeName);
                        this.MergeItemStates(mergeStyle);
                        match = true;
                        break;
                    }

                    //match is found for current registration, continue with top-level loop
                    if (match)
                    {
                        return thisRegistration;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the current <see cref="StyleBuilderRegistration"/> and tries
        /// to re-merge the states which have defined styles within their
        /// corresponding state managers.
        /// </summary>
        public void ReMergeItemStatesForCurrentRegistration()
        {
            StyleBuilderRegistration currentRegistration = this.GetStyleBuilderRegistration(this);
            this.MergeItemStates((currentRegistration.Builder as DefaultStyleBuilder).Style);
        }

        /// <summary>
        /// Merges the visual states that come from a loaded stylesheet with the default ones for
        /// each child item metadata in the current control metadata.
        /// </summary>
        /// <param name="mergeStyle">An instance of the <see cref="StyleSheet"/>
        /// class that represents the source stylesheet from which to take the
        /// additional visual states.</param>
        protected virtual void MergeItemStates(StyleSheet mergeStyle)
        {
            foreach (PropertySettingGroup group in mergeStyle.PropertySettingGroups)
            {
                foreach (IElementSelector selector in group.Selectors)
                {
                    if (selector is VisualStateSelector)
                    {
                        this.RegisterItemStateFromVisualStateSelector(selector as VisualStateSelector);
                    }
                    else if (selector is TypeSelector)
                    {
                        if (selector.ChildSelector is VisualStateSelector)
                        {
                            this.RegisterItemStateFromVisualStateSelector(selector.ChildSelector as VisualStateSelector);
                        }
                    }
                }
            }
        }

        private void RegisterItemStateFromVisualStateSelector(VisualStateSelector selector)
        {
            string[] splittedState = selector.VisualState.Split('.');
            Debug.Assert(splittedState != null && splittedState.Length > 0, "Invalid visual state name!");
            string themeRole = splittedState[0];

            string stateName = this.GetStateFromVisualStateSelector(selector.VisualState);

            //This should happen only for the default state.
            if (string.IsNullOrEmpty(stateName))
            {
                return;
            }

            try
            {
                this.TryAddItemMetadataState(this, themeRole, stateName);
            }
            catch (Exception)
            {

            }
        }

        private string GetStateFromVisualStateSelector(string visualStateSelector)
        {
            int indexOfFirstPoint = visualStateSelector.IndexOf('.');

            if (indexOfFirstPoint == -1)
            {
                return null;
            }

            string state = visualStateSelector.Substring(indexOfFirstPoint + 1);

            return state;
        }

        private void TryAddItemMetadataState(VsbItemMetadata itemMetadata, string targetItemThemeRole, string stateToMerge)
        {
            foreach (VsbItemMetadata currentItemMetadata in itemMetadata.ChildItemInfos)
            {
                if (currentItemMetadata.ItemThemeRole == targetItemThemeRole
                    && !currentItemMetadata.StateExists(stateToMerge)
                    && currentItemMetadata.StateManager.VerifyState(stateToMerge))
                {
                    currentItemMetadata.AddVisibleState(stateToMerge);
                }

                this.TryAddItemMetadataState(currentItemMetadata, targetItemThemeRole, stateToMerge);
            }
        }

        private void AddUniqueRelations(StyleBuilderRegistration registration, RadStyleSheetRelationList relations, string themeName)
        {
            bool added = false;
            foreach (RadStylesheetRelation relationToCheck in relations)
            {
                bool contains = false;

                foreach (RadStylesheetRelation currentRelation in registration.StylesheetRelations)
                {
                    if (currentRelation.Equals(relationToCheck))
                    {
                        contains = true;
                        break;
                    }
                }

                if (!contains)
                {
                    registration.StylesheetRelations.Add(relationToCheck);
                    added = true;
                }
            }

            if (added)
            {
                //update ThemeResolutionService registrations
                ThemeResolutionService.RegisterStyleBuilder(registration, themeName);
            }
        }

        #endregion

        #region Events

        protected internal virtual void OnMetadataDeselected()
        {
        }

        protected internal virtual void OnMetadataSelected()
        {
        }

        #endregion
    }
}
