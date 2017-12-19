/******************************************************************************
 * 接口名称:IOperation                                                        *
 * ---------------------------------------------------------------------------*
 * 功能描述:                                                                  *
 *     进入操作栈中能够执行Redo()和Undo()操作的动作接口，某一类型(class)要想  *
 *     进入操作栈，必须实现IOperation接口                                     *
 * ---------------------------------------------------------------------------*
 * 变更记录：                                                                 *
 * 1、时间    :2007-11-29                                                     * 
 *    程序员  :冯德财                                                         *
 *    动作    :创建                                                           *
 ******************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace CodeCell.Bricks.RedoUndo
{
    public interface IOperation
    {
        /// <summary>
        /// 操作名称，可以作为在Redo和Undo操作按钮上设置提示文本等用途
        /// </summary>
        string Name { get;set;}
        /// <summary>
        /// 执行Do动作(第一次执行该操作)
        /// </summary>
        void Do();
        /// <summary>
        /// 执行重做动作
        /// </summary>
        void Redo();
        /// <summary>
        /// 执行撤销动作
        /// </summary>
        void Undo();
        /// <summary>
        /// 关联的数据，用于删除时标识操作
        /// </summary>
        object Data { get;set;}
    }
}
