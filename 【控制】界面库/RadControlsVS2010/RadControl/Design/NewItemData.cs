using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Design
{
    /// <summary>
    /// Represents a new item's data.
    /// </summary>
    public class NewItemData
    {
        public bool AllowEdit;
        public string EditText;
        public RadItemCollection TargetCollection;

        public bool PaintGlyph = true;
        public bool AddMenuVerb = true;

        /// <summary>
        /// Initializes a new instance of the NewItemData class.
        /// </summary>
        /// <param name="allowEdit"></param>
        /// <param name="editText"></param>
        /// <param name="targetCollection"></param>
        public NewItemData(bool allowEdit, string editText, RadItemCollection targetCollection)
        {
            this.AllowEdit = allowEdit;
            this.EditText = editText;
            this.TargetCollection = targetCollection;
        }

        /// <summary>
        /// Initializes a new instance of the NewItemData class.
        /// </summary>
        /// <param name="allowEdit"></param>
        /// <param name="editText"></param>
        /// <param name="targetCollection"></param>
        /// <param name="paintGlyph"></param>
        public NewItemData(bool allowEdit, string editText, RadItemCollection targetCollection, bool paintGlyph)
        {
            this.AllowEdit = allowEdit;
            this.EditText = editText;
            this.TargetCollection = targetCollection;
            this.PaintGlyph = paintGlyph;
        }

        /// <summary>
        /// Initializes a new instance of the NewItemData class.
        /// </summary>
        /// <param name="allowEdit"></param>
        /// <param name="editText"></param>
        /// <param name="targetCollection"></param>
        /// <param name="paintGlyph"></param>
        /// <param name="addMenuVerb"></param>
        public NewItemData(bool allowEdit, string editText, RadItemCollection targetCollection, bool paintGlyph, bool addMenuVerb)
        {
            this.AllowEdit = allowEdit;
            this.EditText = editText;
            this.TargetCollection = targetCollection;
            this.PaintGlyph = paintGlyph;
            this.AddMenuVerb = addMenuVerb;
        }
    }
}
