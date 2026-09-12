using System;
using System.Runtime.InteropServices;

namespace OodleCoreSharp
{
    /// <summary>
    /// Options for the compressor.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct OodleLZ_CompressOptions : IEquatable<OodleLZ_CompressOptions>
    {
        /// <summary>
        /// Maximum value of <see cref="MaxLocalDictionarySize"/> in <see cref="OodleLZ_CompressOptions"/>.
        /// </summary>
        public const int OODLELZ_LOCALDICTIONARYSIZE_MAX = 1 << 30;

        /// <summary>
        /// Verbosity of compression.
        /// </summary>
        public OodleLZ_Verbosity Verbosity;

        /// <summary>
        /// Minimum match length; Cannot be used to reduce a compressor's default MML, but can be higher. On some types of data, a large MML (6 or 8) is a space-speed win.
        /// </summary>
        public int MinMatchLen;

        /// <summary>
        /// Whether chunks should be independent, for seeking and parallelism.
        /// </summary>
        [MarshalAs(UnmanagedType.Bool)]
        public bool SeekChunkReset;

        /// <summary>
        /// Length of independent seek chunks (if <see cref="SeekChunkReset"/>); must be a power of 2 and >= OODLELZ_BLOCK_LEN; you can use OodleLZ_MakeSeekChunkLen.
        /// </summary>
        public int SeekChunkLen;

        /// <summary>
        /// Decoder profile to target (set to zero).
        /// </summary>
        public OodleLZ_Profile Profile;

        /// <summary>
        /// Sets a maximum offset for matches, if lower than the maximum the format supports.<br/>
        /// <= 0 means infinite (use whole buffer).<br/>
        /// Often power of 2 but doesn't have to be.
        /// </summary>
        public int DictionarySize;

        /// <summary>
        /// This is a number of bytes; I must gain at least this many bytes of compressed size to accept a speed-decreasing decision.
        /// </summary>
        public int SpaceSpeedTradeoffBytes;

        /// <summary>
        /// Max huffmans per chunk.
        /// </summary>
        public int MaxHuffmansPerChunk;

        /// <summary>
        /// Should the encoder send a CRC of each compressed quantum, for integrity checks; This is necessary if you want to use <see cref="OodleLZ_CheckCRC.Yes"/> on decode.
        /// </summary>
        [MarshalAs(UnmanagedType.Bool)]
        public bool SendQuantumCRCs;

        /// <summary>
        /// (Optimals) size of local dictionary before needing a long range matcher.<br/>
        /// This does not set a window size for the decoder; it's useful to limit memory use and time taken in the encoder.<br/>
        /// <see cref="MaxLocalDictionarySize"/> must be a power of 2.<br/>
        /// Must be <= <see cref="OODLELZ_LOCALDICTIONARYSIZE_MAX"/>.
        /// </summary>
        public int MaxLocalDictionarySize;

        /// <summary>
        /// (Optimals) should the encoder find matches beyond <see cref="MaxLocalDictionarySize"/> using an LRM.
        /// </summary>
        public int MakeLongRangeMatcher;

        /// <summary>
        /// (Non-Optimals) When variable, sets the size of the match finder structure (often a hash table); Use 0 for the compressor's default.
        /// </summary>
        public int MatchTableSizeLog2;

        #region Equality

        /// <summary>
        /// Gets whether or not this instance is equal to another instance.
        /// </summary>
        /// <param name="other">The other instance.</param>
        /// <returns>Whether or not this instance is equal to another instance.</returns>
        public readonly bool Equals(OodleLZ_CompressOptions other)
        {
            return Verbosity == other.Verbosity
                && MinMatchLen == other.MinMatchLen
                && SeekChunkReset == other.SeekChunkReset
                && SeekChunkLen == other.SeekChunkLen
                && Profile == other.Profile
                && DictionarySize == other.DictionarySize
                && SpaceSpeedTradeoffBytes == other.SpaceSpeedTradeoffBytes
                && MaxHuffmansPerChunk == other.MaxHuffmansPerChunk
                && SendQuantumCRCs == other.SendQuantumCRCs
                && MaxLocalDictionarySize == other.MaxLocalDictionarySize
                && MakeLongRangeMatcher == other.MakeLongRangeMatcher
                && MatchTableSizeLog2 == other.MatchTableSizeLog2;
        }

        /// <inheritdoc/>
        public override readonly bool Equals(object? obj)
            => obj is OodleLZ_CompressOptions other && Equals(other);

        /// <inheritdoc/>
        public override readonly int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Verbosity);
            hashCode.Add(MinMatchLen);
            hashCode.Add(SeekChunkLen);
            hashCode.Add(Profile);
            hashCode.Add(DictionarySize);
            hashCode.Add(SpaceSpeedTradeoffBytes);
            hashCode.Add(MaxHuffmansPerChunk);
            hashCode.Add(SendQuantumCRCs);
            hashCode.Add(MaxLocalDictionarySize);
            hashCode.Add(MakeLongRangeMatcher);
            hashCode.Add(MatchTableSizeLog2);
            return hashCode.ToHashCode();
        }

        /// <summary>
        /// Compares two <see cref="OodleLZ_CompressOptions"/> for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the comparsion.</returns>
        public static bool operator ==(OodleLZ_CompressOptions left, OodleLZ_CompressOptions right)
            => left.Equals(right);

        /// <summary>
        /// Compares two <see cref="OodleLZ_CompressOptions"/> for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the comparsion.</returns>
        public static bool operator !=(OodleLZ_CompressOptions left, OodleLZ_CompressOptions right)
            => !(left == right);

        #endregion
    }
}
