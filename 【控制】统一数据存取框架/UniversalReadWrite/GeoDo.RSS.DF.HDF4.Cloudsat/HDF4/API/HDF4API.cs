/* 读取VData的例子（c语言）：
 * 打开HDF文件并初始化VSET接口。
 * 获取第一个VDATA使用的参考号VSgetid
 * Attaches to the first Vdata in read mode.
 * 获取关于使用VDATA信息VSinquire
 * 获取VDATA的使用类名VSgetclass
 * 确定哪些字段用来阅读VSsetfields
 * 读取数据到缓冲区中，使用VSread
 * 解压使用数据VSfpack（vsfnpak / vsfcpak），然后打印出数据。
 * 从Vdata分离,关闭接口和文件 Detach from the Vdata, close the interface and the file.
 * http://www.hdfgroup.org/training/HDFtraining/tutorial/vdata/vd_ex5.html
    file_id = Hopen("VD_Ex4.hdf", DFACC_READ, 0);
    istat = Vstart(file_id);
    vdata_ref = -1;
    vdata_ref =  VSgetid(file_id, vdata_ref);
    vdata_id = VSattach(file_id, vdata_ref, "r");
    for (i=0; i < 60; i++)
        fields[i] = '\0';
    istat =VSinquire(vdata_id, &n_records, &interlace, fields, &vdata_size, vdata_name);
    istat = VSgetclass(vdata_id, vdata_class);
    istat = VSsetfields(vdata_id, fields);
    istat = VSread(vdata_id, (VOIDP)databuf, n_records, FULL_INTERLACE);
    istat = VSfpack(vdata_id, _HDF_VSUNPACK, fields, databuf,
                     bufsz, n_records, "Ident,Speed,Height,Temp",
                          fldbufptrs);
    istat = VSdetach(vdata_id);
    istat = Vend(file_id);
    istat = Hclose(file_id);
 * */

/*
 * Tag/ref (Data Identifier)
 * A tag and its associated reference number, abbreviated as tag/ref, uniquely identify a data element
 * in an HDF file. The tag/ref combination is also known as a data identifier.
 * Only the full tag/ref uniquely identifies a data element.
 * 
 * **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.DF.HDF4.Cloudsat
{
    public static class HDF4API
    {
        #region VData(Table)
        private const string hd426m = "hd426m.dll";
        private const string hm426m = "hm426m.dll";
        /*VData说明
         * 一个VData类似于一个表格
         * 有表头表格数据，类似下面的内容
         * field1   field2  field3
         * 12   12  12
         * 13   43  65
         * ...
         * */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file_id"></param>
        /// <returns></returns>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Vinitialize")]
        public static extern int Vstart(int file_id);

        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Vinitialize")]
        public static extern int VSlone(int file_id, int[] ref_array, int maxsize);

        /// <summary>
        /// VSgetid sequentially searches through a file identified by the parameter file_id and returns the reference number of the next vdata after the vdata that has reference number vdata_ref. This routine is generally used to sequentially search the file for vdatas. Searching past the last vdata in a file will result in an error condition.
        /// To initiate a search, this routine must be called with the value of vdata_ref equal to -1. Doing so returns the reference number of the first vdata in the file.
        /// </summary>
        /// <param name="file_id">File identifier returned by Hopen</param>
        /// <param name="vdata_ref">Vdata reference number</param>
        /// <returns>Returns the reference number for the next vdata if successful and FAIL (or -1) otherwise.</returns>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int VSgetid(int file_id, int vdata_ref);
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int VSgetname(int vdata_id, StringBuilder vdata_name);

        /// <summary>
        /// Searches an HDF file for a vdata with a given name.
        /// </summary>
        /// <param name="file_id"></param>
        /// <param name="vdata_name"></param>
        /// <returns></returns>
        /// <remarks>
        /// VSfind returns the reference number of the vdata with the name specified by the parameter vdata_name in the file specified by the parameter file_id. If there is more than one vdata with the same name, VSfind will only find the reference number of the first vdata in the file with that name.
        /// </remarks>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int VSfind(int file_id, string vdata_name);

        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int VSfexist(int vdata_id, string fieldname_list);

        /// <summary>
        /// Retrieves the index of a field within a vdata.
        /// 在vdata检索字段的索引。
        /// </summary>
        /// <param name="file_id"></param>
        /// <param name="fieldname"></param>
        /// <param name="field_index">OUT:Index of the field</param>
        /// <returns></returns>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int VSfindex(int file_id, string fieldname, out int field_index);

        /// <summary>
        /// Attaches to an existing vdata or creates a new vdata.
        /// </summary>
        /// <param name="file_id"></param>
        /// <param name="vdata_ref">Reference number of the vdata</param>
        /// <param name="access">Access mode</param>
        /// <returns>Returns a vdata identifier if successful and FAIL (or -1) otherwise.</returns>
        /// <remarks>
        /// VSattach attaches to the vdata identified by the reference number, vdata_ref, in the file identified by the parameter file_id. Access to the vdata is specified by the parameter access. VSattach returns an identifier to the vdata, through which all further operations on that vdata are carried out.
        /// An existing vdata may be multiply-attached for reads. Only one attach with write access to a vdata is allowed.
        /// The default interlace mode for a new vdata is FULL_INTERLACE (or 0). This may be changed using VSsetinterlace.
        /// The value of the parameter vdata_ref may be -1. This is used to create a new vdata.
        /// Valid values for access are "r" for read access and "w" for write access.
        /// If access is "r", then vdata_ref must be the valid reference number of an existing vdata returned from any of the vdata and vgroup search routines (e.g., Vgetnext or VSgetid). It is an error to attach to a vdata with a vdata_ref of -1 with "r" access.
        /// If access is "w", then vdata_ref must be the valid reference number of an existing vdata or -1. An existing vdata is generally attached with "w" access to replace part of its data, or to append new data to it.
        /// </remarks>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int VSattach(int file_id, int vdata_ref, string access);

        /// <summary>
        /// Retrieves general information about a vdata.
        /// </summary>
        /// <param name="vdata_id">IN:Vdata identifier returned by VSattach</param>
        /// <param name="n_records">OUT:Number of records</param>
        /// <param name="interlace_mode">OUT:Interlace mode of the data</param>
        /// <param name="field_name_list">OUT:List of field names,以逗号分割的vdata fields(e.g., "PX,PY,PZ")</param>
        /// <param name="vdata_size">OUT:Size of a record</param>
        /// <param name="vdata_name">OUT:Name of the vdata</param>
        /// <returns>Returns SUCCEED (or 0) if successful and FAIL (or -1) if it is unable to return any of the requested information.</returns>
        /// <remarks>
        /// VSinquire retrieves the number of records, the interlace mode of the data, the name of the fields, the size, and the name of the vdata, vdata_id, and stores them in the parameters n_records, interlace_mode, field_name_list, vdata_size, and vdata_name, respectively. In C, if any of the output parameters are NULL, the corresponding information will not be retrieved. Refer to the Reference Manual pages on VSelts, VSgetfields, VSgetinterlace, VSsizeof and VSgetname for other routines that can be used to retrieve specific information.
        /// Possible returned values for interlace_mode are FULL_INTERLACE (or 0) and NO_INTERLACE (or 1). The returned value of vdata_size is the number of bytes in a record and is machine-dependent.
        /// The parameter field_name_list is a character string that contains the names of all the vdata fields, separated by commas. (e.g., "PX,PY,PZ" in C and 'PX,PY,PZ' in Fortran).</remarks>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int VSinquire(int vdata_id, out int n_records, out int interlace_mode, StringBuilder field_name_list, out int vdata_size, StringBuilder vdata_name);

        /// <summary>
        /// Retrieves the vdata class name, if any.
        /// </summary>
        /// <param name="vdata_id"></param>
        /// <param name="vdata_class"></param>
        /// <returns>Returns SUCCEED (or 0) if successful and FAIL (or -1) otherwise.</returns>
        /// <remarks>VSgetclass检索所确定的参数的VDATA的类名vdata_id并在缓冲器中放置它vdata_class。
        /// 为缓冲空间vdata_class必须由调用程序之前被分配VSgetclass被调用。类名的最大长度是由宏定义VSNAMELENMAX（或64） 。
        /// VSgetclass retrieves the class name of the vdata identified by the parameter vdata_id and places it in the buffer vdata_class.
        /// Space for the buffer vdata_class must be allocated by the calling program before VSgetclass is called. The maximum length of the class name is defined by the macro VSNAMELENMAX (or 64).</remarks>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int VSgetclass(int vdata_id, StringBuilder vdata_class);

        /// <summary>
        /// Specifies the fields to be accessed.
        /// 要访问指定的字段。
        /// </summary>
        /// <param name="vdata_id">IN:Vdata identifier returned by VSattach</param>
        /// <param name="field_name_list">IN:List of the field names to be accessed</param>
        /// <returns></returns>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl, CharSet=CharSet.Ansi)]
        public static extern int VSsetfields(int vdata_id, string field_name_list);
        
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl, CharSet=CharSet.Ansi)]
        public static extern int VSsizeof(int vdata_id, string field_name_list);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vdata_id"></param>
        /// <param name="databuf">OUT:Buffer to store the retrieved data</param>
        /// <param name="n_records">Number of records to be retrieved</param>
        /// <param name="interlace_mode">Interlace mode of the data to be stored in the buffer</param>
        /// <returns>
        /// num_of_records:
        /// </returns>
        /// <remarks>
        /// VSread reads n_records records from the vdata identified by the parameter vdata_id and stores the data in the buffer databuf using the interlace mode specified by the parameter interlace_mode.
        /// The user can specify the fields and the order in which they are to be read by calling VSsetfields prior to reading. VSread stores the requested fields in databuf in the specified order.
        /// Valid values for interlace_mode are FULL_INTERLACE (or 1) and NO_INTERLACE (or 0). Selecting FULL_INTERLACE causes databuf to be filled by record and is recommended for speed and efficiency. Specifying NO_INTERLACE causes databuf to be filled by field, i.e., all values of a field in n_records records are filled before moving to the next field. Note that the default interlace mode of the buffer is FULL_INTERLACE.
        /// As the data is stored contiguously in the vdata, VSfpack should be used to unpack the fields after reading. Refer to the discussion of VSfpack in the HDF User's Guide for more information.
        /// </remarks>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int VSread(int vdata_id, IntPtr databuf, int n_records, INTERLACE_MODE interlace_mode);

        /// <summary>
        /// Packs field data into a buffer or unpacks buffered field data into vdata field(s) for fully interlaced fields.
        /// 包现场数据到缓冲区或缓冲解压现场数据到VDATA场（次）为全面隔行场。
        /// </summary>
        /// <param name="vdata_id"></param>
        /// <param name="action"></param>
        /// <param name="fields_in_buf"></param>
        /// <param name="buf">IN/OUT:Buffer containing the values of the packed fields to write to or read from the vdata</param>
        /// <param name="buf_size">以字节为单位的缓冲区大小</param>
        /// <param name="n_records"></param>
        /// <param name="field_name_list"></param>
        /// <param name="bufptrs">IN/OUT:Array of pointers to the field buffers</param>
        /// <returns></returns>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int VSfpack(int vdata_id, int action, StringBuilder fields_in_buf, byte[] buf,
            int buf_size, int n_records, StringBuilder field_name_list, byte[] bufptrs);

        /// <summary>
        /// Detaches from the current vdata, terminating further access to that vdata.
        /// 从当前vdata分离,终止进一步访问该VDATA。
        /// </summary>
        /// <param name="vdata_id"></param>
        /// <returns></returns>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int VSdetach(int vdata_id);

        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int VSnattrs(int vdata_id);

        /// <summary>
        /// Provides a mechanism for random-access I/O within a vdata.
        /// </summary>
        /// <param name="vdata_id">Vdata identifier returned by VSattach</param>
        /// <param name="record_pos">Position of the record</param>
        /// <returns>Returns the record position (zero or a positive integer) if successful and FAIL (or -1) otherwise.</returns>
        /// <remarks>
        /// VSseek moves the access pointer within the vdata identified by the parameter vdata_id to the position of the record specified by the parameter record_pos. The next call to VSread or VSwrite will read from or write to the record where the access pointer has been moved to.
        /// The value of record_pos is zero-based. For example, to seek to the third record in the vdata, set record_pos to 2. The first record position is specified by specifying a record_pos value of 0. Each seek is constrained to a record boundary within the vdata.
        /// </remarks>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int VSseek(int vdata_id, int record_pos);

        //VF
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int VFfieldisize(int vdata_id, int field_index);
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int VFfieldesize(int vdata_id, int field_index);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vdata_id"></param>
        /// <param name="field_index"></param>
        /// <returns>
        /// IntPtr ptr;
        /// string ret = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ptr);
        /// </returns>
        [DllImport(hd426m, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl,SetLastError=true)]
        //[return: MarshalAs(UnmanagedType.LPStr)]
        public static extern IntPtr VFfieldname(int vdata_id, int field_index);
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int VFfieldorder(int vdata_id, int field_index);
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int VFfieldtype(int vdata_id, int field_index);
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int VFnfields(int vdata_id);
        //VSQ
        //[DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        //public static extern int VSQuerycount(int vdata_id, out int n_records);
        //[DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        //public static extern int VSQueryfields(int vdata_id, StringBuilder field_name_list);
        //[DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        //public static extern int VSQueryinterlace(int vdata_id, out int interlace_mode);
        //[DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        //public static extern int VSQueryname(int vdata_id, StringBuilder vdata_name);
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int VSQueryref(int vdata_id);
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int VSQuerytag(int vdata_id);

        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int VSQueryvsize(int vdata_id, ref int[] vdata_size);

        /// <summary>
        /// Terminates access to a vgroup and/or vdata interface.
        /// 终止访问vgroup和/或vdata接口。
        /// C：Vend
        /// </summary>
        /// <param name="file_id"></param>
        /// <returns></returns>
        /// <remarks>
        /// Vend terminates access to the vgroup and/or vdata interfaces initiated by Vstart and all internal data structures allocated by Vstart.
        /// Vend must be called after all vdata and vgroup operations on the file file_id are completed. Further attempts to use vdata or vgroup routines after calling Vend will result in a FAIL (or -1) being returned.
        /// </remarks>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Vfinish")]
        public static extern int Vend(int file_id);
        #endregion

        //Access/Create
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Vattach(int file_id, int vgroup_ref, string vg_access_mode);
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Vdetach(int vgroup_id);
        //Manipulation

        //Vgroup Inquiry
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Vfind(int file_id, StringBuilder vgroup_name);
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Vfindclass(int file_id, StringBuilder vgroup_class);
        /// <summary>
        /// Retrieves the class name of a vgroup.
        /// </summary>
        /// <param name="vgroup_id"></param>
        /// <param name="vgroup_class"></param>
        /// <returns></returns>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Vgetclass(int vgroup_id, StringBuilder vgroup_class);
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Vgetclassnamelen(int vgroup_id, IntPtr classname_len);
        /// <summary>
        /// Returns the reference number of the next vgroup.
        /// </summary>
        /// <param name="file_id"></param>
        /// <param name="vgroup_ref"></param>
        /// <returns>Vgetid</returns>
        /// <remarks>sequentially searches through an HDF file to obtain the reference number of the vgroup immediately following the vgroup specified by the reference number</remarks>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Vgetid(int file_id, int vgroup_ref);
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Vgetname(int vgroup_id, StringBuilder vgroup_name);
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Vgetnamelen(int vgroup_id, IntPtr name_len);
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Vgetversion(int file_id);
        /// <summary>
        /// //finds the next vgroup or vdata and returns its reference number.
        /// </summary>
        /// <param name="vgroup_id"></param>
        /// <param name="v_ref"></param>
        /// <returns></returns>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Vgetnext(int vgroup_id, out int v_ref);
        /// <summary>
        /// Testing Whether a Data Object Belongs to a Vgroup
        /// </summary>
        /// <param name="vgroup_id"></param>
        /// <param name="obj_tag"></param>
        /// <param name="obj_ref"></param>
        /// <returns></returns>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Vinqtagref(int vgroup_id, int obj_tag, int obj_ref);
        /// <summary>
        /// Retrieves the number of entries in a vgroup and its name.
        /// </summary>
        /// <param name="file_id"></param>
        /// <param name="n_members"></param>
        /// <param name="vgroup_name"></param>
        /// <returns></returns>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Vinquire(int vgroup_id, out int n_entries, StringBuilder vgroup_name);//Retrieves general information about a vgroup
        /// <summary>
        /// Retrieves the reference numbers of lone vgroups, i.e., vgroups that are at the top of the grouping hierarchy, in a file.
        /// </summary>
        /// <param name="file_id"></param>
        /// <param name="ref_array"></param>
        /// <param name="maxsize"></param>
        /// <returns></returns>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Vlone(int file_id, int[] ref_array, int max_refs);

        /// <summary>
        /// Returns the number of attributes assigned to a vgroup.
        /// </summary>
        /// <param name="vgroup_id"></param>
        /// <returns></returns>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Vnattrs(int vgroup_id);
        /// <summary>
        /// Returns the number of tags of a given tag type in a vgroup.
        /// </summary>
        /// <param name="vgroup_id"></param>
        /// <param name="tag_type"></param>
        /// <returns></returns>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Vnrefs(int vgroup_id, int tag_type);
        /// <summary>
        /// Retrieving the Reference Number of a Vgroup: VQueryref
        /// </summary>
        /// <param name="vgroup_id"></param>
        /// <returns></returns>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Vntagrefs(int vgroup_id);//
        /// <summary>
        /// returns the reference number of the vgroup identified by the parameter vgroup_id, or FAIL (or -1) if unsuccessful.
        /// </summary>
        /// <param name="vgroup_id"></param>
        /// <returns></returns>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int VQueryref(int vgroup_id);
        /// <summary>
        /// Returns the tag of a vgroup.
        /// </summary>
        /// <param name="vgroup_id"></param>
        /// <returns></returns>
        /// <remarks>returns DFTAG_VG (or 1965), which would be the tag of the vgroup identified by the parameter vgroup_id, or FAIL (or -1) if unsuccessful.</remarks>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int VQuerytag(int vgroup_id);
        //Member Inquiry
        //Vflocate()
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vgroup_id"></param>
        /// <param name="v_ref">IN:Reference number of the vgroup or vdata</param>
        /// <returns></returns>
        /// <remarks>
        /// Vgetnext searches in the vgroup identified by the parameter vgroup_id for the object following the object specified by its reference number v_ref. Either of the two objects can be a vgroup or a vdata. If v_ref is set to -1, the routine will return the reference number of the first vgroup or vdata in the vgroup
        /// Note that this routine only gets a vgroup or a vdata in a vgroup. Vgettagrefs gets any object in a vgroup.
        /// </remarks>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Vgetnext(int vgroup_id, int v_ref);
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Vgettagref(int vgroup_id, int index, out int tag, out int ref_id);
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Vgettagrefs(int vgroup_id, int[] tag_array, int[] ref_array, int num_of_pairs);//Returns the number of tag/reference number pairs obtained from a vgroup if successful and FAIL (or -1) otherwise.

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vgroup_id"></param>
        /// <param name="obj_ref"></param>
        /// <returns>is_group: Returns TRUE (or 1) if the object is a vgroup and FALSE (or 0) otherwise.</returns>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Visvg(int vgroup_id, int obj_ref);//determines if the object specified by the reference number
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vgroup_id"></param>
        /// <param name="obj_ref"></param>
        /// <returns>is_vdata: Returns TRUE (or 1) if the object is a vdata and FALSE (or 0) otherwise.</returns>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Visvs(int vgroup_id, int obj_ref);//determines if the object specified by the reference number
        /*Attributes*/
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Vattrinfo(int file_id);//
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Vfindattr(int file_id);//
        /// <summary>
        /// Retrieves the values of a vgroup attribute.
        /// </summary>
        /// <param name="vgroup_id"></param>
        /// <param name="attr_index"></param>
        /// <param name="attr_values"></param>
        /// <returns></returns>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Vgetattr(int vgroup_id, int attr_index, IntPtr attr_values);
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Vsetattr(int vgroup_id);//

        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Vaddtagref(int vgroup_id, int DFTAG_NDG, int sds_ref);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="access">DFACC_READ=1,DFACC_WRITE=2,DFACC_CREATE=4</param>
        /// <param name="n_dds"></param>
        /// <returns>file_id:</returns>
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Hopen(string filename, DFACC access, int n_dds);
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Hclose(int file_id);

        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Hgetlibversion(out int major_v, out int minor_v, out int release, StringBuilder libraryversion);
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Hgetfileversion(int file_id, out int major_v, out int minor_v, out int release, StringBuilder libraryversion);

        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Hishdf(string filename);

        //
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GRstart(int file_id);

        //H
        [DllImport(hd426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Hgetelement(int file_id, int tag, int ref_number, int[] data);

        //SD
        [DllImport(hm426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDidtoref(int sds_id);
        [DllImport(hm426m, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDreftoindex(int sd_id, int obj_ref);
    }

    /// <summary>
    /// 预定义值，来自hdf.h
    /// </summary>
    public enum DFACC
    {
        /* internal file access codes */
        DFACC_READ = 1,
        DFACC_WRITE = 2,
        DFACC_CREATE = 4,
        DFACC_ALL = 7,
        DFACC_RDONLY = 1,
        DFACC_RDWR = 3,
        DFACC_CLOBBER = 4,
        /* New file access codes (for Hstartaccess only, currently) */
        DFACC_BUFFER = 8,/* buffer the access to this AID */
        DFACC_APPENDABLE = 0x10,/* make this AID appendable */
        DFACC_CURRENT = 0x20/* start looking for a tag/ref from the current */
    }

    /// <summary>
    /// interlacing supported by the vset.来自hdf.h
    /// </summary>
    public enum INTERLACE_MODE
    {
        FULL_INTERLACE = 0,
        NO_INTERLACE = 1
    }
}
