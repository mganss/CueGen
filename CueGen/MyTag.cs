using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CueGen
{
    [Table("djmdMyTag")]
    public class MyTag: CommonTable
    {
        [PrimaryKey]
        public string ID { get; set; }
        public int? Seq { get; set; }
        public string Name { get; set; }
        public int? Attribute { get; set; }
        public string ParentId { get; set; }
    }
}
