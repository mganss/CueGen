using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CueGen
{
    /// <summary>
    /// Common columns of tables in the Rekordbox database.
    /// </summary>
    public class CommonTable
    {
        public string UUID { get; set; }
#pragma warning disable IDE1006 // Naming Styles
        public int rb_data_status { get; set; }
        public int rb_local_data_status { get; set; }
        public byte rb_local_deleted { get; set; }
        public byte rb_local_synced { get; set; }
        public long? usn { get; set; }
        public long? rb_local_usn { get; set; }

        private DateTime _created_at;

        [NotNull]
        public DateTime created_at
        {
            get
            {
                return _created_at;
            }

            set
            {
                if (value.Kind == DateTimeKind.Local)
                    value = value.ToUniversalTime();
                _created_at = value;
            }
        }

        private DateTime _updated_at;

        [NotNull]
        public DateTime updated_at
        {
            get
            {
                return _updated_at;
            }
            set
            {
                if (value.Kind == DateTimeKind.Local)
                    value = value.ToUniversalTime();
                _updated_at = value;
            }
        }
#pragma warning restore IDE1006 // Naming Styles
    }
}
