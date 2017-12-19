using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Telerik.WinControls.Themes;

namespace Telerik.WinControls.Styles
{
    public class XmlStyleRepository
    {
        private List<XmlRepositoryItem> xmlRepositoryItems = new List<XmlRepositoryItem>();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<XmlRepositoryItem> RepositoryItems
        {
            get
            {
                return this.xmlRepositoryItems;
            }
        }

        public void MergeWith(XmlStyleRepository styleRepository)
        {
            LinkedList<XmlRepositoryItem> newItems = new LinkedList<XmlRepositoryItem>();

            for (int i = 0; i < styleRepository.xmlRepositoryItems.Count; i++)
            {
                XmlRepositoryItem newList = styleRepository.xmlRepositoryItems[i];

                bool found = false;
                for (int k = 0; k < this.xmlRepositoryItems.Count; k++)
                {
                    XmlRepositoryItem existingList = this.xmlRepositoryItems[k];
                    if (existingList.Key == newList.Key)
                    {
                        this.xmlRepositoryItems[k] = styleRepository.xmlRepositoryItems[i];
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    newItems.AddLast(newList);
                }
            }

            this.xmlRepositoryItems.AddRange(newItems);
        }
    }
}
