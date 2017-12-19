using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace Telerik.WinControls.UI.Multimedia
{
    /// <summary>
    /// This class is using DirectShow to play audio and / or video. It allows adjusting
    /// of the audio volume as well as adjusting of the video size and playing on full screen.
    /// The video is played in the content rectangle of the control.
    /// </summary>
    /// <example>
    /// 	<para>RadPlayer player = new RadPlayer();</para>
    /// 	<para>myForm.Children.Add(player);</para>
    /// 	<para>if (player.OpenClip("MyVideoFile.avi"))</para>
    /// 	<para>player.StartClip();</para>
    /// </example>
    /// <remarks>
    ///     All methods are bool and return false on error. To get the last error as a string
    ///     use method <see cref="GetLastError"/>.
    /// </remarks>
    /// <requirements>Installed DirectShow (after Windows 98 it is intergated in the OS).</requirements>
	[ToolboxItem(false)]
    [DefaultEvent("MediaNotification")]
	public class RadPlayer : RadControl
    {
        #region Private fields
        private const int WMGraphNotify = 0x0400 + 13;
        public const int VolumeFull = 0;
        public const int VolumeSilence = -10000;

        private IGraphBuilder graphBuilder = null;
        private IMediaControl mediaControl = null;
        private IMediaEventEx mediaEventEx = null;
        private IVideoWindow videoWindow = null;
        private IBasicAudio basicAudio = null;
        private IBasicVideo basicVideo = null;
        private IMediaSeeking mediaSeeking = null;
        private IMediaPosition mediaPosition = null;
        private IVideoFrameStep frameStep = null;

        private bool isMute = false;
        private IntPtr hDrain = IntPtr.Zero;

        private bool deleteTempFile = true;
        private string tempFileName = null;

#if DEBUG
        private DsROTEntry rot = null;
#endif

        private int lastError;
        private string errorMessage;
        #endregion

        #region Events
        public event RadPlayerEventHandler MediaNotification;
        public virtual void OnMediaNotification(RadPlayerEventArgs args)
        {
            if (this.MediaNotification != null)
                MediaNotification(this, args);
        }
        #endregion

        #region Properties
        private RadPlayState currentState = RadPlayState.Init;
        [Browsable(false)]
        public RadPlayState CurrentState
        {
            get { return this.currentState; }
        }

        private bool hasAudio = false;
        [Browsable(false)]
        public bool HasAudio
        {
            get { return this.hasAudio; }
        }

        private bool hasVideo = false;
        [Browsable(false)]
        public bool HasVideo
        {
            get { return this.hasVideo; }
        }

        private Size originalVideoSize = Size.Empty;
        [Browsable(false)]
        public Size OriginalVideoSize
        {
            get { return this.originalVideoSize; }
        }

        // Value in units of 100 nanoseconds
        private long duartion = 0;
        [Browsable(false)]
        public long Duration
        {
            get { return this.duartion; }
        }

        // Value in units of 100 nanoseconds
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public long CurrentPosition
        {
            get
            {
                long res = GetCurrentPosition();
                if (res < 0) res = 0;
                return res;
            }

            set
            {
                SetCurrentPosition(value);
            }
        }

        private int volume = VolumeFull;
        [DefaultValue(VolumeFull), Category(RadDesignCategory.BehaviorCategory)]
        public int Volume
        {
            get
            {
                return this.volume;
            }
            set
            {
                int newVolume = Math.Min(Math.Max(value, VolumeSilence), VolumeFull);
                isMute = false;
                SetVolume(newVolume);
                int testVolume;
                if (GetVolume(out testVolume))
                    this.volume = testVolume;
                else
                    this.volume = newVolume;
            }
        }

        [Browsable(false)]
        public bool IsMute
        {
            get { return this.isMute; }
        }

        private bool isFullScreen = false;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsFullScreen
        {
            get { return isFullScreen; }
            set { SetFullScreen(value); }
        }

        private bool repeat = false;
        [DefaultValue(false), Category(RadDesignCategory.BehaviorCategory)]
        public bool Repeat
        {
            get { return repeat; }
            set { repeat = value; }
        }

        private double playbackRate;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public double PlaybackRate
        {
            get { return this.playbackRate; }
            set { SetRate(value); }
        }
        #endregion

        #region Constructors
        public RadPlayer()
        {
        }
        #endregion

        #region Overrides
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case WMGraphNotify:
                    {
                        HandleGraphEvent();
                        break;
                    }
            }

            // Pass this message to the video window for notification of system changes
            if (this.videoWindow != null)
                this.videoWindow.NotifyOwnerMessage(m.HWnd, m.Msg, m.WParam.ToInt32(), m.LParam.ToInt32());

            base.WndProc(ref m);
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            SetVideoWindowRect(this.ClientRectangle);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CloseInterfaces(true);
            }
            base.Dispose(disposing);
        }
        #endregion

        #region APIs
        public string GetLastError()
        {
            if (this.errorMessage != null)
                return this.errorMessage;
            return DsError.GetErrorText(this.lastError);
        }

        public void ResetVideoWindow()
        {
            if (this.currentState != RadPlayState.Init && this.hasVideo && this.videoWindow != null)
            {
                SetVideoWindowRect(this.ClientRectangle);
                this.videoWindow.put_Visible(OABool.False);
                this.videoWindow.put_Visible(OABool.True);
            }
        }

        public bool OpenClip(string fileName)
        {
            if (fileName == string.Empty)
                return false;

            CloseClip();
            ClearLastError();
            deleteTempFile = true; // From now on CloseClip() will delete the file if it is temporary
            
            this.graphBuilder = (IGraphBuilder)new FilterGraph();

            // Have the graph builder construct its the appropriate graph automatically
            if (!Check(this.graphBuilder.RenderFile(fileName, null)))
                return false;

            // QueryInterface for DirectShow interfaces
            this.mediaControl = this.graphBuilder as IMediaControl;
            this.mediaEventEx = this.graphBuilder as IMediaEventEx;
            this.mediaSeeking = this.graphBuilder as IMediaSeeking;
            this.mediaPosition = this.graphBuilder as IMediaPosition;

            // Query for video interfaces, which may not be relevant for audio files
            this.videoWindow = this.graphBuilder as IVideoWindow;
            this.basicVideo = this.graphBuilder as IBasicVideo;

            // Query for audio interfaces, which may not be relevant for video-only files
            this.basicAudio = this.graphBuilder as IBasicAudio;

            // Is this an audio-only file (no video component)?
            CheckVisibility();

            // Have the graph signal event via window callbacks for performance
            if (!Check(this.mediaEventEx.SetNotifyWindow(this.Handle, WMGraphNotify, IntPtr.Zero)))
            {
                CloseClipNoError();
                return false;
            }

            if (this.hasVideo)
            {
                // Setup the video window
                if (!Check(this.videoWindow.put_Owner(this.Handle)))
                {
                    CloseClipNoError();
                    return false;
                }

                if (!Check(this.videoWindow.put_WindowStyle(
                    WindowStyle.Child | WindowStyle.ClipSiblings | WindowStyle.ClipChildren)))
                {
                    CloseClipNoError();
                    return false;
                }

                int videoWidth, videoHeight;
                if (this.basicVideo != null &&
                    this.basicVideo.GetVideoSize(out videoWidth, out videoHeight) != DsResults.E_NoInterface)
                {
                    this.originalVideoSize = new Size(videoWidth, videoHeight);
                }
                else
                {
                    this.originalVideoSize = Size.Empty;
                }

                if (!this.originalVideoSize.IsEmpty)
                {
                    this.ClientSize = this.originalVideoSize;

                    Check(this.videoWindow.SetWindowPosition(0, 0,
                        this.originalVideoSize.Width, this.originalVideoSize.Height));
                }

                GetFrameStepInterface();
            }

#if DEBUG
            rot = new DsROTEntry(this.graphBuilder);
#endif

            // Reset time format
            if (this.mediaSeeking != null)
            {
                Guid currentTimeFormat;
                if (Check(this.mediaSeeking.GetTimeFormat(out currentTimeFormat)))
                {
                    if (currentTimeFormat != TimeFormat.MediaTime)
                        Check(this.mediaSeeking.SetTimeFormat(TimeFormat.MediaTime));
                }
            }
            
            this.duartion = GetDuration();

            this.currentState = RadPlayState.Stopped;
            return true;
        }

        public bool OpenClipFromResource(string resourceName)
        {
            // Ensure the previous temp file is deleted
            this.deleteTempFile = true;
            CloseClip();
            ClearLastError();

            // Get the resource stream
            Stream stream = Assembly.GetCallingAssembly().GetManifestResourceStream(resourceName.Trim());
            if (stream == null)
            {
                stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName.Trim());
            }
            if (stream == null)
            {
                Assembly asm = Assembly.GetEntryAssembly();
                //there is not any entry assembly in design mode!
                if (asm != null)
                {
                    stream = asm.GetManifestResourceStream(resourceName);
                }
            }

            if (stream == null)
            {
                SetLastError("Cannot find the resource");
                return false;
            }

            // Get temproray file name
            try
            {
                this.tempFileName = Path.GetTempFileName();
                if (File.Exists(this.tempFileName))
                    File.Delete(this.tempFileName);
            }
            catch
            {
                stream.Close();
                SetLastError("Cannot get temporary file name");
                this.tempFileName = null;
                return false;
            }

            // Save the resource stream to a temporary file
            try
            {
                using (stream)
                using (FileStream fs = File.OpenWrite(this.tempFileName))
                {

                    byte[] buffer = new byte[4096];

                    long length = stream.Length;
                    fs.SetLength(length);

                    int count = (int)(length / buffer.Length);
                    int left = (int)(length % buffer.Length);
                    for (int i = 0; i < count; i++)
                    {
                        stream.Read(buffer, 0, buffer.Length);
                        fs.Write(buffer, 0, buffer.Length);
                    }
                    if (left > 0)
                    {
                        stream.Read(buffer, 0, left);
                        fs.Write(buffer, 0, left);
                    }
                }
            }
            catch
            {
                try
                {
                    if (File.Exists(this.tempFileName))
                        File.Delete(this.tempFileName);
                }
                catch
                {
                }
                this.tempFileName = null;
                SetLastError("Cannot dump the resource to a temporary file");
                return false;
            }

            // Prevent the first CloseClip() in OpenClip() from deleting the temporary file
            this.deleteTempFile = false;
            return OpenClip(this.tempFileName);
        }

        public bool CloseClip()
        {
            bool res = true;

            // Don't report closing errors if the state is Init - in most cases there is nothing to close
            // so errors while closing will occur.
            // Anyway even the state is Init we will try to close all open interfaces - we could be in
            // some faulty situation...
            bool dontReportErrors = this.currentState == RadPlayState.Init;

            // Stop media playback
            if (this.mediaControl == null)
            {
                SetLastError("No funtionality for control of the playing is presented");
                res = false;
            }
            else
            {
                res = Check(this.mediaControl.Stop());
            }

            // Clear global flags
            this.currentState = RadPlayState.Stopped;

            // Free DirectShow interfaces
            if (!CloseInterfaces(false))
                res = false;

            this.hasAudio = false;
            this.hasVideo = false;

            // No current media state
            this.currentState = RadPlayState.Init;

            if (dontReportErrors)
            {
                ClearLastError();
                res = true;
            }

            return res;
        }

        public bool PlayClip()
        {
            if (this.currentState == RadPlayState.Init)
            {
                SetLastError("There is nothing to play");
                return false;
            }

            if (this.mediaControl == null)
            {
                SetLastError("No funtionality for control of the playing is presented");
                return false;
            }

            // Run the graph to play the media file
            if (!Check(this.mediaControl.Run()))
                return false;

            this.currentState = RadPlayState.Running;
            return true;
        }

        public bool StopClip()
        {
            if (this.currentState == RadPlayState.Init)
            {
                SetLastError("There is nothing to stop");
                return false;
            }

            if (this.mediaControl == null)
            {
                SetLastError("No funtionality for control of the playing is presented");
                return false;
            }

            // Stop and reset postion to beginning
            if ((this.currentState == RadPlayState.Paused) || (this.currentState == RadPlayState.Running))
            {
                Check(this.mediaControl.Stop());

                // Seek to the beginning
                SetCurrentPosition(0);
                
                // Display the first frame to indicate the reset condition
                Check(this.mediaControl.Pause());

                this.currentState = RadPlayState.Stopped;
            }

            return true;
        }

        public bool PauseClip()
        {
            if (this.currentState == RadPlayState.Init)
            {
                SetLastError("There is nothing to pause");
                return false;
            }

            if (this.mediaControl == null)
            {
                SetLastError("No funtionality for control of the playing is presented");
                return false;
            }

            if (!Check(this.mediaControl.Pause()))
                return false;

            this.currentState = RadPlayState.Paused;
            return true;
        }

        public bool ToggleMute()
        {
            isMute = !isMute;
            return SetVolume(isMute ? VolumeSilence : this.volume);
        }

        public bool ToggleFullScreen()
        {
            return SetFullScreen(!this.isFullScreen);
        }

        public bool SetRate(double rate)
        {
            if (this.mediaPosition == null)
            {
                this.playbackRate = 1.0;
                SetLastError("Cannot modify rate");
                return false;
            }

            if (!Check(this.mediaPosition.put_Rate(rate)))
                return false;

            Check(this.mediaPosition.get_Rate(out this.playbackRate));
            return true;
        }
        #endregion

        #region Implementation
        //
        // Some video renderers support stepping media frame by frame with the
        // IVideoFrameStep interface.  See the interface documentation for more
        // details on frame stepping.
        //
        private bool GetFrameStepInterface()
        {
            // Get the frame step interface, if supported
            IVideoFrameStep frameStepTest = this.graphBuilder as IVideoFrameStep;
            if (frameStepTest == null)
            {
                SetLastError("Frame Step interface could be get");
                return false;
            }

            // Check if this decoder can step
            if (Check(frameStepTest.CanStep(0, null)))
            {
                this.frameStep = frameStepTest;
                return true;
            }
            else
            {
                Marshal.ReleaseComObject(frameStepTest);
                frameStepTest = null;
                return false;
            }
        }

        private void CloseClipNoError()
        {
            int error = this.lastError;
            string errorMessage = this.errorMessage;

            CloseClip();

            if (errorMessage != null)
                SetLastError(errorMessage);
            else
                Check(error);
        }

        private bool CloseInterfaces(bool disposing)
        {
            this.lastError = 0;

            try
            {
                lock (this)
                {
                    if (disposing)
                    {
                        // Stop media playback (could be disposing while playing...)
                        if (this.mediaControl != null)
                            Check(this.mediaControl.Stop());
                    }

                    // Relinquish ownership (IMPORTANT!) after hiding video window
                    if (this.hasVideo)
                    {
                        Check(this.videoWindow.put_Visible(OABool.False));
                        Check(this.videoWindow.put_Owner(IntPtr.Zero));
                    }

                    if (this.mediaEventEx != null)
                    {
                        Check(this.mediaEventEx.SetNotifyWindow(IntPtr.Zero, 0, IntPtr.Zero));
                    }

#if DEBUG
                    if (rot != null)
                    {
                        rot.Dispose();
                        rot = null;
                    }
#endif
                    // Release and zero DirectShow interfaces
                    if (this.mediaEventEx != null)
                        this.mediaEventEx = null;
                    if (this.mediaSeeking != null)
                        this.mediaSeeking = null;
                    if (this.mediaPosition != null)
                        this.mediaPosition = null;
                    if (this.mediaControl != null)
                        this.mediaControl = null;
                    if (this.basicAudio != null)
                        this.basicAudio = null;
                    if (this.basicVideo != null)
                        this.basicVideo = null;
                    if (this.videoWindow != null)
                        this.videoWindow = null;
                    if (this.frameStep != null)
                        this.frameStep = null;

                    if (this.graphBuilder != null)
                    {
                        Marshal.ReleaseComObject(this.graphBuilder);
                        this.graphBuilder = null;
                    }

                    GC.Collect();

                    if (deleteTempFile && this.tempFileName != null && File.Exists(this.tempFileName))
                    {
                        File.Delete(this.tempFileName);
                        this.tempFileName = null;
                    }
                }
            }
            catch
            {
                if (this.lastError >= 0)
                    SetLastError("Error closing DirectShow interfaces");
            }

            return this.lastError >= 0 && this.errorMessage == null;
        }

        private long GetDuration()
        {
            long res = 0;
            if (this.mediaSeeking != null)
            {
                Guid currentTimeFormat;
                if (Check(this.mediaSeeking.GetTimeFormat(out currentTimeFormat)))
                {
                    if (currentTimeFormat == TimeFormat.MediaTime)
                    {
                        if (!Check(this.mediaSeeking.GetDuration(out res)))
                            res = 0;
                    }
                }
            }
            else if (this.mediaPosition != null)
            {
                double seconds = 0.0;
                if (Check(this.mediaPosition.get_Duration(out seconds)))
                {
                    res = (long)Math.Round(seconds * 10000000);
                }
            }
            return res;
        }

        // return -1 on error
        private long GetCurrentPosition()
        {
            long res = -1;
            if (this.mediaSeeking != null)
            {
                Guid currentTimeFormat;
                if (Check(this.mediaSeeking.GetTimeFormat(out currentTimeFormat)))
                {
                    if (currentTimeFormat == TimeFormat.MediaTime)
                    {
                        if (!Check(this.mediaSeeking.GetCurrentPosition(out res)))
                            res = -1;
                    }
                }
            }
            else if (this.mediaPosition != null)
            {
                double seconds = 0.0;
                if (Check(this.mediaPosition.get_CurrentPosition(out seconds)))
                {
                    res = (long)Math.Round(seconds * 10000000);
                }
            }
            return res;
        }

        private bool SetCurrentPosition(long position)
        {
            bool res = false;
            if (this.mediaSeeking != null)
            {
                Guid currentTimeFormat;
                if (Check(this.mediaSeeking.GetTimeFormat(out currentTimeFormat)))
                {
                    if (currentTimeFormat == TimeFormat.MediaTime)
                    {
                        res = Check(this.mediaSeeking.SetPositions(
                            new DsLong(position), AMSeekingSeekingFlags.AbsolutePositioning,
                            null, AMSeekingSeekingFlags.NoPositioning));
                    }
                }
            }
            else if (this.mediaPosition != null)
            {
                double seconds = ((double)position) / 10000000;
                res = Check(this.mediaPosition.put_CurrentPosition(seconds));
            }
            return res;
        }

        private bool Restart()
        {
            if (this.mediaControl == null)
            {
                SetLastError("No funtionality for control of the playing is presented");
                return false;
            }

            bool res = SetCurrentPosition(0);
            if (!res)
            {
                // Some custom filters (like the Windows CE MIDI filter)
                // may not implement seeking interfaces (IMediaSeeking)
                // to allow seeking to the start.  In that case, just stop
                // and restart for the same effect.  This should not be
                // necessary in most cases.
                bool stop = Check(this.mediaControl.Stop());
                bool run = Check(this.mediaControl.Run());
                res = stop && run;
            }
            return res;
        }

        private void SetVideoWindowRect(Rectangle videoRect)
        {
            if (this.hasVideo && this.videoWindow != null)
            {
                Check(this.videoWindow.SetWindowPosition(
                  videoRect.Left,
                  videoRect.Top,
                  videoRect.Width,
                  videoRect.Height));
            }
        }

        private bool SetVolume(int newVolume)
        {
            if (!this.hasAudio)
                return true;

            if (this.graphBuilder == null || this.basicAudio == null)
            {
                SetLastError("No funtionality for control of the audio is presented");
                return false;
            }

            // Read current volume
            int testVolume;
            int hr = this.basicAudio.get_Volume(out testVolume);
            if (hr == DsResults.E_NotImplemented) //E_NOTIMPL
            {
                // Fail quietly if this is a video-only media file
                return true;
            }
            else if (!Check(hr))
            {
                return false;
            }

            // Set new volume
            return Check(this.basicAudio.put_Volume(newVolume));
        }

        private bool GetVolume(out int newVolume)
        {
            newVolume = this.volume;

            if (!this.hasAudio)
                return true;

            if (this.graphBuilder == null || this.basicAudio == null)
            {
                SetLastError("No funtionality for control of the audio is presented");
                return false;
            }

            // Read current volume
            int hr = this.basicAudio.get_Volume(out newVolume);
            if (hr == DsResults.E_NotImplemented) //E_NOTIMPL
            {
                // Fail quietly if this is a video-only media file
                return true;
            }
            else if (!Check(hr))
            {
                return false;
            }

            return true;
        }

        private bool SetFullScreen(bool fullScreen)
        {
            // Don't bother with full-screen for audio-only files
            if ((!this.hasVideo) || (this.videoWindow == null))
            {
                SetLastError("There is no video to be switched to full screen");
                return false;
            }

            OABool lMode;
            if (fullScreen)
            {
                this.Focus();

                // Save current message drain
                // Set message drain to application main window
                if (!Check(this.videoWindow.get_MessageDrain(out hDrain)) ||
                    !Check(this.videoWindow.put_MessageDrain(this.Handle)))
                {
                    return false;
                }

                // Switch to full-screen mode
                lMode = OABool.True;
                if (!Check(this.videoWindow.put_FullScreenMode(lMode)))
                    return false;
            }
            else
            {
                // Switch back to windowed mode
                lMode = OABool.False;
                if (!Check(this.videoWindow.put_FullScreenMode(lMode)))
                    return false;

                // Undo change of message drain
                if (!Check(this.videoWindow.put_MessageDrain(hDrain)))
                    return false;

                // Reset video window
                if (!Check(this.videoWindow.SetWindowForeground(OABool.True)))
                    return false;

                // Reclaim keyboard focus for player application
                //this.Focus();
            }

            // Read current state
            if (Check(this.videoWindow.get_FullScreenMode(out lMode)))
                this.isFullScreen = lMode != OABool.False;

            return fullScreen == this.isFullScreen;
        }

        private bool CheckVisibility()
        {
            bool res = true;

            int testVolume;
            this.hasAudio = (this.basicAudio != null) && (this.basicAudio.get_Volume(out testVolume) >= 0);

            if ((this.videoWindow == null) || (this.basicVideo == null))
            {
                // No video interfaces or a file whose video component uses an unknown video codec.
                this.hasVideo = false;
            }
            else
            {
                this.hasVideo = true;

                OABool lVisible;
                int hr = this.videoWindow.get_Visible(out lVisible);
                if (hr < 0)
                {
                    this.hasVideo = false;
                    // If this is an audio-only clip, get_Visible() won't work.
                    //
                    // Also, if this video is encoded with an unsupported codec,
                    // we won't see any video, although the audio will work if it is
                    // of a supported format.
                    if (hr != unchecked((int)0x80004002)) //E_NOINTERFACE
                        res = Check(hr);
                }
            }

            return res;
        }

        private void HandleGraphEvent()
        {
            int hr = 0;
            EventCode evCode;
            int evParam1, evParam2;

            // Make sure that we don't access the media event interface
            // after it has already been released.
            if (this.mediaEventEx == null)
                return;

            // Process all queued events
            while (this.mediaEventEx.GetEvent(out evCode, out evParam1, out evParam2, 0) == 0)
            {
                // Free memory associated with callback, since we're not using it
                hr = this.mediaEventEx.FreeEventParams(evCode, evParam1, evParam2);

                switch (evCode)
                {
                    case EventCode.FullScreenLost:
                        this.isFullScreen = false;
                        break;
                    case EventCode.Complete:
                        if (repeat)
                            Restart();
                        else
                            StopClip();
                        break;
                }

                OnMediaNotification(new RadPlayerEventArgs(evCode));
            }
        }

        // return false on error code (E_*); status code is not treated as error (S_*)
        private bool Check(int hr)
        {
            bool isOK = hr >= 0;
            if (!isOK)
            {
                this.errorMessage = null;
                this.lastError = hr;
            }
            return isOK;
        }

        private void SetLastError(string errorMessage)
        {
            this.errorMessage = errorMessage;
        }

        private void ClearLastError()
        {
            this.lastError = 0;
            this.errorMessage = null;
        }
        #endregion

        protected override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(parent);
            this.RootElement.Children.Add(new RadElement());
        }
    }
}
