using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace OodleCoreSharp
{
    public partial class Oodle26 : IOodleCompressor
    {
        /// <summary>
        /// Return value of OodleLZ_Decompress on failure.
        /// </summary>
        public const int OODLELZ_FAILED = 0;

        /// <summary>
        /// The number of raw bytes per "seek chunk".
        /// </summary>
        public const int OODLELZ_BLOCK_LEN = 1 << 18;

        #region Managed

        /// <summary>
        /// Compress some data from memory to memory, synchronously, with OodleLZ.
        /// </summary>
        /// <param name="compressor">Which OodleLZ variant to use in compression.</param>
        /// <param name="rawBuf">Raw data to compress.</param>
        /// <param name="compBuf">Buffer to write compressed data to; Should be at least <see cref="GetCompressedBufferSizeNeeded"/> bytes.</param>
        /// <param name="level"><see cref="OodleLZ_CompressionLevel"/> controls how much CPU effort is put into maximizing compression.</param>
        /// <param name="options">(Optional) Options; If <see cref="null"/>, <see cref="CompressOptions_GetDefault"/> is used</param>
        /// <returns>Size of compressed data written, or <see cref="OODLELZ_FAILED"/> for failure.</returns>
        public unsafe long Compress(
            OodleLZ_Compressor compressor,
            ReadOnlySpan<byte> rawBuf,
            Span<byte> compBuf,
            OodleLZ_CompressionLevel level,
            OodleLZ_CompressOptions? options = null)
        {
            fixed (byte* pRawBuf = &rawBuf[0])
            fixed (byte* pCompBuf = &compBuf[0])
            {
                OodleLZ_CompressOptions* pOptions = (OodleLZ_CompressOptions*)&options;
                return OodleLZ_Compress(compressor, pRawBuf, rawBuf.Length, pCompBuf, level, pOptions);
            }
        }

        /// <summary>
        /// Decompress some data from memory to memory, synchronously.
        /// </summary>
        /// <param name="compBuf">The buffer of the compressed data.</param>
        /// <param name="rawBuf">The buffer to write the decompressed data to.</param>
        /// <param name="fuzzSafe">(Optional) Should the decode fail if it contains non-fuzz safe codecs?</param>
        /// <param name="checkCRC">(Optional) If data could be corrupted and you want to know about it, pass <see cref="OodleLZ_CheckCRC.Yes"/>.</param>
        /// <param name="verbosity">(Optional) If not <see cref="OodleLZ_Verbosity.None"/>, logs some info.</param>
        /// <param name="threadPhase">(Optional) for threaded decode; see OodleLZ_About_ThreadPhasedDecode (default <see cref="OodleLZ_Decode_ThreadPhase.Unthreaded"/>).</param>
        /// <returns>The number of decompressed bytes output, <see cref="OODLELZ_FAILED"/> if none can be decompressed.</returns>
        public unsafe long Decompress(
            ReadOnlySpan<byte> compBuf,
            Span<byte> rawBuf,
            OodleLZ_FuzzSafe fuzzSafe = OodleLZ_FuzzSafe.Yes,
            OodleLZ_CheckCRC checkCRC = OodleLZ_CheckCRC.No,
            OodleLZ_Verbosity verbosity = OodleLZ_Verbosity.None,
            OodleLZ_Decode_ThreadPhase threadPhase = OodleLZ_Decode_ThreadPhase.Unthreaded)
        {
            fixed (byte* pCompBuf = &compBuf[0])
            fixed (byte* pRawBuf = &rawBuf[0])
            {
                return OodleLZ_Decompress(
                    pCompBuf,
                    compBuf.Length,
                    pRawBuf,
                    rawBuf.Length,
                    fuzzSafe,
                    checkCRC,
                    verbosity,
                    threadPhase: threadPhase);
            }
        }

        /// <summary>
        /// Provides default compression options.
        /// </summary>
        /// <remarks>Use to fill your own <see cref="OodleLZ_CompressOptions"/>, then change individual fields.</remarks>
        /// <param name="compressor">Which OodleLZ variant to use in compression.</param>
        /// <param name="lzLevel">The compression level.</param>
        /// <returns>Default compression options.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe OodleLZ_CompressOptions CompressOptions_GetDefault(
            OodleLZ_Compressor compressor = OodleLZ_Compressor.Invalid,
            OodleLZ_CompressionLevel lzLevel = OodleLZ_CompressionLevel.Normal)
            => *OodleLZ_CompressOptions_GetDefault(compressor, lzLevel);

        /// <summary>
        /// Get maximum expanded size for compBuf alloc.
        /// </summary>
        /// <param name="compressor">Which OodleLZ variant to use in compression.</param>
        /// <param name="rawSize">Uncompressed size you will compress into this buffer.</param>
        /// <remarks>This is actually larger than the maximum compressed stream, it includes trash padding.</remarks>
        /// <returns>The maximum expanded size for compBuf alloc.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetCompressedBufferSizeNeeded(
            OodleLZ_Compressor compressor,
            long rawSize)
            => OodleLZ_GetCompressedBufferSizeNeeded(rawSize);

        /// <summary>
        /// The decode buffer size required for the specified raw length.
        /// </summary>
        /// <param name="compressor">Which OodleLZ variant to use in compression.</param>
        /// <param name="rawSize">Uncompressed size without padding.</param>
        /// <param name="corruptionPossible">Whether or not it is possible for the decoder to get corrupted data.</param>
        /// <returns>The decode buffer size required for the specified raw length.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long GetDecodeBufferSize(
            OodleLZ_Compressor compressor,
            long rawSize,
            bool corruptionPossible)
            => OodleLZ_GetDecodeBufferSize(rawSize, corruptionPossible);

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
#if WINDOWS
        [LibraryImport("oo2core_6_win64.dll")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvStdcall) })]
#elif OSX
        [LibraryImport("liboo2coremac64.2.6.dylib")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvStdcall) })]
#elif LINUX
        [LibraryImport("liboo2corelinux64.so.6")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvStdcall) })]
#endif
        private static unsafe partial long OodleLZ_Compress(
            OodleLZ_Compressor compressor,
            byte* rawBuf,
            long rawLen,
            byte* compBuf,
            OodleLZ_CompressionLevel level,
            OodleLZ_CompressOptions* pOptions = null,
            nint dictionaryBase = 0,
            nint lrm = 0,
            nint scratchMem = 0,
            long scratchSize = 0);

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
#if WINDOWS
        [LibraryImport("oo2core_6_win64.dll")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvStdcall) })]
#elif OSX
        [LibraryImport("liboo2coremac64.2.6.dylib")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvStdcall) })]
#elif LINUX
        [LibraryImport("liboo2corelinux64.so.6")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvStdcall) })]
#endif
        private static unsafe partial long OodleLZ_Decompress(
            byte* compBuf,
            long compBufSize,
            byte* rawBuf,
            long rawLen,
            OodleLZ_FuzzSafe fuzzSafe = OodleLZ_FuzzSafe.Yes,
            OodleLZ_CheckCRC checkCRC = OodleLZ_CheckCRC.No,
            OodleLZ_Verbosity verbosity = OodleLZ_Verbosity.None,
            nint decBufBase = 0,
            long decBufSize = 0,
            nint fpCallback = 0,
            nint callbackUserData = 0,
            nint decoderMemory = 0,
            long decoderMemorySize = 0,
            OodleLZ_Decode_ThreadPhase threadPhase = OodleLZ_Decode_ThreadPhase.Unthreaded);

        /// <summary>
        /// Provides a pointer to default compression options.
        /// </summary>
        /// <remarks>Use to fill your own <see cref="OodleLZ_CompressOptions"/>, then change individual fields.</remarks>
        /// <param name="compressor">Which OodleLZ variant to use in compression.</param>
        /// <param name="lzLevel">The compression level.</param>
        /// <returns>A pointer to default compression options.</returns>
#if WINDOWS
        [LibraryImport("oo2core_6_win64.dll")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvStdcall) })]
#elif OSX
        [LibraryImport("liboo2coremac64.2.6.dylib")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvStdcall) })]
#elif LINUX
        [LibraryImport("liboo2corelinux64.so.6")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvStdcall) })]
#endif
        private static unsafe partial OodleLZ_CompressOptions* OodleLZ_CompressOptions_GetDefault(
            OodleLZ_Compressor compressor = OodleLZ_Compressor.Invalid,
            OodleLZ_CompressionLevel lzLevel = OodleLZ_CompressionLevel.Normal);

        /// <summary>
        /// Get maximum expanded size for compBuf alloc.
        /// </summary>
        /// <remarks>This is actually larger than the maximum compressed stream, it includes trash padding.</remarks>
        /// <param name="rawSize">Uncompressed size you will compress into this buffer.</param>
        /// <returns>The maximum expanded size for compBuf alloc.</returns>
#if WINDOWS
        [LibraryImport("oo2core_6_win64.dll")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvStdcall) })]
#elif OSX
        [LibraryImport("liboo2coremac64.2.6.dylib")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvStdcall) })]
#elif LINUX
        [LibraryImport("liboo2corelinux64.so.6")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvStdcall) })]
#endif
        private static partial long OodleLZ_GetCompressedBufferSizeNeeded(long rawSize);

        /// <summary>
        /// The decode buffer size required for the specified raw length.
        /// </summary>
        /// <param name="rawSize">Uncompressed size without padding.</param>
        /// <param name="corruptionPossible">Whether or not it is possible for the decoder to get corrupted data.</param>
        /// <returns>The decode buffer size required for the specified raw length.</returns>
#if WINDOWS
        [LibraryImport("oo2core_6_win64.dll")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvStdcall) })]
#elif OSX
        [LibraryImport("liboo2coremac64.2.6.dylib")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvStdcall) })]
#elif LINUX
        [LibraryImport("liboo2corelinux64.so.6")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvStdcall) })]
#endif
        private static partial long OodleLZ_GetDecodeBufferSize(
            long rawSize,
            [MarshalAs(UnmanagedType.Bool)] bool corruptionPossible);

        #endregion
    }
}
