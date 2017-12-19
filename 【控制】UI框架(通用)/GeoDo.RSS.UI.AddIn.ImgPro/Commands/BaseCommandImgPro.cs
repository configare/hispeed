using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.CA;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.MEF;
using System.Windows.Forms;
using GeoDo.RSS.CA;

namespace GeoDo.RSS.UI.AddIn.ImgPro
{
    public abstract class BaseCommandImgPro : Command
    {
        private static IRgbProcessorArgEditor[] _registeredEditors = null;

        public BaseCommandImgPro()
        {
        }

        public static void PreLoading()
        {
            _registeredEditors = GetRegisteredEditors();
        }

        public override void Execute()
        {
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer as ICanvasViewer;
            if (viewer == null)
                return;
            IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
            if (drawing == null)
                return;
            IRgbProcessor pro = GetRgbProcessor();
            drawing.RgbProcessorStack.Process(pro);
            DisplayArgEditor(pro, drawing, viewer);
        }

        public override void Execute(string argument)
        {
            base.Execute(argument);
        }

        protected abstract IRgbProcessor GetRgbProcessor();

        private void DisplayArgEditor(IRgbProcessor processor, IRasterDrawing drawing, ICanvasViewer viewer)
        {
            if (_registeredEditors == null)
                _registeredEditors = GetRegisteredEditors();
            if (_registeredEditors == null)
                return;
            foreach (IRgbProcessorArgEditor editor in _registeredEditors)
            {
                if (editor.IsSupport(processor.GetType()))
                {
                    IRgbProcessorArgEditor argeditor = Activator.CreateInstance(editor.GetType()) as IRgbProcessorArgEditor;
                    if (processor.Arguments == null)
                        processor.CreateDefaultArguments();
                    if (drawing.SelectedBandNos.Length == 1 || drawing.BandCount == 1)
                    {
                        processor.BytesPerPixel = 1;
                        if (argeditor is frmReversalColorEditor)
                        {
                            return;
                        }
                    }
                    else
                        processor.BytesPerPixel = 3;
                    argeditor.OnPreviewing += new OnArgEditorPreviewing((senser, arg) =>
                    {
                        viewer.Canvas.Refresh(Core.DrawEngine.enumRefreshType.RasterLayer);
                    }
                    );
                    argeditor.OnApplyClicked += new OnArgEditorApplyClick((senser, arg) =>
                    {
                        viewer.Canvas.Refresh(Core.DrawEngine.enumRefreshType.RasterLayer);
                    }
                    );
                    argeditor.OnCancelClicked += new OnArgEditorCancelClick((senser, arg) =>
                    {
                        drawing.RgbProcessorStack.RemoveLast();
                        viewer.Canvas.Refresh(Core.DrawEngine.enumRefreshType.RasterLayer);
                    });
                    if (argeditor is Form)
                    {
                        (argeditor as Form).Owner = _smartSession.SmartWindowManager.MainForm as Form;
                        (argeditor as Form).StartPosition = FormStartPosition.Manual;
                        (argeditor as Form).Location = _smartSession.SmartWindowManager.ViewLeftUpperCorner;
                        (argeditor as Form).Text = processor.Name + "参数设置...";
                    }
                    argeditor.Init(viewer.RgbProcessorArgEditorEnvironment as IRgbArgEditorEnvironmentSupport, processor);
                    argeditor.Show(processor.Arguments);
                    return;
                }
            }
        }

        private static IRgbProcessorArgEditor[] GetRegisteredEditors()
        {
            string[] dlls = MefConfigParser.GetAssemblysByCatalog("图像处理");
            using (IComponentLoader<IRgbProcessorArgEditor> loader = new ComponentLoader<IRgbProcessorArgEditor>())
            {
                return loader.LoadComponents(dlls);
            }
        }
    }
}
