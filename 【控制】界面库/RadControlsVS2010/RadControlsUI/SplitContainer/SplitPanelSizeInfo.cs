using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.UI;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Telerik.WinControls.UI.Docking
{
    //TODO: Code copied from DockHelper 
    internal static class SplitPanelHelper
    {
        public static readonly Size MaxSize = new Size(Int32.MaxValue, Int32.MaxValue);
        public static readonly Size MinSize = new Size(Int32.MinValue, Int32.MinValue);

        public static Size EnsureSizeMaxBounds(Size size, Size max)
        {
            return EnsureSizeBounds(size, MinSize, max);
        }

        public static Size EnsureSizeMinBounds(Size size, Size min)
        {
            return EnsureSizeBounds(size, min, MaxSize);
        }

        public static Size EnsureSizeBounds(Size size, Size min, Size max)
        {
            size.Width = Math.Max(min.Width, size.Width);
            size.Width = Math.Min(max.Width, size.Width);

            size.Height = Math.Max(min.Height, size.Height);
            size.Height = Math.Min(max.Height, size.Height);

            return size;
        }

        public static SizeF EnsureSizeBounds(SizeF size, SizeF min, SizeF max)
        {
            size.Width = Math.Max(min.Width, size.Width);
            size.Width = Math.Min(max.Width, size.Width);

            size.Height = Math.Max(min.Height, size.Height);
            size.Height = Math.Min(max.Height, size.Height);

            return size;
        }

        public static bool ShouldBeginDrag(Point curr, Point capture)
        {
            Size dragSize = SystemInformation.DragSize;
            Rectangle dragRect = new Rectangle(capture.X - (dragSize.Width / 2),
                                               capture.Y - (dragSize.Height / 2),
                                               dragSize.Width, dragSize.Height);
            return !dragRect.Contains(curr);
        }
    }

    /// <summary>
    /// Encapsulates all size-related properties for a SplitPanel instance residing on a RadSplitContainer.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SplitPanelSizeInfo : RadDockObject
    {      
        #region Constructor

        public SplitPanelSizeInfo()
        {
            this.sizeMode = SplitPanelSizeMode.Auto;
            this.absoluteSize = DefaultAbsoluteSize;
            this.minimumSize = DefaultMinimumSize;
        }

        public SplitPanelSizeInfo(SplitPanelSizeInfo source)
        {
            this.Copy(source);
        }

        #endregion

        #region Public Methods

        public void Copy(SplitPanelSizeInfo source)
        {
            this.relativeRatio = source.relativeRatio;
            this.absoluteSize = source.absoluteSize;
            this.splitterCorrection = source.splitterCorrection;
            this.autoSizeScale = source.autoSizeScale;
            this.minimumSize = source.minimumSize;

            //fill is special mode and does not need to be copied when splittig layout
            if (source.sizeMode != SplitPanelSizeMode.Fill && 
                this.sizeMode != SplitPanelSizeMode.Fill)
            {
                this.sizeMode = source.sizeMode;
            }
        }

        public void Reset()
        {
            this.relativeRatio = SizeF.Empty;
            this.absoluteSize = DefaultAbsoluteSize;
            this.splitterCorrection = Size.Empty;
            this.autoSizeScale = SizeF.Empty;
            this.minimumSize = DefaultMinimumSize;
        }

        public SplitPanelSizeInfo Clone()
        {
            return new SplitPanelSizeInfo(this);
        }

        #endregion

        #region Overrides

        protected override bool ShouldSerializeProperty(string propName)
        {
            switch (propName)
            {
                case "MaximumSize":
                    return this.maximumSize != Size.Empty;
                case "MinimumSize":
                    return this.minimumSize != DefaultMinimumSize;
                case "AbsoluteSize":
                    return this.absoluteSize != DefaultAbsoluteSize;
                case "RelativeRatio":
                    return this.relativeRatio != SizeF.Empty;
                case "AutoSizeScale":
                    return this.autoSizeScale != SizeF.Empty;
                case "SplitterCorrection":
                    return this.splitterCorrection != Size.Empty;
            }

            return base.ShouldSerializeProperty(propName);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the minimum size for the associated SplitPanel.
        /// </summary>
        [Description("Gets or sets the minimum size for the associated SplitPanel.")]
        public Size MinimumSize
        {
            get
            {
                return this.minimumSize;
            }
            set
            {
                if (this.minimumSize == value)
                {
                    return;
                }

                if (!OnPropertyChanging("MinimumSize"))
                {
                    return;
                }

                this.minimumSize = value;
                this.OnPropertyChanged("MinimumSize");
            }
        }

        private bool ShouldSerializeMinimumSize()
        {
            return this.ShouldSerializeProperty("MinimumSize");
        }

        /// <summary>
        /// Gets or sets the maximum size for the associated SplitPanel.
        /// </summary>
        [Description("Gets or sets the maximum size for the associated SplitPanel.")]
        public Size MaximumSize
        {
            get
            {
                return this.maximumSize;
            }
            set
            {
                if (this.maximumSize == value)
                {
                    return;
                }

                if (!OnPropertyChanging("MaximumSize"))
                {
                    return;
                }

                this.maximumSize = value;
                this.OnPropertyChanged("MaximumSize");
            }
        }

        private bool ShouldSerializeMaximumSize()
        {
            return this.ShouldSerializeProperty("MaximumSize");
        }

        /// <summary>
        /// Gets or sets the amount (in pixels) applied to the size of the panel by a splitter.
        /// </summary>
        [Description("Gets or sets the amount (in pixels) applied to the size of the panel by a splitter.")]
        public Size SplitterCorrection
        {
            get
            {
                return this.splitterCorrection;
            }
            set
            {
                if (this.splitterCorrection == value)
                {
                    return;
                }

                if (!this.OnPropertyChanging("SplitterCorrection"))
                {
                    return;
                }

                this.splitterCorrection = value;
                this.OnPropertyChanged("SplitterCorrection");
            }
        }

        private bool ShouldSerializeSplitterCorrection()
        {
            return this.ShouldSerializeProperty("SplitterCorrection");
        }

        /// <summary>
        /// Gets or sets the scale factor for relatively-sized panels.
        /// </summary>
        [Description("Gets or sets the scale factor for relatively-sized panels.")]
        public SizeF RelativeRatio
        {
            get
            {
                return this.relativeRatio;
            }
            set
            {
                value = SplitPanelHelper.EnsureSizeBounds(value, SizeF.Empty, new SizeF(1, 1));
                if (this.relativeRatio == value)
                {
                    return;
                }

                if (!this.OnPropertyChanging("RelativeRatio"))
                {
                    return;
                }

                this.relativeRatio = value;
                this.OnPropertyChanged("RelativeRatio");
            }
        }

        private bool ShouldSerializeRelativeRatio()
        {
            return this.ShouldSerializeProperty("RelativeRatio");
        }

        /// <summary>
        /// Gets or sets the scale factor to be used by auto-sized panels.
        /// Usually this is internally updated by a splitter resize.
        /// </summary>
        [Browsable(false)]
        [Description("Gets or sets the scale factor to be used by auto-sized panels. Usually this is internally updated by a splitter resize.")]
        public SizeF AutoSizeScale
        {
            get
            {
                return this.autoSizeScale;
            }
            set
            {
                value = SplitPanelHelper.EnsureSizeBounds(value, new SizeF(-1, -1), new SizeF(1, 1));
                if (this.autoSizeScale == value)
                {
                    return;
                }

                if (!this.OnPropertyChanging("AutoSizeScale"))
                {
                    return;
                }

                this.autoSizeScale = value;
                this.OnPropertyChanged("AutoSizeScale");
            }
        }

        private bool ShouldSerializeAutoSizeScale()
        {
            return this.ShouldSerializeProperty("AutoSizeScale");
        }

        /// <summary>
        /// Gets or sets the size mode for the owning panel.
        /// </summary>
        [Description("Gets or sets the size mode for the owning panel.")]
        [DefaultValue(SplitPanelSizeMode.Auto)]
        public SplitPanelSizeMode SizeMode
        {
            get
            {
                return this.sizeMode;
            }
            set
            {
                if (this.sizeMode == value)
                {
                    return;
                }

                if (!this.OnPropertyChanging("SizeMode"))
                {
                    return;
                }

                this.sizeMode = value;
                this.OnPropertyChanged("SizeMode");
            }
        }

        /// <summary>
        /// Gets or sets the size used when size mode is Absolute.
        /// </summary>
        [Description("Gets or sets the size used when size mode is Absolute.")]
        public Size AbsoluteSize
        {
            get
            {
                return this.absoluteSize;
            }
            set
            {
                if (absoluteSize == value)
                {
                    return;
                }

                if (!OnPropertyChanging("AbsoluteSize"))
                {
                    return;
                }

                this.absoluteSize = value;
                this.OnPropertyChanged("AbsoluteSize");
            }
        }

        private bool ShouldSerializeAbsoluteSize()
        {
            return this.ShouldSerializeProperty("AbsoluteSize");
        }

        /// <summary>
        /// Gets or sets the desired (measured) size for the owning panel.
        /// This field is internally updated by a SplitContainerLayoutStrategy upon a layout operation.
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MeasuredLength
        {
            get
            {
                return this.measuredLength;
            }
            set
            {
                this.measuredLength = value;
            }
        }

        #endregion

        #region Fields

        private SplitPanelSizeMode sizeMode;
        private Size absoluteSize;
        private Size minimumSize;
        private Size maximumSize;
        private Size splitterCorrection;
        private SizeF relativeRatio;
        private SizeF autoSizeScale;

        //internally used to store last measured size for the panel
        [NonSerialized]
        internal int measuredLength;
        [NonSerialized]
        internal int minLength;

        #endregion

        #region Static Members

        public static readonly Size DefaultAbsoluteSize = new Size(200, 200);
        public static readonly Size DefaultMinimumSize = new Size(25, 25);

        #endregion
    }

    /// <summary>
    /// Defines the posiible size modes for a split panel residing on a RadSplitContainer.
    /// </summary>
    public enum SplitPanelSizeMode
    {
        /// <summary>
        /// The panel is auto-sized. Its size depends on the size of its container
        /// as well as the number of all auto-sizable panels within the container.
        /// </summary>
        Auto,
        /// <summary>
        /// The panel has fixed size, regardless of the size of its container.
        /// </summary>
        Absolute,
        /// <summary>
        /// The panel occupies a relative amount of its container's available size.
        /// </summary>
        Relative,
        /// <summary>
        /// A special mode, used to specify that a certain panel should fill all the available auto-sizable area.
        /// </summary>
        Fill,
    }
}
