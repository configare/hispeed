using System;
using CodeCell.Bricks.Runtime;

namespace CodeCell.Bricks.UIs
{
	/// <summary>
	/// 异常处理类
	/// </summary>
	public static class ExceptionWnd
	{
		public static void ShowExceptionWnd(Exception Ex,string ActionDes)
		{
			try
			{
                frmException _frm = new frmException();
				_frm.ErrException=Ex;
				_frm.ActionDescription=ActionDes; 
				_frm.ShowDialog(); 
			}
			catch(Exception ex)
			{
                Log.WriterException(ex);
            }
		}

        public static void ShowExceptionWndWithTrackStack(Exception Ex, string ActionDes)
		{
			try
			{
                frmException _frm = new frmException();
				_frm.ErrExceptionTrack=Ex;
				_frm.ActionDescription=ActionDes; 
				_frm.ShowDialog(); 
			}
			catch(Exception ex)
			{
                Log.WriterException(ex);
            }
		}
	}
}
