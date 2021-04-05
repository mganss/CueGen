using BinarySerialization;
using System.Diagnostics.CodeAnalysis;

namespace CueGen.Analysis
{
    [ExcludeFromCodeCoverage]
    public class CueExtendedEntry
    {
        [FieldOrder(0)]
        [FieldLength(4)]
        public string Magic { get; set; }

        [FieldOrder(1)]
        [FieldLength(4)]
        public uint LenHeader { get; set; }

        [FieldOrder(2)]
        [FieldLength(4)]
        public uint LenEntry { get; set; }

        [FieldOrder(3)]
        [FieldLength(4)]
        public uint HotCue { get; set; }

        [FieldOrder(4)]
        [FieldLength(1)]
        public CueEntryType Type { get; set; }

        [FieldOrder(5)]
        [FieldLength(3)]
        public uint Unknown { get; set; }

        [FieldOrder(6)]
        [FieldLength(4)]
        public uint Time { get; set; }

        [FieldOrder(7)]
        [FieldLength(4)]
        public uint LoopTime { get; set; }

        [FieldOrder(8)]
        [FieldLength(1)]
        public int ColorId { get; set; }

        [FieldOrder(9)]
        [FieldLength(11)]
        public byte[] Unknown2 { get; set; }

        [FieldOrder(10)]
        [FieldLength(4)]
        public uint LenComment { get; set; }

        [FieldOrder(11)]
        [FieldLength(nameof(LenComment))]
        [FieldEncoding("utf-16BE")]
        public string Comment { get; set; }

        [FieldOrder(12)]
        [FieldLength(1)]
        public int ColorCode { get; set; }

        [FieldOrder(13)]
        [FieldLength(1)]
        public int ColorRed { get; set; }

        [FieldOrder(14)]
        [FieldLength(1)]
        public int ColorGreen { get; set; }

        [FieldOrder(15)]
        [FieldLength(1)]
        public int ColorBlue { get; set; }

        [FieldOrder(16)]
        public byte[] Unknown3 { get; set; }
    }

}