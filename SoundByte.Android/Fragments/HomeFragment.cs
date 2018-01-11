﻿/* |----------------------------------------------------------------|
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