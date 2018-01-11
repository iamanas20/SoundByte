/* |----------------------------------------------------------------|
 * | Copyright (c) 2017 - 2018 Grid Entertainment                   |
 * | All Rights Reserved                                            |
 * |                                                                |
 * | This source code is to only be used for educational            |
 * | purposes. Distribution of SoundByte source code in             |
 * | any form outside this repository is forbidden. If you          |
 * | would like to contribute to the SoundByte source code, you     |
 * | are welcome.                                                   |
 * |----------------------------------------------------------------|
 */

using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using SoundByte.Android.Fragments;

namespace SoundByte.Android
{
    [Activity(Theme = "@style/AppTheme.Base", Label = "@string/app_name")]
    public class MainActivity : Activity
    {
        private BottomNavigationView _bottomNavigationView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.MainActivity);

            _bottomNavigationView = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);
            _bottomNavigationView.NavigationItemSelected += BottomNavigation_NavigationItemSelected;
        }

        private void BottomNavigation_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            LoadFragment(e.Item.ItemId);
        }

        private void LoadFragment(int id)
        {
            Fragment fragment = null;

            switch (id)
            {
                case Resource.Id.menu_home:
                    fragment = HomeFragment.NewInstance();
                    break;
                case Resource.Id.menu_library:
                    fragment = LibraryFragment.NewInstance();
                    break;
                case Resource.Id.menu_search:
                    fragment = SearchFragment.NewInstance();
                    break;
                case Resource.Id.menu_menu:
                    fragment = MenuFragment.NewInstance();
                    break;
            }

            if (fragment == null)
                return;

            FragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, fragment).Commit();     
        }
    }
}