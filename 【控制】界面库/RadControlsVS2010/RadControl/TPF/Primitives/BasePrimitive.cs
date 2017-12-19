using Telerik.WinControls.Paint;
using System.ComponentModel;
using System.Drawing;

namespace Telerik.WinControls.Primitives
{
    /// <summary>
    /// Represents a base type for all primitives. Defines PaintPrimitive method that is
    /// overridden in all derived classes.
    /// <para>Primitives are these RadElement(s) that are actually drawn on the 
    /// screen.</para>
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BasePrimitive : VisualElement, IPrimitive
    {
        #region Constructor

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.ShouldPaint = true;
        }

        #endregion

        #region Overrides

        protected override void PaintElement(IGraphics graphics, float angle, SizeF scale)
        {
            this.PaintPrimitive(graphics, angle, scale);
        }

        /// <summary>Virtual function that draws the primitive on the screen.</summary>
        public virtual void PaintPrimitive(IGraphics graphics, float angle, SizeF scale)
        {
        }

        #endregion

        #region Properties

        /// <summary><para>Gets a value indicating whether the primitive has content.</para></summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool IsEmpty
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Fields

        public const string BoxCategory = "Box";

        //keep bit state declaration consistency
        internal const ulong BasePrimitiveLastStateKey = VisualElementLastStateKey;

        #endregion
    }
}