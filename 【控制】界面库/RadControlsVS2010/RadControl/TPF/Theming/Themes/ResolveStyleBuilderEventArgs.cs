using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    public delegate void ResolveStyleBuilderEventHandler(object sender, ResolveStyleBuilderEventArgs e);

    public class ResolveStyleBuilderEventArgs : EventArgs
    {
        #region Fields

        private string themeName;
        private StyleBuilder builder;

        #endregion

        #region Constructor

        public ResolveStyleBuilderEventArgs(string themeName, StyleBuilder builder)
        {
            this.builder = builder;
            this.themeName = themeName;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the StyleBuilder instance.
        /// </summary>
        public StyleBuilder Builder
        {
            get
            {
                return this.builder;
            }
            set
            {
                this.builder = value;
            }
        }

        /// <summary>
        /// Gets the name of the theme for which StyleBuilder is required.
        /// </summary>
        public string ThemeName
        {
            get
            {
                return this.themeName;
            }
        }

        #endregion
    }
}
