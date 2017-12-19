using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a button on the <c ref="BackstageItemsPanelElement"/>.
    /// </summary>
    public class BackstageButtonItem : BackstageVisualElement
    {
        public static readonly RadProperty IsCurrentProperty = RadProperty.Register(
                "IsCurrent", typeof(bool), typeof(BackstageVisualElement), new RadElementPropertyMetadata(
                    false, ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsArrange));

        public BackstageButtonItem()
        {

        }

        public BackstageButtonItem(String text)
        {
            this.Text = text;
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.Class = "BackstageButtonItem";
            this.ThemeRole = "BackstageButtonItem"; 
            this.DrawFill = true;
            this.MinSize = new Size(0, 26);
            this.TextAlignment = ContentAlignment.MiddleLeft;
            this.ImageAlignment = ContentAlignment.MiddleLeft;
            this.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.Padding = new System.Windows.Forms.Padding(13, 0, 13, 0);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            BackstageItemsPanelElement parent = this.Parent as BackstageItemsPanelElement;
            if(parent != null)
            {
                parent.Owner.OnItemClicked(this);
            }
        }
    }
}
