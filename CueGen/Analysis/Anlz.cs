using BinarySerialization;
using System;
using System.Collections.Generic;

namespace CueGen.Analysis
{
    public class Anlz
    {
        [FieldOrder(0)]
        [FieldLength(4)]
        public string Magic { get; set; }

        [FieldOrder(1)]
        [FieldLength(4)]
        public uint LenHeader { get; set; }

        [FieldOrder(2)]
        [FieldLength(4)]
        public uint LenFile { get; set; }

        [FieldOrder(3)]
        [FieldLength(nameof(LenHeader), ConverterType = typeof(LengthConverter), ConverterParameter = 12)]
#pragma warning disable CA1819 // Properties should not return arrays
        public byte[] Unknown { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

        [FieldOrder(4)]
        public List<Section> Sections { get; set; }

        public static Anlz Deserialize(byte[] bytes)
        {
            var serializer = new BinarySerializer { Endianness = Endianness.Big };
            var anlz = serializer.Deserialize<Anlz>(bytes);
            return anlz;
        }
    }
}
