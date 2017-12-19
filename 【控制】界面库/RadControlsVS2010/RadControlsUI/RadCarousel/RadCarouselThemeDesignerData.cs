using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.Themes.Design;

namespace Telerik.WinControls.UI.Carousel
{
    public class RadCarouselThemeDesignerData: RadControlDesignTimeData
    {
        public RadCarouselThemeDesignerData()
        { }

        public RadCarouselThemeDesignerData(string name)
            : base(name)
        { }

        public override ControlStyleBuilderInfoList GetThemeDesignedControls(Control previewSurface)
        {

            RadCarousel carousel = new RadCarousel();
            carousel.Size = new Size(320,240);

            carousel.Text = "RadCarousel";

            RadCarousel carouselStructure = new RadCarousel();
            carouselStructure.AutoSize = true;

            carouselStructure.Text = "RadCarousel";
            carouselStructure.Size = new Size(320,240);

            ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(carousel, carouselStructure.RootElement);
            designed.MainElementClassName = typeof(RadCarouselElement).FullName;

            ControlStyleBuilderInfoList res = new ControlStyleBuilderInfoList();
            res.Add(designed);

            return res;
        }
    }
}
