using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Represents a DockingGuidesTemplate which uses embedded images and may not be modified by the user.
    /// </summary>
    public abstract class PredefinedDockingGuidesTemplate : DockingGuidesTemplate
    {
        public PredefinedDockingGuidesTemplate()
        {
            this.LoadEmbeddedImages();
            this.UpdateImagesMetrics();
            this.SetReadOnly();
        }

        private void SetReadOnly()
        {
            ((DockingGuideImage)this.LeftImage).SetReadOnly(true);
            ((DockingGuideImage)this.TopImage).SetReadOnly(true);
            ((DockingGuideImage)this.RightImage).SetReadOnly(true);
            ((DockingGuideImage)this.BottomImage).SetReadOnly(true);
            ((DockingGuideImage)this.FillImage).SetReadOnly(true);
            ((DockingGuideImage)this.CenterBackgroundImage).SetReadOnly(true);
        }

        internal virtual void UpdateImagesMetrics()
        {
        }

        internal virtual string ResourceFolder
        {
            get
            {
                return string.Empty;
            }
        }

        private void LoadEmbeddedImages()
        {
            string folder = this.ResourceFolder;
            if (string.IsNullOrEmpty(folder))
            {
                throw new ArgumentException("Resource folder is null or empty");
            }

            //load embedded images
            Type type = typeof(PredefinedDockingGuidesTemplate);
            string path = "Telerik.WinControls.UI.Docking.Resources.DockingGuides." + folder + ".";

            //load LeftImage
            this.LeftImage.Image = ResourceHelper.ImageFromResource(type, path + LeftImageName);
            this.LeftImage.HotImage = ResourceHelper.ImageFromResource(type, path + LeftImageNameHot);

            //load TopImage
            this.TopImage.Image = ResourceHelper.ImageFromResource(type, path + TopImageName);
            this.TopImage.HotImage = ResourceHelper.ImageFromResource(type, path + TopImageNameHot);

            //load RightImage
            this.RightImage.Image = ResourceHelper.ImageFromResource(type, path + RightImageName);
            this.RightImage.HotImage = ResourceHelper.ImageFromResource(type, path + RightImageNameHot);

            //load BottomImage
            this.BottomImage.Image = ResourceHelper.ImageFromResource(type, path + BottomImageName);
            this.BottomImage.HotImage = ResourceHelper.ImageFromResource(type, path + BottomImageNameHot);

            //load FillImage
            this.FillImage.Image = ResourceHelper.ImageFromResource(type, path + FillImageName);
            this.FillImage.HotImage = ResourceHelper.ImageFromResource(type, path + FillImageNameHot);

            //load BackgroundImage
            this.CenterBackgroundImage.Image = ResourceHelper.ImageFromResource(type, path + CenterBackgroundImageName);
        }

        #region Static Members

        private const string LeftImageName = "Left.png";
        private const string LeftImageNameHot = "Left_Hot.png";
        private const string TopImageName = "Top.png";
        private const string TopImageNameHot = "Top_Hot.png";
        private const string RightImageName = "Right.png";
        private const string RightImageNameHot = "Right_Hot.png";
        private const string BottomImageName = "Bottom.png";
        private const string BottomImageNameHot = "Bottom_Hot.png";
        private const string FillImageName = "Fill.png";
        private const string FillImageNameHot = "Fill_Hot.png";
        private const string CenterBackgroundImageName = "Center_Background.png";

        public static readonly DockingGuidesTemplate VS2010 = new VS2010DockingGuidesTemplate();
        public static readonly DockingGuidesTemplate VS2008 = new VS2008DockingGuidesTemplate();
        public static readonly DockingGuidesTemplate ControlDefault = new ControlDefaultDockingGuidesTemplate();
        public static readonly DockingGuidesTemplate Office2010 = new Office2010DockingGuidsTemplete();

        #endregion
    }
}
