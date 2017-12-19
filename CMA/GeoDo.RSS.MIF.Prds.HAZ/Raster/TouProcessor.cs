using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.HAZ
{
    public class TouProcessor
    {
        public static void ProcessTouFile(string touFile, ref string outFilename, string chinaMask)
        {
            if (string.IsNullOrEmpty(touFile) || !File.Exists(touFile))
                return;
            string[] contexts = File.ReadAllLines(touFile, Encoding.Default);
            if (contexts == null || contexts.Length == 0)
                return;
            if (contexts.Length > 360)
            {
                outFilename = touFile;
                return;
            }
            Dictionary<int, string[]> srcTouFileContexts = new Dictionary<int, string[]>();
            AnalysisSrcFileBySeparator(contexts, srcTouFileContexts);
            //数据行列翻转并处理无效数据
            int row = contexts.Length;
            int col = srcTouFileContexts[0].Length;
            List<string> result = new List<string>();
            List<string> oneCol = new List<string>();
            List<string> endCol = new List<string>();
            StringBuilder rowData = null;
            for (int srcCol = 0; srcCol < col; srcCol++)
            {
                rowData = new StringBuilder();
                rowData.Append("NAN" + " ");
                for (int srcRow = 0; srcRow < row; srcRow++)
                {
                    if (srcCol == 0)
                        oneCol.Add(srcTouFileContexts[srcRow][srcCol]);
                    if (srcCol == col - 1)
                        endCol.Add(srcTouFileContexts[srcRow][srcCol]);
                    //if (float.Parse(srcTouFileContexts[srcRow][srcCol]) <= 0.1f)
                    //    rowData.Append("NAN" + " ");
                    //else
                        rowData.Append(srcTouFileContexts[srcRow][srcCol] + " ");
                }
                result.Add(rowData.ToString());
            }
            //增加一列，数据值为:（第1列+最后1列）/2 
            rowData = new StringBuilder();
            float value = float.NaN;
            for (int i = 0; i < oneCol.Count; i++)
            {
                //value = (float.Parse(oneCol[i]) + float.Parse(endCol[i])) / 2;
                //if (value <= 0.1f || float.IsNaN(value))
                //    rowData.Append("NAN" + " ");
                //else
                    rowData.Append(value + " ");
            }
            result.Add(rowData.ToString());
            if (!string.IsNullOrEmpty(chinaMask) && File.Exists(chinaMask))
                result = ProcessChina(result, chinaMask);
            File.WriteAllLines(outFilename, result.ToArray(), Encoding.Default);
        }

        private static List<string> ProcessChina(List<string> result, string chinaMask)
        {
            string[] contexts = GetContexts(chinaMask);
            if (contexts == null || contexts.Length == 0)
                return result;
            List<string> chinaTou = new List<string>();
            int startRow = 500;
            int startCol = 215;
            int dstRow = 0, dstCol = 0;
            string[] split = null;
            string[] splitChina = null;
            StringBuilder sb = null;
            for (int row = 0; row < result.Count; row++)
            {
                sb = new StringBuilder();
                split = result[row].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (split == null || split.Length == 0)
                    continue;
                if (row < startRow || row >= startRow + contexts.Length)
                    for (int col = 0; col < split.Length; col++)
                        sb.Append("NAN ");
                else
                {
                    splitChina = contexts[dstRow].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitChina == null || splitChina.Length == 0)
                        continue;
                    for (int col = 0; col < split.Length; col++)
                    {
                        if (col < startCol || col >= startCol + splitChina.Length)
                            sb.Append("NAN ");
                        else
                        {
                            if (splitChina[dstCol] == "0")
                                sb.Append("NAN ");
                            else
                                sb.Append(split[col] + " ");
                            dstCol++;
                        }
                    }
                    dstCol = 0;
                    dstRow++;
                }
                chinaTou.Add(sb.ToString());
            }
            return chinaTou.Count == 0 ? null : chinaTou;
        }

        private static string[] GetContexts(string chinaMask)
        {
            string[] contexts = File.ReadAllLines(chinaMask, Encoding.Default);
            if (contexts == null || contexts.Length == 0)
                return null;
            List<string> resultContexts = new List<string>();
            foreach (string item in contexts)
            {
                if (string.IsNullOrEmpty(item))
                    continue;
                resultContexts.Add(item);
            }
            return resultContexts.Count == 0 ? null : resultContexts.ToArray();
        }

        private static void AnalysisSrcFileBySeparator(string[] contexts, Dictionary<int, string[]> srcTouFileContexts)
        {
            List<string> contextList = null;
            string[] splits = null;
            string[] splitTemp = null;
            for (int i = 0; i < contexts.Length; i++)
            {
                contextList = new List<string>();
                splits = contexts[i].Replace("-99.00"," NAN ").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (splits == null || splits.Length == 0)
                    continue;
                for (int length = 0; length < splits.Length; length++)
                {
                    splitTemp = splits[length].Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitTemp.Length <= 2)
                        contextList.Add(splits[length] + " ");
                    else
                    {
                        contextList.Add(splitTemp[0] + "." + splitTemp[1] + " ");
                        for (int tempLength = 2; tempLength < splitTemp.Length; tempLength++)
                            contextList.Add("0." + splitTemp[tempLength] + " ");
                    }
                }
                srcTouFileContexts.Add(i, contextList.ToArray());
            }
        }

        private static void AnalysisSrcFileByLength(string[] contexts, Dictionary<int, string[]> srcTouFileContexts)
        {
            //按长度为6读取数据
            int splitLength = 6;
            List<string> contextList = null;
            string contextTemp;
            for (int i = 0; i < contexts.Length; i++)
            {
                contextList = new List<string>();
                contexts[i] = contexts[i].TrimStart(' ');
                for (int length = 0; i < contexts[i].Length; length += splitLength)
                {
                    if (length >= contexts[i].Length)
                        break;
                    if (length + splitLength > contexts[i].Length)
                        contextTemp = contexts[i].Substring(length);
                    else
                        contextTemp = contexts[i].Substring(length, splitLength);
                    if (contextTemp.IndexOf(" ") == -1)
                        contextTemp = "0" + contextTemp.Substring(contextTemp.IndexOf("."));
                    contextList.Add(contextTemp.PadRight(6, ' '));
                }
                srcTouFileContexts.Add(i, contextList.ToArray());
            }
        }
    }
}
