using System.Collections.Generic;

namespace IMWinUi.Models
{
    internal class FavoriteItem
    {

        internal required string Text { get; set; } 
        internal required List<string> ImageUrls { get; set; } 

        internal FavoriteItem(string text, List<string> imageUrls)
        {
            Text = text;
            ImageUrls = imageUrls.Count<9 ? imageUrls : imageUrls.GetRange(0, 9);
        }
    }
}
