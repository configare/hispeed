namespace Telerik.Data.Expressions
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    static class Utils
    {
        public static string TokenToString(Token token)
        {
            try
            {
                return string.Format("token {0} ({1})", (int)token, Enum.GetName(typeof(Token), token));
            }
            catch (Exception /*e*/)
            {
                return ("Unknown token " + ((int)token).ToString());

            }
        }

        public static bool IsHexDigit(char ch)
        {
            switch (ch)
            {
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
            case 'a':
            case 'b':
            case 'c':
            case 'd':
            case 'e':
            case 'f':
            case 'A':
            case 'B':
            case 'C':
            case 'D':
            case 'E':
            case 'F':
                return true;
            }
            return false;
        }       
    }
}