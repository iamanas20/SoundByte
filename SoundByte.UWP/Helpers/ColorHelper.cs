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

using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.UI;
using ColorThiefDotNet;
using Microsoft.Toolkit.Uwp.UI;

namespace SoundByte.UWP.Helpers
{
    public static class ColorHelper
    {
        public static async Task<Windows.UI.Color> GetDominantHue(Uri imageLocation)
        {
            var tcs = new TaskCompletionSource<Windows.UI.Color>();

            // ensure we've precached the image in Storage, not memory. Not sure what
            // will happen if the image is already there from a binding, which it should be.
            await ImageCache.Instance.PreCacheAsync(imageLocation);

            // see if the file is in the cache
            var imgFile = await ImageCache.Instance.GetFileFromCacheAsync(imageLocation);

            if (imgFile != null)
            {
                var decoder = await Task.Run(async () =>
                {
                    var stream = await imgFile.OpenStreamForReadAsync();
                    return await BitmapDecoder.CreateAsync(stream.AsRandomAccessStream());
                });

                var colorThief = new ColorThief();

                var color = await Task.Run(async () => await colorThief.GetColor(decoder, 5));

                tcs.SetResult(new Windows.UI.Color
                {
                    R = color.Color.R,
                    G = color.Color.G,
                    B = color.Color.B,
                    A = 255
                });
            }
            else
            {
                tcs.SetResult(Colors.Black);
            }

            return tcs.Task.Result;

        }
    }
}
