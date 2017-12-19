using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.Bricks.ModelFabric
{
    internal static class ScriptLoader
    {
        class LinkObject
        {
            public int ActionId = -1;
            public int RefActionId = -1;

            public LinkObject(int actionId, int refActionId)
            {
                ActionId = actionId;
                RefActionId = refActionId;
            }
        }

        public static void LoadFrom(string[] assemblyDirs, string scriptfilename, out List<ActionElement> elements, out List<ActionElementLink> links)
        {
            elements = null;
            links = null;
            using (TaskScriptParser parser = new TaskScriptParser(assemblyDirs))
            {
                ArgAutoBingdingEnvironment env = null;
                parser.FromTaskScriptFile(scriptfilename, out env);
                if (env == null || env.ActionArgSettings == null || env.ActionArgSettings.Length ==0)
                    throw new Exception("脚本文件\"" + scriptfilename+"\"为空。");
                Dictionary<int,ActionElement> eles = new Dictionary<int,ActionElement>();
                Dictionary<LinkObject, ActionElementLink> linkObjs = new Dictionary<LinkObject, ActionElementLink>();
                foreach (ActionArgCollection action in env.ActionArgSettings)
                {
                    ActionElement ele = GetActionElement(action);
                    eles.Add(action.Id, ele);
                    //
                    foreach (ActionArg arg in action.ActionArgs)
                    {
                        switch (arg.ArgType)
                        {
                            case enumArgType.Value:
                                break;
                            case enumArgType.Var:
                                break;
                            case enumArgType.Ref:
                                LinkObject lnkObj = GetLinkObject(linkObjs, action.Id, arg.RefActionId);
                                if (lnkObj != null) //添加一条函数映射关系
                                {
                                }
                                else
                                {
                                    ActionElement refActionElement = GetActionElement(arg.RefActionId, eles);
                                    ActionElementLink actionLnk = new ActionElementLink(refActionElement, ele);
                                    linkObjs.Add(new LinkObject(action.Id, arg.RefActionId), actionLnk);
                                }
                                break;
                        }
                    }
                }
                elements = new List<ActionElement>(eles.Values.ToArray());
                links = new List<ActionElementLink>(linkObjs.Values.ToArray());
            }
        }

        private static ActionElement GetActionElement(int actionId, Dictionary<int, ActionElement> eles)
        {
            foreach(int id in eles.Keys)
                if(id == actionId)
                    return eles[id];
            return null;
        }

        private static LinkObject GetLinkObject(Dictionary<LinkObject, ActionElementLink> linkObjs, int actionId, int refAciontId)
        {
            foreach (LinkObject obj in linkObjs.Keys)
            {
                if (obj.ActionId == actionId && obj.RefActionId == refAciontId)
                    return obj;
            }
            return null;
        }

        private static ActionElement GetActionElement(ActionArgCollection action)
        {
            ActionInfo info = new ActionInfo(action.AssemblyUrl, action.ClassName, action.ActionAttribute);
            return new ActionElement(info);
        }
    }
}
