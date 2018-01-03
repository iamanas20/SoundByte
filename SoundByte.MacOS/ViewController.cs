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
using AppKit;
using System.Linq;
using SoundByte.Core.Sources.SoundCloud;
using SoundByte.Core.Sources.SoundCloud.Search;

namespace SoundByte.MacOS
{
    public partial class ViewController : NSViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        System.Threading.CancellationTokenSource searchToken = new System.Threading.CancellationTokenSource();

        async partial void SearchBox(NSSearchField sender)
        {
            // Cancel pending search
            searchToken.Cancel();

            // Get search results
            var searchResults = await new SoundCloudSearchTrackSource
            {
                SearchQuery = sender.StringValue
            }.GetItemsAsync(10, null, searchToken);

            System.Diagnostics.Debug.WriteLine(searchResults.Items.FirstOrDefault()?.Title);
        }
    }
}
