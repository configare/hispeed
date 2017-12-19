using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents an image button.
    /// </summary>
	[ToolboxItem(false)]
	public class ImageButton : Button
	{
		Image clickImage;
		Image hoverImage;
		Image currentImage;

		string clickImageSrc;
		string hoverImageSrc;
		string backgroundImageSrc;
        /// <summary>
        /// Initializes a new instance of the ImageButton class.
        /// </summary>
		public ImageButton()
		{
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (hoverImage != null)
            {
                hoverImage.Dispose();
            }

            if (clickImage != null)
            {
                clickImage.Dispose();
            }
        }

        #region Properties
        /// <summary>
        /// Gets or sets the background image.
        /// </summary>
        [DefaultValue("")]
		[Category(RadDesignCategory.AppearanceCategory)]
		public string BackgroundImageSrc
		{
			get
			{
				return backgroundImageSrc;
			}
			set
			{
				backgroundImageSrc = value;
				if (!DesignMode)
				{
					BackgroundImage = GetImageFromResource(backgroundImageSrc);
					Region = Convert((Bitmap)BackgroundImage);
				}
			}
		}
		/// <summary>
		/// Gets or sets the image source when the button is hovered.
		/// </summary>
		[DefaultValue("")]
		[Category(RadDesignCategory.AppearanceCategory)]
		public string HoverImageSrc
		{
			get
			{
				return hoverImageSrc;
			}
			set
			{
				hoverImageSrc = value;
				if (!DesignMode)
				{
					hoverImage = GetImageFromResource(hoverImageSrc);	
				}
			}
		}
        /// <summary>
        /// Gets or sets the image source when the button is clicked.
        /// </summary>
		[DefaultValue("")]
		[Category(RadDesignCategory.AppearanceCategory)]
		public string ClickImageSrc
		{
			get
			{
				return clickImageSrc;
			}
			set
			{
				clickImageSrc = value;
				if (!DesignMode)
				{
					clickImage = GetImageFromResource(clickImageSrc);	
				}
			}
        }

        #endregion

        internal static Image GetImageFromResource(string name)
		{
			name = "Telerik.WinControls.Resources." + name;
			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
			if (stream == null)
			{
				throw new NullReferenceException("Cannot find " + name);
			}
			return new Bitmap(stream);
		}
        internal static Region Convert(Bitmap bitmap)
        {
            //sanity check
            if (bitmap == null)
                throw new ArgumentNullException("Bitmap", "Bitmap cannot be null");

            Color transparencyKey = bitmap.GetPixel(0, 0);

            GraphicsUnit unit = GraphicsUnit.Pixel;

            RectangleF boundsF = bitmap.GetBounds(ref unit);

            int yMax = (int)boundsF.Height;
            int xMax = (int)boundsF.Width;
            Region r = null;
            using (GraphicsPath path = new GraphicsPath())
            {

                for (int y = 0; y < yMax; y++)
                {
                    for (int x = 0; x < xMax; x++)
                    {
                        if (bitmap.GetPixel(x, y) == transparencyKey)
                        {
                            continue;
                        }

                        int x0 = x;

                        while (x < xMax && bitmap.GetPixel(x, y) != transparencyKey)
                        {
                            ++x;
                        }

                        path.AddRectangle(new Rectangle(x0, y, x - x0, 1));
                    }
                }

                r = new Region(path);
            }
            return r;            
        } 

        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignMode)
            {
                base.OnPaint(e);
                return;
            }

            if (currentImage == null && BackgroundImage != null)
            {
                currentImage = BackgroundImage;
            }

            if (currentImage != null)
            {
                e.Graphics.DrawImage(currentImage, 0, 0);
            }
        }
        
        protected override void OnMouseDown(MouseEventArgs e)
		{
			currentImage = clickImage;
			this.Invalidate();
			base.OnMouseDown (e);
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			currentImage = BackgroundImage;
			this.Invalidate();
			base.OnMouseUp (e);
		}
		protected override void OnMouseEnter(System.EventArgs e)
		{
			currentImage = hoverImage;
			Invalidate();
			base.OnMouseHover (e);
		}
		protected override void OnMouseLeave(System.EventArgs e)
		{
			currentImage = BackgroundImage;
			Invalidate();
			base.OnMouseLeave (e);
		}	
    }
}
