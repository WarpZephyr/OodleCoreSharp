using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace OodleCoreSharp
{
    /// <summary>
    /// Oodle major version 6.
    /// </summary>
    public partial class Oodle26 : IOodle
    {
        /// <summary>
        /// The library name used for resolution.
        /// </summary>
        private const string LibName = "oo2core_6";

        /// <summary>
        /// A handle to the Oodle6 library.
        /// </summary>
        private static nint Handle = Load();

        /// <summary>
        /// A singleton for <see cref="Oodle26"/>.
        /// </summary>
        private static readonly Oodle26 Instance = new();

        /// <summary>
        /// The current managed print callback.
        /// </summary>
        private static OodlePrintCallback? PrintCallback;

        /// <summary>
        /// Return value of OodleLZ_Decompress on failure.
        /// </summary>
        public const int OODLELZ_FAILED = 0;

        /// <summary>
        /// The number of raw bytes per "seek chunk".
        /// </summary>
        public const int OODLELZ_BLOCK_LEN = 1 << 18;

        /// <summary>
        /// Registers this library to the resolver.
        /// </summary>
        static unsafe Oodle26()
        {
            NativeResolver.Register(&Resolve);
            if (OodleInterop.IsLoaded())
            {
                if (NativeResolver.TryGetExport(Handle, "OodlePlugins_SetPrintf", out nint pSetPrintf))
                {
                    OodleInterop.OodleInterop_Oodle26_SetPrint(pSetPrintf, &UnmanagedPrint);
                }
            }
        }

        /// <summary>
        /// Hide the constructor from the public.
        /// </summary>
        private Oodle26() { }

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

            TryLoad(out _);
            return Handle;
        }

        /// <summary>
        /// Loads the handle to the library or returns <see cref="nint.Zero"/> if not found.
        /// </summary>
        /// <returns>The handle or <see cref="nint.Zero"/> if not found.</returns>
        private static nint Load()
        {
            TryLoad(out _);
            return Handle;
        }

        /// <summary>
        /// Tries to load the Oodle6 library.
        /// </summary>
        /// <param name="oodle">An instance for accessing Oodle6 functions, or null.</param>
        /// <returns>Whether or not the library loaded.</returns>
        public static bool TryLoad([NotNullWhen(true)] out Oodle26? oodle)
        {
            if (Handle != nint.Zero)
            {
                oodle = Instance;
                return true;
            }

            if (Oodle.TryLoadCore(6, out nint handle))
            {
                Handle = handle;
                oodle = Instance;
                return true;
            }

            oodle = null;
            return false;
        }

        /// <summary>
        /// Gets whether or not the library is loaded.
        /// </summary>
        /// <returns>Whether or not the library is loaded</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLoaded()
            => Handle != nint.Zero;

        #endregion

        #region Interop

        /// <summary>
        /// Receives print messages from the native side.
        /// </summary>
        /// <param name="verbosity">The verbosity of the message.</param>
        /// <param name="file">The file path associated with the message.</param>
        /// <param name="line">The file line associated with the message.</param>
        /// <param name="message">The message.</param>
        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        private static unsafe void UnmanagedPrint(OodleLZ_Verbosity verbosity, byte* file, int line, byte* message)
        {
            if (PrintCallback == null)
                return;

            var fileStr = Marshal.PtrToStringAnsi((nint)file);
            var msgStr = Marshal.PtrToStringAnsi((nint)message);
            PrintCallback.Invoke(verbosity, fileStr, line, msgStr);
        }

        #endregion

        #region Managed

        /// <inheritdoc/>
        public unsafe long Compress(
            OodleLZ_Compressor compressor,
            ReadOnlySpan<byte> rawBuf,
            Span<byte> compBuf,
            OodleLZ_CompressionLevel level,
            OodleLZ_CompressOptions? options = null)
        {
            fixed (byte* pRawBuf = rawBuf)
            fixed (byte* pCompBuf = compBuf)
            {
                if (!options.HasValue)
                {
                    return OodleLZ_Compress(compressor, pRawBuf, rawBuf.Length, pCompBuf, level, null);
                }

                OodleLZ_CompressOptions unwrap = options.Value;
                OodleLZ_CompressOptions* pOptions = &unwrap;
                return OodleLZ_Compress(compressor, pRawBuf, rawBuf.Length, pCompBuf, level, pOptions);
            }
        }

        /// <inheritdoc/>
        public unsafe long Decompress(
            ReadOnlySpan<byte> compBuf,
            Span<byte> rawBuf,
            long rawLen,
            OodleLZ_FuzzSafe fuzzSafe = OodleLZ_FuzzSafe.Yes,
            OodleLZ_CheckCRC checkCRC = OodleLZ_CheckCRC.No,
            OodleLZ_Verbosity verbosity = OodleLZ_Verbosity.None,
            OodleLZ_Decode_ThreadPhase threadPhase = OodleLZ_Decode_ThreadPhase.Unthreaded)
        {
            fixed (byte* pCompBuf = compBuf)
            fixed (byte* pRawBuf = rawBuf)
            {
                return OodleLZ_Decompress(
                    pCompBuf,
                    compBuf.Length,
                    pRawBuf,
                    (nint)rawLen,
                    fuzzSafe,
                    checkCRC,
                    verbosity,
                    threadPhase: threadPhase);
            }
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe OodleLZ_CompressOptions CompressOptions_GetDefault(
            OodleLZ_Compressor compressor = OodleLZ_Compressor.Invalid,
            OodleLZ_CompressionLevel lzLevel = OodleLZ_CompressionLevel.Normal)
            => *OodleLZ_CompressOptions_GetDefault(compressor, lzLevel);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetCompressedBufferSizeNeeded(
            OodleLZ_Compressor compressor,
            long rawSize)
            => OodleLZ_GetCompressedBufferSizeNeeded((nint)rawSize);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetDecodeBufferSize(
            OodleLZ_Compressor compressor,
            long rawSize,
            bool corruptionPossible)
            => OodleLZ_GetDecodeBufferSize((nint)rawSize, corruptionPossible);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPrint(OodlePrintCallback callback)
            => PrintCallback = callback;

        #endregion

        #region Native

        /// <summary>
        /// Compress some data from memory to memory, synchronously, with OodleLZ.
        /// </summary>
        /// <param name="compressor">Which OodleLZ variant to use in compression.</param>
        /// <param name="rawBuf">Raw data to compress.</param>
        /// <param name="rawLen">Number of bytes in rawBuf to compress.</param>
        /// <param name="compBuf">Pointer to write compressed data to; Should be at least <see cref="OodleLZ_GetCompressedBufferSizeNeeded"/> bytes.</param>
        /// <param name="level"><see cref="OodleLZ_CompressionLevel"/> controls how much CPU effort is put into maximizing compression.</param>
        /// <param name="pOptions">(Optional) Options; If <see cref="null"/>, <see cref="OodleLZ_CompressOptions_GetDefault"/> is used</param>
        /// <param name="dictionaryBase">(Optional) If not <see cref="null"/>, provides preceding data to prime the dictionary;<br/>
        /// must be contiguous with <paramref name="rawBuf"/>, the data between the pointers <paramref name="dictionaryBase"/> and <paramref name="rawBuf"/> is used as the preconditioning data.<br/>
        /// The exact same precondition must be passed to encoder and decoder.</param>
        /// <param name="lrm">(Optional) Long range matcher.</param>
        /// <param name="scratchMem">(Optional) Pointer to scratch memory.</param>
        /// <param name="scratchSize">(optional) size of scratch memory (see <see cref="OodleLZ_GetCompressScratchMemBound"/>)</param>
        /// <returns>Size of compressed data written, or <see cref="OODLELZ_FAILED"/> for failure.</returns>
        [LibraryImport(LibName)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        [DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
        private static unsafe partial nint OodleLZ_Compress(
            OodleLZ_Compressor compressor,
            byte* rawBuf,
            nint rawLen,
            byte* compBuf,
            OodleLZ_CompressionLevel level,
            OodleLZ_CompressOptions* pOptions = null,
            nint dictionaryBase = 0,
            nint lrm = 0,
            nint scratchMem = 0,
            nint scratchSize = 0);

        /// <summary>
        /// Decompress some data from memory to memory, synchronously.
        /// </summary>
        /// <param name="compBuf">Pointer to compressed data.</param>
        /// <param name="compBufSize">Number of compressed bytes available (must be greater or equal to the number consumed).</param>
        /// <param name="rawBuf">Pointer to output uncompressed data into.</param>
        /// <param name="rawLen">Number of uncompressed bytes to output.</param>
        /// <param name="fuzzSafe">(Optional) Should the decode fail if it contains non-fuzz safe codecs?</param>
        /// <param name="checkCRC">(Optional) If data could be corrupted and you want to know about it, pass <see cref="OodleLZ_CheckCRC.Yes"/>.</param>
        /// <param name="verbosity">(Optional) If not <see cref="OodleLZ_Verbosity.None"/>, logs some info.</param>
        /// <param name="decBufBase">(optional) if not <see cref="null"/>, provides preceding data to prime the dictionary;<br/>
        /// must be contiguous with <paramref name="rawBuf"/>, the data between the pointers dictionaryBase and <paramref name="rawBuf"/> is used as the preconditioning data.<br/>
        /// The exact same precondition must be passed to encoder and decoder.<br/>
        /// The <paramref name="decBufBase"/> must be a reset point.</param>
        /// <param name="decBufSize">(Optional) Size of decode buffer starting at <paramref name="decBufBase"/>, if 0, <paramref name="rawLen"/> is assumed.</param>
        /// <param name="fpCallback">(Optional) OodleDecompressCallback to call incrementally as decode proceeds.</param>
        /// <param name="callbackUserData">(Optional) Passed as userData to <paramref name="fpCallback"/>.</param>
        /// <param name="decoderMemory">(Optional) Pre-allocated memory for the Decoder, of size <paramref name="decoderMemorySize"/>.</param>
        /// <param name="decoderMemorySize">(Optional) size of the buffer at <paramref name="decoderMemory"/>;<br/>
        /// must be at least <see cref="OodleLZDecoder_MemorySizeNeeded"/> bytes to be used</param>
        /// <param name="threadPhase">(Optional) for threaded decode; see OodleLZ_About_ThreadPhasedDecode (default <see cref="OodleLZ_Decode_ThreadPhase.Unthreaded"/>).</param>
        /// <returns>The number of decompressed bytes output, <see cref="OODLELZ_FAILED"/> if none can be decompressed.</returns>
        [LibraryImport(LibName)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        [DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
        private static unsafe partial nint OodleLZ_Decompress(
            byte* compBuf,
            nint compBufSize,
            byte* rawBuf,
            nint rawLen,
            OodleLZ_FuzzSafe fuzzSafe = OodleLZ_FuzzSafe.Yes,
            OodleLZ_CheckCRC checkCRC = OodleLZ_CheckCRC.No,
            OodleLZ_Verbosity verbosity = OodleLZ_Verbosity.None,
            nint decBufBase = 0,
            nint decBufSize = 0,
            nint fpCallback = 0,
            nint callbackUserData = 0,
            nint decoderMemory = 0,
            nint decoderMemorySize = 0,
            OodleLZ_Decode_ThreadPhase threadPhase = OodleLZ_Decode_ThreadPhase.Unthreaded);

        /// <summary>
        /// Provides a pointer to default compression options.
        /// </summary>
        /// <remarks>Use to fill your own <see cref="OodleLZ_CompressOptions"/>, then change individual fields.</remarks>
        /// <param name="compressor">Which OodleLZ variant to use in compression.</param>
        /// <param name="lzLevel">The compression level.</param>
        /// <returns>A pointer to default compression options.</returns>
        [LibraryImport(LibName)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        [DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
        private static unsafe partial OodleLZ_CompressOptions* OodleLZ_CompressOptions_GetDefault(
            OodleLZ_Compressor compressor = OodleLZ_Compressor.Invalid,
            OodleLZ_CompressionLevel lzLevel = OodleLZ_CompressionLevel.Normal);

        /// <summary>
        /// Get maximum expanded size for compBuf alloc.
        /// </summary>
        /// <remarks>This is actually larger than the maximum compressed stream, it includes trash padding.</remarks>
        /// <param name="rawSize">Uncompressed size you will compress into this buffer.</param>
        /// <returns>The maximum expanded size for compBuf alloc.</returns>
        [LibraryImport(LibName)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        [DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
        private static partial nint OodleLZ_GetCompressedBufferSizeNeeded(nint rawSize);

        /// <summary>
        /// The decode buffer size required for the specified raw length.
        /// </summary>
        /// <param name="rawSize">Uncompressed size without padding.</param>
        /// <param name="corruptionPossible">Whether or not it is possible for the decoder to get corrupted data.</param>
        /// <returns>The decode buffer size required for the specified raw length.</returns>
        [LibraryImport(LibName)]
        [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
        [DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]
        private static partial nint OodleLZ_GetDecodeBufferSize(
            nint rawSize,
            [MarshalAs(UnmanagedType.Bool)] bool corruptionPossible);

        #endregion
    }
}
