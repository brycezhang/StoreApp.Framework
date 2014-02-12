using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace StoreApp.Framework.Storage
{
    public static class StorageFolderEx
    {
        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<bool> FileExistsAsync(this IStorageFolder folder, string fileName)
        {
            try
            {
                await folder.GetFileAsync(fileName);
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }
        /// <summary>
        /// 文件夹是否存在
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static async Task<bool> FolderExistsAsync(this IStorageFolder folder, string folderName)
        {
            try
            {
                await folder.GetFolderAsync(folderName);
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }
    }
}
