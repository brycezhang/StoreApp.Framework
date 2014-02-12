using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace StoreApp.Framework.Storage
{
    public static class StorageFileEx
    {
        /// <summary>
        /// 读取文本
        /// </summary>
        /// <param name="storageFile"></param>
        /// <returns></returns>
        public static async Task<string> ReadText(this StorageFile storageFile)
        {
            string text;
            IRandomAccessStream accessStream = await storageFile.OpenReadAsync();

            using (Stream stream = accessStream.AsStreamForRead((int)accessStream.Size))
            {
                var content = new byte[stream.Length];
                await stream.ReadAsync(content, 0, (int)stream.Length);
                text = Encoding.UTF8.GetString(content, 0, content.Length);
            }

            return text;
        }
    }
}
