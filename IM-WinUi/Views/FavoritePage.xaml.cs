using IMWinUi.Models;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace IMWinUi.Views
{
    internal sealed partial class FavoritePage : Page
    {

        internal ObservableCollection<FavoriteItem> Items { get; set; }
        internal FavoritePage()
        {

        this.InitializeComponent();

            Items = new ObservableCollection<FavoriteItem>
            {
                new FavoriteItem
                {
                    Text = "Item 1",
                    ImageUrls = new List<string>
                    {
                        "https://example.com/image1.jpg",
                        "https://example.com/image2.jpg",
                        "https://example.com/image3.jpg"
                    }
                },
                new FavoriteItem
                {
                    Text = "Item 2",
                    ImageUrls = new List<string>
                    {
                        "https://example.com/image4.jpg",
                        "https://example.com/image5.jpg",
                        "https://example.com/image6.jpg",
                        "https://example.com/image7.jpg"
                    }
                }
            };
        }


    }
}
