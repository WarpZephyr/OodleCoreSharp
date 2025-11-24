# OodleCoreSharp
A simple library to access oodle library functions in C#.  

# User Usage
Users using an app which uses this library must find an oodle library file and provide it to the app's folder.  
The app may also include the library themselves if they are allowed to do so by the owners of oodle of course.  
Users can generally find oodle library files in games that use them next to the game's exe file.  

The following are the library files expected by this library per platform.  
Windows oodle library files include:  
oo2core_5_win64.dll  
oo2core_6_win64.dll  
oo2core_8_win64.dll  
oo2core_9_win64.dll  

OSX oodle library files include:  
liboo2coremac64.2.5.dylib  
liboo2coremac64.2.6.dylib  
liboo2coremac64.2.8.dylib  
liboo2coremac64.2.9.dylib  

Linux oodle library files include:  
liboo2corelinux64.so.5  
liboo2corelinux64.so.6  
liboo2corelinux64.so.8  
liboo2corelinux64.so.9  

Only one oodle library is necessary to get started.  

# Development Usage
The easiest way to make use of any given oodle library is to call Oodle.GetOodleCompressor().  
This returns the newest oodle library imports available.  

It is also possible to choose a specific oodle version, by checking if it is available, then getting an instance of it.  
For example, a developer could call Oodle.CanUseOodle5() to see if it is available.  
A developer could then create a new Oodle25() object.  

What is supported right now is pretty barebones, mainly compression and decompression with options.  
This may be expanded in the future.  

# Supports
| Oodle  |  Platforms                                                                           |
| :----- | :----------------------------------------------------------------------------------- |
| 2.5    | <ul><li>Windows</li>           <li>Linux (Untested)</li><li>OSX (Untested)</li></ul> |
| 2.6    | <ul><li>Windows (Untested)</li><li>Linux (Untested)</li><li>OSX (Untested)</li></ul> |
| 2.8    | <ul><li>Windows</li>           <li>Linux (Untested)</li><li>OSX (Untested)</li></ul> |
| 2.9    | <ul><li>Windows (Untested)</li><li>Linux (Untested)</li><li>OSX (Untested)</li></ul> |

# Building
This project requires the following libraries to be cloned alongside it.  
Place them in the same top-level folder as this project.  
These dependencies may change at any time.  
```
git clone https://github.com/WarpZephyr/Edoke.git  
```

# Credits
Thanks to [SoulsFormatsNEXT](https://github.com/soulsmods/SoulsFormatsNEXT) for some initial ideas on how to pull this off.