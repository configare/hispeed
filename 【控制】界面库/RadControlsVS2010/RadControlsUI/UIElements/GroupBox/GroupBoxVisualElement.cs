using Telerik.WinControls.Primitives;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public class GroupBoxVisualElement : RadItem
    {
        #region Fields
        protected FillPrimitive fill;
        protected BorderPrimitive border; 
        #endregion

        #region Constructors
        static GroupBoxVisualElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new GroupBoxVisualElementStateManagerFactory(), typeof(GroupBoxVisualElement));
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the FillPrimitive contained in the Content area
        /// </summary>
        [Browsable(false)]
        public FillPrimitive Fill
        {
            get
            {
                return fill;
            }
        }
        #endregion

        #region Methods
        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.fill = new FillPrimitive();
            this.border = new BorderPrimitive();
            this.Children.Add(this.fill);
            this.Children.Add(this.border);
            this.fill.Class = "fill" + this.ToString();
            this.border.Class = "border" + this.ToString();
        } 
        #endregion     
    }
}