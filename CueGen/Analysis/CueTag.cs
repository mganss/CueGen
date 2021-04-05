using BinarySerialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CueGen.Analysis
{
    public enum CueType: uint
    {
        Memory = 0,
        Hot = 1
    }

    [ExcludeFromCodeCoverage]
    public class CueTag: Tag
    {
        [FieldOrder(0)]
        [FieldLength(4)]
        public CueType Type { get; set; }

        [FieldOrder(1)]
        [FieldLength(2)]
        public ushort Unknown { get; set; }

        [FieldOrder(2)]
        [FieldLength(2)]
        public ushort Length { get; set; }

        [FieldOrder(3)]
        [FieldLength(4)]
        public uint MemoryCount { get; set; }

        [FieldOrder(4)]
        [FieldCount(nameof(Length))]
        public List<CueEntry> Cues { get; set; }
    }
}
