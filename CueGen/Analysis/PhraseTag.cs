using BinarySerialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace CueGen.Analysis
{
    public enum Mood : ushort
    {
        High = 1,
        Mid = 2,
        Low = 3
    }

    public class PhraseTag : Tag
    {
        [FieldOrder(0)]
        [FieldLength(4)]
        public uint LenEntry { get; set; }

        [FieldOrder(1)]
        [FieldLength(2)]
        public ushort Length { get; set; }

        [FieldOrder(2)]
        [FieldLength(2)]
        public Mood Mood { get; set; }

        [FieldOrder(3)]
        [FieldLength(6)]
#pragma warning disable CA1819 // Properties should not return arrays
        public byte[] Unknown { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

        [FieldOrder(4)]
        [FieldLength(2)]
        public ushort EndBeat { get; set; }

        [FieldOrder(5)]
        [FieldLength(2)]
        public ushort Unknown2 { get; set; }

        [FieldOrder(6)]
        [FieldLength(1)]
        public byte Bank { get; set; }

        [FieldOrder(7)]
        [FieldLength(1)]
        public byte Unknown3 { get; set; }

        [FieldOrder(8)]
        [FieldCount(nameof(Length))]
        public List<PhraseEntry> Phrases { get; set; }
    }
}
