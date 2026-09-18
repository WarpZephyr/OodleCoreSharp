using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace OodleCoreSharp;

/// <summary>
/// A static class for manually resolving native imports.
/// </summary>
internal static class NativeResolver
{
    /// <summary>
    /// The registered callbacks for library resolution.
    /// </summary>
    private static readonly ConcurrentQueue<nint> Registry = new();

    /// <summary>
    /// The path to the app directory.
    /// </summary>
    private static readonly string AppBaseDirectory = AppContext.BaseDirectory;

    /// <summary>
    /// The runtime identifier for native libraries.
    /// </summary>
    private static readonly string NativeRid = GetNativeRid();

    /// <summary>
    /// The simple runtime identifier for native libraries.
    /// </summary>
    private static readonly string? UniversalNativeRid = GetUniversalNativeRid();

    /// <summary>
    /// Initializes the resolver.
    /// </summary>
    static NativeResolver()
    {
        NativeLibrary.SetDllImportResolver(typeof(NativeResolver).Assembly, ResolveLibrary);
    }

    /// <summary>
    /// Determines the native runtime identifier or defaults.
    /// </summary>
    /// <returns>The native runtime identifier or a default.</returns>
    [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase",
        Justification = "This lowercasing is for file pathing and normalization to uppercase is not valid.")]
    private static string GetNativeRid()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return $"win-{RuntimeInformation.ProcessArchitecture}".ToLowerInvariant();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return $"linux-{RuntimeInformation.ProcessArchitecture}".ToLowerInvariant();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return $"osx-{RuntimeInformation.ProcessArchitecture}".ToLowerInvariant();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            return $"freebsd-{RuntimeInformation.ProcessArchitecture}".ToLowerInvariant();

        return RuntimeInformation.RuntimeIdentifier;
    }

    /// <summary>
    /// Determines the simple native runtime identifier or returns <see langword="null"/>.
    /// </summary>
    /// <returns>The simple native runtime identifier or <see langword="null"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string? GetUniversalNativeRid()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return "win";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return "linux";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return "osx";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            return "freebsd";

        return null;
    }

    /// <summary>
    /// A native library resolver callback.
    /// </summary>
    /// <param name="libraryName">The name of the native library.</param>
    /// <param name="assembly">The assembly this resolver was registered for.</param>
    /// <param name="searchPath">The search path.</param>
    /// <returns>A pointer to the native library or <see cref="nint.Zero"/> if not found.</returns>
    private static unsafe nint ResolveLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        foreach (nint pRaw in Registry)
        {
            var pResolve = (delegate* managed<string, nint>)pRaw;
            var handle = pResolve(libraryName);
            if (handle != nint.Zero)
            {
                return handle;
            }
        }

        return nint.Zero;
    }

    /// <summary>
    /// Registers a resolver callback for library resolution.
    /// </summary>
    /// <param name="pResolve">The callback to register.</param>
    public static unsafe void Register(delegate* managed<string, nint> pResolve)
    {
        ArgumentNullException.ThrowIfNull(pResolve, nameof(pResolve));
        Registry.Enqueue((nint)pResolve);
    }

    /// <summary>
    /// Tries to load a specified library if the specified platform is the current one.
    /// </summary>
    /// <param name="platform">The platform this library must load for.</param>
    /// <param name="libraryName">The file name of the library to load.</param>
    /// <param name="handle">The loaded handle or <see cref="nint.Zero"/> if not loaded.</param>
    /// <returns>Whether or not the library loaded.</returns>
    public static bool TryLoad(OSPlatform platform, string libraryName, [NotNullWhen(true)] out nint handle)
    {
        if (RuntimeInformation.IsOSPlatform(platform))
        {
            // Attempt default
            if (NativeLibrary.TryLoad(libraryName, out handle))
            {
                return true;
            }

            // Attempt direct path specific architecture
            string path = Path.Combine(AppBaseDirectory, "runtimes", NativeRid, "native", libraryName);
            if (NativeLibrary.TryLoad(path, out handle))
            {
                return true;
            }

            // Attempt direct path universal architecture
            if (UniversalNativeRid != null)
            {
                path = Path.Combine(AppBaseDirectory, "runtimes", UniversalNativeRid, "native", libraryName);
                if (NativeLibrary.TryLoad(path, out handle))
                {
                    return true;
                }
            }
        }

        handle = nint.Zero;
        return false;
    }

    /// <summary>
    /// Tries to get an export, providing <see cref="nint.Zero"/> if it could not be found or the library is not loaded.
    /// </summary>
    /// <param name="libraryHandle">The handle to the library to load from, may be <see cref="nint.Zero"/>.</param>
    /// <param name="name">The name of the export to load.</param>
    /// <param name="handle">The loaded export handle, or <see cref="nint.Zero"/> if not found.</param>
    /// <returns>Whether or not the export loaded.</returns>
    public static bool TryGetExport(nint libraryHandle, string name, [NotNullWhen(true)] out nint handle)
    {
        if (libraryHandle != nint.Zero)
        {
            if (NativeLibrary.TryGetExport(libraryHandle, name, out handle))
            {
                return true;
            }
        }

        handle = nint.Zero;
        return false;
    }
}
