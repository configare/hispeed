using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Paint;
using Telerik.WinControls.Primitives;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using Telerik.WinControls.Design;
using Telerik.WinControls;
using Telerik.WinControls.Collections;
using Telerik.WinControls.Commands;
using Telerik.WinControls.Themes;
using System.Collections.Specialized;

namespace Telerik.WinControls
{
    /// <summary>
    /// RadElement class represents the smallest unit in a RadControl that can be painted or that has a layout slot in a RadControl.
    /// Generally each RadCotnrol is composed of a tree of RadElements. The tree has as a root the <see cref="RadControl.RootElement"/> and
    /// children <see cref="Children"/> property.
    /// </summary>
    /// <remarks>
    ///     Elements nesting also represents the visual nesting. Elements are painted starting
    ///     from the root to the leaves of the tree. the leaves are most often primitive
    ///     elements like, text, fills, borders and so on. Elements that are descendants of 
    ///     LayoutPanel are responsible for arranging their children in the available space
    ///     and/or for notifying the parent that the layout space is not enough to expand.
    ///     Layout behavior of each element can be adjusted using the properties:
    ///     <see cref="AutoSize"/>, <see cref="Size"/>,
    ///     <see cref="AutoSizeMode"/> (old layouts), and <see cref="StretchHorizontally"/> and <see cref="StretchVertically"/> for
    ///		the new layouts. 
    ///		RadElement is the base class of all elements that need to take advantage of TPF features, like 
    ///		property inheritance, layouts, styling
    ///     with the Visual Style Builder application. Each property change of a RadElement or
    ///     of its inheritance parent would result in calling the method OnPropertyChange,
    ///     which can be overridden in order to customize the response to changes of any
    ///     RadPoperty.
    /// </remarks>
    public class RadElement : RadObject, IRadLayoutElement, ISupportSystemSkin
    {
        #region Constructor/Initializers

        static RadElement()
        {
            MouseMoveEventKey = new object();
            MouseDownEventKey = new object();
            MouseUpEventKey = new object();
            MouseEnterEventKey = new object();
            MouseLeaveEventKey = new object();
            EnabledChangedEventKey = new object();
            MouseHoverEventKey = new object();
            //Removed in favor of RadItem-MouseWheel
            //MouseWheelEventKey = new object();
            ChildrenChangedKey = new object();

            SetPropertyValueCommand = new SetPropertyValueCommand();
            SetPropertyValueCommand.Name = "SetPropertyValueCommand";
            SetPropertyValueCommand.Text = "This command sets a provided value to a property of an object. No type verification is done.";
            SetPropertyValueCommand.OwnerType = typeof(RadElement);

            GetPropertyValueCommand = new GetPropertyValueCommand();
            GetPropertyValueCommand.Name = "GetPropertyValueCommand";
            GetPropertyValueCommand.Text = "This command gets the value of a property. No type verification is done.";
            GetPropertyValueCommand.OwnerType = typeof(RadElement);
        }

        public RadElement()
        {
            this.state = ElementState.Constructing;
            this.InitializeFields();
            this.Construct();
        }

        private void Construct()
        {
            this.SuspendThemeRefresh();
            //layout will be suspended until loaded
            this.SuspendLayout(false);
            this.CallCreateChildElements();
            this.ResumeThemeRefresh();
            this.state = ElementState.Constructed;
        }

        /// <summary>
        /// Creates the child elements and sets their locally applied values as Default
        /// </summary>
        protected virtual void CallCreateChildElements()
        {
            this.CreateChildElements();
            this.SetChildrenLocalValuesAsDefault(true);
        }

        /// <summary>
        /// Temporary suspends UpdateReferences method.
        /// Useful when modifying the element tree without changing the actual element's references.
        /// </summary>
        public void SuspendReferenceUpdate()
        {
            this.suspendReferenceUpdate++;
        }

        /// <summary>
        /// Resumes previously suspended UpdateReference method.
        /// </summary>
        public void ResumeReferenceUpdate()
        {
            if (this.suspendReferenceUpdate > 0)
            {
                this.suspendReferenceUpdate--;
            }
        }

        /// <summary>
        /// Initializes member fields to their default values.
        /// This method is called prior the CreateChildItems one and allows for initialization of members on which child elements depend.
        /// </summary>
        protected virtual void InitializeFields()
        {
            this.zOrderedChildren = new RadElementZOrderCollection(this);
            this.children = new RadElementCollection(this);
            this.useSystemSkin = UseSystemSkinMode.Inherit;
            this.paintSystemSkin = null;

            RadBitVector64 bitState = this.BitState;
            bitState[InvalidateMeasureOnRemoveStateKey] = true;
            bitState[ProtectFocusedPropertyStateKey] = true;
            bitState[NeverArrangedStateKey] = true;
            bitState[SerializeChildrenStateKey] = true;
            bitState[SerializeElementStateKey] = true;
            bitState[ShouldPaintChildrenStateKey] = true;
            bitState[IsLayoutInvalidatedStateKey] = true;
        }

        /// <summary>
        /// Called by the element when constructed. Allows inheritors to build the element tree.
        /// </summary>
        protected virtual void CreateChildElements()
        {
        }

        /// <summary>
        /// A callback used by the owning RadControl to notify the element for a first-time screen visualization.
        /// </summary>
        /// <param name="recursive">True to notify entire subtree for the load process, false otherwise.</param>
        protected internal void OnLoad(bool recursive)
        {
            Debug.Assert(this.layoutManager != null, "Must have layout manager at this point.");
            this.state = ElementState.Loading;

            //copy local layout events to the layout manager
            this.layoutManager.LayoutEvents.AddRange(this.layoutEvents);
            this.layoutEvents.Clear();
            this.ResetLayout();

            //allow inheritors to provide custom load logic
            this.LoadCore();

            if (recursive)
            {
                //delegate the event to all children
                foreach (RadElement child in this.children)
                {
                    child.OnLoad(recursive);
                }
            }

            this.state = ElementState.Loaded;
            //notify for successfully completed load process
            this.OnLoaded();
        }

        /// <summary>
        /// Allows inheritors to porvide custom load logic.
        /// </summary>
        protected virtual void LoadCore()
        {
        }

        /// <summary>
        /// Called when the element has been successfully loaded. That includes loading of all its children as well.
        /// </summary>
        protected virtual void OnLoaded()
        {
        }

        /// <summary>
        /// Unloads the element if it was previously loaded on an element tree.
        /// </summary>
        /// <param name="oldTree">Reference to the element tree from which we are in a process of unload.</param>
        /// <param name="recursive"></param>
        protected internal void OnUnload(ComponentThemableElementTree oldTree, bool recursive)
        {
            Debug.Assert(oldTree != null, "Invalid Unload event");

            this.UnloadCore(oldTree);
            if (recursive)
            {
                foreach (RadElement child in this.children)
                {
                    child.OnUnload(oldTree, recursive);
                }
            }

            this.state = ElementState.Unloaded;
            this.OnUnloaded(oldTree);
        }

        /// <summary>
        /// Executes the core unload logic. Allows inheritors to perform additional action while the element is unloading itself.
        /// </summary>
        /// <param name="oldTree">Reference to the element tree from which we are in a process of unload.</param>
        protected virtual void UnloadCore(ComponentThemableElementTree oldTree)
        {
            this.DetachFromElementTree(oldTree);
        }

        /// <summary>
        /// Notifies that the element has been successfully unloaded from an element tree.
        /// Allows inheritors to provide custom logic at this stage.
        /// </summary>
        /// <param name="oldTree">Reference to the element tree from which the element has been unloaded.</param>
        protected virtual void OnUnloaded(ComponentThemableElementTree oldTree)
        {
        }

        /// <summary>
        /// The element gets notified for a change in its current ElementTree member.
        /// </summary>
        /// <param name="previousTree"></param>
        protected virtual void OnElementTreeChanged(ComponentThemableElementTree previousTree)
        {
        }

        /// <summary>
        /// A callback used by the owning RadControl to notify the element for the beginning of a disposing process.
        /// </summary>
        protected internal virtual void OnBeginDispose()
        {
            this.state = ElementState.Disposing;
            this.suspendThemeRefresh++;
            this.layoutSuspendCount++;
            this.suspendReferenceUpdate++;
        }

        /// <summary>
        /// Gets the current state of the element.
        /// </summary>
        [Browsable(false)]
        public ElementState ElementState
        {
            get
            {
                return this.state;
            }
        }

        /// <summary>
        /// Applies the specified <see cref="RadElement">RadElement</see> instance as parent of the current instance.
        /// </summary>
        /// <param name="parent"></param>
        protected void SetParent(RadElement parent)
        {
            if (this.state == ElementState.Disposed)
            {
                throw new InvalidOperationException("Setting parent to an already disposed element");
            }
            //the visual scene is disposing, do nothing
            if (this.state == ElementState.Disposing)
            {
                this.parent = null;
                return;
            }
            //same parent, do nothing
            if (this.parent == parent)
            {
                return;
            }

            //TODO: Rework styling system
            //unapply parent style
            //if (this.PropagateStyleToChildren && !this.IsThemeRefreshSuspended)
            //{
            //    this.UnapplyStyle();
            //}            

            RadElement oldParent = this.parent;
            this.parent = parent;
            this.OnParentChanged(oldParent);
        }

        /// <summary>
        /// Notifies for a change in the Parent value.
        /// </summary>
        /// <param name="previousParent">The previous parent element (if any)</param>
        protected virtual void OnParentChanged(RadElement previousParent)
        {
            ComponentThemableElementTree previousTree = this.ElementTree;

            //Notify element tree change listeners, like StyleMnager 
            if (previousTree != null)
            {
                previousTree.NotifyElementRemoved(previousParent, this);
            }

            //inherit local references from the parent
            if (this.parent != null)
            {
                this.UpdateReferences(this.parent.elementTree, true, true);
            }
            else
            {
                this.UpdateReferences(null, true, true);
            }

            if (this.elementTree != null)
            {
                this.elementTree.NotifyElementAdded(this);
            }

            this.InvalidateMeasure(false);
            this.InvalidateArrange(false);

            if (parent == null)
            {
                if (!this.IsThemeRefreshSuspended)
                {
                    this.BitState[IsThemeAppliedStateKey] = false;
                }
            }
        }

        /// <summary>
        /// Updates the local references using the provided element tree.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="updateInheritance">True to update inheritance chain, false otherwise.</param>
        /// <param name="recursive">True to update children also, false otherwise.</param>
        protected internal virtual void UpdateReferences(ComponentThemableElementTree tree, bool updateInheritance, bool recursive)
        {
            //do nothing if we have reference update suspended
            if (this.suspendReferenceUpdate > 0)
            {
                this.UpdateState(tree);
                return;
            }

            ComponentThemableElementTree previousTree = this.elementTree;
            if (this.layoutManager != null)
            {
                this.layoutManager.RemoveElementFromQueues(this);
            }
            this.elementTree = tree;
            if (tree != null)
            {
                this.layoutManager = tree.ComponentTreeHandler.LayoutManager;
            }
            else
            {
                this.layoutManager = null;
            }

            bool load;
            bool unload;
            this.UpdateFromParent(out load, out unload);

            if (unload)
            {
                this.OnUnload(previousTree, false);
            }

            this.suspendRecursiveEnabledUpdates++;
            this.UpdateEnabledFromParent();
            this.suspendRecursiveEnabledUpdates--;

            if (recursive)
            {
                foreach (RadElement child in this.children)
                {
                    child.UpdateReferences(tree, updateInheritance, recursive);
                }
            }

            if (previousTree != tree)
            {
                this.OnElementTreeChanged(previousTree);
            }

            if (updateInheritance)
            {
                this.UpdateInheritanceChain(!recursive);
            }

            if (load)
            {
                this.OnLoad(true);
            }
        }

        private void UpdateFromParent(out bool load, out bool unload)
        {
            load = false;
            unload = false;

            if (this.parent != null)
            {
                this.IsDesignMode = this.parent.IsDesignMode;
                this.treeLevel = (byte)(this.parent.treeLevel + 1);
                this.BitState[UseNewLayoutStateKey] = this.parent.UseNewLayoutSystem;
                //we will get the element state from the parent
                //e.g if the parent is detached from the element tree, it will become unloaded, so should we
                if (this.IsCurrentStateInheritable() && this.parent.IsCurrentStateInheritable())
                {
                    if (this.state != this.parent.state)
                    {
                        //check whether we are added to a parent, which is already loaded, so that we raise the Load event properly
                        load = this.parent.state == ElementState.Loaded;
                        unload = this.parent.state == ElementState.Unloaded && this.state == ElementState.Loaded;
                        if (!(load || unload))
                        {
                            this.state = this.parent.state;
                        }
                    }
                }
            }
            else
            {
                this.treeLevel = 0;
                this.IsDesignMode = false;
                if (this.IsInValidState(false))
                {
                    unload = this.state == ElementState.Loaded;
                    if (!unload)
                    {
                        this.state = ElementState.Constructed;
                    }
                }
            }
        }

        private void UpdateEnabledFromParent()
        {
            if (this.parent != null)
            {
                if (!this.parent.Enabled)
                {
                    this.BindProperty(RadElement.EnabledProperty, this.parent, RadElement.EnabledProperty, PropertyBindingOptions.OneWay);
                }
                else
                {
                    this.ResetValue(RadElement.EnabledProperty, ValueResetFlags.Binding);
                }
            }
            else
            {
                this.ResetValue(RadElement.EnabledProperty, ValueResetFlags.Binding);
            }
        }

        internal void SetIsDesignMode(bool value, bool recursive)
        {
            if (this.suspendReferenceUpdate > 0)
            {
                return;
            }

            this.IsDesignMode = value;
            if (recursive)
            {
                foreach (RadElement child in this.children)
                {
                    child.SetIsDesignMode(value, recursive);
                }
            }
        }

        /// <summary>
        /// Updates the state of the element when reference update is suspended and we have a change in our parent.
        /// </summary>
        /// <param name="tree"></param>
        private void UpdateState(ComponentThemableElementTree tree)
        {
            if (this.layoutManager != null)
            {
                this.layoutManager.MeasureQueue.Remove(this);
                this.layoutManager.ArrangeQueue.Remove(this);
            }

            if (tree != null)
            {
                if (this.state == ElementState.Unloaded)
                {
                    this.state = ElementState.Loaded;
                }
            }
            else
            {
                if (this.state == ElementState.Loaded)
                {
                    this.state = ElementState.Unloaded;
                }
            }
        }

        private bool IsCurrentStateInheritable()
        {
            return this.state == ElementState.Constructed || this.state == ElementState.Loaded || this.state == ElementState.Unloaded;
        }

        /// <summary>
        /// Performs an update after a change in the Children collection.
        /// </summary>
        /// <param name="child">The element associated with the change.</param>
        /// <param name="changeOperation"></param>
        internal void ChangeCollection(RadElement child, ItemsChangeOperation changeOperation)
        {
            //if (this.state == ElementState.Disposed)
            //{
            //    throw new InvalidOperationException("Changing Children collection of an already disposed element");
            //}
            /*
             * by fdc
             * 注释以上四行
             * 目的是解决停靠窗口关闭时强制Dispose()后，关系程序是出错
             */
            //element is disposing, do nothing
            if (this.state == ElementState.Disposing)
            {
                return;
            }

            this.UpdateZOrderedCollection(child, changeOperation);

            switch (changeOperation)
            {
                case ItemsChangeOperation.Inserted:
                case ItemsChangeOperation.Set:
                    if (!this.GetBitState(UseNewLayoutStateKey))
                    {
                        child.InvalidateTransformations();
                    }
                    child.SetParent(this);
                    break;
                case ItemsChangeOperation.Removed:
                    if (!this.GetBitState(UseNewLayoutStateKey))
                    {
                        child.InvalidateTransformations();
                    }
                    child.SetParent(null);
                    break;
                case ItemsChangeOperation.Clearing:
                    int count = this.children.Count;
                    for (int i = 0; i < count; i++)
                    {
                        RadElement childElement = this.children[i];
                        if (!this.GetBitState(UseNewLayoutStateKey))
                        {
                            childElement.InvalidateTransformations();
                        }
                        childElement.SetParent(null);
                    }
                    break;
            }

            // update layout accordingly
            if (this.state == ElementState.Loaded)
            {
                if (this.GetBitState(UseNewLayoutStateKey))
                {
                    if (this.GetBitState(InvalidateMeasureOnRemoveStateKey))
                    {
                        this.InvalidateMeasure();
                    }
                }
                else
                {
                    if (child != null)
                    {
                        child.cachedBorderSize = LayoutUtils.InvalidSize;
                        child.cachedBorderOffset = LayoutUtils.InvalidSize;
                        if (child.ElementState == ElementState.Loaded)
                        {
                            child.LayoutEngine.InvalidateCachedBorder();
                        }
                    }
                    this.LayoutEngine.SetLayoutInvalidated(true);
                    this.Invalidate();
                    this.LayoutEngine.PerformParentLayout();
                }
            }

            //raise the ChildrenChanged event
            OnChildrenChanged(child, changeOperation);
        }

        /// <summary>
        /// Resets all layout related fields and puts the element in its initial layout state.
        /// </summary>
        /// <param name="recursive"></param>
        public void ResetLayout(bool recursive)
        {
            this.ResetLayout();
            if (recursive)
            {
                foreach (RadElement child in this.children)
                {
                    child.ResetLayout(recursive);
                }
            }
        }

        /// <summary>
        /// Determines whether there is an ancestor in this element tree that is not visible.
        /// </summary>
        /// <returns></returns>
        public bool HasInvisibleAncestor()
        {
            RadElement parent = this.parent;
            while (parent != null)
            {
                if (parent.Visibility != ElementVisibility.Visible)
                {
                    return true;
                }

                parent = parent.parent;
            }

            return false;
        }

        private void ResetLayout()
        {
            this._previousAvailableSize = SizeF.Empty;
            this.layoutTransformDataField = null;
            this._finalRect = RectangleF.Empty;
            this.UnclippedDesiredSize = null;
            this._desiredSize = SizeF.Empty;
            this.layoutOffset = Point.Empty;

            //update bit state
            RadBitVector64 bitState = this.BitState;
            bitState[ArrangeDirtyStateKey] = false;
            bitState[ArrangeInProgressStateKey] = false;
            bitState[NeverArrangedStateKey] = true;
            bitState[MeasureDirtyStateKey] = false;
            bitState[MeasureDuringArrangeStateKey] = false;
            bitState[MeasureInProgressStateKey] = false;
            bitState[NeverMeasuredStateKey] = true;
            bitState[IsLayoutInvalidatedStateKey] = true;

            if (this.ArrangeRequest != null && this.layoutManager != null)
            {
                this.layoutManager.ArrangeQueue.Remove(this);
            }
            this.ArrangeRequest = null;
            if (this.MeasureRequest != null && this.layoutManager != null)
            {
                this.layoutManager.MeasureQueue.Remove(this);
            }
            this.MeasureRequest = null;
            //reset bounds
            this.SuspendPropertyNotifications();

            //Do not reset bounds in case autosize is false. In this way we preserve
            //the last actual size of the element before the AutoSize property was set.
            if (this.AutoSize)
            {
                this.cachedBounds = LayoutUtils.InvalidBounds;
                this.ResetValue(BoundsProperty, ValueResetFlags.Local);
            }

            this.ResetTransformations();
            this.ResumePropertyNotifications();

            this.ResumeLayout(false, false);
        }

        private void ResetTransformations()
        {
            this.transform = RadMatrix.Empty;
            this.totalTransform = RadMatrix.Empty;

            this.cachedFaceRectangle = Rectangle.Empty;
            this.cachedFullRectangle = Rectangle.Empty;
            this.cachedControlBoundingRectangle = Rectangle.Empty;
            this.cachedBoundingRectangle = Rectangle.Empty;
        }

        private void DetachFromElementTree(ComponentThemableElementTree oldTree)
        {
            if (oldTree == null)
            {
                return;
            }

            ComponentInputBehavior behavior = oldTree.ComponentTreeHandler.Behavior;
            if (behavior.currentFocusedElement == this)
            {
                behavior.currentFocusedElement = null;
            }
            if (behavior.selectedElement == this)
            {
                behavior.selectedElement = null;
            }
            if (behavior.itemCapture == this)
            {
                behavior.itemCapture = null;
            }
            if (this.IsFocused)
            {
                this.SuspendPropertyNotifications();
                this.SetValue(IsFocusedProperty, false);
                if (behavior.FocusedElement == this)
                {
                    behavior.FocusedElement = null;
                }
                this.ResumePropertyNotifications();
            }

            oldTree.ComponentTreeHandler.LayoutManager.RemoveElementFromQueues(this);
            this.MeasureRequest = null;
            this.ArrangeRequest = null;
        }

        internal void SetChildrenLocalValuesAsDefault(bool recursive)
        {
            int count = this.children.Count;
            for (int i = 0; i < count; i++)
            {
                this.children[i].SetAllLocalValuesAsDefault(false);
                if (recursive)
                {
                    this.children[i].SetChildrenLocalValuesAsDefault(recursive);
                }
            }
        }

        internal void SetAllLocalValuesAsDefault(bool recursive)
        {
            this.PropertyValues.SetLocalValuesAsDefault();

            if (recursive)
            {
                int count = this.children.Count;
                for (int i = 0; i < count; i++)
                {
                    this.children[i].SetAllLocalValuesAsDefault(recursive);
                }
            }
        }

        #endregion

        #region Layout

        #region New Layout

        public SizeF GetDesiredSize(bool checkCollapsed)
        {
            if (checkCollapsed && this.Visibility == ElementVisibility.Collapsed)
            {
                return SizeF.Empty;
            }

            return this._desiredSize;
        }

#if DEBUG
        [Browsable(true), Category(RadDesignCategory.TPFDebugCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool NeverArrangedProperty
        {
            get
            {
                return this.GetBitState(NeverArrangedStateKey);
            }
            set
            {
                this.SetBitState(NeverArrangedStateKey, value);
            }
        }

        [Browsable(true), Category(RadDesignCategory.TPFDebugCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool NeverMeasuredProperty
        {
            get
            {
                return this.GetBitState(NeverMeasuredStateKey);
            }
            set
            {
                this.SetBitState(NeverMeasuredStateKey, value);
            }
        }
#endif

        /// <summary>
        /// Gets the layout manager of the element. Will be null until the element is loaded on a visual scene.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ILayoutManager LayoutManager
        {
            get
            {
                return this.layoutManager;
            }
        }

        [Browsable(false)]
        public event EventHandler LayoutUpdated
        {
            add
            {
                if (this.layoutManager != null)
                {
                    this.layoutManager.LayoutEvents.Add(value);
                }
                else
                {
                    this.layoutEvents.Add(value);
                }
            }
            remove
            {
                if (this.layoutManager != null)
                {
                    this.layoutManager.LayoutEvents.Remove(value);
                }
                else
                {
                    this.layoutEvents.Remove(value);
                }
            }
        }

#if DEBUG
        [Browsable(true), Category(RadDesignCategory.TPFDebugCategory)]
#else
        [Browsable(false)]
#endif
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool BypassLayoutPolicies
        {
            get
            {
                return this.GetBitState(BypassLayoutPoliciesStateKey);
            }
            set
            {
                this.SetBitState(BypassLayoutPoliciesStateKey, value);
            }
        }

#if DEBUG
        [Browsable(true), Category(RadDesignCategory.TPFDebugCategory)]
#else
        [Browsable(false)]
#endif
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsArrangeValid
        {
            get
            {
                return !this.GetBitState(ArrangeDirtyStateKey);
            }
        }

#if DEBUG
        [Browsable(true), Category(RadDesignCategory.TPFDebugCategory)]
#else
        [Browsable(false)]
#endif
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsMeasureValid
        {
            get
            {
                return !this.GetBitState(MeasureDirtyStateKey);
            }
        }

        private LayoutTransformData layoutTransformDataField;
#if DEBUG
        [Browsable(true), Category(RadDesignCategory.TPFDebugCategory)]
        public SizeF UntransformedDesiredSize
        {
            get
            {
                if (this.layoutTransformDataField != null)
                    return this.layoutTransformDataField.UntransformedDS;
                return new Size(-1, -1);
            }
        }
#endif

#if DEBUG
        [Browsable(true), Category(RadDesignCategory.TPFDebugCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PointF LayoutOffset
        {
            get
            {
                return layoutOffset;
            }
        }
#endif

#if DEBUG
        [Browsable(true), Category(RadDesignCategory.TPFDebugCategory)]
#else
        [Browsable(false)]
#endif
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SizeF DesiredSize
        {
            get
            {
                if (this.Visibility == ElementVisibility.Collapsed)
                {
                    return SizeF.Empty;
                }
                return this._desiredSize;
            }
        }

#if DEBUG
        [Browsable(true), Category(RadDesignCategory.TPFDebugCategory)]
#else
        [Browsable(false)]
#endif
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal RectangleF PreviousArrangeRect
        {
            get
            {
                return this._finalRect;
            }
        }

#if DEBUG
        [Browsable(true), Category(RadDesignCategory.TPFDebugCategory)]
#else
        [Browsable(false)]
#endif
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal SizeF PreviousConstraint
        {
            get
            {
                return this._previousAvailableSize;
            }
        }

        /// <summary>
        /// Gets the level of this element in the ElementTree it currently resides.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public byte TreeLevel
        {
            get
            {
                return this.treeLevel;
            }
        }

        public void InvalidateArrange()
        {
            this.InvalidateArrange(false);
        }

        public void InvalidateArrange(bool recursive)
        {
            if (!this.IsInValidState(true))
            {
                this.SetBitState(ArrangeDirtyStateKey, true);
                return;
            }

            if (!this.GetBitState(ArrangeDirtyStateKey) && !this.GetBitState(ArrangeInProgressStateKey))
            {
                if (!this.GetBitState(NeverArrangedStateKey) && this.layoutSuspendCount == 0)
                {
                    this.layoutManager.ArrangeQueue.Add(this);
                }
                this.BitState[ArrangeDirtyStateKey] = true;
            }

            if (recursive)
            {
                foreach (RadElement child in this.children)
                {
                    child.InvalidateArrange(recursive);
                }
            }
        }

        internal void InvalidateArrangeInternal()
        {
            this.BitState[ArrangeDirtyStateKey] = true;
        }

        public void InvalidateMeasure()
        {
            this.InvalidateMeasure(false);
        }

        public void InvalidateMeasure(bool recursive)
        {
            if (!this.IsInValidState(true))
            {
                this.BitState[MeasureDirtyStateKey] = true;
                return;
            }

            if (!this.GetBitState(MeasureDirtyStateKey) && !this.GetBitState(MeasureInProgressStateKey))
            {
                if (!this.GetBitState(NeverMeasuredStateKey) && this.layoutSuspendCount == 0)
                {
                    this.layoutManager.MeasureQueue.Add(this);
                }

                this.BitState[MeasureDirtyStateKey] = true;
            }

            if (recursive)
            {
                foreach (RadElement child in this.children)
                {
                    child.InvalidateMeasure(true);
                }
            }
        }

        internal void InvalidateMeasureInternal()
        {
            this.BitState[MeasureDirtyStateKey] = true;
        }

        public void UpdateLayout()
        {
            if (!this.CanExecuteLayoutOperation())
            {
                return;
            }

            Debug.Assert(this.layoutManager != null, "Not properly updated LayoutManager reference.");
            this.layoutManager.UpdateLayout();
        }

        protected virtual PointF CalcLayoutOffset(PointF startPoint)
        {
            startPoint.X = startPoint.X + this.Margin.Left + this.Location.X;
            startPoint.Y = startPoint.Y + this.Margin.Top + this.Location.Y;
            return startPoint;
        }

        private void SetLayoutParams(PointF newOffset, SizeF newSize)
        {
            bool paramChanged =
                !DoubleUtil.AreClose(newOffset, this.layoutOffset) ||
                !DoubleUtil.AreClose(newSize, this.Size);
            if (this.GetBitState(InvalidateOnArrangeStateKey) || paramChanged)
            {
                if (paramChanged)
                {
                    if (TraceInvalidation)
                        Console.WriteLine("SetLayoutParams FIRST: {0}; ", this.GetType().Name);

                    this.Invalidate();

                    this.layoutOffset = Point.Round(newOffset);
                    this.Size = Size.Round(newSize);
                    InvalidateTransformations(false);
                    CallOnTransformationInvalidatedRecursively();
                }

                if (TraceInvalidation)
                    Console.WriteLine("SetLayoutParams SECOND: {0}; ", this.GetType().Name);
                this.Invalidate();

                this.BitState[InvalidateOnArrangeStateKey] = false;
            }
        }

        private bool markForSizeChangedIfNeeded(Size oldSize, Size newSize)
        {
            if (!this.IsInValidState(true))
            {
                return false;
            }

            bool widthChanged = oldSize.Width != newSize.Width;
            bool heightChanged = oldSize.Height != newSize.Height;
            SizeChangedInfo info1 = this.sizeChangedInfo;
            if (info1 != null)
            {
                info1.Update(widthChanged, heightChanged);
                return true;
            }
            if (!widthChanged && !heightChanged)
            {
                return false;
            }
            info1 = new SizeChangedInfo(this, oldSize, widthChanged, heightChanged);
            this.sizeChangedInfo = info1;
            this.layoutManager.AddToSizeChangedChain(info1);
            return true;
        }

        protected internal virtual void OnRenderSizeChanged(SizeChangedInfo info)
        {
        }

        protected virtual void OnChildDesiredSizeChanged(RadElement child)
        {
            // ??? Should we invalidate the measure if we are not AutoSize
            if (this.IsMeasureValid && this.AutoSize)
            {
                this.InvalidateMeasure();
            }
        }

        // Call this function when Visibility is set to Collapsed (or to something different than Collapsed)
        private void SignalDesiredSizeChange()
        {
            RadElement parent = this.Parent;
            if (parent != null)
            {
                parent.OnChildDesiredSizeChanged(this);
            }
        }

        private SizeF FindMaximalAreaLocalSpaceRect(RadMatrix layoutTransform, SizeF transformSpaceBounds)
        {
            float width = transformSpaceBounds.Width;
            float height = transformSpaceBounds.Height;
            if (DoubleUtil.IsZero(width) || DoubleUtil.IsZero(height))
            {
                return new Size(0, 0);
            }
            bool flag = float.IsInfinity(width);
            bool flag2 = float.IsInfinity(height);
            if (flag && flag2)
            {
                return new SizeF(float.PositiveInfinity, float.PositiveInfinity);
            }
            if (flag)
            {
                width = height;
            }
            else if (flag2)
            {
                height = width;
            }
            if (!layoutTransform.IsInvertible)
            {
                return new Size(0, 0);
            }
            float num3 = layoutTransform.M11;
            float num4 = layoutTransform.M12;
            float num5 = layoutTransform.M21;
            float num6 = layoutTransform.M22;
            float num7 = 0;
            float num8 = 0;
            if (DoubleUtil.IsZero(num4) || DoubleUtil.IsZero(num5))
            {
                float num9 = flag2 ? float.PositiveInfinity : Math.Abs((float)(height / num6));
                float num10 = flag ? float.PositiveInfinity : Math.Abs((float)(width / num3));
                if (DoubleUtil.IsZero(num4))
                {
                    if (DoubleUtil.IsZero(num5))
                    {
                        num8 = num9;
                        num7 = num10;
                    }
                    else
                    {
                        num8 = Math.Min(0.5f * Math.Abs((float)(width / num5)), num9);
                        num7 = num10 - ((num5 * num8) / num3);
                    }
                }
                else
                {
                    num7 = Math.Min(0.5f * Math.Abs((float)(height / num4)), num10);
                    num8 = num9 - ((num4 * num7) / num6);
                }
            }
            else if (DoubleUtil.IsZero(num3) || DoubleUtil.IsZero(num6))
            {
                float num11 = Math.Abs((float)(height / num4));
                float num12 = Math.Abs((float)(width / num5));
                if (DoubleUtil.IsZero(num3))
                {
                    if (DoubleUtil.IsZero(num6))
                    {
                        num8 = num12;
                        num7 = num11;
                    }
                    else
                    {
                        num8 = Math.Min(0.5f * Math.Abs((float)(height / num6)), num12);
                        num7 = num11 - ((num6 * num8) / num4);
                    }
                }
                else
                {
                    num7 = Math.Min(0.5f * Math.Abs((float)(width / num3)), num11);
                    num8 = num12 - ((num3 * num7) / num5);
                }
            }
            else
            {
                float num13 = Math.Abs((float)(width / num3));
                float num14 = Math.Abs((float)(width / num5));
                float num15 = Math.Abs((float)(height / num4));
                float num16 = Math.Abs((float)(height / num6));
                num7 = Math.Min(num15, num13) * 0.5f;
                num8 = Math.Min(num14, num16) * 0.5f;
                if ((DoubleUtil.GreaterThanOrClose(num13, num15) && DoubleUtil.LessThanOrClose(num14, num16)) || (DoubleUtil.LessThanOrClose(num13, num15) && DoubleUtil.GreaterThanOrClose(num14, num16)))
                {
                    RectangleF rect = TelerikHelper.GetBoundingRect(new RectangleF(0, 0, num7, num8), layoutTransform);
                    float d = Math.Min((float)(width / rect.Width), (float)(height / rect.Height));
                    if (!float.IsNaN(d) && !float.IsInfinity(d))
                    {
                        num7 *= d;
                        num8 *= d;
                    }
                }
            }
            return new SizeF(num7, num8);
        }

        /// <summary>
        /// Determines whether the element can perform layout operation.
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanExecuteLayoutOperation()
        {
            return this.state == ElementState.Loaded && this.layoutSuspendCount == 0;
        }

        private void UpdateLayoutRequest(bool isArrange)
        {
            if (this.layoutManager == null)
            {
                return;
            }

            ContextLayoutManager.LayoutQueue.Request request;
            ILayoutQueue queue;
            if (isArrange)
            {
                request = this.ArrangeRequest;
                queue = this.layoutManager.ArrangeQueue;
            }
            else
            {
                request = this.MeasureRequest;
                queue = this.layoutManager.MeasureQueue;
            }

            if (request == null)
            {
                return;
            }

            if (this.layoutSuspendCount > 0 || this.layoutManager.IsUpdating)
            {
                queue.Remove(this);
            }
        }

        public void Arrange(RectangleF finalRect)
        {
            if (!this.CanExecuteLayoutOperation())
            {
                this.UpdateLayoutRequest(true);
                return;
            }

            try
            {
                this.layoutManager.IncrementLayoutCalls();

                if ((float.IsPositiveInfinity(finalRect.Width) || float.IsPositiveInfinity(finalRect.Height)) || (float.IsNaN(finalRect.Width) || float.IsNaN(finalRect.Height)))
                {
                    RadElement element1 = this.Parent;
                    string str = string.Format("Arrange with infinity or NaN size (parent: {0}; this: {1})", (element1 == null) ? "" : element1.GetType().FullName, this.GetType().FullName);
                    throw new InvalidOperationException(str);
                }
                if ((this.Visibility == ElementVisibility.Collapsed) || this.IsLayoutSuspended)
                {
                    if (this.ArrangeRequest != null)
                    {
                        this.layoutManager.ArrangeQueue.Remove(this);
                    }
                    this._finalRect = finalRect;
                    return;
                }

                if (this.GetBitState(MeasureDirtyStateKey) || this.GetBitState(NeverMeasuredStateKey))
                {
                    try
                    {
                        this.BitState[MeasureDuringArrangeStateKey] = true;
                        if (this.GetBitState(NeverMeasuredStateKey))
                        {
                            this.Measure(finalRect.Size);
                        }
                        else
                        {
                            this.Measure(this._previousAvailableSize);
                        }
                    }
                    finally
                    {
                        this.BitState[MeasureDuringArrangeStateKey] = false;
                    }
                }
                if ((!this.IsArrangeValid || this.GetBitState(NeverArrangedStateKey)) || !DoubleUtil.AreClose(finalRect, this._finalRect) || !this.AutoSize)
                {
                    this.BitState[NeverArrangedStateKey] = false;
                    this.BitState[ArrangeInProgressStateKey] = true;
                    ILayoutManager manager1 = this.layoutManager;
                    Size size1 = this.Size;
                    bool sizeChanged = false;
                    try
                    {
                        manager1.EnterArrange();
                        this.ArrangeCore(finalRect);
                        sizeChanged = this.markForSizeChangedIfNeeded(size1, this.Size);
                    }
                    finally
                    {
                        this.BitState[ArrangeInProgressStateKey] = false;
                        manager1.ExitArrange();
                    }
                    this._finalRect = finalRect;
                    this.BitState[ArrangeDirtyStateKey] = false;
                    if (this.ArrangeRequest != null)
                    {
                        this.layoutManager.ArrangeQueue.Remove(this);
                    }
                }
            }
            finally
            {
                this.layoutManager.DecrementLayoutCalls();
            }
        }

        protected virtual void ArrangeCore(RectangleF finalRect)
        {
            if (!this.AutoSize)
            {
                Size sz = this.Size;
                this.ArrangeOverride(sz);
                SetLayoutParams(CalcLayoutOffset(finalRect.Location), sz);
                return;
            }

            if (this.BypassLayoutPolicies)
            {
                SizeF size1 = this.ArrangeOverride(finalRect.Size);
                SetLayoutParams(CalcLayoutOffset(finalRect.Location), size1);
            }
            else
            {
                SizeF transformSpaceBounds = finalRect.Size;
                Padding thickness1 = this.Margin;
                int horizontalMargin = thickness1.Left + thickness1.Right;
                int verticalMargin = thickness1.Top + thickness1.Bottom;

                transformSpaceBounds.Width = Math.Max(0, (transformSpaceBounds.Width - horizontalMargin));
                transformSpaceBounds.Height = Math.Max(0, (transformSpaceBounds.Height - verticalMargin));

                SizeF untransformedDS = SizeF.Empty;
                SizeBox box = this.UnclippedDesiredSize;
                if (box == null)
                {
                    untransformedDS = new SizeF(this.DesiredSize.Width - horizontalMargin, this.DesiredSize.Height - verticalMargin);
                }
                else
                {
                    untransformedDS = new SizeF(box.Width, box.Height);
                }

                if (DoubleUtil.LessThan(transformSpaceBounds.Width, untransformedDS.Width))
                {
                    transformSpaceBounds.Width = untransformedDS.Width;
                }
                if (DoubleUtil.LessThan(transformSpaceBounds.Height, untransformedDS.Height))
                {
                    transformSpaceBounds.Height = untransformedDS.Height;
                }
                if (!this.StretchHorizontally)
                {
                    transformSpaceBounds.Width = untransformedDS.Width;
                }
                if (!this.StretchVertically)
                {
                    transformSpaceBounds.Height = untransformedDS.Height;
                }

                LayoutTransformData data1 = this.layoutTransformDataField;
                if (data1 != null)
                {
                    SizeF size5 = this.FindMaximalAreaLocalSpaceRect(data1.transform, transformSpaceBounds);
                    transformSpaceBounds = size5;
                    untransformedDS = data1.UntransformedDS;

                    if ((!DoubleUtil.IsZero(size5.Width) && !DoubleUtil.IsZero(size5.Height)) && (DoubleUtil.LessThan(size5.Width, untransformedDS.Width) || DoubleUtil.LessThan(size5.Height, untransformedDS.Height)))
                    {
                        transformSpaceBounds = untransformedDS;
                    }

                    if (DoubleUtil.LessThan(transformSpaceBounds.Width, untransformedDS.Width))
                    {
                        transformSpaceBounds.Width = untransformedDS.Width;
                    }

                    if (DoubleUtil.LessThan(transformSpaceBounds.Height, untransformedDS.Height))
                    {
                        transformSpaceBounds.Height = untransformedDS.Height;
                    }
                }

                MinMax max1 = new MinMax(this);
                float num3 = (float)Math.Max(untransformedDS.Width, max1.maxWidth);
                if (DoubleUtil.LessThan(num3, transformSpaceBounds.Width))
                {
                    transformSpaceBounds.Width = num3;
                }

                float num4 = (float)Math.Max(untransformedDS.Height, max1.maxHeight);
                if (DoubleUtil.LessThan(num4, transformSpaceBounds.Height))
                {
                    transformSpaceBounds.Height = num4;
                }

                SizeF size7 = this.ArrangeOverride(transformSpaceBounds);

                SizeF inkSize = size7;
                if (max1.maxWidth > 0)
                    inkSize.Width = Math.Min(size7.Width, max1.maxWidth);
                if (max1.maxHeight > 0)
                    inkSize.Height = Math.Min(size7.Height, max1.maxHeight);

                if (data1 != null)
                {
                    RectangleF boundingRect = TelerikHelper.GetBoundingRect(
                        new RectangleF(Point.Empty, inkSize), data1.transform);
                    inkSize = boundingRect.Size;
                }

                SizeF clientSize = new SizeF(Math.Max(0f, finalRect.Width - horizontalMargin), Math.Max(0f, finalRect.Height - verticalMargin));

                // Gele was here BEGIN
                ContentAlignment alignment = this.RightToLeft ? TelerikAlignHelper.RtlTranslateContent(this.Alignment) : this.Alignment;
                RectangleF alignedRect = LayoutUtils.Align(inkSize, new RectangleF(finalRect.Location, clientSize), alignment);
                SetLayoutParams(CalcLayoutOffset(alignedRect.Location), size7);
            }
        }

        /// <summary>
        /// Arranges the <see cref="RadElement"/> to its final location.
        /// The element must call the Arrange method of each of its children.
        /// </summary>
        /// <param name="finalSize">The size that is available for element.</param>
        /// <returns>The rectangle occupied by the element. Usually <paramref name="finalSize"/>. Should you return different size, the Layout system will restart measuring and rearraning the items. That could lead to infinite recursion.</returns>
        /// <remarks>In this method call to the Arrange method of each child must be made.</remarks>
        protected virtual SizeF ArrangeOverride(SizeF finalSize)
        {
            for (int i = 0; i < this.children.Count; i++)
            {
                RadElement child = this.children[i];
                RectangleF fullRect = new RectangleF(PointF.Empty, finalSize);

                if (!this.BypassLayoutPolicies)
                {
                    if (child.FitToSizeMode == RadFitToSizeMode.FitToParentContent ||
                        child.FitToSizeMode == RadFitToSizeMode.FitToParentPadding)
                    {
                        Padding border = this.BorderThickness;
                        fullRect.Location = PointF.Add(fullRect.Location, new SizeF(border.Left, border.Top));
                        fullRect.Size = SizeF.Subtract(fullRect.Size, border.Size);
                    }
                    if (child.FitToSizeMode == RadFitToSizeMode.FitToParentContent)
                    {
                        fullRect.Location = PointF.Add(fullRect.Location, new SizeF(this.Padding.Left, this.Padding.Top));
                        fullRect.Size = SizeF.Subtract(fullRect.Size, this.Padding.Size);
                    }
                }

                child.Arrange(fullRect);
            }
            return finalSize;
        }

        public void Measure(SizeF availableSize)
        {
            if (!this.CanExecuteLayoutOperation())
            {
                this.UpdateLayoutRequest(false);
                return;
            }

            try
            {
                this.layoutManager.IncrementLayoutCalls();

                if (float.IsNaN(availableSize.Width) || float.IsNaN(availableSize.Height))
                {
                    throw new InvalidOperationException("Measure with NaN available size");
                }

                if ((this.Visibility == ElementVisibility.Collapsed) || this.IsLayoutSuspended)
                {
                    if (this.MeasureRequest != null)
                    {
                        this.layoutManager.MeasureQueue.Remove(this);
                    }
                    if (!DoubleUtil.AreClose(availableSize, this._previousAvailableSize))
                    {
                        this.InvalidateMeasureInternal();
                        this._previousAvailableSize = availableSize;
                    }
                    return;
                }
                if ((this.IsMeasureValid && !this.GetBitState(NeverMeasuredStateKey)) && DoubleUtil.AreClose(availableSize, this._previousAvailableSize))
                {
                    if (this.MeasureRequest != null)
                    {
                        this.layoutManager.MeasureQueue.Remove(this);
                    }
                    return;
                }
                this.BitState[NeverMeasuredStateKey] = false;
                SizeF oldDesiredSize = this._desiredSize;
                this.InvalidateArrange();
                this.BitState[MeasureInProgressStateKey] = true;
                SizeF size1 = new SizeF(0, 0);
                ILayoutManager manager1 = this.layoutManager;
                try
                {
                    manager1.EnterMeasure();
                    size1 = this.MeasureCore(availableSize);
                }
                finally
                {
                    this.BitState[MeasureInProgressStateKey] = false;
                    this._previousAvailableSize = availableSize;
                    manager1.ExitMeasure();
                }
                if (float.IsPositiveInfinity(size1.Width) || float.IsPositiveInfinity(size1.Height))
                {
                    string str = string.Format("MeasureOverride returned positive infinity: {0}", GetType().FullName);
                    throw new InvalidOperationException(str);
                }
                if (float.IsNaN(size1.Width) || float.IsNaN(size1.Height))
                {
                    string str = string.Format("MeasureOverride returned NaN: {0}", GetType().FullName);
                    throw new InvalidOperationException(str);
                }
                this.BitState[MeasureDirtyStateKey] = false;
                if (this.MeasureRequest != null)
                {
                    this.layoutManager.MeasureQueue.Remove(this);
                }
                this._desiredSize = size1;
                if (!this.GetBitState(MeasureDuringArrangeStateKey) && !DoubleUtil.AreClose(oldDesiredSize, size1))
                {
                    this.BitState[InvalidateOnArrangeStateKey] = true;
                    RadElement element1 = this.Parent;
                    if ((element1 != null) && !element1.GetBitState(MeasureInProgressStateKey))
                    {
                        element1.OnChildDesiredSizeChanged(this);
                    }
                }
            }
            finally
            {
                if (this.layoutManager != null)
                {
                    this.layoutManager.DecrementLayoutCalls();
                }
            }
        }

        protected virtual SizeF MeasureCore(SizeF availableSize)
        {
            if (!this.AutoSize)
            {
                Size sz = this.Size;
                this.MeasureOverride(sz);
                if (this.BypassLayoutPolicies)
                    sz = Size.Empty;
                return sz;
            }

            if (this.BypassLayoutPolicies)
            {
                return this.MeasureOverride(availableSize);
            }

            Padding marginThickness = this.Margin;
            int horizontalMargin = marginThickness.Left + marginThickness.Right;
            int verticalMargin = marginThickness.Top + marginThickness.Bottom;

            SizeF transformSpaceBounds = new SizeF(Math.Max(availableSize.Width - horizontalMargin, 0f),
                Math.Max(availableSize.Height - verticalMargin, 0f));
            MinMax max1 = new MinMax(this);

            LayoutTransformData data1 = this.layoutTransformDataField;
            if (!this.Transform.IsIdentity)
            {
                if (data1 == null)
                {
                    data1 = new LayoutTransformData();
                    this.layoutTransformDataField = data1;
                }
                data1.CreateTransformSnapshot(this.Transform);
                data1.UntransformedDS = new SizeF();
            }
            else if (data1 != null)
            {
                data1 = null;
                this.layoutTransformDataField = null;
            }
            if (data1 != null)
            {
                transformSpaceBounds = this.FindMaximalAreaLocalSpaceRect(data1.transform, transformSpaceBounds);
            }

            transformSpaceBounds.Width = Math.Max(max1.minWidth, Math.Min(transformSpaceBounds.Width, max1.maxWidth));
            transformSpaceBounds.Height = Math.Max(max1.minHeight, Math.Min(transformSpaceBounds.Height, max1.maxHeight));

            SizeF measuredSize = this.MeasureOverride(transformSpaceBounds);

            measuredSize = new SizeF(Math.Max(measuredSize.Width, max1.minWidth), Math.Max(measuredSize.Height, max1.minHeight));

            SizeF size3 = measuredSize;
            if (data1 != null)
            {
                data1.UntransformedDS = size3;
                RectangleF boundingRect = TelerikHelper.GetBoundingRect(
                    new RectangleF(PointF.Empty, size3), data1.transform);
                size3 = boundingRect.Size;
            }

            bool flag1 = false;
            if (measuredSize.Width > max1.maxWidth && max1.maxWidth > 0)
            {
                measuredSize.Width = max1.maxWidth;
                flag1 = true;
            }
            if (measuredSize.Height > max1.maxHeight && max1.maxHeight > 0)
            {
                measuredSize.Height = max1.maxHeight;
                flag1 = true;
            }

            if (data1 != null)
            {
                RectangleF boundingRect = TelerikHelper.GetBoundingRect(
                    new RectangleF(Point.Empty, measuredSize), data1.transform);
                measuredSize = boundingRect.Size;
            }

            float fullWidth = measuredSize.Width + horizontalMargin;
            float fullHeight = measuredSize.Height + verticalMargin;
            if (fullWidth > availableSize.Width)
            {
                fullWidth = availableSize.Width;
                flag1 = true;
            }
            if (fullHeight > availableSize.Height)
            {
                fullHeight = availableSize.Height;
                flag1 = true;
            }

            SizeBox box1 = this.UnclippedDesiredSize;
            if (flag1 || fullWidth < 0 || fullHeight < 0)
            {
                if (box1 == null)
                {
                    box1 = new SizeBox(size3);
                    this.UnclippedDesiredSize = box1;
                }
                else
                {
                    box1.Width = size3.Width;
                    box1.Height = size3.Height;
                }
            }
            else if (box1 != null)
            {
                this.UnclippedDesiredSize = null;
            }

            return new SizeF(Math.Max(0f, fullWidth), Math.Max(0f, fullHeight));
        }

        /// <summary>
        /// Measures the space required by the <see cref="RadElement"/>
        /// 
        /// Used by the layout system.
        /// </summary>
        /// <param name="availableSize">The size that is available to the <see cref="RadElement"/>. The available size can be infinity (to take the full size of the element)</param>
        /// <returns>The minimum size required by the element to be completely visible. Cannot be infinify.</returns>
        /// <remarks>In this method call to the Measure method of each child must be made.</remarks>
        protected virtual SizeF MeasureOverride(SizeF availableSize)
        {
            return this.MeasureChildren(availableSize);
        }

        protected SizeF MeasureChildren(SizeF availableSize)
        {
            SizeF totalSize = SizeF.Empty;

            //TODO: Added by Georgi
            //This is the way it should be. Test carefully however since there might be some bugs relying on NOT HAVING THIS.
            //Padding padding = this.Padding;
            //availableSize.Width -= padding.Horizontal;
            //availableSize.Height -= padding.Vertical;

            if (this.AutoSize)
            {
                for (int i = 0; i < this.Children.Count; i++)
                {
                    RadElement child = this.Children[i];
                    child.Measure(availableSize);

                    SizeF s = child.DesiredSize;

                    if (!this.BypassLayoutPolicies)
                    {
                        if (child.FitToSizeMode == RadFitToSizeMode.FitToParentContent)
                        {
                            s = SizeF.Add(s, this.Padding.Size);
                        }
                        if (child.FitToSizeMode == RadFitToSizeMode.FitToParentContent ||
                            child.FitToSizeMode == RadFitToSizeMode.FitToParentPadding)
                        {
                            s = SizeF.Add(s, this.BorderThickness.Size);
                        }
                    }

                    if (totalSize.Width < s.Width)
                        totalSize.Width = s.Width;
                    if (totalSize.Height < s.Height)
                        totalSize.Height = s.Height;
                }
            }
            else
            {
                for (int i = 0; i < this.Children.Count; i++)
                {
                    RadElement child = this.Children[i];
                    child.Measure(availableSize);
                }
                totalSize = this.Size;
            }

            return totalSize;
        }

        /// <summary>
        /// Gets the arrange rectangle, valid for this element.
        /// </summary>
        /// <param name="proposed"></param>
        /// <returns></returns>
        protected internal virtual RectangleF GetArrangeRect(RectangleF proposed)
        {
            return proposed;
        }

        #endregion

        #region Old Layout

        /// <summary>
        /// Gets or sets the coerced size of the element. The coerced size is with higher
        /// priority than the size which is calculated when the element's AutoSizeMode is taken
        /// into consideration.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Size CoercedSize
        {
            get
            {
                return this.coercedSize;
            }
            set
            {
                if (this.state != ElementState.Loaded)
                {
                    return;
                }

                this.LayoutEngine.SetCoercedSize(value);
                this.Size = this.coercedSize;
            }
        }

        protected Size ParentFixedSize
        {
            get
            {
                Size coercedSize = LayoutUtils.InvalidSize;

                if (this.Parent == null)
                    return coercedSize;

                if (this.Parent != null)
                    coercedSize = this.Parent.CoercedSize;

                if (!this.Parent.AutoSize)
                    coercedSize = this.Parent.Size;

                if (coercedSize == LayoutUtils.InvalidSize)
                    return coercedSize;

                foreach (RadElement child in this.Parent.GetChildren(ChildrenListOptions.Normal))
                {
                    if (child == this)
                        continue;

                    IBoxElement boxChild = child as IBoxElement;

                    if (boxChild == null)
                        continue;

                    if (child.LayoutableChildrenCount > 0)
                        continue;

                    coercedSize.Width -= (int)boxChild.HorizontalWidth;
                    coercedSize.Height -= (int)boxChild.VerticalWidth;
                }

                return coercedSize;
            }
        }

        public void SetCoercedSize(Size coercedSize)
        {
            this.coercedSize = coercedSize;
        }

        /// <summary>
        /// Gets or sets the coerced bounds of the element. The bounds are with higher
        /// priority than the bounds which are calculated when the element's AutoSizeMode is taken
        /// into consideration.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle CoercedBounds
        {
            get
            {
                return new Rectangle(this.Location, this.CoercedSize);
            }
            set
            {
                if (this.state != ElementState.Loaded)
                {
                    return;
                }

                this.LayoutEngine.SetCoercedSize(value.Size);
                this.Bounds = value;
            }
        }

        /// <summary>
        /// Gets or sets the layout engine which is responsible for the element's layout. The
        /// Layout Engine defines the default behavior of the element's layout - how the element
        /// size is calculated and how it delegates the layout calculations to its child
        /// elements.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual ILayoutEngine LayoutEngine
        {
            get
            {
                if (this.state != ElementState.Loaded)
                {
                    return null;
                }

                if (this.layoutEngine == null)
                {
                    this.layoutEngine = new TelerikLayoutEngine(this);
                }

                return this.layoutEngine;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the element has been invalidated and
        /// whether the layout system will recalculate its bounds the next time a layout is
        /// performing. The property is set automatically to true when a RadProperty which
        /// invalidates the layout has been changed.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsLayoutInvalidated
        {
            get
            {
                return this.GetBitState(IsLayoutInvalidatedStateKey);
            }
            set
            {
                this.SetBitState(IsLayoutInvalidatedStateKey, value);
            }
        }

#if DEBUG
        [Browsable(true), Category(RadDesignCategory.TPFDebugCategory)]
#else
        [Browsable(false)]
#endif
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsLayoutSuspended
        {
            get
            {
                return (layoutSuspendCount > 0);
            }
        }

        /// <summary>Gets the topmost parent element which layout has been suspended.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadElement SuspendedParent
        {
            get
            {
                if (this.parent != null)
                {
                    if (this.parent.IsLayoutSuspended)
                        return this.parent;

                    return this.parent.SuspendedParent;
                }
                return (this.IsLayoutSuspended ? this : null);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the element is overriding the default
        ///     layout logic. Typically this property is set to true when the element is a
        ///     <see cref="Telerik.WinControls.Layouts.LayoutPanel">LayoutPanel</see>.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool OverridesDefaultLayout
        {
            get
            {
                return this.GetBitState(OverridesDefaultLayoutStateKey);
            }
            set
            {
                this.SetBitState(OverridesDefaultLayoutStateKey, value);
            }
        }

        // Should be absoleted
        /// <exclude />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool AffectsInnerLayout
        {
            get
            {
                return false;
            }
        }

        protected virtual Size AvailableSize
        {
            get
            {
                if (this.state != ElementState.Loaded)
                {
                    return Size.Empty;
                }

                return this.LayoutEngine.AvailableSize;
            }
        }

        internal Dictionary<RadElement, ElementLayoutData> SuspendedChildren
        {
            get
            {
                if (this.suspendedChildren == null)
                    this.suspendedChildren = new Dictionary<RadElement, ElementLayoutData>();
                return this.suspendedChildren;
            }
            set
            {
                this.suspendedChildren = value;
            }
        }

        /// <summary>
        ///		Detects whether the element is both automatically sized by the layout system and fits the parent element bounds.
        /// </summary>
        /// <returns></returns>
        public bool IsFitInSize()
        {
            return this.AutoSize && this.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize;
        }

        /// <summary>
        ///		Detects whether the element is with fixed size or automatically sized with its size conforming to the parent bounds.
        /// </summary>
        /// <returns></returns>
        [Obsolete("Please, use ShouldIgnoreChildSizes!")]//p.p. 06.07.09 changed to Obsolete
        [Browsable(false)]
        public bool ShouldIgrnoreChildSizes()
        {
            return !this.AutoSize || IsFitInSize();
        }

        /// <summary>
        ///		Detects whether the element is with fixed size or automatically sized with its size conforming to the parent bounds.
        /// </summary>
        /// <returns></returns>
        public bool ShouldIgnoreChildSizes()
        {
            return !this.AutoSize || IsFitInSize();
        }

        public void PerformLayout()
        {
            this.PerformLayout(this);
        }

        public void PerformLayout(RadElement affectedElement)
        {
            if (!this.CanExecuteLayoutOperation())
            {
                return;
            }

            if (this.GetBitState(UseNewLayoutStateKey))
            {
                this.InvalidateMeasure();
                this.InvalidateArrange();
            }
            else
            {
                this.LayoutEngine.PerformLayout(affectedElement, false);
            }
        }

        public virtual void PerformLayoutCore(RadElement affectedElement)
        {
            if (!this.CanExecuteLayoutOperation())
            {
                return;
            }

            this.LayoutEngine.PerformLayoutCore(affectedElement);
        }

        public Size GetPreferredSize(Size proposedSize)
        {
            if (this.state != ElementState.Loaded)
            {
                return proposedSize;
            }

            return this.LayoutEngine.GetPreferredSize(proposedSize);
        }

        public virtual Size GetPreferredSizeCore(Size proposedSize)
        {
            if (this.state != ElementState.Loaded)
            {
                return proposedSize;
            }

            return this.LayoutEngine.GetPreferredSizeCore(proposedSize);
        }

        /// <summary>
        /// Temporary suspends layout operations upon this element.
        /// </summary>
        public void SuspendLayout()
        {
            this.SuspendLayout(false);
        }

        /// <summary>
        /// Temporary suspends layout operations upon this element.
        /// </summary>
        /// <param name="recursive">True to suspend children also, false otherwise.</param>
        public virtual void SuspendLayout(bool recursive)
        {
            this.layoutSuspendCount++;

            if (!this.GetBitState(UseNewLayoutStateKey) && this.state == ElementState.Loaded)
            {
                this.LayoutEngine.RegisterLayoutRunning();
            }

            if (recursive)
            {
                foreach (RadElement child in this.children)
                {
                    child.SuspendLayout(recursive);
                }
            }
        }

        public void ResumeLayout(bool performLayout)
        {
            this.ResumeLayout(false, performLayout);
        }

        public virtual void ResumeLayout(bool recursive, bool performLayout)
        {
            if (this.layoutSuspendCount > 0)
            {
                this.layoutSuspendCount--;
            }

            if (this.state == ElementState.Loaded && this.layoutSuspendCount == 0)
            {
                this.InvalidateTransformations();
                if (performLayout)
                {
                    if (this.GetBitState(UseNewLayoutStateKey))
                    {
                        this.BitState[MeasureDirtyStateKey] = false;
                        this.InvalidateMeasure(false);
                        this.BitState[ArrangeDirtyStateKey] = false;
                        this.InvalidateArrange(false);
                    }
                    else
                    {
                        RoutedEventArgs args = new RoutedEventArgs(EventArgs.Empty, RootRadElement.RootLayoutResumedEvent);
                        RaiseTunnelEvent(this, args);

                        ILayoutEngine engine = this.LayoutEngine;
                        engine.PerformRegisteredSuspendedLayouts();
                        if (performLayout && this.GetBitState(IsLayoutPendingStateKey))
                        {
                            this.BitState[IsLayoutPendingStateKey] = false;
                            engine.PerformLayout(this, true);
                        }

                        if (this.GetBitState(IsPendingInvalidateStateKey))
                        {
                            this.Invalidate();
                            this.BitState[IsPendingInvalidateStateKey] = false;
                        }

                        engine.UnregisterLayoutRunning();
                    }
                }
            }

            if (recursive)
            {
                foreach (RadElement child in this.children)
                {
                    child.ResumeLayout(recursive, performLayout);
                }
            }
        }

        #endregion

        #region General

        protected virtual void LockBounds()
        {
            this.boundsLocked++;
        }

        protected virtual void UnlockBounds()
        {
            this.boundsLocked--;
        }

        /// <summary>
        /// Sets the bounds of the element to the specified rectangle (locating and
        /// size).
        /// </summary>
        public void SetBounds(Rectangle bounds)
        {
            SetBoundsCore(bounds);
        }

        /// <summary>
        /// Sets the bounds of the element to the specified rectangle (X, Y, width and
        /// height).
        /// </summary>
        public void SetBounds(int x, int y, int width, int height)
        {
            this.SetBounds(new Rectangle(x, y, width, height));
        }

        protected virtual void SetBoundsCore(Rectangle bounds)
        {
            if (this.boundsLocked != 0)
            {
                throw new InvalidOperationException("Bounds cannot be changed while locked.");
            }

            //do not allow negative size
            bounds.Width = Math.Max(0, bounds.Width);
            bounds.Height = Math.Max(0, bounds.Height);

            SetCachedBounds(bounds);
            this.SetValue(BoundsProperty, this.cachedBounds);
        }

        private void SetCachedBounds(Rectangle bounds)
        {
            this.cachedBounds = CheckBounds(bounds);
            if (bounds.Size != this.cachedSize)
            {
                InvalidateCachedSize();
            }
            InvalidateBoundsRectangles();
        }

        private void InvalidateBoundsRectangles()
        {
            InvalidateFaceRectangle();
            InvalidateFullRectangle();
            InvalidateBoundingRectangle();
        }

        /// <summary>
        /// Retrieves a point in screen coordinates taking as a parameter a point which is in
        /// element coordinates (this means that the top left corner of the element is with
        /// coordinates 0, 0).
        /// </summary>
        public Point PointToScreen(Point point)
        {
            if (!this.IsInValidState(true))
            {
                return Point.Empty;
            }

            Control ctl = this.elementTree.Control;
            Point pointInControl = PointToControl(point);
            Point res = ctl != null ? ctl.PointToScreen(pointInControl) : pointInControl;

            return res;
        }

        public Point PointFromScreen(Point point)
        {
            if (!this.IsInValidState(true))
            {
                return Point.Empty;
            }

            Point res = PointToScreen(new Point(0, 0));
            res.X = point.X - res.X;
            res.Y = point.Y - res.Y;
            return res;
        }

        /// <summary>
        /// Retrieves a rectangle in screen coordinates taking as a parameter a rectangle
        /// which is in element coordinates (this means that the top left corner of the element is
        /// with coordinates 0, 0).
        /// </summary>
        public Rectangle RectangleToScreen(Rectangle rect)
        {
            if (this.state != ElementState.Loaded)
            {
                return Rectangle.Empty;
            }

            rect = TelerikHelper.GetBoundingRect(rect, this.TotalTransform);
            return this.elementTree.Control.RectangleToScreen(rect);
        }

        internal void InvalidateFaceRectangle()
        {
            this.cachedFaceRectangle = Rectangle.Empty;
        }

        /// <summary>
        /// Determines whether the element is currently in valid state.
        /// That is having a valid RadElementTree reference and being in either Constructed or Loaded state.
        /// </summary>
        /// <returns></returns>
        protected internal bool IsInValidState(bool checkElementTree)
        {
            if (this.elementTree == null && checkElementTree)
            {
                return false;
            }

            return this.state == ElementState.Constructed || this.state == ElementState.Loaded || this.state == ElementState.Unloaded;
        }

        internal void InvalidateFullRectangle()
        {
            this.cachedFullRectangle = Rectangle.Empty;
        }

        public void InvalidateCachedSize()
        {
            this.cachedSize = LayoutUtils.InvalidSize;
        }

        internal void InvalidateChildrenFaceRectangle()
        {
            foreach (RadElement child in this.Children)
            {
                child.InvalidateFaceRectangle();
                if (child.FitToSizeMode == RadFitToSizeMode.FitToParentPadding)
                    child.InvalidateChildrenFaceRectangle();
            }
        }

        private void InvalidateBoundingRectangle()
        {
            this.cachedBoundingRectangle = Rectangle.Empty;
            this.cachedControlBoundingRectangle = Rectangle.Empty;
        }

        private void InvalidateBorderThickness()
        {
            this.InvalidateBoundsRectangles();
            this.InvalidateChildrenFaceRectangle();

            RadElement child;
            for (int i = 0; i < this.children.Count; i++)
            {
                child = this.children[i];
                child.InvalidateOwnTransformation();
                child.InvalidateTotalTransformationOnly(false);
            }
        }

        private void InvalidatePadding()
        {
            this.InvalidateBoundsRectangles();
            this.InvalidateChildrenFaceRectangle();

            RadElement child;
            for (int i = 0; i < this.children.Count; i++)
            {
                child = this.Children[i];
                child.InvalidateOwnTransformation();
                child.InvalidateTotalTransformationOnly(false);
            }
        }

        private Rectangle CheckBounds(Rectangle bounds)
        {
            if (this.state != ElementState.Loaded)
            {
                return bounds;
            }

            Size size = this.LayoutEngine.CheckSize(bounds.Size);
            bounds.Size = size;

            return bounds;
        }

        /// <summary>
        /// Gets the offset that is caused by scrolling. The difference between this method and
        /// PositionOffset property is that GetScrollingOffset() takes into account RightToLeft.
        /// </summary>
        /// <returns>The scrolling offset for this element.</returns>
        internal Point GetScrollingOffset()
        {
            Size offset = Size.Round(this.PositionOffset);
            if (this.RightToLeft)
                offset.Width = -offset.Width;
            return new Point(offset);
        }

        public bool HitTest(Point point)
        {
            if ((this.Size.Width == 0) || (this.Size.Height == 0))
                return false;

            Point offset = GetScrollingOffset();
            point.Offset(offset);

            if (this.Shape != null)
            {
                using (GraphicsPath path = this.Shape.CreatePath(new Rectangle(Point.Empty, this.Size)))
                {
                    bool hit = false;
                    using (Matrix m = this.TotalTransform.ToGdiMatrix())
                    {
                        path.Transform(m);
                        hit = path.IsVisible(point);
                    }
                    return hit;
                }
            }
            else if (this.TotalTransform.M12 != 0f)
            {
                Point[] points = new Point[4];
                points[0] = new Point(0, 0);
                points[1] = new Point(this.Bounds.Width, 0);
                points[2] = new Point(this.Bounds.Width, this.Bounds.Height);
                points[3] = new Point(0, this.Bounds.Height);

                TelerikHelper.TransformPoints(points, this.TotalTransform.Elements);

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddPolygon(points);
                    return path.IsVisible(point);
                }
            }
            else
                return ControlBoundingRectangle.Contains(point);
        }

        /// <summary>
        ///		Gets the rectangle which surronds the rotated element (if having AngleTransform property set).
        /// </summary>
        /// <param name="size">The size of the element which is accepted as a parameter (for example when returned from GetPreferredSize).</param>
        /// <returns></returns>
        public Rectangle GetBoundingRectangle(Size size)
        {
            return GetBoundingRectangle(new Rectangle(Point.Empty, size));
        }

        public Rectangle GetBoundingRectangle(Rectangle bounds)
        {
            if (this.AngleTransform == 0f && this.ScaleTransform.Width == 1f && this.ScaleTransform.Height == 1f)
                return bounds;

            Rectangle boundingRect = TelerikHelper.GetBoundingRect(
                new Rectangle(Point.Empty, bounds.Size), this.Transform);
            return new Rectangle(bounds.Location, boundingRect.Size);
        }

        protected virtual void OnLayoutPropertyChanged(RadPropertyChangedEventArgs e)
        {
            if (this.state != ElementState.Loaded)
            {
                return;
            }

            if (!this.IsLayoutSuspended)
            {
                if (this.GetBitState(UseNewLayoutStateKey))
                {
                    if (e.Property == RadElement.AngleTransformProperty || e.Property == RadElement.ScaleTransformProperty)
                    {
                        InvalidateTransformations(true);
                    }
                }
                else
                {
                    if (e.Property == RadElement.BoundsProperty ||
                        e.Property == RadElement.PositionOffsetProperty ||
                        e.Property == RadElement.MarginProperty ||
                        e.Property == RadElement.PaddingProperty ||
                        e.Property == RadElement.AngleTransformProperty ||
                        e.Property == RadElement.ScaleTransformProperty ||
                        e.Property == RadElement.AlignmentProperty)
                    {
                        InvalidateTransformations();
                    }
                }
            }

            if (!this.GetBitState(UseNewLayoutStateKey) && this.elementTree == null)
                return;

            RadElementPropertyMetadata metadata = e.Metadata as RadElementPropertyMetadata;
            if (metadata == null)
            {
                return;
            }

            if (e.Property == RadElement.BoundsProperty)
            {
                Rectangle oldBounds = (Rectangle)e.OldValue;
                Rectangle newBounds = (Rectangle)e.NewValue;

                this.SetCachedBounds(newBounds);

                bool locationChanged = newBounds.Location != oldBounds.Location;
                bool sizeChanged = newBounds.Size != oldBounds.Size;

                if (this.GetBitState(UseNewLayoutStateKey))
                {
                    if (!this.layoutManager.IsUpdating)
                    {
                        if (this.parent != null)
                        {
                            this.parent.InvalidateArrange();
                        }

                        if (sizeChanged)
                        {
                            this.InvalidateMeasure();
                            this.InvalidateArrange();
                        }
                    }

                    this.OnBoundsChanged(e);

                    if (locationChanged)
                    {
                        this.OnLocationChanged(e);
                    }
                }
                else
                {
                    this.LayoutEngine.LayoutPropertyChanged(e);

                    RoutedEventArgs args = new RoutedEventArgs(EventArgs.Empty, RadElement.BoundsChangedEvent);
                    this.RaiseTunnelEvent(this, args);
                    OnBoundsChanged(e);

                    // TODO: See why oldBounds.Location must be NULL - it affects the scrolling...
                    oldBounds.Location = Point.Empty;
                    PerformInvalidate(oldBounds);

                    if (locationChanged)
                        OnLocationChanged(e);
                }
            }
            else
            {
                if (this.GetBitState(UseNewLayoutStateKey))
                {
                    RadElement parentElement = this.parent != null ? this.parent : this;
                    if (metadata.AffectsParentMeasure)
                        parentElement.InvalidateMeasure();
                    if (metadata.AffectsParentArrange)
                        parentElement.InvalidateArrange();

                    if (metadata.AffectsMeasure)
                        this.InvalidateMeasure();
                    if (metadata.AffectsArrange || metadata.AffectsLayout)
                        this.InvalidateArrange();
                }
                else
                {
                    if (metadata.AffectsLayout || e.Property == RadElement.VisibilityProperty)
                    {
                        this.LayoutEngine.LayoutPropertyChanged(e);
                    }
                }
            }
        }

#if DEBUG
        [Browsable(true), Category(RadDesignCategory.TPFDebugCategory)]
#else
        [Browsable(false)]
#endif
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool UseNewLayoutSystem
        {
            get
            {
                return this.GetBitState(UseNewLayoutStateKey);
            }
            set
            {
                this.SetNewLayoutSystem(value);
            }
        }

        protected virtual void SetNewLayoutSystem(bool useNewLayoutSystem)
        {
            this.BitState[UseNewLayoutStateKey] = useNewLayoutSystem;
            //check whether we are on the element tree already
            if (this.parent != null)
            {
                for (int i = 0; i < this.children.Count; i++)
                {
                    RadElement child = this.children[i];
                    child.SetNewLayoutSystem(useNewLayoutSystem);
                }
            }
        }

        public Point PointToControl(Point point)
        {
            if (this.state != ElementState.Loaded)
            {
                return point;
            }

            Point[] points = new Point[] { new Point(point.X, point.Y) };
            TelerikHelper.TransformPoints(points, this.TotalTransform.Elements);
            return points[0];
        }

        public Point PointFromControl(Point point)
        {
            if (this.state != ElementState.Loaded)
            {
                return point;
            }

            Point[] points = new Point[] { new Point(0, 0) };
            TelerikHelper.TransformPoints(points, this.TotalTransform.Elements);

            points[0].X = point.X - points[0].X;
            points[0].Y = point.Y - points[0].Y;

            float angle = this.AngleTransform;
            for (RadElement parent = this.Parent; parent != null; parent = parent.Parent)
                angle += parent.AngleTransform;

            if (angle != 0f)
            {
                using (Matrix temp = new Matrix())
                {
                    temp.Rotate(-angle);
                    TelerikHelper.TransformPoints(points, temp.Elements);
                }
            }

            return points[0];
        }

        public Point LocationToControl()
        {
            return ControlBoundingRectangle.Location;
        }

        internal bool PropertyAffectsLayout(RadElementPropertyMetadata metadata)
        {
            return (metadata.AffectsLayout ||
                        metadata.AffectsMeasure ||
                        metadata.AffectsArrange ||
                        metadata.InvalidatesLayout);
        }

        #endregion

        #region CLR Properties

        /// <summary>
        /// Get or sets the maximum size to apply on an element when layout is
        /// calculated.
        /// </summary>
        [Description("Represents the maximum size of the element")]
        [RadPropertyDefaultValue("MaxSize", typeof(RadElement))]
        [Category(RadDesignCategory.LayoutCategory)]
        public virtual Size MaxSize
        {
            get
            {
                return (Size)this.GetValue(MaxSizeProperty);
            }
            set
            {
                this.SetValue(MaxSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the direction of flow of the elements and whether elements are aligned to support locales 
        /// using right-to-left fonts.
        /// </summary>
        [RadPropertyDefaultValue("RightToLeft", typeof(RadElement))]
        [Category(RadDesignCategory.AppearanceCategory)]
        [Localizable(true)]
        public virtual bool RightToLeft
        {
            get
            {
                return (bool)this.GetValue(RightToLeftProperty);
            }
            set
            {
                this.SetValue(RightToLeftProperty, value);
            }
        }

        /// <summary>
        /// Determines whether to use compatible text rendering engine (GDI+) or not (GDI). 
        /// </summary>
        [RadPropertyDefaultValue("UseCompatibleTextRendering", typeof(RadElement))]
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Determines whether to use compatible text rendering engine (GDI+) or not (GDI).")]
        public virtual bool UseCompatibleTextRendering
        {
            get
            {
                return (bool)this.GetValue(UseCompatibleTextRenderingProperty);
            }
            set
            {
                this.SetValue(UseCompatibleTextRenderingProperty, value);
            }
        }



        /// <summary>
        /// Gest of sets the order of painting an element compared to its sibling elements. Greater ZIndex means an element would be 
        /// painted on top of other elements amongst its sibligs. ZIndex changes the order of the elements in the list returned by
        /// <see cref="GetChildren"/>.
        /// </summary>
        [Description("Specifies the order of painting an element compared to its sibling elements")]
        [RadPropertyDefaultValue("ZIndex", typeof(RadElement)), Category(RadDesignCategory.BehaviorCategory)]
        public virtual int ZIndex
        {
            get
            {
                return (int)this.GetValue(ZIndexProperty);
            }
            set
            {
                this.SetValue(ZIndexProperty, value);
            }
        }

        /// <summary>
        ///		Gets the bounds of the element along with its margins.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Rectangle FullRectangle
        {
            get
            {
                if (this.cachedFullRectangle == Rectangle.Empty)
                {
                    this.cachedFullRectangle = new Rectangle(this.Location, Size.Add(this.Size, this.Margin.Size));
                }
                return this.cachedFullRectangle;
            }
        }

        /// <summary>
        ///		Represents the size of the element along with its margins.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Size FullSize
        {
            get
            {
                return FullRectangle.Size;
            }
        }

        /// <summary>
        ///		Gets the bounds of the element along with its parent paddings.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Rectangle FaceRectangle
        {
            get
            {
                if (this.state != ElementState.Loaded)
                {
                    return Rectangle.Empty;
                }

                if (this.cachedFaceRectangle == Rectangle.Empty)
                {
                    Padding padding = this.LayoutEngine.GetParentPadding();
                    this.cachedFaceRectangle = this.Bounds;
                    this.cachedFaceRectangle.Size = Size.Add(this.cachedFaceRectangle.Size, padding.Size);
                }
                return this.cachedFaceRectangle;
            }
        }

        /// <summary>
        ///		Gets the bounds of the element with its paddings excluded.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Rectangle FieldRectangle
        {
            get
            {
                if (this.state != ElementState.Loaded)
                {
                    return Rectangle.Empty;
                }

                Rectangle fieldRectanle = new Rectangle(this.Location, Size.Subtract(this.Size, this.Padding.Size));
                fieldRectanle.Size = Size.Subtract(fieldRectanle.Size, this.LayoutEngine.GetBorderSize());
                return fieldRectanle;
            }
        }

        /// <summary>
        ///		Gets the size of the element with its paddings excluded.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Size FieldSize
        {
            get
            {
                return this.FieldRectangle.Size;
            }
        }

        /// <summary>
        ///		Represents the visible part of the element bounds which is not clipped by its parent bounds.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Rectangle DisplayRectangle
        {
            get
            {
                if (this.Parent == null)
                    return this.BoundingRectangle;

                int horizontalOutsidePixels = Math.Min(0, this.BoundingRectangle.Right - this.Parent.FieldSize.Width);
                int verticalOutsidePixels = Math.Min(0, this.BoundingRectangle.Bottom - this.Parent.FieldSize.Height);

                Size displaySize = Size.Subtract(this.BoundingRectangle.Size, new Size(horizontalOutsidePixels, verticalOutsidePixels));
                return new Rectangle(this.BoundingRectangle.Location, displaySize);
            }
        }

        /// <summary>
        ///		Represents the rectangle which surrounds the element bounds after the rotation caused by setting the AngleTransform property to some degree. The rectangle is in parent element's coordinates.
        /// </summary>
#if DEBUG
        [Browsable(true), Category(RadDesignCategory.TPFDebugCategory)]
#else
        [Browsable(false)]
#endif
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Rectangle BoundingRectangle
        {
            get
            {
                if (this.cachedBoundingRectangle == Rectangle.Empty)
                {
                    if (this.UseNewLayoutSystem)
                    {
                        PointF offset = new PointF(this.Transform.DX, this.Transform.DY);
                        Rectangle untransformedBounds = new Rectangle(Point.Round(offset), this.Size);
                        this.cachedBoundingRectangle = GetBoundingRectangle(untransformedBounds);
                    }
                    else
                    {
                        this.cachedBoundingRectangle = GetBoundingRectangle(this.Bounds);
                    }
                }

                return this.cachedBoundingRectangle;
            }
        }

        /// <summary>
        ///		Represents the rectangle which surrounds the element bounds after the rotation caused by setting the AngleTransform property to some degree. The rectangle is in control coordinates.
        /// </summary>
#if DEBUG
        [Browsable(true), Category(RadDesignCategory.TPFDebugCategory)]
#else
        [Browsable(false)]
#endif
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Rectangle ControlBoundingRectangle
        {
            get
            {
                if (this.cachedControlBoundingRectangle == Rectangle.Empty)
                {
                    this.cachedControlBoundingRectangle = TelerikHelper.GetBoundingRect(new Rectangle(Point.Empty, this.Size), this.TotalTransform);
                }

                return this.cachedControlBoundingRectangle;
            }
        }

        /// <summary>
        ///		Gets the bounding rotated rectangle along with its margins.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Rectangle FullBoundingRectangle
        {
            get
            {
                return new Rectangle(this.BoundingRectangle.Location, Size.Add(this.BoundingRectangle.Size, this.Margin.Size));
            }
        }

        #endregion

        #endregion

        #region Painting

        public void Invalidate()
        {
            if (this.state != ElementState.Loaded)
            {
                return;
            }

            if (this.GetBitState(UseNewLayoutStateKey))
            {
                if (this.elementTree != null)
                {
                    this.elementTree.ComponentTreeHandler.InvalidateElement(this);
                }
            }
            else
            {
                this.Invalidate(false);
            }
        }

        public void Invalidate(bool checkSuspended)
        {
            if (checkSuspended && IsLayoutSuspended)
            {
                this.BitState[IsPendingInvalidateStateKey] = true;
                return;
            }

            PerformInvalidate();
        }

        private void PerformInvalidate()
        {
            this.PerformInvalidate(Rectangle.Empty);
        }

        private void PerformInvalidate(Rectangle bounds)
        {
            if (this.state != ElementState.Loaded)
            {
                return;
            }

            Debug.Assert(this.elementTree != null, "Must have an ElementTree reference when loaded.");

            if (bounds != Rectangle.Empty)
            {
                this.elementTree.ComponentTreeHandler.InvalidateElement(this, bounds);
            }
            else
            {
                this.elementTree.ComponentTreeHandler.InvalidateElement(this);
            }
        }

        protected virtual void NotifyInvalidate(RadElement invalidatedChild)
        {
        }

#if DEBUG
        [Browsable(true), Category(RadDesignCategory.TPFDebugCategory)]
#else
        [Browsable(false)]
#endif
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public RadMatrix Transform
        {
            get
            {
                if (this.transform.IsEmpty)
                {
                    this.transform = RadMatrix.Identity;
                    this.PerformTransformation(ref this.transform);
                }
                return this.transform;
            }
        }

#if DEBUG
        [Browsable(true), Category(RadDesignCategory.TPFDebugCategory)]
#else
        [Browsable(false)]
#endif
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public RadMatrix TotalTransform
        {
            get
            {
                if (this.totalTransform.IsEmpty)
                {
                    this.totalTransform = this.Transform;
                    if (this.parent != null)
                    {
                        //Matrix.Mult
                        this.totalTransform.Multiply(this.parent.TotalTransform, MatrixOrder.Append);
                    }
                }
                return this.totalTransform;
            }
        }

        private void InvalidateTotalTransformationReqursively()
        {
            InvalidateTotalTransformation();

            RadElementCollection children = this.Children;
            for (int i = 0; i < children.Count; i++)
            {
                RadElement child = children[i];
                child.InvalidateTotalTransformationReqursively();
            }
        }

        private void InvalidateTotalTransformationOnly(bool saveOldMatrix)
        {
            this.totalTransform = RadMatrix.Empty;
            this.InvalidateBoundingRectangle();

            if (!this.GetBitState(UseNewLayoutStateKey))
            {
                OnTransformationInvalidated();
            }
        }

        private void InvalidateTotalTransformation()
        {
            this.InvalidateTotalTransformationOnly(true);
            this.totalTransform = RadMatrix.Empty;

            if (!this.GetBitState(UseNewLayoutStateKey))
            {
                OnTransformationInvalidated();
            }
        }

        /// <summary>
        /// This method is executed when a property which affects the absolute position of the element has been changed.
        /// </summary>
        protected virtual void OnTransformationInvalidated()
        {
        }

        private void CallOnTransformationInvalidatedRecursively()
        {
            this.OnTransformationInvalidated();
            for (int i = 0; i < this.Children.Count; i++)
            {
                RadElement child = this.Children[i];
                child.CallOnTransformationInvalidatedRecursively();
            }
        }

        internal void InvalidateLayoutTransforms()
        {
            this.InvalidateOwnTransformation();
            this.InvalidateTotalTransformation();
        }

        private void InvalidateOwnTransformation()
        {
            this.InvalidateBoundingRectangle();
            this.transform = RadMatrix.Empty;
        }

        private void InvalidateTransformations(bool invalidateElement)
        {
            if (!this.IsInValidState(true))
            {
                return;
            }

            if (this.GetBitState(UseNewLayoutStateKey) && invalidateElement)
            {
                if (TraceInvalidation)
                    Console.WriteLine("InvalidateTransformations: {0}; ElementBounds:{1}", this.GetType().Name, this.Bounds);
                this.Invalidate();
            }
            InvalidateOwnTransformation();
            // Invalidate in deep the total transformartion matrix of each child
            InvalidateTotalTransformationReqursively();
        }

        internal void InvalidateTransformations()
        {
            InvalidateTransformations(true);
        }

        protected virtual void PaintElement(IGraphics graphics, float angle, SizeF scale)
        {
        }

        protected virtual bool PerformLayoutTransformation(ref RadMatrix matrix)
        {
            if (this.state != ElementState.Loaded)
            {
                return false;
            }

            PointF transformationPoint = this.GetBitState(UseNewLayoutStateKey) ?
                this.layoutOffset :
                this.LayoutEngine.GetTransformationPoint();
            matrix.Translate(transformationPoint.X, transformationPoint.Y, MatrixOrder.Append);
            return !transformationPoint.IsEmpty;
        }

        protected virtual bool PerformPaintTransformation(ref RadMatrix matrix)
        {
            bool res = false;
            float angleTransform = this.AngleTransform;

            if (angleTransform != 0f)
            {
                res = true;
                if (this.GetBitState(UseNewLayoutStateKey))
                {
                    RectangleF bounds = new RectangleF(PointF.Empty, this.Bounds.Size);
                    TelerikHelper.PerformTopLeftRotation(ref matrix, bounds, angleTransform);
                }
                else
                {
                    Rectangle bounds = new Rectangle(Point.Empty, this.Bounds.Size);
                    TelerikHelper.PerformTopLeftRotation(ref matrix, bounds, angleTransform);
                }
            }

            SizeF scaleTransform = this.ScaleTransform;
            if ((scaleTransform.Width > 0) && (scaleTransform.Height > 0))
            {
                if ((scaleTransform.Width != 1) || (scaleTransform.Height != 1))
                {
                    res = true;
                    matrix.Scale(scaleTransform.Width, scaleTransform.Height, MatrixOrder.Append);
                }
            }

            SizeF posOffset = this.PositionOffset;
            if (posOffset != SizeF.Empty)
            {
                res = true;
                matrix.Translate(
                    this.RightToLeft ? -posOffset.Width : posOffset.Width,
                    posOffset.Height, MatrixOrder.Append);
            }

            return res;
        }

        private bool PerformTransformation(ref RadMatrix matrix)
        {
            // Ensure both functions are called...
            bool hasPaintTransform = this.PerformPaintTransformation(ref matrix);
            bool hasLayoutTransform = this.PerformLayoutTransformation(ref matrix);

            return hasLayoutTransform || hasPaintTransform;
        }

        protected bool IsInGetAsBitmap()
        {
            RadElement element = this;
            while (element != null)
            {
                if (element.GetBitState(UseIdentityMatrixStateKey))
                    return true;
                element = element.Parent;
            }
            return false;
        }

        internal bool IsThisTheTopDisabledItem
        {
            get
            {
                return (!this.Enabled && (this.Parent == null || this.Parent.Enabled));
            }
        }

        public Bitmap GetAsBitmap(IGraphics screenRadGraphics, Brush backgroundBrush, float totalAngle, SizeF totalScale)
        {
            if (this.state != ElementState.Loaded)
            {
                return null;
            }

            Size size = this.Size;
            if (size.Width <= 0 || size.Height <= 0)
            {
                return null;
            }

            Graphics screenGraphics = (Graphics)screenRadGraphics.UnderlayGraphics;

            Bitmap memoryBitmap = new Bitmap(size.Width, size.Height);

            this.BitState[UseIdentityMatrixStateKey] = true;
            Rectangle clipingRectangle = new Rectangle(Point.Empty, size);
            clipingRectangle.Size = this.elementTree.Control.Size;
            this.Paint(screenRadGraphics, clipingRectangle, totalAngle, totalScale, true);
            this.BitState[UseIdentityMatrixStateKey] = false;

            TelerikPaintHelper.CopyImageFromGraphics(screenGraphics, memoryBitmap);

            return memoryBitmap;
        }

        public Bitmap GetAsBitmap(Brush backgroundBrush, float totalAngle, SizeF totalScale)
        {
            if (this.state != ElementState.Loaded)
            {
                return null;
            }

            Size size = this.Size;

            if (size.Width > 0 && size.Height > 0)
            {
                Bitmap memoryBitmap = new Bitmap(size.Width, size.Height);
                Graphics memoryGraphics = Graphics.FromImage(memoryBitmap);
                RadGdiGraphics memoryRadGraphics = new RadGdiGraphics(memoryGraphics);

                memoryGraphics.FillRectangle(backgroundBrush, new Rectangle(Point.Empty, size));

                this.BitState[UseIdentityMatrixStateKey] = true;
                Rectangle clipingRectangle = new Rectangle(Point.Empty, size);
                clipingRectangle.Size = this.elementTree.Control.Size;
                this.Paint(memoryRadGraphics, clipingRectangle, totalAngle, totalScale, true);
                this.BitState[UseIdentityMatrixStateKey] = false;

                //memoryGraphics.Dispose();
                memoryRadGraphics.Dispose();

                return memoryBitmap;
            }

            return null;
        }

        public Bitmap GetAsBitmapEx(Color backColor, float totalAngle, SizeF totalScale)
        {
            if (this.state != ElementState.Loaded)
            {
                return null;
            }

            Size size = this.Size;

            if (size.Width > 0 && size.Height > 0)
            {
                Bitmap memoryBitmap = new Bitmap(size.Width, size.Height);
                Graphics memoryGraphics = Graphics.FromImage(memoryBitmap);
                memoryGraphics.Clear(backColor);
                RadGdiGraphics memoryRadGraphics = new RadGdiGraphics(memoryGraphics);
                this.BitState[UseIdentityMatrixStateKey] = true;
                Rectangle clipingRectangle = new Rectangle(Point.Empty, size);
                Point position = this.LocationToControl();
                clipingRectangle.Location = position;
                this.Paint(memoryRadGraphics, clipingRectangle, totalAngle, totalScale, true);
                this.BitState[UseIdentityMatrixStateKey] = false;

                memoryRadGraphics.Dispose();

                return memoryBitmap;
            }

            return null;
        }

        public Bitmap GetAsBitmapEx(Brush backgroundBrush, float totalAngle, SizeF totalScale)
        {
            return this.GetAsBitmapEx(Color.Empty, totalAngle, totalScale);
        }

        public Bitmap GetAsTransformedBitmap(IGraphics screenRadGraphics, Brush backgroundBrush, float totalAngle, SizeF totalScale)
        {
            if (this.state != ElementState.Loaded)
            {
                return null;
            }

            // Fill the Control bitmap
            Size size = this.ElementTree.Control.Size;

            if (size.Width > 0 && size.Height > 0)
            {
                Graphics screenGraphics = (Graphics)screenRadGraphics.UnderlayGraphics;

                Bitmap controlBitmap = new Bitmap(size.Width, size.Height);
                Rectangle clipingRectangle = new Rectangle(Point.Empty, this.ElementTree.Control.Size);
                this.Paint(screenRadGraphics, clipingRectangle, totalAngle, totalScale, false);

                TelerikPaintHelper.CopyImageFromGraphics(screenGraphics, controlBitmap);

                return controlBitmap;
            }

            return null;
        }

        public Bitmap GetAsTransformedBitmap(Brush backgroundBrush, float totalAngle, SizeF totalScale)
        {
            Rectangle clippingRectangle = new Rectangle(Point.Empty, this.ElementTree.Control.Size);
            return this.GetAsTransformedBitmap(clippingRectangle, backgroundBrush, totalAngle, totalScale);
        }

        public Bitmap GetAsTransformedBitmap(Rectangle clippingRectangle, Brush backgroundBrush, float totalAngle, SizeF totalScale)
        {
            if (this.state != ElementState.Loaded)
            {
                return null;
            }

            // Fill the Control bitmap
            Rectangle boundingRect = this.ControlBoundingRectangle;
            Rectangle myRect = boundingRect;
            //determine the minimum rect to paint in
            myRect.Intersect(clippingRectangle);
            Size size = myRect.Size;//this.ElementTree.Control.Size;

            if (size.Width > 0 && size.Height > 0)
            {
                Bitmap controlBitmap = new Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Graphics controlGraphics = Graphics.FromImage(controlBitmap);
                RadGdiGraphics controlRadGraphics = new RadGdiGraphics(controlGraphics);

                //Paint expects graphics with parent coordenate system, therefore set offset
                Rectangle parentRect;
                if (this.Parent != null)
                {
                    parentRect = this.Parent.ControlBoundingRectangle;
                }
                else
                {
                    parentRect = this.ControlBoundingRectangle;
                }

                int leftOffset = parentRect.Left - boundingRect.Left;
                int topOffset = parentRect.Top - boundingRect.Top;

                //the clipping changes the initial coordinates of the bitmap
                if (boundingRect.Left < myRect.Left)
                {
                    leftOffset -= (myRect.Left - boundingRect.Left);
                }

                if (boundingRect.Top < myRect.Top)
                {
                    topOffset -= (myRect.Top - boundingRect.Top);
                }

                controlRadGraphics.TranslateTransform(leftOffset, topOffset);

                //Clear should be more relevent than fillrectangle?
                //controlGraphics.FillRectangle(backgroundBrush, new Rectangle(Point.Empty, boundingRect.Size));
                SolidBrush solidBack = (backgroundBrush as SolidBrush);
                if (solidBack != null)
                {
                    controlGraphics.Clear(solidBack.Color);
                }
                else
                {
                    controlGraphics.FillRectangle(backgroundBrush, new Rectangle(Point.Empty, boundingRect.Size));
                }

                this.Paint(controlRadGraphics, clippingRectangle, totalAngle, totalScale, true);


                controlRadGraphics.Dispose();

                return controlBitmap;
            }

            return null;
        }

        /// <summary>
        /// Virtual layer between PaintChildren() and Paint().
        /// Can be overriden to fully customize element hierarchy paint.
        /// Used for painting disabled items.
        /// </summary>
        /// <param name="graphics">The graphics object.</param>
        /// <param name="clipRectangle">The rectangle which has been invalidated.</param>
        /// <param name="angle">The angle (in degrees) to which the current element is rotated. This angle is a sum of all AngleTransform properties of this element's parents.</param>
        /// <param name="scale"></param>
        /// <param name="useRelativeTransformation"></param>
        protected virtual void PaintOverride(IGraphics graphics, Rectangle clipRectangle, float angle, SizeF scale, bool useRelativeTransformation)
        {
            scale.Width = scale.Width * this.ScaleTransform.Width;
            scale.Height = scale.Height * this.ScaleTransform.Height;

            this.Paint(graphics, clipRectangle, angle + this.AngleTransform, scale, useRelativeTransformation);
        }

        internal void Paint(IGraphics graphics, Rectangle clipRectangle, float angle, SizeF scale, bool useRelativeTransformation)
        {
            //validate conditions to continue with paint process
            if (this.Visibility != ElementVisibility.Visible)
            {
                return;
            }
            if (!this.IsInVisibleClipBounds(clipRectangle))
            {
                return;
            }

            Graphics rawGraphics = (Graphics)graphics.UnderlayGraphics;
            //Lock bounds to warn when bounds are changed while painting - this should not happen
            this.LockBounds();

            //save current graphics state
            object state = graphics.SaveState();

            this.TranformGraphics(rawGraphics, useRelativeTransformation);
            this.DoOwnPaint(graphics, angle, scale);

            if (this.ClipDrawing)
            {
                this.SetClipping(rawGraphics);
            }

            this.PaintChildren(graphics, clipRectangle, angle, scale, useRelativeTransformation);
            this.PostPaintChildren(graphics, clipRectangle, angle, scale);

            if (state != null)
            {
                graphics.RestoreState(state);
            }

            this.UnlockBounds();

            if (this.ElementPainted != null)
            {
                this.ElementPainted(this, new PaintEventArgs(rawGraphics, graphics.ClipRectangle));
            }
        }

        private bool IsInVisibleClipBounds(Rectangle clipRectangle)
        {
            bool useIdentityMatrix = this.GetBitState(UseIdentityMatrixStateKey);
            if (!useIdentityMatrix)
            {
                Rectangle newClip = clipRectangle;
                Point offset = this.GetScrollingOffset();
                newClip.Offset(offset);
                Rectangle intersectRect = this.ControlBoundingRectangle;
                if (!intersectRect.IntersectsWith(newClip))
                {
                    return false;
                }
            }

            return true;
        }

        private void DoOwnPaint(IGraphics graphics, float angle, SizeF scale)
        {
            if (!this.ShouldPaint)
            {
                return;
            }

            bool firstTimeSkinPaint = false;
            if (this.paintSystemSkin == null)
            {
                this.paintSystemSkin = this.ShouldPaintSystemSkin();
                firstTimeSkinPaint = true;
            }

            //we are in system skin paint mode
            if (this.paintSystemSkin == true && this.PrepareSystemSkin())
            {
                //initialize system drawing
                if (firstTimeSkinPaint)
                {
                    this.InitializeSystemSkinPaint();
                }
                //paint the system skin
                this.PaintElementSkin(graphics);
            }
            //we are in default TPF render mode
            else
            {
                this.PrePaintElement(graphics);
                this.PaintElement(graphics, angle, scale);
                this.PostPaintElement(graphics);
            }
        }

        private Matrix TranformGraphics(Graphics rawGraphics, bool relativeTransform)
        {
            Matrix currentTranform = rawGraphics.Transform;
            RadMatrix transform = this.GetPaintTransform(currentTranform, relativeTransform);

            if (relativeTransform)
            {
                if (this.GetBitState(UseIdentityMatrixStateKey))
                {
                    rawGraphics.ResetTransform();
                }
                else
                {
                    rawGraphics.Transform = transform.ToGdiMatrix();
                }
            }
            else
            {
                currentTranform = transform.ToGdiMatrix();
                rawGraphics.Transform = currentTranform;
            }

            return currentTranform;
        }

        internal virtual RadMatrix GetPaintTransform(Matrix currentTransform, bool relative)
        {
            RadMatrix result;
            if (relative)
            {
                result = new RadMatrix(currentTransform);
                result.Multiply(this.Transform, MatrixOrder.Prepend);
            }
            else
            {
                result = this.TotalTransform;
            }

            return result;
        }

        //Todo optimize events
        internal event PaintEventHandler ElementPainted;

        protected virtual void PostPaintElement(IGraphics graphics)
        {
        }

        protected virtual void PrePaintElement(IGraphics graphics)
        {
            RadImageShape background = this.BackgroundShape;
            if (background != null)
            {
                background.Paint(graphics.UnderlayGraphics as Graphics, new RectangleF(PointF.Empty, this.Size));
            }
        }

        protected virtual void SetClipping(Graphics rawGraphics)
        {
            RectangleF clipRect = this.GetClipRect();
            rawGraphics.SetClip(clipRect, CombineMode.Intersect);
        }

        protected virtual RectangleF GetClipRect()
        {
            return this.Bounds;
        }

#if DEBUG_PAINT
        private void DebugPaint(IGraphics graphics)
        {
            Color color = Color.Blue;
            if (this is LayoutPanel)
            {
                color = Color.FromArgb(123, Color.Red);
            }

            if (this is IPrimitive)
            {
                color = Color.FromArgb( 123, Color.Yellow);
            }

            graphics.DrawRectangle(new Rectangle(
                0, 0, this.Bounds.Size.Width - 1, this.Bounds.Size.Height - 1), color);
        }
#endif

        protected virtual void PostPaintChildren(IGraphics graphics, Rectangle clipRectange, float angle, SizeF scale)
        {
            if (this.IsFocused && this.elementTree != null)
            {
                if (this.elementTree.ComponentTreeHandler.Behavior.ShouldShowFocusCues)
                {
                    this.PaintFocusCues(graphics, clipRectange);
                }
            }

#if DEBUG_PAINT
			if (this.IsMouseOverElement)
			{
			    graphics.DrawRectangle(new Rectangle(0, 0, this.Size.Width - 1, this.Size.Height - 1), Color.Red);
			}
			else if (this.IsMouseOver)
			{
			    graphics.DrawRectangle(new Rectangle(0, 0, this.Size.Width - 1, this.Size.Height - 1), Color.Blue);
			}
#endif
        }

        protected virtual void PaintChildren(IGraphics graphics, Rectangle clipRectange, float angle, SizeF scale, bool useRelativeTransformation)
        {
            if (!this.GetBitState(ShouldPaintChildrenStateKey))
            {
                return;
            }

            foreach (RadElement child in this.GetChildren(ChildrenListOptions.ZOrdered))
            {
                if (ShouldPaintChild(child))
                {
                    this.PaintChild(child, graphics, clipRectange, angle, scale, useRelativeTransformation);
                }
            }
        }

        protected virtual void PaintChild(RadElement child, IGraphics graphics, Rectangle clipRectange, float angle, SizeF scale, bool useRelativeTransformation)
        {
            child.PaintOverride(graphics, clipRectange, angle, scale, useRelativeTransformation);
        }

        protected virtual bool ShouldPaintChild(RadElement element)
        {
            return true;
        }

        protected virtual void PaintFocusCues(IGraphics graphics, Rectangle clipRectange)
        {
            Rectangle focusRect = GetFocusRect();
            if (focusRect.Width <= 0 || focusRect.Height <= 0)
            {
                return;
            }

            Graphics real = (Graphics)graphics.UnderlayGraphics;
            ControlPaint.DrawFocusRectangle(real, focusRect);
        }

        protected virtual Rectangle GetFocusRect()
        {
            Size thisSize = this.FaceRectangle.Size;

            if (thisSize.Width < 4 || thisSize.Height < 4)
                return Rectangle.Empty;

            return new Rectangle(new Point(2, 2), new Size(thisSize.Width - 4, thisSize.Height - 4));
        }

        protected ElementShape GetCurrentShape()
        {
            ElementShape currentShape = this.Shape;
            if (currentShape == null && this.ShouldPaintUsingParentShape)
            {
                RadElement parentElement = this.Parent;
                if (parentElement != null)
                {
                    currentShape = parentElement.Shape;
                }
            }

            return currentShape;
        }

        protected virtual bool ShouldPaintUsingParentShape
        {
            get
            {
                return false;
            }
        }

        protected RectangleF GetPaintRectangle(float borderWidth, float angle, SizeF scale)
        {
            Size size = this.Size;

            float halfBorderWidth = 0;

            if (borderWidth != 0)
            {
                halfBorderWidth = (float)Math.Floor((double)borderWidth / 2);
                borderWidth = 2 * halfBorderWidth + 1;
            }

            RectangleF paintRectangle = new RectangleF(halfBorderWidth, halfBorderWidth,
                    size.Width - borderWidth,
                    size.Height - borderWidth);

            paintRectangle = this.GetPatchedRect(paintRectangle, angle, scale);

            return paintRectangle;
        }

        protected RectangleF GetPatchedRect(RectangleF rect, float angle, SizeF scale)
        {
            //return new Rectangle(new Point(0, 0), Size.Add(size, new Size(1, 1)));

            SizeF finalSize = rect.Size;
            RectangleF res = new RectangleF(new PointF(0, 0), finalSize);

            int k = (int)(angle / 360);
            angle -= k * 360f;

            k = (int)(angle / 90);
            if (scale.Width < 2f && scale.Height < 2f)
            {
                if (Math.Abs(angle - k * 90f) > 1e-2)
                    return new RectangleF(rect.Left + 1, rect.Top + 1, finalSize.Width - 1, finalSize.Height - 1);


                if (angle < -180f)
                    angle += 360;
                if (angle > 180f)
                    angle -= 360;


                if (angle > -181f && angle < -135f) // [-180; -135]
                {
                    res.Location = new Point(1, 1);
                }
                else if (angle < -45f)              // [-135; -45]
                {
                    res.Location = new Point(1, 0);
                }
                else if (angle < 45f)               // [-45; 45]
                {
                    res.Location = new Point(0, 0);
                }
                else if (angle < 135f)              // [45; 135]
                {
                    res.Location = new Point(0, 1);
                }
                else if (angle < 181f)              // [135; 180]
                {
                    res.Location = new Point(1, 1);
                }
            }
            else
                res.Size = rect.Size;

            return new RectangleF(rect.Left + res.Left, rect.Top + res.Top, finalSize.Width, finalSize.Height);
        }

        #endregion

        #region ISupportSystemSkin

        /// <summary>
        /// Gets the VisualStyleElement instance that describes the skin appearance for the element when the current OS is Windows XP.
        /// </summary>
        /// <returns></returns>
        public virtual System.Windows.Forms.VisualStyles.VisualStyleElement GetXPVisualStyle()
        {
            return null;
        }

        /// <summary>
        /// Gets the VisualStyleElement instance that describes the skin appearance for the element when the current OS is Windows Vista.
        /// </summary>
        /// <returns></returns>
        public virtual System.Windows.Forms.VisualStyles.VisualStyleElement GetVistaVisualStyle()
        {
            return null;
        }

        /// <summary>
        /// Gets or sets the mode that describes the usage of system skinning (if available).
        /// </summary>
        [DefaultValue(UseSystemSkinMode.Inherit)]
        [Description("Gets or sets the mode that describes the usage of system skinning (if available)." +
            "WARNING: This feature is not yet implemented and it will not work as expected.")]
        public virtual UseSystemSkinMode UseSystemSkin
        {
            get
            {
                return this.useSystemSkin;
            }
            set
            {
                if (this.useSystemSkin == value)
                {
                    return;
                }

                this.useSystemSkin = value;
                OnUseSystemSkinChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Performs initialization when the element is first-time painted using system skin.
        /// </summary>
        protected virtual void InitializeSystemSkinPaint()
        {
        }

        protected virtual void UnitializeSystemSkinPaint()
        {

        }

        /// <summary>
        /// Provides a routine to paint element's content when system skin appearance is desired.
        /// </summary>
        /// <param name="graphics"></param>
        protected virtual void PaintElementSkin(IGraphics graphics)
        {
            Graphics gdiGraphics = (Graphics)graphics.UnderlayGraphics;
            SystemSkinManager.Instance.PaintCurrentElement(gdiGraphics, this.GetSystemSkinPaintBounds());
        }

        /// <summary>
        /// Gets the rectangle where skin background should be painted.
        /// Defaults to BoundingRectangle.
        /// </summary>
        /// <returns></returns>
        protected virtual Rectangle GetSystemSkinPaintBounds()
        {
            return this.cachedBounds;
        }

        /// <summary>
        /// The element gets notified for a change in the <see cref="RadElement.UseSystemSkin">UseSystemSkin</see> property.
        /// This method will recursively notify all descendants for the change.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnUseSystemSkinChanged(EventArgs e)
        {
            if (this.paintSystemSkin == true)
            {
                this.UnitializeSystemSkinPaint();
            }

            this.paintSystemSkin = null;
            foreach (RadElement element in this.children)
            {
                element.OnUseSystemSkinChanged(e);
            }

            this.Invalidate(true);
        }

        /// <summary>
        /// Determines whether we should paint system skin.
        /// </summary>
        /// <returns></returns>
        protected virtual bool ShouldPaintSystemSkin()
        {
            //check whether skinning is enabled at Application level
            if (!Application.RenderWithVisualStyles)
            {
                return false;
            }

            bool shouldPaintSkin = false;
            if (this.useSystemSkin != UseSystemSkinMode.Inherit)
            {
                shouldPaintSkin = this.useSystemSkin == UseSystemSkinMode.YesLocal ||
                                  this.useSystemSkin == UseSystemSkinMode.YesInheritable;
            }
            else
            {
                shouldPaintSkin = ComposeShouldPaintSystemSkin();
            }

            return shouldPaintSkin;
        }

        /// <summary>
        /// Composes a value which determines whether the element should use system skins when painting.
        /// This method will traverse the element and control tree and will end with the global <see cref="SystemSkinManager.UseSystemSkin">UseSystemSkin</see> property.
        /// </summary>
        /// <returns></returns>
        protected virtual bool ComposeShouldPaintSystemSkin()
        {
            //traverse element tree first
            RadElement parentElement = this.parent;
            while (parentElement != null)
            {
                switch (parentElement.useSystemSkin)
                {
                    case UseSystemSkinMode.NoInheritable:
                        return false;
                    case UseSystemSkinMode.YesInheritable:
                        return true;
                }
                parentElement = parentElement.Parent;
            }

            Control surface = this.ElementTree.Control;
            //compose value using parent chain
            if (surface != null)
            {
                Control parent = surface.Parent;
                while (parent != null)
                {
                    ISupportSystemSkin skinnableParent = parent as ISupportSystemSkin;
                    if (skinnableParent != null)
                    {
                        switch (skinnableParent.UseSystemSkin)
                        {
                            case UseSystemSkinMode.NoInheritable:
                                return false;
                            case UseSystemSkinMode.YesInheritable:
                                return true;
                        }
                    }

                    parent = parent.Parent;
                }
            }

            //we have traversed parent chain, all are Inherit, use SystemSkinManager's value
            return SystemSkinManager.Instance.UseSystemSkin;
        }

        private bool PrepareSystemSkin()
        {
            //prepare the SystemSkinManager for painting
            System.Windows.Forms.VisualStyles.VisualStyleElement skinElement;
            if (SystemSkinManager.IsVistaOrLater)
            {
                skinElement = this.GetVistaVisualStyle();
            }
            else
            {
                skinElement = this.GetXPVisualStyle();
            }

            if (skinElement == null || skinElement == SystemSkinManager.EmptyElement)
            {
                return false;
            }

            return SystemSkinManager.Instance.SetCurrentElement(skinElement);
        }

        #endregion

        #region Theming & Styling

        /// <summary>
        /// Maps a style property to another property. This method is used
        /// to map corresponding properties of LightVisualElement
        /// instances and <see cref="BasePrimitive"/> instances.
        /// </summary>
        /// <param name="propertyToMap">An instance of the <see cref="RadProperty"/>
        /// class that represents the property to map.</param>
        /// <param name="settingType"></param>
        /// <returns>An instance of the <see cref="RadProperty"/>
        /// class which represents the mapped property. If no property is found,
        /// the method returns null</returns>
        internal virtual RadProperty MapStyleProperty(RadProperty propertyToMap, string settingType)
        {
            return null;
        }

        /// <summary>
        /// Determines whether the element may be added associated with metadata in the Visual Style Builder.
        /// </summary>
        internal virtual bool VsbVisible
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the IFilter instance that may be used to filter the properties, treated as Stylable for this element.
        /// </summary>
        /// <returns></returns>
        public virtual Filter GetStylablePropertiesFilter()
        {
            return Telerik.WinControls.PropertyFilter.ExcludeFilter;
        }

        /// <summary>
        /// Resets the Style modifier of each registered property.
        /// </summary>
        public void ResetStyleSettings(bool recursive)
        {
            this.ResetStyleSettings(recursive, null);
        }

        /// <summary>
        /// Resets the Style modifier for the specified property. Will reset all properties if null is passed.
        /// </summary>
        public virtual void ResetStyleSettings(bool recursive, RadProperty property)
        {
            if (!this.IsInValidState(false))
            {
                return;
            }

            if (property != null)
            {
                this.ResetValue(property, ValueResetFlags.Style);
            }
            else
            {
                this.PropertyValues.ResetStyleProperties();
            }

            if (recursive)
            {
                foreach (RadElement child in this.children)
                {
                    child.ResetStyleSettings(recursive);
                }
            }
        }

        protected virtual void ProcessBehaviors(RadPropertyChangedEventArgs e)
        {
            for (int i = 0; i < this.behaviors.Count; i++)
            {
                PropertyChangeBehavior behavior = this.behaviors[i];

                if (behavior.Property == e.Property)
                {
                    if (this.IsDesignMode &&
                        e.Property == RadElement.IsMouseDownProperty)
                    {
                        //Skipping mouse down/up behaviors when in design time
                        return;
                    }

                    behavior.OnPropertyChange(this, e);
                }
            }
        }

        /// <summary>
        /// Adds a property change behavior to the list of behaviors of the element.
        /// </summary>
        /// <remarks>
        /// Behaviors can be used to specify how an element should respond when a certain element property changes.
        /// Behaviors are used internally by stylesheets when applying to an hiearrchy of elements.
        /// </remarks>
        /// <param name="behavior">behavior instance - should not be null (or Nothing in VB.NET)</param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void AddBehavior(PropertyChangeBehavior behavior)
        {
            if (!this.behaviors.Contains(behavior))
            {
                this.behaviors.Add(behavior);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public PropertyChangeBehaviorCollection GetBehaviors()
        {
            return this.behaviors;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void RemoveBehavior(PropertyChangeBehavior behavior)
        {
            this.behaviors.Remove(behavior);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void ClearBehaviors()
        {
            this.behaviors.Clear();
        }

        /// <summary>
        /// <see cref="AddBehavior"/>
        /// </summary>
        /// <param name="behaviors">list of behaviors</param>
        public void AddRangeBehavior(PropertyChangeBehaviorCollection behaviors)
        {
            this.behaviors.AddRange(behaviors);
        }

        /// <summary>
        /// Used internally to support RadControl infrastructure. This method is not intended for use directly from your code.
        /// </summary>
        /// <param name="newValue"></param>
        internal protected void SetThemeApplied(bool newValue)
        {
            this.BitState[IsThemeAppliedStateKey] = newValue;
        }

        protected virtual void OnStyleChanged(RadPropertyChangedEventArgs e)
        {
        }

        internal protected void SuspendThemeRefresh()
        {
            this.suspendThemeRefresh++;
#if DEBUG
            if (suspendThemeRefresh > 42)
                throw new InvalidOperationException("Theme refresh suspended too many times!");
#endif
        }

        internal protected void ResumeThemeRefresh()
        {
            this.suspendThemeRefresh--;
#if DEBUG
            if (suspendThemeRefresh < 0)
                throw new InvalidOperationException("Theme refresh resumed more times than it was suspended!");
#endif
        }

        internal bool IsThemeRefreshSuspended
        {
            get
            {
                if (this.suspendThemeRefresh > 0)
                {
                    return true;
                }

                //theme refresh is allowed only for constructed or loaded elements
                if (this.state == ElementState.Constructed || this.state == ElementState.Loaded)
                {
                    return false;
                }

                return true;
            }
        }

        internal void ApplyThemeRecursive()
        {
            // Old Style system
            //    if (this is RadItem) //Mike - optimization, as themes can be aapplied only on items
            //    {
            //        if (this.ElementTree.ComponentTreeHandler != null)
            //        {
            //            this.ElementTree.ComponentTreeHandler.SuspendUpdate();
            //            //this.ElementTree.Control.SuspendLayout();
            //            if (!this.UseNewLayoutSystem)
            //            {
            //                this.ElementTree.RootElement.SuspendLayout();
            //            }
            //        }
            //    }

            //    this.BitState[IsThemeAppliedStateKey] = true;

            //    if (this.PropagateStyleToChildren)
            //    {
            //        foreach (RadElement child in this.Children)
            //        {
            //            child.ApplyThemeRecursive();
            //        }
            //    }

            //    if (this.CanHaveOwnStyle) //Mike - optimization, as themes can be aapplied only on RadItems or custom 
            //    {
            //        this.ApplyTheme();
            //        if (this.elementTreec != null)
            //        {
            //            if (!this.UseNewLayoutSystem)
            //            {
            //                this.ElementTree.RootElement.ResumeLayout(true);
            //            }
            //            this.ElementTree.ComponentTreeHandler.ResumeUpdate();
            //        }i
            //    }
        }

        protected internal virtual bool CanHaveOwnStyle
        {
            get
            {
                return false;
            }
        }

        // Old Style system
        //[Obsolete]
        protected virtual void ApplyTheme()
        {
            //            this.InvalidateOwnTransformation();
            //            this.InvalidateTotalTransformation();

            //            StyleBuilder builder = ThemeResolutionService.GetStyleSheetBuilder(this);
            //            if (builder == null)
            //            {
            //                this.Style = null;
            //                return;
            //            }

            //            try
            //            {
            //                builder.BuildStyle(this);
            //                this.OnStyleBuilt();
            //            }
            //            catch (Exception ex)
            //            {
            //                string errMessge = "Error applying theme to an element of type " + this.GetType().FullName;
            //                if (this.elementTree != null)
            //                {
            //                    if (string.IsNullOrEmpty(this.ElementTree.Control.Name))
            //                    {
            //                        errMessge += " that is part of control " + this.ElementTree.Control.Name;
            //                        errMessge += " of type " + this.ElementTree.Control.GetType().FullName;
            //                    }
            //                    else
            //                    {
            //                        errMessge += " that is part of control: " + this.ElementTree.Control + "";
            //                    }
            //                }

            //                errMessge += ". Theme builder: " + builder.ToString();
            //                XmlStyleSheet xmlStyleSheet = builder.BuilderData as XmlStyleSheet;
            //                if (xmlStyleSheet != null)
            //                {
            //                    errMessge += ". Theme file location: " + xmlStyleSheet.ThemeLocation;
            //                }

            //                Debug.Fail(errMessge + ". Exception details:" + ex.ToString());
            //#if !DEBUG
            //                            Trace.WriteLine(errMessge + ". Exception details:" + ex.ToString());
            //#endif
            //            }
        }

        protected internal virtual void OnStyleBuilt()
        {
        }

        protected virtual void UnapplyStyle()
        {
            UnapplyParentStyle(this.parent);
        }

        private void UnapplyParentStyle(RadElement parent)
        {
            if (parent == null)
            {
                return;
            }

            UnapplyParentStyle(parent.parent);

            //TODO: This is a very heavy routine. When new styling mechanism is complete, the StylingManager will be responsible for unapplying style
            //if (parent.IsThemeApplied && parent.Style != null)
            //{
            //    parent.Style.Unapply(this);
            //}
        }


        protected void ReApplyStyle(bool traverseElementTree)
        {
            if (this.elementTree != null)
            {
                this.elementTree.StyleManager.ReApplyStyle(this, traverseElementTree);
            }
        }

        protected void ReApplyStyle()
        {
            this.ReApplyStyle(false);
        }

        public virtual void RemoveRangeBehaviors(PropertyChangeBehaviorCollection propertyChangeBehaviorCollection)
        {
            foreach (PropertyChangeBehavior target in propertyChangeBehaviorCollection)
            {
                for (int i = 0; i < this.behaviors.Count; i++)
                {
                    PropertyChangeBehavior toTest = this.behaviors[i];
                    if (toTest == target)
                    {
                        this.behaviors.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        public virtual void RemoveBehaviors(PropertyChangeBehavior behavior)
        {
            for (int i = 0; i < this.behaviors.Count; i++)
            {
                PropertyChangeBehavior toTest = this.behaviors[i];
                if (toTest == behavior)
                {
                    this.behaviors.RemoveAt(i);
                    i--;
                }
            }
        }

        public virtual void RemoveRangeRoutedEventBehaviors(RoutedEventBehaviorCollection routedEventBehaviorCollection)
        {
            foreach (RoutedEventBehavior target in routedEventBehaviorCollection)
            {
                for (int i = 0; i < this.routedEventBehaviors.Count; i++)
                {
                    RoutedEventBehavior toTest = this.routedEventBehaviors[i];
                    if (toTest.RaisedRoutedEvent.IsSameEvent(target.RaisedRoutedEvent))
                    {
                        this.routedEventBehaviors.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        /// <summary>
        /// Defines whether stylesheet rules should be applied for this element and its children, or only for this element
        /// </summary>
        [Browsable(false)]
        public virtual bool PropagateStyleToChildren
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Forces the theming mechanism to reapply the styles to the current element.
        /// There is an option to instruct the machanism to traverse the hierarchy of this
        /// element tree and reset all style maps found in the hierarchy. In other words,
        /// if there are child elements of this element which have styles that do not come from
        /// the style providing element of this element, they are also reset and reapplied.
        /// </summary>
        /// <param name="traverseElementTree">Defines whether children style maps are reset.</param>
        public void ForceReApplyStyle(bool traverseElementTree)
        {
            this.ReApplyStyle(traverseElementTree);

        }

        public void ForceReApplyStyle()
        {
            this.ReApplyStyle(false);
        }

        protected internal virtual bool ElementThemeAffectsChildren
        {
            get
            {
                return true;
            }
        }

        public Type GetThemeEffectiveType()
        {
            return this.ThemeEffectiveType;
        }

        protected virtual Type ThemeEffectiveType
        {
            get
            {
                return this.GetType();
            }
        }

        #endregion

        #region Children & Element Tree

        /// <summary>
        /// Gets a reference to the tree object, that contains information about the scene where the element is currently visualized.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual ComponentThemableElementTree ElementTree
        {
            get
            {
                return this.elementTree;
            }
            internal set
            {
                this.elementTree = value;
                if (value != null)
                {
                    this.layoutManager = this.elementTree.ComponentTreeHandler.LayoutManager;
                }
            }
        }

        /// <summary>
        /// Gets the collection of elements that are child elements in the element
        /// tree.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [Description("Gets the collection of elements that are child elements in the element tree.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual RadElementCollection Children
        {
            get
            {
                return this.children;
            }
        }

        /// <summary>
        /// Enumerates entire subtree of elements (using depth-first approach), 
        /// starting from this one as a root.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public IEnumerable<RadElement> ChildrenHierarchy
        {
            get
            {
                foreach (RadElement child in this.children)
                {
                    yield return child;

                    foreach (RadElement element in child.ChildrenHierarchy)
                    {
                        yield return element;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a list of child elements using the type to filter the results.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<RadElement> GetChildrenByType(Type type)
        {
            List<RadElement> list = new List<RadElement>(1);
            for (int i = 0; i < this.Children.Count; i++)
            {
                RadElement element = this.Children[i];
                if (element.GetType() == type)
                {
                    list.Add(this.Children[i]);
                }
            }
            return list;
        }

        public List<RadElement> GetChildrenByBaseType(Type type)
        {
            List<RadElement> list = new List<RadElement>(1);
            for (int i = 0; i < this.Children.Count; i++)
            {
                RadElement element = this.Children[i];
                if (type.IsAssignableFrom(element.GetType()))
                {
                    list.Add(this.Children[i]);
                }
            }
            return list;
        }

        /// <summary>
        /// Composes the current stylesheet, applied to this element. May be either set locally or retrieved from the parent chain.
        /// </summary>
        /// <returns></returns>
        public StyleSheet ComposeStyle()
        {
            StyleSheet currStyle = this.Style;
            RadElement currParent = this.parent;

            while (currStyle == null && currParent != null)
            {
                currStyle = currParent.Style;
                currParent = currParent.parent;
            }

            return currStyle;
        }

        /// <summary>
        /// Searches up the parent chain and returns the first parent with the provided ThemeEffectiveType.
        /// </summary>
        /// <returns></returns>
        public RadElement FindAncestorByThemeEffectiveType(Type themeEffectiveType)
        {
            RadElement currParent = this.parent;
            while (currParent != null)
            {
                if (currParent.ThemeEffectiveType.Equals(themeEffectiveType))
                {
                    return currParent;
                }

                currParent = currParent.parent;
            }

            return null;
        }

        /// <summary>
        /// Searches up the parent chain and returns the first parent of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T FindAncestor<T>() where T : RadElement
        {
            RadElement currParent = this.parent;
            while (currParent != null)
            {
                if (currParent is T)
                {
                    return (T)currParent;
                }

                currParent = currParent.parent;
            }

            return null;
        }

        /// <summary>
        /// Gets a boolean value that determines whether a given element
        /// resides in the element hierarchy of this element.
        /// </summary>
        /// <param name="element">An instance of the <see cref="RadElement"/>
        /// class which is checked.</param>
        /// <returns></returns>
        public bool IsAncestorOf(RadElement element)
        {
            RadElement parent = element.parent;

            while (parent != null)
            {
                if (parent == this)
                {
                    return true;
                }

                parent = parent.parent;
            }

            return false;
        }

        /// <summary>
        /// Searches down the subtree of elements, using breadth-first approach, and returns the first descendant of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T FindDescendant<T>() where T : RadElement
        {
            Queue<RadElement> children = new Queue<RadElement>();
            children.Enqueue(this);

            while (children.Count > 0)
            {
                RadElement child = children.Dequeue();
                foreach (RadElement nestedChild in child.children)
                {
                    if (nestedChild is T)
                    {
                        return (T)nestedChild;
                    }

                    children.Enqueue(nestedChild);
                }
            }

            return null;
        }

        /// <summary>
        /// Searches down the subtree of elements, using breadth-first approach, and returns the first descendant of type T.
        /// </summary>
        /// <returns></returns>
        public RadElement FindDescendant(Predicate<RadElement> criteria)
        {
            if (criteria == null)
            {
                return this;
            }

            Queue<RadElement> children = new Queue<RadElement>();
            children.Enqueue(this);

            while (children.Count > 0)
            {
                RadElement child = children.Dequeue();
                if (criteria(child))
                {
                    return child;
                }

                foreach (RadElement nestedChild in child.children)
                {
                    children.Enqueue(nestedChild);
                }
            }

            return null;
        }

        /// <summary>
        /// Searches down the subtree of elements, using breadth-first approach, and returns the first descendant of the specified Type.
        /// </summary>
        /// <returns></returns>
        public RadElement FindDescendant(Type descendantType)
        {
            if (descendantType == null)
            {
                return this;
            }

            Queue<RadElement> children = new Queue<RadElement>();
            children.Enqueue(this);

            while (children.Count > 0)
            {
                RadElement child = children.Dequeue();
                if (child.GetType() == descendantType)
                {
                    return child;
                }

                foreach (RadElement nestedChild in child.children)
                {
                    children.Enqueue(nestedChild);
                }
            }

            return null;
        }

        /// <summary>
        /// Provides flexible routine for traversing all descendants of this instance that match the provided predicate.
        /// </summary>
        /// <param name="traverseMode">The mode used to traverse the subtree.</param>
        /// <returns></returns>
        public IEnumerable<RadElement> EnumDescendants(TreeTraversalMode traverseMode)
        {
            Filter filter = null;
            return EnumDescendants(filter, traverseMode);
        }

        /// <summary>
        /// Provides flexible routine for traversing all descendants of this instance that match the provided predicate.
        /// </summary>
        /// <param name="predicate">The filter that defines the match criteria.</param>
        /// <param name="traverseMode">The mode used to traverse the subtree.</param>
        /// <returns></returns>
        public IEnumerable<RadElement> EnumDescendants(Predicate<RadElement> predicate, TreeTraversalMode traverseMode)
        {
            switch (traverseMode)
            {
                case TreeTraversalMode.BreadthFirst:
                    Queue<RadElement> children = new Queue<RadElement>();
                    children.Enqueue(this);

                    while (children.Count > 0)
                    {
                        RadElement child = children.Dequeue();
                        foreach (RadElement nestedChild in child.children)
                        {
                            if (predicate == null)
                            {
                                yield return nestedChild;
                            }
                            else if (predicate(nestedChild))
                            {
                                yield return nestedChild;
                            }

                            children.Enqueue(nestedChild);
                        }
                    }
                    break;
                default:
                    foreach (RadElement child in this.children)
                    {
                        if (predicate == null)
                        {
                            yield return child;
                        }
                        else if (predicate(child))
                        {
                            yield return child;
                        }

                        child.EnumDescendants(predicate, traverseMode);
                    }
                    break;
            }
        }

        /// <summary>
        /// Provides flexible routine for traversing all descendants of this instance that match the provided filter.
        /// </summary>
        /// <param name="filter">The filter that defines the match criteria.</param>
        /// <param name="traverseMode">The mode used to traverse the subtree.</param>
        /// <returns></returns>
        public IEnumerable<RadElement> EnumDescendants(Filter filter, TreeTraversalMode traverseMode)
        {
            switch (traverseMode)
            {
                case TreeTraversalMode.BreadthFirst:
                    Queue<RadElement> children = new Queue<RadElement>();
                    children.Enqueue(this);

                    while (children.Count > 0)
                    {
                        RadElement child = children.Dequeue();
                        foreach (RadElement nestedChild in child.children)
                        {
                            if (filter == null)
                            {
                                yield return nestedChild;
                            }
                            else if (filter.Match(nestedChild))
                            {
                                yield return nestedChild;
                            }

                            children.Enqueue(nestedChild);
                        }
                    }
                    break;
                default:
                    foreach (RadElement child in this.children)
                    {
                        if (filter == null)
                        {
                            yield return child;
                        }
                        else if (filter.Match(child))
                        {
                            yield return child;
                        }

                        child.EnumDescendants(filter, traverseMode);
                    }
                    break;
            }
        }

        /// <summary>
        /// Gets a list with all the descendants that match the provided filter.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="traverseMode"></param>
        /// <returns></returns>
        public List<RadElement> GetDescendants(Predicate<RadElement> predicate, TreeTraversalMode traverseMode)
        {
            return new List<RadElement>(this.EnumDescendants(predicate, traverseMode));
        }

        /// <summary>
        /// Gets a list with all the descendants that match the provided filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="traverseMode"></param>
        /// <returns></returns>
        public List<RadElement> GetDescendants(Filter filter, TreeTraversalMode traverseMode)
        {
            return new List<RadElement>(this.EnumDescendants(filter, traverseMode));
        }

        /// <summary>
        /// Provides a routine which enumerates all ancestors up in the parent chain of this element, which match the provided Filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IEnumerable<RadElement> GetAncestors(Filter filter)
        {
            RadElement parent = this.parent;
            while (parent != null)
            {
                if (filter == null)
                {
                    yield return parent;
                }
                if (filter.Match(parent))
                {
                    yield return parent;
                }

                parent = parent.parent;
            }
        }

        /// <summary>
        /// Provides a routine which enumerates all ancestors up in the parent chain of this element, which match the provided predicate.
        /// </summary>
        /// <param name="predicate">The predicate used to filter parents.</param>
        /// <returns></returns>
        public IEnumerable<RadElement> GetAncestors(Predicate<RadElement> predicate)
        {
            RadElement parent = this.parent;
            while (parent != null)
            {
                if (predicate == null)
                {
                    yield return parent;
                }
                if (predicate(parent))
                {
                    yield return parent;
                }

                parent = parent.parent;
            }
        }

        /// <summary>Gets a reference to the parent element in the visual element tree.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadElement Parent
        {
            get
            {
                return this.parent;
            }
        }

        #region Routed Events and Methods

        public virtual void RaiseRoutedEvent(RadElement sender, RoutedEventArgs args)
        {
            this.RaiseTunnelEvent(sender, args);
            if (!args.Canceled)
            {
                this.RaiseBubbleEvent(sender, args);
            }
        }

        ///// <summary>
        ///// We override this method to rise additiona routed events based on some application logis, say
        ///// the Routed Click Event.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="args"></param>
        public virtual void RaiseTunnelEvent(RadElement sender, RoutedEventArgs args)
        {
            //args.RoutedEvent
            this.OnTunnelEvent(sender, args);
            args.Direction = RoutingDirection.Tunnel;
            if (!args.Canceled)
            {
                this.SetEventProcessed(sender, args);
                this.PocessRootedEventBehaviors(sender, args);
                this.SetEventNotProcessed(sender, args);

                //delegate the event to all children
                //The foreach loop has been replaced 
                //because reparenting is allowed during TunnelEvent
                //and thus Children collection might be modified.
                for (int i = 0; i < this.children.Count; i++)
                {
                    RadElement child = this.children[i];

                    child.RaiseTunnelEvent(sender, args);

                    if (args.Canceled)
                    {
                        break;
                    }
                }
            }
        }

        public virtual void RaiseBubbleEvent(RadElement sender, RoutedEventArgs args)
        {
            //args.RoutedEvent
            this.OnBubbleEvent(sender, args);
            args.Direction = RoutingDirection.Bubble;
            if (!args.Canceled)
            {
                this.SetEventProcessed(sender, args);
                this.PocessRootedEventBehaviors(sender, args);
                this.SetEventNotProcessed(sender, args);
                if (this.Parent != null)
                {
                    this.Parent.RaiseBubbleEvent(sender, args);
                }
            }
        }

        private void SetEventProcessed(RadElement sender, RoutedEventArgs args)
        {
            this.processedEvents.Add(new ProcessedEvent(sender, args));
        }

        private void SetEventNotProcessed(RadElement sender, RoutedEventArgs args)
        {
            for (int i = 0; i < this.processedEvents.Count; i++)
            {
                ProcessedEvent processed = (ProcessedEvent)this.processedEvents[i];
                if (processed.Sender == sender && processed.Args == args)
                {
                    this.processedEvents.RemoveAt(i);
                    i--;
                }
            }
        }

        protected virtual void OnTunnelEvent(RadElement sender, RoutedEventArgs args)
        {
        }

        protected virtual void OnBubbleEvent(RadElement sender, RoutedEventArgs args)
        {
            RadElement parentItemToNotify = null;

            if (sender.NotifyParentOnMouseInput &&
                (args.RoutedEvent == RadElement.MouseDownEvent ||
                args.RoutedEvent == RadElement.MouseUpEvent ||
                args.RoutedEvent == RadElement.MouseClickedEvent ||
                args.RoutedEvent == RadElement.MouseDoubleClickedEvent))
            {
                for (RadElement parent = sender.Parent; parent != null; parent = parent.Parent)
                {
                    if (parent.ShouldHandleMouseInput)
                    {
                        parentItemToNotify = parent;
                        break;
                    }
                }
            }

            if (sender == this ||
                parentItemToNotify == this)
            {
                this.OnCLREventsRise(args);
            }
        }
        /// <exclude/>
        [Browsable(false)]
        public RoutedEventBehaviorCollection RoutedEventBehaviors
        {
            get { return routedEventBehaviors; }
        }

        private void PocessRootedEventBehaviors(RadElement sender, RoutedEventArgs args)
        {
            foreach (RoutedEventBehavior behavior in this.routedEventBehaviors)
            {
                if (behavior.RaisedRoutedEvent.IsSameEvent(sender, args))
                {
                    behavior.OnEventOccured(sender, this, args);
                }
            }
        }

        public bool IsEventInProcess(RaisedRoutedEvent raisedEvent)
        {
            for (int i = 0; i < this.processedEvents.Count; i++)
            {
                ProcessedEvent processed = (ProcessedEvent)this.processedEvents[i];
                if (raisedEvent.IsSameEvent(processed.Sender, processed.Args))
                {
                    return true;
                }
            }

            return false;
        }

        public static RoutedEvent RegisterRoutedEvent(string eventName, Type ownerType)
        {
            EnsureRegisteredRoutedEvents();

            RoutedEvent registered = new RoutedEvent(eventName, ownerType);
            registered.EventName = eventName;
            registered.OwnerType = ownerType;

            RadProperty.FromNameKey key = new RadProperty.FromNameKey(eventName, ownerType);

            registeredRoutedEvents[key] = registered;

            return registered;
        }

        private static void EnsureRegisteredRoutedEvents()
        {
            if (registeredRoutedEvents == null)
            {
                registeredRoutedEvents = new HybridDictionary();
            }
        }

        public static RoutedEvent GetRegisterRoutedEvent(string eventName, Type ownerType)
        {
            EnsureRegisteredRoutedEvents();

            RadProperty.FromNameKey key = new RadProperty.FromNameKey(eventName, ownerType);

            RoutedEvent registered = (RoutedEvent)registeredRoutedEvents[key];

            return registered;
        }

        public static RoutedEvent GetRegisterRoutedEvent(string eventName, string className)
        {
            EnsureRegisteredRoutedEvents();

            Type ownerType = RadTypeResolver.Instance.GetTypeByName(className);
            RadProperty.FromNameKey key = new RadProperty.FromNameKey(eventName, ownerType);

            RoutedEvent registered = (RoutedEvent)registeredRoutedEvents[key];

            return registered;
        }

        public RoutedEvent GetRegisterRoutedEvent(string eventName)
        {
            EnsureRegisteredRoutedEvents();

            RadProperty.FromNameKey key = new RadProperty.FromNameKey(eventName, this.GetType());

            RoutedEvent registered = (RoutedEvent)registeredRoutedEvents[key];

            return registered;
        }

        #endregion

        /// <summary>
        /// Forces an update in the z-ordered collection after a change in the Children collection.
        /// </summary>
        /// <param name="child"></param>
        /// <param name="change"></param>
        private void UpdateZOrderedCollection(RadElement child, ItemsChangeOperation change)
        {
            //notify our z-ordered collection about the change
            switch (change)
            {
                case ItemsChangeOperation.Cleared:
                    this.zOrderedChildren.Clear();
                    break;
                case ItemsChangeOperation.Inserted:
                    this.zOrderedChildren.Add(child);
                    break;
                case ItemsChangeOperation.Removed:
                    this.zOrderedChildren.Remove(child);
                    break;
                case ItemsChangeOperation.Set:
                    this.zOrderedChildren.OnElementSet(child);
                    break;
            }
        }

        protected virtual void OnChildrenChanged(RadElement child, ItemsChangeOperation changeOperation)
        {
            ChildrenChangedEventHandler handler1 = (ChildrenChangedEventHandler)this.Events[RadElement.ChildrenChangedKey];
            if (handler1 != null)
            {
                handler1(this, new ChildrenChangedEventArgs(child, changeOperation));
            }
        }

        /// <summary>
        /// Allows enumerating of this element's children, using the specified options.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public IEnumerable<RadElement> GetChildren(ChildrenListOptions options)
        {
            bool includeCollapsed = (options & ChildrenListOptions.IncludeCollapsed) == ChildrenListOptions.IncludeCollapsed;
            bool onlyVisible = (options & ChildrenListOptions.IncludeOnlyVisible) == ChildrenListOptions.IncludeOnlyVisible;
            bool reversedOrder = (options & ChildrenListOptions.ReverseOrder) == ChildrenListOptions.ReverseOrder;

            //we have the ZOrdered bit set, use our z-order collection
            if ((options & ChildrenListOptions.ZOrdered) == ChildrenListOptions.ZOrdered)
            {
                List<RadElement> elements = this.zOrderedChildren.Elements;
                int count = elements.Count;
                RadElement element;

                if (reversedOrder)
                {
                    for (int i = count - 1; i >= 0; i--)
                    {
                        element = elements[i];
                        ElementVisibility visibility = element.Visibility;

                        if (visibility == ElementVisibility.Collapsed && !includeCollapsed)
                        {
                            continue;
                        }
                        if (visibility == ElementVisibility.Hidden && onlyVisible)
                        {
                            continue;
                        }

                        yield return element;
                    }
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        element = elements[i];
                        ElementVisibility visibility = element.Visibility;

                        if (visibility == ElementVisibility.Collapsed && !includeCollapsed)
                        {
                            continue;
                        }
                        if (visibility == ElementVisibility.Hidden && onlyVisible)
                        {
                            continue;
                        }

                        yield return element;
                    }
                }
            }
            else
            {
                RadElementCollection list = this.Children;
                int count = list.Count;
                RadElement child;
                ElementVisibility visibility;

                if (reversedOrder)
                {
                    for (int i = count - 1; i >= 0; i--)
                    {
                        child = list[i];
                        visibility = child.Visibility;

                        if (visibility == ElementVisibility.Collapsed && !includeCollapsed)
                        {
                            continue;
                        }
                        if (visibility == ElementVisibility.Hidden && onlyVisible)
                        {
                            continue;
                        }

                        yield return child;
                    }
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        child = list[i];
                        visibility = child.Visibility;

                        if (visibility == ElementVisibility.Collapsed && !includeCollapsed)
                        {
                            continue;
                        }
                        if (visibility == ElementVisibility.Hidden && onlyVisible)
                        {
                            continue;
                        }

                        yield return child;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the count of all elements, which visibility is not ElementVisibility.Collapsed.
        /// </summary>
        [Browsable(false)]
        public int LayoutableChildrenCount
        {
            get
            {
                return this.zOrderedChildren.LayoutableCount;
            }
        }

        /// <summary>
        /// Sends this element to the beginning of its parent's z-ordered collection.
        /// </summary>
        public void SendToBack()
        {
            if (this.parent != null)
            {
                this.parent.zOrderedChildren.SendToBack(this);
            }
        }

        /// <summary>
        /// Sends this element at the end of its parent's z-ordered collection.
        /// </summary>
        public void BringToFront()
        {
            if (this.parent != null)
            {
                this.parent.zOrderedChildren.BringToFront(this);
            }
        }

        /// <summary>
        /// Method used by control Code Dom serializer to access items in the collection
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public RadElement GetChildAt(int index)
        {
            return this.Children[index];
        }

        /// <summary>
        /// Get a value indincating whether the element is a direct or indirect child of specified parent element
        /// </summary>
        /// <param name="parent">Parent to test</param>
        /// <returns>true if the element is child of parent, false otherwise</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool IsChildOf(RadElement parent)
        {
            if (parent.elementTree != this.elementTree)
            {
                return false;
            }

            for (RadElement thisParent = this.Parent; thisParent != null; thisParent = thisParent.Parent)
            {
                if (thisParent == parent)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Input - Mouse and Keyboard

        #region Events

        /// <summary>
        /// Occurs when the mouse pointer rests on the element. 
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler MouseHover
        {
            add
            {
                this.Events.AddHandler(RadElement.MouseHoverEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadElement.MouseHoverEventKey, value);
            }
        }

        //Removed in favor of RadItem-MouseWheel
        ///// <summary>
        ///// Occurs when the mouse pointer rests on the element. 
        ///// </summary>
        //[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        //public event MouseEventHandler MouseWheel
        //{
        //    add
        //    {
        //        this.Events.AddHandler(RadElement.MouseWheelEventKey, value);
        //    }
        //    remove
        //    {
        //        this.Events.RemoveHandler(RadElement.MouseWheelEventKey, value);
        //    }
        //}

        /// <summary>
        /// Occurs when the mouse pointer is moved over the element.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public event MouseEventHandler MouseMove
        {
            add
            {
                this.Events.AddHandler(RadElement.MouseMoveEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadElement.MouseMoveEventKey, value);
            }
        }
        /// <summary>
        /// Occurs when the mouse pointer is over the element and a mouse button is pressed.
        /// </summary>
        [Browsable(true)]
        public event MouseEventHandler MouseDown
        {
            add
            {
                this.Events.AddHandler(RadElement.MouseDownEventKey, value);//MouseDownEvent
            }
            remove
            {
                this.Events.RemoveHandler(RadElement.MouseDownEventKey, value);
            }
        }
        /// <summary>
        /// Occurs when the mouse pointer is over the element and a mouse button is released.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public event MouseEventHandler MouseUp
        {
            add
            {
                this.Events.AddHandler(RadElement.MouseUpEventKey, value);//MouseUpEvent
            }
            remove
            {
                this.Events.RemoveHandler(RadElement.MouseUpEventKey, value);
            }
        }
        /// <summary>
        /// Occurs when the mouse pointer enters the element.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler MouseEnter
        {
            add
            {
                this.Events.AddHandler(RadElement.MouseEnterEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadElement.MouseEnterEventKey, value);
            }
        }
        /// <summary>
        /// Occurs when the mouse pointer leaves the element.
        /// </summary>
        public event EventHandler EnabledChanged
        {
            add
            {
                this.Events.AddHandler(RadElement.EnabledChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadElement.EnabledChangedEventKey, value);
            }

        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler MouseLeave
        {
            add
            {
                this.Events.AddHandler(RadElement.MouseLeaveEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadElement.MouseLeaveEventKey, value);
            }
        }

        /// <summary>
        /// Occurs when the children collection of the element is changed.
        /// </summary>
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Advanced)]
        public event ChildrenChangedEventHandler ChildrenChanged
        {
            add
            {
                this.Events.AddHandler(RadElement.ChildrenChangedKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadElement.ChildrenChangedKey, value);
            }
        }

        #endregion

        #region Focus

        /// <summary>
        /// Determines whether the element or one of its descendants currently contains the keyboard focus.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ContainsFocus
        {
            get
            {
                return (bool)this.GetValue(ContainsFocusProperty);
            }
            internal set
            {
                this.SetValue(ContainsFocusProperty, value);
            }
        }

        public virtual bool Focus()
        {
            return this.Focus(true);
        }

        protected bool Focus(bool setParentControlFocus)
        {
            bool result = false;
            if (!this.CanFocus)
            {
                return result;
            }
            if (this.elementTree != null)
            {
                if (setParentControlFocus && this.ElementTree.ComponentTreeHandler.OnFocusRequested(this))
                {
                    SetFocusPropertySafe(true);
                    result = true;
                }
                else if (!this.IsFocused)
                {
                    SetFocusPropertySafe(true);
                }
            }
            return result;
        }

        private void SetFocusPropertySafe(bool isFocused)
        {
            if (this.elementTree != null)
            {
                this.ElementTree.ComponentTreeHandler.Behavior.SettingElementFocused(this, isFocused);
            }
            SetElementFocused(isFocused);
        }

        internal void SetElementFocused(bool isFocused)
        {
            this.BitState[ProtectFocusedPropertyStateKey] = false;
            this.SetValue(IsFocusedProperty, isFocused);
            this.BitState[ProtectFocusedPropertyStateKey] = true;
        }

        protected virtual void KillFocus()
        {
            this.SetFocusPropertySafe(false);
        }

        #endregion

        #region Mouse

        protected virtual void OnCLREventsRise(RoutedEventArgs args)
        {
            // We fire the appropriate CLR events for the current instance through the use
            // of a virtual method OnCLREventsRise that calls the On[Event] signatures of the CLR events.
            // We can set the args.Canceled property inside that method based on a custom logic, published through
            // a regular CLR eventhandler.
            if (this.state != ElementState.Loaded)
            {
                return;
            }

            MouseEventArgs originalEventArgs = args.OriginalEventArgs as MouseEventArgs;

            if (args.RoutedEvent == RadElement.MouseDownEvent)
            {
                this.OnMouseDown(originalEventArgs);
            }
            if (args.RoutedEvent == RadElement.MouseUpEvent)
            {
                this.OnMouseUp(originalEventArgs);
            }
        }

        protected virtual void OnMouseMove(MouseEventArgs e)
        {
            MouseEventHandler handler1 =
            (MouseEventHandler)this.Events[RadElement.MouseMoveEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        protected virtual void OnMouseHover(EventArgs e)
        {
            EventHandler handler1 =
            (EventHandler)this.Events[RadElement.MouseHoverEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        //Removed in favor of RadItem-MouseWheel
        //protected virtual void OnMouseWheel(MouseEventArgs e)
        //{
        //    MouseEventHandler handler1 =
        //    (MouseEventHandler)this.Events[RadElement.MouseWheelEventKey];
        //    if (handler1 != null)
        //    {
        //        handler1(this, e);
        //    }
        //}

        protected virtual void OnEnabledChanged(RadPropertyChangedEventArgs e)
        {
            bool enabled = (bool)e.NewValue;

            if (this.Visibility == ElementVisibility.Visible && enabled && this.state == ElementState.Loaded)
            {
                Point client = this.elementTree.Control.PointToClient(Control.MousePosition);
                this.IsMouseOver = this.ControlBoundingRectangle.Contains(client);
            }
            else
            {
                this.IsMouseDown = false;
                this.IsMouseOver = false;
                this.IsMouseOverElement = false;
                this.SetElementFocused(false);
                this.ContainsFocus = false;
                this.ContainsMouse = false;
            }

            if (this.suspendRecursiveEnabledUpdates == 0)
            {
                foreach (RadElement child in this.Children)
                {
                    child.OnParentEnabledChanged(e);
                }
            }

            EventHandler handler1 = (EventHandler)this.Events[RadElement.EnabledChangedEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        protected virtual void OnParentEnabledChanged(RadPropertyChangedEventArgs e)
        {
            this.UpdateEnabledFromParent();
        }

        protected virtual void OnMouseDown(MouseEventArgs e)
        {
            //Focus();
            MouseEventHandler handler1 =
            (MouseEventHandler)this.Events[RadElement.MouseDownEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
            //this.RaiseRoutedEvent(this, new RoutedEventArgs(e, RadElement.MouseDownEvent));
        }

        protected virtual void OnMouseUp(MouseEventArgs e)
        {
            MouseEventHandler handler1 =
            (MouseEventHandler)this.Events[RadElement.MouseUpEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
            //this.RaiseRoutedEvent(this, new RoutedEventArgs(e, RadElement.MouseUpEvent));
        }

        protected virtual void OnMouseEnter(EventArgs e)
        {
            EventHandler handler1 =
            (EventHandler)this.Events[RadElement.MouseEnterEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        protected virtual void OnMouseLeave(EventArgs e)
        {
            EventHandler handler1 =
            (EventHandler)this.Events[RadElement.MouseLeaveEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        /// Specifies whether the Item should handle MouseOver, MouseMove and related mouse events.
        /// </summary>
        /// <remarks>
        /// By default only elements that inherit RadItem can process mouse input.
        /// </remarks>
#if DEBUG
        [Browsable(true)]
#else
        [Browsable(false)]
#endif
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool ShouldHandleMouseInput
        {
            get
            {
                return this.GetBitState(ShouldHandleMouseInputStateKey);
            }
            set
            {
                this.SetBitState(ShouldHandleMouseInputStateKey, value);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the element should pass the handled mouse
        ///     event to the first parent element which has the
        ///     <see cref="ShouldHandleMouseInput"/> property set to true.
        /// </summary>
#if DEBUG
        [Browsable(true)]
#else
        [Browsable(false)]
#endif
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool NotifyParentOnMouseInput
        {
            get
            {
                return this.GetBitState(NotifyParentOnMouseInputStateKey);
            }
            set
            {
                this.SetBitState(NotifyParentOnMouseInputStateKey, value);
            }
        }

        internal bool IsAbleToRespondToMouseEvents
        {
            get
            {
                if (this.state != ElementState.Loaded)
                {
                    return false;
                }

                return this.Enabled;//TODO MUST revised && this.shouldHandleMouseInput;
            }
        }

        protected virtual MouseEventArgs MouseEventArgsFromControl(MouseEventArgs e)
        {
            Point localPoint = PointFromControl(e.Location);
            return new MouseEventArgs(e.Button, e.Clicks, localPoint.X, localPoint.Y, e.Delta);
        }

        protected virtual void DoMouseDown(MouseEventArgs e)
        {
            if (!this.IsAbleToRespondToMouseEvents)
                return;

            this.IsMouseDown = true;

            RoutedEventArgs args = new RoutedEventArgs(e, RadElement.MouseDownEvent);
            this.RaiseRoutedEvent(this, args);
        }

        protected virtual void DoMouseUp(MouseEventArgs e)
        {
            if (!this.IsAbleToRespondToMouseEvents)
                return;

            this.IsMouseDown = false;
            RoutedEventArgs args = new RoutedEventArgs(e, RadElement.MouseUpEvent);
            this.RaiseRoutedEvent(this, args);
        }

        protected virtual void DoMouseMove(MouseEventArgs e)
        {
            if (!this.IsAbleToRespondToMouseEvents)
                return;

            OnMouseMove(e);

            if (this.ShouldHandleMouseInput && this.NotifyParentOnMouseInput && this.Parent != null)
                this.Parent.DoMouseMove(e);

            foreach (RadElement child in this.Children)
            {
                if (!child.ShouldHandleMouseInput)
                {
                    child.DoMouseMove(e);
                }
            }
        }

        protected virtual void DoMouseHover(EventArgs e)
        {
            if (!this.IsAbleToRespondToMouseEvents)
                return;

            OnMouseHover(e);
        }

        /// <summary>
        /// Updates the ContainsMouse property. The notification may be received from a child whose IsMouseOver property has changed.
        /// </summary>
        protected internal virtual void UpdateContainsMouse()
        {
            if (this.state != ElementState.Loaded)
            {
                this.ContainsMouse = false;
            }
            else
            {
                bool boundsContainsMouse = this.RectangleToScreen(this.Bounds).Contains(Control.MousePosition);
                if (boundsContainsMouse)
                {
                    Control underMouse = ControlHelper.GetControlUnderMouse();
                    this.ContainsMouse = underMouse != null && 
                        (underMouse == this.elementTree.Control ||
                        ControlHelper.IsDescendant(this.elementTree.Control, underMouse));
                }
                else
                {
                    this.ContainsMouse = false;
                }
            }

            if (this.parent != null)
            {
                this.parent.UpdateContainsMouse();
            }
        }

        /// <summary>
        /// Updates the ContainsFocus property. The notification may be received from a child whose IsFocused property has changed.
        /// </summary>
        protected virtual void UpdateContainsFocus(bool isFocused)
        {
            this.ContainsFocus = isFocused;
            if (this.parent != null)
            {
                this.parent.UpdateContainsFocus(isFocused);
            }
        }

        protected virtual void DoMouseEnter(EventArgs e)
        {
            if (!this.IsAbleToRespondToMouseEvents)
                return;

            //Note: IsMouseOver state propagates to children that ha ShouldHandleMouseInput in 
            //OnPropertyChanged for IsMouseOver property
            this.IsMouseOver = true;
            OnMouseEnter(e);
        }

        protected virtual void DoMouseLeave(EventArgs e)
        {
            if (!this.IsAbleToRespondToMouseEvents)
                return;

            //Note: IsMouseOver state propagates to children that ha ShouldHandleMouseInput in 
            //OnPropertyChanged
            this.IsMouseOver = false;
            OnMouseLeave(e);
        }

        protected virtual void DoClick(EventArgs e)
        {
            if (!this.IsAbleToRespondToMouseEvents)
                return;
            //
        }

        protected virtual void DoDoubleClick(EventArgs e)
        {
            if (!this.IsAbleToRespondToMouseEvents)
                return;
            //
        }

        internal void CallDoMouseDown(MouseEventArgs e)
        {
            DoMouseDown(e);
        }

        internal void CallDoMouseUp(MouseEventArgs e)
        {
            DoMouseUp(e);
        }

        internal void CallDoMouseMove(MouseEventArgs e)
        {
            DoMouseMove(e);
        }

        internal void CallDoMouseHover(EventArgs e)
        {
            DoMouseHover(e);
        }

        internal void CallDoMouseEnter(EventArgs e)
        {
            DoMouseEnter(e);
        }

        internal void CallDoMouseLeave(EventArgs e)
        {
            DoMouseLeave(e);
        }

        internal void CallDoClick(EventArgs e)
        {
            DoClick(e);
        }

        internal void CallDoDoubleClick(EventArgs e)
        {
            DoDoubleClick(e);
        }
        #endregion

        #endregion

        #region Property System

        protected override ValueUpdateResult SetValueCore(RadPropertyValue propVal, object propModifier, object newValue, ValueSource source)
        {
            //While element is initializing, all local values are treated as default
            if (source == ValueSource.Local && this.state == ElementState.Constructing)
            {
                source = ValueSource.DefaultValueOverride;
            }

            return base.SetValueCore(propVal, propModifier, newValue, source);
        }

        private void InvalidateCachedValues(RadProperty property)
        {
            if (this.state != ElementState.Loaded)
            {
                return;
            }

            if (property == RadElement.VisibilityProperty)
            {
                this.LayoutEngine.InvalidateCachedBorder();
                return;
            }

            if (property == RadElement.BorderThicknessProperty)
            {
                this.InvalidateBorderThickness();
            }
            else if (property == RadElement.PaddingProperty)
            {
                this.InvalidatePadding();
            }
            else if (property == RadElement.MarginProperty)
            {
                this.InvalidateFullRectangle();
            }
            else if ((this is IBoxElement) && CheckBoxProperty(property.Name))
            {
                this.LayoutEngine.InvalidateCachedBorder();
            }
            else if (property == RadElement.RightToLeftProperty)
            {
                this.InvalidateTransformations(false);
            }
            else if (property == RadElement.AngleTransformProperty)
            {
                InvalidateBoundingRectangle();
            }
            else if (property == RadElement.PositionOffsetProperty)
            {
                /*
                RoutedEventArgs args = new RoutedEventArgs(EventArgs.Empty, RadElement.PositionOffsetChangedEvent);
                this.RaiseTunnelEvent(this, args);
                 */

                InvalidateTransformations();
                //Fix for bug - RadTextBoxElement not scolling correctly in RadScrollViewer
                this.CallOnTransformationInvalidatedRecursively();
            }
            else if (property == RadElement.ScaleTransformProperty)
            {
                InvalidateBoundingRectangle();
            }
        }

        private bool CheckBoxProperty(string propertyName)
        {
            if ((propertyName == "Width") ||
                (propertyName == "LeftWidth") || (propertyName == "TopWidth") ||
                (propertyName == "RightWidth") || (propertyName == "BottomWidth"))
                return true;

            return false;
        }

        protected virtual void NotifyChildren(RadPropertyChangedEventArgs e)
        {
            RadElementCollection childCollection = this.Children;
            for (int i = 0; i < childCollection.Count; i++)
            {
                RadElement child = childCollection[i];
                child.OnParentPropertyChanged(e);
            }
        }

        private void SetControlPropertyValue(RadElement parentElement, RadProperty property, object value)
        {
            RadElementCollection childCollection = parentElement.Children;
            int count = childCollection.Count;

            for (int i = 0; i < count; i++)
            {
                RadElement child = childCollection[i];
                if (!child.ShouldHandleMouseInput)
                {
                    child.SetValue(property, value);
                    this.SetControlPropertyValue(child, property, value);
                }
            }
        }

        protected virtual void OnBoundsChanged(RadPropertyChangedEventArgs e)
        {
            //TODO: fire BoundsChanged event

            // Invalidate direct children's transformation because of the alignment
            if (!this.GetBitState(UseNewLayoutStateKey))
            {
                for (int i = 0; i < this.children.Count; i++)
                {
                    RadElement child = this.Children[i];
                    if (child.Alignment != ContentAlignment.TopLeft)
                    {
                        child.InvalidateOwnTransformation();
                        child.InvalidateTotalTransformationOnly(false);
                    }

                }
            }
        }

        protected virtual void OnLocationChanged(RadPropertyChangedEventArgs e)
        {
            //TODO: fire LocationChanged event
            //InvalidateTransformations();
        }

        protected virtual void OnDisplayPropertyChanged(RadPropertyChangedEventArgs e)
        {
            if (this.state == ElementState.Loaded)
            {
                this.PerformInvalidate();
            }
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            RadElementPropertyMetadata metadata = (RadElementPropertyMetadata)e.Metadata;
            Debug.Assert(metadata != null);

            //Notify element tree which will redirect changes to StyleMnager
            if (this.elementTree != null)
            {
                this.elementTree.NotifyElementPropertyChanged(this, e);
            }

            if (metadata.IsInherited)
            {
                this.NotifyChildren(e);
            }

            this.ProcessBehaviors(e);
            this.InvalidateCachedValues(e.Property);

            if (e.Property == IsMouseOverProperty)
            {
                this.UpdateContainsMouse();
            }
            else if (e.Property == VisibilityProperty)
            {
                //TODO: Update mouse and focus flags upon visibility changed
                if (this.parent != null)
                {
                    this.parent.zOrderedChildren.OnElementVisibilityChanged(this);
                }

                RoutedEventArgs args = new RoutedEventArgs(EventArgs.Empty, RadElement.VisibilityChangingEvent);
                RaiseTunnelEvent(this, args);

                if (this.state == ElementState.Loaded)
                {
                    if (ElementVisibility.Collapsed == (ElementVisibility)e.OldValue || ElementVisibility.Collapsed == (ElementVisibility)e.NewValue)
                    {
                        if (this.GetBitState(UseNewLayoutStateKey))
                        {
                            this.SignalDesiredSizeChange();
                            this.Invalidate();
                        }
                        else
                        {
                            this.LayoutEngine.InvalidateLayout();
                        }
                        this.OnLayoutPropertyChanged(e);
                    }
                    else
                    {
                        this.OnDisplayPropertyChanged(e);
                    }
                }
            }
            else if (state == ElementState.Loaded)
            {
                if (!this.GetBitState(UseNewLayoutStateKey))
                {
                    if (metadata.InvalidatesLayout && (this.elementTree != null))
                        this.LayoutEngine.InvalidateLayout();
                }
                if ((metadata.AffectsLayout || metadata.AffectsMeasure || metadata.AffectsArrange))
                    this.OnLayoutPropertyChanged(e);

                if ((metadata.AffectsDisplay))
                    this.OnDisplayPropertyChanged(e);
            }

            if (e.Property == IsMouseOverProperty || e.Property == IsMouseDownProperty)
            {
                if (this.ShouldHandleMouseInput)
                {
                    this.SetControlPropertyValue(this, e.Property, e.NewValue);
                }
            }
            else if (e.Property == IsFocusedProperty)
            {
                if (this.GetBitState(ProtectFocusedPropertyStateKey))
                {
                    throw new InvalidOperationException("The property IsFocused can be set only by the method Focus()");
                }
                else
                {
                    this.UpdateContainsFocus((bool)e.NewValue);
                }
            }
            else if (e.Property == ZIndexProperty)
            {
                if (this.parent != null)
                {
                    this.parent.zOrderedChildren.OnElementZIndexChanged(this);
                }
            }
            else if (e.Property == RadElement.EnabledProperty)
            {
                this.OnEnabledChanged(e);
            }
        }

        protected override bool CanRaisePropertyChangeNotifications(RadPropertyValue propVal)
        {
            if (!base.CanRaisePropertyChangeNotifications(propVal))
            {
                return false;
            }

            return this.state == ElementState.Constructed || this.state == ElementState.Loaded || this.state == ElementState.Unloaded;
        }

        protected internal override bool IsPropertyCancelable(RadPropertyMetadata metadata)
        {
            RadElementPropertyMetadata elementMetadata = metadata as RadElementPropertyMetadata;
            if (elementMetadata != null)
            {
                return elementMetadata.Cancelable;
            }

            return base.IsPropertyCancelable(metadata);
        }

        /// <summary>
        /// Invalidates all Ambient (inherited) properties down in the parent chain.
        /// Called when the parent for this element changes.
        /// </summary>
        /// <param name="recursive">True to update children also, false otherwise.</param>
        private void UpdateInheritanceChain(bool recursive)
        {
            //ivalidate all rad properties which ValueSource is currently Inherited or Default
            this.PropertyValues.ResetInheritableProperties();
            //reset the system skin paint flag
            this.paintSystemSkin = null;

            if (recursive)
            {
                //propagate notification down the element tree
                int count = this.children.Count;
                for (int i = 0; i < count; i++)
                {
                    this.children[i].UpdateInheritanceChain(recursive);
                }
            }
        }

        /// <summary>
        /// The object gets notified for a parent property change.
        /// </summary>
        /// <param name="args"></param>
        protected internal virtual void OnParentPropertyChanged(RadPropertyChangedEventArgs args)
        {
            //property is not inheritable
            if (this.parent == null || !args.Metadata.IsInherited)
            {
                return;
            }

            RadPropertyValue propVal = this.PropertyValues.GetEntry(args.Property, false);
            //do not create new entry if not needed
            if (propVal == null)
            {
                //check whether we need to invalidate layout
                RadElementPropertyMetadata metadata = args.Metadata as RadElementPropertyMetadata;
                if (metadata != null)
                {
                    if (PropertyAffectsLayout(metadata))
                        this.OnLayoutPropertyChanged(args);
                }

                //No notification fot this element, but children may still need to recieve Notifications
                this.NotifyChildren(args);
                return;
            }

            //current value source is Inherited, so reset the value
            if (propVal.ValueSource == ValueSource.Inherited || propVal.ValueSource == ValueSource.DefaultValue)
            {
                this.ResetValueCore(propVal, ValueResetFlags.Inherited);
            }
            else
            {
                //simply invalidate previously cached value without forcing an update
                propVal.InvalidateInheritedValue();
            }
        }

        internal override RadObject InheritanceParent
        {
            get
            {
                if (this.parent != null &&
                    !(this.parent.IsDisposing || this.parent.IsDisposed))
                {
                    return this.parent;
                }

                return null;
            }
        }

        /// <summary>
        /// Add the ElementTree property if we are in the context of RadControlSpy.
        /// </summary>
        /// <param name="props"></param>
        /// <returns></returns>
        internal override PropertyDescriptorCollection ReplaceDefaultDescriptors(PropertyDescriptorCollection props)
        {
            PropertyDescriptorCollection baseProps = base.ReplaceDefaultDescriptors(props);

            if ((bool)this.GetValue(IsEditedInSpyProperty))
            {
                PropertyDescriptor elementTreeProperty = TypeDescriptor.CreateProperty(this.GetType(),
                                                                                       "ElementTree",
                                                                                       typeof(ComponentThemableElementTree),
                                                                                       new Attribute[] { new BrowsableAttribute(true) });
                baseProps.Add(elementTreeProperty);
            }

            return baseProps;
        }

        #endregion

        #region OLD_RAD_ELEMENT

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                return this.GetType().Name;
            }

            return this.Name + "[" + this.GetType().Name + "]";
        }

        #region Properties

        #region Size and Position

        /// <summary>
        /// Gets or sets a value indicating whether the element size will be calculated
        /// automatically by the layout system. Value of false indicates that the element's size
        /// will not be changed when calculating the layout.
        /// </summary>
        [Description("Indicates whether the element would automatically calculate its bounds depending on the value of AutoSizeMode and its children.")]
        [RadPropertyDefaultValue("AutoSize", typeof(RadElement)), Category(RadDesignCategory.LayoutCategory)]
        public virtual bool AutoSize
        {
            get
            {
                return (bool)this.GetValue(AutoSizeProperty);
            }
            set
            {
                this.SetValue(AutoSizeProperty, BooleanBoxes.Box(value));
            }
        }

        public static RadProperty BoundsProperty =
            RadProperty.Register("Bounds", typeof(Rectangle), typeof(RadElement),
                new RadElementPropertyMetadata(Rectangle.Empty,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));

        /// <summary>
        ///     Gets or sets a value corresponding to the bounding rectangle of the element.
        ///     Location and/or Size portions of the bounds may be calculated automatically based
        ///     on the current <see cref="AutoSize"/> and <see cref="AutoSizeMode"/>
        ///     settings.
        /// </summary>
        [Description("Represents the element bounding rectangle")]
        [RadPropertyDefaultValue("Bounds", typeof(RadElement)), Category(RadDesignCategory.LayoutCategory)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [NotifyParentProperty(true)]
        public virtual Rectangle Bounds
        {
            get
            {
                if (this.cachedBounds == LayoutUtils.InvalidBounds)
                {
                    this.cachedBounds = (Rectangle)this.GetValue(BoundsProperty);
                    InvalidateBoundsRectangles();
                }

                return this.cachedBounds;
            }
            set
            {
                this.SetBounds(value);
            }
        }

        /// <summary>
        ///     Gets or sets the location of the element based on the element parent rectangle.
        ///     Corresponds to <see cref="Bounds"/>.Location
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.LayoutCategory)]
        [Description("Gets or sets the location of the element based on the element parent rectangle.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [NotifyParentProperty(true)]
        public virtual Point Location
        {
            get
            {
                return this.Bounds.Location;
            }
            set
            {
                this.SetBounds(new Rectangle(value, this.Size));
            }
        }

        /// <summary>
        ///     Gets or sets the size of the element which is the height and width of the visual
        ///     rectangle that would contain the graphics of the element. Size corresponds to
        ///     element's Bounds.Size. When the <see cref="AutoSize">AutoSize</see> property is set
        ///     to true setting the Size property to some value has no effect.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.LayoutCategory)]
        [Description("Size of the element, based on its bounds.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [NotifyParentProperty(true)]
        public virtual Size Size
        {
            get
            {
                return this.Bounds.Size;
            }
            set
            {
                this.SetBounds(new Rectangle(Location, value));
            }
        }

        public static RadProperty BorderThicknessProperty =
            RadProperty.Register("BorderThickness", typeof(Padding), typeof(RadElement),
                new RadElementPropertyMetadata(Padding.Empty,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the border thickness of the element. This thickness is included into the
        /// element's bounding rectangle.
        /// </summary>
        [Description("Represents the thickness of the element's border.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [RadPropertyDefaultValue("BorderThickness", typeof(RadElement)), Category(RadDesignCategory.LayoutCategory)]
        public virtual Padding BorderThickness
        {
            get
            {
                return (Padding)this.GetValue(BorderThicknessProperty);
            }
            set
            {
                this.SetValue(BorderThicknessProperty, value);
            }
        }

        public static RadProperty PaddingProperty =
            RadProperty.Register("Padding", typeof(Padding), typeof(RadElement),
                new RadElementPropertyMetadata(Padding.Empty,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the padding sizes of the element. The paddings are included into the
        /// element's bounding rectangle.
        /// </summary>
        [Description("Represents the padding sizes of the element. Paddings do not calculate into element bounds")]
        [RadPropertyDefaultValue("Padding", typeof(RadElement)), Category(RadDesignCategory.LayoutCategory)]
        [Localizable(true)]
        public virtual Padding Padding
        {
            get
            {
                return (Padding)this.GetValue(PaddingProperty);
            }
            set
            {
                this.SetValue(PaddingProperty, value);
            }
        }

        public static RadProperty MarginProperty =
            RadProperty.Register("Margin", typeof(Padding), typeof(RadElement),
                new RadElementPropertyMetadata(Padding.Empty,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsParentMeasure));

        /// <summary>
        /// Gets or sets a value corresponding to the margins of the element. Margins are not
        /// included into the element's bounding rectangle.
        /// </summary>
        [Description("Represents the margins of the element. Margins do not calculate into element bounds.")]
        [RadPropertyDefaultValue("Margin", typeof(RadElement)), Category(RadDesignCategory.LayoutCategory)]
        [Localizable(true)]
        public virtual Padding Margin
        {
            get
            {
                return (Padding)this.GetValue(MarginProperty);
            }
            set
            {
                this.SetValue(MarginProperty, value);
            }
        }

        public static RadProperty AlignmentProperty =
            RadProperty.Register("Alignment", typeof(ContentAlignment), typeof(RadElement),
                new RadElementPropertyMetadata(ContentAlignment.TopLeft,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        /// <summary>
        /// Gets or sets the preferred location of the element if its size is less than its
        /// parent size.
        /// </summary>
        [Description("Represents the preferred location of the element if its size is less than its parent size.")]
        [RadPropertyDefaultValue("Alignment", typeof(RadElement)), Category(RadDesignCategory.LayoutCategory),
        Localizable(true)]
        public virtual ContentAlignment Alignment
        {
            get
            {
                return (ContentAlignment)this.GetValue(AlignmentProperty);
            }
            set
            {
                this.SetValue(AlignmentProperty, value);
            }
        }

        public static RadProperty AutoSizeModeProperty =
            RadProperty.Register("AutoSizeMode", typeof(RadAutoSizeMode), typeof(RadElement),
                new RadElementPropertyMetadata(RadAutoSizeMode.WrapAroundChildren,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        /// <summary>
        ///     Gets or sets the way the element should calculate its <see cref="Size"/>, when
        ///     the <see cref="AutoSize"/> property is set to true.
        /// </summary>
        [Description("If element AutoSize is set to true, corresponds to the way the element whould calculate its size.")]
        [RadPropertyDefaultValue("AutoSizeMode", typeof(RadElement)), Category(RadDesignCategory.LayoutCategory)]
        public virtual RadAutoSizeMode AutoSizeMode
        {
            get
            {
                return (RadAutoSizeMode)this.GetValue(AutoSizeModeProperty);
            }
            set
            {
                this.SetValue(AutoSizeModeProperty, AutoSizeModeBoxes.Box(value));
            }
        }

        public static RadProperty FitToSizeModeProperty =
            RadProperty.Register("FitToSizeMode", typeof(RadFitToSizeMode), typeof(RadElement),
                new RadElementPropertyMetadata(RadFitToSizeMode.FitToParentContent,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsParentMeasure));

        /// <summary>
        /// Gets or sets a value indicating the way element will fill its available size when
        /// parent element is calculating element size and location.
        /// </summary>
        [RadPropertyDefaultValue("FitToSizeMode", typeof(RadElement)), Category(RadDesignCategory.LayoutCategory)]
        public virtual RadFitToSizeMode FitToSizeMode
        {
            get
            {
                return (RadFitToSizeMode)this.GetValue(FitToSizeModeProperty);
            }
            set
            {
                this.SetValue(FitToSizeModeProperty, value);
            }
        }

        public static RadProperty MinSizeProperty =
            RadProperty.Register("MinSize", typeof(Size), typeof(RadElement),
                new RadElementPropertyMetadata(Size.Empty,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));

        /// <summary>
        /// Get or sets the minimum size to apply on an element when layout is calculated.
        /// </summary>
        [Description("Represents the minimum size of the element")]
        [RadPropertyDefaultValue("MinSize", typeof(RadElement)), Category(RadDesignCategory.LayoutCategory)]
        public virtual Size MinSize
        {
            get
            {
                return (Size)this.GetValue(MinSizeProperty);
            }
            set
            {
                this.SetValue(MinSizeProperty, value);
            }
        }

        #endregion

        #region Action

        public static RadProperty EnabledProperty =
            RadProperty.Register("Enabled", typeof(bool), typeof(RadElement),
                new RadElementPropertyMetadata(true, ElementPropertyOptions.AffectsDisplay));

        /// <summary>
        /// Gets or sets a value indicating whether the element can respond to user
        /// interaction.
        /// </summary>
        /// <remarks> 
        /// By default, if element is currently selected when Enalbed set to false, next element would be selected.
        /// Values inherits from Parent.Enabled.
        /// When a scrollable control is disabled, the scroll bars are also disabled. 
        /// For example, a disabled multiline textbox is unable to scroll to display all the lines of text.
        /// </remarks>
        [RadPropertyDefaultValue("Enabled", typeof(RadElement)), Category(RadDesignCategory.BehaviorCategory)]
        public virtual bool Enabled
        {
            get
            {
                return (bool)this.GetValue(EnabledProperty);
            }
            set
            {

                this.SetValue(EnabledProperty, BooleanBoxes.Box(value));
            }
        }

        public static RadProperty CanFocusProperty = RadProperty.Register(
            "CanFocus",
            typeof(bool),
            typeof(RadElement),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

        /// <summary>
        /// Gets or sets a value indicating whether the element can receive input
        /// focus.
        /// </summary>
        [Description("Gets or sets value indicating whether an element can receive input focus.")]
        [RadPropertyDefaultValue("CanFocus", typeof(RadElement)), Category(RadDesignCategory.BehaviorCategory)]
        public virtual bool CanFocus
        {
            get
            {
                return (bool)this.GetValue(CanFocusProperty);
            }
            set
            {
                this.SetValue(CanFocusProperty, BooleanBoxes.Box(value));
            }
        }

        public static RadProperty IsItemFocusedProperty =
            RadProperty.Register("IsItemFocused", typeof(bool), typeof(RadElement),
                new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty IsFocusedProperty = RadProperty.Register(
            "IsFocused",
            typeof(bool),
            typeof(RadElement),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

        /// <summary>
        /// Gets a value indicating whether the element has input focus.
        /// </summary>
        [Description("Gets a value indicating whether the element has input focus.")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsFocused
        {
            get
            {
                return (bool)this.GetValue(IsFocusedProperty);
            }
        }

        public static RadProperty IsMouseOverProperty =
            RadProperty.Register("IsMouseOver", typeof(bool), typeof(VisualElement),
                new RadElementPropertyMetadata(false,
                ElementPropertyOptions.None));

        /// <summary>
        /// Gets or sets a value indicating whether the mouse has entered the bounds of the
        /// element or any of its sibling elements in the parent RadItem.
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsMouseOver
        {
            get
            {
                return (bool)this.GetValue(IsMouseOverProperty);
            }
            set
            {
                this.SetValue(IsMouseOverProperty, BooleanBoxes.Box(value));
            }
        }

        public static RadProperty IsMouseOverElementProperty =
            RadProperty.Register("IsMouseOverElement", typeof(bool), typeof(RadElement),
                new RadElementPropertyMetadata(false,
                ElementPropertyOptions.None));

        /// <summary>
        /// Gets or sets a value indicating whether the mouse has entered the bounds of the
        /// element.
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsMouseOverElement
        {
            get
            {
                return (bool)this.GetValue(IsMouseOverElementProperty);
            }
            set
            {
                this.SetValue(IsMouseOverElementProperty, BooleanBoxes.Box(value));
            }
        }

        public static RadProperty IsMouseDownProperty =
            RadProperty.Register("IsMouseDown", typeof(bool), typeof(VisualElement),
                new RadElementPropertyMetadata(false,
                ElementPropertyOptions.AffectsDisplay));

        /// <summary>
        /// Gets or sets a value indicating whether the mouse button has been pressed when
        /// inside the bounds of the element.
        /// </summary>
        [Browsable(false),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsMouseDown
        {
            get
            {
                return (bool)this.GetValue(IsMouseDownProperty);
            }
            set
            {
                this.SetValue(IsMouseDownProperty, BooleanBoxes.Box(value));
            }
        }

        /// <summary>
        /// Provide for use within TelerikLayoutEngine.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool InvalidateChildrenOnChildChanged
        {
            get
            {
                return this.GetBitState(InvalidateChildrenOnChildChangedStateKey);
            }
            set
            {
                this.SetBitState(InvalidateChildrenOnChildChangedStateKey, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the element takes into consideration its
        /// children' Alignment properties when calculating their positions.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Obsolete("Alignment cannot be disabled anymore (use TopLeft to avoid alignment offset)")]
        public virtual bool SuppressChildrenAlignment
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region Visibility

        public static readonly RadProperty ShouldPaintProperty =
            RadProperty.Register("ShouldPaint", typeof(bool), typeof(RadElement),
                new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

        /// <summary>Gets or sets a value indicating whether the element should be painted.</summary>
        /// <remarks>
        /// Children visibility is not be affected.
        /// </remarks>
        [Description("Indicates whether the element should be painted. Children visibility would not be affected.")]
        [RadPropertyDefaultValue("ShouldPaint", typeof(RadElement)), Category(RadDesignCategory.AppearanceCategory)]
        public virtual bool ShouldPaint
        {
            get
            {
                return (bool)this.GetValue(ShouldPaintProperty);
            }
            set
            {
                this.SetValue(ShouldPaintProperty, BooleanBoxes.Box(value));
            }
        }

        public static RadProperty VisibilityProperty =
            RadProperty.Register("Visibility", typeof(ElementVisibility), typeof(RadElement),
                new RadElementPropertyMetadata(ElementVisibility.Visible, ElementPropertyOptions.AffectsDisplay));

        /// <summary>Gets or sets a value indicating element visibility.</summary>
        /// <remarks>
        /// Setting this property affects also the children of the element. Collapsed means the element and its children would not be painted and would not be 
        /// calculated in the layout.
        /// This property has no effect in design-time on <c ref="RadItem"/> objects.
        /// </remarks>
        [Description("Indicates element visibility, affecting also its children. Collapsed means the element and its children would not be painted and would not be calculated in the layout. This property has no effect in design-time on RadItem objects.")]
        [RadPropertyDefaultValue("Visibility", typeof(RadElement)), Category(RadDesignCategory.AppearanceCategory)]
        public ElementVisibility Visibility
        {
            get
            {
                return (ElementVisibility)this.GetValue(VisibilityProperty);
            }
            set
            {
                this.SetValue(VisibilityProperty, VisibilityBoxes.Box(value));
            }
        }

        /// <summary>Gets a value indicating if the element is visible.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool IsElementVisible
        {
            get
            {
                ElementVisibility visible = (ElementVisibility)this.GetValue(VisibilityProperty);
                if (visible == ElementVisibility.Visible)
                {
                    if (this.Parent != null)
                    {
                        return parent.IsElementVisible;
                    }
                }
                else
                {
                    return false;
                }

                return true;
            }
        }

        public static RadProperty StyleProperty =
            RadProperty.Register("Style", typeof(StyleSheet), typeof(RadElement),
                new RadElementPropertyMetadata(null,
                ElementPropertyOptions.None));
        /// <summary>
        /// Gets or sets the stylesheet associated with the element.
        /// </summary>
        /// <remarks>
        /// Stylesheets provide dynamic property settings for elements' RadProperties organized into groups, each regarding a 
        /// certain state of the element. State means a property has certain value. 
        /// Style of an element can affect also element children.
        /// Generally element style is set through control theme, which is a holder for various styles for many controls.
        /// </remarks>
        [Browsable(true), Category(RadDesignCategory.StyleSheetCategory)]
        [Description("Gets or sets the stylesheet associated with the element.")]
        [TypeConverter(typeof(ComponentConverter))]
        [DefaultValue(null)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StyleSheet Style
        {
            get
            {
                return (StyleSheet)this.GetValue(StyleProperty);
            }
            set
            {
                this.SetValue(StyleProperty, value);
            }
        }

        /// <summary>
        /// Searches up the parent chain for finding the first set StyleSheet.
        /// </summary>
        [Browsable(false)]
        public StyleSheet ParentStyle
        {
            get
            {
                StyleSheet style = this.Style;
                if (style != null)
                {
                    return style;
                }

                RadElement parent = this.parent;
                while (parent != null)
                {
                    style = parent.Style;
                    if (style != null)
                    {
                        break;
                    }

                    parent = parent.parent;
                }

                return style;
            }
        }

        public static RadProperty NameProperty =
            RadProperty.Register("Name", typeof(string), typeof(RadElement),
                new RadElementPropertyMetadata(String.Empty,
                ElementPropertyOptions.None));

        /// <summary>Represents the element unique name.</summary>
        [RadPropertyDefaultValue("Name", typeof(RadElement)), Category(RadDesignCategory.StyleSheetCategory)]
        [Browsable(false)]
        [StyleBuilderReadOnly]
        public virtual string Name
        {
            get
            {
                return (string)this.GetValue(NameProperty);
            }
            set
            {
                this.SetValue(NameProperty, value);
            }
        }

        public static RadProperty ClassProperty =
            RadProperty.Register("Class", typeof(string), typeof(RadElement),
                new RadElementPropertyMetadata(String.Empty,
                ElementPropertyOptions.AffectsTheme));

        /// <summary>
        ///     Gets or sets a string value indicating the element visual class name. It's used
        ///     when a stylesheet (<see cref="Style"/>) has been applied to this element.
        /// </summary>
        /// <remarks>
        /// Style sheets contain groups of property settings categorized by element type and/or class, thus 
        /// element "class" is used to determine whether certain style rule would be applied over an element.
        /// Generally this property is assigned by the control developer but it can be changed design time or runtime if 
        /// certain element is decided to have different style class.
        /// </remarks>
        [RadPropertyDefaultValue("Class", typeof(RadElement)), Category(RadDesignCategory.StyleSheetCategory)]
        [StyleBuilderReadOnly]
        public string Class
        {
            get
            {
                return (string)this.GetValue(ClassProperty);
            }
            set
            {
                this.SetValue(ClassProperty, value);
            }
        }

        public static RadProperty ClipDrawingProperty =
            RadProperty.Register("ClipDrawing", typeof(bool), typeof(RadElement),
                new RadElementPropertyMetadata(false,
                ElementPropertyOptions.AffectsDisplay));

        /// <summary>
        /// Indicates whether the painting of the element and its children should be
        /// restricted to its bounds.
        /// </summary>
        [Description("Indicated whether the painting of the element and its children should be restricted to its bounds /through clipping/. Some elements like FillPrimitive apply clipping always, ignoring this property value.")]
        [Category(RadDesignCategory.AppearanceCategory)]
        public bool ClipDrawing
        {
            get
            {
                return (bool)this.GetValue(ClipDrawingProperty);
            }
            set
            {
                this.SetValue(ClipDrawingProperty, BooleanBoxes.Box(value));
            }
        }

        public static RadProperty ShapeProperty =
            RadProperty.Register("Shape", typeof(ElementShape), typeof(RadElement),
                new RadElementPropertyMetadata(null,
                ElementPropertyOptions.AffectsDisplay));

        /// <summary>
        /// Gets or sets an instance of the Shape object of an element. The shape of the
        /// element is both responsible for clipping the element's children and for providing its'
        /// border(s) with custom shape.
        /// </summary>
        /// <remarks>
        /// Value of null (or Nothing in VisualBasic.Net) indicates that element has rectangular (or no) shape.
        /// Shape is an object that defines the bounding graphics path of an element. Graphics clip is always applied when an element has shape.
        /// Shape is considered when painting the border element, and when hit-testing an element.
        /// Some predefined shapes are available, like <see cref="RoundRectShape"/> or <see cref="EllipseShape"/>.
        /// <see cref="CustomShape"/> offers a way to specify element's shape with a sequance of points and curves using code 
        /// or the design time <see cref="ElementShapeEditor"/>
        /// 	<see cref="UITypeEditor"/>.
        /// </remarks>
        [Description("Represents the shape of the element. Setting shape affects also border element painting. Shape is considered when painting, clipping and hit-testing an element.")]
        [RadPropertyDefaultValue("Shape", typeof(RadElement))]
        [TypeConverter(typeof(ElementShapeConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Category(RadDesignCategory.AppearanceCategory)]
        public ElementShape Shape
        {
            get
            {
                return (ElementShape)this.GetValue(ShapeProperty);
            }
            set
            {
                this.SetValue(ShapeProperty, value);
            }
        }

        #endregion

        #endregion

        #endregion

        #region Routed Events

        /// <summary>
        /// Tunnels and bubbles on MouseClick on current element
        /// </summary>
        public static RoutedEvent MouseClickedEvent = RadElement.RegisterRoutedEvent("MouseClickedEvent", typeof(RadElement));

        /// <summary>
        /// Tunnels and bubbles on MouseDoubleClick on current element
        /// </summary>
        public static RoutedEvent MouseDoubleClickedEvent = RadElement.RegisterRoutedEvent("MouseDoubleClickedEvent", typeof(RadElement));

        /// <summary>
        /// Tunnels and bubbles on MouseDown on current element
        /// </summary>
        public static RoutedEvent MouseDownEvent = RadElement.RegisterRoutedEvent("MouseDownEvent", typeof(RadElement));

        /// <summary>
        /// Tunnels and bubbles on MouseUp on current element
        /// </summary>
        public static RoutedEvent MouseUpEvent = RadElement.RegisterRoutedEvent("MouseUpEvent", typeof(RadElement));

        /// <summary>
        /// Routed event key for ChildElementAdded event. Bubbles when element is added
        /// </summary>
        public static RoutedEvent ChildElementAddedEvent = RadElement.RegisterRoutedEvent("ChildElementAddedEvent", typeof(RadElement));

        /// <summary>
        /// Routed event key for ParentChanged event. Tunnels when element parent changes
        /// </summary>
        public static RoutedEvent ParentChangedEvent = RadElement.RegisterRoutedEvent("ParentChangedEvent", typeof(RadElement));

        /// <summary>
        /// Tunnels when bounds changed in order to notify any children that should take special actions in this case - like RadHostItem.
        /// </summary>
        public static RoutedEvent BoundsChangedEvent = RadElement.RegisterRoutedEvent("BoundsChangedEvent", typeof(RadElement));

        /// <summary>
        /// Tunnels and bubbles when changes the current element
        /// </summary>
        public static RoutedEvent VisibilityChangingEvent = RadElement.RegisterRoutedEvent("VisibilityChangingEvent", typeof(RadElement));

        /// <summary>
        /// Tunnels when the Enabled property changes in order to notify any children that should take special actions.
        /// </summary>
        public static RoutedEvent EnabledChangedEvent = RadElement.RegisterRoutedEvent("EnabledChangedEvent", typeof(RadElement));

        /// <summary>
        /// Tunnels when the winforms control has been changed.
        /// </summary>
        public static RoutedEvent ControlChangedEvent = RadElement.RegisterRoutedEvent("ControlChangedEvent", typeof(RadElement));

        #endregion

        #region Disposing

        protected override void DisposeManagedResources()
        {
            if (this.elementTree != null && !this.elementTree.Disposing)
            {
                RadControl control = this.elementTree.Control as RadControl;
                if (control != null && control.Behavior.currentFocusedElement == this)
                {
                    control.Behavior.currentFocusedElement.SetElementFocused(false);
                    control.Behavior.currentFocusedElement = null;
                    control.Behavior.lastFocusedElement = null;
                }
                this.elementTree.NotifyElementDisposed(this.parent, this);
            }

            if (this.parent != null)
            {
                int index = this.parent.children.IndexOf(this);
                if (index != -1)
                {
                    this.parent.children.RemoveAt(index);
                }
            }

            //clear animators
            foreach (ElementValuesAnimator animator in this.ValuesAnimators.Values)
            {
                animator.OnElementDisposed();
            }
            this.ValuesAnimators.Clear();
            this.ValuesAnimators = null;

            this.IsStyleSelectorValueSet.Clear();
            this.IsStyleSelectorValueSet = null;

            //clear behaviors
            this.behaviors.Clear();
            this.layoutEvents.Clear();

            this.DetachFromElementTree(this.elementTree);

            if (this.zOrderedChildren != null)
            {
                this.zOrderedChildren.Clear();
            }

            //do not forget to remove locally set Shape instance
            RadPropertyValue shapeProp = this.GetPropertyValue(ShapeProperty);
            if (shapeProp != null && shapeProp.ValueSource == ValueSource.Local)
            {
                ElementShape shape = shapeProp.GetCurrentValue(false) as ElementShape;
                if (shape != null)
                {
                    shape.Dispose();
                }
            }            

            base.DisposeManagedResources();
        }

        protected override void PerformDispose(bool disposing)
        {
            if (this.state != ElementState.Disposing)
            {
                this.OnBeginDispose();
                //we are disposing branch of the tree, not entire control, so delegate dispose start down the subtree
                foreach (RadElement element in this.ChildrenHierarchy)
                {
                    element.OnBeginDispose();
                }
            }

            //dispose children recursively
            this.DisposeChildren();
            base.PerformDispose(disposing);

            this.elementTree = null;
            this.layoutManager = null;
            this.state = ElementState.Disposed;
        }

        public void DisposeChildren()
        {
            int count = this.children.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                this.children[i].Dispose();
            }
        }

        #endregion

        #region Rad Properties

        public static RadProperty BackgroundShapeProperty = RadProperty.Register(
            "BackgroundShape",
           typeof(RadImageShape),
           typeof(RadElement),
           new RadElementPropertyMetadata(null, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty ContainsFocusProperty = RadProperty.Register("ContainsFocus",
            typeof(bool),
            typeof(RadElement),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.None));

        public static RadProperty ContainsMouseProperty = RadProperty.Register("ContainsMouse",
            typeof(bool),
            typeof(RadElement),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.None));

        /// <summary>
        /// Used by RadControlSpy to display certain hidden properties in the Property Grid.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static RadProperty IsEditedInSpyProperty =
            RadProperty.Register("IsEditedInSpy",
            typeof(bool), typeof(RadElement),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.None));

        /// <summary>
        /// Get or sets the maximum size to apply on an element when layout is calculated.
        /// </summary>
        public static RadProperty MaxSizeProperty =
            RadProperty.Register("MaxSize", typeof(Size), typeof(RadElement),
                new RadElementPropertyMetadata(Size.Empty,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));

        public static RadProperty RightToLeftProperty =
            RadProperty.Register("RightToLeft", typeof(bool), typeof(RadElement),
                new RadElementPropertyMetadata(false,
                ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.CanInheritValue));

        public static RadProperty AutoSizeProperty =
            RadProperty.Register("AutoSize", typeof(bool), typeof(RadElement),
                new RadElementPropertyMetadata(true, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));

        /// <summary>
        /// Property key of the ZIndex Property. 
        /// </summary>
        public static RadProperty ZIndexProperty =
           RadProperty.Register("ZIndex", typeof(int), typeof(RadElement),
               new RadElementPropertyMetadata(0, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty AngleTransformProperty =
            RadProperty.Register("AngleTransform", typeof(float), typeof(RadElement),
                new RadElementPropertyMetadata(0f, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout
                        | ElementPropertyOptions.AffectsMeasure));

        public static RadProperty ScaleTransformProperty =
            RadProperty.Register("ScaleTransform", typeof(SizeF), typeof(RadElement),
                new RadElementPropertyMetadata(new SizeF(1, 1), ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.AffectsMeasure));

        public static RadProperty PositionOffsetProperty =
            RadProperty.Register("PositionOffset", typeof(SizeF), typeof(RadElement),
                new RadElementPropertyMetadata(System.Drawing.SizeF.Empty, ElementPropertyOptions.AffectsDisplay));

        public static readonly RadProperty TagProperty = RadProperty.Register(
            "Tag", typeof(object), typeof(RadElement), new RadElementPropertyMetadata(
                (string)null, ElementPropertyOptions.None));

        public readonly static RadProperty StretchHorizontallyProperty =
            RadProperty.Register("StretchHorizontally", typeof(bool), typeof(RadElement),
                new RadElementPropertyMetadata(BooleanBoxes.TrueBox,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsParentArrange));

        public readonly static RadProperty StretchVerticallyProperty =
            RadProperty.Register("StretchVertically", typeof(bool), typeof(RadElement),
                new RadElementPropertyMetadata(BooleanBoxes.TrueBox,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsParentArrange));

        public static RadProperty UseCompatibleTextRenderingProperty =
            RadProperty.Register("UseCompatibleTextRendering", typeof(bool), typeof(RadElement),
                new RadElementPropertyMetadata(BooleanBoxes.TrueBox,
                ElementPropertyOptions.InvalidatesLayout |
                ElementPropertyOptions.AffectsLayout |
                ElementPropertyOptions.CanInheritValue |
                ElementPropertyOptions.AffectsMeasure |
                ElementPropertyOptions.AffectsArrange |
                ElementPropertyOptions.AffectsDisplay |
                ElementPropertyOptions.AffectsParentArrange |
                ElementPropertyOptions.AffectsParentMeasure |
                ElementPropertyOptions.AffectsTheme));

        #endregion

        #region CLR Properties

        /// <summary>
        /// Gets or sets the RadImageShape that describes the background of the element.
        /// </summary>
        [Browsable(false)]
        [VsbBrowsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadImageShape BackgroundShape
        {
            get
            {
                return (RadImageShape)this.GetValue(BackgroundShapeProperty);
            }
            set
            {
                this.SetValue(BackgroundShapeProperty, value);
            }
        }

        /// <summary>
        /// Determines whether the element or one of its descendants currently contains the mouse.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ContainsMouse
        {
            get
            {
                return (bool)this.GetValue(ContainsMouseProperty);
            }
            internal set
            {
                this.SetValue(ContainsMouseProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the scale transform factors, when paintin the
        /// element and its children.
        /// </summary>
        [Description("Scale factors for painting the element and its children.")]
        [RadPropertyDefaultValue("ScaleTransform", typeof(RadElement)), Category(RadDesignCategory.AppearanceCategory)]
        public virtual SizeF ScaleTransform
        {
            get
            {
                return (SizeF)this.GetValue(ScaleTransformProperty);
            }
            set
            {
                this.SetValue(ScaleTransformProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the rotation transform angle used when painting the element and its
        /// children.
        /// </summary>
        [Description("Rotation transform angle for painting the element and its children.")]
        [RadPropertyDefaultValue("AngleTransform", typeof(RadElement)), Category(RadDesignCategory.AppearanceCategory),
       Localizable(true)]
        public virtual float AngleTransform
        {
            get
            {
                return (float)this.GetValue(AngleTransformProperty);
            }
            set
            {
                this.SetValue(AngleTransformProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the offset of the origin of the coordinate system used when 
        /// painting the element and its children.
        /// </summary>
        /// <remarks>
        /// TrnslateTransform of the graphics is used prior to painting the elemen and after painting element children, 
        /// to reset the transformation
        /// </remarks>
        [Description("Offset of the origin of the coordinate system used when painting the element and its children.")]
        [RadPropertyDefaultValue("PositionOffset", typeof(RadElement)), Category(RadDesignCategory.AppearanceCategory)]
        public virtual SizeF PositionOffset
        {
            get
            {
                return (SizeF)this.GetValue(PositionOffsetProperty);
            }
            set
            {
                this.SetValue(PositionOffsetProperty, value);
            }
        }

        /// <summary>
        ///		Gets or sets whether the properties of this element should be serialized
        /// </summary>
        [Browsable(true), Category("Design")]
        [Description("Gets or sets whether the properties of this element should be serialized via CodeDom")]
        [DefaultValue(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual bool SerializeProperties
        {
            get
            {
                return this.GetBitState(SerializePropertiesStateKey);
            }
            set
            {
                this.SetBitState(SerializePropertiesStateKey, value);
            }
        }

        /// <summary>
        ///		Gets or sets whether the element should be serialized in designer
        /// </summary>
        [Browsable(false), Category("Design")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets or sets whether this element should be serialized via CodeDom")]
        [DefaultValue(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual bool SerializeElement
        {
            get
            {
                return this.GetBitState(SerializeElementStateKey);
            }
            set
            {
                this.SetBitState(SerializeElementStateKey, value);
            }
        }

        /// <summary>
        ///		Gets or sets whether the children of this element should be serialized
        /// </summary>
        [Browsable(true), Category("Design")]
        [Description("Gets or sets whether the children of this element should be serialized via CodeDom")]
        [DefaultValue(true), EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual bool SerializeChildren
        {
            get
            {
                return this.GetBitState(SerializeChildrenStateKey);
            }
            set
            {
                this.SetBitState(SerializeChildrenStateKey, value);
            }
        }

        /// <summary>Gets or sets a value indicating maximum rendered frames per second.</summary>
        public static int RenderingMaxFramerate
        {
            get
            {
                if (animationMaxFramerate < 1 || animationMaxFramerate > 1000)
                {
                    return 200;
                }

                return RadElement.animationMaxFramerate;
            }
            set
            {
                RadElement.animationMaxFramerate = value;
            }
        }

        /// <summary>
        /// Gets a value indicationg if theme finished applying
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsThemeApplied
        {
            get
            {
                return this.GetBitState(IsThemeAppliedStateKey);
            }
            internal set
            {
                this.SetBitState(IsThemeAppliedStateKey, value);
            }
        }

        [Category("Data"),
        Localizable(false),
        Bindable(true),
        DefaultValue((string)null),
        TypeConverter(typeof(StringConverter)),
        Description("Tag object that can be used to store user data, corresponding to the element")]
        public object Tag
        {
            get
            {
                return this.GetValue(RadElement.TagProperty);
            }
            set
            {
                this.SetValue(RadElement.TagProperty, value);
            }
        }

        [Description("Allow stretching in horizontal direction")]
        [RadPropertyDefaultValue("StretchHorizontally", typeof(RadElement))]
        [Category(RadDesignCategory.LayoutCategory)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [NotifyParentProperty(true)]
        public virtual bool StretchHorizontally
        {
            get
            {
                return (bool)this.GetValue(StretchHorizontallyProperty);
            }
            set
            {
                this.SetValue(StretchHorizontallyProperty, value);
            }
        }

        [Description("Allow stretching in vertical direction")]
        [RadPropertyDefaultValue("StretchVertically", typeof(RadElement))]
        [Category(RadDesignCategory.LayoutCategory)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [NotifyParentProperty(true)]
        public virtual bool StretchVertically
        {
            get
            {
                return (bool)this.GetValue(StretchVerticallyProperty);
            }
            set
            {
                this.SetValue(StretchVerticallyProperty, value);
            }
        }

        #endregion

        #region Fields

        private ComponentThemableElementTree elementTree;
        private ILayoutManager layoutManager;
        private RadElement parent;
        private ElementState state;

        //cached values
        private Rectangle cachedBounds = LayoutUtils.InvalidBounds;
        private Rectangle cachedFaceRectangle = Rectangle.Empty;
        private Rectangle cachedFullRectangle = Rectangle.Empty;
        private Rectangle cachedBoundingRectangle = Rectangle.Empty;
        private Rectangle cachedControlBoundingRectangle = Rectangle.Empty;

        //collections
        private RadElementCollection children;
        private RadElementZOrderCollection zOrderedChildren;
        internal HybridDictionary IsStyleSelectorValueSet = new HybridDictionary();
        internal HybridDictionary ValuesAnimators = new HybridDictionary();
        private List<EventHandler> layoutEvents = new List<EventHandler>();
        private RoutedEventBehaviorCollection routedEventBehaviors = new RoutedEventBehaviorCollection();
        private Dictionary<RadElement, ElementLayoutData> suspendedChildren;
        private ArrayList processedEvents = new ArrayList();
        private PropertyChangeBehaviorCollection behaviors = new PropertyChangeBehaviorCollection();

        //transformations & painting
        private RadMatrix transform;
        private RadMatrix totalTransform;

        //layout
        private byte suspendReferenceUpdate;
        private byte treeLevel;
        private SizeF _previousAvailableSize;
        private RectangleF _finalRect;
        private SizeBox UnclippedDesiredSize;
        private SizeF _desiredSize;
        private PointF layoutOffset;

        internal SizeChangedInfo sizeChangedInfo;
        internal ContextLayoutManager.LayoutQueue.Request ArrangeRequest;
        internal ContextLayoutManager.LayoutQueue.Request MeasureRequest;

        internal byte layoutsRunning = 0;
        internal Size cachedSize = LayoutUtils.InvalidSize;
        internal Size cachedBorderSize = LayoutUtils.InvalidSize;
        internal Size cachedBorderOffset = LayoutUtils.InvalidSize;

        private byte layoutSuspendCount = 0;
        private byte boundsLocked = 0;
        private Size coercedSize = LayoutUtils.InvalidSize;
        private ILayoutEngine layoutEngine = null;

        internal PerformLayoutType PerformLayoutAfterFinishLayout = PerformLayoutType.None;

        private byte suspendThemeRefresh = 0;
        private byte suspendRecursiveEnabledUpdates = 0;

        //OS skin painting
        private UseSystemSkinMode useSystemSkin;
        protected internal bool? paintSystemSkin;

        #endregion

        #region Static Members

        private static HybridDictionary registeredRoutedEvents;
        internal static bool TraceInvalidation = false;
        private static int animationMaxFramerate = 25;

        public readonly static SetPropertyValueCommand SetPropertyValueCommand;
        public readonly static GetPropertyValueCommand GetPropertyValueCommand;

        //event keys
        private static readonly object MouseMoveEventKey;
        private static readonly object MouseDownEventKey;
        private static readonly object MouseUpEventKey;
        private static readonly object MouseEnterEventKey;
        private static readonly object MouseLeaveEventKey;
        private static readonly object EnabledChangedEventKey;
        private static readonly object MouseHoverEventKey;

        //Removed in favor of RadItem-MouseWheel
        //private static readonly object MouseWheelEventKey;

        private static readonly object ChildrenChangedKey;

        #endregion

        #region BitState Keys

        internal const ulong UseIdentityMatrixStateKey = RadObjectLastStateKey << 1;
        internal const ulong InvalidateMeasureOnRemoveStateKey = UseIdentityMatrixStateKey << 1;
        internal const ulong NotifyParentOnMouseInputStateKey = InvalidateMeasureOnRemoveStateKey << 1;
        internal const ulong ShouldHandleMouseInputStateKey = NotifyParentOnMouseInputStateKey << 1;
        internal const ulong ProtectFocusedPropertyStateKey = ShouldHandleMouseInputStateKey << 1;
        internal const ulong UseNewLayoutStateKey = ProtectFocusedPropertyStateKey << 1;
        internal const ulong BypassLayoutPoliciesStateKey = UseNewLayoutStateKey << 1;
        internal const ulong InvalidateOnArrangeStateKey = BypassLayoutPoliciesStateKey << 1;
        internal const ulong ArrangeDirtyStateKey = InvalidateOnArrangeStateKey << 1;
        internal const ulong ArrangeInProgressStateKey = ArrangeDirtyStateKey << 1;
        internal const ulong NeverArrangedStateKey = ArrangeInProgressStateKey << 1;
        internal const ulong MeasureDirtyStateKey = NeverArrangedStateKey << 1;
        internal const ulong MeasureDuringArrangeStateKey = MeasureDirtyStateKey << 1;
        internal const ulong MeasureInProgressStateKey = MeasureDuringArrangeStateKey << 1;
        internal const ulong NeverMeasuredStateKey = MeasureInProgressStateKey << 1;
        internal const ulong OverridesDefaultLayoutStateKey = NeverMeasuredStateKey << 1;
        internal const ulong IsDelayedSizeStateKey = OverridesDefaultLayoutStateKey << 1;
        internal const ulong IsLayoutPendingStateKey = IsDelayedSizeStateKey << 1;
        internal const ulong IsLayoutInvalidatedStateKey = IsLayoutPendingStateKey << 1;
        internal const ulong InvalidateChildrenOnChildChangedStateKey = IsLayoutInvalidatedStateKey << 1;
        internal const ulong IsPendingInvalidateStateKey = InvalidateChildrenOnChildChangedStateKey << 1;
        internal const ulong IsPendingLayoutInvalidatedStateKey = IsPendingInvalidateStateKey << 1;
        internal const ulong IsPerformLayoutRunningStateKey = IsPendingLayoutInvalidatedStateKey << 1;
        internal const ulong IsThemeAppliedStateKey = IsPerformLayoutRunningStateKey << 1;
        internal const ulong SerializePropertiesStateKey = IsThemeAppliedStateKey << 1;
        internal const ulong SerializeChildrenStateKey = SerializePropertiesStateKey << 1;
        internal const ulong SerializeElementStateKey = SerializeChildrenStateKey << 1;
        internal const ulong HideFromElementHierarchyEditorStateKey = SerializeElementStateKey << 1;
        internal const ulong ShouldPaintChildrenStateKey = HideFromElementHierarchyEditorStateKey << 1;

        internal const ulong RadElementLastStateKey = ShouldPaintChildrenStateKey;

        #endregion

        #region Nested Types

        private class ProcessedEvent
        {
            RadElement sender;

            public RadElement Sender
            {
                get { return sender; }
                /*
                                set { sender = value; }
                */
            }
            RoutedEventArgs args;

            public RoutedEventArgs Args
            {
                get { return args; }
                /*
                                set { args = value; }
                */
            }

            public ProcessedEvent(RadElement sender, RoutedEventArgs args)
            {
                this.sender = sender;
                this.args = args;
            }
        }

        internal struct MinMax
        {
            internal float minWidth;
            internal float maxWidth;
            internal float minHeight;
            internal float maxHeight;
            internal MinMax(RadElement e)
            {
                Size max = e.MaxSize;
                Size min = e.MinSize;

                float tempMaxHeight = max.Height > 0 ? max.Height : float.PositiveInfinity;
                float tempMaxWidth = max.Width > 0 ? max.Width : float.PositiveInfinity;
                float tempMinHeight = min.Height > 0 ? min.Height : 0;
                float tempMinWidth = min.Width > 0 ? min.Width : 0;

                this.maxHeight = Math.Max(tempMaxHeight, tempMinHeight);
                this.minHeight = Math.Min(tempMaxHeight, tempMinHeight);
                this.maxWidth = Math.Max(tempMaxWidth, tempMinWidth);
                this.minWidth = Math.Min(tempMaxWidth, tempMinWidth);
            }
        }

        private class LayoutTransformData
        {
            internal RadMatrix transform;
            internal SizeF UntransformedDS;

            // Methods
            internal void CreateTransformSnapshot(RadMatrix sourceTransform)
            {
                this.transform = sourceTransform;
            }
        }

        private class SizeBox
        {
            private float height;
            internal float Height
            {
                get { return this.height; }
                set
                {
                    if (value < 0)
                        throw new ArgumentException("Width and Height cannot be Negative");
                    this.height = value;
                }
            }

            private float width;
            internal float Width
            {
                get { return this.width; }
                set
                {
                    if (value < 0)
                        throw new ArgumentException("Width and Height cannot be Negative");
                    this.width = value;
                }
            }

            internal SizeBox(SizeF size)
                : this(size.Width, size.Height)
            {
            }

            internal SizeBox(float width, float height)
            {
                if ((width < 0) || (height < 0))
                {
                    throw new ArgumentException("Width and Height cannot be Negative");
                }
                this.width = width;
                this.height = height;
            }
        }
        #endregion
    }
}
