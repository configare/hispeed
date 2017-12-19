using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Telerik.WinControls.Styles
{
    public delegate void RadItemStateChangedEventHandler(RadItem sender, RadPropertyChangedEventArgs args);    

    public abstract class ItemStateManagerBase
    {
        #region Constructors

        public ItemStateManagerBase()
        {            
        }

        #endregion

        #region Properties

        public virtual string GetInitialState(RadItem item)
        {
            return item.ThemeRole;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Combines ThemeRoleName with state names using "." as delimiter and sets the result to AttachedElement.Class
        /// </summary>
        /// <param name="item"></param>
        /// <param name="stateNames"></param>
        protected void SetItemState(RadItem item, params string[] stateNames)
        {
            if (item == null)
            {
                Debug.Fail("SetElementState called and item parameter is null.");
                return;
            }

            StringBuilder stateStringBuilder = new StringBuilder();
            stateStringBuilder.Append(item.ThemeRole);
            if (stateNames != null && stateNames.Length > 0 &&
                stateNames[0].Length > 0)
            {
                if (stateStringBuilder.Length > 0)
                {
                    stateStringBuilder.Append(stateDelimiter);
                }

                stateStringBuilder.Append(string.Join(stateDelimiterString, stateNames));
            }

            item.VisualState = stateStringBuilder.ToString();
        }

        public virtual StateManagerAttachmentData AttachToItem(RadItem item)
        {
            StateManagerAttachmentData data = new StateManagerAttachmentData(item, this.ItemStateChanged);
            this.AttachToItemOverride(data, item);
            this.ItemStateChanged(item, null);
            return data;
        }        

        public virtual void Detach(StateManagerAttachmentData data)
        {
            if (data != null)
            {
                data.Dispose();
            }
        }

        public bool VerifyState(string state)
        {
            return this.VerifyState(string.Empty, ItemStateManager.stateDelimiter + state);
        }

		public bool VerifyState(string themeRoleName, string key)
		{
            Debug.Assert(!string.IsNullOrEmpty(key));

            string[] stateNames = key.Split(ItemStateManager.stateDelimiter);

            StateDescriptionNode rootState = this.GetAvailableStates(themeRoleName);

            if (stateNames.Length == 0)
            {
                return false;
            }

            if (rootState.StateName != stateNames[0])
            {
                return false;
            }

            return this.MatchDescriptionNodeToState(rootState, stateNames, 1);
		}

		private bool MatchDescriptionNodeToState(StateDescriptionNode rootState, string[] stateNames, int stateNamesIndex)
		{
            if (stateNamesIndex >= stateNames.Length)
            {
                return true;
            }

            string stateName = stateNames[stateNamesIndex];
            StateDescriptionNode matchedNode = null;

            foreach (StateDescriptionNode descrNode in rootState.Nodes)
            {
                if (descrNode.StateName == stateName)
                {
                    matchedNode = descrNode;
                    break;
                }
            }

            if (matchedNode == null)
            {
                return false;
            }

            return MatchDescriptionNodeToState(matchedNode, stateNames, stateNamesIndex + 1);
		}

        public IEnumerable<string> DefaultVisibleStates
        {
            get
            {
                return this.defaultVisibleStates;
            }
        }

        public void AddDefaultVisibleState(string state)
        {
            if (this.defaultVisibleStates == null)
            {
                this.defaultVisibleStates = new LinkedList<string>();
            }

            if (!this.VerifyState(state))
            {
                throw new InvalidOperationException(string.Format("Default state added for {0} that is not recognized by StateManager - {1}", state, this));
            }

            this.defaultVisibleStates.AddLast(state);
        }

        public void RemoveDefaultVisibleState(string state)
        {
            if (this.defaultVisibleStates == null)
            {
                return;
            }

            if (!this.VerifyState(state))
            {
                throw new InvalidOperationException(string.Format("Default state added for {0} that is not recognized by StateManager - {1}", state, this));
            }

            this.defaultVisibleStates.Remove(state);
        }

        public IEnumerable<string> GetStateFallbackList(RadItem item)
        {
            LinkedList<string> stateList = new LinkedList<string>();            

            //TODO: smarter implementation?
            string currentState = item.VisualState;

            if (string.IsNullOrEmpty(currentState))
            {
                return stateList;
            }            

            string[] stateParts = currentState.Split(ItemStateManagerBase.stateDelimiter);

            if (stateParts.Length < 2)
            {
                stateList.AddLast(currentState);
                return stateList;
            }

            for (int rightIndex = stateParts.Length - 1; rightIndex > 0; rightIndex--)
            {
                for (int leftIndex = 1; leftIndex <= rightIndex; leftIndex++)
                {
                    stateList.AddLast(
                        item.ThemeRole + 
                        ItemStateManagerBase.stateDelimiterString +
                        string.Join(ItemStateManagerBase.stateDelimiterString, stateParts, leftIndex, rightIndex - leftIndex + 1));
                }
            }

            stateList.AddLast(stateParts[0]);

            return stateList;
        }

        public string GetStateFullName(string itemThemeRole, string stateName)
        {
            if (string.IsNullOrEmpty(stateName))
            {
                return itemThemeRole;
            }

            return itemThemeRole + stateDelimiterString + stateName;
        }

        /// <summary>
        /// Combines state names, using stateDelimiter Character.
        /// </summary>
        /// <remarks>
        /// Egg. combining "Selected" and "MouseOver" gives "Selected.MouseOver";
        /// combining "" and "MouseOver" gives "MouseOver"
        /// </remarks>
        /// <param name="stateName"></param>
        /// <param name="stateName1"></param>
        /// <returns></returns>
        public static string CombineStateNames(string stateName, string stateName1)
        {
            string res = stateName;

            if (!string.IsNullOrEmpty(stateName1))
            {
                if (!string.IsNullOrEmpty(res))
                {
                    res += ItemStateManagerBase.stateDelimiter;
                }

                res += stateName1;
            }

            return res;
        }

        public abstract void ItemStateChanged(RadItem senderItem, RadPropertyChangedEventArgs changeArgs);

        protected abstract void AttachToItemOverride(StateManagerAttachmentData attachData, RadItem item);

        public abstract StateDescriptionNode GetAvailableStates(string themeRoleName);

        #endregion

        #region Fields

        private LinkedList<string> defaultVisibleStates = new LinkedList<string>();			

        internal const char stateDelimiter = '.';
        private static string stateDelimiterString = new string(stateDelimiter, 1);

        #endregion
    }
}
