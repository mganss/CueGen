using CueGen.Analysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CueGen
{
    public class Config
    {
        public string DatabasePath { get; set; }
        public bool UseSqlCipher { get; set; } = true;
        public bool HotCues { get; set; } = false;
        public bool Merge { get; set; } = true;
        public int MinDistanceBars { get; set; } = 4;
        public bool DryRun { get; set; }
        public List<int> Colors { get; set; } = new();
        public int MaxCues { get; set; } = 8;
        public string Comment { get; set; }
        public string FileGlob { get; set; }
        public bool RemoveOnly { get; set; }
        public bool SnapToBar { get; set; } = true;
        public bool PhraseCues { get; set; }
        public bool MyTagEnergy { get; set; }
        public bool ColorEnergy { get; set; }
        public bool CueColorEnergy { get; set; } = true;
        public bool CueColorPhrase { get; set; } = true;
        public Dictionary<PhraseGroup, string> PhraseNames { get; set; }
        public Dictionary<PhraseGroup, int> PhraseOrder { get; set; }
        public int MinPhraseLength { get; set; } = 4;
        public DateTime MinCreatedDate { get; set; } = DateTime.MinValue;
        public int LoopIntroLength { get; set; } = 0;
        public int LoopOutroLength { get; set; } = 0;
    }
}
