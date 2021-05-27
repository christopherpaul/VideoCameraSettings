using AForge.Video.DirectShow;
using System;

namespace VideoCameraSettings
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                var devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                foreach (FilterInfo d in devices)
                {
                    Console.WriteLine(d.MonikerString);
                }
                return;
            }

            var device = new VideoCaptureDevice(args[0]);
            device.DisplayPropertyPage(IntPtr.Zero);
        }
    }
}
