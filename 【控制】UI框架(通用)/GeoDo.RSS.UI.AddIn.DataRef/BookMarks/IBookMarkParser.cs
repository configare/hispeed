using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.UI.AddIn.DataRef.BookMarks
{
    public interface IBookMarkParser
    {
        BookMarkGroup[] BookMarkGroups { get; }
        Dictionary<string, CoordEnvelope> BookMarks { get; }
        /// <summary>
        /// 在xml文件中新增书签节点
        /// 如groupName已存在，则在指定的group下新增；
        /// 如groupName不存在，则先新增group节点
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="bookMarkName"></param>
        /// <param name="envelope"></param>
        void AddBookMarkElement(string groupName, string bookMarkName, CoordEnvelope envelope);
        /// <summary>
        /// 在xml文件中新增group节点
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="bookMarkName"></param>
        /// <param name="envelope"></param>
        void AddGroupElement(string groupName, string bookMarkName, CoordEnvelope envelope);
        /// <summary>
        /// 删除指定的bookMark节点
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="bookMarkName"></param> 
        void DeleteBookMarkElement(string groupName, string bookMarkName);
        void DeleteBookMarkElement(string bookMarkName);
        /// <summary>
        /// 删除所有的书签节点
        /// </summary>
        void DeleteAllBookMarkElements();
    }
}
