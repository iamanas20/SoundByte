﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using SoundByte.Core.Items.Comment;
using SoundByte.Core.Items.User;
using System.Threading;
using Newtonsoft.Json;
using SoundByte.Core.Services;
using YoutubeExplode;
using YoutubeExplode.Models.MediaStreams;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace SoundByte.Core.Items.Track
{
    /// <summary>
    ///     A universal track class that is consistent for
    ///     all service types. All elements are updateable by
    ///     the UI.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [Table("tracks", Schema = "sb_data")]
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class BaseTrack : BaseItem
    {
        /// <summary>
        ///     Get the audio stream for this item
        /// </summary>
        /// <param name="youTubeClient">Used for YouTube videos</param>
        /// <returns></returns>
        public async Task<Uri> GetAudioStreamAsync(YoutubeClient youTubeClient)
        {
            // Get the appropriate client Ids
            var service = SoundByteService.Current.Services.FirstOrDefault(x => x.Service == ServiceType);

            if (service == null)
                throw new Exception("Oh shit, this should like, never be null dude. You should probably direct message me on twitter :D (@dominicjmaas)");

            string audioStream;

            switch (ServiceType)
            {
                case ServiceType.Local:
                case ServiceType.ITunesPodcast:
                    // We already set the audio url
                    audioStream = AudioStreamUrl;
                    break;
                case ServiceType.Fanburst:
                    audioStream = $"https://api.fanburst.com/tracks/{TrackId}/stream?client_id={service.ClientId}";
                    break;
                case ServiceType.SoundCloud:
                case ServiceType.SoundCloudV2:
                    // SoundCloud has a fixed rate on playbacks. This system chooses a key on random and plays from it. These
                    // keys are provided by the web service (so more can be added when needed) so chances of expiring the key should
                    // be rare (especially when users start using YouTube and Fanburst Playback instead).

                    // TODO: THIS CAN FAIL (due to 17.10.x using old system). If this fails, the song wont play.

                    // Create list of keys with our default key
                    var apiKeys = new List<string>();
                    apiKeys.Add(service.ClientId);

                    // Add backup keys
                    apiKeys.AddRange(service.ClientIds);

                    // Get random key
                    var randomNumber = new Random().Next(apiKeys.Count);
                    audioStream = $"https://api.soundcloud.com/tracks/{TrackId}/stream?client_id={apiKeys[randomNumber]}";
                    break;
                case ServiceType.YouTube:
                    // Get the video streams
                    var mediaStreams = await youTubeClient.GetVideoMediaStreamInfosAsync(TrackId);

                    audioStream = IsLive 
                        ? mediaStreams.HlsLiveStreamUrl 
                        : mediaStreams.Audio.OrderByDescending(q => q.Bitrate).Last()?.Url;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new Uri(audioStream);
        }

        /// <summary>
        ///     What service this track belongs to. Useful for
        ///     performing service specific tasks such as liking.
        /// </summary>
        [Column("service_type")]
        [JsonProperty("service_type")]
        public ServiceType ServiceType { get; set; }

        /// <summary>
        ///     The SoundByte resource ID
        /// </summary>
        [Column("id")]
        [JsonProperty("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        ///     Id of track
        /// </summary>
        [Column("track_id")]
        [JsonProperty("track_id")]
        public string TrackId { get; set; }

        /// <summary>
        ///     Link to the track
        /// </summary>
        [Column("link")]
        [JsonProperty("link")]
        public string Link { get; set; }

        /// <summary>
        ///     Is the track currently live (used for YouTube videos)
        /// </summary>
        [Column("is_live")]
        [JsonProperty("is_live")]
        public bool IsLive { get; set; }

        /// <summary>
        ///     Url to the audio stream
        /// </summary>
        [Column("audio_stream_url")]
        [JsonProperty("audio_stream_url")]
        public string AudioStreamUrl { get; set; }

        /// <summary>
        ///     Url to the video stream
        /// </summary>
        [Column("video_stream_url")]
        [JsonProperty("video_stream_url")]
        public string VideoStreamUrl { get; set; }

        /// <summary>
        ///     Artwork link
        /// </summary>
        [Column("artwork_url")]
        [JsonProperty("artwork_url")]
        public string ArtworkUrl { get; set; }

        /// <summary>
        ///     Artwork link for thumbnails (smaller)
        /// </summary>
        [Column("thumbnail_url")]
        [JsonProperty("thumbnail_url")]
        public string ThumbnailUrl { get; set; }

        /// <summary>
        ///     Title of the track
        /// </summary>
        [Column("title")]
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        ///     Description of the track
        /// </summary>
        [Column("description")]
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        ///     How long is the track
        /// </summary>
        [Column("duration")]
        [JsonProperty("duration")]
        public TimeSpan Duration { get; set; }

        /// <summary>
        ///     Date and time the track was created/uploaded
        /// </summary>
        [Column("created")]
        [JsonProperty("created")]
        public DateTime Created { get; set; }

        /// <summary>
        ///     Amount of likes
        /// </summary>
        [Column("like_count")]
        [JsonProperty("like_count")]
        public double LikeCount { get; set; }

        /// <summary>
        ///     Amount of dislikes (if supported - YouTube)
        /// </summary>
        [Column("dislike_count")]
        [JsonProperty("dislike_count")]
        public double DislikeCount { get; set; }

        /// <summary>
        ///     Amount of views
        /// </summary>
        [Column("view_count")]
        [JsonProperty("view_count")]
        public double ViewCount { get; set; }

        /// <summary>
        ///     Amount of comments
        /// </summary>
        [Column("comment_count")]
        [JsonProperty("comment_count")]
        public double CommentCount { get; set; }

        /// <summary>
        ///     Track Genre
        /// </summary>
        [Column("genre")]
        [JsonProperty("genre")]
        public string Genre { get; set; }

        /// <summary>
        ///     Has this track been liked
        /// </summary>
        [Column("is_liked")]
        [JsonProperty("is_liked")]
        public bool IsLiked { get; set; }


        [Column("user_id")]
        [JsonProperty("user_id")]
        public Guid UserId { get; set; }

        /// <summary>
        ///     The Track User
        /// </summary>
        [Column("user")]
        [JsonProperty("user")]
        public BaseUser User { get; set; }

        /// <summary>
        ///     Custom properties you can set
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public Dictionary<string, object> CustomProperties { get; } = new Dictionary<string, object>();

        #region Methods
        public async Task<CommentResponse> GetCommentsAsync(int count, string token, CancellationTokenSource cancellationTokenSource = null)
        {
            // Always at least 10 comments.
            if (count <= 10)
                count = 10;

            switch (ServiceType)
            {
                case ServiceType.SoundCloud:
                case ServiceType.SoundCloudV2:
                    return await new SoundCloudTrack(TrackId).GetCommentsAsync(count, token, cancellationTokenSource);
                case ServiceType.Fanburst:
                    return await new FanburstTrack(TrackId).GetCommentsAsync(count, token, cancellationTokenSource);
                case ServiceType.YouTube:
                    return await new YouTubeTrack(TrackId).GetCommentsAsync(count, token, cancellationTokenSource);
                default:
                    throw new ArgumentOutOfRangeException();
            }   
        }

        public  void ToggleLike()
        {
            if (IsLiked)
            {
                Unlike();
            }
            else
            {
                Like();
            }
        }

        public async void Like()
        {
            // We have already liked the track
            if (IsLiked)
                return;

            var hasLiked = false;

            switch (ServiceType)
            {
                case ServiceType.SoundCloud:
                case ServiceType.SoundCloudV2:
                     hasLiked = await new SoundCloudTrack(TrackId).LikeAsync();
                    break;
                case ServiceType.Fanburst:
                    hasLiked = await new FanburstTrack(TrackId).LikeAsync();
                    break;
                case ServiceType.YouTube:
                    hasLiked = await new YouTubeTrack(TrackId).LikeAsync();
                    break;
                case ServiceType.Local: // Don't support liking
                    return;
                case ServiceType.ITunesPodcast: // Use SoundByte like
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // SoundByte Like
            try
            {
                if (SoundByteService.Current.IsSoundByteAccountConnected)
                {
                    await SoundByteService.Current.PostItemAsync(ServiceType.SoundByte, "likes", this);
                }
            }
            catch (Exception e)
            {
                var i = 0;
            }


            IsLiked = hasLiked;
        }

        public async void Unlike()
        {
            // We have already unliked the track
            if (!IsLiked)
                return;

            bool hasUnliked;

            switch (ServiceType)
            {
                case ServiceType.SoundCloud:
                case ServiceType.SoundCloudV2:
                    hasUnliked = await new SoundCloudTrack(TrackId).UnlikeAsync();
                    break;
                case ServiceType.Fanburst:
                    hasUnliked = await new FanburstTrack(TrackId).UnlikeAsync();
                    break;
                case ServiceType.YouTube:
                    hasUnliked = await new YouTubeTrack(TrackId).UnlikeAsync();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            IsLiked = !hasUnliked;
        }
        #endregion

        public class CommentResponse
        {
            public List<BaseComment> Comments { get; set; }
            public string Token { get; set; }
        }
    }
}
