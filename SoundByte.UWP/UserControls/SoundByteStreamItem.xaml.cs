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
using Windows.UI.Popups;
using Windows.UI.Xaml;
using SoundByte.Core.API.Endpoints;
using SoundByte.UWP.Dialogs;

namespace SoundByte.UWP.UserControls
{
    /// <summary>
    /// Base item for users, tracks, reposted tracks, playlists, reposted playlists and groups
    /// </summary>
    public sealed partial class SoundByteStreamItem
    {
        #region Variables
        // What type of track this is
        public static readonly DependencyProperty TrackTypeProperty = DependencyProperty.Register("TrackType", typeof(string), typeof(SoundByteStreamItem), null);
        // When this was created
        public static readonly DependencyProperty CreatedProperty = DependencyProperty.Register("Created", typeof(string), typeof(SoundByteStreamItem), null);
        // The track object
        public static readonly DependencyProperty TrackProperty = DependencyProperty.Register("Track", typeof(Track), typeof(SoundByteStreamItem), null);
        // The playlist object
        public static readonly DependencyProperty PlaylistProperty = DependencyProperty.Register("Playlist", typeof(Playlist), typeof(SoundByteStreamItem), null);
        #endregion

        #region Getters and Setters
        /// <summary>
        /// What is the current type of track (e.g repost, playlist, etc.)
        /// </summary>
        public string TrackType
        {
            get { return GetValue(TrackTypeProperty) as string; }
            set { SetValue(TrackTypeProperty, value); }
        }

        /// <summary>
        /// When the track was created
        /// </summary>
        public string Created
        {
            get { return GetValue(CreatedProperty) as string; }
            set { SetValue(CreatedProperty, value); }
        }

        /// <summary>
        /// The track object 
        /// </summary>
        public Track Track
        {
            get { return GetValue(TrackProperty) as Track; }
            set
            {
                SetValue(TrackProperty, value);
            }
        }

        /// <summary>
        /// The track object 
        /// </summary>
        public Playlist Playlist
        {
            get { return GetValue(PlaylistProperty) as Playlist; }
            set { SetValue(PlaylistProperty, value); }
        }
        #endregion

        public async void ShareTrack()
        {
            await new ShareDialog(Track).ShowAsync();
        }

        public async void SharePlaylist()
        {
            await new MessageDialog("Not yet supported.").ShowAsync();
        }

        public async void AddTrackToPlaylist()
        {
            await new PlaylistDialog(Track).ShowAsync();
        }

        public SoundByteStreamItem()
        {
            // Load the xaml
            InitializeComponent();

            // Setup the even that is called when the data
            // context chanages.
            DataContextChanged += delegate
            {
                // Switch through all the items
                switch (TrackType)
                {
                    case "track-repost":
                    case "track":
                        VisualStateManager.GoToState(this, "TrackItem", false);
                        break;
                    case "playlist-repost":
                    case "playlist":
                        VisualStateManager.GoToState(this, "PlaylistItem", false);
                        break;
                }
            };
        }
    }
}

