using OodleCoreSharp.Exceptions;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace OodleCoreSharp
{
    /// <summary>
    /// A central point of access for oodle related functions.
    /// </summary>
    public static class Oodle
    {
        /// <summary>
        /// Tries to load an oodle library.
        /// </summary>
        /// <param name="handle">The handle if loaded, or <see cref="nint.Zero"/>.</param>
        /// <returns>Whether or not the library loaded.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool TryLoadCore(int version, [NotNullWhen(true)] out nint handle)
        {
            if (NativeResolver.TryLoad(OSPlatform.Windows, $"oo2core_{version}_win64.dll", out handle))
                return true;

            if (NativeResolver.TryLoad(OSPlatform.Linux, $"liboo2corelinux64.so.{version}", out handle))
                return true;

            if (NativeResolver.TryLoad(OSPlatform.OSX, $"liboo2coremac64.2.{version}.dylib", out handle))
                return true;

            // Unsupported platform
            handle = default;
            return false;
        }

        /// <summary>
        /// Tries to load the latest available Oodle library.
        /// </summary>
        /// <param name="oodle">An instance for accessing oodle functions, or null.</param>
        /// <returns>Whether or not the library loaded.</returns>
        public static bool TryLoad([NotNullWhen(true)] out IOodle? oodle)
        {
            if (Oodle29.TryLoad(out Oodle29? oodle9))
            {
                oodle = oodle9;
                return true;
            }

            if (Oodle28.TryLoad(out Oodle28? oodle8))
            {
                oodle = oodle8;
                return true;
            }

            if (Oodle26.TryLoad(out Oodle26? oodle6))
            {
                oodle = oodle6;
                return true;
            }

            if (Oodle25.TryLoad(out Oodle25? oodle5))
            {
                oodle = oodle5;
                return true;
            }

            oodle = default;
            return false;
        }

        /// <summary>
        /// Load an oodle instance, or throw if one is not found.
        /// </summary>
        /// <returns>An oodle instance.</returns>
        /// <exception cref="OodleNotFoundException">No oodle libraries were available.</exception>
        public static IOodle Load()
        {
            if (TryLoad(out IOodle? oodle))
            {
                return oodle;
            }

            throw new OodleNotFoundException("Could not find an oodle library.");
        }
    }
}
