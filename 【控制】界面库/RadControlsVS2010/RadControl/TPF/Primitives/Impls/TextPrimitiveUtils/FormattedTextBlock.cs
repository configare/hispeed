using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Paint;
using Telerik.WinControls.Layouts;
using System.Diagnostics;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.TextPrimitiveUtils
{
    public class FormattedTextBlock
    {
        //consts
        const float defaultLineHeight = 8f;

        //members
        private List<TextLine> lines = new List<TextLine>();        

        //properties
        public List<TextLine> Lines
        {
            get
			{
				return lines;
			}
            set
			{
				lines = value;
			}
        }

        //helper function

        /// <summary>
        /// Move text blocks to next line if there is not avaible space for the current line
        /// </summary>
        /// <param name="textWrap"></param>
        /// <param name="proposedSize"></param>
        public void ArrangeLines(bool textWrap,
								 SizeF proposedSize)
        {
            if (!textWrap)
            {
                return;
            }

            if (proposedSize.Width <= 0 && proposedSize.Height <= 0)
            {
                return;
            }

            for (int i = 0; i < lines.Count; ++i)
            {
                while (GetTextLineSize(lines[i]).Width > proposedSize.Width
                       && lines[i].List.Count > 1)
                {
                    //check if len > avaible size
                    //have to move last item from this line to begining of the next line
                    int lastItemPostion = lines[i].List.Count - 1;//get the last item
                    FormattedText itemToBeMoved = lines[i].List[lastItemPostion];//get an item to be moved
                    lines[i].List.RemoveAt(lastItemPostion);//remove from current line
                    if (i == lines.Count - 1) //if theren't next line
                    {
                        lines.Add(new TextLine());//add new line
                    }

                    TextLine nextLine = lines[i + 1];
                    if (nextLine.List.Count > 0 && !nextLine.List[0].StartNewLine)
                    {
                        nextLine.List.Insert(0, itemToBeMoved);//insert the item from current line in the                     
                    }
                    else
                    {
                        TextLine textLine = new TextLine();
                        textLine.List.Add(itemToBeMoved);
                        lines.Insert(i + 1, textLine);
                    }
                }
            }
        }

        public void RecalculateBlockLines(bool TextWrap)
        {
            if (!TextWrap)
            {
                return;
            }

            for (int i = 0; i < this.Lines.Count; ++i)
            {
                for (int j = 0; j < this.Lines[i].List.Count; ++j)
                {
                    FormattedText baseText = this.Lines[i].List[j];
                    if (string.IsNullOrEmpty(baseText.Text))
                    {
                        continue;
                    }

                    string[] words = baseText.Text.Trim().Split(' ');
                    for (int k = 1; k < words.Length; ++k)
                    {
                        FormattedText formatedText = new FormattedText(baseText);
                        formatedText.Text = words[k];

                        this.Lines[i].List.Insert(j + k, formatedText);
                    }

                    baseText.Text = words[0] + " ";
                }

                int count = this.Lines[i].List.Count;
                if (count > 0)
                {
                    string text = this.Lines[i].List[count - 1].Text;
                    if (!string.IsNullOrEmpty(text))
                    {
                        this.Lines[i].List[count - 1].Text = this.Lines[i].List[count - 1].Text.TrimEnd();
                    }
                }
            }
        }

        public SizeF GetTextSize(SizeF proposedSize, TextParams textParams)
        {
            using (StringFormat sf = textParams.CreateStringFormat())
            {
                return this.GetTextSize(proposedSize, textParams.useCompatibleTextRendering, sf, TextFormatFlags.Default, textParams.textWrap);// && !textParams.autoSize);
            }
        }

        /// <summary>
        /// Calculate Size of the whole FormattedTextBlock
        /// </summary>
        /// <param name="proposedSize"></param>
        /// <param name="useCompatibleTextRendering"></param>
        /// <param name="sf"></param>
        /// <param name="textFormatFlags"></param>
        /// <param name="textWrap"></param>
        /// <returns></returns>
        public SizeF GetTextSize(SizeF proposedSize, bool useCompatibleTextRendering, StringFormat sf, TextFormatFlags textFormatFlags, bool textWrap)
        {
            if (textWrap)
            {
                this.RecalculateBlockLines(textWrap);
                this.ArrangeLines(textWrap, proposedSize);
            }
            
            SizeF totalSize = Size.Empty;
            SizeF res = Size.Empty;
            float lineHeight = 0f;

            foreach (TextLine line in lines)
            {
                res = GetTextLineSize(line);
                if (res.Height > 2f)
                {
                    lineHeight = res.Height;
                }
                else
                {
                    lineHeight = 0;
                }
                
                line.LineSize = new SizeF(res.Width, lineHeight);
                totalSize.Height += lineHeight;
                totalSize.Width = Math.Max(res.Width, totalSize.Width);
            }

            return totalSize;
        }

        /// <summary>
        /// Calculate text size of the Single Text Line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static SizeF GetTextLineSize(TextLine line)
        {
            SizeF currentLineSize = new SizeF(line.GetLineOffset(), 0f);
            SizeF result = SizeF.Empty;
            
            foreach (FormattedText formatedText in line.List)
            {
                if (!string.IsNullOrEmpty(formatedText.Text) || formatedText.Image != null || formatedText.StartNewLine )
                {
                    result = formatedText.BlockSize;
                    currentLineSize.Width += formatedText.StartNewLine && string.IsNullOrEmpty(formatedText.Text) && formatedText.Image == null ? 0: result.Width;
                    currentLineSize.Height = Math.Max(currentLineSize.Height, result.Height);
                    line.BaseLine = Math.Max(line.BaseLine, formatedText.BaseLine);
                }
            }

            return currentLineSize;
        }

        /// <summary>
        /// Draw whole FormattedTextBlock
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="paintingRectangleParam"></param>
        /// <param name="useCompatibleTextRendering"></param>
        /// <param name="format"></param>
        /// <param name="flags"></param>
        /// <param name="textWrap"></param>
        /// <param name="clipText"></param>
        public void PaintFormatTextBlock(
			IGraphics graphics,
			RectangleF paintingRectangleParam,
			bool useCompatibleTextRendering,
			StringFormat format,
			TextFormatFlags flags,
			bool textWrap,
            bool clipText)
        { 
            if (this.lines.Count == 0)
            {
                return;
            }

            SizeF textDesiredSize = this.GetTextSize(paintingRectangleParam.Size, useCompatibleTextRendering, format, flags, textWrap);
            if (textDesiredSize.Height > paintingRectangleParam.Height)
            {
                textDesiredSize.Height = paintingRectangleParam.Height;
            }

            if (textDesiredSize.Width > paintingRectangleParam.Width)
            {
                textDesiredSize.Width = paintingRectangleParam.Width;
            }

            RectangleF paintingRectangle = LayoutUtils.VAlign(textDesiredSize, paintingRectangleParam, lines[0].GetLineAligment());
            paintingRectangle.X += lines[0].GetLineOffset(); 
            foreach (TextLine line in lines)
            {  
                RectangleF currentLineRect = LayoutUtils.HAlign(line.LineSize, paintingRectangle, line.GetLineAligment());
                currentLineRect.Height = line.LineSize.Height;                

                if (currentLineRect.X < paintingRectangle.X)
                {
                    currentLineRect.X = paintingRectangle.X;
                }

                if (currentLineRect.Width > paintingRectangle.Width)
                {
                    currentLineRect.Width = paintingRectangle.Width;
                }

                if (currentLineRect.Y < paintingRectangle.Y)
                {
                    currentLineRect.Y = paintingRectangle.Y;
                }

                if (currentLineRect.Height > paintingRectangle.Height)
                {
                    currentLineRect.Height = paintingRectangle.Height;
                }

                float lineHeight = defaultLineHeight;
               // bool emptyLine = true;
                foreach (FormattedText formatedText in line.List)
                {
                    //emptyLine = !formatedText.StartNewLine;
                    if (string.IsNullOrEmpty(formatedText.Text) && formatedText.Image == null)
                    {
                        continue;
                    }

					PointF currentLineBeginPoint = new PointF(currentLineRect.Location.X + line.GetLineOffset(), currentLineRect.Y);

                    //fix to draw text only into painting rectangle
                    if (currentLineRect.Location.Y > paintingRectangleParam.Height)
                    {
                        return;
                    }

					formatedText.PaintFormatText(graphics,                                                    
												 paintingRectangle,
												 useCompatibleTextRendering,                                                 
												 flags, 
												 currentLineBeginPoint,
												 line.LineSize.Height,
                                                 line.BaseLine,
                                                 clipText);
                    
					currentLineRect.Location = new PointF(currentLineRect.Location.X + formatedText.BlockSize.Width, currentLineRect.Location.Y);
                    
					lineHeight = line.LineSize.Height;
                    //emptyLine = false;
                }

                paintingRectangle.Location = new PointF(paintingRectangle.X, paintingRectangle.Y + lineHeight);//(!emptyLine ? lineHeight : 0));               
            }
        }

        #region Link support
        /// <summary>
        /// Occurs when the mouse is up the element.
        /// </summary>        
        public void MouseUp(object sender, MouseEventArgs e)
        {
            FormattedText formattedText = this.IsMouseOverBlock(sender, e);
            if (formattedText == null)
            {
                return;
            }

            formattedText.FontColor = Color.Red;
            RunLink(formattedText.Link);
        }      

        /// <summary>
        /// Occurs when the mouse pointer is moved over the element.
        /// </summary>        
        public void MouseMove(object sender, MouseEventArgs e)
        {
            FormattedText formattedText = this.IsMouseOverBlock(sender, e);
            if (formattedText == null)
            {
                return;
            }

            Cursor.Current = Cursors.Hand;
        }

        private FormattedText IsMouseOverBlock(object sender, MouseEventArgs e)
		{
			RadElement element = sender as RadElement;
            Debug.Assert(element != null, "Element is not RadElement");
            Point elementAtPoint = element.PointFromControl(e.Location);
            int linesCount = lines.Count;   
            for (int i = 0; i < linesCount; ++i)
            {
                TextLine textLine = lines[i];
                int textLineCount = textLine.List.Count;
                for (int j = 0; j < textLineCount; ++j)
                {                
                    FormattedText formattedText = textLine.List[j];                    
                    if (!string.IsNullOrEmpty(formattedText.Link) && formattedText.DrawingRectangle.Contains(elementAtPoint))
                    {
                        return formattedText;//found link under mouse
                    }
				}
            }

            return null;//notfound
		}

        private static void RunLink(string link)
        {
            try
            {
                //FR# 301805  
                if (link.Contains("~"))
                {
                    link = link.Replace("~", System.Windows.Forms.Application.StartupPath);
                }

                System.Diagnostics.Process.Start(link);
            }
            catch
            {
                //MessageBox.Show("Unable to start '" + link+"'");
            }
        }
        #endregion        
    }
}
