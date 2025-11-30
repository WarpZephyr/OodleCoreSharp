using System;

namespace OodleCoreSharp
{
    /// <summary>
    /// An interface for using different versions of oodle with ease.
    /// </summary>
    public interface IOodleCompressor
    {
        /// <summary>
        /// Compress some data from memory to memory, synchronously, with OodleLZ.
        /// </summary>
        /// <param name="compressor">Which OodleLZ variant to use in compression.</param>
        /// <param name="rawBuf">Raw data to compress.</param>
        /// <param name="compBuf">Buffer to write compressed data to; Should be at least <see cref="GetCompressedBufferSizeNeeded"/> bytes.</param>
        /// <param name="level"><see cref="OodleLZ_CompressionLevel"/> controls how much CPU effort is put into maximizing compression.</param>
        /// <param name="options">(Optional) Options; If <see cref="null"/>, <see cref="CompressOptions_GetDefault"/> is used</param>
        /// <returns>Size of compressed data written, or <see cref="OODLELZ_FAILED"/> for failure.</returns>
        public long Compress(
            OodleLZ_Compressor compressor,
            ReadOnlySpan<byte> rawBuf,
            Span<byte> compBuf,
            OodleLZ_CompressionLevel level,
            OodleLZ_CompressOptions? options = null);

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
        public long Decompress(
            ReadOnlySpan<byte> compBuf,
            Span<byte> rawBuf,
            OodleLZ_FuzzSafe fuzzSafe = OodleLZ_FuzzSafe.Yes,
            OodleLZ_CheckCRC checkCRC = OodleLZ_CheckCRC.No,
            OodleLZ_Verbosity verbosity = OodleLZ_Verbosity.None,
            OodleLZ_Decode_ThreadPhase threadPhase = OodleLZ_Decode_ThreadPhase.Unthreaded);

        /// <summary>
        /// Provides default compression options.
        /// </summary>
        /// <param name="compressor">Which OodleLZ variant to use in compression.</param>
        /// <param name="lzLevel">The compression level.</param>
        /// <returns>Default compression options.</returns>
        /// <remarks>Use to fill your own <see cref="OodleLZ_CompressOptions"/>, then change individual fields.</remarks>
        public OodleLZ_CompressOptions CompressOptions_GetDefault(
            OodleLZ_Compressor compressor = OodleLZ_Compressor.Invalid,
            OodleLZ_CompressionLevel lzLevel = OodleLZ_CompressionLevel.Normal);

        /// <summary>
        /// Get maximum expanded size for compBuf alloc.
        /// </summary>
        /// <param name="compressor">Which OodleLZ variant to use in compression.</param>
        /// <param name="rawSize">Uncompressed size you will compress into this buffer.</param>
        /// <remarks>This is actually larger than the maximum compressed stream, it includes trash padding.</remarks>
        /// <returns>The maximum expanded size for compBuf alloc.</returns>
        public long GetCompressedBufferSizeNeeded(
            OodleLZ_Compressor compressor,
            long rawSize);

        /// <summary>
        /// The decode buffer size required for the specified raw length.
        /// </summary>
        /// <param name="compressor">Which OodleLZ variant to use in compression.</param>
        /// <param name="rawSize">Uncompressed size without padding.</param>
        /// <param name="corruptionPossible">Whether or not it is possible for the decoder to get corrupted data.</param>
        /// <returns>The decode buffer size required for the specified raw length.</returns>
        public long GetDecodeBufferSize(
            OodleLZ_Compressor compressor,
            long rawSize,
            bool corruptionPossible);
    }
}
