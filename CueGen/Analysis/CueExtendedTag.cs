using BinarySerialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CueGen.Analysis
{
    [ExcludeFromCodeCoverage]
    public class CueExtendedTag : Tag
    {
        [FieldOrder(0)]
        [FieldLength(4)]
        public CueType Type { get; set; }

        [FieldOrder(1)]
        [FieldLength(2)]
        public ushort Length { get; set; }

        [FieldOrder(2)]
        [FieldLength(2)]
        public ushort Unknown { get; set; }

        [FieldOrder(3)]
        [FieldCount(nameof(Length))]
        public List<CueExtendedEntry> Cues { get; set; }
    }
}
