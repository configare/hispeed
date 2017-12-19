/******************************************************************************
 * 枚举名称:enumStackStatus                                                   *
 * ---------------------------------------------------------------------------*
 * 功能描述:                                                                  *
 *     标记操作栈当前状态：                                                   *
 *     1、HasUndoOperation | HasRedoOperation //有Redo和Undo动作              *
 *     2、HasUndoOperation                    //有Undo动作                    *
 *     3、HasRedoOperation                    //有Redo动作                    *
 *     4、IsOverCapacity                      //已经超过栈的最大深度          *
 *     5、NoneOperation                       //无Redo\Undo动作               *
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
    [Flags]
    public enum dssStackStatus
    {
        HasUndoOperation = 0x1,
        HasRedoOperation = 0x2,
        IsOverCapacity   = 0x4,
        NoneOperation    = 0x8
    }
}
