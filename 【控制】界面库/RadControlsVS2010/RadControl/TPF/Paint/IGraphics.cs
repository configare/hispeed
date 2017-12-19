using System.Drawing;
using System.Drawing.Text;
using Telerik.WinControls.Primitives;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Telerik.WinControls.Paint
{
    /// <summary>
    /// 	<para>Classes that implement IGraphics interface are capable of drawing on the
    ///     computer screen. Classes that implement this interface can use different APIs to
    ///     perform the actual drawing: GDI+, DirectX, etc.</para>
    /// </summary>
    public interface IGraphics
	{
        /// <summary>Gets the clipping rectangle; the rectangle which needs redrawing.</summary>
        Rectangle ClipRectangle { get; }

        /// <summary>Gets the current context device - graphics object.</summary>
        object UnderlayGraphics { get; }

        object SaveState();

        void RestoreState(object state);

        /// <summary>Gets or sets the opacity level of the device context.</summary>
		double Opacity { get; }

        /// <summary><para>Changes the opacity level of the current device context.</para></summary>
		void ChangeOpacity(double opacity);

        /// <summary>Restores the opacity of the current device context to the previous value.</summary>
		void RestoreOpacity();

        /// <summary>
        /// Saves the current smothingMode, and changes the smoothingmode for the current device
        /// context.
        /// </summary>
		void ChangeSmoothingMode(SmoothingMode smoothingMode);

        /// <summary>Restores the smoothing mode to the previous value.</summary>
		void RestoreSmoothingMode();

        /// <summary>Draws a rectangle specified by a rectangle structure, and a color.</summary>
		void DrawRectangle(Rectangle rectangle, Color color);

        /// <summary>
        /// Draws a rectangle specified by rectangle structure, color, PenAlignment, and pen
        /// width.
        /// </summary>
		void DrawRectangle(Rectangle rectangle, Color color, PenAlignment penAlignment, float penWidth);

        /// <summary>
        /// Draws a rectangle specified by rectangle structure, color, PenAlignment, and pen
        /// width.
        /// </summary>
        void DrawRectangle(RectangleF rectangle, Color color, PenAlignment penAlignment, float penWidth);

        /// <summary>
        /// 	<para>Updates the clipping region of the current Graphics object to exclude 
        ///     the area specified by a Rectangle structure.</para>
        /// </summary>
        void ExcludeClip(Rectangle rectangle);

		void FillPath(Color color, GraphicsPath path);

		void FillPath(Color[] colorStops, float[] colorOffsets, float angle,
			float gradientPercentage, float gradientPercentage2, Rectangle rectangle, GraphicsPath path);

        void DrawBlurShadow(GraphicsPath path, Rectangle r, float offset, Color color);

        /// <summary>
        /// Draws a linear gradient rectangle specified by rectangle structure, color array,
        /// penalignment, penWidth and angle.
        /// </summary>
        void DrawLinearGradientRectangle(RectangleF rectangle, Color[] gradientColors, PenAlignment penAlignment, float penWidth, float angle);

        /// <summary>
        /// Draws a radial gradient rectangle specified by rectangle structure, color, color
        /// array for gradient effect, penalignment, and penWidth.
        /// </summary>
		void DrawRadialGradientRectangle(Rectangle rectangle, Color color, Color[] gradientColors, PenAlignment penAlignment, float penWidth);

        /// <summary>
        /// Draws a radial gradient rectangle specified by rectangle structure, color, color
        /// array for gradient effect, penalignment, and penWidth.
        /// </summary>
        void DrawRadialGradientRectangle(RectangleF rectangle, Color color, Color[] gradientColors, PenAlignment penAlignment, float penWidth);

        /// <summary>
        /// Draws a custom gradient rectangle specified by rectangle structure, graphicsPath,
        /// color, color array for the gradient effect, penalignment, and penwidth.
        /// </summary>
		void DrawCustomGradientRectangle(Rectangle rectangle, GraphicsPath path, Color color, Color[] gradientColors, PenAlignment penAlignment, float penWidth);

        /// <summary>
        /// Draws a custom gradient rectangle specified by rectangle structure, graphicsPath,
        /// color, color array for the gradient effect, penalignment, and penwidth.
        /// </summary>
        void DrawCustomGradientRectangle(RectangleF rectangle, GraphicsPath path, Color color, Color[] gradientColors, PenAlignment penAlignment, float penWidth);

        /// <summary><para>Draws an ellipse defined by a bounding rectangle and color.</para></summary>
        void DrawEllipse(Rectangle BorderRectangle, Color color);

        /// <summary>
        /// 	<para>Draws the specified text string with specified Rectangle, Font, Color,
        ///     ContentAlignment, StringFormat, and Orientation.</para>
        /// </summary>
        void DrawString(string drawString, Rectangle rectangle, Font font, Color color, ContentAlignment alignment, StringFormat stringFormat, Orientation orientation, bool flipText);

		void DrawString(string drawString, RectangleF rectangle, Font font, Color color, ContentAlignment alignment, StringFormat stringFormat, Orientation orientation, bool flipText);

        void DrawString(TextParams textParams, SizeF measuredSize);

		/// <summary>
		/// 	<para>Draws the specified text string with specified Rectangle, Font, Color,
		///     ContentAlignment, StringFormat, and Orientation.</para>
		/// </summary>
		void DrawString(string drawString, RectangleF rectangle, Font font, Color color, ContentAlignment alignment, StringFormat stringFormat, ShadowSettings shadow, TextRenderingHint textRendering, Orientation orientation, bool flipText);

        /// <summary>
        /// 	<para>Draws the specified text string with specified Rectangle, Font, Color,
        ///     ContentAlignment, StringFormat, ShadowSettings, TextRenderingHint, and
        ///     Orientation.</para>
        /// </summary>
        void DrawString(string drawString, Rectangle rectangle, Font font, Color color, ContentAlignment alignment, StringFormat stringFormat, ShadowSettings shadow, TextRenderingHint textRendering, Orientation orientation, bool flipText);

        /// <summary>
        /// Draws the specified Image object with the specified Rectangle, Image,
        /// ContentAlignment, and disable flag.
        /// </summary>
        void DrawImage(Rectangle rectangle, Image image, ContentAlignment alignment, bool enabled);

        /// <summary>
        /// 	<para>Draws the specified Image object with the specified Point, Image, and disable
        ///     flag.</para>
        /// </summary>
        void DrawImage(Point point, Image image, bool enabled);

		/// <summary>
		/// Draws a bitmap image specified by image object, and position from the left-upper
		/// corner of the current device context.
		/// </summary>
		void DrawBitmap(Image image, int x, int y);

		/// <summary>
		/// Draws a bitmap image specified by image object, and position from the left-upper
		/// corner of the current device context and specified opacity.
		/// </summary>
		void DrawBitmap(Image image, int x, int y, double opacity);

		/// <summary>
		/// Draws a bitmap image specified by image object, position from the left-upper
		/// corner of the current device context and specified size.
		/// </summary>
		void DrawBitmap(Image image, int x, int y, int width, int height);

		/// <summary>
		/// Draws a bitmap image specified by image object, position from the left-upper
		/// corner of the current device context, opacity and specified size.
		/// </summary>
		void DrawBitmap(Image image, int x, int y, int width, int height, double opacity);

        /// <summary>Draws a path specified by GraphicsPath, color, penalignment, and penwidth.</summary>
		void DrawPath(GraphicsPath path, Color color, PenAlignment penAlignment, float penWidth);

        /// <summary>
        /// Draws a linear gradient path specified by GraphicsPath, bounding Rectangle, color
        /// gradient array, penalignment, and penwidth, and angle.
        /// </summary>
		void DrawLinearGradientPath(GraphicsPath path, RectangleF bounds, Color[] gradientColors, PenAlignment penAlignment, float penWidth, float angle);

        /// <summary>
        /// Draws a line specified by color, initial x point, initial y point, final x, and
        /// final y point.
        /// </summary>
        void DrawLine(Color color, int x1, int y1, int x2, int y2);

		/// <summary>
		/// Draws a line specified by color, initial x point, initial y point, final x, and
		/// final y point.
		/// </summary>
		void DrawLine(Color color, float x1, float y1, float x2, float y2);

		/// <summary>
		/// Draws a line specified by color, DashStyle, initial x point, initial y point, final x,
		/// and final y point.
		/// </summary>
		void DrawLine(Color color, DashStyle dashStyle, int x1, int y1, int x2, int y2);

        /// <summary>
        /// Draws a redial gradient path specified by Graphicspath, bounding rectangle, color,
        /// color gradient array, penalignment, and penwidth.
        /// </summary>
		void DrawRadialGradientPath(GraphicsPath path, Rectangle bounds, Color color, Color[] gradientColors, PenAlignment penAlignment, float penWidth);

        /// <summary>
        /// Draws a custom gradient path specified by GraphicsPath, GraphicsPath for the
        /// gradient, color, gradient color array, penalignment, and penwidth.
        /// </summary>
		void DrawCustomGradientPath(GraphicsPath path, GraphicsPath gradientPath, Color color, Color[] gradientColors, PenAlignment penAlignment, float penWidth);

        /// <summary>Creates a mask specified by color and bitmap.</summary>
		Bitmap CreateBitmapMask(Color maskColor, Bitmap bitmap);

        /// <summary>
        /// Fills the interior of a rectangle specified by the
        /// borderRectangle and using for color the second argument.
        /// </summary>
        void FillRectangle(Rectangle BorderRectangle, Color color);

		void FillRectangle(RectangleF BorderRectangle, Color color);

        /// <summary>
        /// Fills a rectangle using the image as texture.
        /// </summary>
        /// <param name="rectangle">The rectangle to fill.</param>
        /// <param name="texture">The image to use as a texture.</param>
        void FillTextureRectangle(Rectangle rectangle, Image texture);

        /// <summary>
        /// Fills a rectangle using the image as texture.
        /// </summary>
        /// <param name="rectangle">The rectangle to fill.</param>
        /// <param name="texture">The image to use as a texture.</param>
        /// <param name="wrapMode">Defines the way the image is populated in the rectangle</param>
        void FillTextureRectangle(Rectangle rectangle, Image texture, WrapMode wrapMode);

        /// <summary>
        /// Fills a rectangle using the image as texture.
        /// </summary>
        /// <param name="rectangle">The rectangle to fill.</param>
        /// <param name="texture">The image to use as a texture.</param>
        void FillTextureRectangle(RectangleF rectangle, Image texture);

        /// <summary>
        /// Fills a rectangle using the image as texture.
        /// </summary>
        /// <param name="rectangle">The rectangle to fill.</param>
        /// <param name="texture">The image to use as a texture.</param>
        /// <param name="wrapMode">Defines the way the image is populated in the rectangle</param>
        void FillTextureRectangle(RectangleF rectangle, Image texture, WrapMode wrapMode);

        /// <summary>
        /// Fills gradient rectangle specified by rectangle structure, color, color, color,
        /// color, GradientStyles, and angle.
        /// </summary>
        void FillGradientRectangle(Rectangle rectangle, Color color1, Color color2, Color color3, Color color4, GradientStyles style, float angle);

        /// <summary>
        /// Fills the gradient rectangle specified by rectangle structure, color gradient array,
        /// float offset array, GradientStyles, angle, gradientPercentage, and
        /// gradientPercentage2.
        /// </summary>
		void FillGradientRectangle(Rectangle rectangle, Color[] colorStops, float[] colorOffsets, GradientStyles style, float angle, float gradientPercentage, float gradientPercentage2);

        void FillGradientRectangle(RectangleF rectangle, Color[] colorStops, float[] colorOffsets, GradientStyles style, float angle, float gradientPercentage, float gradientPercentage2);

        /// <summary>
        /// Fills the gradient rectangle specified by rectangle structure, color gradient array,
        /// float offset array, GradientStyles, angle, gradientPercentage, and
        /// gradientPercentage2.
        /// </summary>
        void FillGradientRectangle(Rectangle rectangle, Color color1, Color color2, float angle);

        /// <summary>
        /// Fills the glass gradient rectangle specified by rectangle structure, color, color,
        /// color, color, and gradient percentage.
        /// </summary>
        void FillGlassRectangle(Rectangle rectangle, Color color1, Color color2, Color color3, Color color4, float gradientPercentage, float gradientPercentage2);

        /// <summary>
        /// Fills the office glass gradient rectangle specified by rectangle structure, color,
        /// color, color, color, and gradientPercentage and gradientPercentage2.
        /// </summary>
		void FillOfficeGlassRectangle(Rectangle rectangle, Color color1, Color color2, Color color3, Color color4, float gradientPercentage, float gradientPercentage2, bool drawEllipse);

        /// <summary>
        /// Fills the vista gradient rectangle specified by rectangle structure, color, color,
        /// color, color, gradient percentage, and gradientPercentage2.
        /// </summary>
        void FillVistaRectangle(Rectangle rectangle, Color color1, Color color2, Color color3, Color color4, float gradientPercentage, float gradientPercentage2);

        /// <summary>
        /// Fills the gel gradient rectangle specified by rectangle structure, color, color, and
        /// gradientPercentage.
        /// </summary>
        void FillGellRectangle(Rectangle rectangle, Color[] colorStops, float gradientPercentage, float gradientPercentage2);

        /// <summary>
        /// Fills the interior of a polygon defined by an array of points specified by
        ///    <a href="frlrfsystemdrawingpointclasstopic.asp">Point</a> structures and
        ///    color.
        /// </summary>
        void FillPolygon(Color color, Point[] points);

        /// <summary>
        /// Fills the interior of a polygon defined by color and an array of points specified
        /// by <a href="frlrfsystemdrawingpointclasstopic.asp">Point</a> structures.
        /// </summary>
        void FillPolygon(Color color, PointF[] points);

        /// <summary>
        /// Fills the interior of a polygon defined by brush and an array of points specified
        /// by <a href="frlrfsystemdrawingpointclasstopic.asp">Point</a> structures.
        /// </summary>
        void FillPolygon(Brush brush, PointF[] points);

        /// <summary>
        /// Draws a round rectangle specified by Rectangle structure, color, float borderWidth,
        /// and radius in pixels.
        /// </summary>
        void DrawRoundRect(Rectangle rectangle, Color color, float borderWidth, int radius);

        /// <summary>
        /// 	<para>Translates the local geometric transformation of this TextureBrush object by
        ///     the specified dimensions. This method prepends the translation to the
        ///     transformation.</para>
        /// </summary>
        void TranslateTransform(int offsetX, int offsetY);

        /// <summary>
        /// Translates the local geometric transformation of this TextureBrush object by the
        /// specified dimensions. This method prepends the translation to the
        /// transformation.
        /// </summary>
        void TranslateTransform(float offsetX, float offsetY);

        /// <summary>
        /// Rotates the local geometric transformation by the specified angle. This method
        /// prepends the rotation to the transformation.
        /// </summary>
        void RotateTransform(float angleInDegrees);

        /// <summary>
        /// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Resets the
        /// world transformation matrix of this Graphics to the identity matrix.</span>
        /// </summary>
        void ResetTransform();

        /// <summary>
        /// Scales the world transformation matrix by the specified amount.
        /// </summary>
        void ScaleTransform(SizeF scale);

        void PushCurrentClippingPath(GraphicsPath path);

        GraphicsPath PopCurrentClippingPath();

        SizeF MeasureString(string text, Font font, StringFormat stringFormat);

        void DrawBorder(RectangleF rectangle, IBorderElement borderElement);
	}
}