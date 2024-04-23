using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using Dt.Kpsirs.Common.File;
//using Dt.Kiuss.Core.Model.File;
//using Microsoft.AspNetCore.Http;
using Dt.Kpuirs.Common.File.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.Notification;
using Minio.Exceptions;
using Microsoft.AspNetCore.Builder;
using Dt.File.Store;

namespace Dt.Kpsirs.Common.File.Files
{
    public record MinioConfiguration(string endpoint, string accessKey, string secretKey, string rootFolderName, string bucketName, string location);
    public class FileStoreKpsirs : FileStore, IFileStore
    {
        public FileStoreKpsirs()
        {
            
        }
        public async Task<FileContentDto> LoadFile(Guid fileId, Guid drillingProjectId, string fileName, FileType fileType)
        {
            var objectName = $"{fileId}.{fileName}";
            var folderName = drillingProjectId.ToString();
            var minioFileName = $"{folderName}\\{fileType}\\{objectName}";
            var filePath = $"{rootFolderName}\\{minioFileName}";

            Task<FileContentDto> task = LoadFile_(objectName, folderName, minioFileName, filePath);
            return task.Result;
        }
        public async Task CreateFile(Guid fileId, Guid drillingProjectId, byte[] fileContent, string fileName, FileType fileType)
        {
            var objectName = $"{fileId}.{fileName}";
            var folderName = drillingProjectId.ToString();
            var minioFileName = $"{folderName}\\{fileType}\\{objectName}";
            var filePath = $"{rootFolderName}\\{minioFileName}";

            CreateFile_(objectName, folderName, minioFileName, filePath, fileContent);
          
        }
        public async void DeleteFile(Guid fileId, Guid drillingProjectId, string fileName, FileType fileType)
        {
            var objectName = $"{fileId}.{fileName}";
            var folderName = drillingProjectId.ToString();
            var minioFileName = $"{folderName}\\{fileType}\\{objectName}";
            var filePath = $"{rootFolderName}\\{minioFileName}";

            DeleteFile_(objectName, folderName, minioFileName, filePath);
        }
    }
}