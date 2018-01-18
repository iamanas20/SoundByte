using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
        #region Private Variables
        // Used to stop the UI from updating
        private bool _suppressNotification;
        #endregion

        #region Getters and Setters
        /// <summary>
        ///     Source object telling the collection how to get the next
        ///     set of items.
        /// </summary>
        public TSource Source { get; }

        /// <summary>
        ///     Current token for the next request. Will be eol if
        ///     no more items are avaliable.
        /// </summary>
        public string Token { get; set; }
        
        /// <summary>
        ///     Are there any more items in the collection
        /// </summary>
        public bool HasMoreItems => Token != "eol";
        #endregion

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

        #region Constructors
        public SoundByteCollection() : this(Activator.CreateInstance<TSource>())
        { }

        public SoundByteCollection(TSource source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            Source = source;
        }
        #endregion

        #region Load More Items Method
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return Task.Run(async () =>
            {
                // Lower limit of 10 items
                if (count <= 10)
                    count = 10;

                // Upper limit of 50 items
                if (count >= 50)
                    count = 50;

                // The ammoung of new items added
                var addedCount = 0;

                // Start index (used for collection update method)
                var startIndex = Count;

                try
                {
                    // Show the loading bar
                    await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                    {
                        IsError = false;
                        IsLoading = true;
                    });

                    // Try get some more items
                    var response = await Source.GetItemsAsync((int)count, Token);

                    // If not successful, show error
                    // and return.
                    if (!response.IsSuccess)
                    {
                        await ShowErrorMessageAsync(response.Messages?.MessageTitle, response.Messages?.MessageContent);

                        Token = "eol";
                        return new LoadMoreItemsResult { Count = 0 };
                    }

                    // Set the new token
                    Token = string.IsNullOrEmpty(response.Token) ? "eol" : response.Token;

                    // Set the ammount of items added
                    addedCount = response.Items.Count();

                    // Run on UI thread
                    await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                    {
                        // Don't perform UI updating
                        _suppressNotification = true;

                        // Add items to list
                        foreach (var item in response.Items)
                        {
                            Add(item);
                        }

                        // Start performing UI updating
                        _suppressNotification = false;

                        // Update the Collection
                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<TType>(response.Items), startIndex));
                        OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                    });
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
        #endregion

        #region Methods
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
        #endregion

        #region Internal Methods
        private void UpdateProperty([CallerMemberName] string propertyName = "")
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressNotification)
                base.OnCollectionChanged(e);
        }
        #endregion
    }
}
