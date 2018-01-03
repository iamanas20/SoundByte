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

using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using SoundByte.Core;

namespace SoundByte.UWP.Converters
{
    public class ServiceToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var convertedValue = (ServiceType) value;

            switch (convertedValue)
            {
                case ServiceType.Fanburst:
                    return new BitmapImage(new Uri("ms-appx:///Assets/Services/fanburst.png"));
                case ServiceType.SoundCloud:
                case ServiceType.SoundCloudV2:
                    return new BitmapImage(new Uri("ms-appx:///Assets/Services/soundcloud.png"));
                case ServiceType.YouTube:
                    return new BitmapImage(new Uri("ms-appx:///Assets/Services/youtube.png"));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
