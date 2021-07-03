using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace VideoCameraSettings
{
    class Program
    {
        static void Main(string[] args)
        {
            string lastMoniker = null;

            var notifyIcon = new NotifyIcon();
            notifyIcon.Icon = Resources.Camera;
            notifyIcon.Text = "Video Camera Settings";
            notifyIcon.Visible = true;

            var contextMenu = new ContextMenuStrip();
            contextMenu.Opening += (sender, e) =>
            {
                PopulateContextMenu();
                e.Cancel = false;
            };
            notifyIcon.ContextMenuStrip = contextMenu;
            notifyIcon.DoubleClick += (sender, e) => ShowDefaultDeviceSettings();

            Application.Run();

            void PopulateContextMenu()
            {
                Debug.WriteLine("Populating context menu");
                contextMenu.Items.Clear();

                var header = new ToolStripLabel { Text = "Video Cameras" };
                header.Font = new System.Drawing.Font(Control.DefaultFont, System.Drawing.FontStyle.Bold);
                contextMenu.Items.Add(header);

                var devices = GetDevices();
                if (devices.Any())
                {
                    foreach (var device in devices)
                    {
                        var deviceItem = new ToolStripMenuItem { Text = device.Name };
                        deviceItem.Click += (sender, e) => ShowDeviceSettings(device);
                        contextMenu.Items.Add(deviceItem);
                    }
                }
                else
                {
                    contextMenu.Items.Add(new ToolStripLabel { Text = "No cameras found" });
                }

                contextMenu.Items.Add(new ToolStripSeparator());

                var exitItem = new ToolStripMenuItem { Text = "Exit" };
                exitItem.Click += (sender, e) => Application.Exit();
                contextMenu.Items.Add(exitItem);
            }

            List<FilterInfo> GetDevices()
            {
                return new FilterInfoCollection(FilterCategory.VideoInputDevice).Cast<FilterInfo>().ToList();
            }

            void ShowDefaultDeviceSettings()
            {
                var devices = GetDevices();
                var defaultDevice = devices.FirstOrDefault(d => d.MonikerString == lastMoniker) ??
                    devices.FirstOrDefault();

                if (defaultDevice != null)
                {
                    ShowDeviceSettings(defaultDevice);
                }
            }

            void ShowDeviceSettings(FilterInfo chosenDevice)
            {
                lastMoniker = chosenDevice.MonikerString;

                try
                {
                    var device = new VideoCaptureDevice(chosenDevice.MonikerString);
                    device.DisplayPropertyPage(IntPtr.Zero);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not show camera properties: {ex.Message}", "Video Camera Settings");
                }
            }
        }
    }
}
