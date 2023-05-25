using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.S3;

namespace Framework.Services
{
    public class FileService : IFileService
    {
        private readonly IMinioService _minioService;

        public FileService(IMinioService minioService)
        {
            _minioService = minioService;
        }

        public async Task CreateBucketAsync(string bucketName)
        {
            try
            {
                var client = _minioService.GetClient();

                // Make a bucket on the server, if not already present
                var beArgs = new Minio.BucketExistsArgs()
                    .WithBucket(bucketName);

                bool found = client.BucketExistsAsync(beArgs).Result;
                if (!found)
                {
                    var mbArgs = new Minio.MakeBucketArgs()
                        .WithBucket(bucketName);
                    await client.MakeBucketAsync(mbArgs);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteFileAsync(string bucketName, string fileName)
        {
            try
            {
                // check if bucket exists and if file exists
                var beArgs = new Minio.BucketExistsArgs()
                    .WithBucket(bucketName);

                if (!await _minioService.GetClient().BucketExistsAsync(beArgs))
                {
                    return;
                }

                var soArgs = new Minio.StatObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(fileName);

                if (await _minioService.GetClient().StatObjectAsync(soArgs) == null)
                {
                    return;
                }

                var client = _minioService.GetClient();

                var removeObjectArgs = new Minio.RemoveObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(fileName);

                await client.RemoveObjectAsync(removeObjectArgs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<S3Object?> DownloadFileAsync(string bucketName, string fileName)
        {
            try
            {
                // sync
                AutoResetEvent autoResetEvent = new AutoResetEvent(false);

                var client = _minioService.GetClient();

                // get stat object
                var soArgs = new Minio.StatObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(fileName);

                var statObject = await client.StatObjectAsync(soArgs).ConfigureAwait(false);

                if (statObject == null)
                {
                    return null;
                }

                S3Object s3Object = new S3Object()
                {
                    BucketName = bucketName,
                    FileName = fileName,
                    ContentType = statObject.ContentType,
                    Content = new MemoryStream()
                };

                var getObjectArgs = new Minio.GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(fileName)
                    .WithCallbackStream((stream) =>
                    {
                        stream.CopyTo(s3Object.Content);
                        // reset stream position
                        s3Object.Content.Position = 0;
                        autoResetEvent.Set();
                    });

                var stream = await client.GetObjectAsync(getObjectArgs).ConfigureAwait(false);

                autoResetEvent.WaitOne();

                return s3Object;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ICollection<S3ObjectStat>> GetFilesInBucketAsync(string bucketName)
        {
            try
            {
                // sync
                AutoResetEvent autoResetEvent = new AutoResetEvent(false);

                var client = _minioService.GetClient();

                var listObjectsArgs = new Minio.ListObjectsArgs()
                    .WithBucket(bucketName);

                var observable = client.ListObjectsAsync(listObjectsArgs);

                List<S3ObjectStat> objects = new List<S3ObjectStat>();

                IDisposable? subscription = observable.Subscribe(
                    item => objects.Add(new S3ObjectStat { BucketName = bucketName, FileName = item.Key }),
                    ex => throw ex,
                    () => autoResetEvent.Set());

                autoResetEvent.WaitOne();

                return objects;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UploadFileAsync(S3Object s3object)
        {
            var checkStructureREsult = CheckFileStructure(s3object);
            if (checkStructureREsult != null)
            {
                throw new Exception(checkStructureREsult);
            }

            try
            {
                var client = _minioService.GetClient();

                // Make a bucket on the server, if not already present
                var beArgs = new Minio.BucketExistsArgs()
                    .WithBucket(s3object.BucketName);

                bool found = await client.BucketExistsAsync(beArgs).ConfigureAwait(false);
                if (!found)
                {
                    var mbArgs = new Minio.MakeBucketArgs()
                        .WithBucket(s3object.BucketName);
                    await client.MakeBucketAsync(mbArgs).ConfigureAwait(false);
                }

                // Upload a file to bucket
                var putObjectArgs = new Minio.PutObjectArgs()
                    .WithBucket(s3object.BucketName)
                    .WithObject(s3object.FileName)
                    .WithObjectSize(s3object.Content.Length)
                    .WithContentType(s3object.ContentType)
                    .WithStreamData(s3object.Content);

                await client.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // File validation
        private string? CheckFileStructure(S3Object s3object)
        {
            /// Problem structure
            ///
            /// problem/
            /// ├─ README.md            * problem statement
            /// ├─ main.cpp             * main problem file
            /// ├─ solution_main.cpp    * main problem solution
            /// ├─ test0.in             * test input
            /// ├─ test1.in
            /// │  ...
            /// ├─ test0.ref            * test reference
            /// ├─ test1.ref
            /// │  ...
            /// 

            // check if file is in the structure

            // README.md
            if (s3object.FileName == "README.md")
            {
                return null;
            }

            // main and sol cpp / c
            if (s3object.FileName == "main.cpp" ||
                s3object.FileName == "solution_main.cpp")
            {
                return null;
            }

            // tests
            if (s3object.FileName.StartsWith("test") &&
                (s3object.FileName.EndsWith(".in") || s3object.FileName.EndsWith(".ref")))
            {
                return null;
            }

            return "File is not in the defined structure";
        }
    }
}