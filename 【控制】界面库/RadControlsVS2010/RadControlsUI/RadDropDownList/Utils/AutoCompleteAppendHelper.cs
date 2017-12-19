using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Telerik.WinControls.UI
{
    public class AutoCompleteAppendHelper : BaseAutoComplete
    {
        #region Fields

        private string findString = "";
        private StringComparison stringComparison;
        private bool limitToList = false;

       
        #endregion

        #region Cstors

        public AutoCompleteAppendHelper(RadDropDownListElement owner)
        : base(owner)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// LimitToList Property
        /// </summary>
        public bool LimitToList
        {
            get
            {
                return limitToList;
            }
            set
            {
                limitToList = value;
            }
        }


        protected string FindString
        {
            get
            {
                return this.findString;
            }
        }

        protected StringComparison StringComparison
        {
            get
            {
                return this.stringComparison;
            }
        }

        #region Overrides
        
        override public void AutoComplete(KeyPressEventArgs e)
        {
            this.AutoComplete(e, this.limitToList);
        }

        #endregion

        #endregion

        #region Helpers

        // AutoComplete
        

        public void AutoComplete(KeyPressEventArgs e, bool limitToList)
        {
            string findString = "";
            if (e.KeyChar == (char)Keys.Enter) 
            {
                this.owner.SelectAllText();
                this.owner.Focus();
                return;
            }         
            
            findString = this.CreateFindString(e);            
            // Search the string in the List
            this.SearchForStringInList(findString, e, limitToList);
        }

        private string CreateFindString(KeyPressEventArgs e)
        {
            string findString = "";
            if (owner.SelectionLength == 0)
            {
                findString = owner.EditableElementText + e.KeyChar;
            }
            else
            {
                findString = owner.EditableElementText.Substring(0, owner.SelectionStart) + e.KeyChar;
            }

            return findString;
        }

        private void SearchForStringInList(string findString, KeyPressEventArgs e, bool limitToList)
        {
            int indexOfFoundItem = -1;
            if (owner.Items.Count > 0)
            {
                indexOfFoundItem = this.FindShortestString(findString);
            }

            if (indexOfFoundItem == -1)//not found
            {
                e.Handled = limitToList;
                return;
            }

            owner.BeginUpdate();
            owner.SelectedText = "";
            owner.EditableElementText = this.owner.Items[indexOfFoundItem].CachedText;         
            owner.SelectionStart = findString.Length;
            owner.SelectionLength = owner.EditableElementText.Length;
            e.Handled = true;
            owner.EndUpdate();
        }

        private int FindShortestString(string findString)
        {
            int count = this.owner.ListElement.Items.Count;
            int indexOfFoundItem = -1;
            int lenOfFoundItem = int.MaxValue;
            int findStringLength = findString.Length;
            this.stringComparison = this.owner.CaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
            this.findString = findString;

            SortedDictionary<string, RadListDataItem> shortest = new SortedDictionary<string, RadListDataItem>();

            for (int i = 0; i < count; ++i)
            {
                RadListDataItem item = this.owner.ListElement.Items[i];
                int tempCachedTextLength = item.CachedText.Length;
                bool res = this.DefaultCompare(item);
                if (!res)
                {
                    continue;
                }

                if (findStringLength == tempCachedTextLength)//found exact match
                {
                    return i;
                }

                if (findStringLength < tempCachedTextLength)//found shorter match
                {
                    if(tempCachedTextLength < lenOfFoundItem)
                    {
                        lenOfFoundItem = item.CachedText.Length;
                        indexOfFoundItem = item.RowIndex;
                    }
                    
                    shortest[item.CachedText] = item;
                }
            }

            IEnumerator<string> enumerator = shortest.Keys.GetEnumerator();
            return enumerator.MoveNext() ? shortest[enumerator.Current].RowIndex : indexOfFoundItem;
        }

        protected virtual bool DefaultCompare(RadListDataItem item)
        {
            return item.CachedText.StartsWith(this.FindString, this.StringComparison);
        }

        #endregion
    }
}
