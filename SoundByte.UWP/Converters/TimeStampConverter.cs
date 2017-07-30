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
    /// This class takes in a timespac and converts it into a human
    /// readable string.
    /// </summary>
    public class TimeStampConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                var timeSpan = new TimeSpan();

                if (value is TimeSpan)
                    timeSpan = (TimeSpan) value;

                if (value is int)
                    timeSpan = TimeSpan.FromMilliseconds((int) value);

                if (value is string)
                    timeSpan = TimeSpan.FromMilliseconds(int.Parse(value.ToString()));

                var returnValue = timeSpan.TotalHours < 1.0 ? string.Format("{0}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds) : string.Format("{0}:{1:D2}:{2:D2}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
                
                // Return the formatted value
                return returnValue;
            }
            catch (Exception)
            {
                // There was an error either parsing the value or converting the
                // time stamp. We will show a generic unknown message here.
                return "Unknown";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
