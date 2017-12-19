using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Collections;
using Telerik.WinControls.Design;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Security.Permissions;
using System.Xml;
using System.Globalization;
using System.Drawing.Drawing2D;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls
{
    public class TelerikHelper
    {
        public static string TextWithoutMnemonics(string text)
        {
            if (text == null)
                return null;

            int ampIndex = text.IndexOf('&');
            if (ampIndex == -1)
                return text;

            StringBuilder stringBuilder = new StringBuilder(text.Substring(0, ampIndex));
            while (ampIndex < text.Length)
            {
                if (text[ampIndex] == '&')
                {
                    ampIndex++;
                }
                if (ampIndex < text.Length)
                {
                    stringBuilder.Append(text[ampIndex]);
                }
                ampIndex++;
            }
            return stringBuilder.ToString();
        }

        public static bool ContainsMnemonic(string text)
        {
            if (text == null)
                return false;

            int textLength = text.Length;
            int ampIndex = text.IndexOf('&', 0);
            if ((ampIndex >= 0) && (ampIndex <= (textLength - 2)))
            {
                int charIndex = text.IndexOf('&', ampIndex + 1);
                if (charIndex == -1)
                    return true;
            }
            return false;
        }

        public static StringFormat StringFormatForAlignment(ContentAlignment align)
        {
            StringFormat format1 = new StringFormat();
            format1.Alignment = TelerikAlignHelper.TranslateAlignment(align);
            format1.LineAlignment = TelerikAlignHelper.TranslateLineAlignment(align);
            return format1;
        }

        public static System.Drawing.Image ImageFromString(string encodedImage)
        {
            System.Drawing.Image img = null;
            if (string.IsNullOrEmpty(encodedImage))
            {
                return null;
            }

            byte[] bytes = Convert.FromBase64String(encodedImage);
            MemoryStream mem = new MemoryStream(bytes);
            img = System.Drawing.Image.FromStream(mem);

            System.Drawing.Image res = null;

            if (img.PixelFormat == PixelFormat.Format24bppRgb)
            {
                res = new Bitmap(img);
                (res as Bitmap).MakeTransparent(Color.Magenta);
            }
            else
            {
                BitmapData bitmapData = (img as Bitmap).LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
                Bitmap returnBitmap = new Bitmap(bitmapData.Width, bitmapData.Height, bitmapData.Stride, PixelFormat.Format32bppArgb, bitmapData.Scan0);
                res = new Bitmap(returnBitmap);
            }

            // Create new bitmap since the old one is connected with the stream somehow and
            // becomes invalid when the stream is disposed
            //System.Drawing.Image res = new Bitmap(img);
            mem.Dispose();
            img.Dispose();
            //(res as Bitmap).MakeTransparent(Color.Magenta); 
            return res;
        }

        public static string ImageToString(System.Drawing.Image image)
        {
            if (image == null)
            {
                return string.Empty;
            }

            using (System.IO.MemoryStream mem = new System.IO.MemoryStream(1024))
            {
                // TODO: Beta 2 issue with the ImageFormat. RawFormat on image object does not return the actual image format
                // Right now it is hard coded to PNG but in final version we should get the original image format
                image.Save(mem, GetImageFormat(image));

                string str = Convert.ToBase64String(mem.ToArray(), 0, (int)mem.Length);
                return str;
            }
        }
    
        public static ImageFormat GetImageFormat(System.Drawing.Image img)
        {
            ImageFormat rawFormat = img.RawFormat;

            ImageCodecInfo[] decoders = ImageCodecInfo.GetImageDecoders();
            if (CodecInfoExists(decoders, rawFormat))
            {
                return rawFormat;
            }

            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            if (CodecInfoExists(encoders, rawFormat))
            {
                return rawFormat;
            }

            // The following image formats are not in the above arrays...
            ImageFormat res = rawFormat;
            if (rawFormat != ImageFormat.MemoryBmp && rawFormat != ImageFormat.Exif)
            {
                // Force bitmap format
                res = ImageFormat.Bmp;
            }

            return res;
        }

        private static bool CodecInfoExists(ImageCodecInfo[] codecs, ImageFormat rawFormat)
        {
            int index = GetCodecInfoIndex(codecs, rawFormat);
            if (index >= 0 && index < codecs.Length)
            {
                return true;
            }
            return false;
        }
    
        private static int GetCodecInfoIndex(ImageCodecInfo[] codecs, ImageFormat rawFormat)
        {
            for (int i = 0; i < codecs.Length; i++)
            {
                ImageCodecInfo codecInfo = codecs[i];
                string description = codecInfo.FormatDescription;
                string codecName = codecInfo.CodecName;
                if (rawFormat.Guid == codecInfo.FormatID)
                {
                    return i;
                }
            }
            return -1;
        }

        public static RectangleF PerformTopLeftRotation(ref RadMatrix matrix, RectangleF bounds, float angle)
        {
            RadMatrix temp = RadMatrix.Identity;
            RectangleF boundingRect = PerformCenteredRotation(ref temp, bounds, angle);
            temp.Translate(bounds.X - boundingRect.X, bounds.Y - boundingRect.Y, MatrixOrder.Append);
            matrix.Multiply(temp, MatrixOrder.Append);
            return new RectangleF(bounds.Location, boundingRect.Size);
        }

        public static RectangleF PerformCenteredRotation(ref RadMatrix matrix, RectangleF bounds, float angle)
        {
            SizeF rectCenter = new SizeF(bounds.Width / 2, bounds.Height / 2);
            matrix.RotateAt(angle, PointF.Add(bounds.Location, rectCenter), MatrixOrder.Append);
            return TelerikHelper.GetBoundingRect(bounds, matrix);
        }

        /// <summary>
        /// Get bounding rectangle arround rotated one.
        /// </summary>
        /// <param name="rect">Rectangle that is to be rotated</param>
        /// <param name="matrix"></param>
        /// <returns>Returns the bounding rectangle around the rectangle
        /// that is rotated according to the given matrix</returns>
        public static Rectangle GetBoundingRect(Rectangle rect, RadMatrix matrix)
        {
            if (matrix.IsIdentity)
            {
                return rect;
            }

            Point[] points = new Point[4];
            points[0] = new Point(rect.Left, rect.Top);
            points[1] = new Point(rect.Right, rect.Top);
            points[2] = new Point(rect.Right, rect.Bottom);
            points[3] = new Point(rect.Left, rect.Bottom);

            TransformPoints(points, matrix.Elements);

            int left = Math.Min(Math.Min(points[0].X, points[1].X), Math.Min(points[2].X, points[3].X));
            int right = Math.Max(Math.Max(points[0].X, points[1].X), Math.Max(points[2].X, points[3].X));
            int top = Math.Min(Math.Min(points[0].Y, points[1].Y), Math.Min(points[2].Y, points[3].Y));
            int bottom = Math.Max(Math.Max(points[0].Y, points[1].Y), Math.Max(points[2].Y, points[3].Y));

            return Rectangle.FromLTRB(left, top, right, bottom);
        }

        public static RectangleF GetBoundingRect(RectangleF rect, RadMatrix matrix)
        {
            PointF[] points = new PointF[4];
            points[0] = new PointF(rect.Left, rect.Top);
            points[1] = new PointF(rect.Right, rect.Top);
            points[2] = new PointF(rect.Right, rect.Bottom);
            points[3] = new PointF(rect.Left, rect.Bottom);

            TransformPoints(points, matrix.Elements);

            float left = Math.Min(Math.Min(points[0].X, points[1].X), Math.Min(points[2].X, points[3].X));
            float right = Math.Max(Math.Max(points[0].X, points[1].X), Math.Max(points[2].X, points[3].X));
            float top = Math.Min(Math.Min(points[0].Y, points[1].Y), Math.Min(points[2].Y, points[3].Y));
            float bottom = Math.Max(Math.Max(points[0].Y, points[1].Y), Math.Max(points[2].Y, points[3].Y));

            return RectangleF.FromLTRB(left, top, right, bottom);
        }

        public static void TransformPoints(Point[] points, float[] elements)
        {
            for (int i = 0; i < points.Length; i++)
            {
                int x = points[i].X;
                int y = points[i].Y;

                points[i].X = (int)Math.Round(((float)x * elements[0]) + ((float)y * elements[2]) + elements[4]);
                points[i].Y = (int)Math.Round(((float)x * elements[1]) + ((float)y * elements[3]) + elements[5]);
            }
        }

        public static void TransformPoints(PointF[] points, float[] elements)
        {
            for (int i = 0; i < points.Length; i++)
            {
                float x = points[i].X;
                float y = points[i].Y;

                points[i].X = (x * elements[0]) + (y * elements[2]) + elements[4];
                points[i].Y = (x * elements[1]) + (y * elements[3]) + elements[5];
            }
        }

        #region RadToolTip, mnemonics, screen tips

        public static bool CanProcessMnemonic(Control control)
        {
            if (!control.Enabled || !control.Visible)
            {
                return false;
            }

            if (control.Parent != null)
            {
                return TelerikHelper.CanProcessMnemonic(control.Parent);
            }
            return true;
        }

        public static bool CanProcessMnemonicNoRecursive(Control control)
        {
            if (!control.Enabled || !control.Visible)
            {
                return false;
            }
            return true;
        }

        internal protected static bool IsPseudoMnemonic(char charCode, string text)
        {
            if (!string.IsNullOrEmpty(text) && !WindowsFormsUtils.ContainsMnemonic(text))
            {
                char upperCode = char.ToUpper(charCode, CultureInfo.CurrentCulture);
                if ((char.ToUpper(text[0], CultureInfo.CurrentCulture) == upperCode) ||
                    (char.ToLower(charCode, CultureInfo.CurrentCulture) == char.ToLower(text[0], CultureInfo.CurrentCulture)))
                {
                    return true;
                }
            }
            return false;
        }

        public static void SetDropShadow(IntPtr hWnd)
        {
            if (Environment.OSVersion.Version.Major >= 5 &&
                Environment.OSVersion.Version.Minor >= 1 &&
                Environment.OSVersion.Platform == PlatformID.Win32NT) 
            {
                if (NativeMethods.IsWindow(new HandleRef(null, hWnd)))
                {
                    int GCL_STYLE = -26;
                    int ClassLong = NativeMethods.GetClassLongPtr(new HandleRef(null, hWnd), GCL_STYLE).ToInt32();
                    if ((ClassLong & NativeMethods.CS_DROPSHADOW) == 0)
                    {
                        ClassLong += NativeMethods.CS_DROPSHADOW;
                        NativeMethods.SetClassLong(new HandleRef(null, hWnd), GCL_STYLE, (IntPtr)ClassLong);
                    }
                }
                //int WS_EX_COMPOSITED = 0x02000000;
                //int GWL_EXSTYLE = -20;
                //int WindowLong = GetWindowLong(hWnd, GWL_EXSTYLE);
                //SetWindowLong(hWnd, GWL_EXSTYLE, WindowLong | WS_EX_COMPOSITED);
            }
        }

        #endregion

        #region TabStrip & TreeView
        
        public static readonly ContentAlignment AnyBottomAlign;
        public static readonly ContentAlignment AnyCenterAlign;
        public static readonly ContentAlignment AnyLeftAlign;
        public static readonly ContentAlignment AnyMiddleAlign;
        public static readonly ContentAlignment AnyRightAlign;
        public static readonly ContentAlignment AnyTopAlign;

        public static bool IsBottomAligned(ContentAlignment align)
        {
            return ((align & TelerikHelper.AnyBottomAlign) != ((ContentAlignment)0));
        }

        public static bool IsRightAligned(ContentAlignment align)
        {
            return ((align & TelerikHelper.AnyRightAlign) != ((ContentAlignment)0));
        }

        public static System.Windows.Forms.Form CreateOutlineForm()
        {
            return CreateOutlineForm(null, SystemColors.Highlight);
        }

        public static RadShimControl /*System.Windows.Forms.Form*/ CreateOutlineForm1()
        {
            return CreateOutlineForm1(null, SystemColors.Highlight);
        }

        public static RadShimControl /*System.Windows.Forms.Form*/ CreateOutlineForm1(Bitmap image, Color color)
        {
            RadShimControl form = new RadShimControl();
            //System.Windows.Forms.Form form = new System.Windows.Forms.Form();
            try
            {
                form.Size = new Size(0, 0);
            }
            catch
            {
                form = new RadShimControl();//new System.Windows.Forms.Form();
            }
            if (color != Color.Empty)
            {
                form.BackColor = color;
            }
            //form.MinimizeBox = false;
            //form.MaximizeBox = false;
            //form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            if (DWMAPI.IsAlphaBlendingSupported)
                form.Opacity = .5;
            else
                form.BackColor = System.Windows.Forms.ControlPaint.LightLight(SystemColors.Highlight);
            //form.ShowInTaskbar = false;
            form.Text = "";
            form.Enabled = false;
            form.Visible = false;
            if (image != null)
            {
                form.BackgroundImage = image;
            }
            form.CreateControl();
            return form;
        }

        public static System.Windows.Forms.Form CreateOutlineForm(Bitmap image, Color color)
        {
            System.Windows.Forms.Form form = new System.Windows.Forms.Form();
            try
            {
                form.Size = new Size(0, 0);
            }
            catch
            {
                form = new System.Windows.Forms.Form();
            }
            if (color != Color.Empty)
            {
                form.BackColor = color;
            }
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            if (DWMAPI.IsAlphaBlendingSupported)
                form.Opacity = .5;
            else
                form.BackColor = System.Windows.Forms.ControlPaint.LightLight(SystemColors.Highlight);
            form.ShowInTaskbar = false;
            form.Text = "";
            form.Enabled = false;
            form.Visible = false;
            if (image != null)
            {
                form.BackgroundImage = image;
            }
            form.CreateControl();
            return form;
        }

        #endregion

        #region RadDock

        public static void AnimateWindow(IntPtr animatedControlHandle, int animationDuration, int dwFlags)
        {
            NativeMethods.AnimateWindow(animatedControlHandle, animationDuration, (NativeMethods.AnimateWindowFlags)dwFlags);
        }

        #endregion

        public static Stream GetStreamFromResource(Assembly assembly, string resourceUri)
        {
            Stream tempStream = assembly.GetManifestResourceStream(resourceUri);
            if (tempStream != null)
            {
                return tempStream;
            }
            return null;
        }

        public static Control ControlAtPoint(Point point)
        {
            IntPtr temp = NativeMethods.WindowFromPoint(point.X, point.Y);
            return Control.FromHandle(temp);
        }
    }
   
    //public class TelerikHelper
    //{
    //    public static readonly ContentAlignment AnyBottomAlign;
    //    public static readonly ContentAlignment AnyCenterAlign;
    //    public static readonly ContentAlignment AnyLeftAlign;
    //    public static readonly ContentAlignment AnyMiddleAlign;
    //    public static readonly ContentAlignment AnyRightAlign;
    //    public static readonly ContentAlignment AnyTopAlign;

    //    static TelerikHelper()
    //    {
    //        TelerikHelper.AnyRightAlign = ContentAlignment.BottomRight | (ContentAlignment.MiddleRight | ContentAlignment.TopRight);
    //        TelerikHelper.AnyLeftAlign = ContentAlignment.BottomLeft | (ContentAlignment.MiddleLeft | ContentAlignment.TopLeft);
    //        TelerikHelper.AnyTopAlign = ContentAlignment.TopRight | (ContentAlignment.TopCenter | ContentAlignment.TopLeft);
    //        TelerikHelper.AnyBottomAlign = ContentAlignment.BottomRight | (ContentAlignment.BottomCenter | ContentAlignment.BottomLeft);
    //        TelerikHelper.AnyMiddleAlign = ContentAlignment.MiddleRight | (ContentAlignment.MiddleCenter | ContentAlignment.MiddleLeft);
    //        TelerikHelper.AnyCenterAlign = ContentAlignment.BottomCenter | (ContentAlignment.MiddleCenter | ContentAlignment.TopCenter);
    //    }

    //    public static StringFormat CreateStringFormat(Control ctl, ContentAlignment textAlign, bool showEllipsis, bool useMnemonic)
    //    {
    //        StringFormat format1 = TelerikHelper.StringFormatForAlignment(textAlign);
    //        if (ctl.RightToLeft == RightToLeft.Yes)
    //        {
    //            format1.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
    //        }
    //        if (showEllipsis)
    //        {
    //            format1.Trimming = StringTrimming.EllipsisCharacter;
    //            format1.FormatFlags |= StringFormatFlags.LineLimit;
    //        }
    //        if (!useMnemonic)
    //        {
    //            format1.HotkeyPrefix = HotkeyPrefix.None;
    //        }
    //        // CHANGE:
    //        /*
    //        else if (ctl.ShowKeyboardCues)
    //        {
    //            format1.HotkeyPrefix = HotkeyPrefix.Show;
    //        }
    //         */
    //        else
    //        {
    //            format1.HotkeyPrefix = HotkeyPrefix.Hide;
    //        }
    //        if (ctl.AutoSize)
    //        {
    //            format1.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
    //        }
    //        return format1;
    //    }

    //    public static StringFormat StringFormatForAlignment(ContentAlignment align)
    //    {
    //        StringFormat format1 = new StringFormat();
    //        format1.Alignment = TelerikAlignHelper.TranslateAlignment(align);
    //        format1.LineAlignment = TelerikAlignHelper.TranslateLineAlignment(align);
    //        return format1;
    //    }

    //    public static TextFormatFlags CreateTextFormatFlags(Control ctl, ContentAlignment textAlign, bool showEllipsis, bool useMnemonic)
    //    {
    //        textAlign = TelerikHelper.RtlTranslateContent(ctl, textAlign);
    //        TextFormatFlags flags1 = TelerikHelper.TextFormatFlagsForAlignmentGDI(textAlign);
    //        flags1 |= (TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak);
    //        if (showEllipsis)
    //        {
    //            flags1 |= TextFormatFlags.EndEllipsis;
    //        }
    //        if (ctl.RightToLeft == RightToLeft.Yes)
    //        {
    //            flags1 |= TextFormatFlags.RightToLeft;
    //        }
    //        if (!useMnemonic)
    //        {
    //            return (flags1 | TextFormatFlags.NoPrefix);
    //        }
    //        // CHANGE:
    //        /*
    //        if (!ctl.ShowKeyboardCues)
    //        {
    //            flags1 |= TextFormatFlags.HidePrefix;
    //        }
    //         */
    //        return flags1;
    //    }

    //    public static TextFormatFlags TextFormatFlagsForAlignmentGDI(ContentAlignment align)
    //    {
    //        TextFormatFlags flags1 = TextFormatFlags.GlyphOverhangPadding;
    //        flags1 |= TelerikAlignHelper.TranslateAlignmentForGDI(align);
    //        return (flags1 | TelerikAlignHelper.TranslateLineAlignmentForGDI(align));
    //    }

    //    public static bool IsBottomAligned(ContentAlignment align)
    //    {
    //        return ((align & TelerikHelper.AnyBottomAlign) != ((ContentAlignment)0));
    //    }

    //    public static bool IsRightAligned(ContentAlignment align)
    //    {
    //        return ((align & TelerikHelper.AnyRightAlign) != ((ContentAlignment) 0));
    //    }

    //    public static ContentAlignment RtlTranslateContent(Control ctl, ContentAlignment align)
    //    {
    //        if (RightToLeft.Yes != ctl.RightToLeft)
    //        {
    //            return align;
    //        }
    //        if ((align & TelerikHelper.AnyTopAlign) != ((ContentAlignment)0))
    //        {
    //            ContentAlignment alignment1 = align;
    //            if (alignment1 != ContentAlignment.TopLeft)
    //            {
    //                if (alignment1 == ContentAlignment.TopRight)
    //                {
    //                    return ContentAlignment.TopLeft;
    //                }
    //            }
    //            else
    //            {
    //                return ContentAlignment.TopRight;
    //            }
    //        }
    //        if ((align & TelerikHelper.AnyMiddleAlign) != ((ContentAlignment) 0))
    //        {
    //            ContentAlignment alignment2 = align;
    //            if (alignment2 != ContentAlignment.MiddleLeft)
    //            {
    //                if (alignment2 == ContentAlignment.MiddleRight)
    //                {
    //                    return ContentAlignment.MiddleLeft;
    //                }
    //            }
    //            else
    //            {
    //                return ContentAlignment.MiddleRight;
    //            }
    //        }
    //        if ((align & TelerikHelper.AnyBottomAlign) == ((ContentAlignment) 0))
    //        {
    //            return align;
    //        }
    //        ContentAlignment alignment3 = align;
    //        if (alignment3 != ContentAlignment.BottomLeft)
    //        {
    //            if (alignment3 == ContentAlignment.BottomRight)
    //            {
    //                return ContentAlignment.BottomLeft;
    //            }
    //            return align;
    //        }
    //        return ContentAlignment.BottomRight;
    //    }

    //    public static void CheckParentingCycle(Control bottom, Control toFind)
    //    {
    //        Form form1 = null;
    //        Control control1 = null;
    //        for (Control control2 = bottom; control2 != null; control2 = control2.Parent)
    //        {
    //            control1 = control2;
    //            if (control2 == toFind)
    //            {
    //                throw new ArgumentException(SR.GetString("CircularOwner"));
    //            }
    //        }
    //        if ((control1 != null) && (control1 is Form))
    //        {
    //            Form form2 = (Form) control1;
    //            for (Form form3 = form2; form3 != null; form3 = form3.Owner)
    //            {
    //                form1 = form3;
    //                if (form3 == toFind)
    //                {
    //                    throw new ArgumentException(SR.GetString("CircularOwner"));
    //                }
    //            }
    //        }
    //        if ((form1 != null) && (form1.Parent != null))
    //        {
    //            TelerikHelper.CheckParentingCycle(form1.Parent, toFind);
    //        }
    //    }

    //    public static IntPtr GetSafeHandle(IWin32Window window)
    //    {
    //        IntPtr ptr1 = IntPtr.Zero;
    //        Control control1 = window as Control;
    //        if (control1 != null)
    //        {
    //            return control1.Handle;
    //        }
    //        IntSecurity.AllWindows.Demand();
    //        ptr1 = window.Handle;
    //        if ((ptr1 != IntPtr.Zero) && !NativeMethods.IsWindow(new HandleRef(null, ptr1)))
    //        {
    //            throw new Win32Exception(6);
    //        }
    //        return ptr1;
    //    }

    //    public static int DetermineTopChildIndex(Control parent)
    //    {
    //        int num1 = 0;
    //        num1 = 0;
    //        while (num1 < (parent.Controls.Count - 1))
    //        {
    //            Control control1 = parent.Controls[num1];
    //            if (control1.Site != null)
    //            {
    //                InheritanceAttribute attribute1 = (InheritanceAttribute) TypeDescriptor.GetAttributes(control1)[typeof(InheritanceAttribute)];
    //                InheritanceLevel level1 = InheritanceLevel.NotInherited;
    //                if (attribute1 != null)
    //                {
    //                    level1 = attribute1.InheritanceLevel;
    //                }
    //                if (level1 == InheritanceLevel.NotInherited)
    //                {
    //                    break;
    //                }
    //            }
    //            num1++;
    //        }
    //        return num1;
    //    }

    //    public static void FocusActiveControl(Control control)
    //    {
    //        if ((control != null) && control.Visible)
    //        {
    //            IntPtr ptr1 = NativeMethods.GetFocus();
    //            if ((ptr1 == IntPtr.Zero) || (Control.FromChildHandle(ptr1) != control))
    //            {
    //                NativeMethods.SetFocus(new HandleRef(control, control.Handle));
    //            }
    //        }
    //        else
    //        {
    //            ContainerControl control1 = control as ContainerControl;
    //            while ((control1 != null) && !control1.Visible)
    //            {
    //                Control control2 = control1.Parent;
    //                if (control2 == null)
    //                {
    //                    break;
    //                }
    //                control1 = control2.GetContainerControl() as ContainerControl;
    //            }
    //            if ((control1 != null) && control1.Visible)
    //            {
    //                NativeMethods.SetFocus(new HandleRef(control1, control1.Handle));
    //            }
    //        }
    //    }

    //    public static bool HasFocusableChild(Control control)
    //    {
    //        Control control1 = null;
    //        do
    //        {
    //            control1 = control.GetNextControl(control1, true);
    //        }
    //        while ((((control1 == null) || !control1.CanSelect) || !control1.TabStop) && (control1 != null));
    //        return (control1 != null);
    //    }

    //    public static bool ActivateControlInternal(ContainerControl controlBase, Control control, bool originator)
    //    {
    //        bool flag1 = true;
    //        bool flag2 = false;
    //        ContainerControl control1 = null;
    //        Control control2 = controlBase.Parent;
    //        if (control2 != null)
    //        {
    //            control1 = control2.GetContainerControl() as ContainerControl;
    //            if (control1 != null)
    //            {
    //                flag2 = control1.ActiveControl != controlBase;
    //            }
    //        }
    //        if ((control != controlBase.ActiveControl) || flag2)
    //        {
    //            if (flag2 && !ActivateControlInternal(control1, controlBase, false))
    //            {
    //                return false;
    //            }
    //            flag1 = TelerikHelper.AssignActiveControl(controlBase, (control == controlBase) ? null : control);
    //        }
    //        if (originator)
    //        {
    //            controlBase.ScrollControlIntoView(controlBase.ActiveControl);
    //        }
    //        return flag1;
    //    }

    //    public static bool IsDescendant(Control parent, Control descendant)
    //    {
    //        for (Control control1 = descendant; control1 != null; control1 = control1.Parent)
    //        {
    //            if (control1 == parent)
    //            {
    //                return true;
    //            }
    //        }
    //        return false;
    //    }

    //    public static bool AssignActiveControl(ContainerControl container, Control value)
    //    {
    //        if (container.ActiveControl != value)
    //        {
    //            container.ActiveControl = value;
    //        }
    //        return (container.ActiveControl == value);
    //    }

    //    public static RadShimControl /*System.Windows.Forms.Form*/ CreateOutlineForm1()
    //    {
    //        return CreateOutlineForm1(null, SystemColors.Highlight);
    //    }

    //    public static System.Windows.Forms.Form CreateOutlineForm()
    //    {
    //        return CreateOutlineForm(null, SystemColors.Highlight);
    //    }

    //    // Using Drop Shadow slows down display when using large window.
    //    public static void SetDropShadow(IntPtr hWnd)
    //    {
    //        if (Environment.OSVersion.Version.Major >= 5 &&
    //            Environment.OSVersion.Version.Minor >= 1 &&
    //            Environment.OSVersion.Platform == PlatformID.Win32NT) 
    //        {
    //            if (NativeMethods.IsWindow(new HandleRef(null, hWnd)))
    //            {
    //                int GCL_STYLE = -26;
    //                int ClassLong = NativeMethods.GetClassLongPtr(new HandleRef(null, hWnd), GCL_STYLE).ToInt32();
    //                if ((ClassLong & NativeMethods.CS_DROPSHADOW) == 0)
    //                {
    //                    ClassLong += NativeMethods.CS_DROPSHADOW;
    //                    NativeMethods.SetClassLong(new HandleRef(null, hWnd), GCL_STYLE, (IntPtr)ClassLong);
    //                }
    //            }
    //            //int WS_EX_COMPOSITED = 0x02000000;
    //            //int GWL_EXSTYLE = -20;
    //            //int WindowLong = GetWindowLong(hWnd, GWL_EXSTYLE);
    //            //SetWindowLong(hWnd, GWL_EXSTYLE, WindowLong | WS_EX_COMPOSITED);
    //        }
    //    }

    //    public static RadShimControl /*System.Windows.Forms.Form*/ CreateOutlineForm1(Bitmap image, Color color)
    //    {
    //        RadShimControl form = new RadShimControl();
    //        //System.Windows.Forms.Form form = new System.Windows.Forms.Form();
    //        try
    //        {
    //            form.Size = new Size(0, 0);
    //        }
    //        catch
    //        {
    //            form = new RadShimControl();//new System.Windows.Forms.Form();
    //        }
    //        if (color != Color.Empty)
    //        {
    //            form.BackColor = color; 
    //        }
    //        //form.MinimizeBox = false;
    //        //form.MaximizeBox = false;
    //        //form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
    //        if (TelerikPaintHelper.IsAlphaBlendingSupported)
    //            form.Opacity = .5;
    //        else
    //            form.BackColor = System.Windows.Forms.ControlPaint.LightLight(SystemColors.Highlight);
    //        //form.ShowInTaskbar = false;
    //        form.Text = "";
    //        form.Enabled = false;
    //        form.Visible = false;
    //        if (image != null)
    //        {
    //            form.BackgroundImage = image;
    //        }
    //        form.CreateControl();
    //        return form;
    //    }

    //    public static System.Windows.Forms.Form CreateOutlineForm(Bitmap image, Color color)
    //    {
    //        System.Windows.Forms.Form form = new System.Windows.Forms.Form();
    //        try
    //        {
    //            form.Size = new Size(0, 0);
    //        }
    //        catch
    //        {
    //            form = new System.Windows.Forms.Form();
    //        }
    //        if (color != Color.Empty)
    //        {
    //            form.BackColor = color;
    //        }
    //        form.MinimizeBox = false;
    //        form.MaximizeBox = false;
    //        form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
    //        if (TelerikPaintHelper.IsAlphaBlendingSupported)
    //            form.Opacity = .5;
    //        else
    //            form.BackColor = System.Windows.Forms.ControlPaint.LightLight(SystemColors.Highlight);
    //        form.ShowInTaskbar = false;
    //        form.Text = "";
    //        form.Enabled = false;
    //        form.Visible = false;
    //        if (image != null)
    //        {
    //            form.BackgroundImage = image;
    //        }
    //        form.CreateControl();
    //        return form;
    //    }

    //    public static RadShimControl /*System.Windows.Forms.Form*/ CreateOutlineForm1(Bitmap image)
    //    {
    //        RadShimControl /*System.Windows.Forms.Form*/ form = CreateOutlineForm1();
    //        if (image != null)
    //        {
    //            form.BackgroundImage = image;
    //        }
    //        return form;
    //    }

    //    public static System.Windows.Forms.Form CreateOutlineForm(Bitmap image)
    //    {
    //        System.Windows.Forms.Form form = CreateOutlineForm();
    //        if (image != null)
    //        {
    //            form.BackgroundImage = image;
    //        }
    //        return form;
    //    }



    //    public static List<Control> GetChildWindowsZOrder(Point point)
    //    {
    //        List<Control> list = new List<Control>(2);
    //        IntPtr owner = NativeMethods.GetDesktopWindow();
    //        if (owner != IntPtr.Zero)
    //        {
    //            IntPtr top = NativeMethods.GetTopWindow(new HandleRef(null, owner));
    //            IntPtr next = top;
    //            Control tempControl = null;
    //            while (next != IntPtr.Zero)
    //            {
    //                tempControl = Control.FromHandle(next);
    //                if (tempControl != null && tempControl is Form) 
    //                {
    //                    list.Add(tempControl);
    //                }
    //                next = NativeMethods.GetWindow(new HandleRef(null, next), NativeMethods.GW_HWNDNEXT);
    //            }
    //            if (list.Count>0)
    //            {
    //                //Debug.WriteLine(list.Count);
    //                list.Reverse();
    //                return list;
    //            }
    //        }
    //        return null;
    //    }

    //    public static Control ChildControlAtPoint(Control control, Point point)
    //    {
    //        IntPtr temp = NativeMethods.ChildWindowFromPoint(control, point.X, point.Y);
    //        return Control.FromHandle(temp);
    //    }
    //    public static Control RealChildControlAtPoint(Control control, Point point)
    //    {
    //        IntPtr temp = NativeMethods.RealChildWindowFromPoint(control, point.X, point.Y);
    //        return Control.FromHandle(temp);
    //    }
    //    public static Control ControlAtPoint(Point point)
    //    {
    //        IntPtr temp = NativeMethods.WindowFromPoint(point.X, point.Y);
    //        return Control.FromHandle(temp);
    //    }
    //    //public static Control GetControlAtPoint(IList controls, Point point)
    //    //{
    //    //    return GetControlAtPoint(controls, point.X, point.Y);
    //    //}
    //    //public static Control GetControlAtPoint(IList controls, int x, int y)
    //    //{
    //    //    IEnumerator enumerator1 = controls.GetEnumerator();
    //    //    Control control1 = null;
    //    //    while (enumerator1.MoveNext())
    //    //    {
    //    //        Control control2 = (Control)enumerator1.Current;
    //    //        Control control3 = GetSitedParent(null, control2);
    //    //        Rectangle rectangle1 = control2.Bounds;
    //    //        rectangle1 = control3.RectangleToScreen(rectangle1);
    //    //        if (rectangle1.Contains(x, y))
    //    //        {
    //    //            control1 = control2;
    //    //        }
    //    //    }
    //    //    return control1;
    //    //}

    //    //public static Control GetControlAtPoint(System.Windows.Forms.Control.ControlCollection controls, int x, int y)
    //    //{
    //    //    IEnumerator enumerator1 = controls.GetEnumerator();
    //    //    Control control1 = null;
    //    //    while (enumerator1.MoveNext())
    //    //    {
    //    //        Control control2 = (Control)enumerator1.Current;
    //    //        Control control3 = GetSitedParent(null, control2);
    //    //        Rectangle rectangle1 = control2.Bounds;
    //    //        rectangle1 = control3.RectangleToScreen(rectangle1);
    //    //        if (rectangle1.Contains(x, y))
    //    //        {
    //    //            control1 = control2;
    //    //        }
    //    //    }
    //    //    return control1;
    //    //}
    //    //public static Control GetControlAtPoint(IDesignerHost host, IList controls, int x, int y)
    //    //{
    //    //    IEnumerator enumerator1 = controls.GetEnumerator();
    //    //    Control control1 = null;
    //    //    while (enumerator1.MoveNext())
    //    //    {
    //    //        Control control2 = (Control)enumerator1.Current;
    //    //        Control control3 = GetSitedParent(host, control2);
    //    //        Rectangle rectangle1 = control2.Bounds;
    //    //        rectangle1 = control3.RectangleToScreen(rectangle1);
    //    //        if (rectangle1.Contains(x, y))
    //    //        {
    //    //            control1 = control2;
    //    //        }
    //    //    }
    //    //    return control1;
    //    //}

    //    //public static Control GetSitedParent(IDesignerHost host, Control instance)
    //    //{
    //    //      Control control1 = instance.Parent;
    //    //      while (control1 != null)
    //    //      {
    //    //            ISite site1 = control1.Site;
    //    //            IContainer container1 = null;
    //    //            if (site1 != null)
    //    //            {
    //    //                  container1 = site1.Container;
    //    //            }
    //    //            container1 = DesignerUtils.CheckForNestedContainer(container1);
    //    //            if ((site1 != null) && (container1 == host))
    //    //            {
    //    //                  break;
    //    //            }
    //    //            control1 = control1.Parent;
    //    //      }
    //    //      return control1;
    //    //}

    //    public static ImageFormat GetImageFormat(System.Drawing.Image img)
    //    {
    //        ImageFormat rawFormat = img.RawFormat;

    //        ImageCodecInfo[] decoders = ImageCodecInfo.GetImageDecoders();
    //        if (CodecInfoExists(decoders, rawFormat))
    //        {
    //            return rawFormat;
    //        }

    //        ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
    //        if (CodecInfoExists(encoders, rawFormat))
    //        {
    //            return rawFormat;
    //        }

    //        // The following image formats are not in the above arrays...
    //        ImageFormat res = rawFormat;
    //        if (rawFormat != ImageFormat.MemoryBmp && rawFormat != ImageFormat.Exif)
    //        {
    //            // Force bitmap format
    //            res = ImageFormat.Bmp;
    //        }
            
    //        return res;
    //    }

    //    private static int GetCodecInfoIndex(ImageCodecInfo[] codecs, ImageFormat rawFormat)
    //    {
    //        for (int i = 0; i < codecs.Length; i++)
    //        {
    //            ImageCodecInfo codecInfo = codecs[i];
    //            string description = codecInfo.FormatDescription;
    //            string codecName = codecInfo.CodecName;
    //            if (rawFormat.Guid == codecInfo.FormatID)
    //            {
    //                return i;
    //            }
    //        }
    //        return -1;
    //    }

    //    private static bool CodecInfoExists(ImageCodecInfo[] codecs, ImageFormat rawFormat)
    //    {
    //        int index = GetCodecInfoIndex(codecs, rawFormat);
    //        if (index >= 0 && index < codecs.Length)
    //        {
    //            return true;
    //        }
    //        return false;
    //    }

    //    private static string IconToString(System.Drawing.Icon icon)
    //    {
    //        if (icon == null)
    //        {
    //            return string.Empty;
    //        }
    //        System.IO.MemoryStream mem = new System.IO.MemoryStream(1024);
    //        // TODO: Beta 2 issue with the ImageFormat. RawFormat on image object does not return the actual image format
    //        // Right now it is hard coded to PNG but in final version we should get the original image format
    //        icon.Save(mem);

    //        System.Text.StringBuilder sb = new System.Text.StringBuilder();
    //        System.IO.StringWriter sw = new System.IO.StringWriter(sb);

    //        System.Xml.XmlTextWriter xt = new System.Xml.XmlTextWriter(sw);

    //        //xml.SetAttribute("encoding", "binhex");
    //        //xt.WriteBase64(mem.GetBuffer(),0,(int)mem.Length);
    //        xt.WriteBinHex(mem.GetBuffer(), 0, (int)mem.Length);

    //        return sb.ToString();
    //    }

    //    private static System.Drawing.Icon IconFromString(string encodedIcon)
    //    {
    //        System.Drawing.Icon img = null;
    //        if (string.IsNullOrEmpty(encodedIcon))
    //        {
    //            return null;
    //        }
    //        //bool bDecodeBinHex = false;
    //        //if (xml.HasAttribute("encoding") && xml.GetAttribute("encoding") == "binhex")
    //        //    bDecodeBinHex = true;
    //        System.IO.StringReader sr = new System.IO.StringReader(encodedIcon);
    //        System.Xml.XmlTextReader xr = new System.Xml.XmlTextReader(sr);
    //        System.IO.MemoryStream mem = new System.IO.MemoryStream(1024);
    //        // Skip <image> to data
    //        xr.Read();

    //        byte[] base64 = new byte[1024];
    //        int base64len = 0;
    //        //if (bDecodeBinHex)
    //        //{
    //        do
    //        {
    //            base64len = xr.ReadBinHex(base64, 0, 1024);
    //            if (base64len > 0)
    //                mem.Write(base64, 0, base64len);

    //        } while (base64len != 0);
    //        //}
    //        //else
    //        //{
    //        //    do
    //        //    {
    //        //        base64len = xr.ReadBase64(base64, 0, 1024);
    //        //        if (base64len > 0)
    //        //            mem.Write(base64, 0, base64len);

    //        //    } while (base64len != 0);
    //        //}
    //        mem.Position = 0;
    //        img = new System.Drawing.Icon(mem);

    //        return img;
    //    }

    //    public static string ImageToString(System.Drawing.Image image)
    //    {
    //        if (image == null)
    //        {
    //            return string.Empty;
    //        }

    //        using (System.IO.MemoryStream mem = new System.IO.MemoryStream(1024))
    //        {
    //            // TODO: Beta 2 issue with the ImageFormat. RawFormat on image object does not return the actual image format
    //            // Right now it is hard coded to PNG but in final version we should get the original image format
    //            image.Save(mem, GetImageFormat(image));
				
    //            string str = Convert.ToBase64String(mem.ToArray(), 0, (int)mem.Length);
    //            return str;
    //        }
    //    }

    //    public static System.Drawing.Image ImageFromString(string encodedImage)
    //    {	
    //        System.Drawing.Image img = null;
    //        if (string.IsNullOrEmpty(encodedImage))
    //        {
    //            return null;
    //        }

    //        byte[] bytes = Convert.FromBase64String(encodedImage);
    //        MemoryStream mem = new MemoryStream(bytes);			
    //        img = System.Drawing.Image.FromStream(mem);

    //        System.Drawing.Image res = null;

    //        if (img.PixelFormat == PixelFormat.Format24bppRgb)
    //        {
    //            res = new Bitmap(img);
    //            (res as Bitmap).MakeTransparent(Color.Magenta);
    //        }
    //        else
    //        {
    //            BitmapData bitmapData = (img as Bitmap).LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
    //            Bitmap returnBitmap = new Bitmap(bitmapData.Width, bitmapData.Height, bitmapData.Stride, PixelFormat.Format32bppArgb, bitmapData.Scan0);
    //            res = new Bitmap(returnBitmap);
    //        }
			
    //        // Create new bitmap since the old one is connected with the stream somehow and
    //        // becomes invalid when the stream is disposed
    //        //System.Drawing.Image res = new Bitmap(img);
    //        mem.Dispose();
    //        img.Dispose();
    //        //(res as Bitmap).MakeTransparent(Color.Magenta); 
    //        return res;
    //    }

    //    public static void AnimateWindow(IntPtr animatedControlHandle, int animationDuration, int dwFlags)
    //    {
    //        NativeMethods.AnimateWindow(animatedControlHandle, animationDuration, (NativeMethods.AnimateWindowFlags)dwFlags);
    //    }
        
    //    public static bool IsOSThemeSupported()
    //    {
    //        if (Environment.OSVersion.Version.Major >= 5 &&
    //            Environment.OSVersion.Version.Minor >= 1 && 
    //            Environment.OSVersion.Platform == PlatformID.Win32NT) 
    //        { 
    //            return System.Windows.Forms.OSFeature.Feature.IsPresent(System.Windows.Forms.OSFeature.Themes);
    //        }
    //        return false;
    //    }

    //    public static bool IsOSTransparencySupported()
    //    {
    //        if (Environment.OSVersion.Version.Major >= 5 &&
    //            Environment.OSVersion.Version.Minor >= 1 &&
    //            Environment.OSVersion.Platform == PlatformID.Win32NT) 
    //        {
    //            return System.Windows.Forms.OSFeature.Feature.IsPresent(System.Windows.Forms.OSFeature.LayeredWindows);
    //        }
    //        return false;
    //    }

    //    public static Stream GetStreamFromResource(Assembly assembly, string resourceUri)
    //    {
    //        Stream tempStream = assembly.GetManifestResourceStream(resourceUri);
    //        if (tempStream != null)
    //        {
    //            return tempStream;
    //        }
    //        return null;
    //    }

    //    public static Control GetParent(Control control)
    //    {
    //        try
    //        {
    //            UIPermission perm = new UIPermission(UIPermissionWindow.AllWindows);

    //            perm.Assert();

    //            return control.Parent;
    //        }
    //        catch (System.Security.SecurityException)
    //        {
    //            return null;
    //        }
    //    }

    //    public static void Focus(Control control)
    //    {
    //        if (control == null || !control.CanFocus)
    //        {
    //            return;
    //        }
    //        try
    //        {
    //            UIPermission perm = new UIPermission(UIPermissionWindow.AllWindows);
    //            perm.Assert();
    //            control.Focus();
    //        }
    //        catch (System.Security.SecurityException) { }
    //    }

    //    public static void ResetCursorPosition()
    //    {
    //        try
    //        {
    //            UIPermission perm = new UIPermission(UIPermissionWindow.AllWindows);

    //            perm.Assert();

    //            Cursor.Position = Cursor.Position;
    //        }
    //        catch (System.Security.SecurityException)
    //        {
    //        }
    //    }

    //    public static bool SafeCompareStrings(string string1, string string2, bool ignoreCase)
    //    {
    //        if ((string1 == null) || (string2 == null))
    //        {
    //            return false;
    //        }
    //        if (string1.Length != string2.Length)
    //        {
    //            return false;
    //        }
    //        return (string.Compare(string1, string2, ignoreCase, CultureInfo.InvariantCulture) == 0);
    //    }

    //    public static bool ContainsMnemonic(string text)
    //    {
    //        if (text == null)
    //            return false;

    //        int textLength = text.Length;
    //        int ampIndex = text.IndexOf('&', 0);
    //        if ((ampIndex >= 0) && (ampIndex <= (textLength - 2)))
    //        {
    //            int charIndex = text.IndexOf('&', ampIndex + 1);
    //            if (charIndex == -1)
    //                return true;
    //        }
    //        return false;
    //    }

    //    public static string TextWithoutMnemonics(string text)
    //    {
    //        if (text == null)
    //            return null;

    //        int ampIndex = text.IndexOf('&');
    //        if (ampIndex == -1)
    //            return text;

    //        StringBuilder stringBuilder = new StringBuilder(text.Substring(0, ampIndex));
    //        while (ampIndex < text.Length)
    //        {
    //            if (text[ampIndex] == '&')
    //            {
    //                ampIndex++;
    //            }
    //            if (ampIndex < text.Length)
    //            {
    //                stringBuilder.Append(text[ampIndex]);
    //            }
    //            ampIndex++;
    //        }
    //        return stringBuilder.ToString();
    //    }

    //    public static char GetMnemonic(string text, bool bConvertToUpperCase)
    //    {
    //        char nullChar = '\0';
    //        if (text == null)
    //            return nullChar;

    //        int textLength = text.Length;
    //        for (int i=0; i<(textLength - 1); i++)
    //        {
    //            if (text[i] != '&')
    //                continue;

    //            if (text[i + 1] == '&')
    //            {
    //                i++;
    //            }
    //            else
    //            {
    //                if (bConvertToUpperCase)
    //                {
    //                    return char.ToUpper(text[i + 1], CultureInfo.CurrentCulture);
    //                }
    //                return char.ToLower(text[i + 1], CultureInfo.CurrentCulture);
    //            }
    //        }
    //        return nullChar;
    //    }

    //    public static void InvalidateNCBorderArea(Control control)
    //    {
    //        if (control == null || !control.IsHandleCreated)
    //        {
    //            return;
    //        }
    //        NativeMethods.SetWindowPos(control.Handle, IntPtr.Zero, 0, 0, 0, 0,
    //            NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOZORDER |
    //            NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_FRAMECHANGED | NativeMethods.SWP_DRAWFRAME);
    //    }

    //    public static void InvalidateNCArea(Control control)
    //    {
    //        if (control == null || !control.IsHandleCreated)
    //        {
    //            return;
    //        }
    //        InvalidateNCArea(control.Handle);
    //    }

    //    public static void InvalidateNCArea(IntPtr handle)
    //    {
    //        NativeMethods.SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0,
    //        NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOACTIVATE |
    //        NativeMethods.SWP_DRAWFRAME | NativeMethods.SWP_NOSENDCHANGING);
    //    }

    //    //public static bool IsPseudoMnemonic(char charCode, string text)
    //    //{
    //    //    if (!string.IsNullOrEmpty(text) && !TelerikHelper.ContainsMnemonic(text))
    //    //    {
    //    //        char upperCode = char.ToUpper(charCode, CultureInfo.CurrentCulture);
    //    //        char upperFirstLetter = char.ToUpper(text[0], CultureInfo.CurrentCulture);
    //    //        if ((upperFirstLetter == upperCode) || (char.ToLower(charCode, CultureInfo.CurrentCulture) == char.ToLower(text[0], CultureInfo.CurrentCulture)))
    //    //            return true;
    //    //    }
    //    //    return false;
    //    //}

    //    internal protected static bool IsPseudoMnemonic(char charCode, string text)
    //    {
    //        if (!string.IsNullOrEmpty(text) && !WindowsFormsUtils.ContainsMnemonic(text))
    //        {
    //            char upperCode = char.ToUpper(charCode, CultureInfo.CurrentCulture);
    //            if ((char.ToUpper(text[0], CultureInfo.CurrentCulture) == upperCode) || 
    //                (char.ToLower(charCode, CultureInfo.CurrentCulture) == char.ToLower(text[0], CultureInfo.CurrentCulture)))
    //            {
    //                return true;
    //            }
    //        }
    //        return false;
    //    }

    //    public static bool CanProcessMnemonic(Control control)
    //    {
    //        if (!control.Enabled || !control.Visible)
    //        {
    //            return false;
    //        }

    //        if (control.Parent != null)
    //        {
    //            return TelerikHelper.CanProcessMnemonic(control.Parent);
    //        }
    //        return true;
    //    }

    //    public static bool CanProcessMnemonicNoRecursive(Control control)
    //    {
    //        if (!control.Enabled || !control.Visible)
    //        {
    //            return false;
    //        }
    //        return true;
    //    }
		
    //    public static RectangleF PerformTopLeftRotation(ref RadMatrix matrix, RectangleF bounds, float angle)
    //    {
    //        RadMatrix temp = RadMatrix.Identity;
    //        RectangleF boundingRect = PerformCenteredRotation(ref temp, bounds, angle);
    //        temp.Translate(bounds.X - boundingRect.X, bounds.Y - boundingRect.Y, MatrixOrder.Append);
    //        matrix.Multiply(temp, MatrixOrder.Append);
    //        return new RectangleF(bounds.Location, boundingRect.Size);
    //    }

    //    public static RectangleF PerformCenteredRotation(ref RadMatrix matrix, RectangleF bounds, float angle)
    //    {
    //        SizeF rectCenter = new SizeF(bounds.Width / 2, bounds.Height / 2);
    //        matrix.RotateAt(angle, PointF.Add(bounds.Location, rectCenter), MatrixOrder.Append);
    //        return TelerikHelper.GetBoundingRect(bounds, matrix);
    //    }

    //    public static Rectangle PerformTopLeftRotation(ref RadMatrix matrix, Rectangle bounds, float angle)
    //    {
    //        RadMatrix temp = RadMatrix.Identity; ;
    //        Rectangle boundingRect = PerformCenteredRotation(ref temp, bounds, angle);
    //        temp.Translate(bounds.X - boundingRect.X, bounds.Y - boundingRect.Y, MatrixOrder.Append);
    //        matrix.Multiply(temp, MatrixOrder.Append);
    //        return new Rectangle(bounds.Location, boundingRect.Size);
    //    }

    //    public static Rectangle PerformCenteredRotation(ref RadMatrix matrix, Rectangle bounds, float angle)
    //    {
    //        SizeF rectCenter = new SizeF(bounds.Width / 2, bounds.Height / 2);
    //        matrix.RotateAt(angle, PointF.Add(bounds.Location, rectCenter), MatrixOrder.Append);
    //        return TelerikHelper.GetBoundingRect(bounds, matrix);
    //    }

    //    /// <summary>
    //    /// Get bounding rectangle arround rotated one.
    //    /// </summary>
    //    /// <param name="rect">Rectangle that is to be rotated</param>
    //    /// <param name="matrix"></param>
    //    /// <returns>Returns the bounding rectangle around the rectangle
    //    /// that is rotated according to the given matrix</returns>
    //    public static Rectangle GetBoundingRect(Rectangle rect, RadMatrix matrix)
    //    {
    //        if (matrix.IsIdentity)
    //        {
    //            return rect;
    //        }

    //        Point[] points = new Point[4];
    //        points[0] = new Point(rect.Left, rect.Top);
    //        points[1] = new Point(rect.Right, rect.Top);
    //        points[2] = new Point(rect.Right, rect.Bottom);
    //        points[3] = new Point(rect.Left, rect.Bottom);

    //        TransformPoints(points, matrix.Elements);

    //        int left = Math.Min(Math.Min(points[0].X, points[1].X), Math.Min(points[2].X, points[3].X));
    //        int right = Math.Max(Math.Max(points[0].X, points[1].X), Math.Max(points[2].X, points[3].X));
    //        int top = Math.Min(Math.Min(points[0].Y, points[1].Y), Math.Min(points[2].Y, points[3].Y));
    //        int bottom = Math.Max(Math.Max(points[0].Y, points[1].Y), Math.Max(points[2].Y, points[3].Y));

    //        return Rectangle.FromLTRB(left, top, right, bottom);
    //    }

    //    public static RectangleF GetBoundingRect(RectangleF rect, RadMatrix matrix)
    //    {
    //        PointF[] points = new PointF[4];
    //        points[0] = new PointF(rect.Left, rect.Top);
    //        points[1] = new PointF(rect.Right, rect.Top);
    //        points[2] = new PointF(rect.Right, rect.Bottom);
    //        points[3] = new PointF(rect.Left, rect.Bottom);

    //        TransformPoints(points, matrix.Elements);

    //        float left = Math.Min(Math.Min(points[0].X, points[1].X), Math.Min(points[2].X, points[3].X));
    //        float right = Math.Max(Math.Max(points[0].X, points[1].X), Math.Max(points[2].X, points[3].X));
    //        float top = Math.Min(Math.Min(points[0].Y, points[1].Y), Math.Min(points[2].Y, points[3].Y));
    //        float bottom = Math.Max(Math.Max(points[0].Y, points[1].Y), Math.Max(points[2].Y, points[3].Y));

    //        return RectangleF.FromLTRB(left, top, right, bottom);
    //    }

    //    public static void TransformPoints(Point[] points, float[] elements)
    //    {
    //        for (int i = 0; i < points.Length; i++)
    //        {
    //            int x = points[i].X;
    //            int y = points[i].Y;

    //            points[i].X = (int)Math.Round(((float)x * elements[0]) + ((float)y * elements[2]) + elements[4]);
    //            points[i].Y = (int)Math.Round(((float)x * elements[1]) + ((float)y * elements[3]) + elements[5]);
    //        }
    //    }

    //    public static void TransformPoints(PointF[] points, float[] elements)
    //    {
    //        for (int i = 0; i < points.Length; i++)
    //        {
    //            float x = points[i].X;
    //            float y = points[i].Y;

    //            points[i].X = (x * elements[0]) + (y * elements[2]) + elements[4];
    //            points[i].Y = (x * elements[1]) + (y * elements[3]) + elements[5];
    //        }
    //    }
    //}
}