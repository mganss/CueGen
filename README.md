# CueGen

Create Rekordbox cue points from Mixed in Key cue points or Rekordbox phrases.

## Features

- Create memory cues or hot cues
- Merge with existing cue points or overwrite
- Configure minimum distance in bars to existing cue points
- Configure maximum number of cue points to create
- Configure colors
- Assign colors based on energy level, phrase, or cue number
- Configure cue point names (comments)
- Set cue points only for specific tracks (based on name or creation date)
- All generated cue points can be removed
- Creates a database backup before each run (optional)
- Snap cue points to beat grid
- Add a new Rekordbox MyTag "Energy" with the track's energy level determined by Mixed in Key
- Set track color according to Energy level
- Optionally set intro and/or outro cue points to active loop

## Requirements

- Rekordbox 6
- Windows
- Mixed in Key (phrase cue points work without Mixed in Key)

## Basics

CueGen operates directly on the Rekordbox database, there is no need to import/export tracks to/from XML.

Rekordbox needs to be shut down when CueGen runs. All tracks should already have been analyzed by Rekordbox to allow CueGen to snap cue points to the beginning of the bar.

In Mixed in Key you should have Serato integration checked on the settings page and have exported cue points (Click "Export cue points for existing files"). 
Serato integration writes cue points information to ID3 tags within the media files allowing CueGen to read it.

## Database Backup

CueGen automatically creates a backup of the Rekordbox SQLite database `master.db` in the Rekordbox folder `%AppData%\Pioneer\rekordbox`.
The backup files will have a datetime suffix, e.g. `master.backup.2021-04-04-13-40-05.db`.
If you want to restore one of these backups, replace the `master.db` file with a backup file.

## Mixed in Key cue points

Mixed in Key detects up to 8 cue points for each track. Each of the cue points gets assigned an energy level (in addition to the overall track energy level).
CueGen uses the energy level information to assign a cue point name (such as "Energy 6") and color.

## Phrase cue points

Cue points can also be created based on phrase information as analyzed by Rekordbox. For example, you can automatically set cue points where the outro starts and
whenever a chorus starts.

Rekordbox assigns phrases according to a "mood" it detects. The mood can be low, mid, or high. For each mood the following phrases exist:

- Low, Mid: Intro, Verse 1-6, Bridge, Chorus, Outro
- High: Intro 1-2, Up 1-3, Down, Chorus 1-2, Outro 1-2

CueGen creates cue points whenever a new phrase group starts. A phrase group is a number of consecutive phrases that start with the same but may have different numbers
(such as "Verse 1, Verse 2, Verse 3"). Additionally, you can combine phrase groups: For example, if you combine Down and Outro into a phrase group a cue point will be
created at the start of a sequence of phrases labeled Down or Outro.

## Colors

There are different types of colors in Rekordbox for memory cues and hot cues. 
Memory cues can have one of 8 eight different colors: Pink, Red, Orange, Yellow, Green, Aqua, Blue, and Purple.

Hot cues can technically have any color from 64 color palette, of which only 16 are surfaced in Rekordbox.

Mixed in Key cue points are by default assigned a color according to their energy level. For memory cue points purple is Energy 1 and pink is Energy 8.
Hot cues get similar colors picked from the 16 colors palette.

Phrase cue points by default get a color that is similar to the phrase group in Rekordbox (intro is red, chorus is green, etc).

Through the `--colors` you can choose the palette of 8 colors that are used to map energy level, phrase, or cue point number.
