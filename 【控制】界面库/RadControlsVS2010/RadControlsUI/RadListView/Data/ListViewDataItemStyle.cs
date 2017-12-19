using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    internal class ListViewDataItemStyle : INotifyPropertyChanged
    {
        #region Default Values

        public static readonly Color DefaultForeColor = Color.Empty;
        public static readonly Color DefaultBackColor = Color.Empty;
        public static readonly Color DefaultBackColor2 = Color.Empty;
        public static readonly Color DefaultBackColor3 = Color.Empty;
        public static readonly Color DefaultBackColor4 = Color.Empty;
        public static readonly Color DefaultBorderColor = Color.Empty;
        public static readonly Font DefaultFont = null;
        public static readonly int DefaultNumberOfColors = 4;
        public static readonly float DefaultGradientPercentage = 0.5f;
        public static readonly float DefaultGradientPercentage2 = 0.5f;
        public static readonly float DefaultGradientAngle = 90.0f;
        public static readonly GradientStyles DefaultGradientStyle = GradientStyles.Linear;
        public static readonly ContentAlignment DefaultTextAlignment = ContentAlignment.MiddleLeft;
        public static readonly ContentAlignment DefaultImageAlignment = ContentAlignment.MiddleLeft;
        public static readonly TextImageRelation DefaultTextImageRelation = TextImageRelation.ImageBeforeText;

        #endregion

        #region Fields

        private Color foreColor = DefaultForeColor;
        private Color backColor = DefaultBackColor;
        private Color backColor2 = DefaultBackColor2;
        private Color backColor3 = DefaultBackColor3;
        private Color backColor4 = DefaultBackColor4;
        private Color borderColor = DefaultBorderColor;
        private Font font = DefaultFont;
        private int numberOfColors = DefaultNumberOfColors;
        private float gradientPercentage = DefaultGradientPercentage;
        private float gradientPercentage2 = DefaultGradientPercentage2;
        private float gradientAngle = DefaultGradientAngle;
        private GradientStyles gradientStyle = DefaultGradientStyle;
        private ContentAlignment textAlignment = DefaultTextAlignment;
        private ContentAlignment imageAlignment = DefaultImageAlignment;
        private TextImageRelation textImageRelation = DefaultTextImageRelation;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the relation between the image and the text.
        /// </summary>
        public TextImageRelation TextImageRelation
        {
            get
            {
                return textImageRelation;
            }
            set
            {
                textImageRelation = value;
            }
        }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>The font.</value>
        public Font Font
        {
            get
            {
                return font;
            }
            set
            {
                font = value;
                OnNotifyPropertyChanged("Font");
            }
        }

        /// <summary>
        /// Gets or sets the color of the fore.
        /// </summary>
        /// <value>The color of the fore.</value>
        public Color ForeColor
        {
            get
            {
                return foreColor;
            }
            set
            {
                foreColor = value;
                OnNotifyPropertyChanged("ForeColor");
            }
        }

        /// <summary>
        /// Gets or sets the color of the border.
        /// </summary>
        /// <value>The color of the border.</value>
        public Color BorderColor
        {
            get
            {
                return borderColor;
            }
            set
            {
                borderColor = value;
                OnNotifyPropertyChanged("BorderColor");
            }
        }

        /// <summary>
        /// Gets or sets the back color4.
        /// </summary>
        /// <value>The back color4.</value>
        public Color BackColor4
        {
            get
            {
                return backColor4;
            }
            set
            {
                backColor4 = value;
                OnNotifyPropertyChanged("BackColor4");
            }
        }

        /// <summary>
        /// Gets or sets the back color3.
        /// </summary>
        /// <value>The back color3.</value>
        public Color BackColor3
        {
            get
            {
                return backColor3;
            }
            set
            {
                backColor3 = value;
                OnNotifyPropertyChanged("BackColor3");
            }
        }

        /// <summary>
        /// Gets or sets the back color2.
        /// </summary>
        /// <value>The back color2.</value>
        public Color BackColor2
        {
            get
            {
                return backColor2;
            }
            set
            {
                backColor2 = value;
                OnNotifyPropertyChanged("backColor2");
            }
        }

        /// <summary>
        /// Gets or sets the color of the back.
        /// </summary>
        /// <value>The color of the back.</value>
        public Color BackColor
        {
            get
            {
                return backColor;
            }
            set
            {
                backColor = value;
                OnNotifyPropertyChanged("BackColor");
            }
        }

        /// <summary>
        /// Gets or sets the number of colors.
        /// </summary>
        /// <value>The number of colors.</value>
        public int NumberOfColors
        {
            get
            {
                return numberOfColors;
            }
            set
            {
                numberOfColors = value;
                OnNotifyPropertyChanged("NumberOfColors");
            }
        }

        /// <summary>
        /// Gets or sets the gradient percentage2.
        /// </summary>
        /// <value>The gradient percentage2.</value>
        public float GradientPercentage2
        {
            get
            {
                return gradientPercentage2;
            }
            set
            {
                gradientPercentage2 = value;
                OnNotifyPropertyChanged("GradientPercentage2");
            }
        }

        /// <summary>
        /// Gets or sets the gradient percentage.
        /// </summary>
        /// <value>The gradient percentage.</value>
        public float GradientPercentage
        {
            get
            {
                return gradientPercentage;
            }
            set
            {
                gradientPercentage = value;
                OnNotifyPropertyChanged("GradientPercentage");
            }
        }

        /// <summary>
        /// Gets or sets the gradient angle.
        /// </summary>
        /// <value>The gradient angle.</value>
        public float GradientAngle
        {
            get
            {
                return gradientAngle;
            }
            set
            {
                gradientAngle = value;
                OnNotifyPropertyChanged("GradientAngle");
            }
        }

        /// <summary>
        /// Gets or sets the gradient style.
        /// </summary>
        /// <value>The gradient style.</value>
        public GradientStyles GradientStyle
        {
            get
            {
                return gradientStyle;
            }
            set
            {
                gradientStyle = value;
                OnNotifyPropertyChanged("GradientStyle");
            }
        }

        /// <summary>
        /// Gets or sets the text alignment.
        /// </summary>
        /// <value>The text alignment.</value>
        public ContentAlignment TextAlignment
        {
            get
            {
                return textAlignment;
            }
            set
            {
                textAlignment = value;
                OnNotifyPropertyChanged("TextAlignment");
            }
        }

        /// <summary>
        /// Gets or sets the image alignment.
        /// </summary>
        /// <value>The image alignment.</value>
        public ContentAlignment ImageAlignment
        {
            get
            {
                return this.imageAlignment;
            }
            set
            {
                this.imageAlignment = value;
            }
        }
         
        #endregion

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnNotifyPropertyChanged(string name)
        {
            OnNotifyPropertyChanged(new PropertyChangedEventArgs(name));
        }

        protected virtual void OnNotifyPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }
        #endregion
    }
}