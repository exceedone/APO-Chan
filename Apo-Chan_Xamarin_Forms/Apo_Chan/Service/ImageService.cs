﻿using Apo_Chan.Items;
using Apo_Chan.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Apo_Chan.Service
{
    public class ImageService
    {

        public static async Task SetImageSource(UserItem item)
        {
            ///// set from local
            var buffer = await getImageByteFromLocal(item.Id, "profile");
            if (buffer != null)
            {
                item.UserImage = CustomImageSource.FromByteArray(() => { return buffer; });
            }

            ///// set from Azure blob (not wait)
            setImageSourceFromBlob(item);
        }

        public static async Task SetImageSource(GroupItem item)
        {
            ///// set from local
            var buffer = await getImageByteFromLocal(item.Id, "group");
            if (buffer != null)
            {
                item.GroupImage = CustomImageSource.FromByteArray(() => { return buffer; });
            }

            ///// set from Azure blob (not wait)
            setImageSourceFromBlob(item);
        }

        public static async Task SaveImage(UserItem item, byte[] buffer)
        {
            ///// save to local
            await saveImageByteToLocal(item.Id, "profile", buffer);

            ///// save to Azure blob (not wait)
            uploadFile(item, buffer);
        }

        public static async Task SaveImage(GroupItem item, byte[] buffer)
        {
            ///// save to local
            await saveImageByteToLocal(item.Id, "profile", buffer);

            ///// save to Azure blob (not wait)
            uploadFile(item, buffer);
        }


        private static async Task setImageSourceFromBlob(UserItem item)
        {
            var buffer = await downloadFile(item);
            // replace if not defference image
            if (buffer != null && !buffer.EqualArray(item.UserImage.StreamByte))
            {
                item.UserImage = CustomImageSource.FromByteArray(() => { return buffer; });
                // save as loadfile
                await saveImageByteToLocal(item, buffer);
            }
        }

        private static async Task setImageSourceFromBlob(GroupItem item)
        {
            var buffer = await downloadFile(item);
            // replace if not defference image
            if (buffer != null && !buffer.EqualArray(item.GroupImage.StreamByte))
            {
                item.GroupImage = CustomImageSource.FromByteArray(() => { return buffer; });
                // save as loadfile
                await saveImageByteToLocal(item, buffer);
            }
        }

        #region Azure Blob
        private async static Task<CloudBlockBlob> getBlob(string fileName, string containerName, bool isCreateIfNotExists)
        {
            var storageCredentials = new StorageCredentials("apochanattachmentdev", "hMIt5ZQvvtIFxC/RIk54L6PL8qESIyTplF2Yy0qvmAGXT4NlH4YE5Yb6vDF98TAg/tD/1sDpLKgipBxzoJGZ6w==");
            var cloudStorageAccount = new CloudStorageAccount(storageCredentials, true);
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            var container = cloudBlobClient.GetContainerReference(containerName);
            if (isCreateIfNotExists)
            {
                await container.CreateIfNotExistsAsync();
            }
            var blob = container.GetBlockBlobReference(fileName);
            return blob;
        }

        /// <summary>
        /// Upload File to AzureBlob
        /// https://dotnetcoretutorials.com/2017/06/17/using-azure-blob-storage-net-core/
        /// </summary>
        /// <param name="fileName">upload file name</param>
        /// <param name="containerName">target container name</param>
        /// <returns></returns>
        private static async Task UploadFile(string fileName, string containerName, byte[] streamByte)
        {
            try
            {
                var blob = await getBlob(fileName, containerName, true);
                await blob.UploadFromByteArrayAsync(streamByte, 0, streamByte.Length);
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// Upload File to AzureBlob
        /// https://dotnetcoretutorials.com/2017/06/17/using-azure-blob-storage-net-core/
        /// </summary>
        /// <param name="user"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static async Task uploadFile(UserItem user, byte[] buffer)
        {
            await UploadFile(user.Id, "profile", buffer);
        }

        /// <summary>
        /// Upload Group Image to AzureBlob
        /// https://dotnetcoretutorials.com/2017/06/17/using-azure-blob-storage-net-core/
        /// </summary>
        /// <param name="user"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static async Task uploadFile(GroupItem group, byte[] buffer)
        {
            await UploadFile(group.Id, "group", buffer);
        }

        /// <summary>
        /// Download File from AzureBlob
        /// https://dotnetcoretutorials.com/2017/06/17/using-azure-blob-storage-net-core/
        /// </summary>
        /// <param name="fileName">download file name</param>
        /// <param name="containerName">target container name</param>
        /// <returns></returns>
        private static async Task<byte[]> DownloadFile(string fileName, string containerName)
        {
            try
            {
                byte[] buffer = new byte[16 * 1024];
                var blob = await getBlob(fileName, containerName, false);

                using (MemoryStream stream = new MemoryStream())
                {
                    await blob.DownloadToStreamAsync(stream);
                    buffer = Utils.ReadStram(stream);
                }
                return buffer;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static async Task<byte[]> downloadFile(UserItem user)
        {
            try
            {
                return await DownloadFile(user.Id, "profile");
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static async Task<byte[]> downloadFile(GroupItem group)
        {
            try
            {
                return await DownloadFile(group.Id, "group");
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// get image from local. (wait)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static async Task<byte[]> getImageByteFromLocal(string fileName, string containerName)
        {
            ///// 1. get image from local. (wait)
            // Access the file system for the current platform.
            IFileSystem fileSystem = FileSystem.Current;
            // Get the root directory of the file system for our application.
            IFolder rootFolder = fileSystem.LocalStorage;
            // Create another folder, if one doesn't already exist.
            IFolder photosFolder = await rootFolder.CreateFolderAsync(containerName, CreationCollisionOption.OpenIfExists);

            // Get File
            var checkResult = await photosFolder.CheckExistsAsync(fileName);
            if (checkResult == ExistenceCheckResult.FileExists)
            {
                IFile file = await photosFolder.GetFileAsync(fileName);
                if (file != null)
                {
                    using (System.IO.Stream stream = await file.OpenAsync(PCLStorage.FileAccess.Read))
                    {
                        return Utils.ReadStram(stream);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// save image from local. (wait)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static async Task saveImageByteToLocal(string fileName, string containerName, byte[] buffer)
        {
            ///// 1. get image from local. (wait)
            // Access the file system for the current platform.
            IFileSystem fileSystem = FileSystem.Current;
            // Get the root directory of the file system for our application.
            IFolder rootFolder = fileSystem.LocalStorage;
            // Create another folder, if one doesn't already exist.
            IFolder photosFolder = await rootFolder.CreateFolderAsync(containerName, CreationCollisionOption.OpenIfExists);

            // Get File
            IFile file = await photosFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            if (file != null)
            {
                using (System.IO.Stream stream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
                {
                    await stream.WriteAsync(buffer, 0, buffer.Length);
                }
            }
        }

        private static async Task saveImageByteToLocal(UserItem user, byte[] buffer)
        {
            await saveImageByteToLocal(user.Id, "profile", buffer);
        }

        private static async Task saveImageByteToLocal(GroupItem group, byte[] buffer)
        {
            await saveImageByteToLocal(group.Id, "group", buffer);
        }

        #endregion
    }
}