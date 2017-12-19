using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;

namespace CodeCell.Bricks.ModelFabric
{
    public partial class frmPropertyEditorDialog : Form,IPropertyEditorDialog
    {
        class BtnTag
        {
            public ComboBox ComboBox = null;
            public ListBox ListBox = null;

            public BtnTag(ComboBox cb, ListBox lstBox)
            {
                ComboBox = cb;
                ListBox = lstBox;
            }
        }

        class BindingDef
        {
            public PropertyInfo PropertyInfo = null;
            public BindingAttribute BindingAttribute = null;
            public Control InputControl = null;
            private ISemanticTypeEditor _editor = null;

            public BindingDef(PropertyInfo propertyInfo, BindingAttribute bindingAtt, Control inputControl)
            {
                PropertyInfo = propertyInfo;
                BindingAttribute = bindingAtt;
                InputControl = inputControl;
            }

            public ISemanticTypeEditor Editor
            {
                get 
                {
                    if (_editor == null)
                    {
                        _editor = Activator.CreateInstance(BindingAttribute.SemanticType) as ISemanticTypeEditor;
                        _editor.DataType = BindingAttribute.DataType;
                    }
                    return _editor;
                }
            }
        }

        class BindingScriptObject
        {
            public IAction RefAction = null;
            public PropertyInfo RefPropertyInfo = null;
            public BindingAttribute RefBindingAtt = null;
            public PropertyInfo TargetPropertyInfo = null;

            public BindingScriptObject(IAction refAction,BindingAttribute refBindingAtt, PropertyInfo refPropertyInfo, PropertyInfo targetPropertyInfo)
            {
                RefAction = refAction;
                RefPropertyInfo = refPropertyInfo;
                RefBindingAtt = refBindingAtt;
                TargetPropertyInfo = targetPropertyInfo;
            }

            public override string ToString()
            {
                return "<"+RefAction.Name + ">.<" + RefBindingAtt.Name+">";
            }

            public BindingPair ToBindingPair()
            {
                PropertyValueByRef refv = new PropertyValueByRef();
                refv.ArgType = enumArgType.Ref;
                refv.RefAction = RefAction;
                refv.RefPropertyInfo = RefPropertyInfo;
                refv.RefBindingAttribute = RefBindingAtt;
                return new BindingPair(TargetPropertyInfo, refv);
            }
        }

        private IAction _action = null;
        private IBindingEnvironment _bindingEnvironment = null;
        private const int cstLeft = 20;
        private const int cstRight = 42;
        private List<BindingDef> _dindingDefs = new List<BindingDef>();
        private Dictionary<ComboBox, BindingDef> _comboxes = new Dictionary<ComboBox, BindingDef>();
        private bool _isGetArgsFromEnv = false;

        public frmPropertyEditorDialog()
        {
            InitializeComponent();
            Load += new EventHandler(frmPropertyEditorDialog_Load);
        }

        void frmPropertyEditorDialog_Load(object sender, EventArgs e)
        {
            BuildControls();
        }

        private void BuildControls()
        {
            Type type = _action.GetType();
            Dictionary<PropertyInfo, BindingAttribute> properites = ArgBindingHelper.GetBindingProperties(type);
            if (properites == null || properites.Count == 0)
                return;
            int top = 20;
            BindingPair[] pairs = null;
            if(_bindingEnvironment != null)
                pairs =  _bindingEnvironment.GetBindingPairs(_action);
            foreach (PropertyInfo p in properites.Keys)
            {
                BindingAttribute att = properites[p];
                if (att.Direction == enumBindingDirection.None || att.Direction == enumBindingDirection.Output)
                    continue;
                top = CreateLabel(top,p,att);
                top = CreateInputControl(top, p, att, pairs);
            }
            foreach (Control c in panelTop.Controls)
            {
                if (c is ComboBox || c is ListBox)
                    c.Anchor = c.Anchor | AnchorStyles.Right;
                else if (c is Button)
                    c.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            }
        }

        private int CreateInputControl(int top, PropertyInfo p, BindingAttribute att,BindingPair[] pairs)
        {
            ComboBox cb = null;
            top = CreateComboBox(top, p, att, out  cb,pairs);
            Type type = p.PropertyType;
            if (type.BaseType != null && type.BaseType.Equals(typeof(Array)))
            {
                top = CreateListBox(top, cb, p, att);
            }
            else
            {
                _dindingDefs.Add(new BindingDef(p, att, cb));
            }
            return top;
        }

        private int CreateListBox(int top, ComboBox cb,PropertyInfo p, BindingAttribute att)
        {
            ListBox lstBox = new ListBox();
            //
            _dindingDefs.Add(new BindingDef(p, att, lstBox));
            //
            lstBox.Left = cstLeft;
            lstBox.Top = top + 4;
            lstBox.Height = 100;
            lstBox.Width = Width - lstBox.Left - 66;
            panelTop.Controls.Add(lstBox);
            //
            Button addBtn = new Button();
            //addBtn.Text = "增加";
            addBtn.Image = imageList1.Images[0];
            addBtn.Left = lstBox.Right + 2;
            addBtn.Top = top + 4;
            addBtn.Width = 24;
            addBtn.Height = 22;
            addBtn.Tag = new BtnTag(cb, lstBox);
            addBtn.Click += new EventHandler(addBtn_Click);
            panelTop.Controls.Add(addBtn);
            //
            Button delBtn = new Button();
            //delBtn.Text = "移除";
            delBtn.Image = imageList1.Images[1];
            delBtn.Left = lstBox.Right + 2;
            delBtn.Top = top + 26 + 4;
            delBtn.Width = 24;
            delBtn.Height = 22;
            delBtn.Tag = new BtnTag(cb, lstBox);
            delBtn.Click += new EventHandler(delBtn_Click);
            panelTop.Controls.Add(delBtn);
            //
            return top + lstBox.Height + 8;
        }

        void delBtn_Click(object sender, EventArgs e)
        {
            BtnTag tag = (sender as Button).Tag as BtnTag;
            ListBox lstbox =tag.ListBox as ListBox;
            if (lstbox.SelectedIndices.Count == 0)
                return;
            int idx = lstbox.SelectedIndex;
            lstbox.Items.RemoveAt(lstbox.SelectedIndex);
            if (idx > 0 && idx < lstbox.Items.Count)
                lstbox.SelectedIndex = idx;
        }

        void addBtn_Click(object sender, EventArgs e)
        {
            BtnTag tag = (sender as Button).Tag as BtnTag;
            //从其它Action帮定值
            if (tag.ComboBox.Tag is BindingScriptObject)
                BindValueByAction(tag);
            else
                DirectInputValue(tag);
            //
            tag.ComboBox.Text = string.Empty;
            tag.ComboBox.Tag = null;
        }

        private void BindValueByAction(BtnTag tag)
        {
            //
        }

        private void DirectInputValue(BtnTag tag)
        {
            if (tag.ComboBox.Tag != null && tag.ComboBox.Tag is IEnumerable)
            {
                IEnumerable eable = tag.ComboBox.Tag as IEnumerable;
                if (eable is object[])
                {
                    tag.ListBox.BeginUpdate();
                    tag.ListBox.Items.AddRange(eable as object[]);
                    tag.ListBox.EndUpdate();
                }
                else
                {
                    foreach (object v in eable)
                        tag.ListBox.Items.Add(v);
                }
            }
            else
            {
                if (IsRefType(_comboxes[tag.ComboBox].BindingAttribute.SemanticType))
                {
                    object v = null;
                    if (_comboxes[tag.ComboBox].Editor.TryParse(tag.ComboBox.Text, out v))
                    {
                        if (v != null)
                        {
                            if (v.GetType().BaseType.Equals(typeof(Array)))
                            {
                                foreach (object vi in v as IEnumerable)
                                    tag.ListBox.Items.Add(vi);
                            }
                            else
                            {
                                tag.ListBox.Items.Add(v);
                            }
                        }
                    }
                }
                else
                {
                    tag.ListBox.Items.Add(tag.ComboBox.Text);
                }
            }
        }

        private int CreateComboBox(int top, PropertyInfo p, BindingAttribute att,out ComboBox cb,BindingPair[] pairs)
        {
            cb = new ComboBox();
            if (pairs != null && pairs.Length > 0)
            {
                InitExistBindingPair(cb, p,pairs);
            }
            //填充现有值
            object pValue = _action.GetType().InvokeMember(p.Name, BindingFlags.GetProperty, null, _action,null);
            if (pValue != null)
            {
                cb.Text = (new BindingDef(p, att, cb).Editor).ToString(pValue);
                cb.Tag = pValue;
            }
            //
            cb.Left = cstLeft;
            cb.Top = top;
            cb.Width = Width - cb.Left - cstRight;
            _comboxes.Add(cb, new BindingDef(p, att, cb));
            cb.KeyPress += new KeyPressEventHandler(cb_KeyPress);
            cb.DropDown += new EventHandler(cb_DropDown);
            cb.SelectedIndexChanged += new EventHandler(cb_SelectedIndexChanged);
            panelTop.Controls.Add(cb);
            if (IsRefType(att.SemanticType))
            {
                cb.Width -= 24;
                Button btnEditor = new Button();
                btnEditor.Tag = new BindingDef(p, att, cb);
                if (IsArgFile(att.SemanticType))
                    btnEditor.Image = imageList1.Images[2];
                else if (IsArgDirectory(att.SemanticType))
                    btnEditor.Image = imageList1.Images[3];
                else
                    btnEditor.Image = imageList1.Images[4];
                btnEditor.Left = cb.Right + 2;
                btnEditor.Top = cb.Top;
                btnEditor.Width = 24;
                btnEditor.Height = 20;
                btnEditor.Click += new EventHandler(btnEditor_Click);
                panelTop.Controls.Add(btnEditor);
            }
            return top + cb.Height + 4;
        }

        private void InitExistBindingPair(ComboBox cb, PropertyInfo p, BindingPair[] pairs)
        {
            foreach (BindingPair pir in pairs)
            {
                if (pir.PropertyInfo.Equals(p))
                {
                    if (pir.PropertyValue is PropertyValueByValue)
                    {
                        object txt =  (pir.PropertyValue as PropertyValueByValue).Value;
                        if(txt != null)
                            cb.Text = txt.ToString();
                    }
                    else if (pir.PropertyValue is PropertyValueByVar)
                    {
                        cb.Text = (pir.PropertyValue as PropertyValueByVar).VarValue;
                    }
                    else if (pir.PropertyValue is PropertyValueByRef)
                    {
                        PropertyValueByRef refv = pir.PropertyValue as PropertyValueByRef;
                        BindingScriptObject obj = new BindingScriptObject(refv.RefAction, refv.RefBindingAttribute, refv.RefPropertyInfo, p);
                        cb.Items.Add(obj);
                        cb.SelectedIndex = cb.Items.Count - 1;
                    }
                    break;
                }
            }
        }

        void cb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if((sender as ComboBox).SelectedItem != null)
                (sender as ComboBox).Tag = (sender as ComboBox).SelectedItem;
        }

        void cb_DropDown(object sender, EventArgs e)
        {
            if (!_isGetArgsFromEnv && _bindingEnvironment != null)
            {
                GetArgsAndInitComboBoxes();
                _isGetArgsFromEnv = true;
            }
        }

        private void GetArgsAndInitComboBoxes()
        {
            BindingDef def = null;
            foreach(ComboBox cb in _comboxes.Keys)
            {
                def = _comboxes[cb];
                cb.Items.Clear();
                Dictionary<IAction, PropertyInfo[]> atts = _bindingEnvironment.QueryCompatibleProperty(_action, def.BindingAttribute, def.PropertyInfo);
                if (atts != null && atts.Count > 0)
                {
                    foreach (IAction act in atts.Keys)
                    {
                        PropertyInfo[] ps = atts[act];
                        if (ps == null || ps.Length == 0)
                            continue;
                        foreach (PropertyInfo p in ps)
                        {
                            BindingAttribute ba = ArgBindingHelper.GetBingdingAttribute(p.Name, act);
                            BindingScriptObject script = new BindingScriptObject(act, ba, p, def.PropertyInfo);
                            cb.Items.Add(script);
                        }
                    }
                }
            }
        }

        void cb_KeyPress(object sender, KeyPressEventArgs e)
        {
            BindingDef def = _comboxes[(sender as ComboBox)];
            e.Handled = !def.Editor.IsNeedInput;
        }

        void btnEditor_Click(object sender, EventArgs e)
        {
            BindingDef def = (sender as Button).Tag as BindingDef;
            ISemanticTypeEditor editor = def.Editor;
            object obj = editor.GetValue(_action);
            if (obj != null)
            {
                (def.InputControl as ComboBox).Text = editor.ToString(obj);
                (def.InputControl as ComboBox).Tag = obj;
            }
        }

        private bool IsRefType(Type type)
        {
            while (!type.Equals(typeof(object)))
            {
                if (type.Equals(typeof(ArgRefType)))
                    return true;
                type = type.BaseType;
            }
            return false;
        }


        private bool IsArgDirectory(Type type)
        {
            while (!type.Equals(typeof(object)))
            {
                if (type.Equals(typeof(ArgDirectory)))
                    return true;
                type = type.BaseType;
            }
            return false;
        }

        private bool IsArgFile(Type type)
        {
            while(!type.Equals(typeof(object)))
            {
                if (type.Equals(typeof(ArgFile)))
                    return true;
                type = type.BaseType;
            }
            return false;
        }

        private int CreateLabel(int top, PropertyInfo p, BindingAttribute att)
        {
            Label lb = new Label();
            lb.Top = top;
            lb.Left = cstLeft - 14;
            lb.Text = "○" + att.Name + ":";
            lb.Width = Width - lb.Left - 64;
            lb.Height = 18;
            panelTop.Controls.Add(lb);
            return top + lb.Height;
        }

        #region IPropertyEditorDialog 成员

        public bool ShowDialog(IAction action, IBindingEnvironment bindingEnvironment)
        {
            if (action == null)
                return false;
            _action = action;
            _bindingEnvironment = bindingEnvironment;
            Text = action.Name;
            DialogResult dlg =  ShowDialog();
            return dlg == DialogResult.OK;
        }

        #endregion

        private void DoDirectBindingValue()
        {
            foreach (BindingDef def in _dindingDefs)
            {
                object value = GetDirectValue(def);
                _action.GetType().InvokeMember(def.PropertyInfo.Name,
                                                                   BindingFlags.SetProperty, 
                                                                   null, 
                                                                   _action, 
                                                                   new object[] { value });
            }
        }

        private object GetDirectValue(BindingDef def)
        {
            if (def.InputControl is ComboBox)
            {
                ComboBox cb = def.InputControl as ComboBox;
                object value = null;
                if (def.Editor.IsNeedInput && def.Editor.TryParse(cb.Text, out value))
                {
                    return value;
                }
                else
                {
                    return cb.Tag;
                }
            }
            else if (def.InputControl is ListBox)
            {
                ListBox lstBox = def.InputControl as ListBox;
                if (lstBox.Items.Count == 0)
                    return null;
                int i = 0;
                Array valArray = Array.CreateInstance(def.PropertyInfo.PropertyType.GetElementType(), lstBox.Items.Count);
                foreach (object v in lstBox.Items)
                    valArray.SetValue(v, i++);
                return valArray;
            }
            return null;
        }

        private void DoBindingPair()
        {
            List<BindingPair> ps = new List<BindingPair>();
            foreach (ComboBox cb in _comboxes.Keys)
            {
                BindingPair pair = null;
                //
                BindingDef def = _comboxes[cb];
                //与其它Action绑定
                if (cb.Tag is BindingScriptObject)
                {
                    pair = (cb.Tag as BindingScriptObject).ToBindingPair();
                }
                //直接绑定值
                else
                {
                    PropertyValueByValue pv = new PropertyValueByValue();
                    pv.ArgType = enumArgType.Var;
                    pv.Value = GetDirectValue(def);
                    pair = new BindingPair(def.PropertyInfo, pv);
                }
                ps.Add(pair);
            }
            _bindingEnvironment.UpdateBindingPair(_action, ps.ToArray());
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_bindingEnvironment == null)
                DoDirectBindingValue();
            else
                DoBindingPair();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
