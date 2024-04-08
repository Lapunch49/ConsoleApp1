using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Threading.Tasks;
//using Dt.Kiuss.Core.Model.File;
//using Microsoft.AspNetCore.Http;
using Dt.Kpuirs.Common.File.Dto;
using Minio;
using Minio.DataModel.Args;
using Minio.DataModel.Notification;

namespace Dt.Kiuss.Supervisor.Domain.Utils.File
{
    /// <summary>
    /// Провайдер хранилища файлов
    /// </summary>
    public class FileStore : IFileStore
    {
        //private readonly string filesPath;
        private readonly string bucketName = "bucket-kiuss";
        private string rootFolderName = "D:\\practice\\data";
        IMinioClient minio;
        public FileStore()
        {
            // при создании "хранилища" подключаемся к api MinIO Object Store, используя логин и пароль
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                                                   | SecurityProtocolType.Tls11
                                                   | SecurityProtocolType.Tls12;
            var endpoint = "localhost:9000";
            var accessKey = "chulpan";
            var secretKey = "minio123";

            try
            {
                minio = new MinioClient()
                    .WithEndpoint(endpoint)
                    .WithCredentials(accessKey, secretKey)
                    .Build();
                Console.WriteLine("Подключились к minIO хранилищу");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// Получение содержимого файла
        /// </summary>
        /// <param name="fileId">Идентификатор файла</param>
        /// <param name="drillingProjectId">Идентификатор ПБ</param>
        /// <param name="fileName">Наименование файла</param>
        /// <returns>Содержимое файла</returns>
        public async Task<FileContentDto> LoadFile(Guid fileId, Guid drillingProjectId, string fileName)
        {
            string objectName = fileId.ToString() + "." + fileName;
            string folderName = drillingProjectId.ToString();
            string filePath = rootFolderName + "\\" + folderName + "\\" + objectName;

            try
            {
                Console.WriteLine("Running example for API: GetObjectAsync");
                var args = new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithFile(fileName);
                var stat = await minio.GetObjectAsync(args).ConfigureAwait(false);
                Console.WriteLine($"Downloaded the file {fileName} in bucket {bucketName}");
                Console.WriteLine($"Stat details of object {objectName} in bucket {bucketName}\n" + stat);
                Console.WriteLine();
                byte[] buffer = { 1, 2, 3, 4 };
                return new FileContentDto(filePath, buffer);
            }
            catch (Exception e)
            {
                throw new Exception($"Файла с именем {fileName} не существует") ;
            }
        }

        /// <summary>
        /// Создание файла - перемещение файла из диска в minIO
        /// </summary>
        /// <param name="fileId">Идентификатор файла</param>
        /// <param name="drillingProjectId">Идентификатор ПБ</param>
        /// <param name="fileContent">Содержимое файла</param>
        /// <param name="fileName">Наименование файла</param>
        /// <returns>Результат асинхронной операции</returns>
        public async Task CreateFile(Guid fileId, Guid drillingProjectId, byte[] fileContent, string fileName)
        {
            var location = "us-east-1";

            string objectName = fileId.ToString()+"." + fileName;
            string folderName = drillingProjectId.ToString();
            string filePath = rootFolderName+"\\"+ folderName + "\\" + objectName;

            using (MemoryStream fileStream = new MemoryStream(fileContent))
            {
                try
                {
                    var bktExistArgs = new BucketExistsArgs()
                        .WithBucket(bucketName);
                    var found = await minio.BucketExistsAsync(bktExistArgs).ConfigureAwait(false);
                    if (!found)
                    {
                        var mkBktArgs = new MakeBucketArgs()
                            .WithBucket(bucketName)
                            .WithLocation(location);
                        await minio.MakeBucketAsync(mkBktArgs).ConfigureAwait(false);
                    }

                    var putObjectArgs = new PutObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject(objectName)
                        .WithStreamData(fileStream)
                        .WithObjectSize(fileContent.Length);
                    //.WithFileName(filePath);
                    //.WithContentType(contentType);
                    _ = await minio.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
                    Console.WriteLine($"\nSuccessfully uploaded {objectName}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// Удаление файла
        /// </summary>
        /// <param name="fileId">Идентификатор файла</param>
        /// <param name="drillingProjectId">Идентификатор ПБ</param>
        /// <param name="fileName">Наименование файла</param>
        public async void DeleteFile(Guid fileId, Guid drillingProjectId, string fileName)
        {
            string objectName = fileId.ToString() + "." + fileName;
            string folderName = drillingProjectId.ToString();
            string filePath = rootFolderName + "\\" + folderName + "\\" + objectName;
            string versionId = null;

            try
            {
                var args = new RemoveObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName);

                Console.WriteLine("Running example for API: RemoveObjectAsync");
                minio.RemoveObjectAsync(args).ConfigureAwait(false);
                
                Console.WriteLine($"Removed object {objectName} from bucket {bucketName} successfully");

                //else
                //{
                //    Console.WriteLine($"There is no such object:{objectName} or bucket:{bucketName}");
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Bucket-Object]  Exception: {e}");
            }
            
        }
    }
}