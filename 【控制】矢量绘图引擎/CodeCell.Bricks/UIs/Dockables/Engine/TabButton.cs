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

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CodeCell.Bricks.UIs.Dockables
{
   internal delegate void OnTabButtonSelectedChanged(object sender,bool isSelected);

   /// <summary>
   /// Implementation of a tab button
   /// </summary>
   internal class TabButton
   {
      #region Fields.

      private const int                SpaceImageText          = 5;
      private const int                ImageDimension          = 16;

      private Control                  _container              = null;
      private ITitleData               _titleData              = null;
      private Rectangle                _bounds                 = new Rectangle();
      private bool                     _selected               = false;
      private bool                     _hoover                 = false;
      private bool                     _showSelection          = false;
      private Color                    _notSelectedColor       = Color.DarkGray;
      private Color                    _selectedColor          = Color.Black;
      private Color                    _selectedBorderColor    = Color.FromArgb(75, 75, 111);
      private Color                    _selectedBackColor1     = Color.FromArgb(255, 242, 200);
      private Color                    _selectedBackColor2     = Color.FromArgb(255, 215, 157);

      internal OnTabButtonSelectedChanged _onTabButtonSelectedChanged = null;

      #endregion Fields.

      #region Instance.

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="container">container</param>
      /// <param name="titleData">title data</param>
      public TabButton (Control container, ITitleData titleData)
      {
         _container  = container;
         _titleData  = titleData;
      }

      #endregion Instance.

      #region Public section.

      /// <summary>
      /// Color of the text when the button is not selected
      /// </summary>
      public Color NotSelectedColor
      {
         get { return _notSelectedColor; }
         set { _notSelectedColor = value; }
      }

      /// <summary>
      /// Color of the text when the button is selected
      /// </summary>
      public Color SelectedColor
      {
         get { return _selectedColor; }
         set { _selectedColor = value; }
      }

      /// <summary>
      /// Selected border color
      /// </summary>
      public Color SelectedBorderColor
      {
         get { return _selectedBorderColor; }
         set { _selectedBorderColor = value; }
      }

      /// <summary>
      /// Low gradient color of the text when the button is selected
      /// </summary>
      public Color SelectedBackColor1
      {
         get { return _selectedBackColor1; }
         set { _selectedBackColor1 = value; }
      }

      /// <summary>
      /// High gradient color of the text when the button is selected
      /// </summary>
      public Color SelectedBackColor2
      {
         get { return _selectedBackColor2; }
         set { _selectedBackColor2 = value; }
      }

      /// <summary>
      /// Reset the size of the tab button to empty.
      /// </summary>
      /// <remarks>
      /// It also remove the selection flag.
      /// </remarks>
      public void Reset ()
      {
         _bounds.X        = 0;
         _bounds.Y        = 0;
         _bounds.Width    = 0;
         _bounds.Height   = 0;

         Selected = false;
      }

      /// <summary>
      /// Title data required to draw the button
      /// </summary>
      public ITitleData TitleData
      {
         get { return _titleData; }
      }

      /// <summary>
      /// Bounds of the tab buttom
      /// </summary>
      public Rectangle Bounds
      {
         get { return _bounds; }
      }

      /// <summary>
      /// Flag indicating if the button is selected or not.
      /// </summary>
      public bool Selected
      {
         get { return _selected; }
         set 
         {
             if (_selected != value)
             {
                 if (_onTabButtonSelectedChanged != null)
                     _onTabButtonSelectedChanged(this, value);
                 _selected = value;
             }
         }
      }

      /// <summary>
      /// Flag indicating if the button is under mouse cursor
      /// </summary>
      public bool Hoover
      {
         get { return _hoover; }
         set { _hoover = value; }
      }

      /// <summary>
      /// Flag indicating if the button should draw border and gradient background when is selected
      /// </summary>
      public bool ShowSelection
      {
         get { return _showSelection; }
         set { _showSelection = value; }
      }

      /// <summary>
      /// Draw the tab button
      /// </summary>
      /// <param name="bounds">bounds of the tab button</param>
      /// <param name="scris">font for drawing the button text</param>
      /// <param name="vertical">true if the buttone is vertical</param>
      /// <param name="g">g</param>
      public void Draw (Rectangle bounds, Font font, bool vertical, Graphics g)
      {
         Color drawColor   = NotSelectedColor;
         Font drawFont     = font;
         if (Selected)
         {
            drawFont    = new Font (font, FontStyle.Bold);
            drawColor   = SelectedColor;
         }
         else
         {
            drawFont = new Font (font, FontStyle.Regular);
         }

         using (drawFont)
         {
            _bounds.Location = bounds.Location;
            _bounds.Size     = bounds.Size;

            string text = _titleData.Title();
            Icon icon   = _titleData.Icon;

            Rectangle imageBounds = new Rectangle(0, 0, ImageDimension, ImageDimension);
            SizeF textSize        = g.MeasureString (text, drawFont);
            SizeF averageTextSize = g.MeasureString("x", drawFont);
            textSize.Width       += averageTextSize.Width;

            int textPosition      = ImageDimension + 2 * SpaceImageText;
            int textHeight        = (int)textSize.Height;
            int textWidth         = 0;

            if (vertical)
            {
               textWidth      = Math.Max (0, Math.Min ((int)textSize.Width, bounds.Height - textPosition));
               _bounds.Height = textPosition + textWidth;
            }
            else
            {
               textWidth      = Math.Max (0, Math.Min ((int)textSize.Width, bounds.Width - textPosition));
               _bounds.Width  = textPosition + textWidth;
            }

            g.SetClip (_bounds);

            if (Selected && ShowSelection)
            {
               using (Brush backBrush = new LinearGradientBrush (_bounds, SelectedBackColor1, SelectedBackColor2, LinearGradientMode.Vertical))
               {
                  g.FillRectangle (backBrush, _bounds);
               }

               using (Pen borderPen = new Pen (SelectedBorderColor))
               {
                  g.DrawRectangle (borderPen, _bounds.Left, _bounds.Top, _bounds.Width - 1, _bounds.Height - 1);
               }
            }

            if (Hoover)
            {
               if (Hoover)
               {
                  using (Brush brush = new SolidBrush (Color.FromArgb (255, 233, 186)))
                  {
                     g.FillRectangle (brush, _bounds);
                  }
               }

               using (Pen pen = new Pen (Color.FromArgb (75, 75, 111)))
               {
                  g.DrawRectangle (pen, _bounds.Left, _bounds.Top, _bounds.Width - 1, _bounds.Height - 1);
               }
            }

            using (Bitmap bmp = icon.ToBitmap ())
            {
               if (vertical)
               {
                  imageBounds.Y = bounds.Y + SpaceImageText;
                  imageBounds.X = bounds.X + Math.Max (0, (bounds.Width - ImageDimension) / 2 - 0);
               }
               else
               {
                  imageBounds.X = bounds.X + SpaceImageText;
                  imageBounds.Y = bounds.Y + Math.Max (0, (bounds.Height - ImageDimension) / 2 - 0);
               }
               g.DrawImage (bmp, imageBounds);
                //fdc
               //g.DrawRectangle(Pens.Red, imageBounds);
            }

            Rectangle textBounds = new Rectangle(0, 0, textWidth, textHeight);

            int textX = bounds.X + 2 * SpaceImageText + ImageDimension;
            int textY = bounds.Y + Math.Max (0, (bounds.Height - textHeight) / 2 - 0);
            if (vertical)
            {
               textX = bounds.X + Math.Max (0, (bounds.Width - textHeight) / 2 - 0);
               textY = bounds.Y + 2 * SpaceImageText + ImageDimension;
            }

            if (textBounds.Width > 0 && textBounds.Height > 0)
            {
               using (Bitmap textImg = new Bitmap (textBounds.Width, textBounds.Height))
               {
                  using (Graphics gt = Graphics.FromImage (textImg))
                  {
                     TextRenderer.DrawText (gt, text, drawFont, textBounds, drawColor,
                        TextFormatFlags.ModifyString | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine |
                        TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis);
                  }

                  if (vertical)
                  {
                     g.TranslateTransform (textX + textHeight, textY);
                     g.RotateTransform (90);

                     g.DrawImage (textImg, 0, 0);

                     g.RotateTransform (-90);
                     g.TranslateTransform (-(textX + textHeight), -textY);
                  }
                  else
                  {
                     g.DrawImage (textImg, textX, textY);
                  }
               }
            }
         }
      }

      #endregion Public section.
   }
}
