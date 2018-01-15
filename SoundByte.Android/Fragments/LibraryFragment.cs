using Android.App;
using Android.OS;
using Android.Views;

namespace SoundByte.Android.Fragments
{
    public class LibraryFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate the view
            var view = inflater.Inflate(Resource.Layout.LibraryFragment, container, false);

            return view;
        }

        public static LibraryFragment NewInstance()
        {
            return new LibraryFragment { Arguments = new Bundle() };
        }
    }
}