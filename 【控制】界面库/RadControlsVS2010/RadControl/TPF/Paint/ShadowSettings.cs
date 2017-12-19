using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Xml.Serialization;

namespace Telerik.WinControls.Paint
{
    /// <summary>Represents shadow settings.</summary>
    //[Serializable]
    //[XmlInclude(typeof(Color))]
    //[XmlInclude(typeof(Point))]
    [TypeConverter(DesignerConsts.ShadowSettingsConverterString)]
    public class ShadowSettings
    {
        private Point depth;
        private Color shadowColor;
        private int thickness;

        /// <summary>
        /// Initializes a new instance of the ShadowSettings class using point, thickness, and
        /// shadow color.
        /// </summary>
        [Obsolete("Thickness parameter s no longer used. Please use the contructor with two parameteres.")]
        public ShadowSettings(Point depth, int thickness, Color shadowColor)
        {
            this.depth = depth;
            this.thickness = thickness;
            this.shadowColor = shadowColor;
        }

        /// <summary>
        /// Initializes a new instance of the ShadowSettings class using point and
        /// shadow color.
        /// </summary>
        public ShadowSettings(Point depth, Color shadowColor)
        {
            this.depth = depth;
            this.shadowColor = shadowColor;
        }

        /// <summary>Initializes a new instance of the ShadowSettings class.</summary>
        public ShadowSettings()
        {
            this.depth = new Point(1, 1);
            this.shadowColor = Color.Black;
        }

        /// <summary>Gets or sets the shadow depth.</summary>
        public Point Depth
        {
            get { return this.depth; }
            set { this.depth = value; }
        }

        /// <summary>Gets or sets the shadow thickness.</summary>
        [XmlAttribute]
        [Obsolete("The value of this parameter is not used")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Thickness
        {
            get { return this.thickness; }
            set { this.thickness = value; }
        }

        //[XmlAttribute]
        /// <summary>Gets or sets the shadow color.</summary>
        public Color ShadowColor
        {
            get { return this.shadowColor; }
            set { this.shadowColor = value; }
        }
    }
}
