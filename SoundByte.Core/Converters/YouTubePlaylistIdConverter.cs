/* |----------------------------------------------------------------|
 * | Copyright (c) 2017, Grid Entertainment                         |
 * | All Rights Reserved                                            |
 * |                                                                |
 * | This source code is to only be used for educational            |
 * | purposes. Distribution of SoundByte source code in             |
 * | any form outside this repository is forbidden. If you          |
 * | would like to contribute to the SoundByte source code, you     |
 * | are welcome.                                                   |
 * |----------------------------------------------------------------|
 */

using System;
using Newtonsoft.Json;

namespace SoundByte.Core.Converters
{
    public class YouTubePlaylistIdConverter : JsonConverter
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
                    PlaylistId = (string)reader.Value,
                    Kind = "youtube#playlist"
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
