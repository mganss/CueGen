using BinarySerialization;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TagLib.Id3v2;

namespace CueGen
{
    public class TagFile: MIKBase
    {
        public string FilePath { get; set; }
        public CuePointsAttachment CuePoints { get; set; }
        public KeyAttachment Key { get; set; }
        public EnergyAttachment Energy { get; set; }
        public SeratoMarkers SeratoMarkers { get; set; }

        private static string DecodeJson(AttachmentFrame attachment) => 
            Encoding.ASCII.GetString(Convert.FromBase64String(Encoding.ASCII.GetString(attachment.Data.Data)));

        public TagFile(string filePath)
        {
            FilePath = filePath;

            using var tfile = TagLib.File.Create(filePath);
            var attachments = tfile.Tag.Pictures.Where(p => p.MimeType == "application/json")
                .OfType<AttachmentFrame>().ToList();

            foreach (var attachment in attachments)
            {
                switch (attachment.Description)
                {
                    case "CuePoints":
                        CuePoints = JsonConvert.DeserializeObject<CuePointsAttachment>(DecodeJson(attachment));
                        break;
                    case "Key":
                        Key = JsonConvert.DeserializeObject<KeyAttachment>(DecodeJson(attachment));
                        break;
                    case "Energy":
                        Energy = JsonConvert.DeserializeObject<EnergyAttachment>(DecodeJson(attachment));
                        break;
                }
            }

            var seratoMarkers2Attachment = tfile.Tag.Pictures.Where(p => p.Description == "Serato Markers2")
                .OfType<AttachmentFrame>().FirstOrDefault();

            if (seratoMarkers2Attachment != null)
            {
                var bytes = Convert.FromBase64String(Encoding.ASCII.GetString(seratoMarkers2Attachment.Data.Data[2..]));
                var serializer = new BinarySerializer { Endianness = Endianness.Big };
                SeratoMarkers = serializer.Deserialize<SeratoMarkers>(bytes);
            }
        }
    }
}
