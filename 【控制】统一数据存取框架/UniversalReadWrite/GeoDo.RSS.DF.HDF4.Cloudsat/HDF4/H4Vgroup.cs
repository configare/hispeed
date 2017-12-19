using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.HDF4.Cloudsat
{
    public class H4Vgroup : HGroup
    {
        private List<H4Vgroup> _vgroups = new List<H4Vgroup>();
        private List<H4Vdata> _vdatas = new List<H4Vdata>();
        private List<H4SDS> _sds = new List<H4SDS>();

        public H4Vgroup(HFile theFile, String name, String path, HGroup parent, long[] oid)
            : base(theFile, name, path, parent)
        {
        }

        public int Vgroup_ref { get; set; }

        public int Vgroup_id { get; set; }

        public int Vgroup_tag { get; set; }

        public string Vgroup_class { get; set; }

        public string Vgroup_name { get; set; }

        public int N_attrs { get; set; }

        public int N_entries { get; set; }

        public int N_tagrefs { get; set; }

        public H4Vgroup[] Vgroups { get { return _vgroups.ToArray(); } }

        public H4Vdata[] Vdatas { get { return _vdatas.ToArray(); } }

        public H4SDS[] SDS { get { return _sds.ToArray(); } }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},({3},{4})({5},{6})", Vgroup_ref, Vgroup_tag, Vgroup_id, Vgroup_class, Vgroup_name, N_attrs, N_entries);
        }

        internal void AddVgroup(H4Vgroup vgroup)
        {
            _vgroups.Add(vgroup);
        }

        internal void AddVdata(H4Vdata vdata)
        {
            _vdatas.Add(vdata);
        }

        internal void AddVdata(H4SDS sd)
        {
            _sds.Add(sd);
        }
    }
}
