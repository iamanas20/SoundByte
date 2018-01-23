using System;
using System.Collections.Generic;
using System.Linq;
using SoundByte.Core.Items.Comment;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Items.Podcast;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using SoundByte.Core.Sources;
using SoundByte.Core.Sources.Fanburst;
using SoundByte.Core.Sources.Fanburst.Search;
using SoundByte.Core.Sources.Fanburst.User;
using SoundByte.Core.Sources.Podcast;
using SoundByte.Core.Sources.SoundByte;
using SoundByte.Core.Sources.SoundCloud;
using SoundByte.Core.Sources.SoundCloud.Search;
using SoundByte.Core.Sources.SoundCloud.User;
using SoundByte.Core.Sources.YouTube;
using SoundByte.Core.Sources.YouTube.Search;

namespace SoundByte.Core.Managers
{
    /// <summary>
    ///     This class is used for registering sources that are used within
    ///     the app. This is used to help keep track of sources and the data
    ///     they contain in more complex scenarios (e.g Windows Timeline)
    /// </summary>
    public class SourceManager
    {
        private Dictionary<string, Type> _sources = new Dictionary<string, Type>();

        public void RegisterDefaultSources()
        {
            RegisterSource<PodcastItemsSource, BaseTrack>();
            RegisterSource<PodcastSearchSource, BasePodcast>();
            RegisterSource<SoundByteHistorySource, BaseTrack>();
            RegisterSource<SoundByteLikeSource, BaseTrack>();
            RegisterSource<SoundBytePlaylistSource, BasePlaylist>();
            RegisterSource<CommentSource, BaseComment>();
            RegisterSource<DummyTrackSource, BaseTrack>();
            RegisterSource<FanburstLikeSource, BaseTrack>();
            RegisterSource<FanburstPlaylistSource, BasePlaylist>();
            RegisterSource<FanburstPopularTracksSource, BaseTrack>();
            RegisterSource<FanburstSearchTrackSource, BaseTrack>();
            RegisterSource<FanburstSearchUserSource, BaseUser>();
            RegisterSource<FanburstUserFollowersSource, BaseUser>();
            RegisterSource<FanburstUserFollowingsSource, BaseUser>();
            RegisterSource<FanburstUserPlaylistSource, BasePlaylist>();
            RegisterSource<FanburstUserTrackSource, BaseTrack>();
            RegisterSource<SoundBytePodcastsSource, BasePodcast>();
            RegisterSource<SoundCloudSearchPlaylistSource, BasePlaylist>();
            RegisterSource<SoundCloudSearchTrackSource, BaseTrack>();
            RegisterSource<SoundCloudSearchUserSource, BaseUser>();
            RegisterSource<SoundCloudExploreSource, BaseTrack>();
            RegisterSource<SoundCloudLikeSource, BaseTrack>();
            RegisterSource<SoundCloudStreamSource, GroupedItem>();
            RegisterSource<SoundCloudUserFollowersSource, BaseUser>();
            RegisterSource<SoundCloudUserFollowingsSource, BaseUser>();
            RegisterSource<SoundCloudUserPlaylistSource, BasePlaylist>();
            RegisterSource<SoundCloudUserTrackSource, BaseTrack>();
            RegisterSource<ExploreYouTubeTrendingSource, BaseTrack>();
            RegisterSource<YouTubeSearchPlaylistSource, BasePlaylist>();
            RegisterSource<YouTubeSearchTrackSource, BaseTrack>();
            RegisterSource<YouTubeSearchUserSource, BaseUser>();
            RegisterSource<YouTubeLikeSource, BaseTrack>();
            RegisterSource<YouTubePlaylistSource, BasePlaylist>();
        }

        public void RegisterSource<T, TItem>() where T : ISource<TItem>
        {
            _sources.Add(typeof(T).Name, typeof(T));
        }

        public ISource<BaseTrack> GetTrackSource(string name, Dictionary<string, object> data = null)
        {
            var item = _sources.FirstOrDefault(x => x.Key == name);

            if (item.Value == null)
                throw new Exception("Source not found");

            var instance = Activator.CreateInstance(item.Value);

            if (data != null)
            {
                ((ISource<BaseTrack>)instance).ApplyParameters(data);
            }

            return (ISource<BaseTrack>)instance;
        }
    }
}