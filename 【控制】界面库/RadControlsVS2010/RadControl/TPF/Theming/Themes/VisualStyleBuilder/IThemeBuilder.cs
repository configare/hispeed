using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;

namespace Telerik.WinControls.Themes.Design
{
    ///<exclude/>
	public class ThemeDesignerService
	{
		public static IThemeBuilder CreateDesignerInstance()
		{
			Type styleBuilderType = Type.GetType(string.Format("Telerik.WinControls.Themes.Design.VisualStyleBuilderMain, VisualStyleBuilder.Design, Version={0}, Culture=neutral, PublicKeyToken=5bb2a467cbec794e", VersionNumber.Number));

			if (styleBuilderType != null)
			{
				return (IThemeBuilder)Activator.CreateInstance(styleBuilderType);
			}

            throw new InvalidOperationException("Unable to locate theme designer: Telerik.WinControls.Themes.Design.VisualStyleBuilderMain. Please add reference to assembly VisualStyleBuilder.Design.dll");
		}
	}

	public interface IThemeBuilder: IDisposable
	{
        /// <summary>
        /// Obsolete. Please Use IVisualStyleBuilder interface instead
        /// </summary>
        /// <returns></returns>
        [Obsolete("Please Use IVisualStyleBuilder interface instead")]
		XmlPropertySettingGroup GetGroup();
        void InitializeThemeDesignerForControl(IComponentTreeHandler contorl);
	    void InitializeThemeDesignerForDesignerData(RadControlDesignTimeData data, string useThemeName);
		DialogResult ShowDialog();
	}

	public class ThemeBuilderHelper
	{
        //public static RadControlDesignTimeData GetControlThemeBuilderData(RadControl control)
        //{
        //    return control.DesignTimeData;
        //}

        public static RadControlDesignTimeData GetControlThemeBuilderData(IComponentTreeHandler control)
        {
            return control.DesignTimeData;
        }
	}

	public class RadObjectHelper
	{
		public static RadProperty GetRegisteredRadPropertyFromFullName(string propertyFullName)
		{
			return RadTypeResolver.Instance.GetRegisteredRadPropertyFromFullName(propertyFullName);
		}
	}
}
