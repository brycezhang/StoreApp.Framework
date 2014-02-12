using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.Phone.Tasks;

namespace StoreApp.Framework.Storage
{
    /// <summary>
    /// 本地文件操作服务类
    /// </summary>
    public class LocalFileService
    {
        /// <summary> 
        /// 存储数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="fileName">文件名称</param>
        /// <param name="data">数据</param>
        /// <returns>无</returns>
        public async static Task Save<T>(string fileName, T data)
        {
            //取得当前程序存放数据的目录
            var folder = ApplicationData.Current.LocalFolder;
            //创建文件，如果文件存在就覆盖
            var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            using (var newFileStream = await file.OpenStreamForWriteAsync())
            {
                var ser = new DataContractSerializer(typeof(T));
                ser.WriteObject(newFileStream, data);
                newFileStream.Dispose();
            }
        }
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="fileName">文件名称</param>
        /// <returns>数据</returns>
        public async static Task<T> Read<T>(string fileName)
        {
            var t = default(T);
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.GetFileAsync(fileName);
                if (file == null)
                    return t;
                var newFileStream = await file.OpenStreamForReadAsync();
                var ser = new DataContractSerializer(typeof(T));
                t = (T)ser.ReadObject(newFileStream);
                newFileStream.Dispose();
                return t;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return t;
            }
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <returns>成功true/失败false</returns>
        public async static Task<bool> Delete(string fileName)
        {
            var folder = ApplicationData.Current.LocalFolder;
            return await Delete(folder, fileName);
        }
        /// <summary>
        /// 删除指定文件夹下的文件
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async static Task<bool> Delete(IStorageFolder folder, string fileName)
        {
            var file = await folder.GetFileAsync(fileName);
            if (file != null)
            {
                try
                {
                    await file.DeleteAsync();
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
