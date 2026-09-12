using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace OodleCoreSharp;

/// <summary>
/// The interop library used to access functions difficult to interop otherwise.
/// </summary>
internal static partial class OodleInterop
{
    /// <summary>
    /// The library name used for resolution.
    /// </summary>
    private const string LibName = "oodleinterop";

    /// <summary>
    /// A handle to the library.
    /// </summary>
    private static nint Handle = Load();

    /// <summary>
    /// Registers this library to the resolver.
    /// </summary>
    static unsafe OodleInterop()
    {
        NativeResolver.Register(&Resolve);
    }

    #region Library

    /// <summary>
    /// The resolver for this library.
    /// </summary>
    /// <param name="libraryName">The library name needing resolution.</param>
    /// <returns>The handle or <see cref="nint.Zero"/> if not found.</returns>
    private static nint Resolve(string libraryName)
    {
        if (libraryName != LibName)
            return nint.Zero;

        if (Handle != nint.Zero)
            return Handle;

        Handle = Load();
        return Handle;
    }

    /// <summary>
    /// Loads the handle to the library or returns <see cref="nint.Zero"/> if not found.
    /// </summary>
    /// <returns>The handle or <see cref="nint.Zero"/> if not found.</returns>
    private static nint Load()
    {
        if (NativeResolver.TryLoad(OSPlatform.Windows, "oodleinterop.dll", out nint handle))
            return handle;

        if (NativeResolver.TryLoad(OSPlatform.Linux, "liboodleinterop.so", out handle))
            return handle;

        if (NativeResolver.TryLoad(OSPlatform.OSX, "liboodleinterop.dylib", out handle))
            return handle;

        return nint.Zero;
    }

    /// <summary>
    /// Gets whether or not the library is loaded.
    /// </summary>
    /// <returns>Whether or not the library is loaded</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLoaded()
        => Handle != nint.Zero;

    #endregion

    #region Native

    /// <summary>
    /// Sets the print callback for Oodle25.
    /// </summary>
    /// <param name="pSetPrintf">A function pointer to the native printf setter for oodle.</param>
    /// <param name="pPrint">A callback to pass print calls to.</param>
    [LibraryImport(LibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    internal static unsafe partial void OodleInterop_Oodle25_SetPrint(nint pSetPrintf, delegate* unmanaged[Cdecl]<OodleLZ_Verbosity, byte*, int, byte*, void> pPrint);

    /// <summary>
    /// Sets the print callback for Oodle26.
    /// </summary>
    /// <param name="pSetPrintf">A function pointer to the native printf setter for oodle.</param>
    /// <param name="pPrint">A callback to pass print calls to.</param>
    [LibraryImport(LibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    internal static unsafe partial void OodleInterop_Oodle26_SetPrint(nint pSetPrintf, delegate* unmanaged[Cdecl]<OodleLZ_Verbosity, byte*, int, byte*, void> pPrint);

    /// <summary>
    /// Sets the print callback for Oodle28.
    /// </summary>
    /// <param name="pSetPrintf">A function pointer to the native printf setter for oodle.</param>
    /// <param name="pPrint">A callback to pass print calls to.</param>
    [LibraryImport(LibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    internal static unsafe partial void OodleInterop_Oodle28_SetPrint(nint pSetPrintf, delegate* unmanaged[Cdecl]<OodleLZ_Verbosity, byte*, int, byte*, void> pPrint);

    /// <summary>
    /// Sets the print callback for Oodle29.
    /// </summary>
    /// <param name="pSetPrintf">A function pointer to the native printf setter for oodle.</param>
    /// <param name="pPrint">A callback to pass print calls to.</param>
    [LibraryImport(LibName)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    [DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
    internal static unsafe partial void OodleInterop_Oodle29_SetPrint(nint pSetPrintf, delegate* unmanaged[Cdecl]<OodleLZ_Verbosity, byte*, int, byte*, void> pPrint);

    #endregion
}
