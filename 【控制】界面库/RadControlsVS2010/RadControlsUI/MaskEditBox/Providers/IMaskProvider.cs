using System.Globalization;
using System.Windows.Forms;
using Telerik.WinControls.UI;

namespace Telerik.WinControls.UI
{
    public interface IMaskProvider
    {
        void KeyDown(object sender, KeyEventArgs e);

        void KeyPress(object sender, KeyPressEventArgs e);

        bool Validate(string value);

        bool Click();

        RadTextBoxItem TextBoxItem { get; } 

        string ToString(bool includePromt, bool includeLiterals);

        IMaskProvider Clone();      

        CultureInfo Culture { get; }

        string Mask { get; }

        bool IncludePrompt { get; set; }

        char PromptChar { get; set; }

        object Value { get; set; }

        bool Delete();
    }
}
