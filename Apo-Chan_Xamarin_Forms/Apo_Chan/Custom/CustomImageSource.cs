using Apo_Chan.Models;
using System;
using System.IO;

namespace Xamarin.Forms
{
    /// <summary>
    /// expand ImageSource
    /// (Because ImageSource cannot get stream and filepath)
    /// </summary>
    public class CustomImageSource
    {
        public CustomImageSource()
        { }

        public string FilePath { get; set; }
        public byte[] StreamByte { get; set; }
        public ImageSource ImageSource { get; set; }

        public static CustomImageSource FromFile(string file)
        {
            return new CustomImageSource
            {
                ImageSource = ImageSource.FromFile(file),
                FilePath = file
            };
        }

        public static CustomImageSource FromStream(Func<Stream> stream)
        {
            return new CustomImageSource
            {
                ImageSource = ImageSource.FromStream(stream),
                StreamByte = Utils.ReadStram(stream.Invoke())
            };
        }

        public static CustomImageSource FromByteArray(Func<byte[]> buffer)
        {
            var byteArray = buffer.Invoke();
            return new CustomImageSource
            {
                ImageSource = ImageSource.FromStream(() => { return new MemoryStream(byteArray) as Stream; }),
                StreamByte = byteArray
            };
        }
    }
}
