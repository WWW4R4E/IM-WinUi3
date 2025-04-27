using System;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;

namespace IMWinUi.Converter;

public class ByteArrayToImageConverter : IValueConverter
{
    // 静态加载本地占位图（需确保路径正确）
    private static readonly BitmapImage PlaceholderImage = new BitmapImage(new Uri("ms-appx:///Assets/logo.jpg"));

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is byte[] bytes && bytes.Length > 24)
        {
            var bitmap = new BitmapImage();
            using (var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream())
            {
                stream.WriteAsync(bytes.AsBuffer()).AsTask().GetAwaiter().GetResult();
                stream.Seek(0);
                bitmap.SetSource(stream);
            }

            Debug.Write("转换成功");
            return bitmap;
        }

        Debug.Write("转换失败");
        return PlaceholderImage;
    }

    // 反向转换接口
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}