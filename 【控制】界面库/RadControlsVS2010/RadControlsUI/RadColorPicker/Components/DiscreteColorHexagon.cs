using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI.RadColorPicker
{
	/// <summary>
	/// Represents a hexagon of discrete colors
	/// </summary>
    [ToolboxItem(false)]
    public class DiscreteColorHexagon : UserControl
    {
        private const float coeffcient = 0.824f;
        private ColorHexagonElement[] hexagonElements = new ColorHexagonElement[147];
        private float[] matrix1 = new float[] 
			{ -0.5f, -1f, -0.5f, 
				0.5f, 1f, 0.5f };

        private float[] matrix2 = new float[] 
			{ coeffcient, 0f, -1*coeffcient, 
			-1*coeffcient, 0f, coeffcient };

        private int oldSelectedHexagonIndex = -1;
		private int sectorMaximum = 7;
		private int selectedHexagonIndex = -1;
        private Container container;

        /// <summary>
        /// Fires when the selected color has changed
        /// </summary>
        public event ColorChangedEventHandler ColorChanged;

        public DiscreteColorHexagon()
        {
			//set form styles
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.Opaque, true);
            base.SetStyle(ControlStyles.DoubleBuffer, true);
            base.SetStyle((ControlStyles)0x20000, true);
            base.SetStyle(ControlStyles.ResizeRedraw, true);
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

			//create hexagon elements
			for (int i = 0; i < this.hexagonElements.Length; i++)
				this.hexagonElements[i] = new ColorHexagonElement();

			this.container = new Container();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.container != null))
            {
                this.container.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
				//deselected the previously selected hexagon
                if (this.selectedHexagonIndex >= 0)
                {
                    this.hexagonElements[this.selectedHexagonIndex].IsSelected = false;
                    base.Invalidate(this.hexagonElements[this.selectedHexagonIndex].BoundingRectangle);
                }
                this.selectedHexagonIndex = -1;
				//draw the highlighter around the newly selected hexagon
                if (this.oldSelectedHexagonIndex >= 0)
                {
                    this.selectedHexagonIndex = this.oldSelectedHexagonIndex;
                    this.hexagonElements[this.selectedHexagonIndex].IsSelected = true;
                    if (this.ColorChanged != null)
                    {
						this.ColorChanged(this, new ColorChangedEventArgs(this.SelectedColor));
                    }
                    base.Invalidate(this.hexagonElements[this.selectedHexagonIndex].BoundingRectangle);
                }
            }
            base.OnMouseDown(e);
        }

		protected override void OnPaint(PaintEventArgs e)
		{
			if (this.BackColor == Color.Transparent)
				base.OnPaintBackground(e);

			Graphics graphics = e.Graphics;
			using (SolidBrush brush = new SolidBrush(this.BackColor))
				graphics.FillRectangle(brush, base.ClientRectangle);

			foreach (ColorHexagonElement w in this.hexagonElements)
				w.Paint(graphics);

			if (this.oldSelectedHexagonIndex >= 0)
				this.hexagonElements[this.oldSelectedHexagonIndex].Paint(graphics);

			if (this.selectedHexagonIndex >= 0)
				this.hexagonElements[this.selectedHexagonIndex].Paint(graphics);

			base.OnPaint(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			this.DrawHexagonHighlighter(-1);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			this.DrawHexagonHighlighter(this.GetHexagonIndexFromCoordinates(e.X, e.Y));
		}
		
        protected override void OnResize(EventArgs e)
        {
            this.InitializeHexagons();
            base.OnResize(e);
        }

        private int GetHexagonWidth(int availableHeight)
        {
            int hexagonHeight = (availableHeight / ((2 * this.sectorMaximum) - 1)) + 1;
            if ((((int) Math.Floor(((double) hexagonHeight) / 2)) * 2) < hexagonHeight)
                hexagonHeight--;
            
			return hexagonHeight;
        }

        private void InitializeHexagons()
        {
            Rectangle clientRectangle = base.ClientRectangle;
            clientRectangle.Inflate(-8, -8);
            clientRectangle.Height -= this.GetHexagonWidth(Math.Min(clientRectangle.Height, clientRectangle.Width)) * 3;
            if (clientRectangle.Height < clientRectangle.Width)
                clientRectangle.Inflate(-(clientRectangle.Width - clientRectangle.Height) / 2, 0);
            else
                clientRectangle.Inflate(0, -(clientRectangle.Height - clientRectangle.Width) / 2);
            
			//
            int hexagonWidth = this.GetHexagonWidth(clientRectangle.Height);
            int centerOfMiddleHexagonX = (clientRectangle.Left + clientRectangle.Right) / 2;
            int centerOfMiddleHexagonY = (clientRectangle.Top + clientRectangle.Bottom) / 2;
			//
			this.hexagonElements[0].CurrentColor = Color.White;
            this.hexagonElements[0].SetHexagonPoints((float) centerOfMiddleHexagonX, (float) centerOfMiddleHexagonY, hexagonWidth);
            int index = 1;
            for (int i = 1; i < this.sectorMaximum; i++)
            {
				//
                float currentHexagonY = centerOfMiddleHexagonY;
				float currentHexagonX = centerOfMiddleHexagonX + (hexagonWidth * i);
                
                for (int innerIndex = 0; innerIndex < (this.sectorMaximum - 1); innerIndex++)
                {
                    int verticalStep = (int) (hexagonWidth * this.matrix2[innerIndex]);
					int horizontalStep = (int)(hexagonWidth * this.matrix1[innerIndex]);
                    
                    for (int innermostIndex = 0; innermostIndex < i; innermostIndex++)
                    {
						double colorQuotient2 = ((0.936 * (this.sectorMaximum - i)) / ((double)this.sectorMaximum)) + 0.12;
						float colorQuotient1 = ColorServices.GetColorQuotient(currentHexagonX - centerOfMiddleHexagonX, currentHexagonY - centerOfMiddleHexagonY);
						this.hexagonElements[index].SetHexagonPoints(currentHexagonX, currentHexagonY, hexagonWidth);
						this.hexagonElements[index].CurrentColor = ColorServices.ColorFromRGBRatios((double)colorQuotient1, colorQuotient2, 1);
						currentHexagonY += verticalStep;
						currentHexagonX += horizontalStep;
						index++;
                    }
                }
            }

			InitializeGrayscaleHexagons(ref clientRectangle, hexagonWidth, ref centerOfMiddleHexagonX, ref centerOfMiddleHexagonY, ref index);
        }

		private void InitializeGrayscaleHexagons(ref Rectangle clientRectangle, int hexagonWidth, ref int centerOfMiddleHexagonX, ref int centerOfMiddleHexagonY, ref int index)
		{
            int grayScaleStartValue = 255;
            int grayScaleValue = grayScaleStartValue;
            int numberOfHexagons;
            int grayScaleStep;

            grayScaleStep = 17;
            numberOfHexagons = 16;
            centerOfMiddleHexagonX = clientRectangle.X + ((int)(hexagonWidth * 3)) - (hexagonWidth / 4);

			centerOfMiddleHexagonY = clientRectangle.Bottom;

			//setting up 16/17 grayscale hexagons
			for (int j = 0; j < numberOfHexagons; j++)
			{
                //setting up the grayscale color
                this.hexagonElements[index].CurrentColor = Color.FromArgb(grayScaleValue, grayScaleValue, grayScaleValue);
                //setting up the hexagon points
                this.hexagonElements[index].SetHexagonPoints(
                    (float)centerOfMiddleHexagonX, (float)centerOfMiddleHexagonY, hexagonWidth);

                centerOfMiddleHexagonX += hexagonWidth;
                index++;

                //when the first row is done, move the coordinate of the next hexagon down and to the right
                if (j == 7)
                {
                    centerOfMiddleHexagonX = clientRectangle.X + ((int)(hexagonWidth * 3.5)) - (hexagonWidth / 4);
                    centerOfMiddleHexagonY += (int)(hexagonWidth * coeffcient);
                }
                grayScaleValue -= grayScaleStep;
            }
		}

        private void DrawHexagonHighlighter(int selectedHexagonIndex)
        {
            if (selectedHexagonIndex != this.oldSelectedHexagonIndex)
            {
				//clear the highlighter of the previously selected hexagon
				if (this.oldSelectedHexagonIndex >= 0)
				{
					this.hexagonElements[this.oldSelectedHexagonIndex].IsHovered = false;
					base.Invalidate(this.hexagonElements[this.oldSelectedHexagonIndex].BoundingRectangle);
				}
				//draw the highlighter hexagon around the newly selected hexagon
                this.oldSelectedHexagonIndex = selectedHexagonIndex;
				if (this.oldSelectedHexagonIndex >= 0)
				{
					this.hexagonElements[this.oldSelectedHexagonIndex].IsHovered = true;
					base.Invalidate(this.hexagonElements[this.oldSelectedHexagonIndex].BoundingRectangle);
				}
            }
        }

        private int GetHexagonIndexFromCoordinates(int xCoordinate, int yCoordinate)
        {
            for (int hexagonIndex = 0; hexagonIndex < this.hexagonElements.Length; hexagonIndex++)
            {
                if (this.hexagonElements[hexagonIndex].BoundingRectangle.Contains(xCoordinate, yCoordinate))
                    return hexagonIndex;
            }
            return -1;
        }

        /// <summary>
        /// Gets the selected color
        /// </summary>
        public Color SelectedColor
        {
            get
            {
                if (this.selectedHexagonIndex < 0)
                    return Color.Empty;
            
				return this.hexagonElements[this.selectedHexagonIndex].CurrentColor;
            }
        }
    }
    internal class ColorHexagonElement
    {
        private Point[] hexagonPoints = new Point[6];
		private Color hexagonColor = Color.Empty;
		private Rectangle boundingRectangle = Rectangle.Empty;
		private bool isHovered;
		private bool isSelected;
        
        public void SetHexagonPoints(float xCoordinate, float yCoordinate, int hexagonWidth)
        {
            float num = hexagonWidth * 0.5773503f;
            this.hexagonPoints[0] = new Point((int) Math.Floor((double) (xCoordinate - (hexagonWidth / 2))), ((int) Math.Floor((double) (yCoordinate - (num / 2f)))) - 1);
            this.hexagonPoints[1] = new Point((int) Math.Floor((double) xCoordinate), ((int) Math.Floor((double) (yCoordinate - (hexagonWidth / 2)))) - 1);
            this.hexagonPoints[2] = new Point((int) Math.Floor((double) (xCoordinate + (hexagonWidth / 2))), ((int) Math.Floor((double) (yCoordinate - (num / 2f)))) - 1);
            this.hexagonPoints[3] = new Point((int) Math.Floor((double) (xCoordinate + (hexagonWidth / 2))), ((int) Math.Floor((double) (yCoordinate + (num / 2f)))) + 1);
            this.hexagonPoints[4] = new Point((int) Math.Floor((double) xCoordinate), ((int) Math.Floor((double) (yCoordinate + (hexagonWidth / 2)))) + 1);
            this.hexagonPoints[5] = new Point((int) Math.Floor((double) (xCoordinate - (hexagonWidth / 2))), ((int) Math.Floor((double) (yCoordinate + (num / 2f)))) + 1);
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddPolygon(this.hexagonPoints);
                this.boundingRectangle = Rectangle.Round(path.GetBounds());
                this.boundingRectangle.Inflate(2, 2);
            }
        }
		/// <summary>
		/// Paints the hexagon
		/// </summary>
		/// <param name="graphics"></param>
        public void Paint(Graphics graphics)
        {
            //creat the hexagonal path
			GraphicsPath path = new GraphicsPath();
            path.AddPolygon(this.hexagonPoints);
            path.CloseAllFigures();
			
			//
            using (SolidBrush brush = new SolidBrush(this.hexagonColor))
                graphics.FillPath(brush, path);

			//draw the enclosing hexagon in case the hexagon is selected or hovered
            if (this.isHovered || this.isSelected)
            {
                SmoothingMode savedMode = graphics.SmoothingMode;
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
				using (Pen pen = new Pen(Color.FromArgb(42, 91, 150), 2f))
					graphics.DrawPath(pen, path);

				using (Pen pen2 = new Pen(Color.FromArgb(150, 177, 239), 1f))
					graphics.DrawPath(pen2, path);
				
				graphics.SmoothingMode = savedMode;
            }
            path.Dispose();
        }

        /// <summary>
        /// Gets or sets the hexagon color
        /// </summary>
        public Color CurrentColor
        {
            get
            {
                return this.hexagonColor;
            }
            set
            {
                this.hexagonColor = value;
            }
        }
		/// <summary>
		/// Gets a rectangle containing the hexagon
		/// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                return this.boundingRectangle;
            }
        }
		/// <summary>
		/// Gets or sets a value indicating whether the hexagon is hovered
		/// </summary>
        public bool IsHovered
        {
            get
            {
                return this.isHovered;
            }
            set
            {
                this.isHovered = value;
            }
        }
		/// <summary>
		/// Gets or sets a value indicating whether the hexagon is selected
		/// </summary>
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
            set
            {
                this.isSelected = value;
            }
        }
    }
}