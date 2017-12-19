using System;

namespace Telerik.WinControls.UI
{
    public class TreeViewMenuItem : RadMenuItem
    {
        #region Fields

        private string command;

        #endregion

        #region Constructor

        public TreeViewMenuItem(string command, string text)
            : base(text)
        {
            this.command = command;
        }

        #endregion

        #region Properties

        public string Command
        {
            get { return command; }
        }

        protected override Type ThemeEffectiveType
        {
            get
            {
                return typeof(RadMenuItem);
            }
        }

        #endregion
    }
}
