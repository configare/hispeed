using Telerik.WinControls.Paint;
using System.Drawing;

namespace Telerik.WinControls.Primitives
{
	public interface IPrimitive
    {
        /// <summary><para>Gets or sets a value indicating whether the primitive should 
        /// be painted.</para></summary>
        bool ShouldPaint
        {
            get;
            set;
        }

        /// <summary>Draws the primitive on the screen.</summary>
		void PaintPrimitive(IGraphics graphics, float angle, SizeF scale);
	}
}