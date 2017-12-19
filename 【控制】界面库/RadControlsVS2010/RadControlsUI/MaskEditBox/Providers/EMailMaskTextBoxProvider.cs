using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Telerik.WinControls.UI
{
    public class EMailMaskTextBoxProvider : RegexMaskTextBoxProvider
    {
        static string mask = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
         @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
         @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

        public EMailMaskTextBoxProvider(CultureInfo culture, RadMaskedEditBoxElement owner)
            : base(EMailMaskTextBoxProvider.mask, culture, owner)
        {
            this.ErrorMessage = "E-Mail is not valid";
        }
    }
}
