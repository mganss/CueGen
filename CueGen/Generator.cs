using Microsoft.Extensions.FileSystemGlobbing;
using CueGen.Analysis;
using Newtonsoft.Json;
using NLog;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CueGen
{
    public record Status(int Total, int Count, Content Current);

    public class Generator
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public Config Config { get; set; }
        public SQLiteConnectionString ConnectionString { get; set; }

        public Progress<Status> Progress { get; } = new Progress<Status>();

        public Generator(Config config)
        {
            Config = config;
            ConnectionString = new SQLiteConnectionString(Config.DatabasePath,
                openFlags: SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite,
                storeDateTimeAsTicks: false,
                key: "402fd482c38817c35ffa8ffb8c7d93143b749e7d315df7a81732a1ff43608497",
                dateTimeStringFormat: "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffzzz");
        }

        public IList<Cue> GetCues()
        {
            Log.Info("Getting cues from database {database}", ConnectionString.DatabasePath);

            using var db = new SQLiteConnection(ConnectionString);
            var cues = db.Table<Cue>().OrderBy(c => c.ID).ToList();

            Log.Info("{cues} cues read", cues.Count);

            return cues;
        }

        public IList<Content> GetContents()
        {
            Log.Info("Getting contents from database {database}", ConnectionString.DatabasePath);

            using var db = new SQLiteConnection(ConnectionString);
            var contents = db.Table<Content>().OrderBy(c => c.ID).ToList();

            Log.Info("{count} contents read", contents.Count);

            var cues = GetCues();
            var contentCues = GetContentCues();
            var songMyTags = GetSongMyTags();

            foreach (var content in contents)
            {
                content.Cues.AddRange(cues.Where(c => c.ContentID == content.ID));
                content.ContentCues.AddRange(contentCues.Where(c => c.ContentID == content.ID));
                content.MyTags.AddRange(songMyTags.Where(t => t.ContentID == content.ID));
            }

            return contents;
        }

        public IList<ContentCue> GetContentCues()
        {
            Log.Info("Getting contentCues from database {database}", ConnectionString.DatabasePath);

            using var db = new SQLiteConnection(ConnectionString);
            var contentCues = db.Table<ContentCue>().OrderBy(c => c.ID).ToList();

            Log.Info("{count} contentCues read", contentCues.Count);

            return contentCues;
        }

        public IList<MyTag> GetMyTags()
        {
            Log.Info("Getting MyTags from database {database}", ConnectionString.DatabasePath);

            using var db = new SQLiteConnection(ConnectionString);
            var myTags = db.Table<MyTag>().OrderBy(c => c.ID).ToList();

            Log.Info("{count} myTags read", myTags.Count);

            return myTags;
        }

        public IList<SongMyTag> GetSongMyTags()
        {
            Log.Info("Getting SongMyTags from database {database}", ConnectionString.DatabasePath);

            using var db = new SQLiteConnection(ConnectionString);
            var songMyTags = db.Table<SongMyTag>().OrderBy(c => c.created_at).ToList();

            Log.Info("{count} songMyTags read", songMyTags.Count);

            return songMyTags;
        }

        IList<MyTag> CreateMyTagEnergy(SQLiteConnection db, IList<MyTag> myTags)
        {
            var energyMyTag = myTags.FirstOrDefault(t => t.Name == "Energy" && t.ParentId == "root");

            if (energyMyTag == null && !Config.RemoveOnly)
            {
                var roots = myTags.Where(t => t.ParentId == "root");
                var maxRootId = roots.Max(t => long.Parse(t.ID));
                var maxRootSeq = roots.Max(t => t.Seq) ?? 0;
                var maxRootRbLocalUsn = roots.Max(t => t.rb_local_usn) ?? 0;

                energyMyTag = new MyTag
                {
                    ID = (maxRootId + 1).ToString(),
                    Seq = maxRootSeq + 1,
                    Name = "Energy",
                    Attribute = 1,
                    ParentId = "root",
                    UUID = Guid.NewGuid().ToString(),
                    rb_local_usn = maxRootRbLocalUsn + 9,
                    created_at = DateTime.UtcNow,
                    updated_at = DateTime.UtcNow
                };

                Log.Info("Inserting MyTag Energy");

                if (!Config.DryRun)
                    db.Insert(energyMyTag);
            }
            else if (Config.RemoveOnly)
            {
                var removeMyTags = myTags.Where(t => t.ParentId == energyMyTag.ID).ToList();
                var songMyTags = GetSongMyTags();

                Log.Info("Removing MyTag Energy");

                if (!Config.DryRun)
                {
                    foreach (var songMyTag in songMyTags.Where(t => removeMyTags.Any(r => r.ID == t.MyTagID)))
                        db.Delete(songMyTag);
                    db.Table<MyTag>().Delete(t => t.ParentId == energyMyTag.ID);
                    db.Delete(energyMyTag);
                }

                return removeMyTags;
            }

            var energyMyTags = new List<MyTag>();
            var maxId = myTags.Max(t => long.Parse(t.ID));
            var maxRbLocalUsn = myTags.Max(t => t.rb_local_usn) ?? 0;

            foreach (var energy in Enumerable.Range(1, 8))
            {
                var myTag = myTags.FirstOrDefault(t => t.Name == energy.ToString() && t.ParentId == energyMyTag.ID);

                if (myTag == null)
                {
                    maxId++;
                    maxRbLocalUsn++;

                    myTag = new MyTag
                    {
                        ID = maxId.ToString(),
                        Seq = energy,
                        Name = energy.ToString(),
                        Attribute = 0,
                        ParentId = energyMyTag.ID,
                        UUID = Guid.NewGuid().ToString(),
                        rb_local_usn = maxRbLocalUsn,
                        created_at = DateTime.UtcNow,
                        updated_at = DateTime.UtcNow
                    };

                    Log.Info("Inserting MyTag Energy {energy}", energy);

                    if (!Config.DryRun)
                        db.Insert(myTag);
                }

                energyMyTags.Add(myTag);
            }

            return energyMyTags;
        }

        public bool Generate()
        {
#pragma warning disable CA1031 // Do not catch general exception types
            var contents = GetContents();
            var error = false;
            var maxId = contents.SelectMany(c => c.Cues).Select(c => ulong.Parse(c.ID)).DefaultIfEmpty().Max() + 1;
            IList<MyTag> energyMyTags = new List<MyTag>();
            long maxMyTagUsn = 0L;
            using var db = new SQLiteConnection(ConnectionString);

            if (Config.MyTagEnergy)
            {
                try
                {
                    var myTags = GetMyTags();
                    maxMyTagUsn = myTags.Max(t => t.rb_local_usn) ?? 0L;
                    db.RunInTransaction(() => energyMyTags = CreateMyTagEnergy(db, myTags));
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error occurred during creation of Energy MyTag");
                    error = true;
                }
            }

            if (Config.MinCreatedDate > DateTime.MinValue)
                contents = contents.Where(c => c.created_at >= Config.MinCreatedDate).ToList();

            if (!string.IsNullOrEmpty(Config.FileGlob))
            {
                var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
                matcher.AddInclude(Config.FileGlob);
                contents = contents.Where(c => matcher.Match(c.FolderPath).HasMatches).ToList();
            }

            var count = 0;

            foreach (var content in contents)
            {
                ((IProgress<Status>)Progress).Report(new Status(contents.Count, count, content));

                if (Config.ColorEnergy)
                {
                    try
                    {
                        db.RunInTransaction(() => CreateColorEnergy(content, db));
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error occurred when setting energy color for {contentID} from {path}", content.ID, content.FolderPath);
                        error = true;
                    }
                }

                if (Config.MyTagEnergy)
                {
                    try
                    {
                        db.RunInTransaction(() => CreateSongMyTagEnergy(content, maxMyTagUsn, energyMyTags, db));
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error occurred during creation of Energy MyTag for {contentID} from {path}", content.ID, content.FolderPath);
                        error = true;
                    }
                }

                try
                {
                    db.RunInTransaction(() => CreateCuesForContent(ref maxId, db, content));
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error occurred during creation of cues for {contentID} from {path}", content.ID, content.FolderPath);
                    error = true;
                }

                count++;
            }

            Log.Info($"Finished cue points creation {(error ? "with" : "without")} errors");

            return error;
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private void CreateColorEnergy(Content content, SQLiteConnection db)
        {
            var tagFile = content.GetTag();
            var energy = tagFile.Energy?.EnergyLevel ?? 0;

            if (energy > 0 && energy <= 8)
            {
                var colorId = 9 - energy;
                Log.Info("Setting color for {contentId} to energy {energy} (color id {colorId})", content.ID, energy, colorId);
                content.ColorID = colorId.ToString();
                db.Update(content);
            }
            else
            {
                Log.Warn("No energy level found for {contentId}", content.ID);
            }
        }

        private void CreateSongMyTagEnergy(Content content, long maxMyTagUsn, IList<MyTag> energyMyTags, SQLiteConnection db)
        {
            if (!energyMyTags.Any()) return;

            var tagFile = content.GetTag();
            var energy = tagFile.Energy?.EnergyLevel ?? 0;

            if (energy > 0 && energy <= 8)
            {
                Log.Info("Energy level for {contentId} is {energy}", content.ID, energy);
                var energyMyTag = energyMyTags[energy - 1];
                var songMyTag = content.MyTags.FirstOrDefault(t => t.MyTagID == energyMyTag.ID);
                if (songMyTag == null)
                {
                    maxMyTagUsn++;

                    songMyTag = new SongMyTag
                    {
                        ID = Guid.NewGuid().ToString(),
                        MyTagID = energyMyTag.ID,
                        ContentID = content.ID,
                        UUID = Guid.NewGuid().ToString(),
                        rb_local_usn = maxMyTagUsn,
                        created_at = DateTime.UtcNow,
                        updated_at = DateTime.UtcNow
                    };

                    Log.Info("Inserting Energy MyTag {energy} for {contentId}", energy, content.ID);

                    if (!Config.DryRun)
                        db.Insert(songMyTag);
                }
                else
                {
                    Log.Info("Energy MyTag for {contentId} already at {energy}", content.ID, energy);
                }

                foreach (var myTag in content.MyTags.Select(t => (Energy: energyMyTags.FirstOrDefault(e => e.ID == t.MyTagID), Tag: t))
                    .Where(t => t.Energy != null && t.Tag.MyTagID != energyMyTag.ID))
                {
                    Log.Info("Removing Energy MyTag {energy} for {contentId}", myTag.Energy.Seq, content.ID);
                    db.Delete(myTag.Tag);
                }
            }
            else
            {
                Log.Warn("No energy level found for {contentId}", content.ID);
            }
        }

        static readonly Dictionary<PhraseGroup, string> DefaultPhraseNames = new Dictionary<PhraseGroup, string>
        {
            [PhraseGroup.Intro] = "Intro",
            [PhraseGroup.Verse] = "Verse",
            [PhraseGroup.Bridge] = "Bridge",
            [PhraseGroup.Chorus] = "Chorus",
            [PhraseGroup.Outro] = "Outro",
            [PhraseGroup.Up] = "Up",
            [PhraseGroup.Down] = "Down",
        };

        static readonly Dictionary<PhraseGroup, int> DefaultPhraseOrder = new Dictionary<PhraseGroup, int>
        {
            [PhraseGroup.Intro] = 0,
            [PhraseGroup.Outro] = 1,
            [PhraseGroup.Verse] = 2,
            [PhraseGroup.Chorus] = 3,
            [PhraseGroup.Bridge] = 4,
            [PhraseGroup.Up] = 5,
            [PhraseGroup.Down] = 5,
        };

        private List<CuePoint> GetPhraseCuePoints(Content content)
        {
            var extAnlz = content.GetAnlz(AnalysisKind.Ext, Config);
            if (extAnlz == null || extAnlz.Sections == null) return new();
            var phraseTag = extAnlz.Sections.Select(s => s.Tag).OfType<PhraseTag>().FirstOrDefault();
            var phrases = phraseTag?.Phrases;
            if (phrases == null || !phrases.Any()) return new();

            var datAnlz = content.GetAnlz(AnalysisKind.Dat, Config);
            if (datAnlz == null || datAnlz.Sections == null) return new();
            var beats = datAnlz.Sections.Select(s => s.Tag).OfType<BeatGridTag>().FirstOrDefault()?.Beats;
            if (beats == null || !beats.Any()) return new();

            var groups = new List<List<PhraseEntry>>();
            var groupKind = -1;
            var groupPhrases = new List<PhraseEntry>();
            var phraseOrder = Config.PhraseOrder ?? DefaultPhraseOrder;
            var startBeat = -1;

            foreach (var phrase in phrases)
            {
                if (phraseOrder.TryGetValue(phrase.Kind.Group, out var order))
                {
                    if (order != groupKind)
                    {
                        if (groupPhrases.Any())
                        {
                            var len = (phrase.Beat - startBeat) / 4;
                            if (len >= Config.MinPhraseLength)
                                groups.Add(groupPhrases);
                        }
                        groupPhrases = new();
                        groupKind = order;
                        startBeat = phrase.Beat;
                    }

                    groupPhrases.Add(phrase);
                }
                else
                {
                    groupKind = -1;
                }
            }

            if (groupPhrases.Any())
            {
                var len = (phraseTag.EndBeat - startBeat) / 4;
                if (len >= Config.MinPhraseLength)
                    groups.Add(groupPhrases);
            }

            var cues = new List<CuePoint>();

            foreach (var group in groups)
            {
                var startPhrase = group.First();
                var beatNum = startPhrase.Beat - 1;
                var phraseNames = Config.PhraseNames ?? DefaultPhraseNames;

                phraseNames.TryGetValue(startPhrase.Kind.Group, out var name);

                if (beatNum >= 0 && beatNum < beats.Count)
                    cues.Add(new CuePoint
                    {
                        Name = name,
                        Time = beats[beatNum].Time,
                        Phrase = startPhrase
                    });
                else
                    Log.Warn("Beat number {beatNum} not found in grid analysis", beatNum);
            }

            return cues;
        }

        private void CreateCuesForContent(ref ulong maxId, SQLiteConnection db, Content content)
        {
            List<CuePoint> cuePoints;

            if (Config.PhraseCues)
            {
                Log.Info("Getting cue points from phrase analysis for {contentID} with file at {path}", content.ID, content.FolderPath);
                cuePoints = GetPhraseCuePoints(content) ?? new();
            }
            else
            {
                Log.Info("Reading cue points for {contentID} from tag of {path}", content.ID, content.FolderPath);
                var tagFile = new TagFile(content.FolderPath);

                cuePoints = tagFile?.SeratoMarkers?.Cues.Select(c => new CuePoint { Time = c.Time, Name = c.Name, Energy = c.Energy }).ToList();

                if (cuePoints == null || !cuePoints.Any())
                    cuePoints = tagFile?.CuePoints?.Cues ?? new();
            }

            Log.Info("Found {count} cue points", cuePoints.Count);

            var cues = content.Cues;
            var contentCues = content.ContentCues;
            var cueNum = 1;
            var bpm = content.BPM ?? 120;
            Anlz anlz = null;

            if (Config.SnapToBar)
                anlz = content.GetAnlz(AnalysisKind.Dat, Config);

            if (content.BPM == null)
                Log.Info("BPM is unknown, assuming {bpm} BPM", bpm);

            if (!Config.Merge && !Config.RemoveOnly)
            {
                Log.Info("Removing all existing cue points for {contentID}", content.ID);

                cues.Clear();

                if (!Config.DryRun)
                    db.Table<Cue>().Delete(c => c.ContentID == content.ID);
            }
            else
            {
                Log.Info("Removing existing generated cue points for {contentID}", content.ID);

                cues.RemoveAll(c => c.UUID.StartsWith(UUIDPrefix));

                if (!Config.DryRun)
                    db.Table<Cue>().Delete(c => c.ContentID == content.ID && c.UUID.StartsWith(UUIDPrefix));
            }

            if (!Config.RemoveOnly)
            {
                var cueCandidates = new List<CuePoint>();
                var maxCues = Config.MaxCues - cues.Count(c => (c.Kind == 0 && !Config.HotCues) || (c.Kind > 0 && Config.HotCues));

                maxCues = Math.Min(Config.HotCues ? 8 : 10, maxCues);

                Log.Info("Can create {cues} cue points", maxCues);

                // iterate alternatingly between front and back
                foreach (var cue in cuePoints.OrderBy(c => c.Time)
                    .Select((c, i) => (Cue: c, Index: i))
                    .OrderBy(c => Math.Min(c.Index, Math.Abs((cuePoints.Count - 1) - c.Index)))
                    .Select(c => c.Cue))
                {
                    if (cueCandidates.Count >= maxCues)
                        break;

                    if (Config.SnapToBar)
                        SnapToBar(anlz, cue);

                    var bars = Bars(cue.Time, bpm);
                    var closeCues = cues.Where(c => Math.Abs(Bars(c.InMsec ?? 0, bpm) - bars) < Config.MinDistanceBars).ToList();

                    Log.Info("Cue point candidate #{num} is at {time}ms ({bars} bars)", cueNum, cue.Time, bars);
                    if (cue.Energy > 0)
                        Log.Info("Energy is {energy}", cue.Energy);
                    if (cue.Phrase != null)
                        Log.Info("Phrase is {phrase}", DefaultPhraseNames[cue.Phrase.Kind.Group]);

                    if (!closeCues.Any())
                    {
                        cueCandidates.Add(cue);
                    }
                    else
                    {
                        Log.Info("Ignoring cue point because there is an existing cue point within {bars} bars", Config.MinDistanceBars);
                        Log.Info("Close cues:");
                        foreach (var closeCue in closeCues)
                            Log.Info("ID {cueID} at {time}ms ({bars} bars)", closeCue.ID, closeCue.InMsec, Bars(closeCue.InMsec ?? 0, bpm));
                    }

                    cueNum++;
                }

                cueNum = 1;

                foreach (var cue in cueCandidates.OrderBy(c => c.Time))
                {
                    var newCue = CreateCue(cue, cues, content, cueNum, maxId);
                    var bars = Bars(cue.Time, bpm);

                    Log.Info("Inserting cue point #{num} with id {cueId} at {time}ms ({bars} bars)", cueNum, newCue.ID, cue.Time, bars);

                    cues.Add(newCue);

                    if (!Config.DryRun)
                        db.Insert(newCue);

                    maxId++;
                    cueNum++;
                }
            }

            var contentCue = contentCues.FirstOrDefault();

            if (contentCue != null)
            {
                Log.Info("Updating contentCue {cueID}", contentCue.ID);
                contentCue.SetCues(cues.Where(c => c.ContentID == content.ID));
                if (!Config.DryRun)
                    db.Update(contentCue);
            }
        }

        private void SnapToBar(Anlz anlz, CuePoint cue)
        {
            if (anlz == null || anlz.Sections == null) return;
            var beats = anlz.Sections.Select(s => s.Tag).OfType<BeatGridTag>().FirstOrDefault()?.Beats;
            if (beats == null || !beats.Any()) return;
            var closestBar = beats.Where(b => b.BeatNumber == 1).OrderBy(b => Math.Abs(b.Time - cue.Time)).First();
            Log.Info("Snapping cue point from {time}ms to {snappedTime}ms", cue.Time, closestBar.Time);
            cue.Time = closestBar.Time;
        }

        const string UUIDPrefix = "e134b57e-5bc1-4554-";
        static readonly int[] ColorTableIndexes = { 49, 56, 60, 62, 1, 5, 9, 14, 18, 22, 26, 30, 32, 38, 42, 45 };
        static readonly List<int> DefaultColorIndexes = new List<int> { 1, 4, 6, 9, 12, 13, 14, 15 };

        (int Color, int? ColorIndex) GetColor(CuePoint cue, int cueNum)
        {
            var color = -1;
            int? colorIndex = null;
            var val = cueNum - 1;

            if (Config.CueColorEnergy && cue.Energy > 0 && cue.Energy <= 8)
                val = cue.Energy - 1;
            else if (Config.CueColorPhrase && cue.Phrase != null)
            {
                if (!Config.Colors.Any())
                {
                    if (!Config.HotCues)
                        color = cue.Phrase.Kind.Color;
                    else
                        colorIndex = cue.Phrase.Kind.ColorIndex;

                    return (color, colorIndex);
                }

                val = (int)cue.Phrase.Kind.Group;
            }

            if (!Config.HotCues)
            {
                if (!Config.Colors.Any())
                    color = 7 - (val % 8);
                else
                    color = Math.Clamp(Config.Colors[val % Config.Colors.Count], 0, 7);
            }
            else
            {
                var colors = Config.Colors.Any() ? Config.Colors : DefaultColorIndexes;
                colorIndex = ColorTableIndexes[Math.Clamp(colors[val % colors.Count], 0, ColorTableIndexes.Length - 1)];
            }

            return (color, colorIndex);
        }

        Cue CreateCue(CuePoint cue, IList<Cue> cues, Content content, int cueNum, ulong maxId)
        {
            var frame = (int)((cue.Time * 150.0) / 1000.0);
            var date = DateTime.UtcNow;
            var kind = 0;
            var maxIdHex = maxId.ToString("x12");
            var uuid = $"{UUIDPrefix}{maxIdHex[0..4]}-{maxIdHex[4..]}";
            (var color, var colorIndex) = GetColor(cue, cueNum);

            if (Config.HotCues)
            {
                var maxKind = cues.Select(c => c.Kind ?? 0).DefaultIfEmpty().Max();
                kind = maxKind + 1;
                if (kind == 4) kind++;
            }

            var newCue = new Cue
            {
                ID = maxId.ToString(),
                InMsec = (int)cue.Time,
                InFrame = frame,
                ContentID = content.ID,
                Kind = kind,
                ColorTableIndex = colorIndex,
                Color = color,
                ContentUUID = content.UUID,
                UUID = uuid,
                created_at = date,
                updated_at = date,
            };

            if (!string.IsNullOrEmpty(Config.Comment) && cue.Energy > 0)
                newCue.Comment = Config.Comment.Replace("#", cue.Energy.ToString());
            else if (!string.IsNullOrEmpty(cue.Name))
                newCue.Comment = cue.Name;

            Log.Info("Created cue point {json}", JsonConvert.SerializeObject(newCue));

            return newCue;
        }

        int MsToBeats(double ms, int bpm) => (int)Math.Round(bpm * (ms / (60.0 * 1000.0)) / 100.0);
        int Bars(double ms, int bpm) => MsToBeats(ms, bpm) / 4 + 1;
    }
}
