using Newtonsoft.Json;
using System;

namespace SoundByte.Core.Converters
{
    public class YouTubeIdConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Items.Track.YouTubeTrack.YouTubeId);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                return new Items.Track.YouTubeTrack.YouTubeId
                {
                    VideoId = (string)reader.Value,
                    Kind = "youtube#video"
                };
            }

            return serializer.Deserialize<Items.Track.YouTubeTrack.YouTubeId>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
