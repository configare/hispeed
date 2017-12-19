using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public abstract class BaseAutoComplete : IDisposable
    {
        #region Fields
        protected RadDropDownListElement owner;
       
        #endregion

        #region Cstors

        public BaseAutoComplete(RadDropDownListElement owner)
        {
            this.owner = owner;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Owner Property
        /// </summary>
        public RadDropDownListElement Owner
        {
            get
            {
                return owner;
            }
        }

        #endregion

        #region Abstracts

        public abstract void AutoComplete(KeyPressEventArgs e);
        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
        }

        #endregion
    }
}
