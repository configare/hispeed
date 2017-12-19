using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.FileProject;
using System.Collections;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    public class BlockDefined
    {
        private const int MixCount = 0;//预定义区域区间【不可更改】
        private List<BlockItemGroup> _blockItemGroups = new List<BlockItemGroup>();

        public BlockItemGroup[] BlockItemGroups
        {
            get { return _blockItemGroups.ToArray(); }
        }

        public BlockItemGroup[] UserDefineRegion
        {
            get
            {
                List<BlockItemGroup> groups = new List<BlockItemGroup>();
                if (_blockItemGroups == null || _blockItemGroups.Count <= MixCount)
                    return null;
                for (int i = MixCount; i < _blockItemGroups.Count; i++)
                {
                    BlockItemGroup group = _blockItemGroups[i];
                    groups.Add(group);
                }
                return groups.ToArray();
            }
        }

        public BlockDefined()
        { }

        public BlockDefined(BlockItemGroup[] blockItemGroups)
        {
            if (blockItemGroups != null && blockItemGroups.Length != 0)
                this._blockItemGroups.AddRange(blockItemGroups);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public BlockItemGroup FindGroup(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName) || BlockItemGroups == null)
                return null;
            return BlockItemGroups.Single(item => { return (item.Name == groupName); });
        }

        public void Clear()
        {
            this._blockItemGroups.Clear();
        }

        private bool ExistGroup(string groupName)
        {
            return this._blockItemGroups.Exists(item => { return (item.Name == groupName); });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item">
        /// 要添加的group
        /// 如果此group的名字已经存在，则覆盖之前同名的group。
        /// </param>
        public void Add(BlockItemGroup item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Name))
                return;
            if (ExistGroup(item.Name))
            {
                BlockItemGroup gp = FindGroup(item.Name);
                gp.BlockItems = item.BlockItems;
            }
            else
            {
                this._blockItemGroups.Add(item);
            }
        }

        public void AddRange(BlockItemGroup[] items)
        {
            foreach (BlockItemGroup item in items)
            {
                Add(item);
            }
        }

        public PrjEnvelopeItem[] GetPrjEnvelopeItems()
        {
            List<PrjEnvelopeItem> items = new List<PrjEnvelopeItem>();
            foreach (BlockItemGroup item in _blockItemGroups)
            {
                items.AddRange(item.BlockItems);
            }
            return items.ToArray();
        }

        internal void SetUserDefineRegion(BlockItemGroup[] userDefineGroups)
        {
            List<BlockItemGroup> newGroups = new List<BlockItemGroup>();
            int maxCount = _blockItemGroups.Count > MixCount ? MixCount : _blockItemGroups.Count;
            for (int i = 0; i < maxCount; i++)
            {
                newGroups.Add(_blockItemGroups[i]);
            }
            newGroups.AddRange(userDefineGroups);
            _blockItemGroups = newGroups;
        }
    }
}