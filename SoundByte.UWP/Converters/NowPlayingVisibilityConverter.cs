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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace SoundByte.UWP.Converters
{
    /// <summary>
    /// This converter is used to show or hide the now playing
    /// UI depending on if an item is playing or we are on the
    /// track page.
    /// </summary>
    public class NowPlayingVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Check to see if we are on the track page
            var isTrackPage = ((MainShell) Window.Current.Content)?.RootFrame.CurrentSourcePageType == typeof(Views.Track);

            if (value == null || isTrackPage)
                return Visibility.Collapsed;
            
            return Visibility.Visible;
        }

        /// <summary>
        /// We can not convert back in this case
        /// </summary>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
