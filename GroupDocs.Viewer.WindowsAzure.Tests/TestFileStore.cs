using GroupDocs.Viewer.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GroupDocs.Viewer.WindowsAzure.Tests
{
    public class TestFileStorage : IFileStorage
    {
        public char PathDelimiter { get { return '/'; } }

        private readonly Dictionary<string, Stream> _files = new Dictionary<string, Stream>();

        public IDictionary<string, Stream> Files
        {
            get
            {
                return _files;
            }
        }

        public void SaveFile(string path, Stream file)
        {
            var ms = new MemoryStream();
            file.CopyTo(ms);

            ms.Position = 0;
            file.Position = 0;

            if (_files.ContainsKey(path))
            {
                _files[path] = ms;
            }
            else
            {
                _files.Add(path, ms);
            }
        }

        public Stream GetFile(string path)
        {
            path = NormalizePath(path);
            var stream = new MemoryStream();

            _files[path].CopyTo(stream);
            _files[path].Position = 0;

            stream.Position = 0;
            return stream;
        }

        public bool FileExists(string fileName)
        {
            fileName = NormalizePath(fileName);

            return _files.ContainsKey(fileName);
        }

        public IEnumerable<IFileInfo> GetFilesInfo(string folder)
        {
            return _files.Where(p => p.Key.StartsWith(folder))
                .Select(p => (IFileInfo) new Storage.FileInfo
                {
                    LastModified = new DateTime(1, 1, 1),
                    IsDirectory = false,
                    Path = p.Key,
                    Size = p.Value.Length
                }).ToList();
        }

        public IFileInfo GetFileInfo(string path)
        {
            var now = new DateTime(DateTime.Now.Year, 7, 7);

            var file = new Storage.FileInfo
            {
                Path = path,
                LastModified = now,
            };

            if (_files.ContainsKey(path))
                file.Size = _files[path].Length;

            return file;
        }

        public void DeleteDirectory(string path)
        {
            foreach (var entry in _files.Where(file => file.Key.StartsWith(path)).ToList())
            {
                _files[entry.Key].Close();
                _files[entry.Key].Dispose();

                _files.Remove(entry.Key);
            }
        }

        /// <summary>
        /// Gets normalized blob name, updates guid from dir\\file.ext to dir/file.ext
        /// </summary>
        /// <param name="path">The unique identifier.</param>
        /// <returns>Normalized blob name.</returns>
        public string NormalizePath(string path)
        {
            return Regex.Replace(path, @"\\+", PathDelimiter.ToString()).Trim(PathDelimiter);
        }
    }
}
