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
using System.Drawing.Drawing2D;

namespace CodeCell.Bricks.UIs.Dockables
{
   /// <summary>
   /// Utility for drawing
   /// </summary>
   internal class DrawUtility
   {
      /// <summary>
      /// Draw the context menu button for dockable tool window
      /// </summary>
      /// <param name="bounds">bounds of the button</param>
      /// <param name="hoover">flag indicating that mouse cursor is over button</param>
      /// <param name="g">graphics context</param>
      public static void DrawContextMenuButton (Rectangle bounds, bool hoover, Graphics g)
      {
         if (hoover)
         {
            g.FillRectangle (Brushes.LightGray, bounds);
            g.DrawRectangle (Pens.DarkGray, bounds);
         }

         using (GraphicsPath buttonBorderPath = new GraphicsPath ())
         {
            Point point1 = new Point (bounds.Left  + 3, bounds.Top + 5);
            Point point2 = new Point (bounds.Right - 2, bounds.Top + 5);
            Point point3 = new Point (bounds.Left + bounds.Width / 2, bounds.Bottom - 3);
            buttonBorderPath.AddPolygon (new Point[] { point1, point2, point3 });

            g.FillPath(Brushes.Navy, buttonBorderPath);
         }
      }

      /// <summary>
      /// Draw the auto-hide button for dockable tool window
      /// </summary>
      /// <param name="bounds">bounds of the button</param>
      /// <param name="hoover">flag indicating that mouse cursor is over button</param>
      /// <param name="autoHide">true if auto-hide mode is set, false otherwise</param>
      /// <param name="g">draw context</param>
      public static void DrawAutoHideButton (Rectangle bounds, bool autoHide, bool hoover, Graphics g)
      {
         if (hoover)
         {
            g.FillRectangle (Brushes.LightGray, bounds);
            g.DrawRectangle (Pens.DarkGray, bounds);
         }

         if (autoHide)
         {
            g.DrawLine (Pens.Navy, bounds.Left +  4, bounds.Top + 9, bounds.Left +  4, bounds.Top + 3);
            g.DrawLine (Pens.Navy, bounds.Left +  1, bounds.Top + 6, bounds.Left +  4, bounds.Top + 6);
            g.DrawLine (Pens.Navy, bounds.Left + 10, bounds.Top + 8, bounds.Left + 10, bounds.Top + 4);
            g.DrawLine (Pens.Navy, bounds.Left +  4, bounds.Top + 4, bounds.Left + 10, bounds.Top + 4);
            g.DrawLine (Pens.Navy, bounds.Left +  4, bounds.Top + 8, bounds.Left + 10, bounds.Top + 8);
            g.DrawLine (Pens.Navy, bounds.Left +  4, bounds.Top + 7, bounds.Left + 10, bounds.Top + 7);
         }
         else
         {
            g.DrawLine (Pens.Navy, bounds.Left + 3, bounds.Top + 8, bounds.Left + 9, bounds.Top + 8);
            g.DrawLine (Pens.Navy, bounds.Left + 6, bounds.Top + 8, bounds.Left + 6, bounds.Top + 11);
            g.DrawLine (Pens.Navy, bounds.Left + 4, bounds.Top + 2, bounds.Left + 8, bounds.Top + 2);
            g.DrawLine (Pens.Navy, bounds.Left + 4, bounds.Top + 2, bounds.Left + 4, bounds.Top + 8);
            g.DrawLine (Pens.Navy, bounds.Left + 8, bounds.Top + 2, bounds.Left + 8, bounds.Top + 8);
            g.DrawLine (Pens.Navy, bounds.Left + 7, bounds.Top + 2, bounds.Left + 7, bounds.Top + 8);
         }
   }
   
      /// <summary>
      /// Draw the close button for dockable tool window
      /// </summary>
      /// <param name="bounds">bounds of the button</param>
      /// <param name="hoover">flag indicating that mouse cursor is over button</param>
      /// <param name="g">context desenare</param>
      public static void DrawCloseButton (Rectangle bounds, bool hoover, Graphics g)
      {
         if (hoover)
         {
            g.FillRectangle (Brushes.LightGray, bounds);
            g.DrawRectangle (Pens.DarkGray, bounds);
         }

         using (Font font = new Font ("Courier", 8.0F, FontStyle.Bold))
         {
            string text = "X";

            SizeF textSize = g.MeasureString(text, font);
            Point location = bounds.Location;
            location.X += (int)((bounds.Width  - textSize.Width) / 2);
            location.Y += (int)((bounds.Height - textSize.Height) / 2);

            g.DrawString (text, font, Brushes.Maroon, location);
         }
      }
   }
}
