using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.CodeDom.Compiler;
using System.Collections;

namespace GeoDo.RSS.MIF.Core
{
    public static class ExtractFuncProviderFactory
    {
        //最大9999个波段
        private static Regex _bandRegex = new Regex(@"(?<BandVar>band(?<BandNo>\d{1,4}))", RegexOptions.Compiled);
        private static Regex _varRegex = new Regex(@"(?<VarName>var_(?<Var>\w[a-zA-Z_0-9]*))", RegexOptions.Compiled);

        public static IFeatureComputeFuncProvider<TDataType, TFeature> CreateFeatureComputeFuncProvider<TDataType, TFeature>(int[] visitBandNos, string express, IArgumentProvider argProvider)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("public Func<int, %DataType%[], %Feature%> GetComputeFunc()");
            sb.AppendLine("{");
            sb.AppendLine(" return (idx, values) => { return " + GetFuncString(visitBandNos, express, "values[{0}]", argProvider) + ";};");
            sb.AppendLine("}");
            string s = sb.ToString();
            string code = TemplateCode.FeatureComputeFuncTemplate;
            code = code.Replace("%Func%", s);
            code = code.Replace("%DataType%", typeof(TDataType).ToString());
            code = code.Replace("%Feature%", typeof(TFeature).ToString());
            Microsoft.CSharp.CSharpCodeProvider cp = new Microsoft.CSharp.CSharpCodeProvider();
            System.CodeDom.Compiler.CompilerParameters cpar = new System.CodeDom.Compiler.CompilerParameters();
            cpar.GenerateInMemory = true;
            cpar.GenerateExecutable = false;
            cpar.ReferencedAssemblies.Add("System.dll");
            cpar.ReferencedAssemblies.Add(AppDomain.CurrentDomain.BaseDirectory + "GeoDo.RSS.MIF.Core.dll");
            System.CodeDom.Compiler.CompilerResults cr = cp.CompileAssemblyFromSource(cpar, code);
            StringBuilder errorMessages = new StringBuilder();
            foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                errorMessages.AppendLine(ce.ErrorText + "行号:" + ce.Line + "列号:" + ce.Column);
            if (cr.Errors.Count == 0 && cr.CompiledAssembly != null)
            {
                Type ObjType = cr.CompiledAssembly.GetType("GeoDo.RSS.MIF.Core.FuncBuilder");
                try
                {
                    if (ObjType != null)
                    {
                        object obj = Activator.CreateInstance(ObjType);
                        return obj as IFeatureComputeFuncProvider<TDataType, TFeature>;
                    }
                }
                catch (Exception ex)
                {
                    errorMessages.Append(ex.Message);
                }
            }
            if (errorMessages != null && errorMessages.Length > 0)
                throw new Exception(errorMessages.ToString());
            return null;
        }

        public static IExtractFuncProvider<T> CreateExtractFuncProvider<T>(int[] visitBandNos, string express, IArgumentProvider argProvider)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("public Func<int, %DataType%[], bool> GetBoolFunc()");
            sb.AppendLine("{");
            sb.AppendLine(" return (idx, values) => { return " + GetFuncString(visitBandNos, express, "values[{0}]", argProvider) + ";};");
            sb.AppendLine("}");
            string s = sb.ToString();
            string code = TemplateCode.FuncTemplate;
            code = code.Replace("%Func%", s);
            code = code.Replace("%DataType%", typeof(T).ToString());
            Microsoft.CSharp.CSharpCodeProvider cp = new Microsoft.CSharp.CSharpCodeProvider();
            System.CodeDom.Compiler.CompilerParameters cpar = new System.CodeDom.Compiler.CompilerParameters();
            cpar.GenerateInMemory = true;
            cpar.GenerateExecutable = false;
            cpar.ReferencedAssemblies.Add("System.dll");
            cpar.ReferencedAssemblies.Add(AppDomain.CurrentDomain.BaseDirectory + "GeoDo.RSS.MIF.Core.dll");
            System.CodeDom.Compiler.CompilerResults cr = cp.CompileAssemblyFromSource(cpar, code);
            StringBuilder errorMessages = new StringBuilder();
            foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                errorMessages.AppendLine(ce.ErrorText + "行号:" + ce.Line + "列号:" + ce.Column);
            if (cr.Errors.Count == 0 && cr.CompiledAssembly != null)
            {
                Type ObjType = cr.CompiledAssembly.GetType("GeoDo.RSS.MIF.Core.FuncBuilder");
                try
                {
                    if (ObjType != null)
                    {
                        object obj = Activator.CreateInstance(ObjType);
                        return obj as IExtractFuncProvider<T>;
                    }
                }
                catch (Exception ex)
                {
                    errorMessages.Append(ex.Message);
                }
            }
            if (errorMessages != null && errorMessages.Length > 0)
                throw new Exception(errorMessages.ToString());
            return null;
        }

        public static IFeatureSimpleComputeFuncProvider<TDataType, TFeature> CreateSimpleFeatureComputeFuncProvider<TDataType,TFeature>(string express)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("public Func<%DataType%,%Feature%> GetComputeFunc()");
            sb.AppendLine("{");
            sb.AppendLine("  return (x) => { return " + express + ";};");
            sb.AppendLine("}");
            string s = sb.ToString();
            string code = TemplateCode.FeatureSimpleComputeFuncTemplate;
            code = code.Replace("%Func%", s);
            code = code.Replace("%DataType%", typeof(TDataType).ToString());
            code = code.Replace("%Feature%", typeof(TFeature).ToString());
            Microsoft.CSharp.CSharpCodeProvider cp = new Microsoft.CSharp.CSharpCodeProvider();
            System.CodeDom.Compiler.CompilerParameters cpar = new System.CodeDom.Compiler.CompilerParameters();
            cpar.GenerateInMemory = true;
            cpar.GenerateExecutable = false;
            cpar.ReferencedAssemblies.Add("System.dll");
            cpar.ReferencedAssemblies.Add(AppDomain.CurrentDomain.BaseDirectory + "GeoDo.RSS.MIF.Core.dll");
            System.CodeDom.Compiler.CompilerResults cr = cp.CompileAssemblyFromSource(cpar, code);
            StringBuilder errorMessages = new StringBuilder();
            foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                errorMessages.AppendLine(ce.ErrorText + "行号:" + ce.Line + "列号:" + ce.Column);
            if (cr.Errors.Count == 0 && cr.CompiledAssembly != null)
            {
                Type ObjType = cr.CompiledAssembly.GetType("GeoDo.RSS.MIF.Core.FuncBuilder");
                try
                {
                    if (ObjType != null)
                    {
                        object obj = Activator.CreateInstance(ObjType);
                        return obj as IFeatureSimpleComputeFuncProvider<TDataType,TFeature>;
                    }
                }
                catch (Exception ex)
                {
                    errorMessages.Append(ex.Message);
                }
            }
            if (errorMessages != null && errorMessages.Length > 0)
                throw new Exception(errorMessages.ToString());
            return null;
        }

        public static string GetFuncString(int[] visitBandNos, string express, string bandValuesFormat, IArgumentProvider argProvider)
        {
            if (string.IsNullOrEmpty(express))
                return null;
            Dictionary<string, string> bandVars = GetBandVars(visitBandNos, express, bandValuesFormat);
            Dictionary<string, string> vars = GetVars(express, argProvider);
            string exp = express;
            //20120614 by chennan 修改band3 band31 同时存在时，字符串替换为空导致通道解析错误问题
            foreach (string bstr in bandVars.Keys)
                ReplaceExpress(ref exp, exp, bstr, bandVars[bstr]);
            //exp = exp.Replace(bstr, bandVars[bstr]);
            foreach (string vstr in vars.Keys)
                exp = exp.Replace(vstr, vars[vstr]);
            return exp;
        }

        private static Dictionary<string, string> GetVars(string express, IArgumentProvider argProvider)
        {
            Dictionary<string, string> bandVars = new Dictionary<string, string>();
            Match m = null;
        cntLine:
            m = _varRegex.Match(express);
            if (m.Success)
            {
                string var = m.Groups["Var"].Value;
                string varName = m.Groups["VarName"].Value;
                bandVars.Add(varName, argProvider.GetArg(var).ToString());
                express = express.Replace(varName, string.Empty);
                goto cntLine;
            }
            return bandVars;
        }

        private static Dictionary<string, string> GetBandVars(int[] visitBandNos, string express, string bandValuesFormat)
        {
            Dictionary<string, string> bandVars = new Dictionary<string, string>();
            Match m = null;
        cntLine:
            m = _bandRegex.Match(express);
            if (m.Success)
            {
                int bandNo = int.Parse(m.Groups["BandNo"].Value);
                string bandVar = m.Groups["BandVar"].Value;
                //20120614 by chennan 修改band3 band31 同时存在时，字符串替换为空导致通道解析错误问题
                if (!bandVars.ContainsKey(bandVar))
                    bandVars.Add(bandVar, string.Format(bandValuesFormat, GetBandValuesIndex(visitBandNos, bandNo)));
                UpdateExpress(ref express, bandVar);
                //express = express.Replace(bandVar, string.Empty);
                goto cntLine;
            }
            return bandVars;
        }

        private static void UpdateExpress(ref string express, string bandVar)
        {
            int index = 0;
            int bandNo = 0;
            if (express.IndexOf(bandVar) != -1)
            {
                index = express.IndexOf(bandVar);
                if (index + bandVar.Length >= express.Length || !int.TryParse(express.Substring(index + bandVar.Length, 1), out bandNo))
                    express = express.Substring(0, index) + express.Substring(index + bandVar.Length);
            }
        }

        private static void ReplaceExpress(ref string express, string expressTemp, string srcStr, string dstStr)
        {
            int index = 0;
            int bandNo = 0;
            int indexTemp = 0;
            if (expressTemp.IndexOf(srcStr) != -1)
            {
                index = express.IndexOf(srcStr);
                if (index + srcStr.Length >= express.Length || !int.TryParse(express.Substring(index + srcStr.Length, 1), out bandNo))
                    express = express.Substring(0, index) + dstStr + express.Substring(index + srcStr.Length);
                indexTemp = expressTemp.IndexOf(srcStr);
                expressTemp = expressTemp.Substring(0, indexTemp) + expressTemp.Substring(indexTemp + srcStr.Length);
                ReplaceExpress(ref express, expressTemp, srcStr, dstStr);
            }
        }

        private static string GetBandValuesIndex(int[] visitBandNos, int bandNo)
        {
            for (int i = 0; i < visitBandNos.Length; i++)
                if (visitBandNos[i] == bandNo)
                    return i.ToString();
            throw new ArgumentException("visitBandNos与express'bandNo不一致！");
        }
    }
}
