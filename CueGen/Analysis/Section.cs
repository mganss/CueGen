using BinarySerialization;
using System.Collections.Generic;

namespace CueGen.Analysis
{
    public class Section
    {
        [FieldOrder(0)]
        [FieldLength(4)]
        public string Magic { get; set; }

        [FieldOrder(1)]
        [FieldLength(4)]
        public uint LenHeader { get; set; }

        [FieldOrder(2)]
        [FieldLength(4)]
        public uint LenTag { get; set; }

        [FieldOrder(3)]
        [Subtype(nameof(Magic), "PCOB", typeof(CueTag))]
        [Subtype(nameof(Magic), "PCO2", typeof(CueExtendedTag))]
        [Subtype(nameof(Magic), "PQTZ", typeof(BeatGridTag))]
        [Subtype(nameof(Magic), "PSSI", typeof(PhraseTag))]
        [SubtypeDefault(typeof(UnknownTag))]
        [FieldLength(nameof(LenTag), ConverterType = typeof(LengthConverter), ConverterParameter = 12)]
        public Tag Tag { get; set; }
    }
}