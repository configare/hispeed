using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using Telerik.WinControls.Styles.PropertySettings;
using Telerik.WinControls.UI;

namespace Telerik.WinControls.XmlSerialization
{
    /// <summary>
    /// Provides XmlSerialization parsing for ColorBlend extention and 
    /// calculates color-blending values for color values of PropertySetting and AnimatedPropertySettings.
    /// </summary>
    /// <remarks>
    /// For more information see <see cref="Theme.AddColorBlend(string, HslColor)"/>
    /// </remarks>
    public class ColorBlendExtension: XmlSerializerExtention, IValueProvider
    {
        private string themePropertyName;
        private IPropertiesProvider styleParameterSerivce;

        private Color originalColor;

        int colorADiff = 0;
        double colorHDiff = 0;
        double colorSDiff = 0;
        double colorLDiff = 0;

        public ColorBlendExtension()
        {
        }

        public ColorBlendExtension(IPropertiesProvider parameterProvider, string parameterName, HslColor blendColor, Color currentColor, Color originalColor)
        {
            this.styleParameterSerivce = parameterProvider;
            this.themePropertyName = parameterName;

            this.originalColor = originalColor;

            HslColor currentHslColor = HslColor.FromColor(currentColor);

            this.colorADiff = currentHslColor.A - blendColor.A;
            this.colorHDiff = currentHslColor.H - blendColor.H;
            this.colorSDiff = currentHslColor.S - blendColor.S;
            this.colorLDiff = currentHslColor.L - blendColor.L;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideTargetValue valueService = IServiceProviderHelper<IProvideTargetValue>.GetService(serviceProvider, typeof(ColorBlendExtension).ToString());

            styleParameterSerivce = IServiceProviderHelper<IPropertiesProvider>.GetService(serviceProvider, typeof(ColorBlendExtension).ToString());

            string[] extenderParameters = valueService.SourceValue.Split(',');

            if (extenderParameters.Length < 4)
            {
                throw new ArgumentException("parameters not valid!");
            }

            this.themePropertyName = extenderParameters[0].Trim();
            if (string.IsNullOrEmpty(ThemePropertyName))
            {
                throw new InvalidOperationException("The first argument of RelativeColor exptrssion should be the name of the ThemeProperty");
            }

            int.TryParse(extenderParameters[1].Trim(), out colorADiff);
            double.TryParse(extenderParameters[2].Trim(), out colorHDiff);
            double.TryParse(extenderParameters[3].Trim(), out colorSDiff);
            double.TryParse(extenderParameters[4].Trim(), out colorLDiff);

            return this;
        }

        #region IValueProvider Members

        /// <summary>
        /// IValueProvider GetValue implementation
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        {
            object baseColor = styleParameterSerivce.GetPropertyValue(this.ThemePropertyName);
            HslColor baseHslColor = HslColor.FromAhsl(255);
            if (baseColor != null)
            {
                if (baseColor.GetType() == typeof(Color))
                {
                    baseHslColor = HslColor.FromColor((Color)baseColor);
                }
                else if (baseColor is HslColor)
                {
                    baseHslColor = (HslColor)baseColor;
                }
            }
            else
            {
                return this.originalColor;
            }

            HslColor hslValue = HslColor.FromAhsl(
                baseHslColor.A + colorADiff,
                Math.Abs(baseHslColor.H + colorHDiff) - Math.Floor(baseHslColor.H + colorHDiff),
                Math.Min(1d, Math.Max(0d, baseHslColor.S + this.colorSDiff)),
                Math.Min(1d, Math.Max(0d, baseHslColor.L + this.colorLDiff)));

            return hslValue.RgbValue;
        }

        /// <summary>
        /// Gets the original Color value
        /// </summary>
        public Color OriginalColor
        {
            get 
            {
                return originalColor; 
            }
        }

        /// <summary>
        /// Gets value corresponding to the name of the ThemeParameter used for color blending calculations
        /// </summary>
        public string ThemePropertyName
        {
            get
            {
                return themePropertyName;
            }
        }

        #endregion
    }
}
