/*============================================================================
 * AUTHOR: Eric G. Miller
 * DATE: 2004-09-15
 * PURPOSE: Performs some tests of the ProjApi wrapper
 * COPYRIGHT: Copyright (c) 2004, 2010 California Department of Fish and Game
 * LICENSE: MIT Style
 *  Permission is hereby granted, free of charge, to any person obtaining a
 *  copy of this software and associated documentation files (the "Software"),
 *  to deal in the Software without restriction, including without limitation
 *  the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * 	and/or sell copies of the Software, and to permit persons to whom the
 * 	Software is furnished to do so, subject to the following conditions:
 * 
 * 	The above copyright notice and this permission notice shall be included
 * 	in all copies or substantial portions of the Software.
 * 
 * 	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * 	OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * 	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * 	THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * 	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * 	FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * 	DEALINGS IN THE SOFTWARE.
 * 	
 *  CHANGES:
 *  For Proj-4.7.0 (2010-01-29, Eric G. Miller)
 *      - Modified for changes introduced in the pj_api.cs file.
 *===========================================================================*/


using System;
using System.Runtime.InteropServices;
using GeoDo.Project;

public class ProjApiTest {

	/// <summary>
	/// Some testing of the proj_api classes
	/// </summary>
	[STAThread]
	public static void Main(string[] args) {
		IntPtr projPJ = Proj.pj_init_plus(String.Join(" ", args));
		if (projPJ != IntPtr.Zero) {
			Console.WriteLine("Initialization succeeded!");
			Console.WriteLine(Proj.pj_get_def(projPJ, 0));
			Proj.pj_free(projPJ);
		}
		else {
			Console.WriteLine("Initialization failed!");
            int errno = Proj4Projection.GetErrNo();
            if (errno != 0)
            {
                string message = Proj4Projection.GetErrorMessage(errno);
                Console.WriteLine(message);
            }
		}
		// begin tests
		int ntests = 0;
		int nsuccess = 0;

		ntests++;
		Console.Write("Testing pj_set_searchpath ... ");
		string[] path = new string[] {System.Environment.CurrentDirectory};
		Proj.pj_set_searchpath(path.Length, path);
		Console.WriteLine("succeeded");
		nsuccess++;
		
		ntests++;
		Console.Write("Testing pj_init ...");
		if (Test_pj_init()) {
			nsuccess++;
			Console.WriteLine("succeeded");
		}
		else {
			Console.WriteLine("failed");
		}

		ntests++;
		Console.Write("Testing pj_init_plus ...");
		if (Test_pj_init_plus()) {
			nsuccess++;
			Console.WriteLine("succeeded");
		}
		else {
			Console.WriteLine("failed");
		}

		ntests++;
		Console.Write("Testing pj_fwd ...");
		if (Test_pj_fwd()) {
			nsuccess++;
			Console.WriteLine("succeeded");
		}
		else {
			Console.WriteLine("failed");
		}

		ntests++;
		Console.Write("Testing pj_inv ...");
		if (Test_pj_inv()) {
			nsuccess++;
			Console.WriteLine("succeeded");
		}
		else {
			Console.WriteLine("failed");
		}

		ntests++;
		Console.Write("Testing pj_transform ...");
		if (Test_pj_transform()) {
			nsuccess++;
			Console.WriteLine("succeeded");
		}
		else {
			Console.WriteLine("failed");
		}

		ntests++;
		Console.Write("Testing pj_geocentric_to_geodetic ...");
		if (Test_pj_geocentric_to_geodetic()) {
			nsuccess++;
			Console.WriteLine("succeeded");
		}
		else {
			Console.WriteLine("failed");
		}

		ntests++;
		Console.Write("Testing pj_geodetic_to_geocentric ...");
		if (Test_pj_geodetic_to_geocentric()) {
			nsuccess++;
			Console.WriteLine("succeeded");
		}
		else {
			Console.WriteLine("failed");
		}

		ntests++;
		Console.Write("Testing pj_deallocate_grids ...");
		if (Test_pj_deallocate_grids()) {
			nsuccess++;
			Console.WriteLine("succeeded");
		}
		else {
			Console.WriteLine("failed");
		}

		ntests++;
		Console.Write("Testing pj_is_latlong ...");
		if (Test_pj_is_latlong()) {
			nsuccess++;
			Console.WriteLine("succeeded");
		}
		else {
			Console.WriteLine("failed");
		}

		ntests++;
		Console.Write("Testing pj_is_geocent ...");
		if (Test_pj_is_geocent()) {
			nsuccess++;
			Console.WriteLine("succeeded");
		}
		else {
			Console.WriteLine("failed");
		}

		ntests++;
		Console.Write("Testing pj_pr_list ...");
		if (Test_pj_pr_list()) {
			nsuccess++;
			Console.WriteLine("succeeded");
		}
		else {
			Console.WriteLine("failed");
		}

		ntests++;
		Console.Write("Testing pj_get_def ...");
		if (Test_pj_get_def()) {
			nsuccess++;
			Console.WriteLine("succeeded");
		}
		else {
			Console.WriteLine("failed");
		}
		
		ntests++;
		Console.Write("Testing pj_latlong_from_proj ...");
		if (Test_pj_latlong_from_proj()) {
			nsuccess++;
			Console.WriteLine("succeeded");
		}
		else {
			Console.WriteLine("failed");
		}

		ntests++;
		Console.Write("Testing pj_get_release ...");
		if (Test_pj_get_release()) {
			nsuccess++;
			Console.WriteLine("succeeded");
		}
		else {
			Console.WriteLine("failed");
		}
		
		ntests++;
		Console.Write("Testing pj_datum_transform ...");
		if (Test_pj_datum_transform()) {
			nsuccess++;
			Console.WriteLine("succeeded");
		}
		else {
			Console.WriteLine("failed");
		}

		ntests++;
		Console.Write("Testing pj_compare_datums ...");
		if (Test_pj_compare_datums()) {
			nsuccess++;
			Console.WriteLine("succeeded");
		}
		else {
			Console.WriteLine("failed");
		}

		ntests++;
		Console.Write("Testing pj_malloc (pj_dalloc) ...");
		if (Test_pj_alloc()) {
			nsuccess++;
			Console.WriteLine("succeeded");
		}
		else {
			Console.WriteLine("failed");
		}

		Console.WriteLine("{0} Tests, {1} Successes, {2} Failures",
			ntests, nsuccess, ntests - nsuccess);

		// Test high level API
		Console.WriteLine("\nTesting Projection API");

		Test_Projection_API();

		Console.ReadLine();
	}

	static void WriteProjError(string test) {
		Console.Error.WriteLine("Test {0} failed!", test);
        int errno = Proj4Projection.GetErrNo();
        if (errno != 0)
        {
            string message = Proj4Projection.GetErrorMessage(errno);
            Console.Error.WriteLine(message);
        }
	}

	static bool Test_pj_init() {
		string[] args = {"+proj=lcc", "+lat_1=33", 
							"+lat_2=45", "+datum=NAD27", "+nodefs"};
		IntPtr projPJ = Proj.pj_init(args.Length, args);
		if (projPJ == IntPtr.Zero) {
			WriteProjError("pj_init");
			return false;
		}
		else {
			Proj.pj_free(projPJ);
			return true;
		}
	}

	static bool Test_pj_init_plus() {
		string arg = "+proj=lcc +lat_1=33 +lat_2=45 +datum=NAD27 +nodefs";
		IntPtr projPJ = Proj.pj_init_plus(arg);
		if (projPJ == IntPtr.Zero) {
			WriteProjError("pj_init_plus");
			return false;
		}
		else {
			Proj.pj_free(projPJ);
			return true;
		}
	}

	static bool Test_pj_fwd() {
		IntPtr projPJ = Proj.pj_init_plus(@"+proj=aea +lat_0=0 +lon_0=-120 "
			+ @"+lat_1=34 +lat_2=40.5 +y_0=-4000000 +datum=NAD83");
		if (projPJ == IntPtr.Zero) {
			WriteProjError("pj_fwd (pj_init_plus)");
			return false;
		}
		else {
			ProjUV uv;
			uv.u = -120.0 * Proj.DEG_TO_RAD;
			uv.v = 34.0 * Proj.DEG_TO_RAD;
			uv = Proj.pj_fwd(uv, projPJ);
			Proj.pj_free(projPJ);
			if (uv.u == System.Double.MaxValue || uv.v == System.Double.MaxValue) {
				WriteProjError("pj_fwd");
				return false;
			}
			else {
				Console.Write(" ({0},{1}) ", uv.u, uv.v);
				return true;
			}
		}
	}

	static bool Test_pj_inv() {
		IntPtr projPJ = Proj.pj_init_plus(@"+proj=aea +lat_0=0 +lon_0=-120 "
			+ @"+lat_1=34 +lat_2=40.5 +y_0=-4000000 +datum=NAD83");
		if (projPJ == IntPtr.Zero) {
			WriteProjError("pj_inv (pj_init_plus)");
			return false;
		}
		else {
			ProjUV uv;
			uv.u = 0.0;
			uv.v = -446166.97;
			uv = Proj.pj_inv(uv, projPJ);
			Proj.pj_free(projPJ);
			if (uv.u == System.Double.MaxValue || uv.v == System.Double.MaxValue) {
				WriteProjError("pj_inv");
				return false;
			}
			else {
				Console.Write(" ({0},{1}) ", 
					uv.u * Proj.RAD_TO_DEG , uv.v * Proj.RAD_TO_DEG);
				return true;
			}
		}
	}

	static bool Test_pj_transform() {
		double[] x = {-119, -120, -121};
		double[] y = {38, 39, 40};
		for (int i = 0; i < x.Length; i++) {
			x[i] *= Proj.DEG_TO_RAD;
			y[i] *= Proj.DEG_TO_RAD;
		}
		IntPtr src = Proj.pj_init_plus("+proj=latlong +datum=NAD27 +nadgrids=conus +nodefs");
		IntPtr dst = Proj.pj_init_plus(@"+proj=aea +lat_0=0 +lon_0=-120 "
			+ @"+lat_1=34 +lat_2=40.5 +y_0=-4000000 +datum=NAD83 +nodefs");
		int errno = Proj.pj_transform(src, dst, x.Length, 1, x, y, null);
		Proj.pj_free(src);
		Proj.pj_free(dst);
		if (errno != 0) {
			WriteProjError("pj_transform");
			return false;
		}
		else {
			return true;
		}
	}

	static bool Test_pj_geocentric_to_geodetic() { return false; }
	static bool Test_pj_geodetic_to_geocentric() { return false; }
	static bool Test_pj_deallocate_grids() {
		Proj.pj_deallocate_grids();
		return true;
	}
	static bool Test_pj_is_latlong() {
		IntPtr projPJ = Proj.pj_init_plus("+proj=latlong +datum=NAD83 +nodefs");
		if (0 == Proj.pj_is_latlong(projPJ)) {
			WriteProjError("pj_is_latlong");
			Proj.pj_free(projPJ);
			return false;
		}
		else {
			Proj.pj_free(projPJ);
			return true;
		}
	}
	static bool Test_pj_is_geocent() {
		IntPtr projPJ = Proj.pj_init_plus("+proj=latlong +datum=NAD83 +nodefs");
		if (0 != Proj.pj_is_geocent(projPJ)) {
			WriteProjError("pj_is_geocent");
			Proj.pj_free(projPJ);
			return false;
		}
		else {
			Proj.pj_free(projPJ);
			return true;
		}
	}
	static bool Test_pj_pr_list() {
		IntPtr projPJ = Proj.pj_init_plus("+proj=latlong +datum=NAD83 +nodefs");
		Proj.pj_pr_list(projPJ);
		Proj.pj_free(projPJ);
		return true;
	}
	static bool Test_pj_get_def() {
		bool testResult = false;
        string orig = @"+proj=aea +lat_0=0 +lon_0=-120 +lat_1=34 +lat_2=40.5 +y_0=-4000000 +datum=NAD83 +nodefs";
		IntPtr projPJ = Proj.pj_init_plus(orig);
        IntPtr pDef = Proj.pj_get_def(projPJ, 0);
        string result = Marshal.PtrToStringAnsi(pDef);
        Proj.pj_dalloc(pDef);
        IntPtr pErrno = Proj.pj_get_errno_ref();
        int errno = Marshal.ReadInt32(pErrno);
        if (errno != 0)
        {
            WriteProjError("pj_get_def");
        }
        else
        {
            Console.Error.WriteLine("ORIGINAL: {0}\nRESULT: {1}",
                orig, result);
            testResult = true;
        }
        Proj.pj_free(projPJ);
        return testResult;
	}

    static bool Test_pj_latlong_from_proj() {
		string orig = @"+proj=aea +lat_0=0 +lon_0=-120 +lat_1=34 +lat_2=40.5 +y_0=-4000000 +datum=NAD83 +nodefs";
		IntPtr projPJ = Proj.pj_init_plus(orig);
		IntPtr dstPJ = Proj.pj_latlong_from_proj(projPJ);
        IntPtr pDef = Proj.pj_get_def(dstPJ, 0);
        string result = Marshal.PtrToStringAnsi(pDef);
        Proj.pj_dalloc(pDef);
		Proj.pj_free(projPJ);
		Proj.pj_free(dstPJ);
		if (result == null) {
			WriteProjError("pj_latlong_from_proj");
			return false;
		}
		else {
			Console.WriteLine(result);
			return true;
		}
	}

	static bool Test_pj_get_release() {
		bool result = true; 
		try {
            // Don't free result
            IntPtr pRelease = Proj.pj_get_release();
            string release = Marshal.PtrToStringAnsi(pRelease);
			Console.WriteLine(release);
		}
		catch { result = false; }
		return result;
	}

	static bool Test_pj_datum_transform() {
		bool result = true;
		try {
			IntPtr src = Proj.pj_init_plus("+proj=latlong +datum=NAD27 +nadgrids=conus +nodefs");
			IntPtr dst = Proj.pj_init_plus("+proj=latlong +datum=NAD83 +nodefs");
			double[] x = {-120.125 * Proj.DEG_TO_RAD};
			double[] y = {38.5 * Proj.DEG_TO_RAD};
			Console.WriteLine("\t({0}, {1})", x[0] * Proj.RAD_TO_DEG, y[0] * Proj.RAD_TO_DEG);
			int errno = Proj.pj_datum_transform(src, dst, 1, 1, x, y, null);
			if (errno != 0) {
				WriteProjError("pj_datum_transform");
				result = false;
			}
			else {
				Console.WriteLine("\n\t({0}, {1})", x[0] * Proj.RAD_TO_DEG, y[0] * Proj.RAD_TO_DEG);
			}
			Proj.pj_free(src);
			Proj.pj_free(dst);
		}
		catch (Exception e) {
			Console.Error.WriteLine(e.Message);
			result = false; 
		}
		return result;
	}
	static bool Test_pj_compare_datums() {
		bool result = true;
		try {
			IntPtr src = Proj.pj_init_plus("+proj=latlong +datum=NAD27 +nadgrids=conus +nodefs");
			IntPtr dst = Proj.pj_init_plus("+proj=latlong +datum=NAD83 +nodefs");
			int same = Proj.pj_compare_datums(src, dst);
			if (same != 0) {
				WriteProjError("pj_compare_datums");
				result = false;
			}
			Proj.pj_free(src);
			Proj.pj_free(dst);
		}
		catch (Exception e) {
			Console.Error.WriteLine(e.Message);
			result = false; 
		}
		return result;
	}
	static bool Test_pj_alloc() {
		bool result = true;
		try {
			IntPtr memory = Proj.pj_malloc(512);
			if (memory == null) {
				result = false;
				WriteProjError("pj_malloc");
			}
			else {
				Proj.pj_dalloc(memory);
			}
		}
		catch (Exception e) {
			Console.Error.WriteLine(e.Message);
			result = false;
		}
		return result;
	}

	static void Test_Projection_API() {
		Proj4Projection.SetSearchPath(new string[] {System.Environment.CurrentDirectory});

        using (
        Proj4Projection src = new Proj4Projection(@"+proj=latlong +datum=NAD27 +nadgrids=conus +nodefs"),
                    dst = new Proj4Projection(@"+proj=aea +lat_0=0 +lon_0=-120 +lat_1=34 +lat_2=40.5 +y_0=-4000000 +datum=NAD83 +nodefs"))
        {

            Console.WriteLine("src.Definition = {0}", src.Definition);
            Console.WriteLine("dst.Definition = {0}", dst.Definition);
            Console.WriteLine("src (.ToString()) = {0}", src);
            Console.WriteLine("dst (.ToString()) = {0}", dst);
            Console.WriteLine("latlong.Definition = {0}", dst.GetLatLong().Definition);
            Console.WriteLine("src.IsLatLong = {0}", src.IsLatLong);
            Console.WriteLine("dst.IsLatLong = {0}", dst.IsLatLong);
            Console.WriteLine("src.IsGeoCentric = {0}", src.IsGeoCentric);
            Console.WriteLine("dst.IsGeoCentric = {0}", dst.IsGeoCentric);
            Console.WriteLine("src.GetType() = {0}", src.GetType());

            double[] x = { -116, -117, -118, -119, -120, -121 };
            double[] y = { 34, 35, 36, 37, 38, 39 };
            double[] z = { 0, 10, 20, 30, 40, 50 };

            Console.WriteLine("Original Coordinates:");
            for (int i = 0; i < x.Length; i++)
            {
                Console.WriteLine("\t({0}, {1}, {2})", x[i], y[i], z[i]);
            }

            Console.WriteLine("Trying Projection.Transform(src, dst, x, y)");
            try
            {
                Proj4Projection.Transform(src, dst, x, y);
                Console.WriteLine("Transformed Coordinates:");
                for (int i = 0; i < x.Length; i++)
                {
                    Console.WriteLine("\t({0}, {1}, {2})", x[i], y[i], z[i]);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Exception: {0}", e.Message);
                return;
            }

            Console.WriteLine("Trying Projection.Transform(dst, src, x, y, z)");
            try
            {
                Proj4Projection.Transform(dst, src, x, y, z);
                Console.WriteLine("Original Coordinates ?");
                for (int i = 0; i < x.Length; i++)
                {
                    Console.WriteLine("\t({0}, {1}, {2})", x[i], y[i], z[i]);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Exception: {0}", e.Message);
                return;
            }

            Console.WriteLine("Trying src.Transform(dst, x, y, z)");
            try
            {
                src.Transform(dst, x, y, z);
                Console.WriteLine("Transformed Coordinates:");
                for (int i = 0; i < x.Length; i++)
                {
                    Console.WriteLine("\t({0}, {1}, {2})", x[i], y[i], z[i]);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Exception: {0}", e.Message);
                return;
            }

            Console.WriteLine("Trying dst.Transform(src, x, y)");
            try
            {
                dst.Transform(src, x, y);
                Console.WriteLine("Original Coordinates ?");
                for (int i = 0; i < x.Length; i++)
                {
                    Console.WriteLine("\t({0}, {1}, {2})", x[i], y[i], z[i]);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Exception: {0}", e.Message);
                return;
            }

        }
	}

}
