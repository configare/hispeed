using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Xml;
using Telerik.WinControls.XmlSerialization;

namespace Telerik.WinControls.Serialization
{
    [Serializable]
    [XmlInclude(typeof(RadThemePackage))]
    public class RadArchivePackage
    {
        #region Fields

        private List<RadArchiveStream> streams;
        private PackageFormat format;

        #endregion

        #region Constructor

        public RadArchivePackage()
            : this(null)
        {
        }

        public RadArchivePackage(params RadArchiveStream[] streamInfos)
        {
            this.format = this.DefaultFormat;
            this.streams = new List<RadArchiveStream>();
            if (streamInfos != null)
            {
                this.streams.AddRange(streamInfos);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the default format for this package.
        /// </summary>
        protected virtual PackageFormat DefaultFormat
        {
            get
            {
                return PackageFormat.Binary;
            }
        }

        /// <summary>
        /// Gets or sets the format used to persist the package.
        /// </summary>
        [XmlAttribute]
        [DefaultValue(PackageFormat.Binary)]
        public PackageFormat Format
        {
            get
            {
                return this.format;
            }
            set
            {
                this.format = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeFormat()
        {
            return this.format != this.DefaultFormat;
        }
        
        /// <summary>
        /// Gets the list which contains the streams of this package.
        /// </summary>
        public List<RadArchiveStream> Streams
        {
            get
            {
                return this.streams;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeStreams()
        {
            return this.streams.Count > 0;
        }

        #endregion

        #region Compress

        public Stream Compress()
        {
            if (this.streams.Count == 0)
            {
                return null;
            }

            int totalByteCount = this.PrepareStreams();
            if (totalByteCount == 0)
            {
                return null;
            }

            return this.SaveToStreamCore(totalByteCount);
        }

        public bool Compress(string fileName)
        {
            Stream stream = this.Compress();
            if (stream == null)
            {
                return false;
            }

            FileStream fileStream = null;

            try
            {
                fileStream = File.Open(fileName, FileMode.Create, FileAccess.ReadWrite);
                byte[] streamBuffer = new byte[stream.Length];
                stream.Position = 0;
                stream.Read(streamBuffer, 0, streamBuffer.Length);
                fileStream.Write(streamBuffer, 0, streamBuffer.Length);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to save compressed stream due to:\r\n" + ex);
                return false;
            }
            finally
            {
                stream.Close();
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        protected virtual Stream SaveToStreamCore(int byteCount)
        {
            //save ourselves to a memory stream and then compress it 
            MemoryStream memStream = new MemoryStream(byteCount);
            try
            {
                if (this.format == PackageFormat.Binary)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(memStream, this);
                }
                else
                {
                    XmlSerializer serializer = new XmlSerializer(this.GetType());
                    serializer.Serialize(memStream, this);
                }

                return memStream;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to compress archive. Exception was:\r\n" + ex);
                if (memStream != null)
                {
                    memStream.Close();
                }
                return null;
            }
        }

        private int PrepareStreams()
        {
            int count = 0;
            foreach (RadArchiveStream stream in this.streams)
            {
                stream.Zip();
                count += stream.ByteCount;
            }

            return count;
        }

        #endregion

        #region Decompress

        /// <summary>
        /// Decompresses the stream using Binary format.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static RadArchivePackage Decompress(Stream stream)
        {
            return DecompressBinary(stream);
        }

        /// <summary>
        /// Decompresses the stream in the provided file using Binary format.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static RadArchivePackage Decompress(string fileName)
        {
            return DecompressFile(fileName, null);
        }

        public static RadArchivePackage Decompress(Stream stream, Type type)
        {
            return DecompressXML(stream, type);
        }

        public static RadArchivePackage Decompress(string fileName, Type type)
        {
            return DecompressFile(fileName, type);
        }

        private static RadArchivePackage DecompressFile(string fileName, Type type)
        {
            if (!File.Exists(fileName))
            {
                return null;
            }

            FileStream fileStream = null;
            try
            {
                fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read);
                if (type != null)
                {
                    return Decompress(fileStream, type);
                }

                return Decompress(fileStream);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to deserialize RadArchivePackage. Exception was:\r\n" + ex);
                return null;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        private static RadArchivePackage DecompressXML(Stream stream, Type type)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(type);
                return serializer.Deserialize(stream) as RadArchivePackage;
            }
            catch
            {
                return null;
            }
        }

        private static RadArchivePackage DecompressBinary(Stream stream)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(stream) as RadArchivePackage;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
