using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Smart.Tools.MODLSTPro
{
    public class KeyValues : List<KeyValue>
    {
        public bool ContainsKey(string key)
        {
            return this.Any(ky => ky.Key == key);
        }

        public string this[string key]
        {
            get { return (from ky in this where ky.Key == key select ky.Value).FirstOrDefault(); }
        }

        public void Add2(KeyValue keyValue)
        {
            if (!ContainsKey(keyValue.Key))
                Add(keyValue);
        }

        public override string ToString()
        {
            StringBuilder sbBuilder = new StringBuilder(this[0].ToString());
            for (int i = 1; i < this.Count; i++)
            {
                sbBuilder.Append("\n");
                sbBuilder.Append(this[i]);
            }
            string rs = sbBuilder.ToString();
            return rs;
        }
    }
}