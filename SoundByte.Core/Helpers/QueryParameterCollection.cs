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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;

namespace SoundByte.Core.Helpers
{
#pragma warning disable CS1584 // XML comment has syntactically incorrect cref attribute
#pragma warning disable CS1658 // Warning is overriding an error
    /// <summary>
    ///     Provides an enumerable way to look at query string parameters from a Uri
    /// </summary>
    /// <seealso cref="System.Collections.ObjectModel.Collection{System.Collections.Generic.KeyValuePair{string, string}}" />
    public class QueryParameterCollection : Collection<KeyValuePair<string, string>>
#pragma warning restore CS1658 // Warning is overriding an error
#pragma warning restore CS1584 // XML comment has syntactically incorrect cref attribute
    {
        private static IList<KeyValuePair<string, string>> CreatePairsFromUri([NotNull]string uri)
        {
            // ReSharper disable once ConstantConditionalAccessQualifier
            var queryStartPosition = uri?.IndexOf('?');
            if (queryStartPosition.GetValueOrDefault(-1) != -1)
            { // Uri has a query string
                var queryString = uri.Substring(queryStartPosition.Value + 1);
                return queryString.Split('&')
                    .Select(param =>
                    {
                        var kvp = param.Split('=');
                        return new KeyValuePair<string, string>(kvp[0], kvp[1]);
                    }).ToList();
            }
            else
            {
                return new List<KeyValuePair<string, string>>();
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="QueryParameterCollection"/> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        public QueryParameterCollection(Uri uri)
            : this(uri?.OriginalString)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="QueryParameterCollection"/> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        public QueryParameterCollection(string uri)
            : base(CreatePairsFromUri(uri))
        {
        }
    }
}