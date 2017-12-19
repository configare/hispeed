using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace Telerik.WinControls
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct InsertionSortMap
	{
		internal LargeSortedObjectMap _mapStore;

		public void Sort()
		{
			if (this._mapStore != null)
			{
				this._mapStore.Sort();
			}
		}

		public void GetKeyValuePair(int index, out int key, out object value)
		{
			if (this._mapStore == null)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			this._mapStore.GetKeyValuePair(index, out key, out value);
		}

		public void Iterate(ArrayList list, FrugalMapIterationCallback callback)
		{
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (this._mapStore != null)
			{
				this._mapStore.Iterate(list, callback);
			}
		}

		public int Count
		{
			get
			{
				if (this._mapStore != null)
				{
					return this._mapStore.Count;
				}
				return 0;
			}
		}

		public object this[int key]
		{
			get
			{
				if (this._mapStore != null)
				{
					return this._mapStore.Search(key);
				}
				return RadProperty.UnsetValue;
			}
			set
			{
				if (value != RadProperty.UnsetValue)
				{
					if (this._mapStore == null)
					{
						this._mapStore = new LargeSortedObjectMap();
					}
					FrugalMapStoreState state1 = this._mapStore.InsertEntry(key, value);
					if (state1 != FrugalMapStoreState.Success)
					{
						if (FrugalMapStoreState.SortedArray != state1)
						{
							throw new InvalidOperationException("FrugalMap: CannotPromoteBeyondHashtable");
						}
						LargeSortedObjectMap map1 = new LargeSortedObjectMap();
						this._mapStore.Promote(map1);
						this._mapStore = map1;
						this._mapStore.InsertEntry(key, value);
					}
				}
				else if (this._mapStore != null)
				{
					this._mapStore.RemoveEntry(key);
					if (this._mapStore.Count == 0)
					{
						this._mapStore = null;
					}
				}
			}
		}
	}

	internal enum FrugalMapStoreState
	{
		Success,
		ThreeObjectMap,
		SixObjectMap,
		Array,
		SortedArray,
		Hashtable
	}

	internal delegate void FrugalMapIterationCallback(ArrayList list, int key, object value);

	internal enum FrugalListStoreState
	{
		Success,
		SingleItemList,
		ThreeItemList,
		SixItemList,
		Array
	}

	internal abstract class FrugalListBase<T>
	{
		// Fields
		protected int _count;

		// Methods
		protected FrugalListBase()
		{
		}

		public abstract FrugalListStoreState Add(T value);
		public abstract void Clear();
		public abstract object Clone();
		public abstract bool Contains(T value);
		public abstract void CopyTo(T[] array, int index);
		public abstract T EntryAt(int index);
		public abstract int IndexOf(T value);
		public abstract void Insert(int index, T value);
		public abstract void Promote(FrugalListBase<T> newList);
		public abstract bool Remove(T value);
		public abstract void RemoveAt(int index);
		public abstract void SetAt(int index, T value);
		public abstract void Sort();
		public abstract T[] ToArray();

		// Properties
		public abstract int Capacity { get; }
		public int Count
		{
			get
			{
				return this._count;
			}
		}
	}


	internal abstract class FrugalMapBase
	{
		// Methods
		protected FrugalMapBase()
		{
		}

		public abstract void GetKeyValuePair(int index, out int key, out object value);
		public abstract FrugalMapStoreState InsertEntry(int key, object value);
		public abstract void Iterate(ArrayList list, FrugalMapIterationCallback callback);
		public abstract void Promote(FrugalMapBase newMap);
		public abstract void RemoveEntry(int key);
		public abstract object Search(int key);
		public abstract void Sort();

		// Properties
		public abstract int Count { get; }

		// Fields
		public const int INVALIDKEY = 0x7fffffff;

		// Nested Types
		[StructLayout(LayoutKind.Sequential)]
		internal struct Entry
		{
			public int Key;
			public object Value;
		}
	}

	internal sealed class LargeSortedObjectMap : FrugalMapBase
	{
		// Fields
		internal int _count;
		private FrugalMapBase.Entry[] _entries;
		private int _lastKey;
		private const int MINSIZE = 2;

		// Methods
		public LargeSortedObjectMap()
		{
			this._lastKey = INVALIDKEY;
		}

		private int FindInsertIndex(int key, out bool found)
		{
			int num1 = 0;
			if ((this._count > 0) && (key <= this._lastKey))
			{
				int num2 = this._count - 1;
				do
				{
					int num3 = (num2 + num1) / 2;
					if (key <= this._entries[num3].Key)
					{
						num2 = num3;
					}
					else
					{
						num1 = num3 + 1;
					}
				}
				while (num1 < num2);
				found = key == this._entries[num1].Key;
				return num1;
			}
			num1 = this._count;
			found = false;
			return num1;
		}

		public override void GetKeyValuePair(int index, out int key, out object value)
		{
			if (index < this._count)
			{
				value = this._entries[index].Value;
				key = this._entries[index].Key;
			}
			else
			{
				value = RadProperty.UnsetValue;
				key = INVALIDKEY;
				throw new ArgumentOutOfRangeException("index");
			}
		}

		public override FrugalMapStoreState InsertEntry(int key, object value)
		{
			bool flag1;
			int num1 = this.FindInsertIndex(key, out flag1);
			if (flag1)
			{
				this._entries[num1].Value = value;
				return FrugalMapStoreState.Success;
			}
			if (this._entries != null)
			{
				if (this._entries.Length <= this._count)
				{
					int num2 = this._entries.Length;
					FrugalMapBase.Entry[] entryArray1 = new FrugalMapBase.Entry[num2 + (num2 >> 1)];
					Array.Copy(this._entries, 0, entryArray1, 0, this._entries.Length);
					this._entries = entryArray1;
				}
			}
			else
			{
				this._entries = new FrugalMapBase.Entry[2];
			}
			if (num1 < this._count)
			{
				Array.Copy(this._entries, num1, this._entries, (int)(num1 + 1), (int)(this._count - num1));
			}
			else
			{
				this._lastKey = key;
			}
			this._entries[num1].Key = key;
			this._entries[num1].Value = value;
			this._count++;
			return FrugalMapStoreState.Success;
		}

		public override void Iterate(ArrayList list, FrugalMapIterationCallback callback)
		{
			if (this._count > 0)
			{
				for (int num1 = 0; num1 < this._count; num1++)
				{
					callback(list, this._entries[num1].Key, this._entries[num1].Value);
				}
			}
		}

		public override void Promote(FrugalMapBase newMap)
		{
			for (int num1 = 0; num1 < this._entries.Length; num1++)
			{
				if (newMap.InsertEntry(this._entries[num1].Key, this._entries[num1].Value) != FrugalMapStoreState.Success)
				{
					object[] objArray1 = new object[] { this.ToString(), newMap.ToString() };
					throw new ArgumentException(string.Format("FrugalMap_TargetMapCannotHoldAllData {0}; {1}", objArray1));
				}
			}
		}

		public override void RemoveEntry(int key)
		{
			bool flag1;
			int num1 = this.FindInsertIndex(key, out flag1);
			if (flag1)
			{
				int num2 = (this._count - num1) - 1;
				if (num2 > 0)
				{
					Array.Copy(this._entries, (int)(num1 + 1), this._entries, num1, num2);
				}
				else if (this._count > 1)
				{
					this._lastKey = this._entries[this._count - 2].Key;
				}
				else
				{
					this._lastKey = INVALIDKEY;
				}
				this._entries[this._count - 1].Key = INVALIDKEY;
				this._entries[this._count - 1].Value = RadProperty.UnsetValue;
				this._count--;
			}
		}

		public override object Search(int key)
		{
			bool flag1;
			int num1 = this.FindInsertIndex(key, out flag1);
			if (flag1)
			{
				return this._entries[num1].Value;
			}
			return RadProperty.UnsetValue;
		}

		public override void Sort()
		{
		}

		// Properties
		public override int Count
		{
			get
			{
				return this._count;
			}
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct ItemStructList<T>
	{
		public int Count;
		public T[] List;

		public ItemStructList(int capacity)
		{
			this.List = new T[capacity];
			this.Count = 0;
		}

		public int Add()
		{
			return this.Add(1, true);
		}

		public int Add(int delta)
		{
			return this.Add(delta, true);
		}

		public void Add(ref T item)
		{
			int num1 = this.Add(1, false);
			this.List[num1] = item;
			this.Count++;
		}

		public void Add(T item)
		{
			int num1 = this.Add(1, false);
			this.List[num1] = item;
			this.Count++;
		}

		private int Add(int delta, bool incrCount)
		{
			if (this.List != null)
			{
				if ((this.Count + delta) > this.List.Length)
				{
					T[] localArray1 = new T[Math.Max((int)(this.List.Length * 2), (int)(this.Count + delta))];
					this.List.CopyTo(localArray1, 0);
					this.List = localArray1;
				}
			}
			else
			{
				this.List = new T[Math.Max(delta, 2)];
			}
			int num1 = this.Count;
			if (incrCount)
			{
				this.Count += delta;
			}
			return num1;
		}

		public void AppendTo(ref ItemStructList<T> destinationList)
		{
			for (int num1 = 0; num1 < this.Count; num1++)
			{
				destinationList.Add(ref this.List[num1]);
			}
		}

		public void Clear()
		{
			Array.Clear(this.List, 0, this.Count);
			this.Count = 0;
		}

		public bool Contains(T value)
		{
			return (this.IndexOf(value) != -1);
		}

		public void EnsureIndex(int index)
		{
			int num1 = (index + 1) - this.Count;
			if (num1 > 0)
			{
				this.Add(num1);
			}
		}

		public int IndexOf(T value)
		{
			int num1 = -1;
			for (int num2 = 0; num2 < this.Count; num2++)
			{
				if (this.List[num2].Equals(value))
				{
					num1 = num2;
					break;
				}
			}
			return num1;
		}

		public bool IsValidIndex(int index)
		{
			if (index >= 0)
			{
				return (index < this.Count);
			}
			return false;
		}

		public void Remove(T value)
		{
			int num1 = this.IndexOf(value);
			if (num1 != -1)
			{
				Array.Copy(this.List, (int)(num1 + 1), this.List, num1, (int)((this.Count - num1) - 1));
				Array.Clear(this.List, this.Count - 1, 1);
				this.Count--;
			}
		}

		public void Sort()
		{
			if (this.List != null)
			{
				Array.Sort<T>(this.List, 0, this.Count);
			}
		}

		public T[] ToArray()
		{
			T[] localArray1 = new T[this.Count];
			Array.Copy(this.List, 0, localArray1, 0, this.Count);
			return localArray1;
		}
	}


	internal sealed class SingleObjectMap : FrugalMapBase
	{
		public SingleObjectMap()
		{
			this._loneEntry.Key = 0x7fffffff;
			this._loneEntry.Value = RadProperty.UnsetValue;
		}

		public override void GetKeyValuePair(int index, out int key, out object value)
		{
			if (index == 0)
			{
				value = this._loneEntry.Value;
				key = this._loneEntry.Key;
			}
			else
			{
				value = RadProperty.UnsetValue;
				key = 0x7fffffff;
				throw new ArgumentOutOfRangeException("index");
			}
		}

		public override FrugalMapStoreState InsertEntry(int key, object value)
		{
			if ((0x7fffffff != this._loneEntry.Key) && (key != this._loneEntry.Key))
			{
				return FrugalMapStoreState.ThreeObjectMap;
			}
			this._loneEntry.Key = key;
			this._loneEntry.Value = value;
			return FrugalMapStoreState.Success;
		}

		public override void Iterate(ArrayList list, FrugalMapIterationCallback callback)
		{
			if (this.Count == 1)
			{
				callback(list, this._loneEntry.Key, this._loneEntry.Value);
			}
		}

		public override void Promote(FrugalMapBase newMap)
		{
			if (newMap.InsertEntry(this._loneEntry.Key, this._loneEntry.Value) != FrugalMapStoreState.Success)
			{
				object[] objArray1 = new object[] { this.ToString(), newMap.ToString(), "newMap" };
				throw new ArgumentException(string.Format("FrugalMap_TargetMapCannotHoldAllData {0}, {1}, {2}", objArray1));
			}
		}

		public override void RemoveEntry(int key)
		{
			if (key == this._loneEntry.Key)
			{
				this._loneEntry.Key = 0x7fffffff;
				this._loneEntry.Value = RadProperty.UnsetValue;
			}
		}

		public override object Search(int key)
		{
			if (key == this._loneEntry.Key)
			{
				return this._loneEntry.Value;
			}
			return RadProperty.UnsetValue;
		}

		public override void Sort()
		{
		}

		// Properties
		public override int Count
		{
			get
			{
				if (0x7fffffff != this._loneEntry.Key)
				{
					return 1;
				}
				return 0;
			}
		}

		// Fields
		private FrugalMapBase.Entry _loneEntry;
	}


	[StructLayout(LayoutKind.Sequential)]
	internal struct FrugalMap
	{
		internal FrugalMapBase _mapStore;

		public object this[int key]
		{
			get
			{
				if (this._mapStore != null)
				{
					return this._mapStore.Search(key);
				}
				return RadProperty.UnsetValue;
			}
			set
			{
				if (value != RadProperty.UnsetValue)
				{
					if (this._mapStore == null)
					{
						this._mapStore = new SingleObjectMap();
					}
					FrugalMapStoreState state1 = this._mapStore.InsertEntry(key, value);
					if (state1 != FrugalMapStoreState.Success)
					{
						FrugalMapBase base1;
						if (FrugalMapStoreState.ThreeObjectMap == state1)
						{
							base1 = new ThreeObjectMap();
						}
						else if (FrugalMapStoreState.SixObjectMap == state1)
						{
							base1 = new SixObjectMap();
						}
						else if (FrugalMapStoreState.Array == state1)
						{
							base1 = new ArrayObjectMap();
						}
						else if (FrugalMapStoreState.SortedArray == state1)
						{
							base1 = new SortedObjectMap();
						}
						else
						{
							if (FrugalMapStoreState.Hashtable != state1)
							{
								throw new InvalidOperationException("CannotPromoteBeyondHashtable");
							}
							base1 = new HashObjectMap();
						}
						this._mapStore.Promote(base1);
						this._mapStore = base1;
						this._mapStore.InsertEntry(key, value);
					}
				}
				else if (this._mapStore != null)
				{
					this._mapStore.RemoveEntry(key);
					if (this._mapStore.Count == 0)
					{
						this._mapStore = null;
					}
				}
			}
		}

		public void Sort()
		{
			if (this._mapStore != null)
			{
				this._mapStore.Sort();
			}
		}

		public void GetKeyValuePair(int index, out int key, out object value)
		{
			if (this._mapStore == null)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			this._mapStore.GetKeyValuePair(index, out key, out value);
		}

		public void Iterate(ArrayList list, FrugalMapIterationCallback callback)
		{
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (this._mapStore != null)
			{
				this._mapStore.Iterate(list, callback);
			}
		}

		public int Count
		{
			get
			{
				if (this._mapStore != null)
				{
					return this._mapStore.Count;
				}
				return 0;
			}
		}

	}

	internal sealed class ThreeObjectMap : FrugalMapBase
	{
		// Fields
		private ushort _count;
		private FrugalMapBase.Entry _entry0;
		private FrugalMapBase.Entry _entry1;
		private FrugalMapBase.Entry _entry2;
		private bool _sorted;
		private const int SIZE = 3;

		// Methods
		public ThreeObjectMap()
		{
		}

		public override void GetKeyValuePair(int index, out int key, out object value)
		{
			if (index < this._count)
			{
				switch (index)
				{
					case 0:
						{
							key = this._entry0.Key;
							value = this._entry0.Value;
							return;
						}
					case 1:
						{
							key = this._entry1.Key;
							value = this._entry1.Value;
							return;
						}
					case 2:
						{
							key = this._entry2.Key;
							value = this._entry2.Value;
							return;
						}
				}
				key = 0x7fffffff;
				value = RadProperty.UnsetValue;
			}
			else
			{
				key = 0x7fffffff;
				value = RadProperty.UnsetValue;
				throw new ArgumentOutOfRangeException("index");
			}
		}

		public override FrugalMapStoreState InsertEntry(int key, object value)
		{
			switch (this._count)
			{
				case 1:
					{
						if (this._entry0.Key != key)
						{
							break;
						}
						this._entry0.Value = value;
						return FrugalMapStoreState.Success;
					}
				case 2:
					{
						if (this._entry0.Key != key)
						{
							if (this._entry1.Key != key)
							{
								break;
							}
							this._entry1.Value = value;
							return FrugalMapStoreState.Success;
						}
						this._entry0.Value = value;
						return FrugalMapStoreState.Success;
					}
				case 3:
					{
						if (this._entry0.Key != key)
						{
							if (this._entry1.Key == key)
							{
								this._entry1.Value = value;
								return FrugalMapStoreState.Success;
							}
							if (this._entry2.Key == key)
							{
								this._entry2.Value = value;
								return FrugalMapStoreState.Success;
							}
							break;
						}
						this._entry0.Value = value;
						return FrugalMapStoreState.Success;
					}
			}
			if (3 <= this._count)
			{
				return FrugalMapStoreState.SixObjectMap;
			}
			switch (this._count)
			{
				case 0:
					{
						this._entry0.Key = key;
						this._entry0.Value = value;
						this._sorted = true;
						break;
					}
				case 1:
					{
						this._entry1.Key = key;
						this._entry1.Value = value;
						this._sorted = false;
						break;
					}
				case 2:
					{
						this._entry2.Key = key;
						this._entry2.Value = value;
						this._sorted = false;
						break;
					}
			}
			this._count = (ushort)(this._count + 1);
			return FrugalMapStoreState.Success;
		}

		public override void Iterate(ArrayList list, FrugalMapIterationCallback callback)
		{
			if (this._count > 0)
			{
				if (this._count >= 1)
				{
					callback(list, this._entry0.Key, this._entry0.Value);
				}
				if (this._count >= 2)
				{
					callback(list, this._entry1.Key, this._entry1.Value);
				}
				if (this._count == 3)
				{
					callback(list, this._entry2.Key, this._entry2.Value);
				}
			}
		}

		public override void Promote(FrugalMapBase newMap)
		{
			if (newMap.InsertEntry(this._entry0.Key, this._entry0.Value) != FrugalMapStoreState.Success)
			{
				object[] objArray1 = new object[] { this.ToString(), newMap.ToString(), "newMap" };
				throw new ArgumentException(string.Format("FrugalMap_TargetMapCannotHoldAllData: {0}, {1}, {2}", objArray1));
			}
			if (newMap.InsertEntry(this._entry1.Key, this._entry1.Value) != FrugalMapStoreState.Success)
			{
				object[] objArray1 = new object[] { this.ToString(), newMap.ToString(), "newMap" };
				throw new ArgumentException(string.Format("FrugalMap_TargetMapCannotHoldAllData: {0}, {1}, {2}", objArray1));
			}
			if (newMap.InsertEntry(this._entry2.Key, this._entry2.Value) != FrugalMapStoreState.Success)
			{
				object[] objArray1 = new object[] { this.ToString(), newMap.ToString(), "newMap" };
				throw new ArgumentException(string.Format("FrugalMap_TargetMapCannotHoldAllData: {0}, {1}, {2}", objArray1));
			}
		}

		public override void RemoveEntry(int key)
		{
			switch (this._count)
			{
				case 1:
					{
						if (this._entry0.Key == key)
						{
							this._entry0.Key = 0x7fffffff;
							this._entry0.Value = RadProperty.UnsetValue;
							this._count = (ushort)(this._count - 1);
						}
						return;
					}
				case 2:
					{
						if (this._entry0.Key != key)
						{
							if (this._entry1.Key == key)
							{
								this._entry1.Key = 0x7fffffff;
								this._entry1.Value = RadProperty.UnsetValue;
								this._count = (ushort)(this._count - 1);
							}
							return;
						}
						this._entry0 = this._entry1;
						this._entry1.Key = 0x7fffffff;
						this._entry1.Value = RadProperty.UnsetValue;
						this._count = (ushort)(this._count - 1);
						return;
					}
				case 3:
					{
						if (this._entry0.Key != key)
						{
							if (this._entry1.Key == key)
							{
								this._entry1 = this._entry2;
								this._entry2.Key = 0x7fffffff;
								this._entry2.Value = RadProperty.UnsetValue;
								this._count = (ushort)(this._count - 1);
							}
							else if (this._entry2.Key == key)
							{
								this._entry2.Key = 0x7fffffff;
								this._entry2.Value = RadProperty.UnsetValue;
								this._count = (ushort)(this._count - 1);
							}
							return;
						}
						this._entry0 = this._entry1;
						this._entry1 = this._entry2;
						this._entry2.Key = 0x7fffffff;
						this._entry2.Value = RadProperty.UnsetValue;
						this._count = (ushort)(this._count - 1);
						return;
					}
			}
		}

		public override object Search(int key)
		{
			if (this._count > 0)
			{
				if (this._entry0.Key == key)
				{
					return this._entry0.Value;
				}
				if (this._count > 1)
				{
					if (this._entry1.Key == key)
					{
						return this._entry1.Value;
					}
					if ((this._count > 2) && (this._entry2.Key == key))
					{
						return this._entry2.Value;
					}
				}
			}
			return RadProperty.UnsetValue;
		}

		public override void Sort()
		{
			if (!this._sorted && (this._count > 1))
			{
				FrugalMapBase.Entry entry1;
				if (this._entry0.Key > this._entry1.Key)
				{
					entry1 = this._entry0;
					this._entry0 = this._entry1;
					this._entry1 = entry1;
				}
				if ((this._count > 2) && (this._entry1.Key > this._entry2.Key))
				{
					entry1 = this._entry1;
					this._entry1 = this._entry2;
					this._entry2 = entry1;
					if (this._entry0.Key > this._entry1.Key)
					{
						entry1 = this._entry0;
						this._entry0 = this._entry1;
						this._entry1 = entry1;
					}
				}
				this._sorted = true;
			}
		}

		// Properties
		public override int Count
		{
			get
			{
				return this._count;
			}
		}
	}

	internal class FrugalObjectList<T>
	{
		// Fields
		internal FrugalListBase<T> _listStore;

		// Methods
		public FrugalObjectList()
		{
		}

		public FrugalObjectList(int size)
		{
			this.Capacity = size;
		}

		public int Add(T value)
		{
			if (this._listStore == null)
			{
				this._listStore = new SingleItemList<T>();
			}
			FrugalListStoreState state1 = this._listStore.Add(value);
			if (state1 != FrugalListStoreState.Success)
			{
				if (FrugalListStoreState.ThreeItemList == state1)
				{
					ThreeItemList<T> list1 = new ThreeItemList<T>();
					list1.Promote(this._listStore);
					list1.Add(value);
					this._listStore = list1;
				}
				else if (FrugalListStoreState.SixItemList == state1)
				{
					SixItemList<T> list2 = new SixItemList<T>();
					list2.Promote(this._listStore);
					this._listStore = list2;
					list2.Add(value);
					this._listStore = list2;
				}
				else
				{
					if (FrugalListStoreState.Array != state1)
					{
						throw new InvalidOperationException("FrugalList_CannotPromoteBeyondArray");
					}
					ArrayItemList<T> list3 = new ArrayItemList<T>(this._listStore.Count + 1);
					list3.Promote(this._listStore);
					this._listStore = list3;
					list3.Add(value);
					this._listStore = list3;
				}
			}
			return (this._listStore.Count - 1);
		}

		public void Clear()
		{
			if (this._listStore != null)
			{
				this._listStore.Clear();
			}
		}

		public FrugalObjectList<T> Clone()
		{
			FrugalObjectList<T> list1 = new FrugalObjectList<T>();
			if (this._listStore != null)
			{
				list1._listStore = (FrugalListBase<T>)this._listStore.Clone();
			}
			return list1;
		}

		public bool Contains(T value)
		{
			if ((this._listStore != null) && (this._listStore.Count > 0))
			{
				return this._listStore.Contains(value);
			}
			return false;
		}

		public void CopyTo(T[] array, int index)
		{
			if ((this._listStore != null) && (this._listStore.Count > 0))
			{
				this._listStore.CopyTo(array, index);
			}
		}

		public void EnsureIndex(int index)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			int num1 = (index + 1) - this.Count;
			if (num1 > 0)
			{
				this.Capacity = index + 1;
				T local1 = default(T);
				for (int num2 = 0; num2 < num1; num2++)
				{
					this._listStore.Add(local1);
				}
			}
		}

		public int IndexOf(T value)
		{
			if ((this._listStore != null) && (this._listStore.Count > 0))
			{
				return this._listStore.IndexOf(value);
			}
			return -1;
		}

		public void Insert(int index, T value)
		{
			if ((index != 0) && (((this._listStore == null) || (index > this._listStore.Count)) || (index < 0)))
			{
				throw new ArgumentOutOfRangeException("index");
			}
			int num1 = 1;
			if ((this._listStore != null) && (this._listStore.Count == this._listStore.Capacity))
			{
				num1 = this.Capacity + 1;
			}
			this.Capacity = num1;
			this._listStore.Insert(index, value);
		}

		public bool Remove(T value)
		{
			if ((this._listStore != null) && (this._listStore.Count > 0))
			{
				return this._listStore.Remove(value);
			}
			return false;
		}

		public void RemoveAt(int index)
		{
			if (((this._listStore == null) || (index >= this._listStore.Count)) || (index < 0))
			{
				throw new ArgumentOutOfRangeException("index");
			}
			this._listStore.RemoveAt(index);
		}

		public void Sort()
		{
			if ((this._listStore != null) && (this._listStore.Count > 0))
			{
				this._listStore.Sort();
			}
		}

		public T[] ToArray()
		{
			if ((this._listStore != null) && (this._listStore.Count > 0))
			{
				return this._listStore.ToArray();
			}
			return null;
		}

		// Properties
		public int Capacity
		{
			get
			{
				if (this._listStore != null)
				{
					return this._listStore.Capacity;
				}
				return 0;
			}
			set
			{
				int num1 = 0;
				if (this._listStore != null)
				{
					num1 = this._listStore.Capacity;
				}
				if (num1 < value)
				{
					FrugalListBase<T> base1;
					if (value == 1)
					{
						base1 = new SingleItemList<T>();
					}
					else if (value <= 3)
					{
						base1 = new ThreeItemList<T>();
					}
					else if (value <= 6)
					{
						base1 = new SixItemList<T>();
					}
					else
					{
						base1 = new ArrayItemList<T>(value);
					}
					if (this._listStore != null)
					{
						base1.Promote(this._listStore);
					}
					this._listStore = base1;
				}
			}
		}

		public int Count
		{
			get
			{
				if (this._listStore != null)
				{
					return this._listStore.Count;
				}
				return 0;
			}
		}

		public T this[int index]
		{
			get
			{
				if (((this._listStore == null) || (index >= this._listStore.Count)) || (index < 0))
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return this._listStore.EntryAt(index);
			}
			set
			{
				if (((this._listStore == null) || (index >= this._listStore.Count)) || (index < 0))
				{
					throw new ArgumentOutOfRangeException("index");
				}
				this._listStore.SetAt(index, value);
			}
		}

	}


	[StructLayout(LayoutKind.Sequential)]
	internal struct Dependent
	{
		private RadProperty _DP;
		private WeakReference _wrDO;
		private WeakReference _wrEX;

		public override bool Equals(object o)
		{
			if (!(o is Dependent))
			{
				return false;
			}
			Dependent dependent1 = (Dependent)o;
			if (!this.IsValid() || !dependent1.IsValid())
			{
				return false;
			}
			if (this._wrEX.Target != dependent1._wrEX.Target)
			{
				return false;
			}
			if (this._DP != dependent1._DP)
			{
				return false;
			}
			if ((this._wrDO != null) && (dependent1._wrDO != null))
			{
				if (this._wrDO.Target != dependent1._wrDO.Target)
				{
					return false;
				}
			}
			else if ((this._wrDO != null) || (dependent1._wrDO != null))
			{
				return false;
			}
			return true;
		}

		public Dependent(RadObject o, RadProperty p/*, Expression e*/)
		{
			this._wrEX = null; //(e == null) ? null : new WeakReference(e);
			this._DP = p;
			this._wrDO = (o == null) ? null : new WeakReference(o);
		}

		public override int GetHashCode()
		{
			//Expression expression1 = (Expression)this._wrEX.Target;
			int num1 = 0; // (expression1 == null) ? 0 : expression1.GetHashCode();
			if (this._wrDO != null)
			{
				RadObject obj1 = (RadObject)this._wrDO.Target;
				num1 += ((obj1 == null) ? 0 : obj1.GetHashCode());
			}
			return (num1 + ((this._DP == null) ? 0 : this._DP.GetHashCode()));
		}

		public bool IsValid()
		{
			if (!this._wrEX.IsAlive)
			{
				return false;
			}
			if ((this._wrDO != null) && !this._wrDO.IsAlive)
			{
				return false;
			}
			return true;
		}

		public static bool operator ==(Dependent first, Dependent second)
		{
			return first.Equals(second);
		}

		public static bool operator !=(Dependent first, Dependent second)
		{
			return !first.Equals(second);
		}

		public RadObject DO
		{
			get
			{
				if (this._wrDO == null)
				{
					return null;
				}
				return (RadObject)this._wrDO.Target;
			}
		}

		public RadProperty DP
		{
			get
			{
				return this._DP;
			}
		}

		/*public Expression Expr
		{
			get
			{
				if (this._wrEX == null)
				{
					return null;
				}
				return (Expression)this._wrEX.Target;
			}
		}*/
	}


	internal class DependentList : FrugalObjectList<Dependent>
	{
		// Fields
		private static int _skipper;

		// Methods
		public DependentList()
		{
		}

		public void Add(RadObject d, RadProperty dp /*, Expression expr*/)
		{
			if ((++DependentList._skipper % (1 + (base.Count / 4))) == 0)
			{
				this.CleanUpDeadWeakReferences(true);
			}

			Dependent dependent1 = new Dependent(d, dp/*, expr*/);
			base.Add((Dependent)dependent1);
		}

		private void CleanUpDeadWeakReferences(bool doAll)
		{
			if (base.Count != 0)
			{
				Dependent[] dependentArray1 = base.ToArray();
				for (int num1 = dependentArray1.Length - 1; num1 >= 0; num1--)
				{
					if (!dependentArray1[num1].IsValid())
					{
						base.RemoveAt(num1);
					}
					else if (!doAll)
					{
						return;
					}
				}
			}
		}

		public void InvalidateDependents(RadObject source, RadProperty sourceDP)
		{
			Dependent[] dependentArray1 = base.ToArray();
			for (int num1 = 0; num1 < dependentArray1.Length; num1++)
			{
				/*Expression expression1 = null;// dependentArray1[num1].Expr;
				if (expression1 != null)
				{
					expression1.OnPropertyInvalidation(source, sourceDP);
					if (!expression1.ForwardsInvalidations)
					{
						DependencyObject obj1 = dependentArray1[num1].DO;
						DependencyProperty property1 = dependentArray1[num1].DP;
						if ((obj1 != null) && (property1 != null))
						{
							obj1.InvalidateProperty(property1, property1.GetMetadata(obj1.RadObjectType));
						}
					}
				}*/
			}
		}

		public void Remove(RadObject d, RadProperty dp/*, Expression expr*/)
		{
			Dependent dependent1 = new Dependent(d, dp/*, expr*/);
			base.Remove((Dependent)dependent1);
		}

		// Properties
		public bool IsEmpty
		{
			get
			{
				if (base.Count == 0)
				{
					return true;
				}
				this.CleanUpDeadWeakReferences(false);
				return (0 == base.Count);
			}
		}
	}

	internal sealed class SixObjectMap : FrugalMapBase
	{
		// Fields
		private ushort _count;
		private FrugalMapBase.Entry _entry0;
		private FrugalMapBase.Entry _entry1;
		private FrugalMapBase.Entry _entry2;
		private FrugalMapBase.Entry _entry3;
		private FrugalMapBase.Entry _entry4;
		private FrugalMapBase.Entry _entry5;
		private bool _sorted;
		private const int SIZE = 6;

		// Methods
		public SixObjectMap()
		{
		}

		public override void GetKeyValuePair(int index, out int key, out object value)
		{
			if (index < this._count)
			{
				switch (index)
				{
					case 0:
						{
							key = this._entry0.Key;
							value = this._entry0.Value;
							return;
						}
					case 1:
						{
							key = this._entry1.Key;
							value = this._entry1.Value;
							return;
						}
					case 2:
						{
							key = this._entry2.Key;
							value = this._entry2.Value;
							return;
						}
					case 3:
						{
							key = this._entry3.Key;
							value = this._entry3.Value;
							return;
						}
					case 4:
						{
							key = this._entry4.Key;
							value = this._entry4.Value;
							return;
						}
					case 5:
						{
							key = this._entry5.Key;
							value = this._entry5.Value;
							return;
						}
				}
				key = 0x7fffffff;
				value = RadProperty.UnsetValue;
			}
			else
			{
				key = 0x7fffffff;
				value = RadProperty.UnsetValue;
				throw new ArgumentOutOfRangeException("index");
			}
		}

		public override FrugalMapStoreState InsertEntry(int key, object value)
		{
			if (this._count > 0)
			{
				if (this._entry0.Key == key)
				{
					this._entry0.Value = value;
					return FrugalMapStoreState.Success;
				}
				if (this._count > 1)
				{
					if (this._entry1.Key == key)
					{
						this._entry1.Value = value;
						return FrugalMapStoreState.Success;
					}
					if (this._count > 2)
					{
						if (this._entry2.Key == key)
						{
							this._entry2.Value = value;
							return FrugalMapStoreState.Success;
						}
						if (this._count > 3)
						{
							if (this._entry3.Key == key)
							{
								this._entry3.Value = value;
								return FrugalMapStoreState.Success;
							}
							if (this._count > 4)
							{
								if (this._entry4.Key == key)
								{
									this._entry4.Value = value;
									return FrugalMapStoreState.Success;
								}
								if ((this._count > 5) && (this._entry5.Key == key))
								{
									this._entry5.Value = value;
									return FrugalMapStoreState.Success;
								}
							}
						}
					}
				}
			}
			if (6 <= this._count)
			{
				return FrugalMapStoreState.Array;
			}
			this._sorted = false;
			switch (this._count)
			{
				case 0:
					{
						this._entry0.Key = key;
						this._entry0.Value = value;
						this._sorted = true;
						break;
					}
				case 1:
					{
						this._entry1.Key = key;
						this._entry1.Value = value;
						break;
					}
				case 2:
					{
						this._entry2.Key = key;
						this._entry2.Value = value;
						break;
					}
				case 3:
					{
						this._entry3.Key = key;
						this._entry3.Value = value;
						break;
					}
				case 4:
					{
						this._entry4.Key = key;
						this._entry4.Value = value;
						break;
					}
				case 5:
					{
						this._entry5.Key = key;
						this._entry5.Value = value;
						break;
					}
			}
			this._count = (ushort)(this._count + 1);
			return FrugalMapStoreState.Success;
		}

		public override void Iterate(ArrayList list, FrugalMapIterationCallback callback)
		{
			if (this._count > 0)
			{
				if (this._count >= 1)
				{
					callback(list, this._entry0.Key, this._entry0.Value);
				}
				if (this._count >= 2)
				{
					callback(list, this._entry1.Key, this._entry1.Value);
				}
				if (this._count >= 3)
				{
					callback(list, this._entry2.Key, this._entry2.Value);
				}
				if (this._count >= 4)
				{
					callback(list, this._entry3.Key, this._entry3.Value);
				}
				if (this._count >= 5)
				{
					callback(list, this._entry4.Key, this._entry4.Value);
				}
				if (this._count == 6)
				{
					callback(list, this._entry5.Key, this._entry5.Value);
				}
			}
		}

		public override void Promote(FrugalMapBase newMap)
		{
			if (newMap.InsertEntry(this._entry0.Key, this._entry0.Value) != FrugalMapStoreState.Success)
			{
				object[] objArray1 = new object[] { this.ToString(), newMap.ToString(), "newMap" };
				throw new ArgumentException(string.Format("FrugalMap_TargetMapCannotHoldAllData: {0}, {1}, {2}", objArray1));
			}
			if (newMap.InsertEntry(this._entry1.Key, this._entry1.Value) != FrugalMapStoreState.Success)
			{
				object[] objArray1 = new object[] { this.ToString(), newMap.ToString(), "newMap" };
				throw new ArgumentException(string.Format("FrugalMap_TargetMapCannotHoldAllData: {0}, {1}, {2}", objArray1));
			}
			if (newMap.InsertEntry(this._entry2.Key, this._entry2.Value) != FrugalMapStoreState.Success)
			{
				object[] objArray1 = new object[] { this.ToString(), newMap.ToString(), "newMap" };
				throw new ArgumentException(string.Format("FrugalMap_TargetMapCannotHoldAllData: {0}, {1}, {2}", objArray1));
			}
			if (newMap.InsertEntry(this._entry3.Key, this._entry3.Value) != FrugalMapStoreState.Success)
			{
				object[] objArray1 = new object[] { this.ToString(), newMap.ToString(), "newMap" };
				throw new ArgumentException(string.Format("FrugalMap_TargetMapCannotHoldAllData: {0}, {1}, {2}", objArray1));
			}
			if (newMap.InsertEntry(this._entry4.Key, this._entry4.Value) != FrugalMapStoreState.Success)
			{
				object[] objArray1 = new object[] { this.ToString(), newMap.ToString(), "newMap" };
				throw new ArgumentException(string.Format("FrugalMap_TargetMapCannotHoldAllData: {0}, {1}, {2}", objArray1));
			}
			if (newMap.InsertEntry(this._entry5.Key, this._entry5.Value) != FrugalMapStoreState.Success)
			{
				object[] objArray1 = new object[] { this.ToString(), newMap.ToString(), "newMap" };
				throw new ArgumentException(string.Format("FrugalMap_TargetMapCannotHoldAllData: {0}, {1}, {2}", objArray1));
			}
		}

		public override void RemoveEntry(int key)
		{
			switch (this._count)
			{
				case 1:
					{
						if (this._entry0.Key == key)
						{
							this._entry0.Key = 0x7fffffff;
							this._entry0.Value = RadProperty.UnsetValue;
							this._count = (ushort)(this._count - 1);
						}
						return;
					}
				case 2:
					{
						if (this._entry0.Key != key)
						{
							if (this._entry1.Key == key)
							{
								this._entry1.Key = 0x7fffffff;
								this._entry1.Value = RadProperty.UnsetValue;
								this._count = (ushort)(this._count - 1);
							}
							return;
						}
						this._entry0 = this._entry1;
						this._entry1.Key = 0x7fffffff;
						this._entry1.Value = RadProperty.UnsetValue;
						this._count = (ushort)(this._count - 1);
						return;
					}
				case 3:
					{
						if (this._entry0.Key != key)
						{
							if (this._entry1.Key == key)
							{
								this._entry1 = this._entry2;
								this._entry2.Key = 0x7fffffff;
								this._entry2.Value = RadProperty.UnsetValue;
								this._count = (ushort)(this._count - 1);
								return;
							}
							if (this._entry2.Key == key)
							{
								this._entry2.Key = 0x7fffffff;
								this._entry2.Value = RadProperty.UnsetValue;
								this._count = (ushort)(this._count - 1);
							}
							return;
						}
						this._entry0 = this._entry1;
						this._entry1 = this._entry2;
						this._entry2.Key = 0x7fffffff;
						this._entry2.Value = RadProperty.UnsetValue;
						this._count = (ushort)(this._count - 1);
						return;
					}
				case 4:
					{
						if (this._entry0.Key != key)
						{
							if (this._entry1.Key == key)
							{
								this._entry1 = this._entry2;
								this._entry2 = this._entry3;
								this._entry3.Key = 0x7fffffff;
								this._entry3.Value = RadProperty.UnsetValue;
								this._count = (ushort)(this._count - 1);
								return;
							}
							if (this._entry2.Key == key)
							{
								this._entry2 = this._entry3;
								this._entry3.Key = 0x7fffffff;
								this._entry3.Value = RadProperty.UnsetValue;
								this._count = (ushort)(this._count - 1);
								return;
							}
							if (this._entry3.Key == key)
							{
								this._entry3.Key = 0x7fffffff;
								this._entry3.Value = RadProperty.UnsetValue;
								this._count = (ushort)(this._count - 1);
							}
							return;
						}
						this._entry0 = this._entry1;
						this._entry1 = this._entry2;
						this._entry2 = this._entry3;
						this._entry3.Key = 0x7fffffff;
						this._entry3.Value = RadProperty.UnsetValue;
						this._count = (ushort)(this._count - 1);
						return;
					}
				case 5:
					{
						if (this._entry0.Key != key)
						{
							if (this._entry1.Key == key)
							{
								this._entry1 = this._entry2;
								this._entry2 = this._entry3;
								this._entry3 = this._entry4;
								this._entry4.Key = 0x7fffffff;
								this._entry4.Value = RadProperty.UnsetValue;
								this._count = (ushort)(this._count - 1);
								return;
							}
							if (this._entry2.Key == key)
							{
								this._entry2 = this._entry3;
								this._entry3 = this._entry4;
								this._entry4.Key = 0x7fffffff;
								this._entry4.Value = RadProperty.UnsetValue;
								this._count = (ushort)(this._count - 1);
								return;
							}
							if (this._entry3.Key == key)
							{
								this._entry3 = this._entry4;
								this._entry4.Key = 0x7fffffff;
								this._entry4.Value = RadProperty.UnsetValue;
								this._count = (ushort)(this._count - 1);
								return;
							}
							if (this._entry4.Key == key)
							{
								this._entry4.Key = 0x7fffffff;
								this._entry4.Value = RadProperty.UnsetValue;
								this._count = (ushort)(this._count - 1);
							}
							return;
						}
						this._entry0 = this._entry1;
						this._entry1 = this._entry2;
						this._entry2 = this._entry3;
						this._entry3 = this._entry4;
						this._entry4.Key = 0x7fffffff;
						this._entry4.Value = RadProperty.UnsetValue;
						this._count = (ushort)(this._count - 1);
						return;
					}
				case 6:
					{
						if (this._entry0.Key != key)
						{
							if (this._entry1.Key == key)
							{
								this._entry1 = this._entry2;
								this._entry2 = this._entry3;
								this._entry3 = this._entry4;
								this._entry4 = this._entry5;
								this._entry5.Key = 0x7fffffff;
								this._entry5.Value = RadProperty.UnsetValue;
								this._count = (ushort)(this._count - 1);
							}
							else if (this._entry2.Key == key)
							{
								this._entry2 = this._entry3;
								this._entry3 = this._entry4;
								this._entry4 = this._entry5;
								this._entry5.Key = 0x7fffffff;
								this._entry5.Value = RadProperty.UnsetValue;
								this._count = (ushort)(this._count - 1);
							}
							else if (this._entry3.Key == key)
							{
								this._entry3 = this._entry4;
								this._entry4 = this._entry5;
								this._entry5.Key = 0x7fffffff;
								this._entry5.Value = RadProperty.UnsetValue;
								this._count = (ushort)(this._count - 1);
							}
							else if (this._entry4.Key == key)
							{
								this._entry4 = this._entry5;
								this._entry5.Key = 0x7fffffff;
								this._entry5.Value = RadProperty.UnsetValue;
								this._count = (ushort)(this._count - 1);
							}
							else if (this._entry5.Key == key)
							{
								this._entry5.Key = 0x7fffffff;
								this._entry5.Value = RadProperty.UnsetValue;
								this._count = (ushort)(this._count - 1);
							}
							return;
						}
						this._entry0 = this._entry1;
						this._entry1 = this._entry2;
						this._entry2 = this._entry3;
						this._entry3 = this._entry4;
						this._entry4 = this._entry5;
						this._entry5.Key = 0x7fffffff;
						this._entry5.Value = RadProperty.UnsetValue;
						this._count = (ushort)(this._count - 1);
						return;
					}
			}
		}

		public override object Search(int key)
		{
			if (this._count > 0)
			{
				if (this._entry0.Key == key)
				{
					return this._entry0.Value;
				}
				if (this._count > 1)
				{
					if (this._entry1.Key == key)
					{
						return this._entry1.Value;
					}
					if (this._count > 2)
					{
						if (this._entry2.Key == key)
						{
							return this._entry2.Value;
						}
						if (this._count > 3)
						{
							if (this._entry3.Key == key)
							{
								return this._entry3.Value;
							}
							if (this._count > 4)
							{
								if (this._entry4.Key == key)
								{
									return this._entry4.Value;
								}
								if ((this._count > 5) && (this._entry5.Key == key))
								{
									return this._entry5.Value;
								}
							}
						}
					}
				}
			}
			return RadProperty.UnsetValue;
		}

		public override void Sort()
		{
			if (!this._sorted && (this._count > 1))
			{
				bool flag1;
				do
				{
					FrugalMapBase.Entry entry1;
					flag1 = false;
					if (this._entry0.Key > this._entry1.Key)
					{
						entry1 = this._entry0;
						this._entry0 = this._entry1;
						this._entry1 = entry1;
						flag1 = true;
					}
					if (this._count > 2)
					{
						if (this._entry1.Key > this._entry2.Key)
						{
							entry1 = this._entry1;
							this._entry1 = this._entry2;
							this._entry2 = entry1;
							flag1 = true;
						}
						if (this._count > 3)
						{
							if (this._entry2.Key > this._entry3.Key)
							{
								entry1 = this._entry2;
								this._entry2 = this._entry3;
								this._entry3 = entry1;
								flag1 = true;
							}
							if (this._count > 4)
							{
								if (this._entry3.Key > this._entry4.Key)
								{
									entry1 = this._entry3;
									this._entry3 = this._entry4;
									this._entry4 = entry1;
									flag1 = true;
								}
								if ((this._count > 5) && (this._entry4.Key > this._entry5.Key))
								{
									entry1 = this._entry4;
									this._entry4 = this._entry5;
									this._entry5 = entry1;
									flag1 = true;
								}
							}
						}
					}
				}
				while (flag1);
				this._sorted = true;
			}
		}

		// Properties
		public override int Count
		{
			get
			{
				return this._count;
			}
		}
	}

	internal sealed class SortedObjectMap : FrugalMapBase
	{
		// Fields
		internal int _count;
		private FrugalMapBase.Entry[] _entries;
		private int _lastKey;
		private const int GROWTH = 8;
		private const int MAXSIZE = 0x80;
		private const int MINSIZE = 0x10;

		// Methods
		public SortedObjectMap()
		{
			this._lastKey = 0x7fffffff;
		}

		private int FindInsertIndex(int key, out bool found)
		{
			int num1 = 0;
			if ((this._count > 0) && (key <= this._lastKey))
			{
				int num2 = this._count - 1;
				do
				{
					int num3 = (num2 + num1) / 2;
					if (key <= this._entries[num3].Key)
					{
						num2 = num3;
					}
					else
					{
						num1 = num3 + 1;
					}
				}
				while (num1 < num2);
				found = key == this._entries[num1].Key;
				return num1;
			}
			num1 = this._count;
			found = false;
			return num1;
		}

		public override void GetKeyValuePair(int index, out int key, out object value)
		{
			if (index < this._count)
			{
				value = this._entries[index].Value;
				key = this._entries[index].Key;
			}
			else
			{
				value = RadProperty.UnsetValue;
				key = 0x7fffffff;
				throw new ArgumentOutOfRangeException("index");
			}
		}

		public override FrugalMapStoreState InsertEntry(int key, object value)
		{
			bool flag1;
			int num1 = this.FindInsertIndex(key, out flag1);
			if (flag1)
			{
				this._entries[num1].Value = value;
				return FrugalMapStoreState.Success;
			}
			if (0x80 <= this._count)
			{
				return FrugalMapStoreState.Hashtable;
			}
			if (this._entries != null)
			{
				if (this._entries.Length <= this._count)
				{
					FrugalMapBase.Entry[] entryArray1 = new FrugalMapBase.Entry[this._entries.Length + 8];
					Array.Copy(this._entries, 0, entryArray1, 0, this._entries.Length);
					this._entries = entryArray1;
				}
			}
			else
			{
				this._entries = new FrugalMapBase.Entry[0x10];
			}
			if (num1 < this._count)
			{
				Array.Copy(this._entries, num1, this._entries, (int)(num1 + 1), (int)(this._count - num1));
			}
			else
			{
				this._lastKey = key;
			}
			this._entries[num1].Key = key;
			this._entries[num1].Value = value;
			this._count++;
			return FrugalMapStoreState.Success;
		}

		public override void Iterate(ArrayList list, FrugalMapIterationCallback callback)
		{
			if (this._count > 0)
			{
				for (int num1 = 0; num1 < this._count; num1++)
				{
					callback(list, this._entries[num1].Key, this._entries[num1].Value);
				}
			}
		}

		public override void Promote(FrugalMapBase newMap)
		{
			for (int num1 = 0; num1 < this._entries.Length; num1++)
			{
				if (newMap.InsertEntry(this._entries[num1].Key, this._entries[num1].Value) != FrugalMapStoreState.Success)
				{
					object[] objArray1 = new object[] { this.ToString(), newMap.ToString(), "newMap" };
					throw new ArgumentException(string.Format("FrugalMap_TargetMapCannotHoldAllData: {0}, {1}, {2}", objArray1));
				}
			}
		}

		public override void RemoveEntry(int key)
		{
			bool flag1;
			int num1 = this.FindInsertIndex(key, out flag1);
			if (flag1)
			{
				int num2 = (this._count - num1) - 1;
				if (num2 > 0)
				{
					Array.Copy(this._entries, (int)(num1 + 1), this._entries, num1, num2);
				}
				else if (this._count > 1)
				{
					this._lastKey = this._entries[this._count - 2].Key;
				}
				else
				{
					this._lastKey = 0x7fffffff;
				}
				this._entries[this._count - 1].Key = 0x7fffffff;
				this._entries[this._count - 1].Value = RadProperty.UnsetValue;
				this._count--;
			}
		}

		public override object Search(int key)
		{
			bool flag1;
			int num1 = this.FindInsertIndex(key, out flag1);
			if (flag1)
			{
				return this._entries[num1].Value;
			}
			return RadProperty.UnsetValue;
		}

		public override void Sort()
		{
		}

		// Properties
		public override int Count
		{
			get
			{
				return this._count;
			}
		}
	}

	internal class ArrayItemList<T> : FrugalListBase<T>
	{
		// Fields
		private T[] _entries;
		private const int GROWTH = 3;
		private const int LARGEGROWTH = 0x12;
		private const int MINSIZE = 9;

		// Methods
		public ArrayItemList()
		{
		}

		public ArrayItemList(int size)
		{
			size += 2;
			size -= (size % 3);
			this._entries = new T[size];
		}

		public override FrugalListStoreState Add(T value)
		{
			if ((this._entries != null) && (base._count < this._entries.Length))
			{
				this._entries[base._count] = value;
				base._count++;
			}
			else
			{
				if (this._entries != null)
				{
					int num1 = this._entries.Length;
					if (num1 < 0x12)
					{
						num1 += 3;
					}
					else
					{
						num1 += (num1 >> 2);
					}
					T[] localArray1 = new T[num1];
					Array.Copy(this._entries, 0, localArray1, 0, this._entries.Length);
					this._entries = localArray1;
				}
				else
				{
					this._entries = new T[9];
				}
				this._entries[base._count] = value;
				base._count++;
			}
			return FrugalListStoreState.Success;
		}

		public override void Clear()
		{
			for (int num1 = 0; num1 < base._count; num1++)
			{
				T local1 = default(T);
				this._entries[num1] = local1;
			}
			base._count = 0;
		}

		public override object Clone()
		{
			ArrayItemList<T> list1 = new ArrayItemList<T>(this.Capacity);
			list1.Promote(this);
			return list1;
		}

		public override bool Contains(T value)
		{
			return (-1 != this.IndexOf(value));
		}

		public override void CopyTo(T[] array, int index)
		{
			for (int num1 = 0; num1 < base._count; num1++)
			{
				array[index + num1] = this._entries[num1];
			}
		}

		public override T EntryAt(int index)
		{
			return this._entries[index];
		}

		public override int IndexOf(T value)
		{
			for (int num1 = 0; num1 < base._count; num1++)
			{
				if (this._entries[num1].Equals(value))
				{
					return num1;
				}
			}
			return -1;
		}

		public override void Insert(int index, T value)
		{
			if ((this._entries == null) || (base._count >= this._entries.Length))
			{
				throw new ArgumentOutOfRangeException("index");
			}
			Array.Copy(this._entries, index, this._entries, (int)(index + 1), (int)(base._count - index));
			this._entries[index] = value;
			base._count++;
		}

		public void Promote(ArrayItemList<T> oldList)
		{
			int num1 = oldList.Count;
			if (this._entries.Length >= num1)
			{
				this.SetCount(oldList.Count);
				for (int num2 = 0; num2 < num1; num2++)
				{
					this.SetAt(num2, oldList.EntryAt(num2));
				}
			}
			else
			{
				object[] objArray1 = new object[] { oldList.ToString(), this.ToString(), "oldList" };
				throw new ArgumentException(string.Format("FrugalList_TargetMapCannotHoldAllData", objArray1));
			}
		}

		public override void Promote(FrugalListBase<T> oldList)
		{
			for (int num1 = 0; num1 < oldList.Count; num1++)
			{
				if (this.Add(oldList.EntryAt(num1)) != FrugalListStoreState.Success)
				{
					object[] objArray1 = new object[] { oldList.ToString(), this.ToString(), "oldList" };
					throw new ArgumentException(string.Format("FrugalList_TargetMapCannotHoldAllData", objArray1));
				}
			}
		}

		public void Promote(SixItemList<T> oldList)
		{
			int num1 = oldList.Count;
			this.SetCount(oldList.Count);
			switch (num1)
			{
				case 0:
					{
						return;
					}
				case 1:
					{
						this.SetAt(0, oldList.EntryAt(0));
						return;
					}
				case 2:
					{
						this.SetAt(0, oldList.EntryAt(0));
						this.SetAt(1, oldList.EntryAt(1));
						return;
					}
				case 3:
					{
						this.SetAt(0, oldList.EntryAt(0));
						this.SetAt(1, oldList.EntryAt(1));
						this.SetAt(2, oldList.EntryAt(2));
						return;
					}
				case 4:
					{
						this.SetAt(0, oldList.EntryAt(0));
						this.SetAt(1, oldList.EntryAt(1));
						this.SetAt(2, oldList.EntryAt(2));
						this.SetAt(3, oldList.EntryAt(3));
						return;
					}
				case 5:
					{
						this.SetAt(0, oldList.EntryAt(0));
						this.SetAt(1, oldList.EntryAt(1));
						this.SetAt(2, oldList.EntryAt(2));
						this.SetAt(3, oldList.EntryAt(3));
						this.SetAt(4, oldList.EntryAt(4));
						return;
					}
				case 6:
					{
						this.SetAt(0, oldList.EntryAt(0));
						this.SetAt(1, oldList.EntryAt(1));
						this.SetAt(2, oldList.EntryAt(2));
						this.SetAt(3, oldList.EntryAt(3));
						this.SetAt(4, oldList.EntryAt(4));
						this.SetAt(5, oldList.EntryAt(5));
						return;
					}
			}
			throw new ArgumentOutOfRangeException("index");
		}

		public override bool Remove(T value)
		{
			for (int num1 = 0; num1 < base._count; num1++)
			{
				if (this._entries[num1].Equals(value))
				{
					this.RemoveAt(num1);
					return true;
				}
			}
			return false;
		}

		public override void RemoveAt(int index)
		{
			int num1 = (base._count - index) - 1;
			if (num1 > 0)
			{
				Array.Copy(this._entries, (int)(index + 1), this._entries, index, num1);
			}
			this._entries[base._count - 1] = default(T);
			base._count--;
		}

		public override void SetAt(int index, T value)
		{
			this._entries[index] = value;
		}

		private void SetCount(int value)
		{
			if ((value < 0) || (value > this._entries.Length))
			{
				throw new ArgumentOutOfRangeException("Count");
			}
			base._count = value;
		}

		public override void Sort()
		{
			Array.Sort<T>(this._entries, 0, base._count);
		}

		public override T[] ToArray()
		{
			T[] localArray1 = new T[base._count];
			for (int num1 = 0; num1 < base._count; num1++)
			{
				localArray1[num1] = this._entries[num1];
			}
			return localArray1;
		}

		// Properties
		public override int Capacity
		{
			get
			{
				if (this._entries != null)
				{
					return this._entries.Length;
				}
				return 0;
			}
		}
	}

	internal class ArrayObjectMap : FrugalMapBase
	{
		// Methods
		public ArrayObjectMap()
		{
		}

		private int Compare(int left, int right)
		{
			return (this._entries[left].Key - this._entries[right].Key);
		}

		public override void GetKeyValuePair(int index, out int key, out object value)
		{
			if (index < this._count)
			{
				value = this._entries[index].Value;
				key = this._entries[index].Key;
			}
			else
			{
				value = RadProperty.UnsetValue;
				key = 0x7fffffff;
				throw new ArgumentOutOfRangeException("index");
			}
		}

		public override FrugalMapStoreState InsertEntry(int key, object value)
		{
			for (int num1 = 0; num1 < this._count; num1++)
			{
				if (this._entries[num1].Key == key)
				{
					this._entries[num1].Value = value;
					return FrugalMapStoreState.Success;
				}
			}
			if (15 <= this._count)
			{
				return FrugalMapStoreState.SortedArray;
			}
			if (this._entries != null)
			{
				this._sorted = false;
				if (this._entries.Length <= this._count)
				{
					FrugalMapBase.Entry[] entryArray1 = new FrugalMapBase.Entry[this._entries.Length + 3];
					Array.Copy(this._entries, 0, entryArray1, 0, this._entries.Length);
					this._entries = entryArray1;
				}
			}
			else
			{
				this._entries = new FrugalMapBase.Entry[9];
				this._sorted = true;
			}
			this._entries[this._count].Key = key;
			this._entries[this._count].Value = value;
			this._count = (ushort)(this._count + 1);
			return FrugalMapStoreState.Success;
		}

		public override void Iterate(ArrayList list, FrugalMapIterationCallback callback)
		{
			if (this._count > 0)
			{
				for (int num1 = 0; num1 < this._count; num1++)
				{
					callback(list, this._entries[num1].Key, this._entries[num1].Value);
				}
			}
		}

		private int Partition(int left, int right)
		{
			FrugalMapBase.Entry entry1;
			int num1 = right;
			int num2 = left - 1;
			int num3 = right;
		Label_0008:
			if (this.Compare(++num2, num1) < 0)
			{
				goto Label_0008;
			}
			while (this.Compare(num1, --num3) < 0)
			{
				if (num3 == left)
				{
					break;
				}
			}
			if (num2 < num3)
			{
				entry1 = this._entries[num3];
				this._entries[num3] = this._entries[num2];
				this._entries[num2] = entry1;
				goto Label_0008;
			}
			entry1 = this._entries[right];
			this._entries[right] = this._entries[num2];
			this._entries[num2] = entry1;
			return num2;
		}

		public override void Promote(FrugalMapBase newMap)
		{
			for (int num1 = 0; num1 < this._entries.Length; num1++)
			{
				if (newMap.InsertEntry(this._entries[num1].Key, this._entries[num1].Value) != FrugalMapStoreState.Success)
				{
					object[] objArray1 = new object[] { this.ToString(), newMap.ToString(), "newMap" };
					throw new ArgumentException(string.Format("FrugalMap_TargetMapCannotHoldAllData", objArray1));
				}
			}
		}

		private void QSort(int left, int right)
		{
			if (left < right)
			{
				int num1 = this.Partition(left, right);
				this.QSort(left, num1 - 1);
				this.QSort(num1 + 1, right);
			}
		}

		public override void RemoveEntry(int key)
		{
			for (int num1 = 0; num1 < this._count; num1++)
			{
				if (this._entries[num1].Key == key)
				{
					int num2 = (this._count - num1) - 1;
					if (num2 > 0)
					{
						Array.Copy(this._entries, (int)(num1 + 1), this._entries, num1, num2);
					}
					this._entries[this._count - 1].Key = 0x7fffffff;
					this._entries[this._count - 1].Value = RadProperty.UnsetValue;
					this._count = (ushort)(this._count - 1);
					return;
				}
			}
		}

		public override object Search(int key)
		{
			for (int num1 = 0; num1 < this._count; num1++)
			{
				if (key == this._entries[num1].Key)
				{
					return this._entries[num1].Value;
				}
			}
			return RadProperty.UnsetValue;
		}

		public override void Sort()
		{
			if (!this._sorted && (this._count > 1))
			{
				this.QSort(0, this._count - 1);
				this._sorted = true;
			}
		}

		// Properties
		public override int Count
		{
			get
			{
				return this._count;
			}
		}

		// Fields
		private ushort _count;
		private FrugalMapBase.Entry[] _entries;
		private bool _sorted;
		private const int GROWTH = 3;
		private const int MAXSIZE = 15;
		private const int MINSIZE = 9;
	}

	internal class HashObjectMap : ArrayObjectMap
	{
	}

	internal class ThreeItemList<T> : ArrayItemList<T>
	{
	}

	internal class SixItemList<T> : ArrayItemList<T>
	{
	}

	internal class SingleItemList<T> : ArrayItemList<T>
	{ }

	internal class HashItemList<T> : ArrayItemList<T>
	{ }
}