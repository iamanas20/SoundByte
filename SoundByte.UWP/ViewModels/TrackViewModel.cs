//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Media;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SoundByte.UWP.Models;
using SoundByte.Core.API.Endpoints;
using SoundByte.UWP.Converters;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;
using Windows.Media.Audio;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Media.Render;
using Windows.Storage;

namespace SoundByte.UWP.ViewModels
{
    public class TrackViewModel : BaseViewModel
    {
        #region Private Variables
        // The previous time that the waveform moved
        private double _previousTime;
        // The comment text box
        private string _commentText;
        // Timer to update the front page UI
        private DispatcherTimer _pageTimer;
        // Used to block the timer when interacting with the slider value
        private bool _blockPageTimer;
        // The pin button text
        private string _pinButtonText = "Pin";
        // The like button text
        private string _likeButtonText = "Like";
        // The repost button text
        private string _repostButtonText = "Repost";
        // The repeat button style
        private Windows.UI.Text.FontWeight _repeatButtonWeight = new Windows.UI.Text.FontWeight { Weight = 400};
        // The shuffle button style
        private Windows.UI.Text.FontWeight _shuffleButtonWeight = new Windows.UI.Text.FontWeight { Weight = 400 };
        #endregion

        #region Models
        // Model for the comments
        public CommentModel CommentItems { get; } = new CommentModel(PlaybackService.Current.CurrentTrack);
        #endregion

        #region Getters and Setters

        /// <summary>
        /// The repeat button weight
        /// </summary>
        public Windows.UI.Text.FontWeight RepeatButtonWeight
        {
            get => _repeatButtonWeight;
            set
            {
                _repeatButtonWeight = value;
                UpdateProperty();
            }
        }

        /// <summary>
        /// The shuffle button weight
        /// </summary>
        public Windows.UI.Text.FontWeight ShuffleButtonWeight
        {
            get => _shuffleButtonWeight;
            set
            {
                _shuffleButtonWeight = value;
                UpdateProperty();
            }
        }

       

        /// <summary>
        /// The text on the pin button
        /// </summary>
        public string PinButtonText
        {
            get => _pinButtonText;
            set
            {
                if (_pinButtonText == value)
                    return;

                _pinButtonText = value;
                UpdateProperty();
            }
        }

        /// <summary>
        /// The text on the like button
        /// </summary>
        public string LikeButtonText
        {
            get => _likeButtonText;
            set
            {
                if (_likeButtonText == value)
                    return;

                _likeButtonText = value;
                UpdateProperty();
            }
        }

        /// <summary>
        /// The text on the repost button
        /// </summary>
        public string RepostButtonText
        {
            get => _repostButtonText;
            set
            {
                if (_repostButtonText == value)
                    return;

                _repostButtonText = value;
                UpdateProperty();
            }
        }

        /// <summary>
        /// The comment text for the comment box
        /// </summary>
        public string CommentText
        {
            get { return _commentText; }

            set
            {
                if (_commentText == value)
                    return;

                _commentText = value;
                UpdateProperty();
            }
        }
        #endregion

        #region Enter and Leave ViewModel Handlers
        /// <summary>
        /// Setup the view model
        /// </summary>
        public void SetupModel()
        {
            // Bind the event handlers   
            // Timer Setup
            _pageTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };

            // Setup the tick event
            _pageTimer.Tick += PlayingSliderUpdate;

            // If the timer is ready, start it
            if (!_pageTimer.IsEnabled)
                _pageTimer.Start();

            // Bind the method once we know a playback list exists
            if (PlaybackService.Current.PlaybackList != null)
                PlaybackService.Current.PlaybackList.CurrentItemChanged += CurrentItemChanged;

            //await InitWaveForm();
        }

        public void MakeFullScreen()
        {
            if (!App.IsFullScreen)
                ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            else
                ApplicationView.GetForCurrentView().ExitFullScreenMode();
        }

        /// <summary>
        /// Clean the view model
        /// </summary>
        public void CleanModel()
        {
            // Stop the timer
            _pageTimer.Stop();

            // Unbind the events
            if(PlaybackService.Current.PlaybackList != null)
                PlaybackService.Current.PlaybackList.CurrentItemChanged -= CurrentItemChanged;

            _pageTimer.Tick -= PlayingSliderUpdate;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Called when the current playing item changes
        /// </summary>
        private async void CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                // Only perform the following actions if there is a new track
                if (args.NewItem == null)
                    return;

                if (Service.CurrentTrack == null)
                    return;

                // Set the pin button text
                PinButtonText = TileService.Current.DoesTileExist("Track_" + Service.CurrentTrack.Id) ? "Unpin" : "Pin";

                // Set the like button text
                LikeButtonText = await SoundByteService.Current.ExistsAsync("/me/favorites/" + Service.CurrentTrack.Id) ? "Unlike" : "Like";

                // Set the repost button text
                RepostButtonText = await SoundByteService.Current.ExistsAsync("/e1/me/track_reposts/" + Service.CurrentTrack.Id) ? "Unpost" : "Repost";

                // Reload all the comments
                CommentItems.RefreshItems();

                // Create a image for the jumplist
                var tempImage = await ImageHelper.CreateCachedImageAsync(ArtworkConverter.ConvertObjectToImage(Service.CurrentTrack), "Jumplist_" + Service.CurrentTrack.Id);
                // Add the track to the jumplist
                if (tempImage != null)
                    await JumplistHelper.AddRecentAsync("soundbyte://core/track?id=" + Service.CurrentTrack.Id, Service.CurrentTrack.Title, "Play " + Service.CurrentTrack.Title + " by " + Service.CurrentTrack.User.Username + ".", "Recent Plays", tempImage);
            });
        }

        /// <summary>
        /// Timer method that is run to make sure the UI is kept up to date
        /// </summary>
        private async void PlayingSliderUpdate(object sender, object e)
        {
            // If the update is blocked, do not run this time
            if (_blockPageTimer)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                return;
            }   

            // Only call the following if the player exists, is playing
            // and the time is greater then 0.
            if (PlaybackService.Current.Player == null || 
                PlaybackService.Current.Player.PlaybackSession.PlaybackState != MediaPlaybackState.Playing || 
                PlaybackService.Current.Player.PlaybackSession.Position.Milliseconds <= 0)
                return;
        }
        #endregion

        #region Method Bindings

        /// <summary>
        /// Opens the share track UI if it exists
        /// </summary>
        public  async void ShareTrack() => await new Dialogs.ShareDialog(Service.CurrentTrack).ShowAsync();

       

        /// <summary>
        /// Display the playlist picker if it exists
        /// </summary>
        public async void DisplayPlaylist() => await new Dialogs.PlaylistDialog(Service.CurrentTrack).ShowAsync();

        /// <summary>
        /// Toggle if we should shuffle the playlist
        /// </summary>
        public void ToggleShuffle()
        {
            // Switch the shuffle functionality
            PlaybackService.Current.PlaybackList.ShuffleEnabled = !PlaybackService.Current.PlaybackList.ShuffleEnabled;
            // If the track should shuffle
            ShuffleButtonWeight = PlaybackService.Current.PlaybackList.ShuffleEnabled ? new Windows.UI.Text.FontWeight { Weight = 600 } : new Windows.UI.Text.FontWeight { Weight = 400 };
            // Track Event
            TelemetryService.Current.TrackEvent("Toggle Shuffle");
        }

        /// <summary>
        /// Toggle if we should repeat the current track
        /// </summary>
        public void ToggleRepeat()
        {
            // Switch the repeat functionality
            PlaybackService.Current.Player.IsLoopingEnabled = !PlaybackService.Current.Player.IsLoopingEnabled;
            // If the track should repeat
            RepeatButtonWeight = PlaybackService.Current.Player.IsLoopingEnabled ? new Windows.UI.Text.FontWeight { Weight = 600 } : new Windows.UI.Text.FontWeight { Weight = 400 };
            // Track Event
            TelemetryService.Current.TrackEvent("Toggle Repeat");
        }

        /// <summary>
        /// Navigate to the selected track in the playlist
        /// </summary>
        public async void GotoRelatedTrack(object sender, ItemClickEventArgs e)
        {
            var startPlayback = await PlaybackService.Current.StartMediaPlayback(PlaybackService.Current.Playlist.ToList(), PlaybackService.Current.TokenValue, false, (Track)e.ClickedItem);
            if (!startPlayback.success)
                await new MessageDialog(startPlayback.message, "Error playing related track.").ShowAsync();
        } 

        /// <summary>
        /// Pin the tile to the start menu
        /// </summary>
        public async void PinTile()
        {
            // Check if the tile exists
            var tileExists = TileService.Current.DoesTileExist("Track_" + Service.CurrentTrack.Id);

            if (tileExists)
            {
                // Remove the tile and check if it was successful
                if (await TileService.Current.RemoveAsync("Track_" + Service.CurrentTrack.Id))
                {
                    PinButtonText = "Pin";
                    // Track Event
                    TelemetryService.Current.TrackEvent("Unpin Track");
                }
                else
                {
                    PinButtonText = "Unpin";
                }
            }
            else
            {
                // Create a live tile and check if it was created
                if (await TileService.Current.CreateTileAsync("Track_" + Service.CurrentTrack.Id, Service.CurrentTrack.Title, "soundbyte://core/track?id=" + Service.CurrentTrack.Id, new Uri(ArtworkConverter.ConvertObjectToImage(Service.CurrentTrack)), ForegroundText.Light))
                {
                    PinButtonText = "Unpin";
                    // Track Event
                    TelemetryService.Current.TrackEvent("Pin Track");
                }
                else
                {
                    PinButtonText = "Pin";
                }
            }
        }

        /// <summary>
        /// Repost the current track to the users stream
        /// </summary>
        public async void RepostTrack()
        {
            if (Service.CurrentTrack == null)
                return;

            // Check to see what the existing string is
            if (RepostButtonText == "Unpost")
            {
                // Delete the repost value and check if it was successful
                if (await SoundByteService.Current.DeleteAsync("/e1/me/track_reposts/" + Service.CurrentTrack.Id))
                {
                    RepostButtonText = "Repost";
                    // Track Event
                    TelemetryService.Current.TrackEvent("Unpost Track");
                }
                else
                {
                    RepostButtonText = "Unpost";
                }
            }
            else
            {
                // Put a value in the reposted tracks and see if successful
                if (await SoundByteService.Current.PutAsync("/e1/me/track_reposts/" + Service.CurrentTrack.Id))
                {
                    RepostButtonText = "Unpost";
                    // Track Event
                    TelemetryService.Current.TrackEvent("Repost Track");
                }
                else
                {
                    RepostButtonText = "Repost";
                }
            }
        }

        /// <summary>
        /// Like the current track
        /// </summary>
        public async void LikeTrack()
        { 
            if (Service.CurrentTrack == null)
                return;

            // Check to see what the existing string is
            if (LikeButtonText == "Unlike")
            {
                // Delete the like from the users likes and see if successful
                if (await SoundByteService.Current.DeleteAsync("/e1/me/track_likes/" + Service.CurrentTrack.Id))
                {
                    LikeButtonText = "Like";
                    // Track Event
                    TelemetryService.Current.TrackEvent("Unlike Track");
                }
                else
                {
                    LikeButtonText = "Unlike";
                }
            }
            else
            {
                // Add a like to the users likes and see if successful
                if (await SoundByteService.Current.PutAsync("/e1/me/track_likes/" + Service.CurrentTrack.Id))
                {
                    LikeButtonText = "Unlike";
                    // Track Event
                    TelemetryService.Current.TrackEvent("Like Track");
                }
                else
                {
                    LikeButtonText = "Like";
                }
            }
        }

        /// <summary>
        /// Called when the user adjusts the playing slider
        /// </summary>
        public async void PlayingSliderChange()
        {
            // Block timer updates
            _blockPageTimer = true;

            // Set the track position
            Service.PlayingSliderChange();
            // Delay the unblock
            await Task.Delay(TimeSpan.FromSeconds(1));

            // Unblock timer update
            _blockPageTimer = false;
        }

        /// <summary>
        /// Called when the user taps on a comment.
        /// This method changes the current position of the track
        /// to the comment time.
        /// </summary>
        public void GoToCommentItemPosition(object sender, ItemClickEventArgs e)
        {
            // Get the comment object
            var comment = e.ClickedItem as Comment;
            // Set the current position
            Service.Player.PlaybackSession.Position = TimeSpan.FromMilliseconds(int.Parse(comment?.Timestamp));
        }

        /// <summary>
        /// Navigates the user to the current track users profile
        /// </summary>
        public async void GoToUserProfile()
        {
            // Show the loading ring as loading the user can take
            // some time.
            App.IsLoading = true;

            // We only support viewing soundcloud profiles at this time
            if (Service.CurrentTrack.ServiceType != ServiceType.SoundCloud)
            {
                await new MessageDialog("SoundByte does not currently supporting user profiles that are not from SoundCloud.", "Not Ready Yet").ShowAsync();
                return;
            }

            // Get the user object
            var currentUser = await SoundByteService.Current.GetAsync<User>("/users/" + Service.CurrentTrack.User.Id);

            // Hide the loading ring
            App.IsLoading = false;

            // Navigate to the user page
            App.NavigateTo(typeof(Views.UserView), currentUser);
        }

        /// <summary>
        /// Post a comment
        /// </summary>
        public async void PostComment()
        {
            // Only perform this action if the user has typed something
            if (string.IsNullOrEmpty(CommentText))
                return;

            // Create a new comment object
            var comment = new Comment
            {
                Body = CommentText,
                Timestamp = PlaybackService.Current.Player.PlaybackSession.Position.TotalMilliseconds.ToString(CultureInfo.InvariantCulture),
                User = SoundByteService.Current.CurrentUser
            };

            // Clear the comment box
            CommentText = string.Empty;


            // Create an http client to post the comment
            using (var client = new HttpClient())
            {
                // Create the json for the comment
                var json = "{\"comment\": {\"body\":\"" + comment.Body + "\", \"timestamp\": " + comment.Timestamp + "}}";

                // Request the soundcloud API
                var commentResponse =  await client.PostAsync("https://api.soundcloud.com/tracks/" + Service.CurrentTrack.Id + "/comments.json?oauth_token=" + SoundByteService.Current.SoundCloudToken.AccessToken + "&client_secret=" + Common.ServiceKeys.SoundCloudClientSecret + "&client_id" + Common.ServiceKeys.SoundCloudClientId, new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json"));

                // If we did not post the comment, return
                if (!commentResponse.IsSuccessStatusCode) return;

                // Track the event
                TelemetryService.Current.TrackEvent("Post Comment");

                // Insert into the list
                CommentItems.Insert(0, comment);
            }
        }

        #endregion

        #region Waveform Logic

        public static byte[] ReadStream(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Setup the waveform for use
        /// </summary>
        /// <returns></returns>
        public async Task InitWaveForm()
        {
            //// First we must save the current audio file

            //// Open the cache folder
            //var cacheFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("cache", CreationCollisionOption.OpenIfExists);

            //// Create a file
            //var imageFile = await cacheFolder.CreateFileAsync(string.Format("{0}-TEMP.mp3", Service.CurrentTrack.Id), CreationCollisionOption.OpenIfExists);

            //try
            //{
            //    var request = (HttpWebRequest)WebRequest.Create(Service.PlaybackList.CurrentItem.Source.Uri);
            //    var response = await request.GetResponseAsync();
            //    var stream = response.GetResponseStream();
            //    await FileIO.WriteBytesAsync(imageFile, ReadStream(stream));

            //    var settings = new AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Media);
            //    var graph = await AudioGraph.CreateAsync(settings);

            //    var t = await graph.Graph.CreateFileInputNodeAsync(imageFile);

            //    var i = t;
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}

            try
            {

                // Initialize the led strip
                //await this.pixelStrip.Begin();

                var devices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(Windows.Media.Devices.MediaDevice.GetAudioRenderSelector());

                // Create graph
                AudioGraphSettings settings = new AudioGraphSettings(AudioRenderCategory.Media);
                settings.DesiredRenderDeviceAudioProcessing = Windows.Media.AudioProcessing.Default;
                settings.QuantumSizeSelectionMode = QuantumSizeSelectionMode.ClosestToDesired;

                CreateAudioGraphResult result = await AudioGraph.CreateAsync(settings);
                if (result.Status != AudioGraphCreationStatus.Success)
                {
                    // Cannot create graph
                    return;
                }
                graph = result.Graph;

                // Create a device input node using the default audio input device
                var deviceInputNodeResult = await graph.CreateDeviceInputNodeAsync(MediaCategory.Other, graph.EncodingProperties, devices[0]);

                if (deviceInputNodeResult.Status != AudioDeviceNodeCreationStatus.Success)
                {
                    return;
                }

                var deviceInputNode = deviceInputNodeResult.DeviceInputNode;

                frameOutputNode = graph.CreateFrameOutputNode();
                deviceInputNode.AddOutgoingConnection(frameOutputNode);
                frameOutputNode.Start();
                graph.QuantumProcessed += AudioGraph_QuantumProcessed;

                // Because we are using lowest latency setting, we need to handle device disconnection errors

                graph.Start();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }







            //    t.DeviceInputNode.ad

            //    var test = await Service.PlaybackList.CurrentItem.Source.





        }

        private AudioGraph graph;

        [ComImport]
        [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        unsafe interface IMemoryBufferByteAccess
        {
            void GetBuffer(out byte* buffer, out uint capacity);
        }


        private AudioFrameOutputNode frameOutputNode;
        private void AudioGraph_QuantumProcessed(AudioGraph sender, object args)
        {
            var frame = frameOutputNode.GetFrame();

            ProcessFrameOutput(frame);
        }

        private unsafe void ProcessFrameOutput(AudioFrame frame)
        {
            using (AudioBuffer buffer = frame.LockBuffer(AudioBufferAccessMode.Write))
            using (IMemoryBufferReference reference = buffer.CreateReference())
            {
                byte* dataInBytes;
                uint capacityInBytes;
                float* dataInFloat;

                // Get the buffer from the AudioFrame
                ((IMemoryBufferByteAccess) reference).GetBuffer(out dataInBytes, out capacityInBytes);


                dataInFloat = (float*) dataInBytes;
                float[] floats = new float[graph.SamplesPerQuantum];

                for (int i = 0; i < graph.SamplesPerQuantum; i++)
                    floats[i] = dataInFloat[i];

                System.Diagnostics.Debug.WriteLine(floats.ToList().Max());
            }
        }


        #endregion


        #region Dispose Handlers

        public override void Dispose()
        {
            // Only clean if we are in the background
            if (!App.IsBackground)
                return;

            // Start Cleanup!
            System.Diagnostics.Debug.WriteLine("We are cleaning this up!");

            CleanModel();

            CommentItems.RefreshItems();

            GC.Collect();
        }
        #endregion
    }
}
