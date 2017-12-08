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
using Windows.UI.Xaml.Data;

namespace SoundByte.UWP.Converters
{
    /// <summary>
    /// If a track is liked show the unliked icon.
    /// </summary>
    public class TrackLikeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var nullIsLiked = value as bool?;

            if (nullIsLiked.HasValue)
            {
                if (nullIsLiked.Value)
                {
                    return "\uEB52";
                }
            }

            return "\uEB51";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
