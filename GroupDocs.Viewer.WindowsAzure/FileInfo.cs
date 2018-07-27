using GroupDocs.Viewer.Storage;
using System;

namespace GroupDocs.Viewer.WindowsAzure
{
    /// <summary>
    /// File information
    /// </summary>
    public struct FileInfo : IFileInfo
    {
        /// <summary>
        /// File or directory path
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// File size in bytes
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Last modification date
        /// </summary>
        public DateTime LastModified { get; set; }
        
        /// <summary>
        /// Indicates if file is directory
        /// </summary>
        public bool IsDirectory { get; set; }

        public FileInfo(string path, bool isDirectory)
        {
            Path = path;
            IsDirectory = isDirectory;
            LastModified = DateTime.MinValue;
            Size = 0;
        }

        public FileInfo(string blobName, bool isDirectory, DateTime lastModificationDate, long size)
        {
            Path = blobName;
            IsDirectory = isDirectory;
            LastModified = lastModificationDate;
            Size = size;
        }
    }
}
