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
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using SoundByte.Android.Adapters;
using SoundByte.Core.Items.Track;
using SoundByte.Android.Services;
using SoundByte.Core.Sources.SoundCloud;

namespace SoundByte.Android.Fragments
{
    public class SearchFragment : Fragment
    {
        private RecyclerView _recyclerView;
        private RecyclerView.LayoutManager _layoutManager;
        private TrackAdapter _adapter;

        private readonly List<BaseTrack> _searchTracks = new List<BaseTrack>();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate the view
            var view = inflater.Inflate(Resource.Layout.SearchFragment, container, false);

            // Get our RecyclerView layout
            _recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);

            // Use the built-in linear layout manager:
            _layoutManager = new LinearLayoutManager(view.Context);

            // Plug the layout manager into the RecyclerView:
            _recyclerView.SetLayoutManager(_layoutManager);

            // Setup the adapter
            _adapter = new TrackAdapter(_searchTracks);
            _adapter.ItemClick += OnItemClick;

            // Plug the adapter into the RecyclerView:
            _recyclerView.SetAdapter(_adapter);

            // Search button logic
            var searchButton = view.FindViewById<Button>(Resource.Id.randPickButton);
            searchButton.Click += async delegate
            {
                _searchTracks.Clear();

                var searchResults = await new SearchSoundCloudTrackSource
                {
                    SearchQuery = "monstercat"
                }.GetItemsAsync(40, null); 

                foreach (var result in searchResults.Items)
                {
                    _searchTracks.Add(result);
                }

                // Update the RecyclerView by notifying the adapter:
                // Notify that the top and a randomly-chosen photo has changed (swapped):
                _adapter.NotifyDataSetChanged();
                _adapter.NotifyItemChanged(0);
            };

            return view;
        }

        private void OnItemClick(object sender, int position)
        {
            var adapter = (TrackAdapter)sender;

            if (adapter == null)
                return;

            var intentA = new Intent(PlaybackService.ActionStop);
            intentA.SetPackage("SoundByte.Android");
            Activity.StartService(intentA);

            var intent = new Intent(PlaybackService.ActionPlay);
            intent.SetPackage("SoundByte.Android");
            intent.PutExtra("URL", "https://api.soundcloud.com/tracks/" + adapter.BaseTrack[position].Id + "/stream?client_id=" + AppKeys.BackupSoundCloudPlaybackIDs[2]);
            Activity.StartService(intent);

            // Display a toast that briefly shows the enumeration of the selected photo:
            Toast.MakeText(Activity, "Starting Song: " + adapter.BaseTrack[position].Title, ToastLength.Short).Show();
        }

        public static SearchFragment NewInstance()
        {
            return new SearchFragment { Arguments = new Bundle() };
        }
    }
}