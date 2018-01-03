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

        /// <summary>
        /// Buttons to display on section
        /// </summary>
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