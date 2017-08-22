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
using SoundByte.Core.Helpers;

namespace SoundByte.Core.Converters
{
    /// <summary>
    ///     Converts a nullable into to a human readable string
    /// </summary>
    public class FormattedValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                // Get our value
                var inValue = value as int?;

                // Does this null int have a value
                if (inValue.HasValue)
                    return inValue.Value == 0 ? "0" : NumberFormatHelper.GetFormattedLargeNumber(inValue.Value);
            }
            catch (Exception)
            {
                return "0";
            }   

            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}