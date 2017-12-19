using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public class WktItem
    {
        public string Name = null;
        public string Value = null;
        public List<WktItem> Items = new List<WktItem>();

        public WktItem()
        { 
        }

        public WktItem(string itemstring)
        {
            Name = itemstring.Substring(0, itemstring.IndexOf('['));
            Value = itemstring.Replace(Name, string.Empty).Replace("[", string.Empty).Replace("]", string.Empty);
        }

        public void Add(WktItem it)
        {
            Items.Add(it);
        }

        public WktItem GetWktItem(string name)
        {
            WktItem item = null;
            GetWktItem(name, this,ref item);
            return item;
        }

        private void GetWktItem(string name, WktItem beginWktItem, ref WktItem item)
        {
            if (beginWktItem.Name.ToUpper() == name.ToUpper())
            {
                item = beginWktItem;
                return;
            }
            if (beginWktItem.Items.Count > 0)
                foreach (WktItem it in beginWktItem.Items)
                    GetWktItem(name, it, ref item);
        }

        public WktItem[] GetWktItems(string name)
        {
            List<WktItem> retItems = new List<WktItem>();
            GetWktItem(name, this, retItems);
            return retItems.Count > 0 ? retItems.ToArray() : null;
        }

        private void GetWktItem(string name, WktItem beginWktItem, List<WktItem> items)
        {
            if (beginWktItem.Name.ToUpper() == name.ToUpper())
            {
                items.Add(beginWktItem);
            }
            if (beginWktItem.Items.Count > 0)
                foreach (WktItem it in beginWktItem.Items)
                    GetWktItem(name, it, items);
        }

      
        public override string ToString()
        {
            return Name+ "["+ Value + "]";
        }
    }

    public class WktSpliter
    {
        public WktItem Split(string wkt)
        {
            Stack<char> stack = new Stack<char>();
            List<WktItem> subitems = new List<WktItem>();
            bool hasLeft = false; 
            for (int i = 0; i < wkt.Length; i++)
            {
                stack.Push(wkt[i]);
                if (wkt[i] == ']')
                {
                    string str = null;
                    bool passLeft = false;
                    do
                    {
                        char c = stack.Pop();
                        if (c == '[')
                            passLeft = true;
                        str = c + str;
                    }
                    while (!(stack.Count == 0 || (passLeft && stack.Peek() == ',')));
                    //
                    WktItem it = new WktItem(str);
                    if (!hasLeft)
                    {
                        it.Items.AddRange(subitems);
                        subitems.Clear();
                    }
                    subitems.Add(it);
                    hasLeft = false;
                }
                else if (wkt[i] == '[')
                {
                    hasLeft = true;
                }
            }
            return subitems[0];
        }
    }
}
