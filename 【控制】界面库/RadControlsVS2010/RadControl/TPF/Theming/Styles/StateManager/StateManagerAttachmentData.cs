using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Telerik.WinControls.Styles
{
    public class StateManagerAttachmentData : IDisposable
    {
        #region methods

        public StateManagerAttachmentData(RadItem elementToAttachTo, RadItemStateChangedEventHandler eventHandler)
        {
            this.eventHandler = eventHandler;
            this.elementToAttachTo = elementToAttachTo;
            elementToAttachTo.RadPropertyChanged += new RadPropertyChangedEventHandler(attachedElement_RadPropertyChanged);
        }

        public void AddEventHandlers(List<RadProperty> properties)
        {
            if (this.affectedProperties == null)
            {
                this.affectedProperties = new List<RadProperty>();
            }

            this.affectedProperties.AddRange(properties);
        }

        private void attachedElement_RadPropertyChanged(object sender, RadPropertyChangedEventArgs e)
        {
            if (this.affectedProperties == null)
            {
                return;
            }

            RadItem senderItem = sender as RadItem;

            Debug.Assert(senderItem != null, "Property changed event received and sender is not RadItem");

            if (senderItem != null)
            {
                for (int i = 0; i < affectedProperties.Count; i++)
                {
                    if (e.Property == affectedProperties[i])
                    {
                        this.eventHandler(senderItem, e);
                        break;
                    }
                }
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (elementToAttachTo != null)
            {
                elementToAttachTo.RadPropertyChanged -= new RadPropertyChangedEventHandler(attachedElement_RadPropertyChanged);
            }
        }

        #endregion

        #region fields

        private List<RadProperty> affectedProperties;

        private RadItem elementToAttachTo;
        private RadItemStateChangedEventHandler eventHandler;

        #endregion
    }
}
