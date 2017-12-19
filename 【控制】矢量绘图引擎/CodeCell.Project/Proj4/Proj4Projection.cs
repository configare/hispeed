using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CodeCell.AgileMap.Core
{
    /// <summary>
    /// This class is a thicker interface to the PROJ.4 library.  It exposes a small 
    /// set of methods, but generally these are all that is needed.
    /// </summary>
    public class Proj4Projection : IDisposable
    {

        // INTERNAL data
        /// <summary>
        /// The pointer to the projection definition object
        /// </summary>
        internal IntPtr prj = IntPtr.Zero;
        /// <summary>
        /// Cache of the definition string returned by pj_get_def
        /// </summary>
        internal string out_def = null;

        public CoordinateDomain _coordinateDomain = null;

        /// <summary>
        /// The default constructor
        /// </summary>
        public Proj4Projection() { }

        /// <summary>
        /// Constructor with a definition
        /// </summary>
        /// <param name="paramaters">string defining the coordinate system</param>
        public Proj4Projection(string definition)
            : base()
        {
            this.Initialize(definition);
        }

        /// <summary>
        /// Common object initialization function
        /// </summary>
        /// <param name="definition">The projection definition string</param>
        /// <exception cref="System.ArgumentException">Thrown when initialization fails.  
        /// The reason may vary and will be documented in the Message</exception>
        private void Initialize(string definition)
        {
            IntPtr thePrj = Proj.pj_init_plus(definition);
            if (thePrj == IntPtr.Zero)
            {
                string message = GetErrorMessage(GetErrNo());
                throw new System.ArgumentException(message);
            }
            this.prj = thePrj;
            this.out_def = null;
        }

        /// <summary>
        /// Read the current pj_errno value.
        /// </summary>
        /// <returns>The current pj_errno value.</returns>
        public static int GetErrNo()
        {
            int errno = 0;
            IntPtr pErrNo = Proj.pj_get_errno_ref();
            errno = Marshal.ReadInt32(pErrNo);
            return errno;
        }

        /// <summary>
        /// Get the error message corresponding to
        /// the errno
        /// </summary>
        /// <param name="errno">The error number</param>
        /// <returns>The message, or null if errno == 0</returns>
        public static string GetErrorMessage(int errno)
        {
            if (errno == 0) return null;
            IntPtr pMsg = Proj.pj_strerrno(errno);
            return Marshal.PtrToStringAnsi(pMsg);
        }

        /// <summary>
        /// Instance version checks initialization status.
        /// </summary>
        private void CheckInitialized()
        {
            Proj4Projection.CheckInitialized(this);
        }

        /// <summary>
        /// Static version that checks initialization status.
        /// </summary>
        /// <param name="p">The projection object</param>
        private static void CheckInitialized(Proj4Projection p)
        {
            if (p.prj == IntPtr.Zero)
            {
                throw new ApplicationException("Projection not initialized");
            }
        }
        // PROPERTIES

        /// <summary>
        /// A string representing the coordinate system. Setting it [re]initializes the
        /// projection definition.
        /// </summary>
        /// <exception cref="System.ArgumentException">Thrown when initialization fails (set).  
        /// The reason may vary and will be documented in the Message</exception>
        /// <exception cref="System.ApplicationException">Thrown when the projection is
        /// not initialized (get).</exception>
        public string Definition
        {
            set { this.Initialize(value); }
            get
            {
                this.CheckInitialized();
                if (this.out_def == null)
                {
                    IntPtr pDef = Proj.pj_get_def(this.prj, 0);
                    this.out_def = Marshal.PtrToStringAnsi(pDef);
                    Proj.pj_dalloc(pDef);
                }
                return this.out_def;
            }
        }

        /// <summary>
        /// Returns true if the projection definition is Lat/Long.
        /// </summary>
        /// <exception cref="System.ApplicationException">Thrown when the projection is
        /// not initialized (get).</exception>
        public bool IsLatLong
        {
            get
            {
                this.CheckInitialized();
                return (Proj.pj_is_latlong(this.prj) == 0) ? false : true;
            }
        }

        /// <summary>
        /// Returns true if the projection definition is Geocentric (XYZ)
        /// </summary>
        /// <exception cref="System.ApplicationException">Thrown when the projection is
        /// not initialized (get).</exception>
        public bool IsGeoCentric
        {
            get
            {
                this.CheckInitialized();
                return (Proj.pj_is_geocent(this.prj) == 0) ? false : true;
            }
        }

        // METHODS

        /// <summary>
        /// Get a projection object with the underlying
        /// Lat/Long definition
        /// </summary>
        /// <returns>Projection</returns>
        /// <exception cref="System.ApplicationException">Thrown when the projection is
        /// not initialized.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the underlying library
        /// does not return a valid Lat/Long projection object.  This might happen if the
        /// original projection does not have an underlying Lat/Long coordinate system.
        /// </exception>
        public Proj4Projection GetLatLong()
        {
            this.CheckInitialized();
            Proj4Projection new_prj = new Proj4Projection();
            new_prj.prj = Proj.pj_latlong_from_proj(this.prj);
            if (new_prj.prj == IntPtr.Zero)
            {
                string message = GetErrorMessage(GetErrNo());
                throw new System.ArgumentException(message);
            }
            return new_prj;
        }

        /// <summary>
        /// Returns the projection definition string (Same as .Definition property)
        /// </summary>
        /// <returns>Projection definition string</returns>
        /// <exception cref="System.ApplicationException">Thrown when the projection is
        /// not initialized.</exception>
        public override string ToString()
        {
            return this.Definition;
        }

        /// <summary>
        /// Sets search directories for the PROJ.4 library to look for its resource
        /// files (such as datum grid files, state plane files, etc...).  Search
        /// paths are only used if other means fail (default install directory, PROJ_LIB
        /// environment variable, installed callback, current directory).  Therefore,
        /// do not expect the search path to override the other methods of specifying
        /// the location of resource files.
        /// </summary>
        /// <param name="path">An array of strings specifying directories to look for
        /// files in.</param>
        public static void SetSearchPath(string[] path)
        {
            if (path != null && path.Length > 0)
                Proj.pj_set_searchpath(path.Length, path);
        }

        /// <summary>
        /// Transform coordinates from one projection system to another
        /// </summary>
        /// <param name="dst">The destination projection</param>
        /// <param name="x">The "X" coordinate values.</param>
        /// <param name="y">The "Y" coordinate values.</param>
        /// <exception cref="System.ApplicationException">Thrown when the projection is
        /// not initialized or the transformation failed.  The message will indicate the error.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// May be thrown for any of the following reasons:
        /// <list type="bullet">
        /// <item>The "x" array is null</item>
        /// <item>The "y" array is null</item>
        /// <item>The length of the x and y arrays don't match</item>
        /// </list>
        /// </exception>
        public void Transform(Proj4Projection dst, double[] x, double[] y)
        {
            this.Transform(dst, x, y, null);
        }

        /// <summary>
        /// Transform coordinates from one projection system to another
        /// </summary>
        /// <param name="dst">The destination projection</param>
        /// <param name="x">The "X" coordinate values.</param>
        /// <param name="y">The "Y" coordinate values.</param>
        /// <param name="z">The "Z" coordinate values.</param>
        /// <exception cref="System.ApplicationException">Thrown when the projection is
        /// not initialized or the transformation failed.  The message will indicate the error.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// May be thrown for any of the following reasons:
        /// <list type="bullet">
        /// <item>The "x" array is null</item>
        /// <item>The "y" array is null</item>
        /// <item>The length of the x, y and z (if not null) arrays don't match</item>
        /// </list>
        /// </exception>
        public void Transform(Proj4Projection dst, double[] x, double[] y, double[] z)
        {
            Proj4Projection.Transform(this, dst, x, y, z);
        }

        /// <summary>
        /// Transform coordinates from one projection system to another
        /// </summary>
        /// <param name="src">The source projection</param>
        /// <param name="dst">The destination projection</param>
        /// <param name="x">The "X" coordinate values.</param>
        /// <param name="y">The "Y" coordinate values.</param>
        /// <exception cref="System.ApplicationException">Thrown when the projection is
        /// not initialized or the transformation failed.  The message will indicate the error.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// May be thrown for any of the following reasons:
        /// <list type="bullet">
        /// <item>The "x" array is null</item>
        /// <item>The "y" array is null</item>
        /// <item>The length of the x and y arrays don't match</item>
        /// </list>
        /// </exception>
        public static void Transform(Proj4Projection src, Proj4Projection dst,
                                        double[] x, double[] y)
        {
            Proj4Projection.Transform(src, dst, x, y, null);
        }

        /// <summary>
        /// Transform coordinates from one projection system to another
        /// </summary>
        /// <param name="src">The source projection</param>
        /// <param name="dst">The destination projection</param>
        /// <param name="x">The "X" coordinate values.</param>
        /// <param name="y">The "Y" coordinate values.</param>
        /// <param name="z">The "Z" coordinate values.</param>
        /// <exception cref="System.ApplicationException">Thrown when the projection is
        /// not initialized or the transformation failed.  The message will indicate the error.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// May be thrown for any of the following reasons:
        /// <list type="bullet">
        /// <item>The "x" array is null</item>
        /// <item>The "y" array is null</item>
        /// <item>The length of the x, y and z (if not null) arrays don't match</item>
        /// </list>
        /// </exception>
        private static object lockObj = new object();
        public static void Transform(Proj4Projection src, Proj4Projection dst,
                double[] x, double[] y, double[] z)
        {
            lock (lockObj)
            {
                //Proj4Projection.CheckInitialized(src);
                //Proj4Projection.CheckInitialized(dst);
                if (x == null)
                {
                    throw new ArgumentException("Argument is required", "x");
                }
                if (y == null)
                {
                    throw new ArgumentException("Argument is required", "y");
                }
                if (x.Length != y.Length || (z != null && z.Length != x.Length))
                {
                    throw new ArgumentException("Coordinate arrays must have the same length");
                }
                if (src.IsLatLong)
                {
                    CoordinateDomain cd = dst._coordinateDomain;
                    if (cd == null)
                    {
                        for (int i = 0; i < x.Length; i++)
                        {
                            x[i] *= Proj.DEG_TO_RAD;
                            y[i] *= Proj.DEG_TO_RAD;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < x.Length; i++)
                        {
                            cd.CorrectX(ref x[i]);
                            cd.CorrectY(ref y[i]);
                            //
                            x[i] *= Proj.DEG_TO_RAD;
                            y[i] *= Proj.DEG_TO_RAD;
                        }
                    }
                }
                int result = Proj.pj_transform(src.prj, dst.prj, x.Length, 1, x, y, z);
                if (result != 0)
                {
                    string message = "Tranformation Error";
                    int errno = GetErrNo();
                    if (errno != 0)
                        message = Proj4Projection.GetErrorMessage(errno);
                    throw new ApplicationException(message);
                }
                if (dst.IsLatLong)
                {
                    for (int i = 0; i < x.Length; i++)
                    {
                        x[i] *= Proj.RAD_TO_DEG;
                        y[i] *= Proj.RAD_TO_DEG;
                    }
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (this.prj != IntPtr.Zero)
                Proj.pj_free(this.prj);
        }

        #endregion
    }
}
