/* |----------------------------------------------------------------|
 * | Copyright (c) 2017, Grid Entertainment                         |
 * | All Rights Reserved                                            |
 * |                                                                |
 * | This source code is to only be used for educational            |
 * | purposes. Distribution of SoundByte source code in             |
 * | any form outside this repository is forbidden. If you          |
 * | would like to contribute to the SoundByte source code, you     |
 * | are welcome.                                                   |
 * |----------------------------------------------------------------|
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Microsoft.Toolkit.Uwp.Helpers;

namespace SoundByte.UWP.Models
{
    /// <summary>
    /// Base Class for all models to extend off
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseModel<T> : ObservableCollection<T>, ISupportIncrementalLoading
    {
        public delegate void MoreItemsLoadedEventHandler(IEnumerable<T> newItems);
        public event MoreItemsLoadedEventHandler OnMoreItemsLoaded;


        #region UI Bindings
        /// <summary>
        /// Is this model currently loading new items
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading == value)
                    return;

                _isLoading = value;
                UpdateProperty();
            }
        }
        private bool _isLoading;

        /// <summary>
        /// Does this model have an error
        /// </summary>
        public bool IsError
        {
            get => _isError;
            set
            {
                if (_isError == value)
                    return;

                _isError = value;
                UpdateProperty();
            }
        }
        private bool _isError;

        /// <summary>
        /// If there is an error, this is the title of the error
        /// </summary>
        public string ErrorHeader
        {
            get => _errorHeader;
            set
            {
                if (_errorHeader == value)
                    return;

                _errorHeader = value;
                UpdateProperty();
            }
        }
        private string _errorHeader;

        /// <summary>
        /// If there is an error, this is the description of the error
        /// </summary>
        public string ErrorDescription
        {
            get => _errorDescription;
            set
            {
                if (_errorDescription == value)
                    return;

                _errorDescription = value;
                UpdateProperty();
            }
        }
        private string _errorDescription;
        #endregion

        public string ModelHeader { get; set; }

        public string ModelType { get; set; }

        public string Token { get; set; }

        protected virtual async Task<int> LoadMoreItemsAsync(int count)
        {
            return await Task.Run(() => 0);
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return Task.Run(async () =>
            {
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    IsError = false;
                    IsLoading = true;
                });

                var previousItems = this.ToList();
                var addedCount = await LoadMoreItemsAsync((int) count);
                var allItems = this.ToList();

                var newItems = allItems.Except(previousItems);
                OnMoreItemsLoaded?.Invoke(newItems);

                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    IsLoading = false;
                });

                return new LoadMoreItemsResult {Count = (uint) addedCount};
            }).AsAsyncOperation();
        }

        public bool HasMoreItems => Token != "eol";

        /// <summary>
        ///     Refresh the list by removing any
        ///     existing items and reseting the token.
        /// </summary>
        public void RefreshItems()
        {
            Token = string.Empty;
            Clear();
        }

        protected async Task ShowErrorMessageAsync(string title, string description)
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                IsError = true;
                ErrorHeader = title;
                ErrorDescription = description;
            });   
        }

        private void UpdateProperty([CallerMemberName] string propertyName = "")
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
    }
}
