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

namespace SoundByte.Core.Helpers
{
    /// <summary>
    ///     This class handles text related helper functions
    /// </summary>
    public static class TextHelper
    {
        /// <summary>
        ///     Cleans a string ready to be used in a
        ///     XML document.
        /// </summary>
        /// <param name="input">The string to clean</param>
        /// <returns>The cleaned string</returns>
        public static string CleanXmlString(string input)
        {
            // Clean and return the string
            return input.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }
    }
}