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
   internal class TextUtility
   {
      /// <summary>
      /// End ellipsis for wrapping text are "..."
      /// </summary>
      public const string EndEllipsis = "...";

      /// <summary>
      /// Wrap the text setting ellipsis if not enough length to draw entire text
      /// </summary>
      /// <param name="text">text to be wrapped</param>
      /// <param name="limitLength">limit length</param>
      /// <param name="font">font</param>
      /// <returns>wrapped text</returns>
      public static string WrapText (string text, int limitLength, Font font)
      {
         if (limitLength <= 0 || text == string.Empty || text == null)
         {
            return string.Empty;
         }

         using (Bitmap bmp = new Bitmap (1, 1))
         {
            using (Graphics g = Graphics.FromImage (bmp))
            {
               SizeF dimText = g.MeasureString (text, font);
               if (dimText.Width <= limitLength)
               {
                  return text;
               }

               SizeF dim3P = g.MeasureString (EndEllipsis, font);
               SizeF dimC1 = g.MeasureString (text.Substring (0, 1), font);

               int preferredWidth = limitLength - (int)dim3P.Width;
               if (preferredWidth <= dimC1.Width || dimText.Width <= 0)
               {
                  return string.Empty;
               }

               double ratio = (double)preferredWidth / (double)dimText.Width;

               int charsCount = text.Length;
               int preferredCount = (int)(charsCount * ratio);

               string preferredText = text.Substring (0, preferredCount);
               SizeF preferredSize = g.MeasureString (preferredText, font);
               if (preferredSize.Width > preferredWidth)
               {
                  while (preferredSize.Width > preferredWidth && preferredCount > 0)
                  {
                     preferredCount--;
                     preferredText = text.Substring (0, preferredCount);
                     preferredSize = g.MeasureString (preferredText, font);
                  }
               }
               else if (preferredSize.Width < preferredWidth)
               {
                  while (preferredSize.Width < preferredWidth && preferredCount < text.Length - 1)
                  {
                     preferredCount++;
                     preferredText = text.Substring (0, preferredCount);
                     preferredSize = g.MeasureString (preferredText, font);
                  }
               }

               return preferredText + EndEllipsis;
            }
         }
      }
   }
}
