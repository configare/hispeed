using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.Runtime;

namespace CodeCell.Bricks.ModelFabric
{
    public abstract class ActionBase:IAction
    {
        protected ILog _log = null;
        protected IProgressTracker _tracker = null;
        protected string _name = null;
        private int _id = -1;
        private Guid _guid = Guid.NewGuid();

        public ActionBase()
        { 
        }

        public ActionBase(string name)
            :this()
        {
            _name = name;
        }

        public Guid Guid
        {
            get { return _guid; }
        }

        internal int Id
        {
            get { return  _id; }
            set { _id = value; }
        }

        #region IAction 成员

        public string Name
        {
            get { return string.IsNullOrEmpty(_name) ? ToString() : _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Task执行Action的入口函数
        /// 注意：子类最好不要覆写该函数
        /// </summary>
        /// <param name="contextMessage"></param>
        /// <returns></returns>
        public bool Do(IContextMessage contextMessage)
        {
            try
            {
                if (!ConditionsIsEnough(contextMessage))
                    return false;
                return Execute(contextMessage);
            }
            finally 
            {
                Sleep(contextMessage);
            }
        }

        /// <summary>
        /// 让Action恢复到休眠状态
        /// 不ResetAction,但是需要释放已开辟的内存与非托管资源
        /// 留着属性信息，待后续的Action绑定使用
        /// </summary>
        /// <param name="contextMessage">上下文消息对象</param>
        internal protected abstract void Sleep(IContextMessage contextMessage);
       
        /// <summary>
        /// 判断Action执行的参数是否具备
        /// </summary>
        /// <param name="contextMessage">上下文消息对象</param>
        /// <returns>是否具备</returns>
        protected abstract bool ConditionsIsEnough(IContextMessage contextMessage);

        /// <summary>
        /// 具体执行业务逻辑的函数
        /// </summary>
        /// <param name="contextMessage">上下文消息对象</param>
        /// <returns>是否执行成功</returns>
        protected abstract bool Execute(IContextMessage contextMessage);

        /// <summary>
        /// 重置Action的内部状态
        /// </summary>
        public abstract void Reset();

        #endregion

        #region IDisposable 成员

        public abstract void Dispose();
      
        #endregion

        public void SetLog(ILog log)
        {
            _log = log;
        }

        public void SetTracker(IProgressTracker Tracker)
        {
            _tracker = Tracker;
        }
    }
}
