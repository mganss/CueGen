using BinarySerialization;
using System.Diagnostics.CodeAnalysis;

namespace CueGen.Analysis
{
    /// <summary>
    /// A variation of cue_tag which was introduced with the nxs2 line,
    /// and adds descriptive names. (Still comes in two forms, either
    /// holding memory cues and loop points, or holding hot cues and
    /// loop points.) Also includes hot cues D through H and color assignment.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CueExtendedEntry
    {
        /// <summary>
        /// Gets or sets the magic value. Always "PCP2".
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
        /// Gets or sets the entry length.
        /// </summary>
        /// <value>
        /// The entry length.
        /// </value>
        [FieldOrder(2)]
        [FieldLength(4)]
        public uint LenEntry { get; set; }

        /// <summary>
        /// Gets or sets a value that signals if zero, this is an ordinary memory cue, otherwise this a
        /// hot cue with the specified number.
        /// </summary>
        /// <value>
        /// The hot cue flag.
        /// </value>
        [FieldOrder(3)]
        [FieldLength(4)]
        public uint HotCue { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [FieldOrder(4)]
        [FieldLength(1)]
        public CueEntryType Type { get; set; }

        /// <summary>
        /// Gets or sets an unknown value.
        /// </summary>
        /// <value>
        /// The unknown value.
        /// </value>
        [FieldOrder(5)]
        [FieldLength(3)]
        public uint Unknown { get; set; }

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>
        /// The time.
        /// </value>
        [FieldOrder(6)]
        [FieldLength(4)]
        public uint Time { get; set; }

        /// <summary>
        /// Gets or sets the loop end time.
        /// </summary>
        /// <value>
        /// The loop end time.
        /// </value>
        [FieldOrder(7)]
        [FieldLength(4)]
        public uint LoopTime { get; set; }

        /// <summary>
        /// Gets or sets a value that references a row in the colors table if this is a memory cue or loop
        /// and has been assigned a color.
        /// </summary>
        /// <value>
        /// The color identifier.
        /// </value>
        [FieldOrder(8)]
        [FieldLength(1)]
        public int ColorId { get; set; }

        /// <summary>
        /// Gets or sets an unknown value.
        /// </summary>
        /// <value>
        /// The unknown value.
        /// </value>
        [FieldOrder(9)]
        [FieldLength(11)]
        public byte[] Unknown2 { get; set; }

        /// <summary>
        /// Gets or sets the length of the comment.
        /// </summary>
        /// <value>
        /// The length of the comment.
        /// </value>
        [FieldOrder(10)]
        [FieldLength(4)]
        public uint LenComment { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>
        /// The comment.
        /// </value>
        [FieldOrder(11)]
        [FieldLength(nameof(LenComment))]
        [FieldEncoding("utf-16BE")]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the color code for hot cues.
        /// </summary>
        /// <value>
        /// The color code.
        /// </value>
        [FieldOrder(12)]
        [FieldLength(1)]
        public int ColorCode { get; set; }

        /// <summary>
        /// Gets or sets the red component of the color.
        /// </summary>
        /// <value>
        /// The red component.
        /// </value>
        [FieldOrder(13)]
        [FieldLength(1)]
        public int ColorRed { get; set; }

        /// <summary>
        /// Gets or sets the green component of the color.
        /// </summary>
        /// <value>
        /// The green component.
        /// </value>
        [FieldOrder(14)]
        [FieldLength(1)]
        public int ColorGreen { get; set; }

        /// <summary>
        /// Gets or sets the blue component of the color.
        /// </summary>
        /// <value>
        /// The blue component.
        /// </value>
        [FieldOrder(15)]
        [FieldLength(1)]
        public int ColorBlue { get; set; }

        /// <summary>
        /// Gets or sets an unknown value.
        /// </summary>
        /// <value>
        /// The unknown value.
        /// </value>
        [FieldOrder(16)]
        public byte[] Unknown3 { get; set; }
    }

}