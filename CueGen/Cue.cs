using SQLite;
using System;
using System.ComponentModel;

namespace CueGen
{
    [Table("djmdCue")]
    public class Cue: CommonTable
    {
        [PrimaryKey]
        public string ID { get; set; }
        public string ContentID { get; set; }
        public int? InMsec { get; set; }
        public int? InFrame { get; set; }
        public int? InMpegFrame { get; set; } = 0;
        public int? InMpegAbs { get; set; } = 0;
        public int? OutMsec { get; set; } = -1;
        public int? OutFrame { get; set; } = 0;
        public int? OutMpegFrame { get; set; } = 0;
        public int? OutMpegAbs { get; set; } = 0;
        public int? Kind { get; set; } = 0;
        public int? Color { get; set; } = -1;
        public int? ColorTableIndex { get; set; }
        public int? ActiveLoop { get; set; }
        [DefaultValue("")]
        public string Comment { get; set; }
        public int? BeatLoopSize { get; set; }
        public int? CueMicrosec { get; set; }
        public string InPointSeekInfo { get; set; }
        public string OutPointSeekInfo { get; set; }
        public string ContentUUID { get; set; }
    }
}
