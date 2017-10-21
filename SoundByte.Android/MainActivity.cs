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
using System.Net;
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
using Xamarin.Android.Net;

namespace SoundByte.Android
{
    [Activity(Theme = "@style/AppTheme.Base", Label = "@string/app_name")]
    public class MainActivity : Activity
    {
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        TrackAdapter mAdapter;

        List<BaseTrack> SearchTracks = new List<BaseTrack>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            

           

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

           

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
            intentA.SetPackage("SoundByte.Android");
            StartService(intentA);

            var intent = new Intent(PlaybackService.ActionPlay);
            intent.SetPackage("SoundByte.Android");
            intent.PutExtra("URL", "https://api.soundcloud.com/tracks/" + adapter?.mBaseTrack[position].Id + "/stream?client_id=" + AppKeys.BackupSoundCloudPlaybackIDs[2]);
            StartService(intent);

            // Display a toast that briefly shows the enumeration of the selected photo:
            Toast.MakeText(this, "Starting Song: " + adapter.mBaseTrack[position].Title, ToastLength.Short).Show();
        }
    }
}

