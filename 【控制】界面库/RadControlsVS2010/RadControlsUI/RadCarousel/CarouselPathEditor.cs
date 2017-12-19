using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Telerik.WinControls.UI.Carousel
{
    public class CarouselPathEditor: UITypeEditor
    {
        private IWindowsFormsEditorService editorService;
        private bool isDirty;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override bool IsDropDownResizable
        {
            get
            {
                return true;
            }
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {            
            editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            Control dropDownControl = CreateListBox(context, value);

            isDirty = false;

            editorService.DropDownControl(dropDownControl);

            if (!isDirty)
            {
                return value;
            }

            ListBox listBox = (ListBox)dropDownControl;

            if (listBox.SelectedIndex == 0 || listBox.SelectedItem == null)
            {
                return new CarouselEllipsePath();
            }

            CarouselPathListItem listItem = (CarouselPathListItem)listBox.SelectedItem;
            return listItem.PathInstance;            
        }

        private class CarouselPathListItem
        {
            private ICarouselPath pathInstance;
            public CarouselPathListItem(ICarouselPath pathInstance)
            {
                this.pathInstance = pathInstance;
            }

            public override string ToString()
            {
                return "new " + this.pathInstance.GetType().Name;
            }

            public ICarouselPath PathInstance
            {
                get { return this.pathInstance; }
            }
        }

        private Control CreateListBox(ITypeDescriptorContext context, object value)
        {
            ListBox listBox = new ListBox();            
            listBox.Dock = DockStyle.Fill;
            listBox.SelectedValueChanged += new EventHandler(listBox_SelectedValueChanged);
            listBox.BorderStyle = BorderStyle.None;
            listBox.ItemHeight = 13;

            listBox.Items.Add("(none)");

            CarouselBezierPath carouselBezierPath = new CarouselBezierPath();

            carouselBezierPath.CtrlPoint1 = new Telerik.WinControls.UI.Point3D(14.0, 76.0, 70);
            carouselBezierPath.CtrlPoint2 = new Telerik.WinControls.UI.Point3D(86.0, 76.0, 70);
            carouselBezierPath.FirstPoint = new Telerik.WinControls.UI.Point3D(10, 20, 0);
            carouselBezierPath.LastPoint = new Telerik.WinControls.UI.Point3D(90, 20, 0);

            CarouselEllipsePath carouselEllipsePath = new CarouselEllipsePath();

            carouselEllipsePath.Center = new Telerik.WinControls.UI.Point3D(50, 50, 0);
            carouselEllipsePath.FinalAngle = -100;
            carouselEllipsePath.InitialAngle = -90;
            carouselEllipsePath.U = new Telerik.WinControls.UI.Point3D(-20.0, -17.0, -50.0);
            carouselEllipsePath.V = new Telerik.WinControls.UI.Point3D(30.0, -25.0, -60.0);
            carouselEllipsePath.ZScale = 500;

            listBox.Items.Add(new CarouselPathListItem( carouselBezierPath ));
            listBox.Items.Add(new CarouselPathListItem( carouselEllipsePath ));

            return listBox;
        }

        private void listBox_SelectedValueChanged(object sender, EventArgs e)
        {
            this.isDirty = true;
            editorService.CloseDropDown();
        }
    }
}
