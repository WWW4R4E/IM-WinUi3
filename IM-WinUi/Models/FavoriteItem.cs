using System.Collections.Generic;

namespace IMWinUi.Models
{
    internal class FavoriteItem
    {

        internal required string Text { get; set; } // 文本内容
        internal required List<string> ImageUrls { get; set; } // 图片 URL 列表

    }
}
