using System.IO;

namespace GeoDo.RSS.DF.AWX
{
    public class AWXLevel2HeaderExImage : AWXLevel2HeaderEx
    {
        private short toningTableDataBlockLength;
        private short calibrationDataBlockLength;
        private short locationDataBlockLength;

        public ToningTableDataBlock TTDB { get; set; }
        public CalibrationDataBlock CDB { get; set; }
        public LocationDataBlock LDB { get; set; }

        private AWXLevel2HeaderExImage() { }

        public AWXLevel2HeaderExImage(short toningTableDataBlockLength, short calibrationDataBlockLength, short locationDataBlockLength, CalibrationDataBlock cdb)
        {
            this.toningTableDataBlockLength = toningTableDataBlockLength;
            this.calibrationDataBlockLength = calibrationDataBlockLength;
            this.locationDataBlockLength = locationDataBlockLength;
            CDB = cdb;
        }

        public override void Read(Stream stream)
        {
            using (BinaryReader br = new BinaryReader(stream))
            {
                if (toningTableDataBlockLength != 0)
                {
                    byte[] ToningTableDataBlockBytes = br.ReadBytes(toningTableDataBlockLength);
                    TTDB = new ToningTableDataBlock();
                    TTDB.Read(new MemoryStream(ToningTableDataBlockBytes));
                }

                if (calibrationDataBlockLength != 0)
                {
                    byte[] CalibrationDataBlockBytes = br.ReadBytes(calibrationDataBlockLength);
                    CDB.Read(new MemoryStream(CalibrationDataBlockBytes));
                }

                if (locationDataBlockLength != 0)
                {
                    byte[] LocationDataBlockBytes = br.ReadBytes(locationDataBlockLength);
                    LDB = new LocationDataBlock();
                    LDB.Read(new MemoryStream(LocationDataBlockBytes));
                }
            }
        }
    }

    public class ToningTableDataBlock
    {
        public byte[] RToningTable { get; set; }
        public byte[] GToningTable { get; set; }
        public byte[] BToningTable { get; set; }

        public void Read(Stream stream)
        {
            using (BinaryReader br = new BinaryReader(stream))
            {
                RToningTable = br.ReadBytes(256);
                GToningTable = br.ReadBytes(256);
                BToningTable = br.ReadBytes(256);
            }
        }
    }

    public abstract class CalibrationDataBlock
    {
        public ushort[] CalibrationData { get; set; }

        public abstract void Read(Stream stream);
    }

    public class CalibrationDataBlockGeostationary : CalibrationDataBlock
    {
        public override void Read(Stream stream)
        {
            using (BinaryReader br = new BinaryReader(stream))
            {
                for (int i = 0; i < 1024; ++i)
                {
                    CalibrationData[i] = br.ReadUInt16();
                }
            }
        }
    }

    public class CalibrationDataBlockPolarOrbit : CalibrationDataBlock
    {
        public override void Read(Stream stream)
        {
            using (BinaryReader br = new BinaryReader(stream))
            {
                for (int i = 0; i < 256; ++i)
                {
                    CalibrationData[i] = br.ReadUInt16();
                }
            }
        }
    }

    public class LocationDataBlock
    {
        GridDataDescription GDD { get; set; }
        GridData GD { get; set; }

        public void Read(Stream stream)
        {
            GDD.Read(stream);
            GD.Read(stream);
        }
    }

    public class GridDataDescription
    {
        public short GridPointCoordinateDefinition { get; set; }
        public short GridDataSource { get; set; }
        public short GridDegree { get; set; }
        public short UpperLeftGridPointLatitude { get; set; }
        public short UpperLeftGridPointLongitude { get; set; }
        public short HorizontalGridPoint { get; set; }
        public short VerticalGridPoint { get; set; }
        public short Reserve { get; set; }

        public void Read(Stream stream)
        {
            using (BinaryReader br = new BinaryReader(stream))
            {
                GridPointCoordinateDefinition = br.ReadInt16();
                GridDataSource = br.ReadInt16();
                GridDegree = br.ReadInt16();
                UpperLeftGridPointLatitude = br.ReadInt16();
                UpperLeftGridPointLongitude = br.ReadInt16();
                HorizontalGridPoint = br.ReadInt16();
                VerticalGridPoint = br.ReadInt16();
                Reserve = br.ReadInt16();
            }
        }
    }

    public class GridData
    {
        public short[,] Data { get; set; }

        public void Read(Stream stream)
        {

        }
    }
}
