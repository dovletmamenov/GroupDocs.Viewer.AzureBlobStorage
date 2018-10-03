using GroupDocs.Viewer.Config;
using GroupDocs.Viewer.Domain.Html;
using GroupDocs.Viewer.Handler;
using GroupDocs.Viewer.Storage;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GroupDocs.Viewer.AzureBlobStorage.Tests
{
    [TestFixture]
    public class FileStorageTests
    {
        private AzureBlobStorage _fileStorage;
        private const string TestContainerName = "test";

        [OneTimeSetUp]
        public void SetupFixture()
        {
            _fileStorage = new AzureBlobStorage(TestContainerName);
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
            Assert.DoesNotThrow(() => { AzureBlobStorage fileStorage = new AzureBlobStorage(TestContainerName); });
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

        [Test]
        public void ShouldRender()
        {
            // Create a file to be rendered
            string filePath = "sample/file.txt";
            using (Stream testStream = GetTestFileStream())
            {
                _fileStorage.SaveFile(filePath, testStream);
            }

            // Create IFileStorage and run the rendering
            AzureBlobStorage storage = new AzureBlobStorage(TestContainerName);
            ViewerHtmlHandler handler = new ViewerHtmlHandler(storage);
            List<PageHtml> pages = handler.GetPages("sample/file.txt");

           
            Assert.True(pages.Count > 0);
            Assert.NotNull(pages[0].HtmlContent);
        }

        private Stream GetTestFileStream()
        {
            return new MemoryStream(new byte[] { 100, 101, 102 });
        }
    }
}