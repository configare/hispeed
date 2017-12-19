using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls
{
	/// <summary>
	/// This class supports the TPF internal infrastructure and is not intended to be used directly from your code.
	/// </summary>
	public class FillRepository: IDisposable
	{
		public Dictionary<Size, Dictionary<int, Bitmap>> PaintBuffers;

		private bool disableBitmapCache = false;

		internal FillRepository()
		{
			this.PaintBuffers = new Dictionary<Size, Dictionary<int, Bitmap>>();
		}

		public bool DisableBitmapCache
		{
			get { return disableBitmapCache; }
			set { disableBitmapCache = value; }
		}

		public Dictionary<int, Bitmap> GetBuffersBySize(Size size)
		{
			if (DisableBitmapCache)
				return null;

			if (!this.PaintBuffers.ContainsKey(size))
				return null;

			return this.PaintBuffers[size];
		}

		public Bitmap GetBitmapBySizeAndHash(Size size, int hash)
		{
			if (DisableBitmapCache)
				return null;

			Dictionary<int, Bitmap> bitmapBuffers = this.GetBuffersBySize(size);
			if (bitmapBuffers == null)
				return null;

            Bitmap cache;
            bitmapBuffers.TryGetValue(hash, out cache);
            return cache;
		}

		public void RemoveBitmapsBySize(Size size)
		{
            if (size == Size.Empty)
                return;

			Dictionary<int, Bitmap> bitmapBuffers = this.GetBuffersBySize(size);

			if (bitmapBuffers == null)
				return;

			foreach (KeyValuePair<int, Bitmap> keyValuePair in bitmapBuffers)
			{
				keyValuePair.Value.Dispose();
			}
			PaintBuffers.Remove(size);
		}

		public void AddNewBitmap(Size size, int hash, Bitmap bitmap)
		{
			if (DisableBitmapCache || size == Size.Empty)
				return;

			Dictionary<int, Bitmap> bitmapBuffers;
            this.PaintBuffers.TryGetValue(size, out bitmapBuffers);

            if (bitmapBuffers == null)
			{
                bitmapBuffers = new Dictionary<int, Bitmap>();
                this.PaintBuffers.Add(size, bitmapBuffers);
			}

			bitmapBuffers[hash] = bitmap;
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
            if (disposing)
            {
                foreach (KeyValuePair<Size, Dictionary<int, Bitmap>> paintBuffer in PaintBuffers)
                {
                    foreach (KeyValuePair<int, Bitmap> bitmapEntry in paintBuffer.Value)
                    {
                        bitmapEntry.Value.Dispose();
                    }
                }

                PaintBuffers.Clear();
            }
		}

		~FillRepository()
        {
            this.Dispose(false);
        }

		#endregion
	}
}