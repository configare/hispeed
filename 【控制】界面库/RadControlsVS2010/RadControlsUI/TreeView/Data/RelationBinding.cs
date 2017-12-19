using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace Telerik.WinControls.UI
{
    public class RelationBinding : INotifyPropertyChanged, ICloneable
    {
        private string relationName = string.Empty;

        private object dataSource;
        private string dataMember = string.Empty;

        private string parentMember = string.Empty;
        private string childMember = string.Empty;
        private string displayMember = string.Empty;
        private string valueMember = string.Empty;

        public RelationBinding()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationBinding"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="dataMember">The data member.</param>
        /// <param name="displayMember">The display member.</param>
        /// <param name="parentMember">The parent member.</param>
        /// <param name="childMember">The child member.</param>
        /// <param name="valueMember">The value member.</param>
        public RelationBinding(object dataSource, string dataMember, string displayMember, string parentMember, string childMember, string valueMember)
        {
            this.dataSource = dataSource;
            this.dataMember = dataMember;
            this.displayMember = displayMember;
            this.parentMember = parentMember;
            this.childMember = childMember;
            this.valueMember = valueMember;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationBinding"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="displayMember">The display member.</param>
        /// <param name="parentMember">The parent member.</param>
        /// <param name="childMember">The child member.</param>
        /// <param name="valueMember">The value member.</param>
        public RelationBinding(object dataSource, string displayMember, string parentMember, string childMember, string valueMember)
            : this(dataSource, "", displayMember, parentMember, childMember, valueMember)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationBinding"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="displayMember">The display member.</param>
        /// <param name="parentMember">The parent member.</param>
        /// <param name="childMember">The child member.</param>
        public RelationBinding(object dataSource, string displayMember, string parentMember, string childMember)
            : this(dataSource, displayMember, parentMember, childMember, "")
        {

        }


        /// <summary>
        /// Initializes a new instance of the <see cref="RelationBinding"/> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="displayMember">The display member.</param>
        /// <param name="parentChildMember">The parent child member.</param>
        public RelationBinding(object dataSource, string displayMember, string parentChildMember)
            : this(dataSource, displayMember, parentChildMember, parentChildMember)
        {

        }

        /// <summary>
        /// Gets or sets the name of the relation.
        /// </summary>
        /// <value>The name of the relation.</value>
        [DefaultValue("")]
        [Category("Data")]
        [Description("Gets or sets the name of the relation.")]
        public string RelationName
        {
            get { return this.relationName; }
            set
            {
                this.relationName = value;
                this.OnPropertyChanged("RelationName");
            }
        }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Obsolete("This property will be removed in the next version.")]
        public string DisplayName
        {
            get { return null; }
            set { }
        }

        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        /// <value>The data source.</value>
        [Category("Data")]
        [Description("Gets or sets the data source that the RelationBinding.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [AttributeProvider(typeof(IListSource))]
        [DefaultValue((string)null)]
        public object DataSource
        {
            get { return this.dataSource; }
            set
            {
                this.dataSource = value;
                this.OnPropertyChanged("DataSource");
            }
        }

        /// <summary>
        /// Gets or sets the data member.
        /// </summary>
        /// <value>The data member.</value>
        [Browsable(true), Category("Data"), DefaultValue("")]
        [Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
        [Description("Gets or sets the data member. ")]
        public string DataMember
        {
            get { return this.dataMember; }
            set
            {
                this.dataMember = value;
                this.OnPropertyChanged("DataMember");
            }
        }

        /// <summary>
        /// Gets or sets the display member.
        /// </summary>
        /// <value>The display member.</value>
        [Category("Data")]
        [Description("Gets or sets the display member.")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [DefaultValue("")]
        public string DisplayMember
        {
            get { return this.displayMember; }
            set
            {
                this.displayMember = value;
                this.OnPropertyChanged("DisplayMember");
            }
        }

        /// <summary>
        /// Gets or sets the parent member.
        /// </summary>
        /// <value>The parent member.</value>
        /// 
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Description("Gets or sets the parent member.")]
        [Category("Data")]
        [DefaultValue("")]
        public string ParentMember
        {
            get { return this.parentMember; }
            set
            {
                this.parentMember = value;
                this.OnPropertyChanged("ParentMember");
            }
        }

        /// <summary>
        /// Gets or sets the child member.
        /// </summary>
        /// <value>The child member.</value>
        /// 
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Description("Gets or sets the child member.")]
        [Category("Data")]
        [DefaultValue("")]
        public string ChildMember
        {
            get { return this.childMember; }
            set
            {
                this.childMember = value;
                this.OnPropertyChanged("ChildMember");
            }
        }

        /// <summary>
        /// Gets or sets the value member.
        /// </summary>
        /// <value>The value member.</value>
        /// 
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Description("Gets or sets the value member.")]
        [Category("Data")]
        [DefaultValue("")]
        public string ValueMember
        {
            get { return this.valueMember; }
            set
            {
                this.valueMember = value;
                this.OnPropertyChanged("ValueMember");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show list name].
        /// </summary>
        /// <value><c>true</c> if [show list name]; otherwise, <c>false</c>.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Obsolete("This method will be removed in the next version.")]
        public bool ShowListName
        {
            get { return true; }
            set { }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        object ICloneable.Clone()
        {
            RelationBinding relationBinding = new RelationBinding();
            relationBinding.dataMember = this.dataMember;
            relationBinding.dataSource = this.dataSource;
            relationBinding.displayMember = this.displayMember;
            relationBinding.valueMember = this.valueMember;
            relationBinding.parentMember = this.parentMember;

            return relationBinding;
        }

        public override bool Equals(object obj)
        {
            RelationBinding relationBinding = obj as RelationBinding;
            if (relationBinding != null && relationBinding.dataMember == this.dataMember && relationBinding.dataSource == this.dataSource
                && relationBinding.displayMember == this.displayMember && relationBinding.valueMember == this.valueMember && relationBinding.parentMember == this.parentMember)
            {
                return true;
            }

            return base.Equals(obj);
        }

        
    }
}
