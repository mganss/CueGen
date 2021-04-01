using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CueGen
{
    [Table("contentCue")]
    public class ContentCue: CommonTable
    {
        [PrimaryKey]
        public string ID { get; set; }
        public string ContentID { get; set; }
        public string Cues { get; set; }
#pragma warning disable IDE1006 // Naming Styles
        public int? rb_cue_count { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        public void SetCues(IEnumerable<Cue> cues)
        {
            var json = JsonConvert.SerializeObject(cues, Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffzzz"
                });
            Cues = json;
        }
    }
}
