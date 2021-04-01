using BinarySerialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CueGen
{
    public class SeratoCue
    {
        [FieldOrder(0)]
        public string Magic { get; set; }

        [FieldOrder(1)]
        [FieldLength(4)]
        public uint Length { get; set; }

        [FieldOrder(2)]
        [FieldLength(1)]
        public byte Zero { get; set; }

        [FieldOrder(3)]
        [FieldLength(1)]
        public byte Index { get; set; }

        [FieldOrder(4)]
        [FieldLength(4)]
        public uint Time { get; set; }

        [FieldOrder(5)]
        [FieldLength(1)]
        public byte Zero2 { get; set; }

        [FieldOrder(6)]
        [FieldLength(1)]
        public byte R { get; set; }

        [FieldOrder(7)]
        [FieldLength(1)]
        public byte G { get; set; }

        [FieldOrder(8)]
        [FieldLength(1)]
        public byte B { get; set; }

        [FieldOrder(9)]
        [FieldLength(2)]
        public ushort Zero3 { get; set; }

        [FieldOrder(10)]
        [FieldEncoding("utf-8")]
        public string Name { get; set; }

        [Ignore]
        private int _energy = -1;

        [Ignore]
        public int Energy
        {
            get
            {
                if (_energy < 0)
                {
                    if (Name.Length >= 8 && char.IsDigit(Name[7])) // "Energy N"
                        _energy = Name[7] - '0';
                    else
                        _energy = 0;
                }

                return _energy;
            }
        }
    }
}
