using Android.App;
using Android.OS;
using Android.Views;

namespace SoundByte.Android.Fragments
{
    public class MenuFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate the view
            var view = inflater.Inflate(Resource.Layout.MenuFragment, container, false);

            return view;
        }

        public static MenuFragment NewInstance()
        {
            return new MenuFragment { Arguments = new Bundle() };
        }
    }
}