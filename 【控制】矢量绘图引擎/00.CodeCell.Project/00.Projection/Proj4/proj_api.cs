/*============================================================================
 * AUTHOR: Eric G. Miller
 * DATE: 2004-09-15
 * PURPOSE: 
 *  Provide both high-level and low-level interfaces to the PROJ.4
 *  projection library.  This interface was written against a slightly
 *  modified copy of version 4.4.8.  Any significant changes were sent to
 *  upstream for incorporation into later versions.
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
 *  For 4.7.0 (2010-01-29, Eric G. Miller)
 *      - Change definition of pj_alloc and pj_dalloc to work with IntPtr's
 *      - Change return values of pj_get_def, pj_strerrno, pj_release to
 *        IntPtr's. Use Marshal.PtrToStringAnsi. Free the IntPtr from pj_get_def
 *        using either pj_dalloc or Marshal.FreeHGlobal.  The results from
 *        the other two functions should not be released (static data).
 *      - Add GetErrorMessage and GetErrno helpers to Projection class.
 *      - Removed all unsafe code
 *      - Implemented IDisposable on the Projection class
 *      - Other minor modifications related to the changes noted above.
 *===========================================================================*/

using System;
using System.Runtime.InteropServices;

namespace CodeCell.AgileMap.Core
{
	
	/// <summary>
	/// Common struct for data points
	/// </summary>
	[ StructLayout(LayoutKind.Sequential) ]
	public struct ProjUV {
		public double u;
		public double v;
	}

	/// <summary>
	/// This class thinly wraps proj.4 library functions. Some of it uses
	/// unsafe code.
	/// </summary>
	public class Proj
	{
		/// <summary>
		/// Constants for converting coordinates between radians and degrees
		/// </summary>
		public const double RAD_TO_DEG = 57.29577951308232;
		public const double DEG_TO_RAD = .0174532925199432958;
		
		/// <summary>
		/// The finder function is used by the projection library to locate
		/// resources like datum shift files and projection configuration
		/// files. (NOTE: Not functional due to calling convention).
		/// </summary>
		public delegate IntPtr FinderFunction([MarshalAs(UnmanagedType.LPStr)] string path); 

        ///// <summary>
        ///// Pointer to the global error number
        ///// </summary>
        //public unsafe static int* pj_errno = pj_get_errno_ref();

		/// <summary>
		/// Perform a forward projection (from lat/long).
		/// </summary>
		/// <param name="LP">The lat/long coordinate in radians</param>
		/// <param name="projPJ">The projection definition</param>
		/// <returns>The projected coordinate in system units</returns>
		[DllImport("proj.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
		public static extern ProjUV pj_fwd(ProjUV LP, IntPtr projPJ);

		/// <summary>
		/// Perform an inverse projection (to lat/long).
		/// </summary>
		/// <param name="XY">The projected coordinate in system units</param>
		/// <param name="projPJ">The projection definition</param>
		/// <returns>The lat/long coordinate in radians</returns>
		[DllImport("proj.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
		public static extern ProjUV pj_inv(ProjUV XY, IntPtr projPJ);

		/// <summary>
		/// Transform a set of coordinates from one system to another (includes datum transformation)
		/// </summary>
		/// <param name="src">Source coordinate system</param>
		/// <param name="dst">Destination coordinate system</param>
		/// <param name="point_count">Number of points in the arrays</param>
		/// <param name="point_offset">Offset to use when iterating through array. Use "1" for 
		/// normal arrays or use "2" or "3" when using a single array for all of the x, y
		/// and [optional] z elements.</param>
		/// <param name="x">The "X" coordinate array</param>
		/// <param name="y">The "Y" coordinate array</param>
		/// <param name="z">The "Z" coordinate array (may be null)</param>
		/// <returns>Zero on success, pj_errno on failure</returns>
		[DllImport("proj.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
		public static extern int pj_transform(IntPtr src, IntPtr dst, 
			int point_count, int point_offset,
			[InAttribute, OutAttribute] double[] x,
			[InAttribute, OutAttribute] double[] y, 
			[InAttribute, OutAttribute] double[] z);

		/// <summary>
		/// Perform a datum transformation on the inputs.  Typically you would use 
		/// pj_transform which calls this function internally.
		/// </summary>
		/// <param name="src">Source coordinate system definition</param>
		/// <param name="dst">Destination coordinate system definition</param>
		/// <param name="point_count">Count of points in the array(s)</param>
		/// <param name="point_offset">Offset of each element (see pj_transform)</param>
		/// <param name="x">Array of "X" values</param>
		/// <param name="y">Array of "Y" values</param>
		/// <param name="z">Array of "Z" values (may be null)</param>
		/// <returns>Zero on success, pj_errno on failure</returns>
		[DllImport("proj.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
		public static extern int pj_datum_transform(IntPtr src, IntPtr dst, 
			int point_count, int point_offset, 
			[InAttribute, OutAttribute] double[] x,
			[InAttribute, OutAttribute] double[] y,
			[InAttribute, OutAttribute] double[] z);

		/// <summary>
		/// Convert geocentric coordinates to geodetic.
		/// </summary>
		/// <param name="a">Ellipsoid semi-major axis</param>
		/// <param name="es">Square of ellipsoid eccentricity</param>
		/// <param name="point_count">Count of points in array(s)</param>
		/// <param name="point_offset">Offset of each element in array(s) (see pj_transform)</param>
		/// <param name="x">Array of "X" values</param>
		/// <param name="y">Array of "Y" values</param>
		/// <param name="z">Array of "Z" values</param>
		/// <returns>Zero</returns>
		[DllImport("proj.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
		public static extern int pj_geocentric_to_geodetic(double a, double es,
			int point_count, int point_offset, 
			[InAttribute, OutAttribute] double[] x, 
			[InAttribute, OutAttribute] double[] y, 
			[InAttribute, OutAttribute] double[] z);

		/// <summary>
		/// Convert geodetic coordinates to geocentric. Called by pj_datum_transform
		/// if needed.
		/// </summary>
		/// <param name="a">Ellipsoid semi-major axis</param>
		/// <param name="es">Square of ellipsoid eccentricity</param>
		/// <param name="point_count">Count of points in array(s)</param>
		/// <param name="point_offset">Offset of each element in array(s) (see pj_transform)</param>
		/// <param name="x">Array of "X" values</param>
		/// <param name="y">Array of "Y" values</param>
		/// <param name="z">Array of "Z" values</param>
		/// <returns>Zero</returns>
		[DllImport("proj.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
		public static extern int pj_geodetic_to_geocentric(double a, double es,
			int point_count, int point_offset, 
			[InAttribute, OutAttribute] double[] x, 
			[InAttribute, OutAttribute] double[] y, 
			[InAttribute, OutAttribute] double[] z);
		
		/// <summary>
		/// Compare the datum definitions in two coordinate system definitions
		/// for equality.
		/// </summary>
		/// <param name="srcdefn">Source coordinate system</param>
		/// <param name="dstdefn">Destination coordinate system</param>
		/// <returns>One if true, Zero if false</returns>
		[DllImport("proj.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
		public static extern int pj_compare_datums(IntPtr srcdefn, IntPtr dstdefn );

		/// <summary>
		/// Apply a gridshift datum transformation on the inputs
		/// </summary>
		/// <param name="nadgrids">name of the gridshift file</param>
		/// <param name="inverse">flag whether shifting to or from</param>
		/// <param name="point_count">Count of points in the array(s)</param>
		/// <param name="point_offset">Offset of each element (see pj_transform)</param>
		/// <param name="x">Array of "X" values</param>
		/// <param name="y">Array of "Y" values</param>
		/// <param name="z">Array of "Z" values (may be null)</param>
		/// <returns>Zero on success, pj_errno on failure</returns>
		[DllImport("proj.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
		public static extern int pj_apply_gridshift([MarshalAs(UnmanagedType.LPStr)] string nadgrids,
			int inverse, int point_count, int point_offset, 
			[InAttribute, OutAttribute] double[] x, 
			[InAttribute, OutAttribute] double[] y, 
			[InAttribute, OutAttribute] double[] z);

		/// <summary>
		/// Free up any loaded datum grids from memory.
		/// </summary>
		[DllImport("proj.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
		public static extern void pj_deallocate_grids();
		
		/// <summary>
		/// Is the coordinate system definition lat/long ?
		/// </summary>
		/// <param name="projPJ">Coordinate system definition</param>
		/// <returns>One if true, Zero if false</returns>
		[DllImport("proj.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
		public static extern int pj_is_latlong(IntPtr projPJ);
		
		/// <summary>
		/// Is the coordinate system definition geocentric?
		/// </summary>
		/// <param name="projPJ">Coordinate system definition</param>
		/// <returns>One if true, Zero if false</returns>
		[DllImport("proj.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
		public static extern int pj_is_geocent(IntPtr projPJ);
		
		/// <summary>
		/// Print the coordinate system definition to stdout.
		/// </summary>
		/// <param name="projPJ">The coordinate system definition</param>
		[DllImport("proj.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
		public static extern void pj_pr_list(IntPtr projPJ);

		/// <summary>
		/// Frees the memory allocated for a projection definition.
		/// Attempting to use the object after calling this function or attempting
		/// to call this function more than once with the same object will cause
		/// your application to blow up.
		/// </summary>
		/// <param name="projPJ">Opaque pointer to a projection definition (null is okay)</param>
		[DllImport("proj.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
		//public unsafe static extern void pj_free(void* projPJ);
		public static extern void pj_free(IntPtr projPJ);

		/// <summary>
		/// Install a custom function to locate resource files.  Once installed, the
		/// library will use this until uninstalled (set to null), so make sure the
		/// delegate variable is not garbage collected without "uninstalling" the
		/// function first. (NOTE: Not useable due to calling convention)
		/// </summary>
		/// <param name="f">The function delegate</param>
		[DllImport("proj.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
		public static extern void pj_set_finder(
				[MarshalAs(UnmanagedType.FunctionPtr)] FinderFunction f);		

		/// <summary>
		/// Initialize a coordinate system definition like a "C" style main function
		/// loop.
		/// </summary>
		/// <param name="argc">Count of elements in argv</param>
		/// <param name="argv">Array of string parameters</param>
		/// <returns>Opaque pointer to a coordinate system definition, or null on failure.</returns>
		[DllImport("proj.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
		public static extern IntPtr pj_init(int argc, 
				[MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.LPStr,
					 SizeParamIndex=1)] string[] argv);

		/// <summary>
		/// Initialize a projection definition object from a string of arguments
		/// </summary>
		/// <param name="pjstr">The string of projection arguments</param>
		/// <returns>Opaque pointer to a projection definition or null on failure</returns>
		[DllImport("proj.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
		public static extern IntPtr pj_init_plus(
				[MarshalAs(UnmanagedType.LPStr)] string pjstr);

		/// <summary>
		/// Get a string representation of the coordinate system definition
		/// </summary>
		/// <param name="projPJ">The coordinate system definition</param>
		/// <param name="options">Unused</param>
		/// <returns>A string representing the coordinate system definition</returns>
		[DllImport("proj.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
		public static extern IntPtr pj_get_def(IntPtr projPJ, int options);
		
		/// <summary>
		/// Return a coordinate system definition defining the lat/long coordinate        */
		/// system on which a projection is based.  If the coordinate
		/// system passed in is latlong, a clone of the same will be returned.                   
		/// </summary>
		/// <param name="projPJ">The source coordinate system definition</param>
		/// <returns>The lat/long coordinate system definition</returns>
		[DllImport("proj.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
		public static extern IntPtr pj_latlong_from_proj(IntPtr projPJ);
		
		/// <summary>
		/// Allocate a chunk of memory using malloc.
		/// </summary>
		/// <param name="size">The size of the memory chunk to allocate</param>
		/// <returns>A pointer to the allocated memory, or null on failure.</returns>
		[DllImport("proj.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
		public static extern IntPtr pj_malloc(uint size);

		/// <summary>
		/// Deallocate a chunk of memory previously allocated with pj_alloc (malloc).
		/// </summary>
		/// <param name="memory">The pointer to the chunk of memory to free.</param>
		[DllImport("proj.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
		public static extern void pj_dalloc(IntPtr memory);

		/// <summary>
		/// Get the string value corresponding to the error number.
		/// </summary>
		/// <param name="errno">The error number</param>
		/// <returns>The error message</returns>
		[DllImport("proj.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
		public static extern IntPtr pj_strerrno(int errno);

		/// <summary>
		/// Get a pointer to the int holding the last error number
		/// </summary>
		/// <returns>pointer to the error number variable</returns>
		[DllImport("proj.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
		public static extern IntPtr pj_get_errno_ref();

		/// <summary>
		/// Get the PROJ.4 library release string
		/// </summary>
		/// <returns>string containing library release version</returns>
		[DllImport("proj.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
		public static extern IntPtr pj_get_release();

		/// <summary>
		/// Specifies directories in which the projection library should look for
		/// resource files.
		/// </summary>
		/// <param name="count">number of elements in the array</param>
		/// <param name="path">array of strings specifying directories to look for files in.</param>
		[DllImport("proj.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
		public static extern void pj_set_searchpath(int count, string[] path );
	}
}
