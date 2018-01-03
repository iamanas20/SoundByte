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
using System.Globalization;
using Windows.UI.Xaml.Data;
using SoundByte.UWP.Helpers;

namespace SoundByte.UWP.Converters
{
    /// <summary>
    ///     This class takes in a DateTime object and converts it into
    ///     a human readable form.
    /// </summary>
    public class DateTimeConverter : IValueConverter
    {
        /// <summary>
        ///     This function takes in a datetime string and converts it
        ///     into a friendly readable string for the UI.
        /// </summary>
        /// <returns>A human readable date time object</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return "Unknown";

            try
            {
                // Convert to a datetime
                var inputDate = DateTime.Parse(value.ToString());

                // Return the formatted DateTime 
                return NumberFormatHelper.GetTimeDateString(inputDate, true);
            }
            catch (Exception)
            {
                // There was an error either parsing the value or converting the
                // date time. We will show a generic unknown message here.
                return "Unknown";
            }
        }

        /// <summary>
        ///     This function is not needed and should not be used.
        ///     It returns the current date time just in case it is
        ///     called.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DateTime.Now.ToString(CultureInfo.InvariantCulture);
        }
    }
}