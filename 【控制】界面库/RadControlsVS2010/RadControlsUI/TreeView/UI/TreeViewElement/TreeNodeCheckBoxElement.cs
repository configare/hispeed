using System;

namespace Telerik.WinControls.UI
{
    public class TreeNodeCheckBoxElement : RadCheckBoxElement
    {
        #region Properties

        protected override Type ThemeEffectiveType
        {
            get
            {
                return typeof(RadCheckBoxElement);
            }
        }

        public TreeNodeElement NodeElement
        {
            get { return this.FindAncestor<TreeNodeElement>(); }
        }


        #endregion

        #region Methods

        protected internal override void OnToggle()
        {
            if (this.IsThreeState)
            {
                base.OnToggle();
                return;
            }

            switch (this.ToggleState)
            {
                case Telerik.WinControls.Enumerations.ToggleState.Off:
                    this.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On;
                    break;

                case Telerik.WinControls.Enumerations.ToggleState.Indeterminate:
                case Telerik.WinControls.Enumerations.ToggleState.On:
                    this.ToggleState = Telerik.WinControls.Enumerations.ToggleState.Off;
                    break;
            }
        }

        protected override void OnToggleStateChanging(StateChangingEventArgs e)
        {
            base.OnToggleStateChanging(e);

            if (e.Cancel && e.NewValue == Telerik.WinControls.Enumerations.ToggleState.Indeterminate && !this.IsThreeState)
            {
                TreeNodeElement nodeElement = this.NodeElement;

                if (nodeElement != null && nodeElement.TreeViewElement.TriStateMode)
                {
                    e.Cancel = false;
                }
            }
        }

        #endregion
    }
}
