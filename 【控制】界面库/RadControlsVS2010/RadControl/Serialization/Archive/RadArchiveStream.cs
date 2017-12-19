using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;
using System.Xml;
using System.ComponentModel;

namespace Telerik.WinControls.Serialization
{
    /// <summary>
    /// Encapsulates information for a single stream within a zipped stream.
    /// </summary>
    [Serializable]
    [XmlInclude(typeof(RadXmlThemeArchiveStream))]
    public abstract class RadArchiveStream
    {
        #region Fields

        private string name;
        private byte[] zippedBytes;
        [NonSerialized]
        private byte[] rawBytes;
        [NonSerialized]
        private object context;

        #endregion

        #region Constructor

        public RadArchiveStream()
        {
        }

        public RadArchiveStream(object context)
        {
            this.context = context;
        }

        #endregion

        #region Methods

        public void Zip()
        {
            //no context to save
            if (this.context == null)
            {
                return;
            }
            //bytes are not null, we are either deserialized or already saved.
            if (this.zippedBytes != null || this.context == null)
            {
                return;
            }

            try
            {
                this.zippedBytes = this.GetZippedBytesCore();
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Failed to write archive bytes. Exception was:\r\n" + ex);
            }
        }

        protected virtual byte[] GetZippedBytesCore()
        {
            this.rawBytes = this.GetRawBytes();
            if (this.rawBytes == null)
            {
                return null;
            }

            MemoryStream zipOutputStream = new MemoryStream();
            DeflateStream zipStream = new DeflateStream(zipOutputStream, CompressionMode.Compress, true);
            zipStream.Write(this.rawBytes, 0, this.rawBytes.Length);
            //close the zip stream to force it write the last 4 bytes which are actually the length of the stream
            zipStream.Close();

            byte[] zippedBytes = zipOutputStream.ToArray();
            zipOutputStream.Close();

            return zippedBytes;
        }

        protected virtual byte[] GetRawBytes()
        {
            return null;
        }

        public object Unzip()
        {
            if (this.context != null)
            {
                return this.context;
            }

            if (this.zippedBytes == null || this.zippedBytes.Length == 0)
            {
                return null;
            }

            try
            {
                this.rawBytes = this.Unzip(this.zippedBytes);
                this.context = this.Deserialize(this.rawBytes);
                return this.context;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to deserialize archive. Exception was:\r\n" + ex);
                return null;
            }
        }

        protected virtual object Deserialize(byte[] rawBytes)
        {
            return null;
        }

        protected virtual byte[] Unzip(byte[] zippedBytes)
        {
            MemoryStream zipMemStream = new MemoryStream(zippedBytes);
            DeflateStream unzipStream = new DeflateStream(zipMemStream, CompressionMode.Decompress, true);
            MemoryStream unzipMemStream = new MemoryStream();

            //read the zipped bytes into another memory stream
            const int bufferSize = 4096;
            byte[] buffer = new byte[bufferSize];
            while (true)
            {
                int readBytes = unzipStream.Read(buffer, 0, bufferSize);
                //write the successfully read bytes
                if (readBytes > 0)
                {
                    unzipMemStream.Write(buffer, 0, readBytes);
                }
                //reached end of stream
                if (readBytes != bufferSize)
                {
                    break;
                }
            }

            //closing the stream will write the final 4 bytes, which are the length of the stream
            unzipStream.Close();
            byte[] rawBytes = unzipMemStream.ToArray();
            unzipMemStream.Close();
            zipMemStream.Close();

            return rawBytes;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the context associated with the archive.
        /// </summary>
        [XmlIgnore]
        public object Context
        {
            get
            {
                return this.context;
            }
            set
            {
                if (this.context == value)
                {
                    return;
                }

                this.context = value;
                this.rawBytes = null;
                this.zippedBytes = null;
            }
        }

        /// <summary>
        /// Gets the raw bytes of the underlying stream.
        /// </summary>
        [XmlIgnore]
        public byte[] RawBytes
        {
            get
            {
                return this.rawBytes;
            }
        }

        /// <summary>
        /// Gets or sets the already zipped raw bytes of the underlying stream.
        /// </summary>
        public byte[] ZippedBytes
        {
            get
            {
                return this.zippedBytes;
            }
            set
            {
                this.zippedBytes = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeZippedBytes()
        {
            return this.zippedBytes != null && this.zippedBytes.Length > 0;
        }

        /// <summary>
        /// Gets the count of the raw bytes that form the stream.
        /// </summary>
        public int ByteCount
        {
            get
            {
                if (this.rawBytes != null)
                {
                    return this.rawBytes.Length;
                }

                if (this.zippedBytes != null)
                {
                    return this.zippedBytes.Length;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets or sets the name of this archive.
        /// </summary>
        [XmlAttribute]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeName()
        {
            return !string.IsNullOrEmpty(this.name);
        }

        /// <summary>
        /// Gets the information about the format of the underlying stream.
        /// </summary>
        public abstract StreamFormat Format
        {
            get;
        }

        #endregion
    }
}
