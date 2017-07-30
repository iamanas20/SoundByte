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
using SoundByte.UWP.Helpers;

namespace SoundByte.UWP.Converters
{
    /// <summary>
    /// Converts a nullable into to a human readable string
    /// </summary>
    public class FormattedValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Get our value
            var inValue = value as int?;

            // Does this null int have a value
            if (inValue.HasValue)
            {
                // Return the actual value of 0 if there is none
                return inValue.Value == 0 ? "0" : NumberFormatHelper.GetFormattedLargeNumber(inValue.Value);
            }

            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
