using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.HDF4;
using System.Diagnostics;

namespace GeoDo.RSS.DF.HDF4.Cloudsat
{
    public class H4File : HFile, IDisposable
    {
        private string _filename;
        private bool _fileOpenState = false;
        private int _gr_id, _sd_id;
        private int _num_datasets;
        private int _num_global_attrs;
        List<H4SDS> _datasets = new List<H4SDS>();
        List<HDFAttribute> _global_attrs = new List<HDFAttribute>();
        private List<H4Vdata> _vdatas = new List<H4Vdata>();
        private H4Vgroup[] _vgroups = null;
        private int status = 0;
        Stopwatch sw = new Stopwatch();
        long em = 0;

        public H4File(HFile theFile, String theName, String thePath, long[] oid)
            : base(theFile, theName, thePath, oid)
        { }

        public void Load(string filename)
        {
            Load(filename, DFACC.DFACC_READ);
        }

        public void Load(string filename, DFACC dfacc)
        {
            sw.Start();
            _filename = filename;
            int file_id, sd_id;
            try
            {
                _fileOpenState = false;
                file_id = HDF4API.Hopen(filename, DFACC.DFACC_READ, 0);
                HDF4API.Vstart(file_id);
                //_gr_id = HDF4API.GRstart(file_id);
            }
            catch
            {
                string strErrInfo = "HDF4打开文件异常";
                throw new Exception(strErrInfo);
            }
            _fileOpenState = true;
            sd_id = HDF4Helper.SDstart(filename, HDF4Helper.AccessCodes.DFACC_READ);
            if (sd_id == -1)
            {
                //打开失败
            }
            _file_id = file_id;
            _sd_id = sd_id;
            sw.Stop();
            em = sw.ElapsedMilliseconds;
            Console.WriteLine("打开" + em + "毫秒");
            // load the file hierarchy
            getRootGroup();
        }

        /** get the root group and all the alone objects */
        private void getRootGroup()
        {
            sw.Restart();
            //LoadVgroups(_file_id);//暂时取消加载整个树结构，目前这个加载比较慢，暂时也用不着。
            sw.Stop();
            em = sw.ElapsedMilliseconds;
            Console.WriteLine("LoadVgroups" + em + "毫秒");

            sw.Restart();
            int num_ds = 0;
            int num_global = 0;
            status = HDF4Helper.SDfileinfo(_sd_id, out num_ds, out num_global);
            _num_datasets = num_ds;
            _num_global_attrs = num_global;
            for (int i = 0; i < num_ds; i++)
            {
                H4SDS ds = H4SDS.Load(_sd_id, i);
                _datasets.Add(ds);
            }
            for (int i = 0; i < num_global; i++)
            {
                HDFAttribute global_attr = HDFAttribute.Load(_sd_id, i);
                _global_attrs.Add(global_attr);
            }
            sw.Stop();
            em = sw.ElapsedMilliseconds;
            Console.WriteLine("SDfileinfo" + em + "毫秒");


            sw.Restart();
            //Finding All Vdatas that are Not Members of a Vgroup: VSlone
            //初始化Vdata(读取表格数据,表格列名称参数)
            int[] ref_array = new int[1024];
            HDF4API.VSlone(_file_id, ref_array, 1024);

            //初始化Vdata(读取表格数据,表格列名称参数) istat;
            int vdata_ref;
            int istat = HDF4API.Vstart(_file_id);
            vdata_ref = -1;
            List<H4Vdata> vdatas = new List<H4Vdata>();
            while ((vdata_ref = HDF4API.VSgetid(_file_id, vdata_ref)) != HDFConstants.FAIL)
            {
                H4Vdata vdata = LoadVdata(_file_id, vdata_ref);
                vdatas.Add(vdata);
            }
            _vdatas = vdatas;
            sw.Stop();
            em = sw.ElapsedMilliseconds;
            Console.WriteLine("VSlone" + em + "毫秒");
        }

        private void LoadVgroups(int file_id)
        {
            int vgroup_ref = -1;
            int[] lone_array = new int[HDFConstants.NLONE];
            int lone_count;
            // get top level VGroup
            int[] tmpN = new int[HDFConstants.NLONE];
            try
            {
                // first call to get the number of lone Vgroup
                lone_count = HDF4API.Vlone(file_id, tmpN, HDFConstants.NLONE);
                lone_array = new int[lone_count];
                // second call to get the references of all lone Vgroup
                lone_count = HDF4API.Vlone(file_id, lone_array, lone_count);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                lone_count = 0;
                return;
            }
            Console.WriteLine(lone_count);
            List<H4Vgroup> vgroups = new List<H4Vgroup>();
            for (int i = 0; i < lone_count; i++)
            {
                vgroup_ref = lone_array[i];
                H4Vgroup vgroup = LoadVgroup(file_id, vgroup_ref, null);
                Console.WriteLine(vgroup);
                vgroups.Add(vgroup);
            }

            //while ((vgroup_ref = HDF4API.Vgetid(file_id, vgroup_ref)) != HDFConstants.FAIL)
            //{
            //    if ((vgroup_id = HDF4API.Vattach(file_id, vgroup_ref, "r")) == HDFConstants.FAIL)
            //        break;
            //    vgroup_class = new StringBuilder(HDFConstants.VGCLASSLENMAX);
            //    vgroup_name = new StringBuilder(HDFConstants.VGNAMELENMAX);
            //    status = HDF4API.Vgetname(vgroup_id, vgroup_name);
            //    status = HDF4API.Vgetclass(vgroup_id, vgroup_class); /* get Vgroup classname */
            //    int ntag = HDF4API.Vntagrefs(vgroup_id);
            //}
            _vgroups = vgroups.ToArray();
            //status = HDF4API.Vdetach(vgroup_id);
            //status = HDF4API.Vend(file_id);
            //status = HDF4API.Hclose(file_id);
        }

        private H4Vgroup LoadVgroup(int file_id, int vgroup_ref, H4Vgroup pgroup)
        {
            int vgroup_id = 0;
            int vgroup_tag = 0;
            int n_vgroup_attr = 0;
            int n_entries = 0;
            int n_tagrefs = 0;
            int[] tag_array = new int[HDFConstants.NLONE];
            int[] ref_array = new int[HDFConstants.NLONE];

            StringBuilder vgroup_class = null;
            StringBuilder vgroup_name = null;
            vgroup_id = HDF4API.Vattach(file_id, vgroup_ref, "r");
            vgroup_tag = HDF4API.VQuerytag(vgroup_id);
            vgroup_class = new StringBuilder(HDFConstants.VGCLASSLENMAX);
            vgroup_name = new StringBuilder(HDFConstants.VGNAMELENMAX);
            status = HDF4API.Vgetclass(vgroup_id, vgroup_class);
            if ((status = HDF4API.Vinquire(vgroup_id, out n_entries, vgroup_name)) == HDFConstants.FAIL)//整个文件fullname也会作为一个lone的group，这时vgroup_name是带路径的文件全名fullname
            {
                HDF4API.Vdetach(vgroup_id);
                return null;
            }
            // ignore the Vgroups created by the GR interface
            if (string.Equals(vgroup_class.ToString(), HDFConstants.GR_NAME, StringComparison.OrdinalIgnoreCase) ||// do not display Vdata named "Attr0.0"
                string.Equals(vgroup_class.ToString(), HDFConstants.RI_NAME, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(vgroup_class.ToString(), HDFConstants.RIGATTRNAME, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(vgroup_class.ToString(), HDFConstants.RIGATTRCLASS, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(vgroup_class.ToString(), HDFConstants.HDF_CDF, StringComparison.OrdinalIgnoreCase)
                )
            {
                HDF4API.Vdetach(vgroup_id);
                return null;
            }
            n_vgroup_attr = HDF4API.Vnattrs(vgroup_id);
            n_tagrefs = HDF4API.Vntagrefs(vgroup_id);
            H4Vgroup vgroup = new H4Vgroup(this, vgroup_name.ToString(), "/", pgroup, new long[] { vgroup_tag, vgroup_ref });
            vgroup.Vgroup_ref = vgroup_ref;
            vgroup.Vgroup_id = vgroup_id;
            vgroup.Vgroup_tag = vgroup_tag;
            vgroup.Vgroup_name = vgroup_name.ToString();
            vgroup.Vgroup_class = vgroup_class.ToString();
            vgroup.N_attrs = n_vgroup_attr;
            vgroup.N_entries = n_entries;
            vgroup.N_tagrefs = n_tagrefs;
            if (n_tagrefs == 0)
            {
                if ((status = HDF4API.Vdetach(vgroup_id)) == HDFConstants.FAIL)
                    return null;
                return vgroup;
            }
            int tag = 0;
            int ref_id = 0;
            int sds_index = 0;
            for (int index = 0; index < n_tagrefs; index++)
            {
                if ((status = HDF4API.Vgettagref(vgroup_id, index, out tag, out ref_id)) == HDFConstants.FAIL)
                {
                    //Vdetach(
                    break;
                }
                switch (tag)
                {
                    case HDFConstants.DFTAG_VG:
                        H4Vgroup vg = LoadVgroup(file_id, ref_id, vgroup);
                        Console.WriteLine(vg);
                        vgroup.AddVgroup(vg);
                        break;
                    case HDFConstants.DFTAG_VH:
                    case HDFConstants.DFTAG_VS:
                        {
                            H4Vdata vdata = LoadVdata(file_id, ref_id);
                            Console.WriteLine(vdata);
                            vgroup.AddVdata(vdata);
                            break;
                        }
                    case HDFConstants.DFTAG_NDG://数据集
                    case HDFConstants.DFTAG_SDG:
                    case HDFConstants.DFTAG_SD:
                        {
                            try
                            {
                                sds_index = HDF4API.SDreftoindex(_sd_id, ref_id);
                            }
                            catch (Exception ex)
                            {
                                sds_index = HDFConstants.FAIL;
                            }
                            if (sds_index != HDFConstants.FAIL)
                            {
                                //H4SDS sds = getSDS(tag, index, fullPath, true);
                                //pgroup.addToMemberList(sds);
                                //if ((sds != null) && (pnode != null))
                                //{
                                //    node = new DefaultMutableTreeNode(sds);
                                //    pnode.add(node);
                                //}
                            }
                            Console.WriteLine(string.Format("TAG:{0},{1},{2}", tag, ref_id, sds_index));
                            H4SDS sd = H4SDS.Load(_sd_id, sds_index);
                            vgroup.AddVdata(sd);
                            Console.WriteLine(sd);
                        }
                        break;
                    case HDFConstants.DFTAG_RIG:
                    case HDFConstants.DFTAG_RI:
                    case HDFConstants.DFTAG_RI8:
                        #region 
                        //try 
                        //{
                        //    index = HDF4API.GRreftoindex(grid, (short) ref);
                        //} 
                        //catch (Exception ex) 
                        //{
                        //    index = HDFConstants.FAIL;
                        //}
                        //if (index != HDFConstants.FAIL) 
                        //{
                        //    H4GRImage gr = getGRImage(tag, index, fullPath, true);
                        //    pgroup.addToMemberList(gr);
                        //    if ((gr != null) && (pnode != null)) 
                        //    {
                        //        node = new DefaultMutableTreeNode(gr);
                        //        pnode.add(node);
                        //    }
                        //}
                        #endregion
                        break;
                    default:
                        Console.WriteLine(string.Format("TAG:{0},{1},{2}", tag, ref_id, index));
                        break;
                }/* end of switch       */
                //if (ret != NO_OBJECT) break;
            }/* end of for loop     */
            if ((status = HDF4API.Vdetach(vgroup_id)) == HDFConstants.FAIL)
                return null;
            return vgroup;
        }

        private H4SDS LoadSD(int file_id, int ref_id)
        {
            throw new NotImplementedException();
        }

        private H4Vdata LoadVdata(int file_id, int vdata_ref)
        {
            StringBuilder vdata_name = new StringBuilder();
            StringBuilder vdata_class = new StringBuilder();
            StringBuilder fields;
            int vdata_id = -1, istat, vdata_tag;
            int n_records, interlace, vdata_size;
            try
            {
                vdata_id = HDF4API.VSattach(file_id, vdata_ref, "r");
                fields = new StringBuilder(60);
                istat = HDF4API.VSinquire(vdata_id, out n_records, out interlace, fields, out vdata_size, vdata_name);
                istat = HDF4API.VSgetclass(vdata_id, vdata_class);
                vdata_tag = HDF4API.VSQuerytag(vdata_id);
                //HDF4API.VSgetinterlace(vdata_id);
                //HDF4API.VSsizeof(vdata_id, char *field_name_list);
                //HDF4API.VSgetname(vdata_id, vdata_name);
                H4Vdata vdata = new H4Vdata(this, vdata_name.ToString(), null, new long[] { 0, vdata_ref });
                vdata.Vdata_ref = vdata_ref;
                vdata.Vdata_id = vdata_id;
                vdata.N_records = n_records;
                vdata.Interlace = interlace;
                vdata.Fields = fields.ToString().Split(',');
                vdata.Vdata_size = vdata_size;
                vdata.Vdata_name = vdata_name.ToString();
                vdata.Vdata_tag = vdata_tag;
                _vdatas.Add(vdata);
                return vdata;
            }
            finally
            {
                if (vdata_id >= 0)
                    HDF4API.VSdetach(vdata_id);
            }
        }

        public string Filename
        {
            get { return _filename; }
        }

        public bool FileOpenState
        {
            get { return _fileOpenState; }
        }

        public int SD_id
        {
            get { return _sd_id; }
        }

        public int Num_Datasets
        {
            get { return _num_datasets; }
        }

        public int Num_Global_Attrs
        {
            get { return _num_global_attrs; }
        }

        public H4SDS[] Datasets
        {
            get { return _datasets.ToArray(); }
        }

        public HDFAttribute[] GlobalAttrs
        {
            get { return _global_attrs.ToArray(); }
        }

        public H4Vdata[] VDatas
        {
            get { return _vdatas.ToArray(); }
        }

        public void Dispose()
        {
            if (_file_id != -1)
            {
                //释放内存
            }
            int istat = HDF4Helper.SDend(_sd_id);
            istat = HDF4API.Vend(_file_id);
            istat = HDF4API.Hclose(_file_id);
            if (_datasets != null)
            {
                foreach (H4SDS sds in _datasets)
                {
                    sds.Dispose();
                }
            }
            if (_vdatas != null)
            {
                foreach (H4Vdata vdata in _vdatas)
                {
                    vdata.Dispose();
                }
            }
        }

        public HObject Get(string path)
        {
            //if (objList == null) {
            //    objList = new Vector();
            //}

            if (string.IsNullOrWhiteSpace(path))
                return null;
            path = path.Replace('\\', '/');
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            string name = null, pPath = null;
            bool isRoot = false;
            if (path.Equals("/"))
            {
                name = "/"; // the root
                isRoot = true;
            }
            else
            {
                if (path.EndsWith("/"))
                {
                    path = path.Substring(0, path.Length - 2);
                }
                int idx = path.LastIndexOf('/');
                name = path.Substring(idx + 1);
                if (idx == 0)
                {
                    pPath = "/";
                }
                else
                {
                    pPath = path.Substring(0, idx);
                }
            }
            HObject obj = null;
            bool isReadOnly = false;
            if (_file_id < 0)
            {
                _file_id = HDF4API.Hopen(_filename, DFACC.DFACC_WRITE, 0);
                if (_file_id < 0)
                {
                    isReadOnly = true;
                    _file_id = HDF4API.Hopen(_filename, DFACC.DFACC_READ, 0);
                }
                HDF4API.Vstart(_file_id);
                //grid = HDF4API.GRstart(_file_id);
                _sd_id = HDF4Helper.SDstart(_filename, HDF4Helper.AccessCodes.DFACC_READ);
            }
            //if (isRoot)
            //{
            //    obj = getRootGroup();
            //}
            //else
            //{
            //    obj = getAttachedObject(pPath, name);
            //}
            return obj;
        }
    }
}
