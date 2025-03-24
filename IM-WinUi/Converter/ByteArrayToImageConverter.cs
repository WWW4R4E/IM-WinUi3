using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;

namespace IMWinUi.Converter;


public class ByteArrayToImageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is byte[] bytes && bytes.Length > 0)
        {
            var bitmap = new BitmapImage();
            using (var stream = new InMemoryRandomAccessStream())
            {
                // 将字节数组写入流
                stream.WriteAsync(bytes.AsBuffer()).AsTask().GetAwaiter().GetResult();
                stream.Seek(0);
                // 设置图片源
                bitmap.SetSource(stream);
            }
            return bitmap;
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}