
namespace Telerik.WinControls.UI
{
    public class TreeAnimatedPropertySetting : AnimatedPropertySetting
    {
        #region Fields

        private RadTreeNode node;

        #endregion

        #region Constructor

        public TreeAnimatedPropertySetting(RadProperty property,
                                           object animationStartValue,
                                           object animationEndValue,
                                           int numFrames,
                                           int interval)
            : base(property, animationStartValue, animationEndValue, numFrames, interval)
        {

        }

        #endregion

        #region Properties

        public RadTreeNode Node
        {
            get
            {
                return this.node;
            }
            set
            {
                this.node = value;
            }
        }

        #endregion
    }
}
