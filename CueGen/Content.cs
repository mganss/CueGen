using CueGen.Analysis;
using NLog;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CueGen
{
    public enum AnalysisKind
    {
        Dat,
        Ext
    }

    [Table("djmdContent")]
    public class Content: CommonTable
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();

        [PrimaryKey]
        public string ID { get; set; }
        public string FolderPath { get; set; }
        public string FileNameL { get; set; }
        public string FileNameS { get; set; }
        public string Title { get; set; }
        public string ArtistID { get; set; }
        public string AlbumID { get; set; }
        public string GenreID { get; set; }
        public int? BPM { get; set; }
        public int? Length { get; set; }
        public int? TrackNo { get; set; }
        public int? BitRate { get; set; }
        public int? BitDepth { get; set; }
        public string Commnt { get; set; }
        public int? FileType { get; set; }
        public int? Rating { get; set; }
        public int? ReleaseYear { get; set; }
        public string RemixerID { get; set; }
        public string LabelID { get; set; }
        public string OrgArtistID { get; set; }
        public string KeyID { get; set; }
        public string StockDate { get; set; }
        public string ColorID { get; set; }
        public int? DJPlayCount { get; set; }
        public string ImagePath { get; set; }
        public string MasterDBID { get; set; }
        public string MasterSongID { get; set; }
        public string AnalysisDataPath { get; set; }
        public string SearchStr { get; set; }
        public int? FileSize { get; set; }
        public int? DiscNo { get; set; }
        public string ComposerID { get; set; }
        public string Subtitle { get; set; }
        public int? SampleRate { get; set; }
        public int? DisableQuantize { get; set; }
        public int? Analysed { get; set; }
        public string ReleaseDate { get; set; }
        public string DateCreated { get; set; }
        public int? ContentLink { get; set; }
        public string Tag { get; set; }
        public string ModifiedByRBM { get; set; }
        public string HotCueAutoLoad { get; set; }
        public string DeliveryControl { get; set; }
        public string DeliveryComment { get; set; }
        public string CueUpdated { get; set; }
        public string AnalysisUpdated { get; set; }
        public string TrackInfoUpdated { get; set; }
        public string Lyricist { get; set; }
        public string ISRC { get; set; }
        public int? SamplerTrackInfo { get; set; }
        public int? SamplerPlayOffset { get; set; }
        public float? SamplerGain { get; set; }
        public string VideoAssociate { get; set; }
        public int? LyricStatus { get; set; }
        public int? ServiceID { get; set; }
        public string OrgFolderPath { get; set; }
        public string Reserved1 { get; set; }
        public string Reserved2 { get; set; }
        public string Reserved3 { get; set; }
        public string Reserved4 { get; set; }
        public string ExtInfo { get; set; }
#pragma warning disable IDE1006 // Naming Styles
        public string rb_file_id { get; set; }
        public string DeviceID { get; set; }
        public string rb_LocalFolderPath { get; set; }
#pragma warning restore IDE1006 // Naming Styles
        public string SrcID { get; set; }
        public string SrcTitle { get; set; }
        public string SrcArtistName { get; set; }
        public string SrcAlbumName { get; set; }
        public int? SrcLength { get; set; }

        [Ignore]
        public List<ContentCue> ContentCues { get; private set; } = new();
        [Ignore]
        public List<Cue> Cues { get; private set; } = new();
        [Ignore]
        public List<SongMyTag> MyTags { get; private set; } = new();

        private Anlz dat;
        private Anlz ext;

        public Anlz GetAnlz(AnalysisKind kind, Config config)
        {
            var cachedAnlz = kind == AnalysisKind.Dat ? dat : ext;
            if (cachedAnlz != null) return cachedAnlz;

            var analysisDataPath = AnalysisDataPath;

            if (string.IsNullOrEmpty(analysisDataPath))
            {
                Log.Warn("No analysis file for {contentID}. Proceeding without", ID);
                return null;
            }

            if (kind == AnalysisKind.Ext)
                analysisDataPath = analysisDataPath.Replace(".DAT", ".EXT", StringComparison.OrdinalIgnoreCase);

            var path = Path.Join(Environment.ExpandEnvironmentVariables(@"%AppData%\Pioneer\rekordbox\share"), analysisDataPath);

            if (!File.Exists(path))
            {
                Log.Warn("Analysis file {path} does not exist", path);
                path = Path.Join(Path.GetDirectoryName(config.DatabasePath), "share", analysisDataPath);
                if (!File.Exists(path))
                {
                    Log.Warn("Analysis file {path} does not exist. Giving up", path);
                    return null;
                }
            }

            Log.Info("Reading analysis data for {contentID} from {path}", ID, path);

            var bytes = File.ReadAllBytes(path);
            var anlz = Anlz.Deserialize(bytes);

            if (anlz == null || anlz.Sections == null || !anlz.Sections.Any())
                Log.Warn("No analysis data found for {contentID} in {path}", ID, path);

            if (anlz == null) anlz = new();

            if (kind == AnalysisKind.Dat)
                dat = anlz;
            else
                ext = anlz;

            return anlz;
        }

        private TagFile tag;

        public TagFile GetTag()
        {
            if (tag == null)
            {
                Log.Info("Reading tag for {contentID} from {path}", ID, FolderPath);
                tag = new TagFile(FolderPath);
            }

            return tag;
        }
    }
}
