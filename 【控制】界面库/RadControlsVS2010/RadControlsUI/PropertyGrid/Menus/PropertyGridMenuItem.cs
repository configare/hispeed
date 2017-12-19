using System;

namespace Telerik.WinControls.UI
{
    public class PropertyGridMenuItem : RadMenuItem
    {
        #region Fields

        private string command;

        #endregion

        #region Constructor

        public PropertyGridMenuItem(string command, string text)
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
