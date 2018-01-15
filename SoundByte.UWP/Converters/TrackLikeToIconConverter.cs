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
