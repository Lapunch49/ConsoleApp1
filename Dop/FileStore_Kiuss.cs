﻿using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Threading.Tasks;
//using Dt.Kiuss.Core.Model.File;
//using Microsoft.AspNetCore.Http;
using Dt.Kpuirs.Common.File.Dto;
using Microsoft.Extensions.Primitives;
using Minio;
using Minio.DataModel.Args;
using Minio.DataModel.Notification;
using Minio.Exceptions;

namespace Dt.Kiuss.Supervisor.Domain.Utils.File
{
    /// <summary>
    /// Провайдер хранилища файлов
    /// </summary>
    public class FileStore : IFileStore
    {
        private string bucketName = "bucket-kiuss";
        private string rootFolderName = "D:\\practice\\data";
        IMinioClient minio;
        public FileStore(string bucketName)
        {
            // при создании "хранилища" подключаемся к api MinIO Object Store, используя логин и пароль
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                                                   | SecurityProtocolType.Tls11
                                                   | SecurityProtocolType.Tls12;
            var endpoint = "localhost:9000";
            var accessKey = "chulpan";
            var secretKey = "minio123";
            // Initialize MinIO Client object.
            minio = new MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secretKey)
                .Build();
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
            byte[] buffer;
            try
            {
                // Check whether the object exists using statObject().
                // If the object is not found, statObject() throws an exception,
                // else it means that the object exists.
                // Execution is successful.
                StatObjectArgs statObjectArgs = new StatObjectArgs()
                                                    .WithBucket(bucketName)
                                                    .WithObject(objectName);
                await minio.StatObjectAsync(statObjectArgs);

                // Get input stream to have content of 'objectName' from 'bucketName'
                using (var memoryStream = new MemoryStream())
                {
                    GetObjectArgs getObjectArgs = new GetObjectArgs()
                                                  .WithBucket(bucketName)
                                                  .WithObject(objectName)
                                                  .WithCallbackStream((stream) =>
                                                  {
                                                      stream.CopyTo(memoryStream);
                                                  });
                    await minio.GetObjectAsync(getObjectArgs);
                    buffer = memoryStream.ToArray();
                    Console.WriteLine($"Downloaded the file {objectName} from the bucket {bucketName}");
                    return new FileContentDto(filePath, buffer);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Файла с именем {objectName} не существует в {bucketName}"+e);
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
                    // Create new bucket with name bucketName if it doesn't exist
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
                    // Upload file to the bucket
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
                    Console.WriteLine($"Не удалось найти файл {filePath}"+e);
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

                minio.RemoveObjectAsync(args).ConfigureAwait(false);

            }
            catch (Exception e)
            {
                Console.WriteLine($"Не удалось удалить файл {objectName} из {bucketName}"+e);
            }
            Console.WriteLine($"Removed object {objectName} from bucket {bucketName} successfully");

        }
        private static async Task ListOfBuckets(IMinioClient minio)
        {
            var bucketName = "mybucket";
            try
            {
                var getListBucketsTask = await minio.ListBucketsAsync().ConfigureAwait(false);

                // Iterate over the list of buckets.
                foreach (var bucket in getListBucketsTask.Buckets)
                {
                    Console.WriteLine(bucket.Name + " " + bucket.CreationDateDateTime);
                }

            }
            catch (MinioException e)
            {
                Console.WriteLine("Error occurred: " + e);
            }
        }

    }
}