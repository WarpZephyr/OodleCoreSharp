using OodleCoreSharp.Exceptions;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace OodleCoreSharp
{
    public static class Oodle
    {
        /// <summary>
        /// Whether or not oodle5 is available.
        /// </summary>
        private static bool Oodle5Exists = false;

        /// <summary>
        /// Whether or not oodle6 is available.
        /// </summary>
        private static bool Oodle6Exists = false;

        /// <summary>
        /// Whether or not oodle8 is available.
        /// </summary>
        private static bool Oodle8Exists = false;

        /// <summary>
        /// Whether or not oodle9 is available.
        /// </summary>
        private static bool Oodle9Exists = false;

        /// <summary>
        /// Whether or not oodle5 is available.
        /// </summary>
        /// <returns>Whether or not oodle5 is available.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CanUseOodle5()
        {
            if (Oodle5Exists)
                return true;
#if WINDOWS
            return Oodle5Exists = File.Exists($@"{AppDomain.CurrentDomain.BaseDirectory}\oo2core_5_win64.dll");
#elif OSX
            return Oodle5Exists = File.Exists($@"{AppDomain.CurrentDomain.BaseDirectory}/liboo2coremac64.2.5.dylib");
#elif LINUX
            return Oodle5Exists = File.Exists($@"{AppDomain.CurrentDomain.BaseDirectory}/liboo2corelinux64.so.5");
#endif
        }

        /// <summary>
        /// Whether or not oodle6 is available.
        /// </summary>
        /// <returns>Whether or not oodle6 is available.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CanUseOodle6()
        {
            if (Oodle6Exists)
                return true;
#if WINDOWS
            return Oodle6Exists = File.Exists($@"{AppDomain.CurrentDomain.BaseDirectory}\oo2core_6_win64.dll");
#elif OSX
            return Oodle6Exists = File.Exists($@"{AppDomain.CurrentDomain.BaseDirectory}/liboo2coremac64.2.6.dylib");
#elif LINUX
            return Oodle6Exists = File.Exists($@"{AppDomain.CurrentDomain.BaseDirectory}/liboo2corelinux64.so.6");
#endif
        }

        /// <summary>
        /// Whether or not oodle8 is available.
        /// </summary>
        /// <returns>Whether or not oodle8 is available.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CanUseOodle8()
        {
            if (Oodle8Exists)
                return true;
#if WINDOWS
            return Oodle8Exists = File.Exists($@"{AppDomain.CurrentDomain.BaseDirectory}\oo2core_8_win64.dll");
#elif OSX
            return Oodle8Exists = File.Exists($@"{AppDomain.CurrentDomain.BaseDirectory}/liboo2coremac64.2.8.dylib");
#elif LINUX
            return Oodle8Exists = File.Exists($@"{AppDomain.CurrentDomain.BaseDirectory}/liboo2corelinux64.so.8");
#endif
        }

        /// <summary>
        /// Whether or not oodle9 is available.
        /// </summary>
        /// <returns>Whether or not oodle9 is available.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CanUseOodle9()
        {
            if (Oodle9Exists)
                return true;
#if WINDOWS
            return Oodle9Exists = File.Exists($@"{AppDomain.CurrentDomain.BaseDirectory}\oo2core_9_win64.dll");
#elif OSX
            return Oodle9Exists = File.Exists($@"{AppDomain.CurrentDomain.BaseDirectory}/liboo2coremac64.2.9.dylib");
#elif LINUX
            return Oodle9Exists = File.Exists($@"{AppDomain.CurrentDomain.BaseDirectory}/liboo2corelinux64.so.9");
#endif
        }

        /// <summary>
        /// Get an oodle compressor.
        /// </summary>
        /// <returns>An oodle compressor.</returns>
        /// <exception cref="OodleNotFoundException">No oodle compressors were available.</exception>
        public static IOodleCompressor GetOodleCompressor()
        {
            if (CanUseOodle9())
                return new Oodle29();

            if (CanUseOodle8())
                return new Oodle28();

            if (CanUseOodle6())
                return new Oodle26();

            if (CanUseOodle5())
                return new Oodle25();

#if WINDOWS
            throw new OodleNotFoundException($"Could not find a supported version of oo2core.\n" +
                $"Please copy oo2core_5_win64.dll, oo2core_6_win64.dll, oo2core_8_win64.dll, or oo2core_9_win64.dll into the program folder at: \"{AppDomain.CurrentDomain.BaseDirectory}\"\n" +
                $"It is generally in the same folder as the game's exe file.");
#elif OSX
            throw new OodleNotFoundException($"Could not find a supported version of oo2core." +
                $"Please copy liboo2coremac64.2.5.dylib, liboo2coremac64.2.6.dylib, liboo2coremac64.2.8.dylib, or liboo2coremac64.2.9.dylib into the folder directory at: \"{AppDomain.CurrentDomain.BaseDirectory}\"" +
                $"It is generally in the same folder as the game's exe file.");
#elif LINUX
            throw new OodleNotFoundException($"Could not find a supported version of oo2core." +
                $"Please copy liboo2corelinux64.so.5, liboo2corelinux64.so.6, liboo2corelinux64.so.8, or liboo2corelinux64.so.9 into the folder directory at: \"{AppDomain.CurrentDomain.BaseDirectory}\"" +
                $"It is generally in the same folder as the game's exe file.");
#endif
        }
    }
}
