# VideoCameraSettings
Program to launch the properties page for a video camera device.

This is simply the code to show video camera properties from the AForge.NET project (https://github.com/andrewkirillov/AForge.NET),
extracted and wrapped up as a very basic command-line tool.

## Building

This is a .NET Core project and should build with:
```
dotnet build
```

## Running

Simply running the program `VideoCameraSettings.exe` with no arguments will show the properties if there is a single video camera
in the system. If there are more than one, it will present a list and ask you to type a number to select the device you want.

You can also pass the name of the device as the first argument.
