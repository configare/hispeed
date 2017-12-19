using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
	/// <exclude/>
	public struct RadDesignCategory
	{
		public const string ActionCategory          = "Action";
		public const string BehaviorCategory        = "Behavior";
		public const string AppearanceCategory      = "Appearance";
		public const string StyleSheetCategory      = "StyleSheet";
		public const string LayoutCategory          = "Layout";
        public const string PropertyChangedCategory = "Property Changed";
		public const string DragDropCategory        = "Drag Drop";
		public const string MouseCategory           = "Mouse";
		public const string KeyCategory             = "Key";
		public const string FocusCategory           = "Focus";
		public const string DataCategory            = "Data";
#if DEBUG
        public const string TPFDebugCategory = "TPF Debug";
#endif
	}
}