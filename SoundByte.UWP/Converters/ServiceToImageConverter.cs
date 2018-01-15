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
