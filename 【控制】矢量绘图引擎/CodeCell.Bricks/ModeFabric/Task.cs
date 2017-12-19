using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using CodeCell.Bricks.Runtime;

namespace CodeCell.Bricks.ModelFabric
{
    public sealed class Task:ITask
    {
        private string _name = null;
        private List<IAction> _actions = null;
        private IVarProvider _varProvider = null;
        private ILog _log = null;
        private IProgressTracker _tracker = null;
        private List<IAction> _executedActions = new List<IAction>();
        private IContextMessage _contextMessage = new ContextMessage();
        private ArgAutoBingdingEnvironment _argAutoBingdingEnvironment = null;
        private const string errBindingInfoLost2 = "没有找到动作\"{0}:{1}\"的参数绑定配置信息,\n任务\"{0}\"执行失败。";
        private const string errVarProviderLost2 = "绑定参数\"{0}:{1}\"失败,\n没有变量提供者。";
        private const string errRefActionLost2 = "绑定参数\"{0}:{1}\"失败,没有\n找到引用的Action或者引用的Action还未执行完毕。";
        private const string errBindingAttributeLost4 = "绑定参数\"{0}:{1}\"失败,\n参数\"{2}:{3}\"没有定义BindingAttribute属性。";
        private const string errArgTypeNotMatch4 =  "绑定参数\"{0}:{1}\"失败,\n参数\"{0}:{1}与参数\"{2}:{3}\"语义类型不兼容。";
        private const string errReadArgValueFail4 =  "绑定参数\"{0}:{1}\"失败,\n读取参数\"{2}:{3}\"失败。";
        private const string errBindArgFaileExceptionAtNextLog2 =  "绑定参数\"{0}:{1}\"失败,\n详细错误信息参见下一条日志。";
        private const string errExecuteUnknowException1 = "任务\"{0}\"执行过程中发生错误,\n详细错误见下一条日志:";
        private const string errActionIsEmpty1 = "无法启动任务\"{0}\",\nAction列表为空。";
        private const string errGetVarFaildOrResQueueIsEmpty = "绑定参数\"{0}:{1}\"失败,\n获取的变量值为空,可能是待处理队列为空。";
        private const string errBeginAction = "开始执行:{0}...";
        private const string errEndAction = "结束:{0}。";
        //是否循环执行
        private bool _isLoopExecute = false;
        //同一类型的任务是否可以并发
        private bool _isCanIntercurrent = true;
        //唯一标识
        private int _id = 0;
        //静态ID,每个Task在同一进程内都用唯一的ID
        private static int maxId = 0;

        public Task()
        {
            _id = maxId;
            maxId++;
        }

        internal void SetArgAutoBingdingEnvironment(ArgAutoBingdingEnvironment argAutoBingdingEnvironment)
        {
            _argAutoBingdingEnvironment = argAutoBingdingEnvironment;
        }

        /// <summary>
        /// 唯一ID
        /// </summary>
        internal int Id
        {
            get { return _id; }
        }

        /// <summary>
        /// 同一类型的任务是否允许并发执行
        /// </summary>
        internal bool IsCanIntercurrent
        {
            get { return _isCanIntercurrent; }
            set { _isCanIntercurrent = value; }
        }

        /// <summary>
        /// 是否循环执行
        /// </summary>
        internal bool IsLoopExecute
        {
            set { _isLoopExecute = value; }
            get { return _isLoopExecute; }
        }

        #region ITask 成员

        public bool VarIsEnough()
        {
            return false;
        }

        public string Name
        {
            get { return string.IsNullOrEmpty(_name) ? ToString() : _name; }
            set { _name = value; }
        }

        public void Execute()
        {
            //执行的条件是否具备
            if (!ConditionIsEnough())
                return;
            try
            {
                bool isOK =false ;
                foreach (IAction act in _actions)
                {
                    if (_tracker != null)
                        _tracker.Tracking(string.Format(errBeginAction, act.Name));
                    try
                    {
                        //为待执行Action绑定参数
                        if (!BindingArgsForAction(act))
                            return;
                        //执行Action
                        isOK = act.Do(_contextMessage);
                        //资源释放，变量不清空
                        (act as ActionBase).Sleep(_contextMessage);
                        //只要其中一个执行失败,那么整个任务结束
                        if (!isOK)
                        {
                            LogWarning(_contextMessage.GetErrorInfoString());
                            return;
                        }
                        //加入到已执行完的Action列表
                        _executedActions.Add(act);
                    }
                    finally 
                    {
                        if (_tracker != null)
                            _tracker.Tracking(string.Format(errEndAction, act.Name));
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(string.Format(errExecuteUnknowException1, Name));
                LogException(ex);
            }
            finally
            {
                _contextMessage.Reset();
            }
        }

        /// <summary>
        /// 为待执行Action绑定参数
        /// </summary>
        /// <param name="act">待执行Action</param>
        /// <returns>所有参数都绑定成功</returns>
        private bool BindingArgsForAction(IAction act)
        {
            //获取参数绑定配置对象(如何绑定是在该对象内定义的)
            ActionArgCollection args = _argAutoBingdingEnvironment.GetActionArgCollection((act as ActionBase).Id);
            if (args == null)
            {
                LogError(string.Format(errBindingInfoLost2, act.Name, Name));
                return false;
            }
            //Action的具体类型(例如:ActionSpecialRegionChecker)
            Type actType = act.GetType();//eg:TypeOf(ActionQualityChecker)
            bool isOK = false;
            //逐一绑定
            foreach (ActionArg arg in args.ActionArgs)
            {
                //参数类型
                switch (arg.ArgType)
                {
                    //直接值
                    case enumArgType.Value:
                        isOK = BindValueToAction(arg.Name, arg.Value, act, actType);
                        if (!isOK)
                            return false;
                        break;
                    //引用其它Action的参数(属性)
                    case enumArgType.Ref:
                        isOK = BindRefValueToAction(arg, act, actType);
                        if (!isOK)
                            return false;
                        break;
                    //变量(可以理解为输入的参数)
                    case enumArgType.Var:
                        isOK = BindVarValueToAction(arg, act, actType);
                        if (!isOK)
                            return false;
                        break;
                    default:
                        break;
                }
            }
            return true;
        }

        /// <summary>
        /// 变量绑定
        /// </summary>
        /// <param name="arg">参数绑定定义对象</param>
        /// <param name="act">待绑定Action</param>
        /// <param name="actType">待绑定Action的具体类型</param>
        /// <returns>是否绑定成功</returns>
        private bool BindVarValueToAction(ActionArg arg, IAction act, Type actType)
        {
            //变量提供者对象为空
            if (_varProvider == null)
            {
                LogError(string.Format(errVarProviderLost2, act.Name, arg.Name));
                return false;
            }
            //获取变量
            object value = _varProvider.GetVarValue(arg.VarType, arg.RefName);
            //临时的
            if (arg.RefName.ToUpper() == "INPUTFILENAME")
                LogInfo("处理:" + Path.GetFileName(value != null ? value.ToString() : string.Empty));
            //将变量帮定到Action的参数Arg.Name
            return BindValueToAction(arg.Name, value, act, actType);
        }

        /// <summary>
        /// 绑定引用到Action
        /// </summary>
        /// <param name="arg">参数绑定定义对象</param>
        /// <param name="act">待绑定Action</param>
        /// <param name="actType">待绑定Action的具体类型</param>
        /// <returns>是否绑定成功</returns>
        private bool BindRefValueToAction(ActionArg arg, IAction act, Type actType)
        {
            try
            {
                //从已执行队列中发现参数来源Action
                IAction resAction = FindAction(arg.RefActionId);
                if (resAction == null)
                {
                    if ((act as ActionBase).Id != arg.RefActionId )
                    {
                        LogError(string.Format(errRefActionLost2, act.Name, arg.Name));
                        return false;
                    }
                    else
                    {
                        resAction = act;
                    }
                }
                //获取源Action的BingdingAttribute 元数据                                                        Filename         QualityChecker
                BindingAttribute resBindingAtt = ArgBindingHelper.GetBingdingAttribute(arg.RefName, resAction);
                if (resBindingAtt == null)
                {
                    LogError(string.Format(errBindingAttributeLost4, act.Name, arg.Name, resAction.Name, arg.RefName));
                    return false;
                }
                //获取目标Action的BingdingAttribute                                                                 Filename    SpecialRegionChecker
                BindingAttribute desBindingAtt = ArgBindingHelper.GetBingdingAttribute(arg.Name, act);
                if (desBindingAtt == null)
                {
                    LogError(string.Format(errBindingAttributeLost4, act.Name, arg.Name, act.Name, arg.Name));
                    return false;
                }
                //绑定参数语义类型是否兼容,泛型关系暂未做处理
                if (!resBindingAtt.SemanticType.Equals(desBindingAtt.SemanticType))
                {
                    LogError(string.Format(errArgTypeNotMatch4, act.Name, arg.Name, resAction.Name, arg.RefName));
                }
                //从源Action获取绑定值                               Action1:QualityChecker.Filename            
                object refValue = resAction.GetType().InvokeMember(arg.RefName, BindingFlags.GetProperty, null, resAction, null);
                //将获取到的绑定值绑定到目标Action的参数arg.Name        Action2:SpecialRegionChecker.Filename
                actType.InvokeMember(arg.Name, BindingFlags.SetProperty, null, act, new object[] { refValue });
                return true;
            }
            catch (Exception ex)
            {
                LogError(string.Format(errBindArgFaileExceptionAtNextLog2, act.Name, arg.Name));
                LogException(ex);
                return false;
            }
        }

        /// <summary>
        /// 从已执行队列获取ID为指定ID的Action
        /// </summary>
        /// <param name="actionId"></param>
        /// <returns></returns>
        private IAction FindAction(int actionId)
        {
            if (_actions == null || _actions.Count == 0)
                return null;
            foreach (IAction act in _executedActions)
                if ((act as ActionBase).Id == actionId)
                    return act;
            return null;
        }

        /// <summary>
        /// 绑定具体值到Action的属性PrpoertyName
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="value">绑定值</param>
        /// <param name="act">目标Action</param>
        /// <param name="actType">目标Action的具体类型</param>
        /// <returns></returns>
        private bool BindValueToAction(string propertyName,object value, IAction act, Type actType)
        {
            actType.InvokeMember(propertyName, BindingFlags.SetProperty, null, act, new object[] { value});
            return true;
        }

        /// <summary>
        /// 检查启动任务的基本条件是否具备
        /// </summary>
        /// <returns></returns>
        private bool ConditionIsEnough()
        {
            if (_actions == null || _actions.Count == 0)
            {
                LogError(string.Format(errActionIsEmpty1, Name));
                return false;
            }
            return true;
        }

        public void SetLog(ILog log)
        {
            _log = log;
            if (_actions != null && _actions.Count > 0)
                foreach (ActionBase act in _actions)
                    act.SetLog(_log);
        }

        public void SetTracker(IProgressTracker Tracker)
        {
            _tracker = Tracker;
            if (_actions != null && _actions.Count > 0)
                foreach (ActionBase act in _actions)
                    act.SetTracker(_tracker);
        }

        public void SetVarProvider(IVarProvider varProvider)
        {
            _varProvider = varProvider;
        }

        public void Reset()
        {
            if (_actions == null)
                return;
            foreach (IAction action in _actions)
                action.Reset();
            _executedActions.Clear();
        }

        public void AddAction(IAction action)
        {
            if (action == null)
                return;
            if (_actions == null)
                _actions = new List<IAction>();
            _actions.Add(action);
        }

        private void LogException(Exception ex)
        {
            if (_log != null)
            {
                //ex = new Exception("任务:" + Name + "\n" + ex.Message,ex.InnerException);
                _log.WriterException(ex);
            }
        }

        private void LogWarning(string info)
        {
            if (_log != null)
                _log.WriterWarning("任务:" + Name + "\n" + info);
        }

        private void LogInfo(string info)
        {
            if (_log != null)
                _log.WriterInfo("任务:" + Name + "\n" + info);
        }

        private void LogError(string errorInfo)
        {
            if (_log != null)
                _log.WriterError("任务:"+Name+"\n" +errorInfo);
        }

        #endregion
    }
}
