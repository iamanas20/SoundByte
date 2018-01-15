using System;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using SoundByte.Android.Helpers;
using SoundByte.Android.ViewHolders;
using SoundByte.Core.Items.Track;

namespace SoundByte.Android.Adapters
{
    public class TrackAdapter : RecyclerView.Adapter
    {
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;

        public List<BaseTrack> BaseTrack;

        public TrackAdapter(List<BaseTrack> baseTrack)
        {
            BaseTrack = baseTrack;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo:
            var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.TrackItemView, parent, false);

            // Create a ViewHolder to find and hold these view references, and 
            // register OnClick with the view holder:
            return new TrackViewHolder(itemView, OnClick);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = (TrackViewHolder)holder;

            // Set the ImageView and TextView in this ViewHolder's CardView 
            // from this position in the photo album:

            var imageBitmap = HttpHelpers.GetImageBitmapFromUrl(BaseTrack[position].ArtworkUrl);
            vh.Image.SetImageBitmap(imageBitmap);
            vh.Caption.Text = BaseTrack[position].Title;
        }

        public override int ItemCount => BaseTrack.Count;

        void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }
    }
}