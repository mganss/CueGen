using BinarySerialization;

namespace CueGen.Analysis
{
    /// <summary>
    /// Describes an individual beat in a beat grid.
    /// </summary>
    public class BeatGridEntry
    {
        /// <summary>
        /// Gets or sets the the position of the beat within its musical bar, where beat 1 is the down beat.
        /// </summary>
        /// <value>
        /// The beat number.
        /// </value>
        [FieldOrder(0)]
        [FieldLength(2)]
        public ushort BeatNumber { get; set; }

        /// <summary>
        /// Gets or sets the tempo at the time of this beat, in beats per minute, multiplied by 100..
        /// </summary>
        /// <value>
        /// The tempo.
        /// </value>
        [FieldOrder(1)]
        [FieldLength(2)]
        public ushort Tempo { get; set; }

        /// <summary>
        /// Gets or sets the , in milliseconds, at which this beat occurs when the track is played at normal (100%) pitch..
        /// </summary>
        /// <value>
        /// The time.
        /// </value>
        [FieldOrder(2)]
        [FieldLength(4)]
        public uint Time { get; set; }
    }
}