namespace GeoDo.Smart.Tools.MODLSTPro
{
    public class HdfDataFormat 
    {
        public string Name { get; set; }
        public string Match { get; set; } 

        public HdfDataFormat()
        { }

        public HdfDataFormat(string name, string match)
        {
            Name = name;
            Match = match;
        }

        public HDFDef HdfDef { get; set; } 
    }
}
