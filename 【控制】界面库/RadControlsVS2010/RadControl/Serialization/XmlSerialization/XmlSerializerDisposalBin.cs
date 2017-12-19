using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Telerik.WinControls.XmlSerialization
{
    public delegate void DisposeObjectDelegate(IDisposable toBeDisposed);

    internal class XmlSerializerDisposalBin
    {
        private List<IDisposable> disposalBin;
        private List<IDisposable> nonDisposalBin;

		public void AddObjectsToDispose(IEnumerable toRead)
		{
            foreach (object toDispose in toRead)
            {
                this.AddObjectToDispose(toDispose);
            }
		}

        public void AddObjectToDispose(object toDispose)
        {
            IDisposable disposable = toDispose as IDisposable;

            if (disposable == null)
            {
                return;
            }

            if (this.nonDisposalBin != null && this.nonDisposalBin.Contains(disposable))
            {
                return;
            }

            if (disposalBin == null)
            {
                disposalBin = new List<IDisposable>();
            }

            if (!this.disposalBin.Contains(disposable))
            {
                this.disposalBin.Add(disposable);
            }
        }

        public void SetObjectShouldNotBeDisposed(object toNotDispose)
        {
            IDisposable disposable = toNotDispose as IDisposable;

            if (disposable == null)
            {
                return;
            }

            if (this.nonDisposalBin == null)
            {
                this.nonDisposalBin = new List<IDisposable>();
            }

            this.nonDisposalBin.Add(disposable);

            if (this.disposalBin == null)
            {
                return;
            }            

            this.disposalBin.Remove(disposable);
        }

        public void DisposeDisposalBin(DisposeObjectDelegate callBack)
        {
            if (this.disposalBin == null)
            {
                return;
            }

            foreach (IDisposable disposable in this.disposalBin)
            {
                if (callBack != null)
                {
                    callBack(disposable);
                }
                else
                {
                    disposable.Dispose();
                }
            }

            disposalBin.Clear();
        }
    }
}
