using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;
using SoundByte.Android.Adapters;
using SoundByte.Android.Services;
using SoundByte.Core;
using SoundByte.Core.Items;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;

namespace SoundByte.Android
{
    [Activity(Label = "SoundByte.Android", MainLauncher = true)]
    public class MainActivity : Activity
    {
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        TrackAdapter mAdapter;

        List<BaseTrack> SearchTracks = new List<BaseTrack>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Load the SoundByte V3 API
            var secretList = new List<ServiceSecret>
            {
                new ServiceSecret
                {
                    Service = ServiceType.SoundCloud,
                    ClientId = AppKeys.SoundCloudClientId,
                    ClientSecret = AppKeys.SoundCloudClientSecret,
                },
                new ServiceSecret
                {
                    Service = ServiceType.SoundCloudV2,
                    ClientId = AppKeys.SoundCloudClientId,
                    ClientSecret = AppKeys.SoundCloudClientSecret,
                },
                new ServiceSecret
                {
                    Service = ServiceType.Fanburst,
                    ClientId = AppKeys.FanburstClientId,
                    ClientSecret = AppKeys.FanburstClientSecret,
                },
                new ServiceSecret
                {
                    Service = ServiceType.YouTube,
                    ClientId = AppKeys.YouTubeClientId,
                },
                new ServiceSecret
                {
                    Service = ServiceType.ITunesPodcast
                }
            };

           

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            SoundByteV3Service.Current.Init(secretList);

            // Get our RecyclerView layout:
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);

            //............................................................
            // Layout Manager Setup:

            // Use the built-in linear layout manager:
            mLayoutManager = new LinearLayoutManager(this);

            // Or use the built-in grid layout manager (two horizontal rows):
            // mLayoutManager = new GridLayoutManager
            //        (this, 2, GridLayoutManager.Horizontal, false);

            // Plug the layout manager into the RecyclerView:
            mRecyclerView.SetLayoutManager(mLayoutManager);

            mAdapter = new TrackAdapter(SearchTracks);
            mAdapter.ItemClick += OnItemClick;

            // Plug the adapter into the RecyclerView:
            mRecyclerView.SetAdapter(mAdapter);

            Button randomPickBtn = FindViewById<Button>(Resource.Id.randPickButton);
            // Handler for the Random Pick Button:
            randomPickBtn.Click += async delegate
            {
                SearchTracks.Clear();

                var searchResults = await SoundCloudTrack.SearchAsync("Monstercat", 40, null);

                foreach (var result in searchResults.Tracks)
                {
                    SearchTracks.Add(result);
                }

                // Update the RecyclerView by notifying the adapter:
                // Notify that the top and a randomly-chosen photo has changed (swapped):
                mAdapter.NotifyDataSetChanged();
                mAdapter.NotifyItemChanged(0);
            };

        }

        // Handler for the item click event:
        void OnItemClick(object sender, int position)
        {
            var adapter = sender as TrackAdapter;

            var intentA = new Intent(PlaybackService.ActionStop);
            intentA.SetPackage("SoundByte.Android.SoundByte.Android");
            StartService(intentA);

            var intent = new Intent(PlaybackService.ActionPlay);
            intent.SetPackage("SoundByte.Android.SoundByte.Android");
            intent.PutExtra("URL", "https://api.soundcloud.com/tracks/" + adapter?.mBaseTrack[position].Id + "/stream?client_id=" + AppKeys.BackupSoundCloudPlaybackIDs[2]);
            StartService(intent);

            // Display a toast that briefly shows the enumeration of the selected photo:
            Toast.MakeText(this, "Starting Song: " + adapter.mBaseTrack[position].Title, ToastLength.Short).Show();
        }
    }
}

