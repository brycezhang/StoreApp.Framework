using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace StoreApp.Framework.Utils
{
    public class FolderZip
    {
#if WINDOWS_PHONE
        /// <summary>
        /// 解压缩文件
        /// </summary>
        /// <param name="zipFileDirectory">zip包所在目录</param>
        /// <param name="zipFilename">zip文件名</param>
        /// <param name="extractFolder">提取目录</param>
        /// <returns></returns>
        public static async Task UnZipFile(IStorageFolder zipFileDirectory, string zipFilename,
            IStorageFolder extractFolder = null)
        {
            if (extractFolder == null) extractFolder = zipFileDirectory;

            var folder = zipFileDirectory;

            using (var zipStream = await folder.OpenStreamForReadAsync(zipFilename))
            using (var archive = SharpCompress.Archive.Zip.ZipArchive.Open(zipStream))
            {
                foreach (SharpCompress.Archive.Zip.ZipArchiveEntry entry in archive.Entries)
                {
                    if (!entry.IsDirectory)
                    {
                        using (Stream fileData = entry.OpenEntryStream())
                        {
                            string fileName = entry.FilePath.Replace("/", "\\");
                            StorageFile outputFile = await extractFolder.CreateFileAsync(fileName,
                                        CreationCollisionOption.ReplaceExisting);

                            using (Stream outputFileStream = await outputFile.OpenStreamForWriteAsync())
                            {
                                await fileData.CopyToAsync(outputFileStream);
                                await outputFileStream.FlushAsync();
                            }
                        }
                    }
                }
            }
        }
#endif

#if WINDOWS_8
        /// <summary>
        /// 压缩文件夹和子文件夹
        /// </summary>
        private static async Task ZipFolderContents(IStorageFolder sourceFolder, ZipArchive archive, string baseDirPath)
        {
            IReadOnlyList<StorageFile> files = await sourceFolder.GetFilesAsync();

            foreach (StorageFile file in files)
            {
                ZipArchiveEntry readmeEntry = archive.CreateEntry(GetCompressedFileName(baseDirPath, file));

                byte[] buffer = WindowsRuntimeBufferExtensions.ToArray(await FileIO.ReadBufferAsync(file));

                // And write the contents to it
                using (Stream entryStream = readmeEntry.Open())
                {
                    await entryStream.WriteAsync(buffer, 0, buffer.Length);
                }
            }

            IReadOnlyList<StorageFolder> subFolders = await sourceFolder.GetFoldersAsync();

            if (!subFolders.Any()) return;

            foreach (StorageFolder subfolder in subFolders)
                await ZipFolderContents(subfolder, archive, baseDirPath);
        }

        public static async Task ZipFolder(IStorageFolder sourceFolder, IStorageFolder destnFolder, string zipFileName)
        {
            StorageFile zipFile =
                await destnFolder.CreateFileAsync(zipFileName, CreationCollisionOption.ReplaceExisting);

            Stream zipToCreate = await zipFile.OpenStreamForWriteAsync();

            ZipArchive archive = new ZipArchive(zipToCreate, ZipArchiveMode.Update);
            await ZipFolderContents(sourceFolder, archive, sourceFolder.Path);
            archive.Dispose();
        }

        private static string GetCompressedFileName(string baseDirPath, IStorageFile file)
        {
            return file.Path.Remove(0, baseDirPath.Length);
        }
        /// <summary>
        /// 解压缩文件
        /// </summary>
        /// <param name="zipFileDirectory">zip包所在目录</param>
        /// <param name="zipFilename">zip文件名</param>
        /// <param name="extractFolder">提取目录</param>
        /// <returns></returns>
        public static async Task UnZipFile(IStorageFolder zipFileDirectory, string zipFilename,
            IStorageFolder extractFolder = null)
        {
            if (extractFolder == null) extractFolder = zipFileDirectory;

            var folder = zipFileDirectory;

            using (var zipStream = await folder.OpenStreamForReadAsync(zipFilename))
            {
                using (MemoryStream zipMemoryStream = new MemoryStream((int)zipStream.Length))
                {
                    await zipStream.CopyToAsync(zipMemoryStream);

                    using (var archive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Read))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            if (entry.Name != "")
                            {
                                using (Stream fileData = entry.Open())
                                {
                                    string fileName = entry.FullName.Replace("/", "\\");
                                    StorageFile outputFile =
                                        await
                                            extractFolder.CreateFileAsync(fileName,
                                                CreationCollisionOption.ReplaceExisting);
                                    using (Stream outputFileStream = await outputFile.OpenStreamForWriteAsync())
                                    {
                                        await fileData.CopyToAsync(outputFileStream);
                                        await outputFileStream.FlushAsync();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
#endif
    }
}
