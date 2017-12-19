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

namespace CodeCell.Bricks.UIs.Dockables
{
   /// <summary>
   /// Dock preview form
   /// </summary>
   internal partial class DockPreview : Form
   {
      #region Instance.

      /// <summary>
      /// Default constructor
      /// </summary>
      public DockPreview ()
      {
         InitializeComponent ();
         TabStop = false;
      }

      #endregion Instance.

      #region Protected section.

      /// <summary>
      /// Hide form from Alt-Tab list
      /// </summary>
      protected override CreateParams CreateParams
      {
         get
         {
            CreateParams cp = base.CreateParams;
            // turn on WS_EX_TOOLWINDOW style bit
            cp.ExStyle |= 0x80;
            return cp;
         }
      }

      #endregion Protected section.
   }
}