using IMWinUi.Models;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.DependencyInjection;


namespace IMWinUi.Views
{
    internal sealed partial class FavoritePage : Page
    {

        internal ObservableCollection<FavoriteItem> Items { get; set; }
        internal FavoritePage()
        {

            InitializeComponent();
            // Items = Ioc.Default.GetRequiredService<LocalDbcontext>().GetFavoriteItems();
            // Items = new ObservableCollection<FavoriteItem>
            // {
            //     new FavoriteItem
            //     {
            //         Text = "今天去看了超级漂亮的四姑娘山,还拍了好多好多照片",
            //         ImageUrls = new List<string>
            //         {
            //             "https://ts1.tc.mm.bing.net/th?id=OIP-C.eWeIyPYcX0e-wBISrt3k8QHaFj&w=200&h=200&c=9&rs=1&qlt=99&o=6&dpr=1.5&pid=23.1",
            //             "https://ts1.tc.mm.bing.net/th?id=OIP-C.9adQYcOPVsDX1AlfIbzviQHaHa&w=80&h=80&c=1&vt=10&bgcl=7d40c9&r=0&o=6&dpr=1.5&pid=ImgRC",
            //             "https://ts1.tc.mm.bing.net/th?id=OIP-C.PXNV2FwHccewrIj7s27xOgHaFj&w=120&h=104&c=7&bgcl=ea3751&r=0&o=6&dpr=1.5&pid=13.1",
            //             "https://ts1.tc.mm.bing.net/th?id=OIP-C.eWeIyPYcX0e-wBISrt3k8QHaFj&w=200&h=200&c=9&rs=1&qlt=99&o=6&dpr=1.5&pid=23.1",
            //             "https://ts1.tc.mm.bing.net/th?id=OIP-C.9adQYcOPVsDX1AlfIbzviQHaHa&w=80&h=80&c=1&vt=10&bgcl=7d40c9&r=0&o=6&dpr=1.5&pid=ImgRC",
            //             "https://ts1.tc.mm.bing.net/th?id=OIP-C.PXNV2FwHccewrIj7s27xOgHaFj&w=120&h=104&c=7&bgcl=ea3751&r=0&o=6&dpr=1.5&pid=13.1",
            //             "https://ts1.tc.mm.bing.net/th?id=OIP-C.eWeIyPYcX0e-wBISrt3k8QHaFj&w=200&h=200&c=9&rs=1&qlt=99&o=6&dpr=1.5&pid=23.1",
            //             "https://ts1.tc.mm.bing.net/th?id=OIP-C.9adQYcOPVsDX1AlfIbzviQHaHa&w=80&h=80&c=1&vt=10&bgcl=7d40c9&r=0&o=6&dpr=1.5&pid=ImgRC",
            //             "https://ts1.tc.mm.bing.net/th?id=OIP-C.PXNV2FwHccewrIj7s27xOgHaFj&w=120&h=104&c=7&bgcl=ea3751&r=0&o=6&dpr=1.5&pid=13.1"
            //         }
            //     },
            //     new FavoriteItem
            //     {
            //         Text = "Item 2",
            //         ImageUrls = new List<string>
            //         {
            //             "https://tse1-mm.cn.bing.net/th/id/OIP-C.mPFUeZ48RUerLChi3NeaTgHaE5?w=297&h=196&c=7&r=0&o=5&dpr=1.5&pid=1.7",
            //             "https://tse3-mm.cn.bing.net/th/id/OIP-C.3hgHOZjm8IJRXxxwUMi1RgHaE7?w=278&h=185&c=7&r=0&o=5&dpr=1.5&pid=1.7",
            //             "https://tse4-mm.cn.bing.net/th/id/OIP-C.UDuXuLqzfdQ4nPnur1srkQHaE7?w=280&h=187&c=7&r=0&o=5&dpr=1.5&pid=1.7",
            //             "https://example.com/image7.jpg"
            //         }
            //     }
            // };
        }


    }
}
