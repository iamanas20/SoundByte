using SoundByte.UWP.ViewModels.SearchViewModels;

namespace SoundByte.UWP.Views.Search
{
    public sealed partial class UsersView 
    {
        public UsersView()
        {
            InitializeComponent();
        }

        public UsersViewModel ViewModel { get; set; }
    }
}
