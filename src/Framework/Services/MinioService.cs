using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Minio;

namespace Framework.Services
{
    public class MinioService : IMinioService
    {
        private readonly IOptions<S3Settings> _minioSettings;

        public MinioService(IOptions<S3Settings> minioSettings)
        {
            _minioSettings = minioSettings;
        }

        public MinioClient GetClient()
        {
            var query = new MinioClient()
                .WithEndpoint(_minioSettings.Value.Endpoint)
                .WithCredentials(_minioSettings.Value.AccessKey, _minioSettings.Value.SecretKey);

            if (_minioSettings.Value.WithSSL)
            {
                query = query.WithSSL();
            }

            return query
                .Build();
        }
    }
}