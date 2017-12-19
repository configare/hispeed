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

namespace CodeCell.Bricks.UIs.Dockables
{
   partial class DockContainer
   {
      /// <summary> 
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent ()
      {
         this.components = new System.ComponentModel.Container ();
         this._mouseCheckTimer = new System.Windows.Forms.Timer (this.components);
         this.SuspendLayout ();
         // 
         // _temporizatorTestCursor
         // 
         this._mouseCheckTimer.Enabled = true;
         // 
         // DockContainer
         // 
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
         this.BackColor = System.Drawing.SystemColors.Window;
         this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.Name = "DockContainer";
         this.Size = new System.Drawing.Size (577, 485);

         this.ResumeLayout (false);

      }

      #endregion

      private System.Windows.Forms.Timer _mouseCheckTimer;
   }
}
