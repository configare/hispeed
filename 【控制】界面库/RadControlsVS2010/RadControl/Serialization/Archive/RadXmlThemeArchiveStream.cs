using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.IO.Compression;
using System.Xml;
using Telerik.WinControls.Themes.Serialization;

namespace Telerik.WinControls.Serialization
{
    /// <summary>
    /// An archived stream, which persists a XmlTheme instance.
    /// </summary>
    [Serializable]
    public class RadXmlThemeArchiveStream : RadArchiveStream
    {
        #region Constructor

        public RadXmlThemeArchiveStream()
        {
        }

        public RadXmlThemeArchiveStream(XmlTheme theme)
            : base(theme)
        {
        }

        #endregion

        #region Properties

        public override StreamFormat Format
        {
            get
            {
                return StreamFormat.XML;
            }
        }

        #endregion

        #region Methods

        protected override byte[] GetRawBytes()
        {
            XmlTheme theme = this.Context as XmlTheme;
            Debug.Assert(theme != null, "Invalid XmlThemeArchiveStream context.");

            MemoryStream savedThemeStream = new MemoryStream();
            theme.SaveToStream(savedThemeStream);
            byte[] savedBuffer = savedThemeStream.ToArray();
            savedThemeStream.Close();

            return savedBuffer;
        }

        protected override object Deserialize(byte[] rawBytes)
        {
            XmlTheme theme = new XmlTheme();

            MemoryStream memStream = new MemoryStream(rawBytes);
            XmlReader reader = XmlReader.Create(memStream);
            theme.DeserializePartiallyThemeFromReader(reader);

            reader.Close();
            memStream.Close();

            return theme;
        }

        #endregion
    }
}
