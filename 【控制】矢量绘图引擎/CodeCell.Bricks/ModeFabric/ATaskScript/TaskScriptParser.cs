using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Runtime.Remoting;
using CodeCell.Bricks.Runtime;

namespace CodeCell.Bricks.ModelFabric
{
    internal class TaskScriptParser:IDisposable
    {
        private XmlNode _rootNode = null;
        private string[] _assembliesFindDirs = null;//程序集查找路径集

        public TaskScriptParser(string[] assemblyFindDirs)
        {
            _assembliesFindDirs = assemblyFindDirs;
        }

        public ITask FromTaskScriptFile(string xmlScriptFielanem, out ArgAutoBingdingEnvironment argAutoBingdingEnvironment)
        {
            argAutoBingdingEnvironment = null;
            try
            {
                OpenTaskScript(xmlScriptFielanem);
                return ParseTaskScript(out argAutoBingdingEnvironment);
            }
            catch(Exception ex)
            {
                Log.WriterError("TaskNodeHandler", "FromTaskScriptFile", ex.Message);
                return null;
            }
        }

        private void OpenTaskScript(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException("任务定义脚本文件不存在", filename);
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            _rootNode = doc.DocumentElement;
            if (_rootNode == null)
                throw new Exception("任务定义脚本[" + filename + "]错误！");
        }

        private ITask ParseTaskScript(out ArgAutoBingdingEnvironment argAutoBingdingEnvironment)
        {
            argAutoBingdingEnvironment = null;
            XmlNode taskNode = _rootNode.SelectSingleNode("Task");
            if (taskNode == null)
                return null;
            return new TaskNodeHandler(_assembliesFindDirs).XmlNodeToTasks(taskNode, out argAutoBingdingEnvironment);
        }

        #region IDisposable 成员

        public void Dispose()
        {
            
        }

        #endregion
    }

    internal class TaskNodeHandler
    {
        protected const string ActionArgPattern = @"^(?<ArgType>\S*):((?<RefActionId>\d+)|(?<VarType>\S*))@(?<RefName>\S*)$";
        private string[] _assembliesFindDirs = null;//dll

        public TaskNodeHandler(string[] assembliesFindDirs)
        {
            _assembliesFindDirs = assembliesFindDirs;
        }

        public ITask XmlNodeToTasks(XmlNode taskNode, out ArgAutoBingdingEnvironment argAutoBingdingEnvironment)
        {
            argAutoBingdingEnvironment = null;
            if (taskNode.Name.ToUpper() != "TASK" || taskNode.ChildNodes.Count == 0)
                return null;
            Task task = new Task();
            task.Name = XMLNodeHelper.NodeAtt2String(taskNode, "name");
            task.IsCanIntercurrent = XMLNodeHelper.NodeAtt2Bool(taskNode, "isCanIntercurrent", task.IsCanIntercurrent);
            List<ActionArgCollection> ActionArgCollections = new List<ActionArgCollection>();
            foreach (XmlNode child in taskNode.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Comment)
                    continue;
                if (child.Name.ToUpper() == "ACTION")
                {
                    ActionArgCollection actionArgCollection = ParseXMLNodeToAction(child, task);
                    if (actionArgCollection != null)
                        ActionArgCollections.Add(actionArgCollection);
                }
            }
            argAutoBingdingEnvironment = new ArgAutoBingdingEnvironment(ActionArgCollections.ToArray());
            return task;
        }

        private ActionArgCollection ParseXMLNodeToAction(XmlNode actionNode, ITask task)
        {
            int actionID = XMLNodeHelper.NodeAtt2Int(actionNode, "id", -1);
            if (actionID == -1)
                throw new Exception("Action的ID定义错误[" + actionNode.OuterXml + "]");
            string assemblyUrl = null,className = null;
            IAction action = ActivatorAction(actionNode,out assemblyUrl,out className);
            if((action as ActionBase)==null)
                throw new Exception("Action[" + action.Name + "]不是继承自ActionBase");
            (action as ActionBase).Id = actionID;
            if (action == null)
                throw new Exception("任务定义脚本处理类[" + actionNode.OuterXml + "]不存在");
            task.AddAction(action);
            List<ActionArg> actionArgs = new List<ActionArg>();
            foreach (XmlNode child in actionNode.ChildNodes)
            {
                ActionArg actionArg = ParseXMLNodeToActionArg(child, actionID);
                if (actionArg != null)
                    actionArgs.Add(actionArg);
            }
            ActionArgCollection actionArgCollection =  new ActionArgCollection(actionID, actionArgs.ToArray());
            actionArgCollection.AssemblyUrl = assemblyUrl;
            actionArgCollection.ClassName = className;
            actionArgCollection.ActionAttribute = GetActionAttribute(action);
            return actionArgCollection;
        }

        private ActionAttribute GetActionAttribute(IAction action)
        {
            Type t = action.GetType();
            object[] atts = t.GetCustomAttributes(typeof(ActionAttribute), true);
            if (atts == null || atts.Length == 0)
            {
                return new ActionAttribute(action.Name, string.Empty);
            }
            else
            {
                return atts[0] as ActionAttribute;
            }
        }

        private IAction ActivatorAction(XmlNode actionNode, out string assemblyFilename, out string className)
        {
            assemblyFilename = null;
            className = null;
            string classString = XMLNodeHelper.NodeAtt2String(actionNode, "class");
            if (string.IsNullOrEmpty(classString))
                throw new Exception("Action未定义处理类[" + actionNode.OuterXml + "]");
            string[] dllImports = classString.Split(',');
            string dllName= "";
            //string actionActivator = "";
            if (dllImports == null || dllImports.Length < 2)
                className = classString;
            else
            {
                dllName = dllImports[0];
                className = dllImports[1];
            }
            object action = null;
            try
            {
                Assembly assembly = null;
                //string assemblyFilename = "";
                if (_assembliesFindDirs == null || _assembliesFindDirs.Length == 0)
                    assemblyFilename = dllName;
                else
                    foreach (string assemblieDir in _assembliesFindDirs)
                    {
                        assemblyFilename = Path.Combine(assemblieDir, dllName);
                        if (File.Exists(assemblyFilename))
                            break;
                    }
                assembly = Assembly.LoadFrom(assemblyFilename);
                Type actionType = assembly.GetType(className, false);
                if (actionType == null)
                    throw new Exception("未能从提供的dll[" + assemblyFilename + "]中找到定义的处理类" + className);
                action = Activator.CreateInstance(actionType);
                if (actionType.GetInterface("AgileMap.Bricks.ModelBuilder.IAction") == null)
                    throw new Exception("任务定义脚本处理类[" + className + "]不存在,或没有实现IAction");
            }
            catch
            {
                throw;
            }
            return action as IAction;
        }

        private ActionArg ParseXMLNodeToActionArg(XmlNode actionArgNode,int actionID)
        {
            ActionArg actionArg = new ActionArg(XMLNodeHelper.NodeAtt2String(actionArgNode, "name"));
            string argValue = XMLNodeHelper.NodeAtt2String(actionArgNode, "value");
            Regex regex = new Regex(ActionArgPattern, RegexOptions.Compiled|RegexOptions.IgnoreCase);//不区分大小写
            Match match = regex.Match(argValue);
            if (match.Success)
            {
                Group ArgType = match.Groups["ArgType"];
                Group VarType = match.Groups["VarType"];
                Group RefName = match.Groups["RefName"];
                Group RefActionId = match.Groups["RefActionId"];
                if (ArgType.Success)
                {
                    switch (ArgType.Value.ToUpper())
                    {
                        case "REF":
                            actionArg.ArgType = enumArgType.Ref;
                            actionArg.RefName = RefName.Value;
                            break;
                        case "VAR":
                            actionArg.ArgType = enumArgType.Var;
                            break;
                        default:
                            throw new Exception("未知的ActionArg.ArgType定义[" + ArgType.Value + "]");
                    }
                }
                if (RefActionId.Success)
                {
                    actionArg.RefActionId = int.Parse(RefActionId.Value);
                    actionArg.RefName = RefName.Value;
                }
                else if (VarType.Success)
                {
                    switch (VarType.Value.ToUpper())
                    {
                        case "ENV":
                            actionArg.VarType = enumVarType.Env;
                            actionArg.RefName = RefName.Value;
                            break;
                        case "THIS":
                            actionArg.RefActionId = actionID;
                            actionArg.RefName = RefName.Value;
                            break;
                        default:
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
                            throw new Exception("未知的ActionArg.VarType定义[" + VarType.Value + "]");
                    }
                }
            }
            else
            {
                actionArg.ArgType = enumArgType.Value;
                actionArg.Value = argValue;
            }
            return actionArg;
        }
    }

    public class XMLNodeHelper
    {        
        public static string NodeAtt2String(XmlNode xmlNode, string attName)
        {
            try
            {
                if (xmlNode.Attributes == null || xmlNode.Attributes[attName]==null)
                    return string.Empty;
                return xmlNode.Attributes[attName].Value;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static float NodeAtt2Float(XmlNode xmlNode, string attName)
        {
            float returnValue = float.NaN;
            return float.TryParse(NodeAtt2String(xmlNode, attName), out returnValue) ? returnValue : 0;
        }

        public static double NodeAtt2Double(XmlNode xmlNode, string attName)
        {
            double returnValue = double.NaN;
            return double.TryParse(NodeAtt2String(xmlNode, attName), out returnValue) ? returnValue : 0;
        }

        public static int NodeAtt2Int(XmlNode xmlNode, string attName)
        {
            int returnValue = 0;
            return int.TryParse(NodeAtt2String(xmlNode, attName), out returnValue) ? returnValue : 0;
        }

        public static int NodeAtt2Int(XmlNode xmlNode, string attName, int invalidValue)
        {
            int returnValue = invalidValue;
            return int.TryParse(NodeAtt2String(xmlNode, attName), out returnValue) ? returnValue : invalidValue;
        }

        /// <summary>
        /// 默认返回false
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="attName"></param>
        /// <returns></returns>
        public static bool NodeAtt2Bool(XmlNode xmlNode, string attName)
        {
            bool returnValue = false;
            return bool.TryParse(NodeAtt2String(xmlNode, attName), out returnValue) ? returnValue : false;
        }

        public static bool NodeAtt2Bool(XmlNode xmlNode, string attName, bool invalidValue)
        {
            bool returnValue = invalidValue;
            return bool.TryParse(NodeAtt2String(xmlNode, attName), out returnValue) ? returnValue : invalidValue;
        }
        
        public static Type NodeAtt2Type(XmlNode xmlNode, string attName)
        {
            try
            {
                TypeCode typeCode = (TypeCode)Enum.Parse(typeof(TypeCode), NodeAtt2String(xmlNode, attName), true);
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        return typeof(Boolean);
                    case TypeCode.Byte:
                        return typeof(Byte);
                    case TypeCode.Char:
                        return typeof(Char);
                    case TypeCode.DBNull:
                        return typeof(DBNull);
                    case TypeCode.DateTime:
                        return typeof(DateTime);
                    case TypeCode.Decimal:
                        return typeof(Decimal);
                    case TypeCode.Double:
                        return typeof(Double);
                    case TypeCode.Empty:
                        return null;
                    case TypeCode.Int16:
                        return typeof(Int16);
                    case TypeCode.Int32:
                        return typeof(Int32);
                    case TypeCode.Int64:
                        return typeof(Int64);
                    case TypeCode.Object:
                        return typeof(Object);
                    case TypeCode.SByte:
                        return typeof(SByte);
                    case TypeCode.Single:
                        return typeof(Single);
                    case TypeCode.String:
                        return typeof(String);
                    case TypeCode.UInt16:
                        return typeof(UInt16);
                    case TypeCode.UInt32:
                        return typeof(UInt32);
                    case TypeCode.UInt64:
                        return typeof(UInt64);
                    default:
                        return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
