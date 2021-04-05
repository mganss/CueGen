using BinarySerialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CueGen.Analysis
{
    /// <summary>
    /// A variation of cue_tag which was introduced with the nxs2 line,
    /// and adds descriptive names. (Still comes in two forms, either
    /// holding memory cues and loop points, or holding hot cues and
    /// loop points.) Also includes hot cues D through H and color assignment.
    /// </summary>
    /// <seealso cref="CueGen.Analysis.Tag" />
    [ExcludeFromCodeCoverage]
    public class CueExtendedTag : Tag
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [FieldOrder(0)]
        [FieldLength(4)]
        public CueType Type { get; set; }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        [FieldOrder(1)]
        [FieldLength(2)]
        public ushort Length { get; set; }

        /// <summary>
        /// Gets or sets an unknown value.
        /// </summary>
        /// <value>
        /// The unknown value.
        /// </value>
        [FieldOrder(2)]
        [FieldLength(2)]
        public ushort Unknown { get; set; }

        /// <summary>
        /// Gets or sets the cues.
        /// </summary>
        /// <value>
        /// The cues.
        /// </value>
        [FieldOrder(3)]
        [FieldCount(nameof(Length))]
        public List<CueExtendedEntry> Cues { get; set; }
    }
}
