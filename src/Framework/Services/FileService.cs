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
                };

                var getObjectArgs = new Minio.GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(fileName)
                    .WithCallbackStream((stream) =>
                    {
                        stream.CopyTo(s3Object.Content);
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

        public async Task UploadFileAsync(S3Object s3object)
        {
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
                    .WithContentType(s3object.ContentType)
                    .WithStreamData(s3object.Content);

                await client.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}