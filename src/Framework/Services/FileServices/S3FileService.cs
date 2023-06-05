using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.S3;
using Amazon.S3;
using Microsoft.Extensions.Options;
using Data.Settings;
using Amazon.S3.Model;
using S3Object = Data.S3.S3Object;

namespace Framework.Services.FileServices
{
    public class S3FileService : IFileService
    {
        private readonly AmazonS3Client _client;
        private readonly IOptions<S3Settings> _s3Settings;

        public S3FileService(
            IOptions<S3Settings> s3Settings
        )
        {
            _s3Settings = s3Settings;
            _client = new AmazonS3Client(
                _s3Settings.Value.AccessKey,
                _s3Settings.Value.SecretKey,
                new AmazonS3Config
                {
                    ServiceURL = _s3Settings.Value.Endpoint,
                    ForcePathStyle = false
                }
            );
        }

        public async Task CreateBucketAsync(string bucketName)
        {
            var response = await _client.PutBucketAsync(bucketName);
        }

        public async Task DeleteFileAsync(string bucketName, string fileName)
        {
            var response = await _client.DeleteObjectAsync(bucketName, fileName);
        }

        public async Task<S3Object?> DownloadFileAsync(string bucketName, string fileName)
        {
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = fileName
            };

            var response = await _client.GetObjectAsync(request);

            return new S3Object
            {
                BucketName = bucketName,
                FileName = fileName,
                Content = response.ResponseStream,
                ContentType = response.Headers.ContentType
            };
        }

        public ICollection<S3ObjectStat> GetFilesInBucket(string bucketName)
        {
            ListObjectsRequest request = new ListObjectsRequest
            {
                BucketName = bucketName,
                MaxKeys = 1000
            };

            var response = _client.ListObjectsAsync(request).Result;

            return response.S3Objects.Select(x => new S3ObjectStat
            {
                BucketName = x.BucketName,
                FileName = x.Key,
            }).ToList();
        }

        public async Task UploadFileAsync(S3Object s3object)
        {
            PutObjectRequest request = new PutObjectRequest
            {
                BucketName = s3object.BucketName,
                Key = s3object.FileName,
                InputStream = s3object.Content,
                ContentType = s3object.ContentType
            };

            var response = await _client.PutObjectAsync(request);
        }
    }
}