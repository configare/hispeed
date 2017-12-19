using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.UI.AddIn.AddInMgr
{
    public class GetFileIcoHelper
    {
     [DllImport("shell32.dll", EntryPoint="SHGetFileInfo")]
     static extern int SHGetFileInfo(string pszPath, uint dwFileAttributes, ref   SHFILEINFO psfi, uint cbFileInfo, uint uFlags);
     struct SHFILEINFO
     {
         public IntPtr hIcon;
         public int iIcon;
         public uint dwAttributes;
         public char szDisplayName;
         public char szTypeName;
     }

     /// <summary>
     /// 从文件扩展名得到文件关联图标
     /// </summary>
     /// <param name="fileName">文件名或文件扩展名</param>
     /// <param name="smallIcon">是否是获取小图标，否则是大图标</param>
     /// <returns>图标</returns>
     static public Icon GetFileIcon(string fileName, bool smallIcon)
     {
         SHFILEINFO fi = new SHFILEINFO();
         Icon ic = null;
         //SHGFI_ICON + SHGFI_USEFILEATTRIBUTES + SmallIcon   
         int iTotal = (int)SHGetFileInfo(fileName, 100, ref fi, 0, (uint)(smallIcon ? 273 : 272));
         if (iTotal > 0)
         {
             ic = Icon.FromHandle(fi.hIcon);
         }
         return ic;
     }

     static public void SaveFileIcon(string fileName, Icon icon)
     {
         icon.ToBitmap().Save(fileName);
         //FileStream fs ;
         //fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
         //icon.Save(fs);
         //fs.Close();
     }
    }
}
