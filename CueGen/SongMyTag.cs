using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CueGen
{
    [Table("djmdSongMyTag")]
    public class SongMyTag: CommonTable
    {
        [PrimaryKey]
        public string ID { get; set; }
        public string MyTagID { get; set; }
        public string ContentID { get; set; }
        public int? TrackNo { get; set; }
    }
}
