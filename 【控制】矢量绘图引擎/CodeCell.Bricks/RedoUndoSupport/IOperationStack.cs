/******************************************************************************
 * 接口名称:IOperationStack                                                   *
 *          可以由任何类型实现该接口，本程序集提供一个默认实现OperationStack  *
 * ---------------------------------------------------------------------------*
 * 功能描述:                                                                  *
 *     操作栈接口，实现该接口的类完成以下功能：                               *
 *     1、管理需要Undo、Redo动作的操作(IOperation)                            *
 *     2、按Stack操作方式，调度每一个IOperation执行Redo、Undo                 *
 *     3、使用事件形式通知外部环境操作栈的变化                                *
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
    public interface IOperationStack
    {
        bool Enabled { get; set; }
        /// <summary>
        /// 栈的深度
        /// </summary>
        int Capacity { get;set;}
        /// <summary>
        /// 将某一操作加入操作栈，并执行其Do()动作
        /// </summary>
        /// <param name="mOperation">实现IOperation接口的类型实例</param>
        /// <retruns>是否加入成功，如果不成功可能是栈超过最大深度</retruns>>
        bool Do(IOperation mOperation);
        /// <summary>
        /// 对刚刚执行了撤销动作的操作执行重做动作
        /// </summary>
        void Redo();
        /// <summary>
        /// 对当前操作(执行了Do动作或Redo动作)执行撤销动作
        /// </summary>
        void Undo();
        /// <summary>
        /// 重置操作栈，执行完后操作栈将被置空
        /// </summary>
        void Reset();
        /// <summary>
        /// 从操作栈中移除掉某个操作
        /// </summary>
        /// <param name="mOperation">操作的引用</param>
        void Remove(IOperation mOperation);
        /// <summary>
        /// 根据关联数据删除操作
        /// </summary>
        /// <param name="data"></param>
        void Remove(object data);
        void ClearUndoedOperations();
        /// <summary>
        /// 通知外部外环操作栈已发生变化(例如：有可以执行Redo动作的操作等等)
        /// </summary>
        StackStatusChangedHandler OnStackStatusChanged{get;set;}
        /// <summary>
        /// 刷新状态
        /// </summary>
        void UpdateStatus();
        /// <summary>
        /// 状态
        /// </summary>
        dssStackStatus Status { get;}
    }
}
