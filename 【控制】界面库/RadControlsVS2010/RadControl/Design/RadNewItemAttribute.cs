using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls//.Design
{
    /// <summary>
    /// Represents a new item attribute. 
    /// </summary>
    public class RadNewItemAttribute : Attribute
    {
        private readonly string newItemName;
        private readonly bool allowEditItemText;
        private readonly bool paintGlyph = true;
        private readonly bool addMenuVerb = true;

        /// <summary>
        /// Initializes a new instance of the RadNewItemAttribute class.
        /// </summary>
        /// <param name="newItemName"></param>
        /// <param name="allowEditItemText"></param>
        public RadNewItemAttribute(string newItemName, bool allowEditItemText)
        {
            this.newItemName = newItemName;
            this.allowEditItemText = allowEditItemText;
        }

        /// <summary>
        /// Initializes a new instance of the RadNewItemAttribute class.
        /// </summary>
        /// <param name="newItemName"></param>
        /// <param name="allowEditItemText"></param>
        /// <param name="paintGlyph"></param>
        public RadNewItemAttribute(string newItemName, bool allowEditItemText, bool paintGlyph)
        {
            this.newItemName = newItemName;
            this.allowEditItemText = allowEditItemText;
            this.paintGlyph = paintGlyph;
        }

        /// <summary>
        /// Initializes a new instance of the RadNewItemAttribute class.
        /// </summary>
        /// <param name="newItemName"></param>
        /// <param name="allowEditItemText"></param>
        /// <param name="paintGlyph"></param>
        /// <param name="addMenuVerb"></param>
        public RadNewItemAttribute(string newItemName, bool allowEditItemText, bool paintGlyph, bool addMenuVerb)
        {
            this.newItemName = newItemName;
            this.allowEditItemText = allowEditItemText;
            this.paintGlyph = paintGlyph;
            this.addMenuVerb = addMenuVerb;
        }

        /// <summary>
        /// Gets a string representing the new item text.
        /// </summary>
        public string NewItemName
        {
            get
            {
                return newItemName;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the item should be editable.
        /// </summary>
        public bool AllowEditItemText
        {
            get
            {
                return allowEditItemText;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a glyph should be added.
        /// </summary>
        public bool PaintGlyph
        {
            get
            {
                return paintGlyph;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a verb should be added.
        /// </summary>
        public bool AddMenuVerb
        {
            get
            {
                return addMenuVerb;
            }
        }
    }
}
