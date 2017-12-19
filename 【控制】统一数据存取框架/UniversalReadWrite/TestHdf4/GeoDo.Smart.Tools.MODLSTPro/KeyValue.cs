namespace GeoDo.Smart.Tools.MODLSTPro
{
    public class KeyValue
    {
        public KeyValue(string kvString)
        {
            Source = kvString;
            string[] kv = kvString.Split(new char[] { '=' });
            Key = kv[0];
            Value = kv[1].Replace("\"", "").Replace("(", "").Replace(")", "");
        }

        public string Source { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return Source;
        }
    }
}