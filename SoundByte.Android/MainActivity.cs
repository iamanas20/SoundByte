using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;

namespace SoundByte.Android
{
    [Activity(Label = "SoundByte.Android", MainLauncher = true)]
    public class MainActivity : Activity
    {
        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;

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

        }

        // Handler for the item click event:
        void OnItemClick(object sender, int position)
        {
            // Display a toast that briefly shows the enumeration of the selected photo:
            int photoNum = position + 1;
            Toast.MakeText(this, "This is photo number " + photoNum, ToastLength.Short).Show();
        }
    }
}

