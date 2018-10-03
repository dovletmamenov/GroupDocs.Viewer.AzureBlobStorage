# GroupDocs.Viewer.AzureBlobStorage
<a href="https://azure.microsoft.com/en-us/services/storage/blobs/">WindowsAzure Blob Storage</a> IFileStorage, IInputDataHandler and ICacheDataHandler providers for GroupDocs.Viewer for .NET

[WindowsAzure Blob Storage](https://azure.microsoft.com/en-us/services/storage/) IFileStorage provider for [GroupDocs.Viewer for .NET](https://www.nuget.org/packages/groupdocs.viewer)
 which allows you to keep files and cache in the cloud. 

## Installation & Configuration

Install via [nuget.org](http://nuget.org)

```powershell
Install-Package GroupDocs.Viewer.AzureBlobStorage ## How to use

```csharp

var amazonS3Config = new AmazonS3Config { RegionEndpoint = RegionEndpoint.USWest2 };
var amazonS3Client = new AmazonS3Client(amazonS3Config);
var amazonBucketName = "my-bucket";

var amazonS3FileManager = new AmazonS3FileManager(amazonS3Client, amazonBucketName);

var viewerDataHandler = new ViewerDataHandler(amazonS3FileManager);

var viewerConfig = new ViewerConfig
{
    EnableCaching = true
};

var viewerHtmlHandler = new ViewerHtmlHandler(viewerConfig, viewerDataHandler, viewerDataHandler);

var pages = viewerHtmlHandler.GetPages("document.docx");
```


## License

GroupDocs.Viewer .NET AmazonS3 is Open Source software released under the [MIT license](https://github.com/harumburum/groupdocs-viewer-net-amazons3/blob/master/LICENSE.md).
