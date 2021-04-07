using CueGen.Analysis;
using Mono.Options;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CueGen.Console
{
    class Program
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();
        readonly Config Config = new();
        bool Error = false;
        bool Backup = true;
        bool ReportProgress = true;

        static int Main(string[] args)
        {
            try
            {
                var logConfig = new LoggingConfiguration();
                var logConsole = new ColoredConsoleTarget("console")
                {
                    Encoding = Encoding.UTF8,
                    UseDefaultRowHighlightingRules = true,
                    ErrorStream = true,
                    Layout = "${level:format=FullName}: ${message} ${exception:format=toString,Data}"
                };
                System.Console.OutputEncoding = Encoding.UTF8;
                var program = new Program();
                var showHelp = false;
                var colors = "";
                var logPath = "";
                var logLevel = LogLevel.Warn;
                var phraseNames = "";
                var phraseOrder = "";
                var logToConsole = true;

                logConfig.AddRule(logLevel, LogLevel.Fatal, logConsole);

                LogManager.Configuration = logConfig;

                try
                {
                    var options = new OptionSet
                    {
                        { "h|help", "Show this message and exit", v => showHelp = v != null },
                        { "dryrun", "Do not alter Rekordbox database, only perform a test run", v => program.Config.DryRun = v != null },
                        { "hc|hotcues", "Create hot cue points instead of memory cue points", v => program.Config.HotCues = v != null },
                        { "m|merge", "Merge with existing cue points (default is enabled)", v => program.Config.Merge = v != null },
                        { "d|distance=", "Minimum distance in bars to existing cue points (default is 4)", (int v) => program.Config.MinDistanceBars = v },
                        { "colors=", "Comma separated list of hot cue colors, same order as in Rekordbox, top left is 1 (default is 1, 4, 6, 9, 12, 13, 14, 15)", v => colors = v },
                        { "x|max=", "Maximum number of cue points to create (default is 8)", (int v) => program.Config.MaxCues = v },
                        { "comment=", "Comment template, # will be replaced by energy level (default is \"Energy #\")", v => program.Config.Comment = v },
                        { "g|glob=", "File glob of track file paths to include, e.g. C:\\Music\\*.mp3 (default is all in Rekordbox database)", v => program.Config.FileGlob = v },
                        { "r|remove", "Remove all cue points created through this program", v => program.Config.RemoveOnly = v != null },
                        { "b|backup", "Create database backup before creating cue points (default is enabled)", v => program.Backup = v != null },
                        { "s|snap", "Snap cue points to nearest bar (default is enabled)", v => program.Config.SnapToBar = v != null },
                        { "p|phrase", "Create cue points from phrases (default is disabled)", v => program.Config.PhraseCues = v != null },
                        { "my|mytag", "Create MyTag with energy level (default is disabled)", v => program.Config.MyTagEnergy = v != null },
                        { "e|energy", "Set track color according to energy level (default is disabled)", v => program.Config.ColorEnergy = v != null },
                        { "energycolor", "Set cue point color according to cue point's energy level (default is enabled)", v => program.Config.CueColorEnergy = v != null },
                        { "phrasecolor", "Set cue point color according to cue point's phrase (default is enabled)", v => program.Config.CueColorPhrase = v != null },
                        { "progress", "Report progress (default is enabled)", v => program.ReportProgress = v != null },
                        { "db|database=", "File path to Rekordbox database (default is autodetect)", v => program.Config.DatabasePath = v },
                        { "v|verbosity=", "Verbosity level (default is warn, possible values are off, fatal, error, warn, info, debug, trace)", v => logLevel = LogLevel.FromString(v) },
                        { "l|log=", "File path to write log file to", v => logPath = v },
                        { "c|console", "Log to console (default is enabled)", v => logToConsole = v != null },
                        { "n|names=", "Phrase names, comma separated (default are Intro,Verse,Bridge,Chorus,Outro,Up,Down)", v => phraseNames = v },
                        { "o|order=", "Phrase group order, comma separated groups of slash separated phrase names (default is Intro,Outro,Verse,Chorus,Bridge,Up/Down)", v => phraseOrder = v },
                        { "phraselength=", "Minimum length of phrase group in bars (default is 4)", (int v) => program.Config.MinPhraseLength = v },
                        { "mindate=", "Minimum creation date of tracks (default is any, format is 2021-12-32T23:31:00, time is optional)", v => program.Config.MinCreatedDate =
                            DateTime.Parse(v, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeLocal) },
                        { "li|loopintro=", "Length in beats of active loop intro (default is disabled)", (int v) => program.Config.LoopIntroLength = v },
                        { "lo|loopoutro=", "Length in beats of active loop outro (default is disabled)", (int v) => program.Config.LoopOutroLength = v },
                    };

                    options.Parse(args);

                    if (showHelp)
                    {
                        ShowHelp(options);
                        return 0;
                    }

                    if (!logToConsole)
                    {
                        logConfig.LoggingRules.Remove(logConfig.LoggingRules.First());
                    }
                    else if (logLevel != LogLevel.Warn)
                    {
                        logConfig.LoggingRules.First().SetLoggingLevels(logLevel, LogLevel.Fatal);
                    }

                    if (!string.IsNullOrEmpty(logPath))
                    {
                        var logFile = new FileTarget("file")
                        {
                            Encoding = Encoding.UTF8,
                            FileName = logPath,
                            Layout = "${longdate}|${level:uppercase=true}|${message} ${exception:format=toString,Data}"
                        };
                        logConfig.AddRule(logLevel, LogLevel.Fatal, logFile);
                    }

                    LogManager.Configuration = logConfig;

                    if (!string.IsNullOrEmpty(colors))
                        program.Config.Colors = colors.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(c => int.Parse(c.Trim()) - 1).Where(i => i >= 0 && i < 16).ToList();

                    if (!string.IsNullOrEmpty(phraseNames))
                        program.Config.PhraseNames = phraseNames.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Take(Enum.GetValues<PhraseGroup>().Length)
                            .Select((n, i) => (Index: i, Name: n))
                            .ToDictionary(n => (PhraseGroup)(n.Index + 1), n => n.Name);

                    if (!string.IsNullOrEmpty(phraseOrder))
                    {
                        var phraseGroups = Enum.GetNames<PhraseGroup>().ToDictionary(n => n, n => Enum.Parse<PhraseGroup>(n));

                        program.Config.PhraseOrder = phraseOrder.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select((n, i) => (Index: i, Names: n.Split('/', StringSplitOptions.RemoveEmptyEntries)))
                            .SelectMany(e => e.Names.Select(n => (e.Index, Name: n)))
                            .ToDictionary(n => phraseGroups.Single(g => g.Key.StartsWith(n.Name, StringComparison.OrdinalIgnoreCase)).Value, n => n.Index);
                    }

                    static void CheckLoopLength(int l)
                    {
                        if ((l & (l - 1)) != 0 || l < 0 || l > 512)
                            throw new ArgumentException("Loop length must be a power of 2 and less than or equal to 512");
                    }

                    CheckLoopLength(program.Config.LoopIntroLength);
                    CheckLoopLength(program.Config.LoopOutroLength);

                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "Error parsing command line arguments");
                    return 1;
                }

                program.Generate();

                return program.Error ? 1 : 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An error has occurred");
                return 2;
            }
        }

        static void ShowHelp(OptionSet p)
        {
            System.Console.WriteLine("Usage: CueGen.Console [OPTION]...");
            System.Console.WriteLine("Create Rekordbox cue points from MIK cue points.");
            System.Console.WriteLine("Version " + typeof(Generator).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version);
            System.Console.WriteLine("Append - to option to disable it, e.g. --progress- or -b-.");
            System.Console.WriteLine();
            System.Console.WriteLine("Options:");
            p.WriteOptionDescriptions(System.Console.Out);
        }

        void Generate()
        {
            if (string.IsNullOrEmpty(Config.DatabasePath))
            {
                var db = Environment.ExpandEnvironmentVariables(@"%AppData%\Pioneer\rekordbox\master.db");
                if (!File.Exists(db))
                {
                    Log.Error("Rekordbox database not found at {database}", db);
                    throw new FileNotFoundException("Rekordbox database not found", db);
                }
                else
                    Log.Info("Rekordbox database is at {database}", db);

                Config.DatabasePath = db;
            }

            if (Backup)
            {
                var dir = Path.GetDirectoryName(Config.DatabasePath);
                var fn = Path.GetFileNameWithoutExtension(Config.DatabasePath);
                var backupPath = Path.Combine(dir, $"{fn}.backup.{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.db");
                Log.Info("Creating database backup at {path}", backupPath);
                File.Copy(Config.DatabasePath, backupPath);
            }

            var generator = new Generator(Config);
            var progress = new ProgressBar();

            if (Environment.UserInteractive && !System.Console.IsInputRedirected && ReportProgress)
            {
                generator.Progress.ProgressChanged += (s, e) =>
                {
                    var pct = (double)e.Count / e.Total;
                    progress.Report(pct, $"{e.Count + 1}/{e.Total}");
                };
            }

            if (!generator.Generate())
                Error = true;
        }
    }
}
