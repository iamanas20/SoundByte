/* |----------------------------------------------------------------|
 * | Copyright (c) 2017 - 2018 Grid Entertainment                   |
 * | All Rights Reserved                                            |
 * |                                                                |
 * | This source code is to only be used for educational            |
 * | purposes. Distribution of SoundByte source code in             |
 * | any form outside this repository is forbidden. If you          |
 * | would like to contribute to the SoundByte source code, you     |
 * | are welcome.                                                   |
 * |----------------------------------------------------------------|
 */

using SoundByte.Core.Items.Podcast;
using SoundByte.Core.Sources;
using SoundByte.UWP.Helpers;

namespace SoundByte.UWP.ViewModels.Generic
{
    public class PodcastShowListViewModel : BaseViewModel
    {
        #region Bindings
        /// <summary>
        /// The title to be displayed on the page.
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
        /// Subtitle to be displayed on the page.
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
        /// The podcast model to show on this page
        /// </summary>
        public SoundByteCollection<ISource<PodcastShow>, PodcastShow> Model
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
        private SoundByteCollection<ISource<PodcastShow>, PodcastShow> _model;
        #endregion

        #region Methods
        /// <summary>
        /// Setup the view model for use
        /// </summary>
        /// <param name="data">The podcast model to use in this view</param>
        public void Init(PodcastShowViewModelHolder data)
        {
            Model = new SoundByteCollection<ISource<PodcastShow>, PodcastShow>(data.PodcastSource);
            Title = data.Title;
            SubTitle = data.Subtitle;
        }
        #endregion

        public class PodcastShowViewModelHolder
        {
            public ISource<PodcastShow> PodcastSource { get; set; }
            public string Title { get; set; }
            public string Subtitle { get; set; }
        }
    }

}
