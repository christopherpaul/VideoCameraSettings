using AForge.Video.DirectShow;
using System;
using System.Diagnostics;
using System.Linq;

namespace VideoCameraSettings
{
    class Program
    {
        static void Main(string[] args)
        {
            var devices = new FilterInfoCollection(FilterCategory.VideoInputDevice).Cast<FilterInfo>().ToList();

            FilterInfo chosenDevice = null;
            if (args.Length > 0)
            {
                chosenDevice = devices.FirstOrDefault(d => d.MonikerString.Equals(args[0], StringComparison.OrdinalIgnoreCase))
                    ?? devices.FirstOrDefault(d => d.Name.Equals(args[0], StringComparison.OrdinalIgnoreCase));

                if (chosenDevice == null)
                {
                    Console.Error.WriteLine($"Could not find video camera device called '{args[0]}'");
                }
            }

            if (chosenDevice == null)
            {
                if (devices.Count == 0)
                {
                    Console.Error.WriteLine("No video camera devices found.");
                    return;
                }

                if (devices.Count == 1)
                {
                    chosenDevice = devices[0];
                }
                else
                {
                    Console.WriteLine($"Found {devices.Count} video camera devices:");
                    for (int i = 0; i < devices.Count; i++)
                    {
                        var d = devices[i];
                        Console.WriteLine($"[{i + 1}] {d.Name}");
                    }

                    Console.WriteLine("Enter device number:");
                    do
                    {
                        Console.Write("> ");
                        string input = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(input))
                        {
                            return;
                        }

                        if (int.TryParse(input, out int chosenNumber) && chosenNumber >= 1 && chosenNumber <= devices.Count)
                        {
                            chosenDevice = devices[chosenNumber - 1];
                        }
                        else
                        {
                            Console.WriteLine($"Please enter a number from 1 to {devices.Count}, or press enter to quit");
                        }
                    }
                    while (chosenDevice == null);
                }
            }

            try
            {
                var device = new VideoCaptureDevice(chosenDevice.MonikerString);
                device.DisplayPropertyPage(IntPtr.Zero);

                bool hasUniqueName = devices.Count(d => d.Name.Equals(chosenDevice.Name, StringComparison.OrdinalIgnoreCase)) == 1;

                Console.WriteLine("To show these settings again, you can use the command:");
                Console.WriteLine($@"{Process.GetCurrentProcess().ProcessName} ""{(hasUniqueName ? chosenDevice.Name : chosenDevice.MonikerString)}""");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Could not show camera properties: {ex.Message}");
            }
        }
    }
}
