using System;

namespace SoundByte.YouTubeParser.Exceptions
{
    /// <summary>
    /// Thrown when there was an error parsing a response from Youtube's frontend
    /// </summary>
    public class ParseException : Exception
    {
        /// <inheritdoc />
        public ParseException(string message)
            : base(message)
        {
        }
    }
}