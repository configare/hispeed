using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using Telerik.WinControls.Paint;

namespace Telerik.WinControls.Primitives
{
    public struct TextParams
    {
        public bool autoEllipsis;
        public bool useMnemonic;
        public bool useCompatibleTextRendering;
        public bool flipText;
        public bool measureTrailingSpaces;
        public bool rightToLeft;
        public bool showKeyboardCues;
        //public bool autoSize;
        public bool textWrap;
        public bool stretchHorizontally;

        public ContentAlignment alignment;
        public Orientation textOrientation;
        public TextRenderingHint textRenderingHint;
        public string text;
        public RectangleF paintingRectangle;        
        public Font font;
        public Color foreColor;
        public ShadowSettings shadow;
        public bool ClipText;
        public bool lineLimit;
        //public Size maxSize;

        public StringFormat CreateStringFormat()
        {
            StringFormat format = TelerikHelper.StringFormatForAlignment(this.alignment);
            if (this.rightToLeft)
            {
                format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
            }

            if (this.autoEllipsis)
            {
                format.Trimming = StringTrimming.EllipsisCharacter;
            }

            if ( this.lineLimit )
            {
                format.FormatFlags |= StringFormatFlags.LineLimit;
            }

            if (!this.useMnemonic)
            {
                format.HotkeyPrefix = HotkeyPrefix.None;
            }

            else if (this.showKeyboardCues)
            {
                format.HotkeyPrefix = HotkeyPrefix.Show;
            }
            else
            {
                format.HotkeyPrefix = HotkeyPrefix.Hide;
            }

            if (this.measureTrailingSpaces)
            {
                format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            }

            if (!this.textWrap)
            {
                format.FormatFlags |= StringFormatFlags.NoWrap;
            }

            if (this.ClipText)
            {
                format.FormatFlags &= ~StringFormatFlags.NoClip;
            }
            else
            {
                format.FormatFlags |= StringFormatFlags.NoClip;
            }

            switch (this.alignment)
            {
                case ContentAlignment.BottomCenter:
                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                    format.Alignment = StringAlignment.Center;
                    break;
                case ContentAlignment.BottomLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.TopLeft:
                    format.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.BottomRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.TopRight:
                    format.Alignment = StringAlignment.Far;
                    break;
                default:
                    break;
            }

            return format;
        }
    }
}
