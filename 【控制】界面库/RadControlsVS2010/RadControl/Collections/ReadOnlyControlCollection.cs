using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls
{
    /// <exclude/>
	public class ReadOnlyControlCollection : Control.ControlCollection
	{
		// Methods
		public ReadOnlyControlCollection(Control owner, bool isReadOnly)
			: base(owner)
		{
			this._isReadOnly = isReadOnly;
		}

		public override void Add(Control value)
		{
			if (this.IsReadOnly)
			{
				throw new NotSupportedException(SR.GetString("ReadonlyControlsCollection"));
			}
			this.AddInternal(value);
		}

		public virtual void AddInternal(Control value)
		{
			base.Add(value);
		}

		public override void Clear()
		{
			if (this.IsReadOnly)
			{
				throw new NotSupportedException(SR.GetString("ReadonlyControlsCollection"));
			}
			base.Clear();
		}

		public override void RemoveByKey(string key)
		{
			if (this.IsReadOnly)
			{
				throw new NotSupportedException(SR.GetString("ReadonlyControlsCollection"));
			}
			base.RemoveByKey(key);
		}

		public virtual void RemoveInternal(Control value)
		{
			base.Remove(value);
		}


		// Properties
		public override bool IsReadOnly
		{
			get
			{
				return this._isReadOnly;
			}
		}


		// Fields
		private readonly bool _isReadOnly;
	}
}
