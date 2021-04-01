using BinarySerialization;

namespace CueGen.Analysis
{
    public enum PhraseGroup
    {
        Intro = 1,
        Verse = 2,
        Bridge = 3,
        Chorus = 4,
        Outro = 5,
        Up = 6,
        Down = 7
    }

    public abstract class PhraseKind
    {
        [Ignore]
        public abstract PhraseGroup Group { get; }

        [Ignore]
        public abstract (byte R, byte G, byte B) RGBColor { get; }

        [Ignore]
        public abstract int ColorIndex { get; }

        [Ignore]
        public abstract int Color { get; }
    }

    public class PhraseLow : PhraseKind
    {
        [FieldOrder(0)]
        [FieldLength(2)]
        public MoodLowPhrase Id { get; set; }

        public override PhraseGroup Group
        {
            get => Id switch
            {
                MoodLowPhrase.Intro => PhraseGroup.Intro,
                MoodLowPhrase.Bridge => PhraseGroup.Bridge,
                MoodLowPhrase.Chorus => PhraseGroup.Chorus,
                MoodLowPhrase.Outro => PhraseGroup.Outro,
                _ => PhraseGroup.Verse
            };
        }

        private static readonly (byte R, byte G, byte B)[] _rgbColors =
        {
            (0xff, 0xaa, 0xb4),
            (0xa5, 0xa0, 0xff),
            (0xa5, 0xa0, 0xff),
            (0xa5, 0xa0, 0xff),
            (0xbe, 0xa0, 0xff),
            (0xbe, 0xa0, 0xff),
            (0xbe, 0xa0, 0xff),
            (0xff, 0xfa, 0xa5),
            (0xb9, 0xe1, 0xb9),
            (0x91, 0xa0, 0xb4),
        };

        public override (byte R, byte G, byte B) RGBColor => _rgbColors[(int)(Id - 1)];

        private static readonly int[] _colorIndexes = { 44, 62, 32, 26, 8, 0, 0 };

        public override int ColorIndex => _colorIndexes[(int)(Group - 1)];

        public override int Color => _colors[(int)(Group - 1)];

        private static readonly int[] _colors = { 0, 7, 3, 4, 5, 0, 0 };
    }

    public class PhraseMid : PhraseKind
    {
        [FieldOrder(0)]
        [FieldLength(2)]
        public MoodMidPhrase Id { get; set; }

        public override PhraseGroup Group
        {
            get => Id switch
            {
                MoodMidPhrase.Intro => PhraseGroup.Intro,
                MoodMidPhrase.Bridge => PhraseGroup.Bridge,
                MoodMidPhrase.Chorus => PhraseGroup.Chorus,
                MoodMidPhrase.Outro => PhraseGroup.Outro,
                _ => PhraseGroup.Verse
            };
        }

        private static readonly (byte R, byte G, byte B)[] _rgbColors =
        {
            (0xe1, 0x46, 0x46),
            (0x50, 0x6e, 0xff),
            (0x50, 0x55, 0xff),
            (0x64, 0x50, 0xff),
            (0x78, 0x50, 0xff),
            (0x8c, 0x50, 0xff),
            (0xa0, 0x50, 0xff),
            (0xe1, 0xd7, 0x41),
            (0x78, 0xc3, 0x7d),
            (0x73, 0x82, 0x96),
        };

        public override (byte R, byte G, byte B) RGBColor => _rgbColors[(int)(Id - 1)];

        private static readonly int[] _colorIndexes = { 40, 2, 29, 23, 62, 0, 0 };
        public override int ColorIndex => _colorIndexes[(int)(Group - 1)];

        public override int Color => _colors[(int)(Group - 1)];

        private static readonly int[] _colors = { 1, 6, 3, 4, 5, 0, 0 };
    }

    public class PhraseHigh : PhraseKind
    {
        [FieldOrder(0)]
        [FieldLength(2)]
        public MoodHighPhrase Id { get; set; }

        public override PhraseGroup Group
        {
            get => Id switch
            {
                MoodHighPhrase.Intro => PhraseGroup.Intro,
                MoodHighPhrase.Up => PhraseGroup.Up,
                MoodHighPhrase.Down => PhraseGroup.Down,
                MoodHighPhrase.Chorus => PhraseGroup.Chorus,
                MoodHighPhrase.Outro => PhraseGroup.Outro,
                _ => PhraseGroup.Verse
            };
        }

        private static readonly (byte R, byte G, byte B)[] _rgbColors =
        {
            (0xc8, 0x00, 0x00),
            (0x8c, 0x32, 0xff),
            (0x9b, 0x73, 0x2d),
            (0, 0, 0),
            (0x0f, 0xaa, 0x00),
            (0x50, 0x87, 0xc3),
            (0, 0, 0),
            (0, 0, 0),
            (0, 0, 0),
            (0, 0, 0),
        };

        public override (byte R, byte G, byte B) RGBColor => _rgbColors[(int)(Id - 1)];

        private static readonly int[] _colorIndexes = { 42, 0, 0, 18, 3, 57, 33 };

        public override int ColorIndex => _colorIndexes[(int)(Group - 1)];

        public override int Color => _colors[(int)(Group - 1)];

        private static readonly int[] _colors = { 1, 0, 0, 4, 5, 7, 2 };
    }

    public enum MoodLowPhrase: ushort
    {
        Intro = 1,
        Verse1 = 2,
        Verse1b = 3,
        Verse1c = 4,
        Verse2 = 5,
        Verse2b = 6,
        Verse2c = 7,
        Bridge = 8,
        Chorus = 9,
        Outro = 10
    }

    public enum MoodMidPhrase: ushort
    {
        Intro = 1,
        Verse1 = 2,
        Verse2 = 3,
        Verse3 = 4,
        Verse4 = 5,
        Verse5 = 6,
        Verse6 = 7,
        Bridge = 8,
        Chorus = 9,
        Outro = 10
    }

    public enum MoodHighPhrase: ushort
    {
        Intro = 1,
        Up = 2,
        Down = 3,
        Chorus = 5,
        Outro = 6
    }

    public class PhraseEntry
    {
        [FieldOrder(0)]
        [FieldLength(2)]
        public ushort PhraseNumber { get; set; }

        [FieldOrder(1)]
        [FieldLength(2)]
        public ushort Beat { get; set; }

        [FieldOrder(2)]
        [FieldLength(2)]
        [Subtype(nameof(PhraseTag.Mood), Mood.Low, typeof(PhraseLow), RelativeSourceMode = RelativeSourceMode.FindAncestor, AncestorType = typeof(PhraseTag))]
        [Subtype(nameof(PhraseTag.Mood), Mood.Mid, typeof(PhraseMid), RelativeSourceMode = RelativeSourceMode.FindAncestor, AncestorType = typeof(PhraseTag))]
        [Subtype(nameof(PhraseTag.Mood), Mood.High, typeof(PhraseHigh), RelativeSourceMode = RelativeSourceMode.FindAncestor, AncestorType = typeof(PhraseTag))]
        public PhraseKind Kind { get; set; }

        [FieldOrder(3)]
        [FieldLength(1)]
        public byte Unknown { get; set; }

        [FieldOrder(4)]
        [FieldLength(1)]
        public byte K1 { get; set; }

        [FieldOrder(5)]
        [FieldLength(1)]
        public byte Unknown2 { get; set; }

        [FieldOrder(6)]
        [FieldLength(1)]
        public byte K2 { get; set; }

        [FieldOrder(7)]
        [FieldLength(1)]
        public byte Unknown3 { get; set; }

        [FieldOrder(8)]
        [FieldLength(1)]
        public byte B { get; set; }

        [FieldOrder(9)]
        [FieldLength(2)]
        public ushort Beat2 { get; set; }

        [FieldOrder(10)]
        [FieldLength(2)]
        public ushort Beat3 { get; set; }

        [FieldOrder(11)]
        [FieldLength(2)]
        public ushort Beat4 { get; set; }

        [FieldOrder(12)]
        [FieldLength(1)]
        public byte Unknown4 { get; set; }

        [FieldOrder(13)]
        [FieldLength(1)]
        public byte K3 { get; set; }

        [FieldOrder(14)]
        [FieldLength(1)]
        public byte Unknown5 { get; set; }

        [FieldOrder(15)]
        [FieldLength(1)]
        public byte Fill { get; set; }

        [FieldOrder(16)]
        [FieldLength(2)]
        public ushort BeatFill { get; set; }
    }
}