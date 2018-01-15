using Android.App;
using Android.OS;
using Android.Views;

namespace SoundByte.Android.Fragments
{
    public class HomeFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate the view
            var view = inflater.Inflate(Resource.Layout.HomeFragment, container, false);

            return view;
        }

        public static HomeFragment NewInstance()
        {
            return new HomeFragment { Arguments = new Bundle() };
        }
    }
}