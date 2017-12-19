using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.FileProject;
using System.Collections;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    public class BlockItemGroup
    {
        private List<PrjEnvelopeItem> _blockItems = new List<PrjEnvelopeItem>();

        public string Name = "";
        public string Description = "";
        public string Identify = "";
        public PrjEnvelopeItem[] BlockItems
        {
            get { return _blockItems.ToArray(); }
            set { _blockItems = new List<PrjEnvelopeItem>(value); }
        }

        public BlockItemGroup(string name)
        {
            Name = name;
        }

        public BlockItemGroup(string name, string description)
        {
            Name = name;
            Description = description;
        }
        
        public BlockItemGroup(string name, string description,string identify)
        {
            Name = name;
            Description = description;
            Identify = identify;
        }

        public BlockItemGroup(string name, string description, PrjEnvelopeItem[] items)
        {
            Name = name;
            Description = description;
            if (items != null && items.Length != 0)
                _blockItems.AddRange(items);
        }

        public BlockItemGroup(string name, string description, string identify,PrjEnvelopeItem[] items)
        {
            Name = name;
            Description = description;
            Identify = identify;
            if (items != null && items.Length != 0)
                _blockItems.AddRange(items);
        }

        public void Add(PrjEnvelopeItem item)
        {
            _blockItems.Add(item);
        }

        public void Remove(PrjEnvelopeItem item)
        {
            _blockItems.Remove(item);
        }

        public void AddRange(PrjEnvelopeItem[] items)
        {
            _blockItems.AddRange(items);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Envelope的Name，如：S01,S02等等</param>
        /// <returns></returns>
        public PrjEnvelopeItem GetPrjEnvelopeItem(string name)
        {
            return _blockItems.Find((item) => { return item.Name == name; });
        }

        public override string ToString()
        {
            return string.Format("{0}[{1}]({2})", Name, Description, _blockItems.Count);
        }
    }
}