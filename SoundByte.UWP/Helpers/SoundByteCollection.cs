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
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core.Sources;
using System.ComponentModel;
using SoundByte.Core.Exceptions;

namespace SoundByte.UWP.Helpers
{
    public class SoundByteCollection<TSource, TType> : ObservableCollection<TType>, ISupportIncrementalLoading where TSource : ISource<TType>
    {
        public TSource Source { get; }

        public SoundByteCollection() : this(Activator.CreateInstance<TSource>())
        { }

        public SoundByteCollection(TSource source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            Source = source;
        }


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

        public string Token { get; set; }


        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return Task.Run(async () =>
            {
                if (count <= 10)
                    count = 10;

                if (count >= 50)
                    count = 50;

                var addedCount = 0;

                try
                {
                    await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                    {
                        IsError = false;
                        IsLoading = true;
                    });

                    var response = await Source.GetItemsAsync((int) count, Token);

                    //
                    if (!response.IsSuccess)
                        throw new SoundByteException(response.Messages?.MessageTitle,
                            response.Messages?.MessageContent);

                    // Set the token
                    Token = string.IsNullOrEmpty(response.Token) ? "eol" : response.Token;


                    addedCount = response.Items.Count();
                    foreach (var item in response.Items)
                    {
                        await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                        {
                            Add(item);
                        });
                    }
                }
                catch (SoundByteException ex)
                {
                    Token = "eol";

                    await ShowErrorMessageAsync(ex.ErrorTitle, ex.ErrorDescription);

                }
                finally
                {
                    await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                    {
                        IsLoading = false;
                    });
                }

                return new LoadMoreItemsResult { Count = (uint)addedCount };

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

            LoadMoreItemsAsync(0);
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
