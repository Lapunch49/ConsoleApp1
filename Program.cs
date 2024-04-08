using System.IO;
using System.Net;
using System.Security.AccessControl;
using Dt.Kiuss.Supervisor.Domain.Utils.File;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace ConsoleApp1
{

    /// <summary>
    ///     This example creates a new bucket if it does not already exist, and
    ///     uploads a file to the bucket. The file name is chosen to be
    ///     "C:\\Users\\vagrant\\Downloads\\golden_oldies.mp3"
    ///     Either create a file with this name or change it with your own file,
    ///     where it is defined down below.
    /// </summary>
    public static class FileUpload
    {
        private static bool IsWindows()
        {
            return OperatingSystem.IsWindows();
        }

        private static async Task Main(string[] args)
        {
            // подключаемся к хранилищу
            FileStore fs = new FileStore();
            
            // загружаем 2 объекта
            Guid fileId = new Guid("4d65eaa9-6b39-4037-b36b-bc2bc4461612");
            Guid drillingProjectId = new Guid("67f5e5e2-a7dc-4bcd-b793-bd4a9a614a09");
            byte[] fileContent = File.ReadAllBytes("D:\\practice\\data\\67f5e5e2-a7dc-4bcd-b793-bd4a9a614a09\\4d65eaa9-6b39-4037-b36b-bc2bc4461612.txt");
            await fs.CreateFile(fileId, drillingProjectId, fileContent, "txt");
            fileId = new Guid("6cfc3e97-c13a-4f08-b5c3-a2c0136fbe58");
            fileContent = File.ReadAllBytes("D:\\practice\\data\\67f5e5e2-a7dc-4bcd-b793-bd4a9a614a09\\6cfc3e97-c13a-4f08-b5c3-a2c0136fbe58.jpg");
            await fs.CreateFile(fileId, drillingProjectId, fileContent, "jpg");

            // удаляем 1 объект
            fs.DeleteFile(fileId, drillingProjectId, "jpg");

            //загружаем 1 объект


        }

        /// <summary>
        ///     Task that uploads a file to a bucket
        /// </summary>
        /// <param name="minio"></param>
        /// <returns></returns>
        private static async Task Run(IMinioClient minio)
        {
            // Make a new bucket called mymusic.
            var bucketName = "mybucket"; //<==== change this
            var location = "us-east-1";
            // Upload the zip file
            var objectName = "Wham! - Last Christmas.mp3";
            // The following is a source file that needs to be created in
            // your local filesystem.
            var filePath = "D:\\музыка\\Wham! - Last Christmas.mp3";
            var contentType = "application/zip";

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
                    .WithFileName(filePath)
                    .WithContentType(contentType);
                _ = await minio.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
                Console.WriteLine($"\nSuccessfully uploaded {objectName}\n");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            // Added for Windows folks. Without it, the window, tests
            // run in, dissappears as soon as the test code completes.
            if (IsWindows()) _ = Console.ReadLine();
        }
        /// <summary>
        ///     Task that output all objects from bucket
        /// </summary>
        /// <param name="minio"></param>
        /// <returns></returns>
        private static async Task Run2(IMinioClient minio)
        {
            var bucketName = "mybucket";
            try
            {
                // Just list of objects
                // Check whether 'mybucket' exists or not.
                var bktExistArgs = new BucketExistsArgs()
                    .WithBucket(bucketName);
                var found = await minio.BucketExistsAsync(bktExistArgs).ConfigureAwait(false);
                if (found)
                {
                    // Create an async task for listing buckets.
                    var getListBucketsTask = await minio.ListBucketsAsync().ConfigureAwait(false);

                    // Iterate over the list of buckets.
                    foreach (var bucket in getListBucketsTask.Buckets)
                    {
                        Console.WriteLine(bucket.Name + " " + bucket.CreationDateDateTime);
                    }
                }
                else
                {
                    Console.WriteLine("mybucket does not exist");
                }
            }
            catch (MinioException e)
            {
                Console.WriteLine("Error occurred: " + e);
            }
        }


        /// <summary>
        ///     Task that output all buckets and their creation datatime
        /// </summary>
        /// <param name="minio"></param>
        /// <returns></returns>
        private static async Task Run3(IMinioClient minio)
        {
            string bucketName = "mybucket";
            try
            {
                // Just list of objects
                // Check whether 'mybucket' exists or not.
                var bktExistArgs = new BucketExistsArgs()
                    .WithBucket(bucketName);
                var found = await minio.BucketExistsAsync(bktExistArgs).ConfigureAwait(false);

                if (found)
                {
                    // Create an async task for listing buckets.
                    var getListBucketsTask = await minio.ListBucketsAsync().ConfigureAwait(false);

                    // Iterate over the list of buckets.
                   foreach (var bucket in getListBucketsTask.Buckets)
                    {
                        Console.WriteLine(bucket.Name + " " + bucket.CreationDateDateTime);
                    }
                }
                else
                {
                    Console.WriteLine(bucketName + " does not exist");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occurred: " + e);
            }
        }
        /// <summary>
        ///     Task that removes a file from a bucket
        /// </summary>
        /// <param name="minio"></param>
        /// <returns></returns>
        public static async Task Run4(IMinioClient minio,
        string bucketName = "mybucket",
        string objectName = "Wham! - Last Christmas.mp3",
        string versionId = null)
        {
            if (minio is null) throw new ArgumentNullException(nameof(minio));

            try
            {
                var args = new RemoveObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName);
                var versions = "";
                if (!string.IsNullOrEmpty(versionId))
                {
                    args = args.WithVersionId(versionId);
                    versions = ", with version ID " + versionId + " ";
                }

                Console.WriteLine("Running example for API: RemoveObjectAsync");
                await minio.RemoveObjectAsync(args).ConfigureAwait(false);
                if (args.IsBucketCreationRequest)
                {
                    Console.WriteLine($"Removed object {objectName} from bucket {bucketName}{versions} successfully");
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine($"There is no such object:{objectName} or bucket:{bucketName}{versions}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Bucket-Object]  Exception: {e}");
            }
        }
    }
}

