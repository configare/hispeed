//using System;
//using System.Collections.Generic;
//using System.Text;
//using Telerik.WinControls.Layouts;

//namespace Telerik.WinControls.UI
//{
//    public class RadSizablePopupForm : RadPopupForm
//    {
//        // Fields
//        private SizeGripElement grip;
//        private DockLayoutPanel panel;

//        public RadSizablePopupForm()
//        {
//        }

//        public RadSizablePopupForm(PopupEditorBaseElement owner)
//            : base(owner)
//        {
//        }

//        public SizingMode SizingMode
//        {
//            get
//            {
//                this.EnsureChildItems();
//                return this.grip.SizingMode;
//            }
//            set
//            {
//                this.EnsureChildItems();
//                this.grip.SizingMode = value;
//            }
//        }

//        public RadElement ResizeGrip
//        {
//            get
//            {
//                this.EnsureChildItems();
//                return this.grip;
//            }
//        }

//        internal protected DockLayoutPanel DockLayoutPanel
//        {
//            get
//            {
//                this.EnsureChildItems();
//                return this.panel;
//            }
//        }

//        protected override void OnRightToLeftChanged(EventArgs e)
//        {
//            base.OnRightToLeftChanged(e);
//            //this.grip.RightToLeft = (this.RightToLeft == System.Windows.Forms.RightToLeft.Yes);
//        }

//        protected override void CreateChildItems(RadElement parent)
//        {
//            base.CreateChildItems(parent);
//            this.panel = new DockLayoutPanel();
//            this.panel.Class = "PopupPanel";
//            grip = new SizeGripElement();
//            grip.SizingMode = SizingMode.None;
//            grip.MinSize = new System.Drawing.Size(0, 12);
//            DockLayoutPanel.SetDock(grip, Telerik.WinControls.Layouts.Dock.Bottom);
//            //base.FormElement.ClientFillPrimitive.Children.Add(grip);
//            base.FormElement.ClientFillPrimitive.Children.Add(this.panel);
//            this.panel.Children.Add(grip);
//            //panel.Children.Add(grip);
//            //parent.Children.Add(this.panel);
//        }

//    }
//}
