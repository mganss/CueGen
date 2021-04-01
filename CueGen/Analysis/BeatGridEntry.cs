using BinarySerialization;

namespace CueGen.Analysis
{
    public class BeatGridEntry
    {
        [FieldOrder(0)]
        [FieldLength(2)]
        public ushort BeatNumber { get; set; }

        [FieldOrder(1)]
        [FieldLength(2)]
        public ushort Tempo { get; set; }

        [FieldOrder(2)]
        [FieldLength(4)]
        public uint Time { get; set; }
    }
}