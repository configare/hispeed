using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public interface IMaskCharacterProvider
    {
        string ToString(bool includePromt, bool includeLiterals);

        bool Set(string input, out int testPosition, out System.ComponentModel.MaskedTextResultHint resultHint);

        /// <summary>
        /// Removes the assigned characters between the specified positions from the formatted
        /// string.
        /// </summary>
        /// <returns>true if the character was successfully removed; otherwise, false.
        /// </returns>
        /// <param name="startPosition">
        /// The zero-based index of the first assigned character to remove.
        /// </param>
        /// <param name="endPosition">
        /// The zero-based index of the last assigned character to remove.
        /// </param>
        bool RemoveAt(int startPosition, int endPosition);

        char PromptChar { get; set; }

        void KeyPress(object sender, KeyPressEventArgs e);

        void KeyDown(object sender, KeyEventArgs e);

        bool Delete();
    }
}
