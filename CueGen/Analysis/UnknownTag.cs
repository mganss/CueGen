using BinarySerialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace CueGen.Analysis
{
    public class UnknownTag: Tag
    {
        [FieldOrder(0)]
        //[FieldLength(nameof(Section.LenTag), RelativeSourceMode = RelativeSourceMode.FindAncestor, AncestorType = typeof(Section), ConverterType = typeof(LengthConverter), ConverterParameter = 12)]
        public byte[] Body { get; set; }
    }
}
