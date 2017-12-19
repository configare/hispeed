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

using System.Drawing;

namespace CodeCell.Bricks.UIs.Dockables
{
   /// <summary>
   /// Data from tool window title
   /// </summary>
   internal interface ITitleData
   {
      /// <summary>
      /// Title of the tool window
      /// </summary>
      /// <returns>title of the tool window</returns>
      string Title ();

      /// <summary>
      /// Icon of the tool window
      /// </summary>
      Icon Icon
      {
         get ;
      }
   }
}
