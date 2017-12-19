using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace Telerik.WinControls
{
    /// <summary>
    /// Defines helper methods for manipulating assembly resources.
    /// </summary>
    public static class ResourceHelper
    {
        /// <summary>
        /// Creates a new Image instance from the specified embedded resource for the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public static Bitmap ImageFromResource(Type type, string resourceName)
        {
            Assembly asm = type.Assembly;
            Bitmap bmp = null;
            Stream stream = null;

            try
            {
                stream = asm.GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    Bitmap temp = (Bitmap)Image.FromStream(stream);
                    bmp = new Bitmap(temp);
                    temp.Dispose();
                }

                return bmp;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load embedded image for resource " + resourceName + "\r\nException was: " + ex.Message);
                return null;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }

        /// <summary>
        /// Creates a new Cursor instance from the specified embedded resource for the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public static Cursor CursorFromResource(Type type, string resourceName)
        {
            Assembly asm = type.Assembly;
            Cursor cursor = null;
            Stream stream = null;

            try
            {
                stream = asm.GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    cursor = new Cursor(stream);
                }

                return cursor;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load embedded image for resource " + resourceName + "\r\nException was: " + ex.Message);
                return null;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }
    }
}
