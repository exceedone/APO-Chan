using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Apo_Chan.Items;

namespace Apo_Chan.Models
{
    /// <summary>
    /// Using for Azure Mobile Apps(*not datatable. ex. getting token, refres token, ...)
    /// </summary>
    public class AMSAPI
    {

    }

    /// <summary>
    /// Using for Azure Blob(File System)
    /// </summary>
    public class AzureBlobAPI
    {
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
        public static async Task UploadFile(string fileName, string containerName, Stream stream)
        {
            try
            {
                var blob = await getBlob(fileName, containerName, true);
                await blob.UploadFromStreamAsync(stream);
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
        public static async Task UploadFile(UserItem user, Stream stream)
        {
            await UploadFile(user.AMSUserId.Replace(":", ""), "profile", stream);
        }

        /// <summary>
        /// Download File from AzureBlob
        /// https://dotnetcoretutorials.com/2017/06/17/using-azure-blob-storage-net-core/
        /// </summary>
        /// <param name="fileName">download file name</param>
        /// <param name="containerName">target container name</param>
        /// <returns></returns>
        public static async Task<Stream> DownloadFile(string fileName, string containerName)
        {
            try
            {
                MemoryStream stream = new MemoryStream();
                var blob = await getBlob(fileName, containerName, false);
                await blob.DownloadToStreamAsync(stream);
                return stream;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static async Task<Stream> DownloadFile(UserItem user)
        {
            try
            {
                return await DownloadFile(user.AMSUserId.Replace(":", ""), "profile");
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
