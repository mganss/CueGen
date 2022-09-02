using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CueGen.Analysis;
using NLog;

namespace CueGen.Test
{
    [TestFixture]
    public class Tests
    {
        public Generator Gen { get; set; }

        [SetUp]
        public void Init()
        {
            var testName = TestContext.CurrentContext.Test.Name;
            var db = $"test.{testName}.db";
            File.Copy("test.db", db, overwrite: true);
            Gen = new Generator(new Config
            {
                DatabasePath = db,
                UseSqlCipher = false
            });
        }

        static readonly Regex dateTimeRegex = new(@"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d{1,3})?");
        static readonly Regex idRegex = new(@"(""(UU)?ID""): ""([^""]+)""");

        private static string ReplaceJson(string json)
        {
            json = dateTimeRegex.Replace(json, "2021-01-01T00:00:00.000");
            json = idRegex.Replace(json, @"$1: """"");
            return json;
        }

        private void AssertContent()
        {
            var testName = TestContext.CurrentContext.Test.Name;
            var contents = Gen.GetContents();
            var contentsJson = JsonConvert.SerializeObject(contents, Formatting.Indented);
            contentsJson = ReplaceJson(contentsJson);
            File.WriteAllText($"{testName}.json", contentsJson);
            var expectedContentsJson = File.ReadAllText($"json/{testName}.json");
            expectedContentsJson = ReplaceJson(expectedContentsJson);
            Assert.AreEqual(expectedContentsJson, contentsJson);
        }

        [Test]
        public void ReadDatabaseTest()
        {
            AssertContent();
        }

        [Test]
        public void GenerateContentCuesTest()
        {
            var cues = Gen.GetCues();
            var contentCues = Gen.GetContentCues();

            foreach (var cc in contentCues)
            {
                var contentCue = new ContentCue();
                var cs = cues.Where(c => c.ContentID == cc.ContentID).OrderBy(c => c.created_at);
                contentCue.SetCues(cs);
                Assert.AreEqual(cc.Cues, contentCue.Cues);
            }
        }

        [Test]
        public void ReadTagsTest()
        {
            var contents = Gen.GetContents();
            var tagFiles = contents.Select(c => new TagFile(c.FolderPath)).ToList();
            var tagFilesJson = JsonConvert.SerializeObject(tagFiles, Formatting.Indented);
            File.WriteAllText("tagFiles.json", tagFilesJson);
            var expectedTagFilesJson = File.ReadAllText("json/tagFiles.json");
            Assert.AreEqual(expectedTagFilesJson, tagFilesJson);
        }

        [Test]
        public void FlacTest()
        {
            var tagFile = new TagFile("content/Microgainz - Effigy.flac");
            var tagFileJson = JsonConvert.SerializeObject(tagFile, Formatting.Indented);
            var expectedTagFileJson = File.ReadAllText("json/tagFileFlac.json");
            Assert.AreEqual(expectedTagFileJson, tagFileJson);
        }

        [Test]
        public void CreateCuesTest()
        {
            Gen.Config.SnapToBar = false;
            Gen.Config.MyTagEnergy = true;
            Gen.Config.ColorEnergy = true;
            Gen.Generate();

            AssertContent();
        }

        [Test]
        public void CreateMinDistanceCuesTest()
        {
            Gen.Config.MinDistanceBars = 64;
            Gen.Generate();

            AssertContent();
        }

        [Test]
        public void CreateHotCuesTest()
        {
            Gen.Config.HotCues = true;
            Gen.Config.Merge = false;
            Gen.Config.Colors = new List<int> { 1, 3, 5, 7, 9, 11, 13, 15 };
            Gen.Config.CueColorEnergy = false;
            Gen.Config.MaxCues = 4;
            Gen.Config.Comment = "#";

            Gen.Generate();

            AssertContent();
        }

        [Test]
        public void RemoveOnlyTest()
        {
            Gen.Generate();

            Gen.Config.FileGlob = "content/*Microgainz - M*";
            Gen.Config.RemoveOnly = true;

            Gen.Generate();

            AssertContent();
        }

        [Test]
        public void BeatGridTest()
        {
            var bytes = File.ReadAllBytes("share/PIONEER/USBANLZ/b72/2f306-40b5-4a55-8ae0-663c684ff0be/ANLZ0000.DAT");
            var anlz = Anlz.Deserialize(bytes);

            Assert.AreEqual(7, anlz.Sections.Count);

            var beatGridTag = anlz.Sections[2].Tag as BeatGridTag;

            Assert.NotNull(beatGridTag);
            Assert.AreEqual(980, beatGridTag.Beats.Count);
            Assert.AreEqual(13, beatGridTag.Beats[0].Time);
        }

        [Test]
        public void PhraseTest()
        {
            var bytes = File.ReadAllBytes("share/PIONEER/USBANLZ/b72/2f306-40b5-4a55-8ae0-663c684ff0be/ANLZ0000.EXT");
            var anlz = Anlz.Deserialize(bytes);

            Assert.AreEqual(10, anlz.Sections.Count);

            var phraseTag = anlz.Sections.Last().Tag as PhraseTag;

            Assert.NotNull(phraseTag);
            Assert.AreEqual(22, phraseTag.Phrases.Count);
            Assert.AreEqual(1, phraseTag.Phrases.First().Beat);
            Assert.AreEqual(945, phraseTag.Phrases.Last().Beat);

            var kind = phraseTag.Phrases.First().Kind as PhraseHigh;

            Assert.AreEqual(MoodHighPhrase.Intro, kind.Id);

            kind = phraseTag.Phrases.Last().Kind as PhraseHigh;

            Assert.AreEqual(MoodHighPhrase.Outro, kind.Id);
        }

        [Test]
        public void CreateCuesFromPhrasesTest()
        {
            Gen.Config.PhraseCues = true;
            Gen.Config.SnapToBar = false;
            Gen.Generate();

            AssertContent();
        }

        [Test]
        public void CreateHotCuesFromPhrasesTest()
        {
            Gen.Config.PhraseCues = true;
            Gen.Config.SnapToBar = false;
            Gen.Config.HotCues = true;
            Gen.Generate();

            AssertContent();
        }

        [Test]
        public void DryRunTest()
        {
            Gen.Config.MyTagEnergy = true;
            Gen.Config.ColorEnergy = true;
            Gen.Config.DryRun = true;
            Gen.Generate();

            AssertContent();
        }

        [Test]
        public void PhraseOrderTest()
        {
            Gen.Config.PhraseCues = true;
            Gen.Config.SnapToBar = false;
            Gen.Config.PhraseNames = new Dictionary<PhraseGroup, string>
            {
                [PhraseGroup.Intro] = "I",
                [PhraseGroup.Verse] = "V",
                [PhraseGroup.Bridge] = "B",
                [PhraseGroup.Chorus] = "C",
                [PhraseGroup.Outro] = "O",
                [PhraseGroup.Down] = "D",
                [PhraseGroup.Up] = "U",
            };
            Gen.Config.PhraseOrder = new Dictionary<PhraseGroup, int>
            {
                [PhraseGroup.Intro] = 0,
                [PhraseGroup.Verse] = 1,
                [PhraseGroup.Bridge] = 2,
                [PhraseGroup.Chorus] = 3,
                [PhraseGroup.Outro] = 4,
                [PhraseGroup.Down] = 5,
                [PhraseGroup.Up] = 6,
            };
            Gen.Config.MinPhraseLength = 1;
            Gen.Generate();

            AssertContent();
        }

        [Test]
        public void FileGlobTest()
        {
            Gen.Config.FileGlob = "**/*Lambda*";
            Gen.Generate();

            AssertContent();
        }

        [Test]
        public void MinCreatedDateTest()
        {
            Gen.Config.MinCreatedDate = new DateTime(2021, 01, 27, 11, 52, 0, DateTimeKind.Utc);
            Gen.Generate();

            AssertContent();
        }

        [Test]
        public void LoopTest()
        {
            Gen.Config.LoopIntroLength = 4;
            Gen.Config.LoopOutroLength = 8;
            Gen.Generate();

            AssertContent();
        }


        [Test]
        public void LoopHotCueTest()
        {
            Gen.Config.LoopIntroLength = 4;
            Gen.Config.LoopOutroLength = 8;
            Gen.Config.HotCues = true;
            Gen.Generate();

            AssertContent();
        }

        private void AssertMyTags()
        {
            var myTags = Gen.GetMyTags();
            var songMyTags = Gen.GetSongMyTags();
            var testName = TestContext.CurrentContext.Test.Name;

            foreach (var myTag in myTags)
                myTag.UUID = "";

            var generatedMyTagsJson = JsonConvert.SerializeObject(myTags, Formatting.Indented);
            File.WriteAllText($"{testName}.MyTags.json", generatedMyTagsJson); ;
            var expectedGeneratedMyTagsJson = File.ReadAllText($"json/{testName}.MyTags.json");
            generatedMyTagsJson = dateTimeRegex.Replace(generatedMyTagsJson, "2021-01-01T00:00:00.000");
            expectedGeneratedMyTagsJson = dateTimeRegex.Replace(expectedGeneratedMyTagsJson, "2021-01-01T00:00:00.000");

            Assert.AreEqual(expectedGeneratedMyTagsJson, generatedMyTagsJson);

            foreach (var songMyTag in songMyTags)
            {
                songMyTag.UUID = "";
                songMyTag.ID = "";
            }

            var generatedSongMyTagsJson = JsonConvert.SerializeObject(songMyTags, Formatting.Indented);
            File.WriteAllText($"{testName}.SongMyTags.json", generatedSongMyTagsJson); ;
            var expectedGeneratedSongMyTagsJson = File.ReadAllText($"json/{testName}.SongMyTags.json");
            generatedSongMyTagsJson = dateTimeRegex.Replace(generatedSongMyTagsJson, "2021-01-01T00:00:00.000");
            expectedGeneratedSongMyTagsJson = dateTimeRegex.Replace(expectedGeneratedSongMyTagsJson, "2021-01-01T00:00:00.000");

            Assert.AreEqual(expectedGeneratedSongMyTagsJson, generatedSongMyTagsJson);
        }

        [Test]
        public void EnergyMyTagTest()
        {
            Gen.Config.MyTagEnergy = true;
            Gen.Generate();

            AssertMyTags();
        }

        [Test]
        public void RemoveEnergyMyTagTest()
        {
            Gen.Config.MyTagEnergy = true;
            Gen.Generate();
            Gen.Config.RemoveOnly = true;
            Gen.Generate();

            AssertMyTags();
        }

        [Test]
        public void OffsetTest()
        {
            Gen.Config.CueOffset = 16;
            Gen.Generate();

            AssertContent();
        }

        [Test]
        public void OffsetNegativeTest()
        {
            Gen.Config.CueOffset = -16;
            Gen.Generate();

            AssertContent();
        }
    }
}
