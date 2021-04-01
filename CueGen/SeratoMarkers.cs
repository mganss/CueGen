using BinarySerialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace CueGen
{
    public class SeratoMarkers
    {
        [FieldOrder(0)]
        [FieldLength(2)]
        public ushort Header { get; set; }

        [FieldOrder(1)]
        public List<SeratoCue> Cues { get; set; }
    }
}
