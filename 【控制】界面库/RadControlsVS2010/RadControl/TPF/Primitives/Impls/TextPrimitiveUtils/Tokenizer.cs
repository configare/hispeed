using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// A String Tokenizer that accepts Strings as source and delimiter. Only 1 delimiter is supported (either String or char[]).
    /// </summary>
    public class StringTokenizer
    {       
        private LinkedList<string> tokens;
        private string sourceString;
        private string delimiter;
        private IEnumerator<string> enumerator;
        /// <summary>
        /// Constructor for StringTokenizer Class.
        /// </summary>
        /// <param name="source">The Source String.</param>
        /// <param name="delimiter">The Delimiter String. If a 0 length delimiter is given, " " (space) is used by default.</param>
        public StringTokenizer(string source, string delimiter)
        {
            this.tokens = new LinkedList<string>();
            this.enumerator = this.tokens.GetEnumerator();
            this.sourceString = source;
            this.delimiter = delimiter;

            if (delimiter.Length == 0)
            {
                this.delimiter = " ";
            }

            this.Tokenize();
        }        
        
        private void Tokenize()
        {
            string tempSource = this.sourceString;
            string token = string.Empty;            
            this.tokens.Clear();
            
            if (string.IsNullOrEmpty( tempSource ))
            {
                return;
            }

            int indexOfDelimiter = tempSource.IndexOf(this.delimiter);

            if (indexOfDelimiter < 0)
            {
                this.tokens.AddLast(tempSource);
                this.enumerator = this.tokens.GetEnumerator();
                return;
            }            

            while ((indexOfDelimiter=tempSource.IndexOf(this.delimiter)) >= 0)
            {
                //Delimiter at beginning of source String.
                if (indexOfDelimiter == 0)
                {
                    if (tempSource.Length > this.delimiter.Length)
                    {
                        tempSource = tempSource.Substring(this.delimiter.Length);
                    }
                    else
                    {
                        tempSource = string.Empty;
                    }
                }
                else
                {
                    token = tempSource.Substring(0, indexOfDelimiter);
                    this.tokens.AddLast(token);
                    this.enumerator = this.tokens.GetEnumerator();
                    if (tempSource.Length > (this.delimiter.Length + token.Length))
                    {
                        tempSource = tempSource.Substring(this.delimiter.Length + token.Length);
                    }
                    else
                    {
                        tempSource = string.Empty;
                    }
                }
            }

            //we may have a string leftover.
            if (tempSource.Length > 0)
            {
                this.tokens.AddLast(tempSource);
                this.enumerator = this.tokens.GetEnumerator();
            }
        }
       
        /// <summary>
        /// Method to get the number of tokens in this StringTokenizer.
        /// </summary>
        /// <returns>The number of Tokens in the internal ArrayList.</returns>
        public int Count()
        {
            return this.tokens.Count;
        }     
        
        /// <summary>
        /// Method to get the next (string)token of this StringTokenizer.
        /// </summary>
        /// <returns>A string representing the next token; null if no tokens or no more tokens.</returns>
        /// 
        public string NextToken()
        {            
            if (this.enumerator.MoveNext())
            {
                return this.enumerator.Current; 
            }
            else
            {
                return null;
            }
        }        
    }
}
