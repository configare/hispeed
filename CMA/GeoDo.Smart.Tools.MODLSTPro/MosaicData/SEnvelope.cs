namespace GeoDo.Smart.Tools.MODLSTPro
{
    public class SEnvelope
    {
        public double XMin { get; set; }
        public double YMin { get; set; }
        public double XMax { get; set; }
        public double YMax { get; set; }
        public double Width { get { return XMax - XMin; } }
        public double Height { get { return YMax - YMin; } }

        public SEnvelope Clone()
        {
            return new SEnvelope
            {
                XMin = XMin,
                XMax = XMax,
                YMin = YMin,
                YMax = YMax
            };
        }
    }
}