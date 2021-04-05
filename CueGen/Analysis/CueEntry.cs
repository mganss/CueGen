using BinarySerialization;
using System.Diagnostics.CodeAnalysis;

namespace CueGen.Analysis
{
    /// <summary>
    /// Indicates whether this is a memory cue or a loop.
    /// </summary>
    public enum CueEntryType: byte
    {
        /// <summary>
        /// Memory cue
        /// </summary>
        Memory = 1,
        /// <summary>
        /// Loop
        /// </summary>
        Loop = 2
    }

    /// <summary>
    /// A cue list entry. Can either represent a memory cue or a loop.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CueEntry
    {
        /// <summary>
        /// Gets or sets a value that identifies this as a cue list entry (cue point). Always "PCPT".
        /// </summary>
        /// <value>
        /// The magic value.
        /// </value>
        [FieldOrder(0)]
        [FieldLength(4)]
        public string Magic { get; set; }

        /// <summary>
        /// Gets or sets the header length.
        /// </summary>
        /// <value>
        /// The header length.
        /// </value>
        [FieldOrder(1)]
        [FieldLength(4)]
        public uint LenHeader { get; set; }

        /// <summary>
        /// Gets or sets the length of the entry.
        /// </summary>
        /// <value>
        /// The length of the entry.
        /// </value>
        [FieldOrder(2)]
        [FieldLength(4)]
        public uint LenEntry { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a hot cue.
        /// </summary>
        /// <value>
        ///   <c>true</c> if it's a hot cue; otherwise, <c>false</c>.
        /// </value>
        [FieldOrder(3)]
        [FieldLength(4)]
        public bool HotCue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CueEntry"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        [FieldOrder(4)]
        [FieldLength(4)]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets an unknown value.
        /// </summary>
        /// <value>
        /// The unknown value.
        /// </value>
        [FieldOrder(5)]
        [FieldLength(4)]
        public uint Unknown { get; set; }

        /// <summary>
        /// Gets or sets a value that is 0xffff for first cue, 0,1,3 for next.
        /// </summary>
        /// <value>
        /// The order value.
        /// </value>
        [FieldOrder(6)]
        [FieldLength(2)]
        public ushort OrderFirst { get; set; }

        /// <summary>
        /// Gets or sets a value that is 1,2,3 for first, second, third cue, 0xffff for last.
        /// </summary>
        /// <value>
        /// The order value.
        /// </value>
        [FieldOrder(7)]
        [FieldLength(2)]
        public ushort OrderLast { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [FieldOrder(8)]
        [FieldLength(1)]
        public CueEntryType Type { get; set; }

        /// <summary>
        /// Gets or sets an unknown value.
        /// </summary>
        /// <value>
        /// The unknown value.
        /// </value>
        [FieldOrder(9)]
        [FieldLength(3)]
        public uint Unknown2 { get; set; }

        /// <summary>
        /// Gets or sets the time in ms.
        /// </summary>
        /// <value>
        /// The time in ms.
        /// </value>
        [FieldOrder(10)]
        [FieldLength(4)]
        public uint Time { get; set; }

        /// <summary>
        /// Gets or sets the loop end time in ms.
        /// </summary>
        /// <value>
        /// The loop end time in ms.
        /// </value>
        [FieldOrder(11)]
        [FieldLength(4)]
        public uint LoopTime { get; set; }

        /// <summary>
        /// Gets or sets an unknown value.
        /// </summary>
        /// <value>
        /// The unknown value.
        /// </value>
        [FieldOrder(12)]
        [FieldLength(16)]
        public byte[] Unknown3 { get; set; }
    }
}