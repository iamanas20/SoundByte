namespace SoundByte.YouTubeParser.Internal.CipherOperations
{
    internal interface ICipherOperation
    {
        /// <summary>
        /// Deciphers the given string
        /// </summary>
        string Decipher(string input);
    }
}