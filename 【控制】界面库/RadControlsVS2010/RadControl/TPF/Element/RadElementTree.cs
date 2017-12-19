using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents RadElementTree. Every Telerik control has a corresponding tree of
    /// RadElements. This gives a lot of flexibility in building controls allowing, for
    /// example, inheritance of properties from the ancenstor nodes.
    /// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public abstract class RadElementTree
	{
		private RootRadElement root;
		private Control control;
        private IComponentTreeHandler component;

		private string treeName;

        /// <summary>Initializes a new instance of RadElementTree class.</summary>
		public RadElementTree(IComponentTreeHandler component)
		{
            this.component = component;
            this.control = component as Control;
			this.treeName = "VisualTree" + treeInstanceCount;
            treeInstanceCount++;

            this.CheckLicense();
		}

        private void CheckLicense()
        {
#if OEM
				if (RadControl.designTime == null) //&& this.component != null
				{
					RadControl.designTime = (Assembly.GetEntryAssembly() == null);
				}
				if(!RadControl.designTime.HasValue || RadControl.designTime.Value)
				{
					return;
				}
				if(RadControl.isOem)
				{
					return;
				}
#endif

#if (EVALUATION || OEM)
                if (RadControl.licenseCount == -1 && numberOfEvalMsgsShown < 3)
				{
					Random random = new Random();
					RadControl.licenseCount = random.Next(0, 10);
					if (RadControl.licenseCount == 1)
					{
                        Assembly uiAssembly = null;

                        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            if (assembly.FullName.Contains("Telerik.WinControls.UI"))
                            {
                                uiAssembly = assembly;
                                break;
                            }
                        }

                        if (uiAssembly != null)
                        {
                            Type evalFormType = uiAssembly.GetType("Telerik.WinControls.UI.Licensing.RadEvaluationForm");
                            object obj = Activator.CreateInstance(evalFormType);
                            object result = evalFormType.InvokeMember("ShowDialog", System.Reflection.BindingFlags.InvokeMethod, null, obj, null);
                        }
                        else
                        {
                            using (EvaluationForm form = new EvaluationForm())
                            {
                                form.ShowDialog();
                                
                            }
                        }
                        numberOfEvalMsgsShown++;
					}
				}
#endif
        }

        /// <summary>Gets the RootElement of the tree.</summary>
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [Description("Gets the RootElement of a Control.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RootRadElement RootElement
        {
            get
            {
                return this.root;
            }
        }

#if (EVALUATION || OEM)
        private static int numberOfEvalMsgsShown = 0;
#endif

        /// <summary>Gets or sets the RadControl for the corresponding tree.</summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Control Control
		{
			get
			{
				return this.control;
			}
			internal set
			{
				this.control = value;
                this.component = value as IComponentTreeHandler;
				

			}
		}

        /// <summary>Gets the bridge between the abstract RadElement layout and the RadControl instance.</summary>
        public IComponentTreeHandler ComponentTreeHandler
        {
            get
            {
                return this.component;
            }
        }

		private static int treeInstanceCount = 0;

        /// <summary>Gets the tree name.</summary>
		public string TreeName
		{
			get
			{
				return this.treeName;
			}
		}

        protected abstract void InitializeRootElement(RootRadElement rootElement);

        protected internal virtual void CreateChildItems(RadElement parent)
        {
            this.RootElement.Name = this.Control.Name;
        }

        protected virtual RootRadElement CreateRootElement()
        {
            return new RootRadElement();
        }

        /// <summary>Creates RootElement if the former does not exists.</summary>
        public void EnsureRootElement()
        {
            if (this.root == null)
            {
                this.root = this.CreateRootElement();
                this.root.layoutsRunning++;

                this.InitializeRootElement(this.root);
                this.root.ElementTree = this.component.ElementTree;
                this.root.SaveCurrentStretchModeAsDefault();
                this.root.RaiseTunnelEvent(this.root, new RoutedEventArgs(EventArgs.Empty, RadElement.ControlChangedEvent));

                this.root.layoutsRunning--;
            }
        }

        internal void ResetAmbientProperties()
        {
            this.root.ResetValue(VisualElement.BackColorProperty, ValueResetFlags.Inherited);
            this.root.ResetValue(VisualElement.ForeColorProperty, ValueResetFlags.Inherited);
            this.root.ResetValue(VisualElement.FontProperty, ValueResetFlags.Inherited);
        }
	}
}