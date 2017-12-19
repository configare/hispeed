using System.ComponentModel;
using System.Text;
using System.Collections.Specialized;

namespace Telerik.WinControls.Data
{
    public class GroupDescriptor : INotifyPropertyChanged
    {
        #region Fields

        private SortDescriptorCollection groupNames;
        private GroupDescriptorCollection owner;
        private StringCollection aggregates;
        private string format;
               
        #endregion

        #region Constructors

        public GroupDescriptor()
        {
            this.groupNames = new SortDescriptorCollection();
            this.groupNames.CollectionChanged += new NotifyCollectionChangedEventHandler(groupNames_CollectionChanged);

            this.format = "{0}: {1}";
            this.aggregates = new StringCollection();
        }

        public GroupDescriptor(string expression)
            : this()
        {
            this.Expression = expression;
        }

        public GroupDescriptor(string expression, string format)
            : this()
        {
            this.Expression = expression;

            if (format == null)
            {
                format = string.Empty;
            }
            this.format = format;
        }

        public GroupDescriptor(params  SortDescriptor[] sortDescriptions)
            : this()
        {
            this.groupNames.AddRange(sortDescriptions);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the aggregates.
        /// </summary>
        /// <value>The aggregates.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StringCollection Aggregates
        {
            get { return aggregates; }
        }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>The format.</value>
        [DefaultValue("{0}: {1}")]
        public string Format
        {
            get { return format; }
            set 
            { 
                format = value;
                OnPropertyChanged("Format");
            }
        }

        /// <summary>
        /// Gets or sets the expression.
        /// </summary>
        /// <value>The expression.</value>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual string Expression
        {
            get
            {
                return groupNames.Expression;
            }
            set
            {
                this.groupNames.Expression = value;
                OnPropertyChanged("Expression");
            }
        }

        /// <summary>
        /// Gets the group names.
        /// </summary>
        /// <value>The group names.</value>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SortDescriptorCollection GroupNames
        {
            get
            {
                return this.groupNames;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GroupDescriptorCollection Owner
        {
            get
            {
                return this.owner;
            }
            internal set
            {
                this.owner = value;
            }
        }

        #endregion

        #region Methods

        //TODO: make custom grouping
        //public virtual object GroupNameFromItem(object item, int level)
        //{
        //    return null;
        //}

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (this.GroupNames.Count < 1)
            {
                return base.ToString();
            }

            StringBuilder expression = new StringBuilder();
            for (int i = 0; i < this.GroupNames.Count; i++)
            {
                string delimiter = "";
                if (i < this.GroupNames.Count - 1)
                {
                    delimiter = ",";
                }

                string direction = "ASC";
                if (this.GroupNames[i].Direction == ListSortDirection.Descending)
                {
                    direction = "DESC";
                }

                expression.AppendFormat("{0} {1}{2}", this.GroupNames[i].PropertyName, direction, delimiter);
            }

            return expression.ToString();

        }

        #endregion

        #region Implementation

        void groupNames_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("GroupNames");
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        protected void OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="e">A <see cref="PropertyChangedEventArgs"/> instance containing event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }

        #endregion
    }
}
