using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using GeoDo.RSS.Core.CA;
using System.Diagnostics;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using System.IO;

namespace test
{
    public partial class Form1 : Form,IRgbArgEditorEnvironmentSupport
    {
        private IRgbProcessorStack _processorStack = new RgbProcessorStack();
        private Bitmap _originalBitmap = null;
        [ImportMany(AllowRecomposition = true)]
        public ObservableCollection<IRgbProcessor> _importedProcessors { get; set; }
        [ImportMany(AllowRecomposition = true)]
        public ObservableCollection<IRgbProcessorArgEditor> _importedArgEditors { get; set; }
        private IPickColorIsFinished _onPickColorIsFinished = null;
        private Bitmap _activeDrawing = null;

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
            SizeChanged += new EventHandler(Form1_SizeChanged);
            LoadRgbProcessors();
            _processorStack.OnStackChanged += new OnStackChangedHandler
                (
                    (sender, processStackIsEmpty, unProcessStackIsEmpty) =>
                    {
                        btnUndo.Enabled = !processStackIsEmpty;
                        btnRedo.Enabled = !unProcessStackIsEmpty;
                    }
                );
            AttachEvents();
        }

        public Bitmap ActiveDrawing
        {
            get { return _activeDrawing; }
        }

        private void AttachEvents()
        {
            MouseMove += new MouseEventHandler(Form1_MouseMove);
            MouseUp += new MouseEventHandler(Form1_MouseUp);
        }

        void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (_onPickColorIsFinished != null)
            {
                _onPickColorIsFinished.PickColorFinished(GetColorAtMouse(e.Location));
                _onPickColorIsFinished = null;
                Cursor = Cursors.Default;
            }
        }

        private Color GetColorAtMouse(Point point)
        {
            if (point.X > _originalBitmap.Width || point.Y > _originalBitmap.Height)
                return Color.Empty;
            return _originalBitmap.GetPixel(point.X, point.Y);
        }

        void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_onPickColorIsFinished != null)
            {
                _onPickColorIsFinished.Picking(GetColorAtMouse(e.Location));
            }
        }

        private void LoadRgbProcessors()
        {
            var catalog = new AggregateCatalog();
            string dir = AppDomain.CurrentDomain.BaseDirectory + @"\Processors";
            catalog.Catalogs.Add(new DirectoryCatalog(dir));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
            int x = btnOpenImage.Right - btnOpenImage.Width;
            int y = btnOpenImage.Bottom + 6;
            foreach (IRgbProcessor p in _importedProcessors)
            {
                CreateButton(p, x, ref y);
                y += 32;
            }
        }

        private void CreateButton(IRgbProcessor p, int x, ref int y)
        {
            Button btn = new Button();
            btn.Text = p.Name;
            btn.Left = x;
            btn.Top = y;
            btn.Width = btnOpenImage.Width;
            btn.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            btn.Tag = p;
            btn.Click += new EventHandler(btn_Click);
            btn.MouseDown += new MouseEventHandler(btn_MouseDown);
            this.Controls.Add(btn);
        }

        void btn_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                IRgbProcessor p = (sender as Button).Tag as IRgbProcessor;
                foreach (IRgbProcessorArgEditor editor in _importedArgEditors)
                {
                    if (editor.IsSupport(p.GetType()))
                    {

                        if (p.Arguments == null)
                            p.CreateDefaultArguments();
                        editor.OnPreviewing += new OnArgEditorPreviewing(RerenderBitmap);
                        editor.OnApplyClicked += new OnArgEditorApplyClick(RerenderBitmap);
                        RgbProcessorArg oldArg = p.Arguments.Clone();
                        (editor as Form).Text = p.Name+"参数设置...";
                        editor.Init(this as IRgbArgEditorEnvironmentSupport,p);
                        editor.Show(p.Arguments);
                        //if (editor.ShowDialog(p.Arguments) == System.Windows.Forms.DialogResult.OK)
                        //{
                        //    Invalidate();
                        //}
                        //else
                        //{
                        //    p.Arguments = oldArg;
                        //    Invalidate();
                        //}
                    }
                }
            }
        }

        void RerenderBitmap(object sender, RgbProcessorArg arg)
        {
            Invalidate();
        }

        void btn_Click(object sender, EventArgs e)
        {
            _processorStack.Process((sender as Button).Tag as IRgbProcessor);
            Invalidate();
        }

        void Form1_SizeChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        Stopwatch sw = new Stopwatch();
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_originalBitmap != null)
            {
                using (Bitmap bitmap = _originalBitmap.Clone() as Bitmap)
                {
                    sw.Start();
                    //
                    _processorStack.Apply(null, bitmap);
                    //
                    this.Text = sw.ElapsedMilliseconds.ToString();
                    sw.Reset();

                    //
                    e.Graphics.DrawImage(bitmap, 0, 0);
                }
            }
        }

        private void btnOpenImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "图片文件(*.JPG,*.BMP)|*.JPG;*.BMP";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _originalBitmap = (Bitmap)Bitmap.FromFile(dlg.FileName);
                    _processorStack.Clear();
                    Invalidate();
                }
            }
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            _processorStack.UnProcess();
            Invalidate();
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            _processorStack.ReProcess();
            Invalidate();
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    string[] error0 = File.ReadAllLines("f:\\error.txt");
        //    string[] true0 = File.ReadAllLines("f:\\true.txt");

        //    for (int i = 0; i < error0.Length; i++)
        //    {
        //        if (error0[i] != true0[i])
        //        {
        //            MessageBox.Show("Not same!");
        //            return;
        //        }
        //    }
        //    MessageBox.Show("Same!");
        //}

        public void StartPickColor(IPickColorIsFinished onPickColorIsFinished)
        {
            _onPickColorIsFinished = onPickColorIsFinished;
            Cursor = Cursors.Cross;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefiledialog = new SaveFileDialog();
            savefiledialog.Filter = "xml文件(*.xml)|*.xml";
            savefiledialog.ShowDialog();
            _processorStack.SaveTo(savefiledialog.FileName);
            MessageBox.Show("保存成功！", "信息提示");
        }

        private void btnOpenXml_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfiledialog = new OpenFileDialog();
            openfiledialog.Filter = "xml文件(*.xml)|*.xml";
            openfiledialog.ShowDialog();
            _processorStack.ReadXmlElement(openfiledialog.FileName);
            Invalidate();
        }



        public List<EventHandler> CanvasRefreshSubscribers
        {
            get { throw new NotImplementedException(); }
        }
    }
}
