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

using Windows.UI.Xaml.Navigation;
using SoundByte.Core.Items.Track;
using SoundByte.UWP.Models;
using SoundByte.UWP.ViewModels.Search;

namespace SoundByte.UWP.Views.Search
{
    public sealed partial class SearchTrackView 
    {
        public SearchTrackViewModel ViewModel { get; } = new SearchTrackViewModel();

        public SearchTrackView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Init((BaseModel<BaseTrack>)e.Parameter);
        }      
    }
}
