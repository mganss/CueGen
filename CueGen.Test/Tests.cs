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
        static readonly Logger Log = LogManager.GetCurrentClassLogger();
        public Generator Gen { get; set; }

        [SetUp]
        public void Init()
        {
            var testName = TestContext.CurrentContext.Test.Name;
            var db = $"test.{testName}.db";
            File.Copy("test.db", db, overwrite: true);
            Gen = new Generator(new Config
            {
                DatabasePath = db
            });
        }

        [Test]
        public void ReadDatabaseTest()
        {
            var contents = Gen.GetContents();
            var cues = contents.SelectMany(c => c.Cues).OrderBy(c => c.ID).ToList();
            var contentCues = contents.SelectMany(c => c.ContentCues).OrderBy(c => c.ID).ToList();
            var contentsJson = JsonConvert.SerializeObject(contents, Formatting.Indented);
            var cuesJson = JsonConvert.SerializeObject(cues, Formatting.Indented);
            var contentCuesJson = JsonConvert.SerializeObject(contentCues, Formatting.Indented);
            var expectedContentsJson = File.ReadAllText("json/contents.json");
            var expectedCuesJson = File.ReadAllText("json/cues.json");
            var expectedContentCuesJson = File.ReadAllText("json/contentCues.json");
            Assert.AreEqual(expectedContentsJson, contentsJson);
            Assert.AreEqual(expectedCuesJson, cuesJson);
            Assert.AreEqual(expectedContentCuesJson, contentCuesJson);
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

        static readonly Regex dateTimeRegex = new Regex(@"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d{1,3})?");

        private void AssertCues()
        {
            var cues = Gen.GetCues();
            var contentCues = Gen.GetContentCues();
            var testName = TestContext.CurrentContext.Test.Name;

            var generatedCuesJson = JsonConvert.SerializeObject(cues, Formatting.Indented);
            File.WriteAllText($"{testName}.json", generatedCuesJson); ;
            var expectedGeneratedCuesJson = File.ReadAllText($"json/{testName}.json");
            generatedCuesJson = dateTimeRegex.Replace(generatedCuesJson, "2021-01-01T00:00:00.000");
            expectedGeneratedCuesJson = dateTimeRegex.Replace(expectedGeneratedCuesJson, "2021-01-01T00:00:00.000");

            Assert.AreEqual(expectedGeneratedCuesJson, generatedCuesJson);

            var generatedContentCuesJson = JsonConvert.SerializeObject(contentCues, Formatting.Indented);
            File.WriteAllText($"{testName}Content.json", generatedContentCuesJson); ;
            var expectedGeneratedContentCuesJson = File.ReadAllText($"json/{testName}Content.json");
            generatedContentCuesJson = dateTimeRegex.Replace(generatedContentCuesJson, "2021-01-01T00:00:00.000");
            expectedGeneratedContentCuesJson = dateTimeRegex.Replace(expectedGeneratedContentCuesJson, "2021-01-01T00:00:00.000");

            Assert.AreEqual(expectedGeneratedContentCuesJson, generatedContentCuesJson);
        }

        [Test]
        public void CreateCuesTest()
        {
            Gen.Config.SnapToBar = false;
            Gen.Config.MyTagEnergy = true;
            Gen.Config.ColorEnergy = true;
            Gen.Generate();

            AssertCues();
        }

        [Test]
        public void CreateHotCuesTest()
        {
            Gen.Config.HotCues = true;
            Gen.Config.Merge = false;
            Gen.Config.Colors = new List<int> { 1, 3, 5, 7, 9, 11, 13, 15 };
            Gen.Config.MaxCues = 4;
            Gen.Config.Comment = "#";

            Gen.Generate();

            AssertCues();
        }

        [Test]
        public void RemoveOnlyTest()
        {
            Gen.Generate();

            Gen.Config.FileGlob = "content/*Microgainz - M*";
            Gen.Config.RemoveOnly = true;

            Gen.Generate();

            AssertCues();
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

            AssertCues();
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
    }
}
