##############################################################################
							     README
							for proj_api.cs
##############################################################################

The ProjApi namespace includes one struct and two class used to interface with
the PROJ.4 C library.  It provides a low-level interface that closely follows
the "proj_api.h" list of available functions.  The ProjUV structure is only
used with the low-level "Proj" class when you use pj_fwd and pj_inv.  Most 
people will probably want to use the higher-level "Projection" class.

VERSION INFO:

This library wrapper was tested with version 4.7.0 of the
projection library.  Some function prototypes have changed from earlier
versions (pj_strerrno, pj_get_def, pj_get_release, pj_malloc, pj_dalloc).
The Projection class has added a couple of static methods and now implements
the IDisposable interface.

Previous string handling was incorrect.  By default, strings returned by
functions are deallocated with CoTaskMemFree.  However, Proj.4 uses what
is the equivalent of AllocHGlobal (malloc).  So, strings must be manually
marshalled Marshal.PtrToStringAnsi().  Additionally, the values returned
by pj_strerrno and pj_get_release should not be freed since they are static
data.

The definitions for pj_malloc and pj_dalloc were changed from unsafe pointers
to IntPtr's.  The global pj_errno was removed. Now there is no unsafe code
in the wrapper.

USAGE NOTES:

1.  While defined, do not use the FinderFunction delegate and the pj_set_finder
function.  They do not work.  The .NET runtime passes delegates using Stdcall,
but the proj.dll (on Windows) uses Cdecl.  This behavior may be different on
other OS's.

2.  The various versions of Projection.Transform convert to/from decimal degrees
and radians before/after calling the underlying pj_transform.  Therefore, you
should not use radians, but decimal degrees for lat/long coordinates.  Generally,
I think this is what most people want.  If you really need radians, you can use
the constants Proj.DEG_TO_RAD and Proj.RAD_TO_DEG.

Although the "high-level" Projection.Transform methods work with decimal degrees,
the "low-level" variants are the same as the PROJ.4 library and expect radians.

3.  Take advantage of the IDisposable interface on the Projection class to
prevent memory leaks.

Eric G. Miller
2010-01-29

