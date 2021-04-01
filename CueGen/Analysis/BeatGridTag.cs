using BinarySerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CueGen.Analysis
{
    public class BeatGridTag: Tag
    {
        [FieldOrder(0)]
        [FieldLength(4)]
        public uint Unknown{ get; set; }

        [FieldOrder(1)]
        [FieldLength(4)]
        public uint Unknown2 { get; set; }

        [FieldOrder(2)]
        [FieldLength(4)]
        public uint Length { get; set; }

        [FieldOrder(3)]
        [FieldCount(nameof(Length))]
        public List<BeatGridEntry> Beats { get; set; }
    }
}
