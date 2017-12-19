/***************************************************************************
 *   CopyRight (C) 2008 by SC Crom-Osec SRL                                *
 *   Contact:  contact@osec.ro                                             *
 *                                                                         *
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the Crom Free License as published by           *
 *   the SC Crom-Osec SRL; version 1 of the License                        *
 *                                                                         *
 *   This program is distributed in the hope that it will be useful,       *
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of        *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         *
 *   Crom Free License for more details.                                   *
 *                                                                         *
 *   You should have received a copy of the Crom Free License along with   *
 *   this program; if not, write to the contact@osec.ro                    *
 ***************************************************************************/

using System.Windows.Forms;
using System.Drawing;

namespace CodeCell.Bricks.UIs.Dockables
{
   /// <summary>
   /// Center button for guiding dock fill
   /// </summary>
   internal partial class CenterButtonToDockFill : DockButton
   {
      #region Instance.

      /// <summary>
      /// Default constructor
      /// </summary>
      public CenterButtonToDockFill ()
      {
         InitializeComponent ();
      }

      #endregion Instance.

      #region Public section.

      /// <summary>
      /// Show fill preview button inside the form
      /// </summary>
      public bool ShowFillPreview
      {
         get { return _fillImage.Visible; }
         set { _fillImage.Visible = value; }
      }

      /// <summary>
      /// Checks if the given mouse location is contained in the fill preview button
      /// </summary>
      /// <param name="location">location</param>
      /// <returns>true if the fill preview button is visible and contains mouse location</returns>
      public bool Contains (Point location)
      {
         if (ShowFillPreview == false)
         {
            return false;
         }

         Rectangle bounds = RectangleToScreen(_fillImage.Bounds);
         return bounds.Contains(location);
      }

      #endregion Public section.
   }
}