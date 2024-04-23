using Minio.DataModel.Args;
using Minio.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;

namespace ConsoleApp1
{
    //public static class FileUpload
    //{
    //    // Initialize the client with access credentials.
    //    private static MinioClient minio = new MinioClient("play.min.io",
    //            "Q3AM3UQ867SPQQA43P2F",
    //            "zuf+tfteSlswRu7BJ86wekitnifILbZam1KYY3TG"
    //            ).WithSSL();

    //    // Create an async task for listing buckets.
    //    var getListBucketsTask = minio.ListBucketsAsync();

    //    // Iterate over the list of buckets.
    //    foreach (Bucket bucket in getListBucketsTask.Result.Buckets)
    //    {
    //        Console.WriteLine(bucket.Name + " " + bucket.CreationDateDateTime);
    //    }

    public class MinioObject
    {

        private static IMinioClient _minio;
        public class GetObjectReply
        {
            public ObjectStat objectstat { get; set; }
            public byte[] data { get; set; }
        }
        public class PutObjectRequest
        {
            public string bucket { get; set; }
            public byte[] data { get; set; }
        }
        public MinioObject()
        {
            _minio = new MinioClient()
                                     .WithEndpoint("localhost:9001")
                                     .WithCredentials("chulpan",
                                              "minio123")
                                     .WithSSL()//if Domain is SSL
                                     .Build();
        }
        public async Task<string> PutObj(PutObjectRequest request)
        {

            var bucketName = request.bucket;
            // Check Exists bucket
            var beArgs = new BucketExistsArgs()
                .WithBucket(bucketName);
            bool found = await _minio.BucketExistsAsync(beArgs).ConfigureAwait(false);

            if (!found)
            {
                // if bucket not Exists,make bucket
                await _minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
            }

            System.IO.MemoryStream filestream = new System.IO.MemoryStream(request.data);

            var filename = Guid.NewGuid();
            // upload object
            await _minio.PutObjectAsync(new PutObjectArgs()
                .WithBucket(bucketName).WithFileName(filename.ToString())
                .WithStreamData(filestream).WithObjectSize(filestream.Length)
                );

            return await Task.FromResult<string>(filename.ToString());
        }
        public async Task<GetObjectReply> GetObject(string bucket, string objectname)
        {


            MemoryStream destination = new MemoryStream();
            // Check Exists object
            var objstatreply = await _minio.StatObjectAsync(new StatObjectArgs()
                                          .WithBucket(bucket)
                                          .WithObject(objectname)
                                          );

            if (objstatreply == null || objstatreply.DeleteMarker)
                throw new Exception("object not found or Deleted");

            // Get object
            await _minio.GetObjectAsync(new GetObjectArgs()
                                        .WithBucket(bucket)
                                        .WithObject(objectname)
                                        .WithCallbackStream((stream) =>
                                        {
                                            stream.CopyTo(destination);
                                        }
                                        )
                                       );

            return await Task.FromResult<GetObjectReply>(new GetObjectReply()
            {
                data = destination.ToArray(),
                objectstat = objstatreply

            });


        }
        //private static async Task ListOfObjects(IMinioClient minio, string bucketName)
        //{
        //    string prefix = null;
        //    bool recursive = true;
        //    bool versions = false;

        //    try
        //    {
        //        // Just list of objects
        //        // Check whether 'mybucket' exists or not.
        //        var bktExistArgs = new BucketExistsArgs()
        //                .WithBucket(bucketName);
        //        var found = await minio.BucketExistsAsync(bktExistArgs).ConfigureAwait(false);
        //        if (found)
        //        {
        //            // List objects from 'my-bucketname'
        //            var args = new ListObjectArgs()
        //                                      .WithBucket(bucketName)
        //                                      .WithPrefix(prefix)
        //                                      .WithRecursive(true);
        //            IObservable<Item> observable = minio.ListObjectsAsync(args);
        //            IDisposable subscription = observable.Subscribe(
        //                    item => Console.WriteLine("OnNext: {0}", item.Key),
        //                    ex => Console.WriteLine("OnError: {0}", ex.Message),
        //                    () => Console.WriteLine("OnComplete: {0}"));
        //        }
        //        else
        //        {
        //            Console.WriteLine($"{bucketName} does not exist");
        //        }
        //    }
        //    catch (MinioException e)
        //    {
        //        Console.WriteLine("Error occurred: " + e);
        //    }
        //}

        //private static async Task ListOfBuckets(IMinioClient minio)
        //{
        //    var bucketName = "mybucket";
        //    try
        //    {
        //        var getListBucketsTask = await minio.ListBucketsAsync().ConfigureAwait(false);

        //        // Iterate over the list of buckets.
        //        foreach (var bucket in getListBucketsTask.Buckets)
        //        {
        //            Console.WriteLine(bucket.Name + " " + bucket.CreationDateDateTime);
        //        }

        //    }
        //    catch (MinioException e)
        //    {
        //        Console.WriteLine("Error occurred: " + e);
        //    }
        //}

    }
}
