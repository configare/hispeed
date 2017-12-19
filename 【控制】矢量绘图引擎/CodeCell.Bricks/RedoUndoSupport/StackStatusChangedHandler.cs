/******************************************************************************
 * 委托名称:StatckStatusChangedHandler                                        *
 * ---------------------------------------------------------------------------*
 * 功能描述:当操作栈状态发生变化时通知外部环境                                *
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
    /// <summary>
    /// 操作栈状态枚举(该枚举有标记类型，可以使用位域操作)
    /// </summary>
    /// <param name="mStackStatus"></param>
    public delegate void StackStatusChangedHandler(dssStackStatus mStackStatus);
}
