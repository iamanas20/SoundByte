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
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using SoundByte.Core.Helpers;
using SoundByte.UWP.Dialogs;

namespace SoundByte.UWP.Services
{
    /// <summary>
    ///     Handles Live Tiles
    /// </summary>
    public class TileService
    {
        private TileService()
        {
            // Setup the tile updaters
            TileUpdater = TileUpdateManager.CreateTileUpdaterForApplication("App");
            TileUpdater.EnableNotificationQueue(true);

            // Gets all the tiles that are for the app
            var allTiles = AsyncHelper.RunSync(async () => await SecondaryTile.FindAllAsync());
            // Clear the list
            _mPTileList.Clear();
            // Loop  through all the tiles and add them to the list
            foreach (var tile in allTiles)
                _mPTileList.Add(tile.TileId, tile);
        }

        private static readonly Lazy<TileService> InstanceHolder =
            new Lazy<TileService>(() => new TileService());

        /// <summary>
        ///     Gets the current instance
        /// </summary>
        public static TileService Instance => InstanceHolder.Value;

        #region Variables
        // Stores all the tiles that are currently pinned to the users screen
        private readonly Dictionary<string, SecondaryTile> _mPTileList = new Dictionary<string, SecondaryTile>();

        /// <summary>
        /// 
        /// </summary>
        public TileUpdater TileUpdater { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Removes all live tiles from the start menu
        ///     or start screen
        /// </summary>
        public async Task RemoveAllAsync()
        {
            // Clear the tile list
            _mPTileList.Clear();
            // Find all the tiles and loop though them all
            foreach (var tile in await SecondaryTile.FindAllAsync())
                // Request a tile delete
                await tile.RequestDeleteAsync();
        }

        /// <summary>
        ///     Removes a tile from the users start screen and deletes it from the list
        /// </summary>
        /// <param name="id">Tile ID</param>
        /// <returns>True is successful</returns>
        public async Task<bool> RemoveAsync(string id)
        {
            // Check if tile exists and get it
            if (!_mPTileList.TryGetValue(id, out SecondaryTile tile)) return true;

            try
            {
                // Request tile deletion
                var success = await tile.RequestDeleteAsync();
                // Remove the tile from the list
                _mPTileList.Remove(id);
                // Return weather the task was successful or not
                return success;
            }
            catch
            {
                // There was some error, tile not removed
                return false;
            }
        }

        /// <summary>
        ///     Returns if the tile exists
        /// </summary>
        /// <param name="id">Tile ID</param>
        /// <returns>True if the tile exists</returns>
        public bool DoesTileExist(string id)
        {
            // Check if tileid is in list
            return _mPTileList.ContainsKey(id);
        }

        /// <summary>
        ///     Creates a tile and pins it to the users screen
        /// </summary>
        /// <param name="tileId">The ID for the tile</param>
        /// <param name="tileTitle">The title that will appear on the tile</param>
        /// <param name="tileParam">Any params that will be passed to the app on launch</param>
        /// <param name="tileImage">Uri to image for the background</param>
        /// <param name="tileForeground">Text to display on tile</param>
        /// <returns></returns>
        public async Task<bool> CreateTileAsync(string tileId, string tileTitle, string tileParam, Uri tileImage,
            ForegroundText tileForeground)
        {
            // Check if the tile already exists
            if (DoesTileExist(tileId))
                return false;

            await new PinTileDialog(tileId, tileTitle, tileParam, tileImage).ShowAsync();
            return true;
        }

        #endregion
    }
}