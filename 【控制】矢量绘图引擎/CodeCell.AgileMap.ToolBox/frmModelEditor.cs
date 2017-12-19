using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeCell.Bricks.AppFramework;
using CodeCell.Bricks.UIs;
using CodeCell.Bricks.ModelFabric;

namespace CodeCell.AgileMap.ToolBox
{
    public partial class frmModelEditor : Form
    {
        private IModelEditor _modelEditor = null;
        private string[] _assemblies = new string[] { "CodeCell.AgileMap.Components.dll", "CodeCell.AgileMap.ToolBox.dll" };
        private IApplication _application = null;

        public frmModelEditor()
        {
            InitializeComponent();
            CreateModelEditor();
            ConstructUIs();
            AttachEvents();
        }

        public void RegAssemblies(string[] assemblies)
        {
            _assemblies = assemblies;
        }

        private void ConstructUIs()
        {
            ResourceLoader r = new ResourceLoader("CodeCell.AgileMap.ToolBox", "CodeCell.AgileMap.ToolBox.ZResources.");
            _application = new ApplicationModelEditor(_modelEditor,AppDomain.CurrentDomain.BaseDirectory + "Temp");
            IUIBuilder uibuilder = new UIBuilderModelEditor(this, _application);
            ICommandProvider cmdProvider = new CommandProvider();
            uibuilder.Building(cmdProvider);
            SetToolStripExsDefaultOwner();
        }

        private void SetToolStripExsDefaultOwner()
        {
            foreach (Control c in Controls)
                if (c is ToolStripEx)
                    (c as ToolStripEx).DefaultOwner = this;
        }

        private void AttachEvents()
        {
            Load += new EventHandler(frmModelEditor_Load);
            Disposed += new EventHandler(frmModelEditor_Disposed);
        }

        void frmModelEditor_Disposed(object sender, EventArgs e)
        {
            _modelEditor.Dispose();
            _modelEditor = null;
        }

        void frmModelEditor_Load(object sender, EventArgs e)
        {
            LoadActions();
        }

        private void LoadActions()
        {
            using (ActionReflector r = new ActionReflector())
            {
                foreach (string f in _assemblies)
                {
                    r.AddScanAssembly(AppDomain.CurrentDomain.BaseDirectory+f);
                }
                ucActionBox1.SetActionInfos(r.Reflector());
            }
        }

        private void CreateModelEditor()
        {
            _modelEditor = new ModelEditor(ucModelEditorView1);
        }
    }
}
