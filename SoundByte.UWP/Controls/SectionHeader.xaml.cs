using System.Collections.Generic;
using Windows.UI.Xaml;

namespace SoundByte.UWP.Controls
{
    /// <summary>
    /// App Section Header
    /// </summary>
    public sealed partial class SectionHeader 
    {
        #region Dependency Properties
        public static readonly DependencyProperty TitleProperty = 
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(SectionHeader), null);
        
        public static readonly DependencyProperty ButtonsProperty = 
            DependencyProperty.Register(nameof(Buttons), typeof(List<AppButton>), typeof(SectionHeader), null);
        #endregion

        /// <summary>
        /// Title of the section
        /// </summary>
        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public List<AppButton> Buttons
        {
            get => (List<AppButton>)GetValue(ButtonsProperty);
            set => SetValue(ButtonsProperty, value);
        }

        public SectionHeader()
        {
            SetValue(ButtonsProperty, new List<AppButton>());

            InitializeComponent();
        }
    }
}