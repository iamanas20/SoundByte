using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BottomNavigationBar;

namespace SoundByte.Android.Activities
{
    [Activity]
    public class TabbedActivity : Activity, BottomNavigationBar.Listeners.IOnMenuTabClickListener
    {
        private BottomBar _bottomBar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.TabbedActivity);

            _bottomBar = BottomBar.Attach(this, savedInstanceState);

            _bottomBar.SetItems(new []
            {
                new BottomBarTab(Resource.Drawable.tab_home, string.Empty),
                new BottomBarTab(Resource.Drawable.abc_ab_share_pack_mtrl_alpha, string.Empty)
            });

            _bottomBar.SetOnMenuTabClickListener(this);

        }

        public void OnMenuTabSelected(int menuItemId)
        {
        }

        public void OnMenuTabReSelected(int menuItemId)
        {
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            // Necessary to restore the BottomBar's state, otherwise we would
            // lose the current tab on orientation change.
            _bottomBar.OnSaveInstanceState(outState);
        }
    }
}