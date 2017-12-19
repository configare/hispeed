using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Text;
using System.IO;

namespace CodeCell.AgileMap.Core
{
    public partial class UCPictureBrowser : UserControl
    {
        private Size _imgSize = new Size(20, 20);
        private Size _gridSize = new Size(32, 32);
        private List<ImageItem> _images = null;
        private ImageItem _currentImage = null;
        private ImageItem _selectImage = null;

        public UCPictureBrowser()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        internal ImageItem GetSelectedImage()
        {
            return _selectImage;
        }
  
        public void ApplyImages(string[] files)
        {
            try
            {
                TryDisposeImages();
                CreateImages(files);
            }
            finally
            {
                Invalidate();
            }
        }

        private void CreateImages(string[] files)
        {
            if (files == null || files.Length == 0)
                return;
            if (_images == null)
                _images = new List<ImageItem>();
            foreach (string f in files)
            {
                if (string.IsNullOrEmpty(f))
                    continue;
                try
                {
                    _images.Add(new ImageItem(f));
                }
                catch
                {
                }
            }
        }

        private void TryDisposeImages()
        {
            if (_images == null)
                return;
            foreach (ImageItem f in _images)
            {
                f.Image.Dispose();
            }
            _images.Clear();
            _images = null;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_images == null || _images.Count == 0)
                return;
            foreach (ImageItem it in _images)
            {
                if (it.Bounds.Contains(e.Location))
                {
                    _currentImage = it;
                    Invalidate();
                    return;
                }
            }
            _currentImage = null;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (_images == null || _images.Count == 0)
                return;
            foreach (ImageItem it in _images)
            {
                if (it.Bounds.Contains(e.Location))
                {
                    _selectImage = it;
                    Invalidate();
                    return;
                }
            }
            _selectImage = null;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_images == null || _images.Count == 0)
                return;
            int banks = 10;
            int x = banks;
            int y = banks;
            foreach (ImageItem f in _images)
            {
                int x1 = x + (_gridSize.Width - Math.Min(_imgSize.Width, f.Image.Width)) / 2;
                int y1 = y + (_gridSize.Height - Math.Min(_imgSize.Height, f.Image.Height)) /2;
                Rectangle desRect = new Rectangle(x1, y1, Math.Min(_imgSize.Width, f.Image.Width), Math.Min(_imgSize.Height, f.Image.Height));
                e.Graphics.DrawImage(f.Image,desRect,new Rectangle(0,0,f.Image.Width,f.Image.Height),GraphicsUnit.Pixel);
                f.Bounds = new RectangleF(x, y, _imgSize.Width, _imgSize.Height);
                if (_currentImage != null && _currentImage.Equals(f))
                    e.Graphics.DrawRectangle(Pens.Green, x, y, _gridSize.Width, _gridSize.Height);
                //
                if (_selectImage != null && _selectImage.Equals(f))
                    e.Graphics.DrawRectangle(Pens.Red, x, y, _gridSize.Width, _gridSize.Height);
                //
                x += _gridSize.Width;
                //
                if (x > Width - 5 * banks)
                {
                    x = banks;
                    y += _gridSize.Height;
                }
                if (y > Height - 5 * banks)
                    Size = new Size(Width, y);
            }
        }
    }

    public class ImageItem
    {
        public string Filename = null;
        public Image Image = null;
        public RectangleF Bounds = RectangleF.Empty;

        public ImageItem()
        { 
        }

        public ImageItem(string filename)
        {
            Filename = filename;
            Image = Image.FromFile(filename);
        }

        public ImageItem(string filename,bool isCreateBitmap)
        {
            Filename = filename;
        }

        public override string ToString()
        {
            return Filename != null ? Path.GetFileName(Filename): string.Empty;
        }
    }
}
