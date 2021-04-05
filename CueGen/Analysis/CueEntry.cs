using BinarySerialization;
using System.Diagnostics.CodeAnalysis;

namespace CueGen.Analysis
{
    public enum CueEntryType: byte
    {
        Memory = 1,
        Loop = 2
    }

    [ExcludeFromCodeCoverage]
    public class CueEntry
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
        public bool HotCue { get; set; }

        [FieldOrder(4)]
        [FieldLength(4)]
        public bool Enabled { get; set; }

        [FieldOrder(5)]
        [FieldLength(4)]
        public uint Unknown { get; set; }

        [FieldOrder(6)]
        [FieldLength(2)]
        public ushort OrderFirst { get; set; }

        [FieldOrder(7)]
        [FieldLength(2)]
        public ushort OrderLast { get; set; }

        [FieldOrder(8)]
        [FieldLength(1)]
        public CueEntryType Type { get; set; }

        [FieldOrder(9)]
        [FieldLength(3)]
        public uint Unknown2 { get; set; }

        [FieldOrder(10)]
        [FieldLength(4)]
        public uint Time { get; set; }

        [FieldOrder(11)]
        [FieldLength(4)]
        public uint LoopTime { get; set; }

        [FieldOrder(12)]
        [FieldLength(16)]
        public byte[] Unknown3 { get; set; }
    }
}