using CueGen.Analysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CueGen
{
    /// <summary>
    /// Configuration class.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Gets or sets the database path.
        /// </summary>
        /// <value>
        /// The database path.
        /// </value>
        public string DatabasePath { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to use SQLCipher. Default is true.
        /// </summary>
        /// <value>
        ///   <c>true</c> if using SQLCipher; otherwise, <c>false</c>.
        /// </value>
        public bool UseSqlCipher { get; set; } = true;
        /// <summary>
        /// Gets or sets a value indicating whether to create hot cues instead of memory cues.
        /// Default is false.
        /// </summary>
        /// <value>
        ///   <c>true</c> if creating hot cues; otherwise, <c>false</c>.
        /// </value>
        public bool HotCues { get; set; } = false;
        /// <summary>
        /// Gets or sets a value indicating whether to merge generated cue points with existing ones.
        /// If this is false, existing cue points will be removed.
        /// Default is true.
        /// </summary>
        /// <value>
        ///   <c>true</c> if merge; otherwise, <c>false</c>.
        /// </value>
        public bool Merge { get; set; } = true;
        /// <summary>
        /// Gets or sets the minimum distance of cue points in bars. Default is 4.
        /// </summary>
        /// <value>
        /// The minimum distance of cue points in bars.
        /// </value>
        public int MinDistanceBars { get; set; } = 4;
        /// <summary>
        /// Gets or sets a value indicating whether to perform only a dry run without changing the database.
        /// </summary>
        /// <value>
        ///   <c>true</c> if performing a dry run; otherwise, <c>false</c>.
        /// </value>
        public bool DryRun { get; set; }
        /// <summary>
        /// Gets or sets the colors to use for cue points.
        /// </summary>
        /// <value>
        /// The colors.
        /// </value>
        public List<int> Colors { get; set; } = new();
        /// <summary>
        /// Gets or sets the maximum number of cue points to create.
        /// </summary>
        /// <value>
        /// The maximum number of cue points.
        /// </value>
        public int MaxCues { get; set; } = 8;
        /// <summary>
        /// Gets or sets the comment. The character # will be replaced by the energy level.
        /// </summary>
        /// <value>
        /// The comment.
        /// </value>
        public string Comment { get; set; }
        /// <summary>
        /// Gets or sets the file glob of track file paths to include.
        /// </summary>
        /// <value>
        /// The file glob.
        /// </value>
        public string FileGlob { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to remove all generated cue points.
        /// </summary>
        /// <value>
        ///   <c>true</c> if removing generated cue points; otherwise, <c>false</c>.
        /// </value>
        public bool RemoveOnly { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to cue points to the start of the bar.
        /// </summary>
        /// <value>
        ///   <c>true</c> if snapping to bars; otherwise, <c>false</c>.
        /// </value>
        public bool SnapToBar { get; set; } = true;
        /// <summary>
        /// Gets or sets a value indicating whether generate cue points from phrases.
        /// </summary>
        /// <value>
        ///   <c>true</c> if generating cue points from phrases; otherwise, <c>false</c>.
        /// </value>
        public bool PhraseCues { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to generate the Energy MyTag.
        /// </summary>
        /// <value>
        ///   <c>true</c> if Energy MyTag is generated; otherwise, <c>false</c>.
        /// </value>
        public bool MyTagEnergy { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to set the color of the track according to the energy level.
        /// </summary>
        /// <value>
        ///   <c>true</c> if setting track colors to the energy level; otherwise, <c>false</c>.
        /// </value>
        public bool ColorEnergy { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to set the color of a cue point according to its energy level.
        /// </summary>
        /// <value>
        ///   <c>true</c> if setting cue point color according to its energy level; otherwise, <c>false</c>.
        /// </value>
        public bool CueColorEnergy { get; set; } = true;
        /// <summary>
        /// Gets or sets a value indicating whether to the color of a cue point according to the phrase.
        /// </summary>
        /// <value>
        ///   <c>true</c> if setting cue point color according to phrase; otherwise, <c>false</c>.
        /// </value>
        public bool CueColorPhrase { get; set; } = true;
        /// <summary>
        /// Gets or sets the phrase group names.
        /// </summary>
        /// <value>
        /// The phrase group names.
        /// </value>
        public Dictionary<PhraseGroup, string> PhraseNames { get; set; }
        /// <summary>
        /// Gets or sets the order of phrase groups. Multiple phrase groups can have the same order value.
        /// </summary>
        /// <value>
        /// The phrase group order.
        /// </value>
        public Dictionary<PhraseGroup, int> PhraseOrder { get; set; }
        /// <summary>
        /// Gets or sets the minimum length of a phrase group in bars. Default is 4.
        /// </summary>
        /// <value>
        /// The minimum length of the phrase.
        /// </value>
        public int MinPhraseLength { get; set; } = 4;
        /// <summary>
        /// Gets or sets the minimum created date of a track.
        /// </summary>
        /// <value>
        /// The minimum created date.
        /// </value>
        public DateTime MinCreatedDate { get; set; } = DateTime.MinValue;
        /// <summary>
        /// Gets or sets the length of the active loop to be set for the first cue point in beats.
        /// Default is 0 (off).
        /// </summary>
        /// <value>
        /// The length of the active loop intro.
        /// </value>
        public int LoopIntroLength { get; set; } = 0;
        /// <summary>
        /// Gets or sets the length of the active loop to be set for the last cue point in beats.
        /// </summary>
        /// <value>
        /// The length of the active loop outro.
        /// </value>
        public int LoopOutroLength { get; set; } = 0;
        /// <summary>
        /// Gets or sets the offset in beats to set cue points at. Can be negative.
        /// </summary>
        /// <value>
        /// The cue point offset in beats.
        /// </value>
        public int CueOffset { get; set; } = 0;
    }
}
