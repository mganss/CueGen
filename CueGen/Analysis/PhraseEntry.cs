using BinarySerialization;

namespace CueGen.Analysis
{
    /// <summary>
    /// Phrase grouos.
    /// </summary>
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

    /// <summary>
    /// Abstract class of phrase kinds.
    /// </summary>
    public abstract class PhraseKind
    {
        /// <summary>
        /// Gets the phrase group.
        /// </summary>
        /// <value>
        /// The phrase group.
        /// </value>
        [Ignore]
        public abstract PhraseGroup Group { get; }

        /// <summary>
        /// Gets the RGB color.
        /// </summary>
        /// <value>
        /// The RGB color.
        /// </value>
        [Ignore]
        public abstract (byte R, byte G, byte B) RGBColor { get; }

        /// <summary>
        /// Gets the index into the Rekordbox color table of 64 colors. Used for hot cues.
        /// </summary>
        /// <value>
        /// The color index.
        /// </value>
        [Ignore]
        public abstract int ColorIndex { get; }

        /// <summary>
        /// Gets the color id between 0 and 7 that references one of the 8 colors that can be selected for a track in Rekordbox.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        [Ignore]
        public abstract int Color { get; }
    }

    /// <summary>
    /// Low mood phrase kind.
    /// </summary>
    /// <seealso cref="CueGen.Analysis.PhraseKind" />
    public class PhraseLow : PhraseKind
    {
        /// <summary>
        /// Gets or sets the phrase id.
        /// </summary>
        /// <value>
        /// The phrase id.
        /// </value>
        [FieldOrder(0)]
        [FieldLength(2)]
        public MoodLowPhrase Id { get; set; }

        /// <summary>
        /// Gets the phrase group.
        /// </summary>
        /// <value>
        /// The phrase group.
        /// </value>
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

        /// <summary>
        /// Gets the RGB color.
        /// </summary>
        /// <value>
        /// The RGB color.
        /// </value>
        public override (byte R, byte G, byte B) RGBColor => _rgbColors[(int)(Id - 1)];

        private static readonly int[] _colorIndexes = { 44, 62, 32, 26, 8, 0, 0 };

        /// <summary>
        /// Gets the index into the Rekordbox color table of 64 colors. Used for hot cues.
        /// </summary>
        /// <value>
        /// The color index.
        /// </value>
        public override int ColorIndex => _colorIndexes[(int)(Group - 1)];

        /// <summary>
        /// Gets the color id between 0 and 7 that references one of the 8 colors that can be selected for a track in Rekordbox.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public override int Color => _colors[(int)(Group - 1)];

        private static readonly int[] _colors = { 0, 7, 3, 4, 5, 0, 0 };
    }

    /// <summary>
    /// Mid mood phrase.
    /// </summary>
    /// <seealso cref="CueGen.Analysis.PhraseKind" />
    public class PhraseMid : PhraseKind
    {
        /// <summary>
        /// Gets or sets the phrase id.
        /// </summary>
        /// <value>
        /// The phrase id.
        /// </value>
        [FieldOrder(0)]
        [FieldLength(2)]
        public MoodMidPhrase Id { get; set; }

        /// <summary>
        /// Gets the phrase group.
        /// </summary>
        /// <value>
        /// The phrase group.
        /// </value>
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

        /// <summary>
        /// Gets the RGB color.
        /// </summary>
        /// <value>
        /// The RGB color.
        /// </value>
        public override (byte R, byte G, byte B) RGBColor => _rgbColors[(int)(Id - 1)];

        private static readonly int[] _colorIndexes = { 40, 2, 29, 23, 62, 0, 0 };

        /// <summary>
        /// Gets the index into the Rekordbox color table of 64 colors. Used for hot cues.
        /// </summary>
        /// <value>
        /// The color index.
        /// </value>
        public override int ColorIndex => _colorIndexes[(int)(Group - 1)];

        /// <summary>
        /// Gets the color id between 0 and 7 that references one of the 8 colors that can be selected for a track in Rekordbox.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public override int Color => _colors[(int)(Group - 1)];

        private static readonly int[] _colors = { 1, 6, 3, 4, 5, 0, 0 };
    }

    /// <summary>
    /// High mood phrase.
    /// </summary>
    /// <seealso cref="CueGen.Analysis.PhraseKind" />
    public class PhraseHigh : PhraseKind
    {
        /// <summary>
        /// Gets or sets the phrase id.
        /// </summary>
        /// <value>
        /// The phrase id.
        /// </value>
        [FieldOrder(0)]
        [FieldLength(2)]
        public MoodHighPhrase Id { get; set; }

        /// <summary>
        /// Gets the phrase group.
        /// </summary>
        /// <value>
        /// The phrase group.
        /// </value>
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

        /// <summary>
        /// Gets the RGB color.
        /// </summary>
        /// <value>
        /// The RGB color.
        /// </value>
        public override (byte R, byte G, byte B) RGBColor => _rgbColors[(int)(Id - 1)];

        private static readonly int[] _colorIndexes = { 42, 0, 0, 18, 3, 57, 33 };

        /// <summary>
        /// Gets the index into the Rekordbox color table of 64 colors. Used for hot cues.
        /// </summary>
        /// <value>
        /// The color index.
        /// </value>
        public override int ColorIndex => _colorIndexes[(int)(Group - 1)];

        /// <summary>
        /// Gets the color id between 0 and 7 that references one of the 8 colors that can be selected for a track in Rekordbox.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public override int Color => _colors[(int)(Group - 1)];

        private static readonly int[] _colors = { 1, 0, 0, 4, 5, 7, 2 };
    }

    /// <summary>
    /// Low mood phrases.
    /// </summary>
    public enum MoodLowPhrase: ushort
    {
        /// <summary>
        /// Intro
        /// </summary>
        Intro = 1,
        /// <summary>
        /// Verse 1
        /// </summary>
        Verse1 = 2,
        /// <summary>
        /// Verse 1b
        /// </summary>
        Verse1b = 3,
        /// <summary>
        /// Verse 1c
        /// </summary>
        Verse1c = 4,
        /// <summary>
        /// Verse 2
        /// </summary>
        Verse2 = 5,
        /// <summary>
        /// Verse 2b
        /// </summary>
        Verse2b = 6,
        /// <summary>
        /// Verse 2c
        /// </summary>
        Verse2c = 7,
        /// <summary>
        /// Bridge
        /// </summary>
        Bridge = 8,
        /// <summary>
        /// Chorus
        /// </summary>
        Chorus = 9,
        /// <summary>
        /// Outro
        /// </summary>
        Outro = 10
    }

    /// <summary>
    /// Mid mood phrase.
    /// </summary>
    public enum MoodMidPhrase: ushort
    {
        /// <summary>
        /// Intro
        /// </summary>
        Intro = 1,
        /// <summary>
        /// Verse 1
        /// </summary>
        Verse1 = 2,
        /// <summary>
        /// Verse 2
        /// </summary>
        Verse2 = 3,
        /// <summary>
        /// Verse 3
        /// </summary>
        Verse3 = 4,
        /// <summary>
        /// Verse 4
        /// </summary>
        Verse4 = 5,
        /// <summary>
        /// Verse 5
        /// </summary>
        Verse5 = 6,
        /// <summary>
        /// Verse 6
        /// </summary>
        Verse6 = 7,
        /// <summary>
        /// Bridge
        /// </summary>
        Bridge = 8,
        /// <summary>
        /// Chorus
        /// </summary>
        Chorus = 9,
        /// <summary>
        /// Outro
        /// </summary>
        Outro = 10
    }

    /// <summary>
    /// High mood phrase.
    /// </summary>
    public enum MoodHighPhrase: ushort
    {
        /// <summary>
        /// Intro
        /// </summary>
        Intro = 1,
        /// <summary>
        /// Up
        /// </summary>
        Up = 2,
        /// <summary>
        /// Down
        /// </summary>
        Down = 3,
        /// <summary>
        /// Chorus
        /// </summary>
        Chorus = 5,
        /// <summary>
        /// Outro
        /// </summary>
        Outro = 6
    }

    /// <summary>
    /// 
    /// </summary>
    public class PhraseEntry
    {
        /// <summary>
        /// Gets or sets the absolute number of the phrase, starting at one.
        /// </summary>
        /// <value>
        /// The phrase number.
        /// </value>
        [FieldOrder(0)]
        [FieldLength(2)]
        public ushort PhraseNumber { get; set; }

        /// <summary>
        /// Gets or sets the beat number at which the phrase starts.
        /// </summary>
        /// <value>
        /// The beat.
        /// </value>
        [FieldOrder(1)]
        [FieldLength(2)]
        public ushort Beat { get; set; }

        /// <summary>
        /// Gets or sets the kind of phrase.
        /// </summary>
        /// <value>
        /// The kind of phrase.
        /// </value>
        [FieldOrder(2)]
        [FieldLength(2)]
        [Subtype(nameof(PhraseTag.Mood), Mood.Low, typeof(PhraseLow), RelativeSourceMode = RelativeSourceMode.FindAncestor, AncestorType = typeof(PhraseTag))]
        [Subtype(nameof(PhraseTag.Mood), Mood.Mid, typeof(PhraseMid), RelativeSourceMode = RelativeSourceMode.FindAncestor, AncestorType = typeof(PhraseTag))]
        [Subtype(nameof(PhraseTag.Mood), Mood.High, typeof(PhraseHigh), RelativeSourceMode = RelativeSourceMode.FindAncestor, AncestorType = typeof(PhraseTag))]
        public PhraseKind Kind { get; set; }

        /// <summary>
        /// Gets or sets a value whose purpose is unknown.
        /// </summary>
        /// <value>
        /// The unknown value.
        /// </value>
        [FieldOrder(3)]
        [FieldLength(1)]
        public byte Unknown { get; set; }

        /// <summary>
        /// Gets or sets one of three flags that identify phrase kind variants in high-mood tracks.
        /// </summary>
        /// <value>
        /// The flag.
        /// </value>
        [FieldOrder(4)]
        [FieldLength(1)]
        public byte K1 { get; set; }

        /// <summary>
        /// Gets or sets a value whose purpose is unknown.
        /// </summary>
        /// <value>
        /// The unknown value.
        /// </value>
        [FieldOrder(5)]
        [FieldLength(1)]
        public byte Unknown2 { get; set; }

        /// <summary>
        /// Gets or sets one of three flags that identify phrase kind variants in high-mood tracks.
        /// </summary>
        /// <value>
        /// The flag.
        /// </value>
        [FieldOrder(6)]
        [FieldLength(1)]
        public byte K2 { get; set; }

        /// <summary>
        /// Gets or sets a value whose purpose is unknown.
        /// </summary>
        /// <value>
        /// The unknown value.
        /// </value>
        [FieldOrder(7)]
        [FieldLength(1)]
        public byte Unknown3 { get; set; }

        /// <summary>
        /// Gets or sets a value that flags how many more beat numbers are in a high-mood "Up 3" phrase.
        /// </summary>
        /// <value>
        /// The beat numbers.
        /// </value>
        [FieldOrder(8)]
        [FieldLength(1)]
        public byte B { get; set; }

        /// <summary>
        /// Gets or sets the extra beat number (falling within phrase) always present in high-mood "Up 3" phrases.
        /// </summary>
        /// <value>
        /// The beat number.
        /// </value>
        [FieldOrder(9)]
        [FieldLength(2)]
        public ushort Beat2 { get; set; }

        /// <summary>
        /// Gets or sets the extra beat number (falling within phrase, larger than beat2) present in high-mood "Up 3" phrases when b has value 1..
        /// </summary>
        /// <value>
        /// The beat number.
        /// </value>
        [FieldOrder(10)]
        [FieldLength(2)]
        public ushort Beat3 { get; set; }

        /// <summary>
        /// Gets or sets the extra beat number (falling within phrase, larger than beat3) present in high-mood "Up 3" phrases when b has value 1..
        /// </summary>
        /// <value>
        /// The beat number.
        /// </value>
        [FieldOrder(11)]
        [FieldLength(2)]
        public ushort Beat4 { get; set; }

        /// <summary>
        /// Gets or sets a value whose purpose is unknown.
        /// </summary>
        /// <value>
        /// The unknown value.
        /// </value>
        [FieldOrder(12)]
        [FieldLength(1)]
        public byte Unknown4 { get; set; }

        /// <summary>
        /// Gets or sets one of three flags that identify phrase kind variants in high-mood tracks..
        /// </summary>
        /// <value>
        /// The flag.
        /// </value>
        [FieldOrder(13)]
        [FieldLength(1)]
        public byte K3 { get; set; }

        /// <summary>
        /// Gets or sets a value whose purpose is unknown.
        /// </summary>
        /// <value>
        /// The unknown value.
        /// </value>
        [FieldOrder(14)]
        [FieldLength(1)]
        public byte Unknown5 { get; set; }

        /// <summary>
        /// Gets or sets a value that if nonzero, signals that fill-in is present at end of phrase.
        /// </summary>
        /// <value>
        /// The fill-in flag.
        /// </value>
        [FieldOrder(15)]
        [FieldLength(1)]
        public byte Fill { get; set; }

        /// <summary>
        /// Gets or sets the beat number at which fill-in starts.
        /// </summary>
        /// <value>
        /// The beat number at which fill-in starts.
        /// </value>
        [FieldOrder(16)]
        [FieldLength(2)]
        public ushort BeatFill { get; set; }
    }
}