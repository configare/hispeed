using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace Telerik.WinControls
{
    [DesignTimeVisible(false)]
    [ToolboxItem(false), ComVisible(false)]
    public class RadComponent : RadObject, IComponent, IDisposable
    {
        #region Fields

        private ISite site;
        private string themeName;

        #endregion

        #region Constructor/Destructor

        public RadComponent()
        {
        }

        ~RadComponent()
        {
            this.Dispose(false);
        }

        #endregion

        #region Overrides

        protected override void DisposeManagedResources()
        {
            if ((this.site != null) && (this.site.Container != null))
            {
                this.site.Container.Remove(this);
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

        #endregion

        #region IComponent Implementation

        protected virtual object GetService(Type service)
        {
            ISite site1 = this.site;
            if (site1 != null)
            {
                return site1.GetService(service);
            }
            return null;
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
                this.site = value;
                if (this.site != null)
                {
                    base.IsDesignMode = this.site.DesignMode;
                }
                else
                {
                    base.IsDesignMode = false;
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the theme name of the component.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the theme name of the component.")]
        [Editor(DesignerConsts.ThemeNameEditorString, typeof(UITypeEditor))]
        public virtual string ThemeName
        {
            get
            {
                return this.themeName;
            }
            set
            {
                if (this.themeName != value)
                {
                    this.themeName = value;
                    this.OnNotifyPropertyChanged("ThemeName");
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets an <see cref="IComponentTreeHandler"/> implementation
        /// which is owned by this component. This method is used
        /// by the ThemeNameEditor to prefilter
        /// the available themes for the current component.
        /// </summary>
        /// <returns>An <see cref="IComponentTreeHandler"/> implementation which
        /// is owned by this <see cref="RadComponent"/>.</returns>
        public virtual IComponentTreeHandler GetOwnedTreeHandler()
        {
            return null;
        }

        #endregion
    }
}
