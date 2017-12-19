using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.CodeDom.Compiler;
using System.Collections;

namespace GeoDo.RSS.RasterTools
{
    public static class ClassRuntimeGenerator
    {
        //最大9999个波段
        private static Regex _bandRegex = new Regex(@"(?<BandVar>b(?<BandNo>\d{1,4}))", RegexOptions.Compiled);

        public static IBandMathExecutor GenerateBandMathExecutor(string srcDataType, string dstDataType)
        {
            string code = TemplateCode.BandMathExecutorTemplate;
            code = code.Replace("%SrcDataType%", srcDataType);
            code = code.Replace("%DstDataType%", dstDataType);
            Microsoft.CSharp.CSharpCodeProvider cp = new Microsoft.CSharp.CSharpCodeProvider();
            System.CodeDom.Compiler.CompilerParameters cpar = new System.CodeDom.Compiler.CompilerParameters();
            cpar.GenerateInMemory = true;
            cpar.GenerateExecutable = false;
            cpar.ReferencedAssemblies.Add("System.dll");
            cpar.ReferencedAssemblies.Add(AppDomain.CurrentDomain.BaseDirectory + "GeoDo.RSS.Core.DF.dll");
            cpar.ReferencedAssemblies.Add(AppDomain.CurrentDomain.BaseDirectory + "GeoDo.RSS.RasterTools.dll");
            System.CodeDom.Compiler.CompilerResults cr = cp.CompileAssemblyFromSource(cpar, code);
            StringBuilder errorMessages = new StringBuilder();
            foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                errorMessages.AppendLine(ce.ErrorText + "行号:" + ce.Line + "列号:" + ce.Column);
            if (cr.Errors.Count == 0 && cr.CompiledAssembly != null)
            {
                Type ObjType = cr.CompiledAssembly.GetType("GeoDo.RSS.RasterTools.BandMathExecutor");
                try
                {
                    if (ObjType != null)
                    {
                        object obj = Activator.CreateInstance(ObjType);
                        return obj as IBandMathExecutor;
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

        public static IPixelValuesOperator<T> GeneratePixelValuesOperator<T>(string express, out int[] bandNos)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("public Func<%DataType%[], float> GetOperatorFunc()");
            sb.AppendLine("{");
            sb.AppendLine(" return (values) => { return " + GetOperatorString(express, out bandNos) + ";};");
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
            cpar.ReferencedAssemblies.Add(AppDomain.CurrentDomain.BaseDirectory + "GeoDo.RSS.RasterTools.dll");
            System.CodeDom.Compiler.CompilerResults cr = cp.CompileAssemblyFromSource(cpar, code);
            StringBuilder errorMessages = new StringBuilder();
            foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                errorMessages.AppendLine(ce.ErrorText + "行号:" + ce.Line + "列号:" + ce.Column);
            if (cr.Errors.Count == 0 && cr.CompiledAssembly != null)
            {
                Type ObjType = cr.CompiledAssembly.GetType("GeoDo.RSS.RasterTools.OperatorBuilder");
                try
                {
                    if (ObjType != null)
                    {
                        object obj = Activator.CreateInstance(ObjType);
                        return obj as IPixelValuesOperator<T>;
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

        public static string GetOperatorString(string express, out int[] bandNos)
        {
            bandNos = null;
            if (string.IsNullOrEmpty(express))
                return null;
            Dictionary<string, string> parts = new Dictionary<string, string>();
            MatchCollection mc = _bandRegex.Matches(express);
            if (mc != null)
            {
                List<int> bNos = new List<int>();
                foreach (Match m in mc)
                {
                    int bandNo = int.Parse(m.Groups["BandNo"].Value);
                    if (!bNos.Contains(bandNo))
                        bNos.Add(bandNo);
                }
                bandNos = bNos.ToArray();
            }
            bandNos = bandNos.Reverse().ToArray();
            int i = 0;
            for (int b = bandNos.Length - 1; b >= 0; b--,i++)
                express = express.Replace("b" + bandNos[i], "values[" + b.ToString() + "]");
            bandNos = bandNos.Reverse().ToArray();
            return express;
        }
    }
}
