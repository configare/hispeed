using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CodeCell.AgileMap.Core
{
    public partial class UCFeatureRendererControlBase : UserControl,IFeaureRenderEditorControl
    {
        protected IFeatureLayer _layer = null;

        public UCFeatureRendererControlBase()
        {
            InitializeComponent();
        }

        internal IFeatureLayer Layer
        {
            set 
            {
                _layer = value;
                SetLayerAfter();
            }
        }

        protected virtual void SetLayerAfter()
        {
        }

        #region IFeaureRenderEditorControl Members

        public IFeatureRenderer Renderer
        {
            get 
            {
                return Apply();
            }
        }

        protected virtual IFeatureRenderer Apply()
        {
            return null;
        }

        #endregion
    }
}
