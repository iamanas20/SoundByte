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

using System.Collections.Generic;
using System.Linq;
using SoundByte.Core.Parser.CipherOperations;

namespace SoundByte.Core.Parser
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