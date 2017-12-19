using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
	class TabStripEditManager
	{
		//fields
        private RadTabStrip tabStrip = null;
        private TabItem tab = null;


		public TabStripEditManager(RadTabStrip tabStrip, TabItem tab)
		{
			this.tabStrip = tabStrip;
			this.tab = tab;
		}

		public IValueEditor DefaultEditor
		{
			get
			{
				object value = tabStrip.TabStripElement.GetEditedValue(tab);

				IValueEditor textEditorFromEvent = this.CallOnEditorRequiredEvent(typeof(TabStripTextEditor));
				if (textEditorFromEvent != null)
					return textEditorFromEvent;
				return new TabStripTextEditor();
			}
		}

		private IValueEditor CallOnEditorRequiredEvent(Type editorType)
		{
			EditorRequiredEventArgs editorRequiredEventArgs = new EditorRequiredEventArgs(editorType);
			if (this.tabStrip != null)
				this.tabStrip.TabStripElement.CallEditorRequired(this.tab, editorRequiredEventArgs);
			return editorRequiredEventArgs.Editor;
		}
	}
}
