using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Net.Http;
using System.Net;
using System.Collections.Specialized;
using System.Xml.Linq;

namespace message_handler
{
    public class MyImageCreator
    {
        public static async Task UploadImage(Bitmap bmp)
        {
            List<byte> imageBytes = new List<byte>();
            {
                using (var stream = new MemoryStream())
                {
                    bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] buffer = new byte[1024];
                    for (int len; (len = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0;)
                    {
                        for (int i = 0; i < len; i++) imageBytes.Add(buffer[i]);
                    }
                }
            }
            using (var w = new WebClient())
            {
                var values = new NameValueCollection
                {
                    { "key", "433a1bf4743dd8d7845629b95b5ca1b4" },
                    { "image", Convert.ToBase64String(imageBytes.ToArray()) }
                };
                byte[] response = await w.UploadValuesTaskAsync("https://api.imgur.com/3/image", values);

                Console.WriteLine(XDocument.Load(new MemoryStream(response)));
            }
        }
    }
}
