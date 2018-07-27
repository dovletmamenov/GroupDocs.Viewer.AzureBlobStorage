using System;
using System.Collections.Generic;
using System.IO;
using GroupDocs.Viewer.Storage;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Text.RegularExpressions;

namespace GroupDocs.Viewer.WindowsAzure
{
    public class FileStorage : IFileStorage
    {
        private CloudStorageAccount _storageAccount;
        private CloudBlobClient _blobClient;
        private CloudBlobContainer _container;

        /// <summary>
        /// The blob delimiter.
        /// </summary>
        public const char Delimiter = '/';

        public FileStorage(string containerName)
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            _blobClient = _storageAccount.CreateCloudBlobClient();
            _container = _blobClient.GetContainerReference(containerName);
        }

        public void DeleteDirectory(string path)
        {   
            string normalizedBlobName = NormalizePath(path);
            CloudBlobDirectory directory = _container.GetDirectoryReference(normalizedBlobName);

            foreach (IListBlobItem blob in directory.ListBlobs())
            {
                if (blob.GetType() == typeof(CloudBlob) || blob.GetType().BaseType == typeof(CloudBlob))
                {
                    try
                    {
                        ((CloudBlob)blob).DeleteIfExists();
                    }
                    catch
                    {
                        //ignore if blob can not be deleted
                    }
                }
            }
        }

        public bool FileExists(string path)
        {
            string blobName = NormalizePath(path);
            CloudBlockBlob blockBlob = _container.GetBlockBlobReference(blobName);
            return blockBlob.Exists();
        }

        public Stream GetFile(string path)
        {
            try
            {
                string blobName = NormalizePath(path);
                CloudBlockBlob blockBlob = _container.GetBlockBlobReference(blobName);
                MemoryStream stream = new MemoryStream();
                blockBlob.DownloadToStream(stream);
                stream.Position = 0;

                return stream;
            }
            catch (StorageException ex)
            {
                throw new System.Exception("Unable to get file.", ex);
            }
        }

        public IFileInfo GetFileInfo(string path)
        {
            try
            {
                string blobName = NormalizePath(path);
                CloudBlob blob = _container.GetBlobReference(blobName);
                blob.FetchAttributes();
                DateTime lastModificationDate = DateTime.MinValue;
                if (blob.Properties.LastModified.HasValue)
                    lastModificationDate = blob.Properties.LastModified.Value.DateTime;
                long size = blob.Properties.Length;

                return new Storage.FileInfo { Path = blobName, IsDirectory = false, LastModified = lastModificationDate, Size = size };
            }
            catch (StorageException ex)
            {
                throw new System.Exception("Unabled to get file description.", ex);
            }
        }

        public IEnumerable<IFileInfo> GetFilesInfo(string path)
        {
            IFileInfo info;

            string normalizedBlobName = NormalizePath(path);
            //if (!normalizedBlobName.EndsWith(Delimiter.ToString()))
            //    normalizedBlobName += Delimiter;

            CloudBlobDirectory directory = _container.GetDirectoryReference(normalizedBlobName);

            List<IFileInfo> filesInfo = new List<IFileInfo>();
            foreach (IListBlobItem blob in directory.ListBlobs())
            {   
                CloudBlobDirectory blobDirectory = blob as CloudBlobDirectory;

                if (blobDirectory != null)
                {
                   info = new Storage.FileInfo { Path = blobDirectory.Prefix, IsDirectory = true };                     
                }
                else
                {
                    ICloudBlob blobFile = (ICloudBlob)blob;

                    DateTime lastModificationDate = DateTime.MinValue;
                    if (blobFile.Properties.LastModified.HasValue)
                        lastModificationDate = blobFile.Properties.LastModified.Value.DateTime;

                    info = new Storage.FileInfo { Path = blobFile.Name, IsDirectory = false, LastModified = lastModificationDate, Size = blobFile.Properties.Length };
                }

                filesInfo.Add(info);
            }            

            return filesInfo;
        }

        public void SaveFile(string path, Stream content)
        {
            try
            {
                string blobName = NormalizePath(path);
                ICloudBlob blob = _container.GetBlockBlobReference(blobName);
                blob.UploadFromStream(content);
            }
            catch (StorageException ex)
            {
                throw new System.Exception("Unable to add file.", ex);
            }
        }

        /// <summary>
        /// Gets normalized blob name, updates guid from dir\\file.ext to dir/file.ext
        /// </summary>
        /// <param name="path">The unique identifier.</param>
        /// <returns>Normalized blob name.</returns>
        public static string NormalizePath(string path)
        {
            return Regex.Replace(path, @"\\+", Delimiter.ToString()).Trim(Delimiter);            
        }
    }
}
