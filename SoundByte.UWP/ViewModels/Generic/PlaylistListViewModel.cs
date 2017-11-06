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

using Windows.UI.Xaml.Controls;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Sources;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Views;

namespace SoundByte.UWP.ViewModels.Generic
{
    public class PlaylistListViewModel : BaseViewModel
    {
        #region Bindings
        /// <summary>
        /// The title to be displayed on the search page.
        /// </summary>
        public string Title
        {
            get => _title;
            set
            {
                if (value == _title)
                    return;

                _title = value;
                UpdateProperty();
            }
        }
        private string _title;

        /// <summary>
        /// Subtitle to be displayed on the search page.
        /// </summary>
        public string SubTitle
        {
            get => _subTitle;
            set
            {
                if (value == _subTitle)
                    return;

                _subTitle = value;
                UpdateProperty();
            }
        }
        private string _subTitle;


        /// <summary>
        /// The track model to show on this page
        /// </summary>
        public SoundByteCollection<ISource<BasePlaylist>, BasePlaylist> Model
        {
            get => _model;
            set
            {
                if (value == _model)
                    return;

                _model = value;
                UpdateProperty();
            }
        }
        private SoundByteCollection<ISource<BasePlaylist>, BasePlaylist> _model;
        #endregion

        #region Methods
        /// <summary>
        /// Setup the view model for use
        /// </summary>
        /// <param name="data">The user model to use in this view</param>
        public void Init(PlaylistViewModelHolder data)
        {
            Model = new SoundByteCollection<ISource<BasePlaylist>, BasePlaylist>(data.Playlist);
            Title = data.Title;
            SubTitle = data.Subtitle;
        }
        #endregion

        public void NavigatePlaylist(object sender, ItemClickEventArgs e)
        {
            App.NavigateTo(typeof(PlaylistView), (BasePlaylist)e.ClickedItem);
        }

        public class PlaylistViewModelHolder
        {
            public ISource<BasePlaylist> Playlist { get; set; }
            public string Title { get; set; }
            public string Subtitle { get; set; }
        }
    }
}
