using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls
{
    public sealed class RadShortcutManager : IKeyboardListener
    {
        #region Fields

        [ThreadStatic]
        private static RadShortcutManager instance;
        private WeakReferenceList<IShortcutProvider> shortcutProviders;
        private bool hookInstalled;
        private bool enabled;
        private List<Keys> currentKeys;

        #endregion

        #region Constructor

        private RadShortcutManager()
        {
            this.enabled = true;
            this.shortcutProviders = new WeakReferenceList<IShortcutProvider>(true, false);
            this.currentKeys = new List<Keys>();
        }

        #endregion

        #region Properties

        public static RadShortcutManager Instance
        {
            get
            {
                //Field is ThreadStatic. No need of double-checking.
                if (instance == null)
                {
                    instance = new RadShortcutManager();
                }
                return instance;
            }
        }

        public bool Enabled
        {
            get
            {
                return this.enabled;
            }
            set
            {
                this.enabled = value;
            }
        }

        /// <summary>
        /// Gets the count of all shortcut providers currently registered with this instance.
        /// </summary>
        public int ProvidersCount
        {
            get
            {
                return this.shortcutProviders.Count;
            }
        }

        #endregion

        #region Methods

        public void AddShortcutProvider(IShortcutProvider provider)
        {
            if (!this.ContainsShortcutProvider(provider))
            {
                this.shortcutProviders.Add(provider);
                this.UpdateHook();
            }
        }

        public void RemoveShortcutProvider(IShortcutProvider provider)
        {
            int index = this.shortcutProviders.IndexOf(provider);
            if (index >= 0)
            {
                this.shortcutProviders.Remove(provider);
                this.UpdateHook();
            }
        }

        public bool ContainsShortcutProvider(IShortcutProvider provider)
        {
            return this.shortcutProviders.IndexOf(provider) >= 0;
        }

        private void UpdateHook()
        {
            if (this.shortcutProviders.Count > 0)
            {
                if (!this.hookInstalled)
                {
                    RadKeyboardFilter.Instance.AddListener(this);
                    this.hookInstalled = true;
                }
            }
            else
            {
                if (this.hookInstalled)
                {
                    RadKeyboardFilter.Instance.RemoveListener(this);
                    this.hookInstalled = false;
                }
            }
        }

        private bool IsModifierKey(Keys currKey)
        {
            return currKey == Keys.ControlKey ||
                   currKey == Keys.ShiftKey ||
                   currKey == Keys.Menu;
        }

        #endregion

        #region IKeyboardListener

        MessagePreviewResult IKeyboardListener.OnPreviewKeyDown(Control target, KeyEventArgs e)
        {
            if (!this.enabled)
            {
                return MessagePreviewResult.NotProcessed;
            }

            if (this.IsModifierKey(e.KeyCode))
            {
                return MessagePreviewResult.NotProcessed;
            }

            this.currentKeys.Add(e.KeyCode);

            Keys modifiers = e.Modifiers;
            Keys[] mappings = this.currentKeys.ToArray();
            bool isPartialShortcut = false;

            //check for shortcut combination first
            foreach (IShortcutProvider provider in this.shortcutProviders)
            {
                foreach (RadShortcut shortcut in provider.Shortcuts)
                {
                    if (shortcut.IsShortcutCombination(modifiers, mappings))
                    {
                        this.currentKeys.Clear();
                        ShortcutEventArgs args = new ShortcutEventArgs(target, shortcut);
                        provider.OnShortcut(args);
                        if (args.Handled)
                        {
                            return MessagePreviewResult.ProcessedNoDispatch;
                        }

                        //See ticket ID: 316158
                        //By returning NotProcessed we interrupt the rerouting of the
                        //keyboard message to the other shortcutproviders which might
                        //want to process the shortcut.
                        //return MessagePreviewResult.NotProcessed;
                    }
                    else if (!isPartialShortcut && shortcut.IsPartialShortcutCombination(modifiers, mappings))
                    {
                        PartialShortcutEventArgs args = new PartialShortcutEventArgs(target, shortcut, mappings);
                        provider.OnPartialShortcut(args);
                        if (args.Handled)
                        {
                            isPartialShortcut = true;
                        }
                    }
                }
            }

            if (isPartialShortcut)
            {
                return MessagePreviewResult.ProcessedNoDispatch;
            }

            //the key is not processed, clear all currently collected keys
            this.currentKeys.Clear();

            return MessagePreviewResult.NotProcessed;
        }

        MessagePreviewResult IKeyboardListener.OnPreviewKeyPress(Control target, KeyPressEventArgs e)
        {
            return MessagePreviewResult.NotProcessed;
        }

        MessagePreviewResult IKeyboardListener.OnPreviewKeyUp(Control target, KeyEventArgs e)
        {
            //we have received a KeyUp for a modifier key, clear currently collected key mappings
            if (this.IsModifierKey(e.KeyCode))
            {
                this.currentKeys.Clear();
            }

            return MessagePreviewResult.NotProcessed;
        }

        #endregion
    }
}
