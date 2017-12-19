namespace Telerik.WinControls
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows.Forms;
 
    internal sealed class WindowsFormsUtils
    {   
        static WindowsFormsUtils()
        {
            WindowsFormsUtils.UninitializedSize = new Size(-7199369, -5999471);
            WindowsFormsUtils.AnyRightAlign = ContentAlignment.BottomRight | ContentAlignment.MiddleRight | ContentAlignment.TopRight;
            WindowsFormsUtils.AnyLeftAlign = ContentAlignment.BottomLeft | ContentAlignment.MiddleLeft | ContentAlignment.TopLeft;
            WindowsFormsUtils.AnyTopAlign = ContentAlignment.TopRight | ContentAlignment.TopCenter | ContentAlignment.TopLeft;
            WindowsFormsUtils.AnyBottomAlign = ContentAlignment.BottomRight | ContentAlignment.BottomCenter | ContentAlignment.BottomLeft;
            WindowsFormsUtils.AnyMiddleAlign = ContentAlignment.MiddleRight | ContentAlignment.MiddleCenter | ContentAlignment.MiddleLeft;
            WindowsFormsUtils.AnyCenterAlign = ContentAlignment.BottomCenter | ContentAlignment.MiddleCenter | ContentAlignment.TopCenter;
        }

        internal static string AssertControlInformation(bool condition, Control control)
        {
            if (condition)
            {
                return string.Empty;
            }
            return WindowsFormsUtils.GetControlInformation(control.Handle);
        }

        internal static Rectangle ConstrainToBounds(Rectangle constrainingBounds, Rectangle bounds)
        {
            if (!constrainingBounds.Contains(bounds))
            {
                bounds.Size = new Size(Math.Min(constrainingBounds.Width - 2, bounds.Width), Math.Min(constrainingBounds.Height - 2, bounds.Height));
                if (bounds.Right > constrainingBounds.Right)
                {
                    bounds.X = constrainingBounds.Right - bounds.Width;
                }
                else if (bounds.Left < constrainingBounds.Left)
                {
                    bounds.X = constrainingBounds.Left;
                }
                if (bounds.Bottom > constrainingBounds.Bottom)
                {
                    bounds.Y = (constrainingBounds.Bottom - 1) - bounds.Height;
                    return bounds;
                }
                if (bounds.Top < constrainingBounds.Top)
                {
                    bounds.Y = constrainingBounds.Top;
                }
            }
            return bounds;
        }

        internal static Rectangle ConstrainToScreenBounds(Rectangle bounds)
        {
            return WindowsFormsUtils.ConstrainToBounds(Screen.FromRectangle(bounds).Bounds, bounds);
        }

        internal static Rectangle ConstrainToScreenWorkingAreaBounds(Rectangle bounds)
        {
            return WindowsFormsUtils.ConstrainToBounds(Screen.GetWorkingArea(bounds), bounds);
        }

        public static bool ContainsMnemonic(string text)
        {
            if (text != null)
            {
                int num1 = text.Length;
                int num2 = text.IndexOf('&', 0);
                if ((num2 >= 0) && (num2 <= (num1 - 2)))
                {
                    int num3 = text.IndexOf('&', num2 + 1);
                    if (num3 == -1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /*public static Graphics CreateMeasurementGraphics()
        {
            return Graphics.FromHdcInternal(WindowsGraphicsCacheManager.MeasurementGraphics.DeviceContext.Hdc);
        }*/

        internal static string EscapeTextWithAmpersands(string text)
        {
            if (text == null)
            {
                return null;
            }
            int num1 = text.IndexOf('&');
            if (num1 == -1)
            {
                return text;
            }
            StringBuilder builder1 = new StringBuilder(text.Substring(0, num1));
            while (num1 < text.Length)
            {
                if (text[num1] == '&')
                {
                    builder1.Append("&");
                }
                if (num1 < text.Length)
                {
                    builder1.Append(text[num1]);
                }
                num1++;
            }
            return builder1.ToString();
        }

        internal static int GetCombinedHashCodes(params int[] args)
        {
            int num1 = -757577119;
            for (int num2 = 0; num2 < args.Length; num2++)
            {
                num1 = (args[num2] ^ num1) * -1640531535;
            }
            return num1;
        }

        public static string GetComponentName(IComponent component, string defaultNameValue)
        {
            string text1 = string.Empty;
            if (string.IsNullOrEmpty(defaultNameValue))
            {
                if (component.Site != null)
                {
                    text1 = component.Site.Name;
                }
                if (text1 == null)
                {
                    text1 = string.Empty;
                }
                return text1;
            }
            return defaultNameValue;
        }

        internal static string GetControlInformation(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
            {
                return "Handle is IntPtr.Zero";
            }
            return "";
        }

        public static char GetMnemonic(string text, bool bConvertToUpperCase)
        {
            char ch1 = '\0';
            if (text != null)
            {
                int num1 = text.Length;
                for (int num2 = 0; num2 < (num1 - 1); num2++)
                {
                    if (text[num2] == '&')
                    {
                        if (text[num2 + 1] == '&')
                        {
                            num2++;
                        }
                        else
                        {
                            if (bConvertToUpperCase)
                            {
                                ch1 = char.ToUpper(text[num2 + 1], CultureInfo.CurrentCulture);
                                break;
                            }
                            ch1 = char.ToLower(text[num2 + 1], CultureInfo.CurrentCulture);
                            break;
                        }
                    }
                }
            }
            return ch1;
        }

        //public static HandleRef GetRootHWnd(HandleRef hwnd)
        //{
        //    IntPtr ptr1 = NativeMethods.GetAncestor(new HandleRef(hwnd, hwnd.Handle), 2);
        //    return new HandleRef(hwnd.Wrapper, ptr1);
        //}

        //public static HandleRef GetRootHWnd(Control control)
        //{
        //    return WindowsFormsUtils.GetRootHWnd(new HandleRef(control, control.Handle));
        //}

        public static int RotateLeft(int value, int nBits)
        {
            nBits = nBits % 0x20;
            return ((value << (nBits & 0x1f)) | (value >> ((0x20 - nBits) & 0x1f)));
        }

        public static bool SafeCompareStrings(string string1, string string2, bool ignoreCase)
        {
            if ((string1 == null) || (string2 == null))
            {
                return false;
            }
            if (string1.Length != string2.Length)
            {
                return false;
            }
            return (string.Compare(string1, string2, ignoreCase, CultureInfo.InvariantCulture) == 0);
        }

        public static string TextWithoutMnemonics(string text)
        {
            if (text == null)
            {
                return null;
            }
            int num1 = text.IndexOf('&');
            if (num1 == -1)
            {
                return text;
            }
            StringBuilder builder1 = new StringBuilder(text.Substring(0, num1));
            while (num1 < text.Length)
            {
                if (text[num1] == '&')
                {
                    num1++;
                }
                if (num1 < text.Length)
                {
                    builder1.Append(text[num1]);
                }
                num1++;
            }
            return builder1.ToString();
        }

        //public static Point TranslatePoint(Point point, Control fromControl, Control toControl)
        //{
        //    NativeMethods.POINT point1 = new NativeMethods.POINT(point.X, point.Y);
        //    NativeMethods.MapWindowPoints(new HandleRef(fromControl, fromControl.Handle), new HandleRef(toControl, toControl.Handle), point1, 1);
        //    return new Point(point1.x, point1.y);
        //}


        //public static Point LastCursorPoint
        //{
        //    get
        //    {
        //        int num1 = NativeMethods.GetMessagePos();
        //        return new Point(NativeMethods.Util.SignedLOWORD(num1), NativeMethods.Util.SignedHIWORD(num1));
        //    }
        //}

       


        public static readonly ContentAlignment AnyBottomAlign;
        public static readonly ContentAlignment AnyCenterAlign;
        public static readonly ContentAlignment AnyLeftAlign;
        public static readonly ContentAlignment AnyMiddleAlign;
        public static readonly ContentAlignment AnyRightAlign;
        public static readonly ContentAlignment AnyTopAlign;
        public static readonly Size UninitializedSize;


        public class ArraySubsetEnumerator : IEnumerator
        {
            public ArraySubsetEnumerator(object[] array, int count)
            {
                this.array = array;
                this.total = count;
                this.current = -1;
            }

            public bool MoveNext()
            {
                if (this.current < (this.total - 1))
                {
                    this.current++;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                this.current = -1;
            }


            public object Current
            {
                get
                {
                    if (this.current == -1)
                    {
                        return null;
                    }
                    return this.array[this.current];
                }
            }


            private object[] array;
            private int current;
            private int total;
        }

        /*[StructLayout(LayoutKind.Sequential)]
        internal struct DCMapping : IDisposable
        {
            private DeviceContext dc;
            private System.Drawing.Graphics graphics;
            private Rectangle translatedBounds;
            public DCMapping(HandleRef hDC, Rectangle bounds)
            {
                if (hDC.Handle == IntPtr.Zero)
                {
                    throw new ArgumentNullException("hDC");
                }
                NativeMethods.POINT point1 = new NativeMethods.POINT();
                HandleRef ref1 = NativeMethods.NullHandleRef;
                NativeMethods.RegionFlags flags1 = NativeMethods.RegionFlags.NULLREGION;
                this.translatedBounds = bounds;
                this.graphics = null;
                this.dc = DeviceContext.FromHdc(hDC.Handle);
                this.dc.SaveHdc();
                NativeMethods.GetViewportOrgEx(hDC, point1);
                HandleRef ref2 = new HandleRef(null, NativeMethods.CreateRectRgn(point1.x + bounds.Left, point1.y + bounds.Top, point1.x + bounds.Right, point1.y + bounds.Bottom));
                try
                {
                    ref1 = new HandleRef(this, NativeMethods.CreateRectRgn(0, 0, 0, 0));
                    int num1 = NativeMethods.GetClipRgn(hDC, ref1);
                    NativeMethods.POINT point2 = new NativeMethods.POINT();
                    NativeMethods.SetViewportOrgEx(hDC, point1.x + bounds.Left, point1.y + bounds.Top, point2);
                    if (num1 != 0)
                    {
                        NativeMethods.RECT rect1 = new NativeMethods.RECT();
                        flags1 = (NativeMethods.RegionFlags) NativeMethods.GetRgnBox(ref1, ref rect1);
                        if (flags1 == NativeMethods.RegionFlags.SIMPLEREGION)
                        {
                            NativeMethods.CombineRgn(ref2, ref2, ref1, 1);
                        }
                    }
                    else
                    {
                        NativeMethods.DeleteObject(ref1);
                        ref1 = new HandleRef(null, IntPtr.Zero);
                        flags1 = NativeMethods.RegionFlags.SIMPLEREGION;
                    }
                    NativeMethods.SelectClipRgn(hDC, ref2);
                }
                catch (Exception exception1)
                {
                    if (ClientUtils.IsSecurityOrCriticalException(exception1))
                    {
                        throw;
                    }
                    this.dc.RestoreHdc();
                    this.dc.Dispose();
                    return;
                }
                finally
                {
                    NativeMethods.DeleteObject(ref2);
                    if (ref1.Handle != IntPtr.Zero)
                    {
                        NativeMethods.DeleteObject(ref1);
                    }
                }
            }

            public void Dispose()
            {
                if (this.graphics != null)
                {
                    this.graphics.Dispose();
                    this.graphics = null;
                }
                if (this.dc != null)
                {
                    this.dc.RestoreHdc();
                    this.dc.Dispose();
                    this.dc = null;
                }
            }

            public System.Drawing.Graphics Graphics
            {
                get
                {
                    if (this.graphics == null)
                    {
                        this.graphics = System.Drawing.Graphics.FromHdcInternal(this.dc.Hdc);
                        this.graphics.SetClip(new Rectangle(Point.Empty, this.translatedBounds.Size));
                    }
                    return this.graphics;
                }
            }
        }*/

        public static class EnumValidator
        {
            public static bool IsEnumWithinShiftedRange(Enum enumValue, int numBitsToShift, int minValAfterShift, int maxValAfterShift)
            {
                int num1 = Convert.ToInt32(enumValue, CultureInfo.InvariantCulture);
                int num2 = num1 >> (numBitsToShift & 0x1f);
                if (((num2 << (numBitsToShift & 0x1f)) == num1) && (num2 >= minValAfterShift))
                {
                    return (num2 <= maxValAfterShift);
                }
                return false;
            }

            public static bool IsValidArrowDirection(ArrowDirection direction)
            {
                switch (direction)
                {
                    case ArrowDirection.Left:
                    case ArrowDirection.Up:
                    case ArrowDirection.Right:
                    case ArrowDirection.Down:
                    {
                        return true;
                    }
                }
                return false;
            }

            /*public static bool IsValidContentAlignment(ContentAlignment contentAlign)
            {
                if (ClientUtils.GetBitCount((uint) contentAlign) != 1)
                {
                    return false;
                }
                int num1 = 0x777;
                return ((((ContentAlignment) num1) & contentAlign) != ((ContentAlignment) 0));
            }

            public static bool IsValidTextImageRelation(TextImageRelation relation)
            {
                return ClientUtils.IsEnumValid(relation, (int) relation, 0, 8, 1);
            }*/

        }

        /*internal class TypedControlCollection : WindowsFormsUtils.ReadOnlyControlCollection
        {
            public TypedControlCollection(Control owner, System.Type typeOfControl) : base(owner, false)
            {
                this.typeOfControl = typeOfControl;
                this.ownerControl = owner;
            }

            public TypedControlCollection(Control owner, System.Type typeOfControl, bool isReadOnly) : base(owner, isReadOnly)
            {
                this.typeOfControl = typeOfControl;
                this.ownerControl = owner;
            }

            public override void Add(Control value)
            {
                Control.CheckParentingCycle(this.ownerControl, value);
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                if (this.IsReadOnly)
                {
                    throw new NotSupportedException(SR.GetString("ReadonlyControlsCollection"));
                }
                if (!this.typeOfControl.IsAssignableFrom(value.GetType()))
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, SR.GetString("TypedControlCollectionShouldBeOfType", new object[] { this.typeOfControl.Name }), new object[0]), value.GetType().Name);
                }
                base.Add(value);
            }


            private Control ownerControl;
            private System.Type typeOfControl;
        }*/

        internal class WeakRefCollection : IList, ICollection, IEnumerable
        {
            internal WeakRefCollection()
            {
                this._innerList = new ArrayList(4);
            }

            internal WeakRefCollection(int size)
            {
                this._innerList = new ArrayList(size);
            }

            public int Add(object value)
            {
                return this.InnerList.Add(this.CreateWeakRefObject(value));
            }

            public void Clear()
            {
                this.InnerList.Clear();
            }

            public bool Contains(object value)
            {
                return this.InnerList.Contains(this.CreateWeakRefObject(value));
            }

            private static void Copy(WindowsFormsUtils.WeakRefCollection sourceList, int sourceIndex, WindowsFormsUtils.WeakRefCollection destinationList, int destinationIndex, int length)
            {
                if (sourceIndex < destinationIndex)
                {
                    sourceIndex += length;
                    destinationIndex += length;
                    while (length > 0)
                    {
                        destinationList.InnerList[--destinationIndex] = sourceList.InnerList[--sourceIndex];
                        length--;
                    }
                }
                else
                {
                    while (length > 0)
                    {
                        destinationList.InnerList[destinationIndex++] = sourceList.InnerList[sourceIndex++];
                        length--;
                    }
                }
            }

            public void CopyTo(Array array, int index)
            {
                this.InnerList.CopyTo(array, index);
            }

            private WindowsFormsUtils.WeakRefCollection.WeakRefObject CreateWeakRefObject(object value)
            {
                if (value == null)
                {
                    return null;
                }
                return new WindowsFormsUtils.WeakRefCollection.WeakRefObject(value);
            }

            public override bool Equals(object obj)
            {
                WindowsFormsUtils.WeakRefCollection collection1 = obj as WindowsFormsUtils.WeakRefCollection;
                if ((collection1 == null) || (this.Count != collection1.Count))
                {
                    return false;
                }
                for (int num1 = 0; num1 < this.Count; num1++)
                {
                    if (this.InnerList[num1] != collection1.InnerList[num1])
                    {
                        return false;
                    }
                }
                return true;
            }

            public IEnumerator GetEnumerator()
            {
                return this.InnerList.GetEnumerator();
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public int IndexOf(object value)
            {
                return this.InnerList.IndexOf(this.CreateWeakRefObject(value));
            }

            public void Insert(int index, object value)
            {
                this.InnerList.Insert(index, this.CreateWeakRefObject(value));
            }

            public void Remove(object value)
            {
                this.InnerList.Remove(this.CreateWeakRefObject(value));
            }

            public void RemoveAt(int index)
            {
                this.InnerList.RemoveAt(index);
            }

            public void ScavengeReferences()
            {
                int num1 = 0;
                int num2 = this.Count;
                for (int num3 = 0; num3 < num2; num3++)
                {
                    if (this[num1] == null)
                    {
                        this.InnerList.RemoveAt(num1);
                    }
                    else
                    {
                        num1++;
                    }
                }
            }


            public int Count
            {
                get
                {
                    return this.InnerList.Count;
                }
            }

            internal ArrayList InnerList
            {
                get
                {
                    return this._innerList;
                }
            }

            public bool IsFixedSize
            {
                get
                {
                    return this.InnerList.IsFixedSize;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return this.InnerList.IsReadOnly;
                }
            }

            public object this[int index]
            {
                get
                {
                    WindowsFormsUtils.WeakRefCollection.WeakRefObject obj1 = this.InnerList[index] as WindowsFormsUtils.WeakRefCollection.WeakRefObject;
                    if ((obj1 != null) && obj1.IsAlive)
                    {
                        return obj1.Target;
                    }
                    return null;
                }
                set
                {
                    this.InnerList[index] = this.CreateWeakRefObject(value);
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return this.InnerList.IsSynchronized;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this.InnerList.SyncRoot;
                }
            }


            private ArrayList _innerList;


            internal class WeakRefObject
            {
                internal WeakRefObject(object obj) : this(obj, true)
                {
                }

                internal WeakRefObject(object obj, bool weakRef)
                {
                    if (obj != null)
                    {
                        this.hash = obj.GetHashCode();
                        if (weakRef)
                        {
                            this.weakHolder = new WeakReference(obj);
                            this.strongHolder = null;
                        }
                        else
                        {
                            this.strongHolder = obj;
                            this.weakHolder = null;
                        }
                    }
                    else
                    {
                        this.weakHolder = null;
                        this.strongHolder = null;
                        this.hash = 0;
                    }
                }

                public override bool Equals(object obj)
                {
                    if (obj == this)
                    {
                        return true;
                    }
                    WindowsFormsUtils.WeakRefCollection.WeakRefObject obj1 = obj as WindowsFormsUtils.WeakRefCollection.WeakRefObject;
                    if (((obj1 != null) && (this.Target == obj1.Target)) && (this.Target != null))
                    {
                        return true;
                    }
                    return false;
                }

                public override int GetHashCode()
                {
                    return this.hash;
                }


                internal bool IsAlive
                {
                    get
                    {
                        if (this.IsWeakReference)
                        {
                            return this.weakHolder.IsAlive;
                        }
                        return true;
                    }
                }

                internal bool IsWeakReference
                {
                    get
                    {
                        return (this.strongHolder == null);
                    }
                }

                internal object Target
                {
                    get
                    {
                        if (!this.IsWeakReference)
                        {
                            return this.strongHolder;
                        }
                        if (this.weakHolder != null)
                        {
                            return this.weakHolder.Target;
                        }
                        return null;
                    }
                }


                private int hash;
                private object strongHolder;
                private WeakReference weakHolder;
            }
        }
    }

    public class VBExamplesHelper
    {
        static private bool isVBContext = false;

        public static bool VBContext
        {
            set
            {
                isVBContext = value;
            }
        }

        public static string StripPath(string path)
        {
            if (isVBContext)
            {
                string[] fileParts = path.Split('.');
                int len = fileParts.Length;
                if ( len > 1)
                {
                    path = fileParts[ len - 2]+"."+ fileParts[ len - 1 ];
                }
            }

            return path;
        }
    }

}

