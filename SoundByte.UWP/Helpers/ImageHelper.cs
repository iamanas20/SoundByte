using System;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.Graphics.Canvas;

namespace SoundByte.UWP.Helpers
{
    /// <summary>
    ///     This class contains helper methods for managing and
    ///     working with images.
    /// </summary>
    public static class ImageHelper
    {
        public static async Task<Uri> CreateCachedImageAsync(string internetUri, string saveName)
        {
            // Check that there is an image to save
            if (string.IsNullOrEmpty(internetUri)) return null;

            try
            {
                // Create a canvas device
                using (var device = new CanvasDevice())
                {
                    // Create items used to same image to file
                    var bitmap = await CanvasBitmap.LoadAsync(device, new Uri(internetUri));

                    // Create a renderer
                    var renderer = new CanvasRenderTarget(device, bitmap.SizeInPixels.Width, bitmap.SizeInPixels.Height,
                        bitmap.Dpi);

                    // Draw the image
                    using (var ds = renderer.CreateDrawingSession())
                    {
                        ds.DrawImage(bitmap);
                    }

                    // Open the cache folder
                    var cacheFolder =
                        await ApplicationData.Current.LocalFolder.CreateFolderAsync("cache",
                            CreationCollisionOption.OpenIfExists);

                    // Create a file
                    var imageFile = await cacheFolder.CreateFileAsync(string.Format("{0}.jpg", saveName),
                        CreationCollisionOption.OpenIfExists);

                    // Write the image to the file
                    using (var stream = await imageFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await renderer.SaveAsync(stream, CanvasBitmapFileFormat.Png);
                        return new Uri(string.Format("ms-appdata:///local/cache/{0}.jpg", saveName), UriKind.Absolute);
                    }
                }
            }
            catch (Exception ex)
            {
                App.Telemetry.TrackException(ex, false);
                return null;
            }
        }
    }
}