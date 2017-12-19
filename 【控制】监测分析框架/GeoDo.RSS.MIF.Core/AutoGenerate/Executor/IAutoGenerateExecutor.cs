using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public interface IAutoGenerateExecutor
    {
        /// <summary>
        /// 上下文环境
        /// </summary>
        IContextEnvironment ContextEnvironment { get; }
        /// <summary>
        /// 获取第一次需要赋值的参数
        /// </summary>
        /// <returns></returns>
        string[] GetNeedFirstSettedArguments();
        string[] GetNeedFirstSettedArguments(string beginSubProduct);
        /// <summary>
        /// 执行自动产品生成
        /// </summary>
        /// <param name="product">监测产品</param>
        /// <param name="contextMessage">上下文消息</param>
        /// <param name="argumentMissProcessor">空参数处理器</param>
        void Execute(IContextMessage contextMessage,IArgumentMissProcessor argumentMissProcessor,string executeGroup,Action<int,string> processTracker);
        void Execute(string beginSubProduct, IContextMessage contextMessage, IArgumentMissProcessor argumentMissProcessor,string executeGroup,Action<int, string> processTracker);
    }
}
