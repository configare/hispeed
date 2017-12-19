using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.RasterTools;
using GeoDo.RSS.Core.RasterDrawing;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandBandMath : Command
    {
        public CommandBandMath()
            : base()
        {
            _name = "BandMath";
            _text = _toolTip = "波段运算";
            _id = 7100;
        }

        public override void Execute(string argument)
        {
            base.Execute(argument);
        }

        public override void Execute()
        {
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return;
            IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
            if (drawing == null)
                return;
            using (frmSelectExpression frm = new frmSelectExpression())
            {
                frm.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string expression = frm.Expression;
                    //
                    using (frmBandVarMapper frm1 = new frmBandVarMapper())
                    {
                        frm1.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                        frm1.SetExpression(expression);
                        UCBandVarSetter.FileBandNames file = new UCBandVarSetter.FileBandNames();
                        file.FileName = drawing.FileName;
                        file.BandNames = new UCBandVarSetter.BandName[drawing.BandCount];
                        for (int b = 0; b < drawing.BandCount; b++)
                            file.BandNames[b] = new UCBandVarSetter.BandName(b + 1);
                        frm1.SetFiles(new UCBandVarSetter.FileBandNames[] { file });
                        frm1.OutFileName = _smartSession.TemporalFileManager.NextTemporalFilename(".LDF", new string[] {".HDR" });
                        if (frm1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            Dictionary<string, int> mappedBandNos = frm1.MappedBandNos;
                            string[] keys = mappedBandNos.Keys.ToArray().Reverse().ToArray();
                            foreach (string var in keys)
                                expression = expression.Replace(var, "band" + mappedBandNos[var]);
                            expression = expression.Replace("band", "b");
                            DoBandMath(drawing,frm1.OutFileName, expression);
                        }
                    }
                }
            }
        }

        private void DoBandMath(IRasterDrawing drawing,string outFilename, string expression)
        {
            string extName = Path.GetExtension(outFilename).Replace(",", "").ToUpper();
            if (extName == ".DAT")
                extName = "MEM";
            else
                extName = "LDF";
            IBandMathTool bandMathTool = new BandMathTool();
            IProgressMonitor progress = _smartSession.ProgressMonitorManager.DefaultProgressMonitor;
            try
            {
                progress.Reset("正在执行波段运算...",100);
                progress.Start(false);
                bandMathTool.Compute(drawing.DataProviderCopy, expression, extName, outFilename,
                    (idx, tip) =>
                    {
                        progress.Boost(idx, "正在执行波段运算...");
                    });
            }
            finally 
            {
                progress.Finish();
                //if (MsgBox.ShowQuestionYesNo("波段运算已输出文件\"" + outFilename + "\"，要打开文件吗？") == System.Windows.Forms.DialogResult.Yes)
                {
                    OpenFileFactory.Open(outFilename);
                }
            }
        }
    }
}
