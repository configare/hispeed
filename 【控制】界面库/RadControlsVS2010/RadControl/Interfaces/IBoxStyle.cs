using System.Drawing;

namespace Telerik.WinControls
{

	public interface IBoxStyle
	{
		Color LeftColor { get; set; }
		Color LeftShadowColor { get; set; }

		Color TopColor { get; set; }
		Color TopShadowColor { get; set; }

		Color RightColor { get; set; }
		Color RightShadowColor { get; set; }

		Color BottomColor { get; set; }
		Color BottomShadowColor { get; set; }
	}
}