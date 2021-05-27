// AForge Direct Show Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2017
// aforge.net@gmail.com
//

namespace AForge.Video.DirectShow
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Runtime.InteropServices;

    using AForge.Video.DirectShow.Internals;

    /// <summary>
    /// Video source for local video capture device (for example USB webcam).
    /// </summary>
    /// 
    /// <remarks><para>This video source class captures video data from local video capture device,
    /// like USB web camera (or internal), frame grabber, capture board - anything which
    /// supports <b>DirectShow</b> interface. For devices which has a shutter button or
    /// support external software triggering, the class also allows to do snapshots. Both
    /// video size and snapshot size can be configured.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // enumerate video devices
    /// videoDevices = new FilterInfoCollection( FilterCategory.VideoInputDevice );
    /// // create video source
    /// VideoCaptureDevice videoSource = new VideoCaptureDevice( videoDevices[0].MonikerString );
    /// // set NewFrame event handler
    /// videoSource.NewFrame += new NewFrameEventHandler( video_NewFrame );
    /// // start the video source
    /// videoSource.Start( );
    /// // ...
    /// // signal to stop when you no longer need capturing
    /// videoSource.SignalToStop( );
    /// // ...
    /// 
    /// private void video_NewFrame( object sender, NewFrameEventArgs eventArgs )
    /// {
    ///     // get new frame
    ///     Bitmap bitmap = eventArgs.Frame;
    ///     // process the frame
    /// }
    /// </code>
    /// </remarks>
    /// 
    public class VideoCaptureDevice
    {
        // moniker string of video capture device
        private string deviceMoniker;

        // dummy object to lock for synchronization
        private object sync = new object( );

        /// <summary>
        /// Video source.
        /// </summary>
        /// 
        /// <remarks>Video source is represented by moniker string of video capture device.</remarks>
        /// 
        public virtual string Source
        {
            get { return deviceMoniker; }
            set
            {
                deviceMoniker = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoCaptureDevice"/> class.
        /// </summary>
        /// 
        public VideoCaptureDevice( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoCaptureDevice"/> class.
        /// </summary>
        /// 
        /// <param name="deviceMoniker">Moniker string of video capture device.</param>
        /// 
        public VideoCaptureDevice( string deviceMoniker )
        {
            this.deviceMoniker = deviceMoniker;
        }

        /// <summary>
        /// Display property window for the video capture device providing its configuration
        /// capabilities.
        /// </summary>
        /// 
        /// <param name="parentWindow">Handle of parent window.</param>
        /// 
        /// <remarks><para><note>If you pass parent window's handle to this method, then the
        /// displayed property page will become modal window and none of the controls from the
        /// parent window will be accessible. In order to make it modeless it is required
        /// to pass <see cref="IntPtr.Zero"/> as parent window's handle.
        /// </note></para>
        /// </remarks>
        /// 
        /// <exception cref="NotSupportedException">The video source does not support configuration property page.</exception>
        /// 
        public void DisplayPropertyPage( IntPtr parentWindow )
        {
            // check source
            if ( ( deviceMoniker == null ) || ( deviceMoniker == string.Empty ) )
                throw new ArgumentException( "Video source is not specified." );

            lock ( sync )
            {
                object tempSourceObject = null;

                // create source device's object
                try
                {
                    tempSourceObject = FilterInfo.CreateFilter( deviceMoniker );
                }
                catch
                {
                    throw new ApplicationException( "Failed creating device object for moniker." );
                }

                if ( !( tempSourceObject is ISpecifyPropertyPages ) )
                {
                    throw new NotSupportedException( "The video source does not support configuration property page." );
                }

                DisplayPropertyPage( parentWindow, tempSourceObject );

                Marshal.ReleaseComObject( tempSourceObject );
            }
        }

        // Display property page for the specified object
        private void DisplayPropertyPage( IntPtr parentWindow, object sourceObject )
        {
            try
            {
                // retrieve ISpecifyPropertyPages interface of the device
                ISpecifyPropertyPages pPropPages = (ISpecifyPropertyPages) sourceObject;

                // get property pages from the property bag
                CAUUID caGUID;
                pPropPages.GetPages( out caGUID );

                // get filter info
                FilterInfo filterInfo = new FilterInfo( deviceMoniker );

                // create and display the OlePropertyFrame
                Win32.OleCreatePropertyFrame( parentWindow, 0, 0, filterInfo.Name, 1, ref sourceObject, caGUID.cElems, caGUID.pElems, 0, 0, IntPtr.Zero );

                // release COM objects
                Marshal.FreeCoTaskMem( caGUID.pElems );
            }
            catch
            {
            }
        }
    }
}
