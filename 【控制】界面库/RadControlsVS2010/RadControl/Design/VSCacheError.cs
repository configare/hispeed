using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Windows.Forms;

namespace Telerik.WinControls.Design
{
	public class VSCacheError
	{
		static VSCacheError()
		{
		}
		
		public static void ShowVSCacheError(Assembly componentAssembly, Assembly designerAssembly)
		{
			  string text1 = msg1 + Environment.NewLine + Environment.NewLine;
			  string text2 = text1;
			  text1 = text2 + "Component Assembly:" + Environment.NewLine + componentAssembly.Location + Environment.NewLine + Environment.NewLine;
			  string text3 = text1;
			  text1 = text3 + "Designer Assembly:" + Environment.NewLine + designerAssembly.Location + Environment.NewLine + Environment.NewLine;
			  string text4 = text1;
			  text1 = text4 + msg2 + Environment.NewLine + Environment.NewLine + msg3;
			  MessageBox.Show(text1, "Visual Studio Error Detected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}

	  // Fields
      private static string msg1 = "Visual Studio is attempting to load class instances from a different assembly than the original used to create your components. This will result in failure to load your designed component. More information that will help you to correct the problem.";
      private static string msg2 = "Please close Visual Studio, remove the errant assembly and try loading your designer again.";
	  private static string msg3 = "Ensure that you do not attempt to save any designer that opens with errors, as this can result in loss of work. Note that you may receive this message multiple times, once for each component instance in your designer.";
	}
}
