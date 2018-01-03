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

using SoundByte.UWP.ViewModels.SearchViewModels;

namespace SoundByte.UWP.Views.SearchViews
{
    public sealed partial class TracksView
    {
        public TracksView()
        {
            InitializeComponent();
        }

        public TracksViewModel ViewModel { get; set; }
    }
}