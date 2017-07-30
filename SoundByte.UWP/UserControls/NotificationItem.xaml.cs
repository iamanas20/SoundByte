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

using Windows.UI.Xaml;

namespace SoundByte.UWP.UserControls
{
    public sealed partial class NotificationItem
    {
        #region Variables
        private static readonly DependencyProperty TrackDataProperty = DependencyProperty.Register("TrackData", typeof(Core.API.Endpoints.Track), typeof(NotificationItem), null);
        private static readonly DependencyProperty PlaylistDataProperty = DependencyProperty.Register("PlaylistData", typeof(Core.API.Endpoints.Playlist), typeof(NotificationItem), null);
        private static readonly DependencyProperty UserDataProperty = DependencyProperty.Register("UserData", typeof(Core.API.Endpoints.User), typeof(NotificationItem), null);
        private static readonly DependencyProperty CommentProperty = DependencyProperty.Register("CommentData", typeof(Core.API.Endpoints.Comment), typeof(NotificationItem), null);
        private static readonly DependencyProperty CreationProperty = DependencyProperty.Register("Creation", typeof(string), typeof(NotificationItem), null);
        private static readonly DependencyProperty CollectionTypeProperty = DependencyProperty.Register("CollectionType", typeof(string), typeof(NotificationItem), null);
        #endregion

        #region Page Setup
        /// <summary>
        /// Called when this item is created
        /// </summary>
        public NotificationItem()
        {
            // Load the xaml
            InitializeComponent();

            // Setup the even that is called when the data
            // context chanages.
            DataContextChanged += delegate
            {
                // Switch through all the items
                switch (CollectionType)
                {
                    case "track-like":
                        RootContentPane.ContentTemplate = Resources["TrackLikeView"] as DataTemplate;
                        break;
                    case "playlist-like":
                        RootContentPane.ContentTemplate = Resources["PlaylistLikeView"] as DataTemplate;
                        break;
                    case "comment":
                        RootContentPane.ContentTemplate = Resources["TrackCommentView"] as DataTemplate;
                        break;
                    case "affiliation":
                        RootContentPane.ContentTemplate = Resources["UserFollowView"] as DataTemplate;
                        break;
                    default:
                        RootContentPane.ContentTemplate = Resources["UnknownView"] as DataTemplate;
                        break;
                }
            };
        }
        #endregion

        #region Getters and Setters
        /// <summary>
        /// The track object
        /// </summary>
        public Core.API.Endpoints.Track TrackData
        {
            get => GetValue(TrackDataProperty) as Core.API.Endpoints.Track;
            set => SetValue(TrackDataProperty, value);
        }

        /// <summary>
        /// The playlist object
        /// </summary>
        public Core.API.Endpoints.Playlist PlaylistData
        {
            get => GetValue(PlaylistDataProperty) as Core.API.Endpoints.Playlist;
            set => SetValue(PlaylistDataProperty, value);
        }

        /// <summary>
        /// The user object
        /// </summary>
        public Core.API.Endpoints.User UserData
        {
            get => GetValue(UserDataProperty) as Core.API.Endpoints.User;
            set => SetValue(UserDataProperty, value);
        }

        /// <summary>
        /// The comment object
        /// </summary>
        public Core.API.Endpoints.Comment CommentData
        {
            get => GetValue(CommentProperty) as Core.API.Endpoints.Comment;
            set => SetValue(CommentProperty, value);
        }

        /// <summary>
        /// When this item was created
        /// </summary>
        public string Creation
        {
            get => GetValue(CreationProperty) as string;
            set => SetValue(CreationProperty, value);
        }

        /// <summary>
        /// What kind of object is this
        /// </summary>
        public string CollectionType
        {
            get => GetValue(CollectionTypeProperty) as string;
            set => SetValue(CollectionTypeProperty, value);
        }
        #endregion
    }
}
