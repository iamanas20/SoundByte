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

using Newtonsoft.Json;

namespace SoundByte.Core.Items.SoundByte
{
    public class PagingHeader
    {
        public PagingHeader(
            int totalItems, int pageNumber, int pageSize, int totalPages)
        {
            TotalItems = totalItems;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = totalPages;
        }

        [JsonProperty("total_items")]
        public int TotalItems { get; }

        [JsonProperty("page_number")]
        public int PageNumber { get; }

        [JsonProperty("page_size")]
        public int PageSize { get; }

        [JsonProperty("total_pages")]
        public int TotalPages { get; }

        public string ToJson() => JsonConvert.SerializeObject(this);
    }
}
