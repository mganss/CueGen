using BinarySerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CueGen.Analysis
{
    /// <summary>
    /// Holds a list of all the beats found within the track, recording
    /// their bar position, the time at which they occur, and the tempo
    /// at that point.
    /// </summary>
    /// <seealso cref="CueGen.Analysis.Tag" />
    public class BeatGridTag: Tag
    {
        /// <summary>
        /// Gets or sets an unknown value.
        /// </summary>
        /// <value>
        /// The unknown value.
        /// </value>
        [FieldOrder(0)]
        [FieldLength(4)]
        public uint Unknown{ get; set; }

        /// <summary>
        /// Gets or sets an unknown value.
        /// </summary>
        /// <value>
        /// The unknown value.
        /// </value>
        [FieldOrder(1)]
        [FieldLength(4)]
        public uint Unknown2 { get; set; }

        /// <summary>
        /// Gets or sets the number of beat entries which follow.
        /// </summary>
        /// <value>
        /// The number of beat entries.
        /// </value>
        [FieldOrder(2)]
        [FieldLength(4)]
        public uint Length { get; set; }

        /// <summary>
        /// Gets or sets the entries of the beat grid.
        /// </summary>
        /// <value>
        /// The beats.
        /// </value>
        [FieldOrder(3)]
        [FieldCount(nameof(Length))]
        public List<BeatGridEntry> Beats { get; set; }
    }
}
