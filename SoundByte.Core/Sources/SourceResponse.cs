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

using System.Collections.Generic;

namespace SoundByte.Core.Sources
{
    public class SourceResponse<TSource>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="token"></param>
        /// <param name="successful">Was the source successful in getting the items. (Does not count exceptions)</param>
        /// <param name="messageTitle">If the source was not successful, this is the title why.</param>
        /// <param name="messageContent"></param>
        public SourceResponse(IEnumerable<TSource> items, string token, bool successful = true, string messageTitle = null, string messageContent = null)
        {
            Items = items;
            Token = token;
            IsSuccess = successful;

            Messages = new Message
            {
                MessageTitle = messageTitle,
                MessageContent = messageContent
            };

        }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public IEnumerable<TSource> Items { get; }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Token { get; }

        public bool IsSuccess { get; }

        /// <summary>
        /// This class contains any error or warning messages
        /// that the model may of generated.
        /// </summary>
        public Message Messages { get; }

        public class Message
        {
            public string MessageTitle { get; set; }

            public string MessageContent { get; set; }
        }
    }
}
