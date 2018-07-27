using GroupDocs.Viewer.Storage;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupDocs.Viewer.WindowsAzure.Tests
{
    [TestFixture]
    public class FileStorageTests
    {
        private FileStorage _fileStorage;
        private const string TestContainerName = "test";

        [OneTimeSetUp]
        public void SetupFixture()
        {
            _fileStorage = new FileStorage(TestContainerName);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _fileStorage.DeleteDirectory(@"test\");
            _fileStorage.DeleteDirectory(@"sample\");
        }

        [Test]
        public void ShouldInitializeStorage()
        {
            Assert.DoesNotThrow(() => { FileStorage fileStorage = new FileStorage(TestContainerName); });
        }

        [Test]
        public void ShouldSaveFile()
        {
            string filePath = "sample/file.txt";
            using (Stream testStream = GetTestFileStream())
            {
                _fileStorage.SaveFile(filePath, testStream);

                Assert.True(_fileStorage.FileExists(filePath));
                Assert.AreEqual(testStream.Length, _fileStorage.GetFileInfo(filePath).Size);
            }
        }

        [Test]
        public void ShouldDeletedirectory()
       {
            string filePath = "test/sample/file.txt";

            using (Stream testStream = GetTestFileStream())
            {
                _fileStorage.SaveFile(filePath, testStream);
            }

            IEnumerable<IFileInfo> entities = _fileStorage.GetFilesInfo("test/");
            int sampleDirectoryCount = entities.Where(item => item.IsDirectory = true && item.Path.Equals(@"test/sample/")).Count<IFileInfo>();
            Assert.AreEqual(sampleDirectoryCount, 1);

            _fileStorage.DeleteDirectory(@"test/sample/");


            entities = _fileStorage.GetFilesInfo("test/");
            sampleDirectoryCount = entities.Where(item => item.IsDirectory = true && item.Path.Equals(@"test/sample/")).Count<IFileInfo>();
            Assert.AreEqual(sampleDirectoryCount, 0);           
        }

        [Test]
        public void ShouldReturnFileExists()
        {
            string unexistingPath = "test/sample/unexisting.doc";
            string existingPath = "test/sample/existing.doc";
            using (Stream testStream = GetTestFileStream())
                _fileStorage.SaveFile(existingPath, testStream);

            Assert.False(_fileStorage.FileExists(unexistingPath));
            Assert.True(_fileStorage.FileExists(existingPath));
        }

        [Test]
        public void ShouldGetFile()
        {
            long testStreamLength = 0;
            string path = "test/sample/existing.doc";
            using (Stream testStream = GetTestFileStream())
            {
                testStreamLength = testStream.Length;
                _fileStorage.SaveFile(path, testStream);              
            }

            using (Stream getStream = _fileStorage.GetFile(path))
            {
                Assert.True(getStream != null);
                Assert.AreEqual(testStreamLength, getStream.Length);
            }
        }

        private Stream GetTestFileStream()
        {
            return new MemoryStream(new byte[] { 100, 101, 102 });
        }
    }
}