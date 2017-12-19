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
   /// <summary>
   /// Utility for enums
   /// </summary>
   internal class EnumUtility
   {
      /// <summary>
      /// Check if current value contains the checked value
      /// </summary>
      /// <param name="currentValue">current value</param>
      /// <param name="checkedValue">checked value</param>
      /// <returns>true if the current value contains checked value</returns>
      public static bool Contains (object currentValue, object checkedValue)
      {
         return ((int)currentValue & (int)checkedValue) == (int)checkedValue;
      }
   }
}
