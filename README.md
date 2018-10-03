# GroupDocs.Viewer .Net AzureBlobStorage

[WindowsAzure Blob Storage](https://azure.microsoft.com/en-us/services/storage/) IFileStorage provider for [GroupDocs.Viewer for .NET](https://www.nuget.org/packages/groupdocs.viewer) which allows you to keep files and cache in the cloud. 

## Installation & Configuration

Install via [nuget.org](http://nuget.org)

```powershell
Install-Package GroupDocs.Viewer.AzureBlobStorage 
```

## How to use

After instalation via nuget, configure a connection string to your Azure storage account in an app.config or web.config file.
Then use the following sample code that shows the usage:
```csharp

var azureStorage = new AzureBlobStorage("ContainerName");
var viewerHtmlHandler = new ViewerHtmlHandler(azureStorage);
var pages = viewerHtmlHandler.GetPages("document.docx");
```


## License

GroupDocs.Viewer .Net AzureBlobStorage is Open Source software released under the [MIT license](https://github.com/dovletmamenov/GroupDocs.Viewer.AzureBlobStorage/blob/master/LICENSE).
