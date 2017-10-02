using System.Collections.Generic;
using System.Linq;
using SoundByte.YouTubeParser.Internal.CipherOperations;

namespace SoundByte.YouTubeParser.Internal
{
    internal class PlayerSource
    {
        public IReadOnlyList<ICipherOperation> CipherOperations { get; }

        public PlayerSource(IEnumerable<ICipherOperation> cipherOperations)
        {
            CipherOperations = cipherOperations.ToArray();
        }

        public string Decipher(string input)
        {
            foreach (var operation in CipherOperations)
                input = operation.Decipher(input);
            return input;
        }
    }
}