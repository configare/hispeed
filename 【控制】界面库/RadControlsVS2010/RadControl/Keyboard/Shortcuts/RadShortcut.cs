using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls
{
    /// <summary>
    /// Describes a combination of keys that may be used as a shortcut to RadItem.PerformClick method or any other arbitrary command.
    /// </summary>
    public class RadShortcut
    {
        #region Fields

        private Keys modifiers;
        private List<Keys> keyMappings;
        public static string ControlText = "Ctrl";
        public static string ShiftText = "Shift";
        public static string AltText = "Alt";
        public const char Delimiter = '+';

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor. Initializes an empty <see cref="RadShortcut">RadShortcut</see> instance.
        /// </summary>
        public RadShortcut()
        {
            this.modifiers = Keys.None;
            this.keyMappings = new List<Keys>();
        }

        /// <summary>
        /// Initializes a new <see cref="RadShortcut">RadShortcut</see> instance, using the specified modifiers and key mappings.
        /// </summary>
        /// <param name="modifiers"></param>
        /// <param name="mappings"></param>
        public RadShortcut(Keys modifiers, params Keys[] mappings)
            : this()
        {
            this.modifiers = modifiers;
            if (mappings != null)
            {
                this.keyMappings.AddRange(mappings);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a list with all the Keys that form the shortcut combination.
        /// E.g. we may have M+O and a Modifier CTRL, then the valid shortcut will be CTRL+M+O
        /// </summary>
        public Keys[] KeyMappings
        {
            get
            {
                return this.keyMappings.ToArray();
            }
        }

        /// <summary>
        /// Gets or sets the Keys value that describes the modifiers for the shortcut.
        /// </summary>
        public Keys Modifiers
        {
            get
            {
                return this.modifiers;
            }
        }

        /// <summary>
        /// Determines whether the Control modifier key is applied.
        /// </summary>
        public bool Ctrl
        {
            get
            {
                return (this.modifiers & Keys.Control) == Keys.Control;
            }
        }

        /// <summary>
        /// Determines whether the Alt modifier key is applied.
        /// </summary>
        public bool Alt
        {
            get
            {
                return (this.modifiers & Keys.Alt) == Keys.Alt;
            }
        }

        /// <summary>
        /// Determines whether the Shift modifier key is applied.
        /// </summary>
        public bool Shift
        {
            get
            {
                return (this.modifiers & Keys.Shift) == Keys.Shift;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the specified Keys are part 
        /// </summary>
        /// <param name="modifiers"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public bool IsShortcutCombination(Keys modifiers, params Keys[] keys)
        {
            if (this.keyMappings.Count == 0)
            {
                return false;
            }
            if (modifiers != this.modifiers)
            {
                return false;
            }
            if (keys == null)
            {
                return false;
            }

            if (keys.Length != this.keyMappings.Count)
            {
                return false;
            }

            bool match = true;
            for (int i = 0; i < keys.Length; i++)
            {
                if (this.keyMappings[i] != keys[i])
                {
                    match = false;
                    break;
                }
            }

            return match;
        }

        /// <summary>
        /// Determines whether the specified Keys are part of a shortcut combination.
        /// E.g. if we have a key mapping CTRL+M+O and the provided keys are CTRL+M, the method will return true.
        /// </summary>
        /// <param name="modifiers"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public bool IsPartialShortcutCombination(Keys modifiers, params Keys[] keys)
        {
            if (this.keyMappings.Count == 0)
            {
                return false;
            }
            if (modifiers != this.modifiers)
            {
                return false;
            }
            if (keys == null)
            {
                return false;
            }

            if (keys.Length >= this.keyMappings.Count)
            {
                return false;
            }

            bool match = true;
            for (int i = 0; i < keys.Length; i++)
            {
                if (this.keyMappings[i] != keys[i])
                {
                    match = false;
                    break;
                }
            }

            return match;
        }

        /// <summary>
        /// Determines whether the specified key is present in the RadDockShortcut KeyMappings list.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsMappingKey(Keys key)
        {
            return this.keyMappings.IndexOf(key) >= 0;
        }

        /// <summary>
        /// Gets the human-readable represention of the current key settings.
        /// </summary>
        /// <returns></returns>
        public string GetDisplayText()
        {
            //no key mappings, ignore modifiers
            if (this.keyMappings.Count == 0)
            {
                return string.Empty;
            }

            string modifierText = string.Empty;
            if (this.Ctrl)
            {
                modifierText += ControlText;
            }
            if (this.Shift)
            {
                if (modifierText.Length > 0)
                {
                    modifierText += Delimiter;
                }
                modifierText += ShiftText;
            }
            if (this.Alt)
            {
                if (modifierText.Length > 0)
                {
                    modifierText += Delimiter;
                }
                modifierText += AltText;
            }

            string mappingsText = string.Empty;
            KeysConverter converter = new KeysConverter();
            foreach (Keys mapping in this.keyMappings)
            {
                mappingsText += converter.ConvertToString(mapping) + Delimiter;
            }
            //remove last "+"
            mappingsText = mappingsText.Remove(mappingsText.Length - 1, 1);

            if (string.IsNullOrEmpty(mappingsText))
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(modifierText))
            {
                return mappingsText;
            }

            return modifierText + Delimiter + mappingsText;
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return "RadShortcut - " + this.GetDisplayText();
        }

        #endregion
    }
}
