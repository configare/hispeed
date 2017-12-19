using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Maps a globally unique identifier (Guid) to a human-readable name.
    /// Used by the new RadDock framework when converting an old DockingManager framework to its new counterpart.
    /// </summary>
	public class GuidToNameMapping
	{
		private Guid guid;
		private string name = string.Empty;

        /// <summary>
        /// Constructs a new default instance.
        /// </summary>
		public GuidToNameMapping()
		{
		}

        /// <summary>
        /// Constructs a new instance, using the provided Guid and Name values.
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="name"></param>
		public GuidToNameMapping(Guid guid, string name)
		{
			this.guid = guid;
			this.name = name;
		}

        /// <summary>
        /// Gets or sets the associated Guid
        /// </summary>
		public Guid Guid
		{
			get
			{
				return this.guid;
			}
			set
			{
				this.guid = value;
			}
		}

        /// <summary>
        /// Gets or sets the associated name.
        /// </summary>
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				this.name = value;
			}
		}
	}

    /// <summary>
    /// A strongly-typed collection of <see cref="GuidToNameMapping">GuidToNameMapping</see> instances.
    /// </summary>
	public class GuidToNameMappingCollection : Collection<GuidToNameMapping>
	{
        /// <summary>
        /// Finds the name that matches the provided Guid.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
		public string FindNameByGuid(Guid guid)
		{
			string name = null;

			GuidToNameMapping mapping = this.FindByGuid(guid);
			if (mapping != null)
			{
				name = mapping.Name;
			}

			return name;
		}

        /// <summary>
        /// Finds the <see cref="GuidToNameMapping">GuidToNameMapping</see> instance that matches the provided Guid.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
		public GuidToNameMapping FindByGuid(Guid guid)
		{
			GuidToNameMapping result = null;

			GuidToNameMapping mapping = null;
			IList<GuidToNameMapping> items = this.Items;
			int count = items.Count;
			for (int i = 0; i < count; i++)
			{
				mapping = items[i];
				if (mapping != null && mapping.Guid == guid)
				{
					result = mapping;
					break;
				}
			}

			return result;
		}

        /// <summary>
        /// Indexer. Gets the Name that matches the provided Guid.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
		public string this[Guid key]
		{
			get
			{
				return this.FindNameByGuid(key);
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				GuidToNameMapping mapping = this.FindByGuid(key);
				if (mapping != null)
				{
					mapping.Name = value;
				}
				else
				{
					mapping = new GuidToNameMapping(key, value);
					this.Add(mapping);
				}
			}
		}
	}
}
