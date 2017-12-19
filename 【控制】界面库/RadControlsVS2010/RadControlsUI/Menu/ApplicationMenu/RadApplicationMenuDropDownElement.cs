using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    public class RadApplicationMenuDropDownElement : RadElement
    {
        private RadApplicationMenuContentElement content;
        private RadApplicationMenuContentElement topContent;
        private RadApplicationMenuDropDownMenuElement topRightContent;
        private RadApplicationMenuContentElement bottomContent;
        private RadApplicationMenuDropDownMenuElement menuElement;

        private int rightColumnWidth = 300;

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.UseNewLayoutSystem = true;
            this.Class = "ApplicationMenuDropDownElement";
        }

        #region Properties

        [DefaultValue(300)]
        public int RightColumnWidth
        {
            get { return this.rightColumnWidth; }
            set
            {
                if (this.rightColumnWidth != value)
                {
                    this.rightColumnWidth = value;
                    this.topRightContent.MinSize = new Size(value, 0);
                    this.topRightContent.MaxSize = new Size(value, 0);
                    this.topRightContent.Layout.MinSize = new Size(this.rightColumnWidth, 0);
                    this.topRightContent.Layout.MaxSize = new Size(this.rightColumnWidth, 0);
                }
            }
        }

        public RadApplicationMenuContentElement ContentElement
        {
            get
            {
                return this.content;
            }
        }

        public RadApplicationMenuContentElement TopContentElement
        {
            get
            {
                return this.topContent;
            }
        }

        public RadDropDownMenuElement TopRightContentElement
        {
            get
            {
                return this.topRightContent;
            }
        }

        public RadDropDownMenuElement MenuElement
        {
            get
            {
                return this.menuElement;
            }
        }

        public RadApplicationMenuContentElement BottomContentElement
        {
            get
            {
                return this.bottomContent;
            }
        }

        #endregion

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF result = base.MeasureOverride(availableSize);
            this.content.Measure(availableSize);
            this.topContent.Measure(availableSize);
            this.bottomContent.Measure(availableSize);
            
            this.topRightContent.Measure(availableSize);
            this.menuElement.Measure(availableSize);

            return result;
        }

        protected override void CreateChildElements()
        {
            content = new RadApplicationMenuContentElement();
            content.Layout.Orientation = Orientation.Vertical;
            content.Class = "AppMenuContentElement";
            content.Fill.Class = "AppMenuFill";
            content.Border.Class = "AppMenuBorder";
            this.Children.Add(content);
            
            topContent = new RadApplicationMenuContentElement();
            topContent.Layout.Orientation = Orientation.Horizontal;
            topContent.Class = "AppMenuTopContentElement";
            topContent.Fill.Class = "AppMenuTopFill";
            topContent.Border.Class = "AppMenuTopBorder";
            content.Layout.Children.Add(topContent);

            menuElement = new RadApplicationMenuDropDownMenuElement();
            topContent.Layout.Children.Add(menuElement);
            menuElement.Layout.Class = "AppMenuLeftLayout";

            topRightContent = new RadApplicationMenuDropDownMenuElement();
            topRightContent.Class = "AppMenuTopRightContentElement";
            topRightContent.Fill.Class = "AppMenuRightColumnFill";
            topRightContent.Border.Class = "AppMenuRightColumnBorder";
            topRightContent.MinSize = new Size(this.rightColumnWidth, 0);
            topRightContent.Layout.MinSize = new Size(this.rightColumnWidth, 0);
            topRightContent.Layout.Class = "AppMenuRightLayout";
            topRightContent.Layout.LeftColumnMinWidth = 0;
            topContent.Layout.Children.Add(topRightContent);
            

            bottomContent = new RadApplicationMenuContentElement();
            bottomContent.Layout = new RadApplicationMenuBottomStripLayout();
            bottomContent.Class = "AppMenuBottomContentElement";
            bottomContent.Fill.Class = "AppMenuBottomFill";
            bottomContent.Border.Class = "AppMenuBottomBorder";
            bottomContent.Layout.Orientation = Orientation.Horizontal;
            bottomContent.Layout.StretchHorizontally = false;
            bottomContent.Layout.Alignment = ContentAlignment.MiddleRight;
            bottomContent.Layout.Class = "AppMenuBottomLayout";

            content.Layout.Children.Add(bottomContent);

            menuElement.SetValue(RadDropDownMenuElement.DropDownPositionProperty, DropDownPosition.LeftContent);
            topRightContent.SetValue(RadDropDownMenuElement.DropDownPositionProperty, DropDownPosition.RightContent);
        }        
    }
}
