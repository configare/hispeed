using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Paint;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Diagnostics;
using Telerik.WinControls.Themes.Design;
using System.Collections;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.TextPrimitiveUtils
{
    public class TinyHTMLParsers
    {
        public class TinyHTMLParsersData
        {
            public Stack<int> lastListNumberCount = new Stack<int>();
            public Stack<FormattedText.HTMLLikeListType> lastListType = new Stack<FormattedText.HTMLLikeListType>();
            public Stack<FormattedText> lastFormattedText = new Stack<FormattedText>();         
        }
        /// <summary>
        /// check is the Text contains html command
        /// </summary>
        /// <param name="text">text to be checked</param>
        /// <returns>text to check</returns>
        public static bool IsHTMLMode(string text)
        {
            if (string.IsNullOrEmpty(text) ||
				text.Length < 7)
            {
                return false;
            }

            if (text[0] == '<' &&
                (text[1] == 'H' || text[1] == 'h') &&
                (text[2] == 'T' || text[2] == 't') &&
                (text[3] == 'M' || text[3] == 'm') &&
                (text[4] == 'L' || text[4] == 'l') &&
				text[5] == '>')
            {
                return true;
            }

            return false;
        }

        public static FormattedTextBlock Parse(TextParams textParams)
        {
            return Parse(textParams.text, textParams.foreColor, textParams.font.Name, textParams.font.Size, textParams.font.Style, textParams.alignment);
        }

         /// <summary>
        /// Main function for parsing process
        /// </summary>
        /// <param name="text">text to parse</param>
        /// <param name="baseForeColor">base Font color</param>
        /// <param name="baseFont">base font</param>
        /// <param name="fontSize">base font size</param>
        /// <param name="aligment">base textaligment</param>
        /// <returns>Formatted text block that contains the whole structure</returns>
        public static FormattedTextBlock Parse(string text, Color baseForeColor, string baseFont, float fontSize, ContentAlignment aligment)
        {
            return Parse(text, baseForeColor, baseFont, fontSize, FontStyle.Regular, aligment);
        }

        /// <summary>
        /// Main function for parsing process
        /// </summary>
        /// <param name="text">text to parse</param>
        /// <param name="baseForeColor">base Font color</param>
        /// <param name="baseFont">base font</param>
        /// <param name="fontSize">base font size</param>
        /// <param name="aligment">base textaligment</param>
        /// <param name="fontStyle"> base font style etc. Regular, Bold</param>
        /// <returns>Formatted text block that contains the whole structure</returns>
        public static FormattedTextBlock Parse(string text, Color baseForeColor, string baseFont, float fontSize, FontStyle fontStyle, ContentAlignment aligment)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new FormattedTextBlock();
            }

            Stack<FormattedText.HTMLLikeListType> lastListType = new Stack<FormattedText.HTMLLikeListType>();
           
            lastListType.Push(FormattedText.HTMLLikeListType.None);
            text = text.Replace("\\<", "&lt;").Replace("\\>", "&gt;").Replace("\r\n", "<br>").Replace("\n", "<br>");
            
            //prepare initially block
            FormattedTextBlock textBlock = new FormattedTextBlock();
            FormattedText previousFormattedText = new FormattedText();//this is a base item for creating HTML
            previousFormattedText.FontColor = baseForeColor;
            previousFormattedText.FontName = baseFont;
            previousFormattedText.FontSize = fontSize;
            previousFormattedText.ContentAlignment = aligment;
            previousFormattedText.FontStyle = fontStyle;

            //create tokens
            StringTokenizer tokenizer = new StringTokenizer(text, "<");            
            int count = tokenizer.Count();
            bool hasOpenTag = text.IndexOf("<") > -1; 
            TextLine currentLine = new TextLine();
            textBlock.Lines.Add(currentLine);
            bool shouldProduceNewLine = false;
        
            TinyHTMLParsersData parsersData = new TinyHTMLParsersData();
            //enumerate tokens
            for (int i = 0; i < count; ++i)
            {                
                FormattedText currentItem = ProcessToken(ref previousFormattedText, tokenizer, hasOpenTag, parsersData, ref shouldProduceNewLine);
                
                if (!string.IsNullOrEmpty(currentItem.HtmlTag) &&
                    currentItem.HtmlTag.Length >= 1 && shouldProduceNewLine)
                {
                    currentLine = new TextLine();
                    textBlock.Lines.Add(currentLine);
                    if (currentItem.HtmlTag.Length >= 2)
                    {
                        currentItem.HtmlTag = currentItem.HtmlTag.TrimEnd(' ').Trim('/');
                    }

                    shouldProduceNewLine = (!string.IsNullOrEmpty(currentItem.text) && currentItem.text.Trim().Length == 0);
                }                
                
                currentLine.List.Add(currentItem);
                previousFormattedText = new FormattedText(currentItem);
            }

            return textBlock;
        }

        /// <summary>
        /// Parse single HTML tag and apply settings
        /// </summary>
        /// <param name="currentFormattedText"></param>
        /// <param name="prevText"></param>
        /// <param name="htmlTag"></param>
        /// <param name="parserData"></param>
        /// <param name="shouldProduceNewLine"></param>
        /// <param name="text"></param>
        public static bool ApplyHTMLSettingsFromTag(ref FormattedText currentFormattedText,
                                                    FormattedText prevText,
                                                    string htmlTag,
                                                    TinyHTMLParsersData parserData,
                                                    ref bool shouldProduceNewLine,
                                                    ref string text)
        {            
            if (string.IsNullOrEmpty(htmlTag))
            {
                return true;
            }          

            htmlTag = htmlTag.Trim('<', '>');
            bool isClosingTag = htmlTag.StartsWith("/");
            currentFormattedText.IsClosingTag = isClosingTag;
            if (isClosingTag)
            {
                htmlTag = htmlTag.TrimStart('/');
            }

            string lowerHtmlTag = htmlTag.ToLower();
            bool isKnowCommand = true;            
            if (lowerHtmlTag == "i" || lowerHtmlTag == "em")
            {
                currentFormattedText.FontStyle = ProcessFontStyle(currentFormattedText.FontStyle, FontStyle.Italic, isClosingTag);
            }
            else if (lowerHtmlTag == "b" || lowerHtmlTag == "strong")
            {
                currentFormattedText.FontStyle = ProcessFontStyle(currentFormattedText.FontStyle, FontStyle.Bold, isClosingTag);
            }
            else if (lowerHtmlTag == "u")
            {
                currentFormattedText.FontStyle = ProcessFontStyle(currentFormattedText.FontStyle, FontStyle.Underline, isClosingTag);
            }
            else if (lowerHtmlTag.StartsWith("color") && htmlTag.Length > 6)
            {
                currentFormattedText.FontColor = ParseColor(htmlTag.Substring(6), currentFormattedText.FontColor);
            }
            else if (lowerHtmlTag.StartsWith("size=") || lowerHtmlTag.StartsWith("size ="))
            {
                currentFormattedText.FontSize = ParseSize(htmlTag, prevText.FontSize);
            }
            else if ((lowerHtmlTag.StartsWith("font=") || lowerHtmlTag.StartsWith("font =")) && htmlTag.Length > 5)
            {
                currentFormattedText.FontName = ParseFont(htmlTag.Substring(5));
            }
            else if (lowerHtmlTag == "strike")
            {
                currentFormattedText.FontStyle = ProcessFontStyle(currentFormattedText.FontStyle, FontStyle.Strikeout, isClosingTag);
            }
            else if (lowerHtmlTag.StartsWith("bgcolor"))
            {
                currentFormattedText.BgColor = ProcessBgColor(lowerHtmlTag, currentFormattedText.BgColor, isClosingTag);
            }
            //lists
            else if (lowerHtmlTag == "ul")
            {
                ProcessListEntry(isClosingTag, FormattedText.HTMLLikeListType.List, parserData.lastListType, parserData.lastListNumberCount, currentFormattedText);
            }
            else if (lowerHtmlTag == "ol")
            {
                ProcessListEntry(isClosingTag, FormattedText.HTMLLikeListType.OrderedList, parserData.lastListType, parserData.lastListNumberCount, currentFormattedText);
            }
            else if (lowerHtmlTag == "li")
            {   
                int lastNumber = 0;
                shouldProduceNewLine = !shouldProduceNewLine && !isClosingTag;
                if (parserData.lastListNumberCount.Count > 0)
                {
                    lastNumber = parserData.lastListNumberCount.Pop();
                }

                if (parserData.lastListType.Count > 0)
                {
                    ProcessSingleListEntry(isClosingTag, ref lastNumber, currentFormattedText, parserData);
                }

                parserData.lastListNumberCount.Push(lastNumber);
            }
            //end lists
            else if (lowerHtmlTag == "html")
            {
            }
            else if (lowerHtmlTag == "br" || lowerHtmlTag == "br /" || lowerHtmlTag == "br/")
            {
                shouldProduceNewLine = !isClosingTag;
            }
            else if (lowerHtmlTag == "p")
            {
                if (isClosingTag)
                {
                    shouldProduceNewLine = false;
                }
                else
                {
                    if (!prevText.IsClosingTag &&
                        !prevText.StartNewLine)
                    {
                        shouldProduceNewLine = !shouldProduceNewLine;
                    }
                }
            }
            else if (lowerHtmlTag.StartsWith("img "))
            {
                string imageName = ParseAttribute(htmlTag, lowerHtmlTag, "src");
                string width = ParseAttribute(htmlTag, lowerHtmlTag, "width");
                string height = ParseAttribute(htmlTag, lowerHtmlTag, "height");

                SetImage(imageName, width, height, currentFormattedText);
            }
            else if (lowerHtmlTag.StartsWith("a"))
            {
                ProcessLink(currentFormattedText, htmlTag, lowerHtmlTag, isClosingTag, ref text);
            }
            else if (lowerHtmlTag.StartsWith("span"))
            {
                ProcessSpan(ref currentFormattedText, lowerHtmlTag, htmlTag, isClosingTag, parserData);
            }
            else
            {
                isKnowCommand = false;
            }

            //if (parserData.shouldClearProduceBullets)
            //{
            //    currentFormattedText.ShouldDisplayBullet = false;
            //    parserData.shouldClearProduceBullets = false;
            //}

            return isKnowCommand;
        }

        private static void ProcessSpan(ref FormattedText currentFormattedText,
                                        string lowerHtmlTag,
                                        string htmlTag,
                                        bool isClosingTag,
                                        TinyHTMLParsersData parserData)
        {            
            if (isClosingTag)
            {
                if (parserData.lastFormattedText.Count > 0)
                {
                    currentFormattedText = parserData.lastFormattedText.Pop();
                    currentFormattedText.ShouldDisplayBullet = false;
                }

                return;
            }
            
            parserData.lastFormattedText.Push(new FormattedText(currentFormattedText));
            string style = ParseAttribute(htmlTag, lowerHtmlTag, "style");
			string value = ProcessStyle(style, "color");
			if (!string.IsNullOrEmpty(value))
			{
                currentFormattedText.FontColor = ParseColor(value, currentFormattedText.FontColor);
			}

            value = ProcessStyle(style, "background-color");
            if (!string.IsNullOrEmpty(value))
            {
                currentFormattedText.BgColor = ParseColor(value, currentFormattedText.BgColor);
            }

            value = ProcessStyle(style, "font-family");
            if (!string.IsNullOrEmpty(value))
            {
                currentFormattedText.FontName = ParseFont(value);
            }

            value = ProcessStyle(style, "font-size");
            if (!string.IsNullOrEmpty(value))
            {
                currentFormattedText.FontSize = ParseNumber(value, currentFormattedText.FontSize);
            }            
        }

        private static string ProcessStyle(string style, string attribute)
        {
            string[] pairs = style.Split(';');
            foreach (string pair in pairs)
            {
                string[] values = pair.Split(':');
                if (values.Length != 2)
                {
                    continue;//unknown
                }

                if (attribute == values[0].Trim())
                {
                    return values[1].Trim();
                }
            }

            return string.Empty;//nothing found
        }

        private static void ProcessSingleListEntry(bool isCloseTag, ref int numberCount, FormattedText currentFormattedText, TinyHTMLParsersData parserData)
        {
            FormattedText.HTMLLikeListType listType = parserData.lastListType.Peek();
            currentFormattedText.ShouldDisplayBullet = !isCloseTag;
            if (listType != FormattedText.HTMLLikeListType.OrderedList)
            {
                return;
            }

            if (!isCloseTag)
            {
                ++numberCount;
            }
            
            currentFormattedText.Number = numberCount;
        }

		private static void ProcessListEntry(bool isCloseTag,
                                             FormattedText.HTMLLikeListType listType,
                                             Stack<FormattedText.HTMLLikeListType> lastListType,
                                             Stack<int> lastListNumber, 
                                             FormattedText currentFormattedText)
		{
            if (!isCloseTag)
            {
                currentFormattedText.ListType = listType;
                lastListType.Push(listType);                
                ++currentFormattedText.Offset;
                //format bullets
                currentFormattedText.BulletFontName = currentFormattedText.FontName;
                currentFormattedText.BulletFontStyle = currentFormattedText.FontStyle;
                currentFormattedText.BulletFontSize = currentFormattedText.FontSize;

                lastListNumber.Push(0);
            }
            else
            {
                if (lastListType.Count > 1)
                {
                    lastListType.Pop();
                    currentFormattedText.ListType = lastListType.Peek();
                }
                else
                {
                    currentFormattedText.ListType = FormattedText.HTMLLikeListType.None;
                }

                --currentFormattedText.Offset;
                if (lastListNumber.Count > 0)
                {
                    lastListNumber.Pop();
                }
            }
		}        

        private static void SetImage(string imageName, string width, string height, FormattedText currentFormattedText)
        {
            int widthRes = 0;
            int heightRes = 0;

            if (!string.IsNullOrEmpty(width))
            {
                int.TryParse(width, out widthRes);
            }

            if (!string.IsNullOrEmpty(height))
            {
                int.TryParse(height, out heightRes);
            }

            if (imageName.StartsWith("res:") || imageName.StartsWith("resource:"))
            {
                imageName = imageName.Replace("res:", "").Replace("resource:", "");
                SetImageFromRes(imageName.Replace("res:", ""), currentFormattedText, widthRes, heightRes);
            }
            else
            {
                SetImageFromDisk(imageName, currentFormattedText, widthRes, heightRes);
            }
        }

        private static void SetImageFromRes(string imageName, FormattedText currentFormattedText, int width, int height)
        {
            if (currentFormattedText.Image != null)
            {
                return;
            }

            if (width > 0 && height > 0)
            {
                currentFormattedText.Image = new Bitmap(GetImageFromResource(imageName), width, height);
            }
            else
            {
                currentFormattedText.Image = GetImageFromResource(imageName);
            }
        }

        private static Image GetImageFromResource(string resourceName)
        {
            foreach (Telerik.WinControls.RadTypeResolver.LoadedAssembly assembly in RadTypeResolver.Instance.LoadedAssemblies)
            {
				using (Stream stream = assembly.assembly.GetManifestResourceStream(resourceName))
				{
                    if (stream != null)
                    {
                        return new Bitmap(Image.FromStream(stream));
                    }
				}
            }

            return null;
        }      

		private static void SetImageFromDisk(string imageName, FormattedText currentFormattedText, int width, int height)
		{
            if (currentFormattedText.Image != null)
            {
                return;
            }

            if (imageName.StartsWith("~"))
            {
                imageName = imageName.Replace("~", System.Windows.Forms.Application.StartupPath);
            }
          
			if (File.Exists(imageName))
			{
                if (width > 0 && height > 0)
				{
                    currentFormattedText.Image = new Bitmap(Image.FromFile(imageName), width, height);
				}
				else
				{
					currentFormattedText.Image = Image.FromFile(imageName);
				}
			}
			else
			{
                if (width > 0 && height > 0)
                {
                    currentFormattedText.Image = new Bitmap(width, height);
                }
			}
		}   

        /// <summary>
        /// process single token from Html string
        /// </summary>
        /// <param name="prevItem"></param>
        /// <param name="tokenizer"></param>
        /// <param name="hasOpenTag"></param>
        /// <param name="parserData"></param>
        /// <param name="shouldProduceNewLine"></param>
        /// <returns>a FormattedText object</returns>
        private static FormattedText ProcessToken(ref FormattedText prevItem,
                                                  StringTokenizer tokenizer,
                                                  bool hasOpenTag, 
                                                  TinyHTMLParsersData parserData,
                                                  ref bool shouldProduceNewLine)
        {
            string currentToken = tokenizer.NextToken(); //text block
            StringTokenizer currentCommand = new StringTokenizer(currentToken, ">");
            bool hasCloseTag = currentToken.IndexOf(">") > -1;
            string htmlTag = currentCommand.NextToken();
            string text = currentCommand.NextToken();
            FormattedText item = new FormattedText(prevItem);
            
            if (!hasOpenTag || !hasCloseTag)
            {
                item.text = htmlTag; //only text without htmlTag
                item.HtmlTag = string.Empty;
            }
            else
            {
                bool isKnowCommand = ApplyHTMLSettingsFromTag(ref item, prevItem, htmlTag, parserData, ref shouldProduceNewLine, ref text);
                if (isKnowCommand)
                {
                    item.text = text;
                }
                else
                {
                    item.text = htmlTag + text;
                }

                item.HtmlTag = htmlTag;
            }

            if (!string.IsNullOrEmpty(item.text))
            {
                item.text = item.text.Replace("&lt;", "<").Replace("&gt;", ">");
            }

            item.StartNewLine = shouldProduceNewLine;
            return item;
        }

        static private FontStyle oldFontStyle;
        static private Color oldFontColor;

        static void ProcessLink(FormattedText textBlock, string htmlTag, string lowerHtmlTag, bool isCloseTag, ref string text)
		{
            const string Href = " href";
            if (isCloseTag)
            {
                Debug.Assert(textBlock != null, "TextBlock is null");
                textBlock.Link = string.Empty;
                textBlock.FontColor = oldFontColor;
                textBlock.FontStyle = oldFontStyle;
                return;
            }

            int hrefPossition = lowerHtmlTag.IndexOf(Href);
            if (hrefPossition == -1)
            {
                textBlock.Link = string.Empty;
                textBlock.FontColor = oldFontColor;
                textBlock.FontStyle = oldFontStyle;
                return;
            }

            int equalPossition = lowerHtmlTag.IndexOf('=', hrefPossition + Href.Length);
            if (equalPossition == -1)
            {
                textBlock.Link = string.Empty;
                textBlock.FontColor = oldFontColor;
                textBlock.FontStyle = oldFontStyle;
                return;            
            }

            string refText = htmlTag.Substring(equalPossition + 1).Trim();
            textBlock.Link = refText;
            oldFontColor = textBlock.FontColor;
            oldFontStyle = textBlock.FontStyle;
            textBlock.FontStyle |= FontStyle.Underline;
            textBlock.FontColor = Color.Navy;
            while(text.Contains(" "))
            {
                text = text.Replace(' ', '_');
            }

            return;
		}

        private static Color? ProcessBgColor(string tag, Color? oldColor, bool isCloseTag)
        {
            if (isCloseTag)
            {
                return null;
            }

            if (tag.Length <= 8)
            {
                return null;
            }
            
            tag = tag.Substring(8);  
            return ParseColor(tag, oldColor);
        }     

        /// <summary>
        /// Handles &lt;u&gt;&lt;i&gt;&lt;b&gt; tags
        /// </summary>
        /// <param name="currentStyle"></param>
        /// <param name="newStyle"></param>
        /// <param name="removeNewStyle"></param>
        /// <returns></returns>
        private static FontStyle ProcessFontStyle(FontStyle currentStyle, FontStyle newStyle, bool removeNewStyle)
        {
            if (removeNewStyle)
            {
                return currentStyle & ~newStyle;
            }
            else
            {
                return currentStyle | newStyle;
            }
        }

        static System.ComponentModel.TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(Color));
        /// <summary>
        /// Handles &lt;color=value&gt;
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="oldColor"></param>        
        /// <returns></returns>        
        private static Color ParseColor(string tag, Color? oldColor)
        {
            Color result = Control.DefaultForeColor;
            if (oldColor.HasValue)
            {
                result = oldColor.Value;
            }

            try
            {
                result = (Color)typeConverter.ConvertFromString(null, CultureInfo.InvariantCulture, tag);
            }
            catch (Exception)
            {
                //failed to convert color
            }  
          
            return result;
        }



        static IFormatProvider formatProvider = NumberFormatInfo.InvariantInfo;
        /// <summary>
        /// Handles &lt;size=[+|-] valie&gt;
        /// </summary>
        /// <param name="htmlTag"></param>
        /// <param name="startValue"></param>
        /// <returns></returns>
        private static float ParseSize(string htmlTag, float startValue)
        {
            float num;
            int startIndex = 5;
            if (htmlTag.Length > 5 && (htmlTag[5] == '+' || htmlTag[5] == '-'))
            {
                startIndex = 6;
            }

            if (!float.TryParse(htmlTag.Substring(startIndex), NumberStyles.Any, formatProvider, out num) || (num <= 0f))
            {
                return startValue;
            }

            if (startIndex != 6)
            {
                return num;
            }

            return startValue + ((htmlTag.Length > 5 && (htmlTag[5] == '-')) ? -num : num);
        }

        /// <summary>
        /// Handles &lt;font=value&gt;
        /// </summary>
        /// <param name="htmlTag"></param>
        /// <returns></returns>
        private static string ParseFont(string htmlTag)
        {
            return htmlTag.Trim('"');
        }

        private static string ParseAttribute(string tag, string lowerTag, string attribute)
        {
            int pos = lowerTag.IndexOf(attribute);
            if (pos == -1)
            {
                return string.Empty;
            }

            pos = tag.IndexOf("=", pos + 1);
            if (pos == -1)
            {
                return string.Empty;
            }

            int pos1 = tag.IndexOfAny("\"'".ToCharArray(), pos);
            int pos2 = tag.IndexOfAny("\"'".ToCharArray(), pos1 + 1);
            if (pos1 == -1)
            {
                pos1 = tag.IndexOfAny(" =".ToCharArray(), pos);
                pos2 = tag.IndexOfAny(" >".ToCharArray(), pos1 + 1);
                if (pos2 == -1)
                {
                    pos2 = tag.Length - 1;
                }
            }

            string value = tag.Substring(pos1 + 1, pos2 - pos1).Trim(' ', '/', '"');
            return value;
        }

        static private float ParseNumber(string tag, float oldValue)
        {
            const float coef = 1.5f;
            string[] adjacent = new string[]{
                "xx-small",//1/1.5/1.5/1.5
				"x-small",//1/1.5/1.5
                "smaller",
				"small",//1/1.5
				"medium",//8
				"large",//
                "larger",
				"x-large",
				"xx-large",				
			};

            float baseCoef = oldValue / coef / coef / coef;
            //remove PT from font size 
            if (tag.EndsWith("pt") && tag.Length > 2)
            {
                tag = tag.Substring(0, tag.Length - 2);
            }

            for (int i = 0; i < adjacent.Length; ++i)
            {
                if (tag == adjacent[i])
                {
                    return baseCoef;
                }

                baseCoef *= coef;
            }

            float num = 0;
            if (!float.TryParse(tag, NumberStyles.Any, (IFormatProvider)NumberFormatInfo.InvariantInfo, out num) || (num <= 0f))
            {
                return oldValue;
            }

            return num;
        }
    }
}

