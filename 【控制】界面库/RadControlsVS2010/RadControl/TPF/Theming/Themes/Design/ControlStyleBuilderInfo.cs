using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Themes.Design
{
    ///<exclude/>
	public enum PreviewControlPlacemenet
	{ 
		TopLeft,
		MiddleLeft,
		TopCenter,
		TopRight,
		MiddleRight,
		BottomLeft,
		BottomCenter,
		MiddleCenter,
		BottomRight,
		ParentBottomLeft,
		Custom
	}

    [Obsolete("Provided for backward compatibility only. Please use ControlStyleBuilderInfo instead")]
    public class ThemeDesignedControl : ControlStyleBuilderInfo
    {
       public ThemeDesignedControl(RadControl previewControl, RadElement theamableRoot, string controlClassName, BuilderRegistrationType builderRegistrationType): base (previewControl, theamableRoot, controlClassName, builderRegistrationType)
       {
       }

        public ThemeDesignedControl(RadControl previewControl, RadElement theamableRoot): 
			base(previewControl, theamableRoot)
		{
		}

        public ThemeDesignedControl(IComponentTreeHandler previewControl, RadElement theamableRoot)
            : base(previewControl, theamableRoot)
        {            
        }

        public ThemeDesignedControl(RadControl previewControl, RadElement theamableRoot, string controlClassName, string mainElementClassName)
            : base (previewControl, theamableRoot, controlClassName, mainElementClassName)
        {
            
        }
    }

	public class ControlStyleBuilderInfo
	{
		//private RadControl previewControl;
	    private IComponentTreeHandler previewControl;
		private RadElement theamableRoot;
		private string mainElementClassName = null;
		private XmlStyleBuilderRegistration registration;
		private PreviewControlPlacemenet placemenet = PreviewControlPlacemenet.MiddleCenter;
		private ControlStyleBuilderInfo parent;
		private RadElement stylePreviewRoot;
		private IStyleDispatcher styleDispatcher;
        private List<RadElement> excludeStructureElementsAndHierarchy = new List<RadElement>();

		public ControlStyleBuilderInfo(RadControl previewControl, RadElement theamableRoot, string controlClassName, BuilderRegistrationType builderRegistrationType)
		{
			this.previewControl = previewControl;
			this.theamableRoot = theamableRoot;

			registration = new XmlStyleBuilderRegistration();
			RadStylesheetRelation relation = new RadStylesheetRelation();
			registration.StylesheetRelations.Add(relation);

			relation.RegistrationType = builderRegistrationType;
			relation.ElementType = theamableRoot.GetThemeEffectiveType().FullName;
			relation.ControlType = controlClassName;
		}

		public ControlStyleBuilderInfo(RadControl previewControl, RadElement theamableRoot): 
			this(previewControl, theamableRoot, (string)null)
		{
		}

        public ControlStyleBuilderInfo(IComponentTreeHandler previewControl, RadElement theamableRoot )
        {
            this.previewControl = previewControl;
            this.theamableRoot = theamableRoot;
            this.mainElementClassName = null;

            registration = new XmlStyleBuilderRegistration();

            RadStylesheetRelation relation = new RadStylesheetRelation();
            registration.StylesheetRelations.Add(relation);

            relation.RegistrationType = BuilderRegistrationType.ElementTypeControlType;
            relation.ElementType = theamableRoot.GetThemeEffectiveType().FullName;

            relation.ControlType = theamableRoot.ElementTree.ComponentTreeHandler.ThemeClassName;
        }

		public ControlStyleBuilderInfo(RadControl previewControl, RadElement theamableRoot, string mainElementClassName)
		{
			this.previewControl = previewControl;
			this.theamableRoot = theamableRoot;
			this.mainElementClassName = mainElementClassName;

			registration = new XmlStyleBuilderRegistration();

			RadStylesheetRelation relation = new RadStylesheetRelation();
			registration.StylesheetRelations.Add(relation);

			relation.RegistrationType = BuilderRegistrationType.ElementTypeControlType;
            relation.ElementType = theamableRoot.GetThemeEffectiveType().FullName;

			relation.ControlType = theamableRoot.ElementTree.ComponentTreeHandler.ThemeClassName;			
		}

		public ControlStyleBuilderInfo(RadControl previewControl, RadElement theamableRoot, string controlClassName, string mainElementClassName)
        {
            this.previewControl = previewControl;
            this.theamableRoot = theamableRoot;
			this.mainElementClassName = mainElementClassName;

            registration = new XmlStyleBuilderRegistration();

			RadStylesheetRelation relation = new RadStylesheetRelation();
			registration.StylesheetRelations.Add(relation);

			relation.RegistrationType = BuilderRegistrationType.ElementTypeControlType;
            relation.ElementType = theamableRoot.GetThemeEffectiveType().FullName;
			relation.ControlType = controlClassName;
        }

		public IComponentTreeHandler PreviewControl
		{
			get { return previewControl; }
			set { previewControl = value; }
		}

		public RadElement TheamableRoot
		{
			get { return theamableRoot; }
			set { theamableRoot = value; }
		}

		public XmlStyleBuilderRegistration Registration
		{
			get { return registration; }
			set { registration = value; }
		}

		public PreviewControlPlacemenet Placemenet
		{
			get { return placemenet; }
			set { placemenet = value; }
		}

		public ControlStyleBuilderInfo Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		public string MainElementClassName
		{
			get { return mainElementClassName; }
			set { this.mainElementClassName = value; }
		}

		public RadElement StylePreviewRoot
		{
			get 
			{
				if (this.stylePreviewRoot != null)
					return this.stylePreviewRoot;

				return this.PreviewControl.RootElement;
			}
			set { this.stylePreviewRoot = value; }
		}

		public IStyleDispatcher StyleDispatcher
		{
			get { return styleDispatcher; }
			set { styleDispatcher = value; }
		}

	    public List<RadElement> ExcludeStructureElementsAndHierarchy
	    {
	        get { return excludeStructureElementsAndHierarchy; }
	    }
	}

    public class ControlStyleBuilderInfoList : List<ControlStyleBuilderInfo>
	{
	}

    /// <summary>
    /// Type provided for backward compatibility only. Please use ControlStyleBuilderInfoList instead.
    /// </summary>
    public class ThemeDesignedControlList : ControlStyleBuilderInfoList
    {
    }
}
