using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;

namespace Telerik.WinControls
{
    /// <exclude/>
    [DesignTimeVisible(false)]
    [ToolboxItem(false), ComVisible(false)]
    public class RadComponentElement : VisualElement, IComponent, IDisposable, IBindableComponent
    {
        #region Overrides

        protected override void DisposeManagedResources()
        {
            if (this.site != null && this.site.DesignMode)
            {
                IDesignerHost host = this.site.GetService(typeof(IDesignerHost)) as IDesignerHost;
                if (host != null)
                {
                    try
                    {
                        host.DestroyComponent(this);
                    }
                    catch (InvalidOperationException)//fixed VS2010 inherit controls issue
                    {
                    }
                }
            }

            base.DisposeManagedResources();
        }

        public override string ToString()
        {
            ISite site1 = this.site;
            if (site1 != null)
            {
                return (site1.Name + " [" + base.GetType().FullName + "]");
            }
            return base.GetType().FullName;
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == BindingContextProperty)
            {
                UpdateBindings(e);
            }
        }

        #endregion

        protected virtual object GetService(Type service)
        {
            ISite site1 = this.site;
            if (site1 != null)
            {
                return site1.GetService(service);
            }
            return null;
        }

        private void UpdateBindings(RadPropertyChangedEventArgs e)
        {
            if (e.Property == RadObject.BindingContextProperty)
            {
                for (int i = 0; i < this.DataBindings.Count; i++)
                {
                    System.Windows.Forms.BindingContext.UpdateBinding(this.BindingContext, this.DataBindings[i]);
                }
            }
        }

        // Properties
        protected virtual bool CanRaiseEvents
        {
            get
            {
                return true;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public IContainer Container
        {
            get
            {
                ISite site1 = this.site;
                if (site1 != null)
                {
                    return site1.Container;
                }
                return null;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected bool DesignMode
        {
            get
            {
                ISite site1 = this.site;
                if (site1 != null)
                {
                    return site1.DesignMode;
                }
                return false;
            }
        }

        protected internal override bool IsDesignMode
        {
            get
            {
                if (this.site != null)
                {
                    return this.site.DesignMode;
                }

                return base.IsDesignMode;
            }
            set
            {
                if (this.site != null)
                {
                    value = this.site.DesignMode;
                }

                base.IsDesignMode = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual ISite Site
        {
            get
            {
                return this.site;
            }
            set
            {
                if (this.site == value)
                {
                    return;
                }

                this.site = value;
                if (this.site != null && this.site.DesignMode)
                {
                    this.SetIsDesignMode(true, true);
                }
                else
                {
                    this.SetIsDesignMode(false, true);
                }
            }
        }


        private ISite site;
        private ControlBindingsCollection bindingsCollection;

        #region IBindableComponent Members

        /// <summary>
        /// Gets the collection of data-binding objects for this IBindableComponent.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        ParenthesizePropertyName(true),
        RefreshProperties(RefreshProperties.All),
        Description("Gets the collection of data-binding objects for this IBindableComponent."),
        Category(RadDesignCategory.DataCategory)]
        public virtual ControlBindingsCollection DataBindings
        {
            get
            {
                if (this.bindingsCollection == null)
                {
                    this.bindingsCollection = new ControlBindingsCollection(this);
                }
                return this.bindingsCollection;
            }
        }

        #endregion

        //keep consistency in bit state keys declaration
        internal const ulong ComponentElementLastStateKey = VisualElementLastStateKey;
    }
}
